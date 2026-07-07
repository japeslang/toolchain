/*
 This file is part of the Japes Programming Language.

 Japes is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as
 published by the Free Software Foundation, either version 3 of
 the License, or (at your option) any later version.

 Japes is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public
 License along with Japes.  If not, see <http://www.gnu.org/licenses/>.
*/

using japes.io;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace japes.io {

    #region Interfaces

    #region Base

    /// <summary>
    /// An interface that can convert two objects.
    /// </summary>
    /// <remarks>
    /// <para>This interface represents a stereotypical encoding stream. 
    /// </para>
    /// </remarks>
    public interface IEncoder : ISource, ISink {

        #region @default

        public static class @default {
            public static object? __defaultConsume(IEncoder @this, object? input)
                => @this.ConsumeAsync(input).Result;
        }

        #endregion @default

        #region Properties

        public Type OutputType
            => typeof(object);

        #endregion Properties

        public ValueTask<object?> ConsumeAsync(object? input, 
            CancellationToken token = default);

        public object? Consume(object? input)
            => @default.__defaultConsume(this, input);

        #region I/O
        public IEncoder Bind(IEncoder dest)
            => new BindPairEncoder(this, dest);

        #endregion I/O
    }

    /// <summary>
    /// A refinement of <c>IEncoder</c> that specifies its bounds.
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public interface IEncoder<I, O> : IEncoder, ISink<I>, ISource<O> {

        #region @default

        public static class @default {
            public static O __defaultConsume(IEncoder<I,O> @this, I input)
                => @this.ConsumeAsync(input).Result;
        }

        #endregion @default

        #region Properties

        Type IEncoder.OutputType
            => typeof(O);

        #endregion Properties

        #region I/O

        /// <inheritdoc cref="IEncoder.ConsumeAsync(object?, CancellationToken)"/>
        public ValueTask<O> ConsumeAsync(I input, 
            CancellationToken token = default);

        async ValueTask<object?> IEncoder.ConsumeAsync(object? input, CancellationToken token)
            => await ConsumeAsync((I)input!, token);

        /// <inheritdoc cref="IEncoder.Consume(object?)"/>
        public O Consume(I input)
            => @default.__defaultConsume(this, input);

        object? IEncoder.Consume(object? input) 
            => Consume((I) input!);

        public IEncoder<I,O2> Bind<O2>(IEncoder<O, O2> dest)
            => new BindPairEncoder<I, O, O2>(this, dest);


        #endregion I/O


    }

    public interface IEncodable<I,O> {
        public IEncoder<I, O> GetEncoder();
    }

    public interface IDecodable<I,O> {
        public IEncoder<O, I> GetEncoder(O output);
    }

    public interface ICodec<I,O> : IEncodable<I,O>, IDecodable<I,O> { }

    #endregion Base


    #endregion Interfaces

    #region Fluency Classes

    #region Bind

    public readonly struct BindPairEncoder : IEncoder {
        private readonly IEncoder m_src;
        private readonly IEncoder m_dest;
        private readonly IBuffer<object?> m_buf;

        #region Properties

        public bool IsOpen
            => m_src.IsOpen && m_dest.IsOpen;

        public bool CanRead
            => m_buf.CanWrite && m_dest.CanRead;

        public bool CanWrite
            => m_src.CanWrite && m_buf.CanRead;

        #endregion Properties

        #region Construtors

        public BindPairEncoder(IEncoder src, IEncoder dest) {
            this.m_src = src;
            this.m_dest = dest;
            this.m_buf = new SingleItemBuffer<object?>();
        }

        #endregion Constructors

        #region Operators

        public async ValueTask<object?> ConsumeAsync(object? input, 
            CancellationToken token) 
        {
            input = await m_src.ConsumeAsync(input, token);
            object? ret = await m_src.ConsumeAsync(input, token);
            return m_dest.ConsumeAsync(ret, token);
        }

        public object? Consume(object? input) {
            input = m_buf.Consume(input);
            object? ret = m_src.Consume(input);
            return m_dest.Consume(ret);
        }
        
        public void Dispose() {
            m_src?.Dispose();
            m_dest?.Dispose();
        }

        public async ValueTask<object?> ReadAsync(CancellationToken token) {
            object? input = await m_buf.ReadAsync(token);
            object? ret = await m_src.ConsumeAsync(input, token);
            return m_dest.Consume(ret);
        }

        public object? Peek()
            => m_buf.Peek();

        public ValueTask WriteAsync(object? output, CancellationToken token) {
            throw new NotImplementedException();
        }


        #endregion Operators
    }

    public readonly struct BindPairEncoder<I, M, O> : IEncoder<I, O> {
        private readonly IEncoder<I, M> m_src;
        private readonly IEncoder<M, O> m_dest;
        private readonly IBuffer<I> m_buf;

        #region Properties

        public bool IsOpen
            => m_src.IsOpen && m_dest.IsOpen;

        public bool CanWrite => m_dest.CanWrite && m_buf.CanRead;

        public bool CanRead => throw new NotImplementedException();

        #endregion Properties

        #region Construtors

        public BindPairEncoder(IEncoder<I, M> src, IEncoder<M, O> dest) {
            this.m_src = src;
            this.m_dest = dest;
        }

        #endregion Constructors

        #region Operators

        public O Consume(I input)
            => m_dest.Consume(m_src.Consume(input));

        public ValueTask<O> ConsumeAsync(I input, CancellationToken token) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public O Peek() {
            throw new NotImplementedException();
        }

        public ValueTask<O> ReadAsync(CancellationToken token) {
            throw new NotImplementedException();
        }

        public ValueTask WriteAsync(I output, CancellationToken token) {
            throw new NotImplementedException();
        }


        #endregion Operators
    }

    #endregion Bind

    #endregion Fluency Classes

    #region Other Classes

    public abstract class Encoder<I, O> : IEncoder<I, O> {
        private IBuffer<I> m_inputBuffer;
        private IBuffer<O> m_outputBuffer;

        #region Properties

        public bool CanWrite
            => m_inputBuffer.CanWrite;

        public bool CanRead
            => m_outputBuffer.CanRead;

        public virtual bool IsOpen => true;

        #endregion Properties

        #region Constructor

        public Encoder() {
            this.m_inputBuffer = new SingleItemBuffer<I>();
            this.m_outputBuffer = new SingleItemBuffer<O>();
        }

        public Encoder(IBuffer<I> inputBuffer, IBuffer<O> outputBuffer) {
            this.m_inputBuffer = inputBuffer;
            this.m_outputBuffer = outputBuffer;
        }

        #endregion Constructor

        #region Encoder

        protected abstract ValueTask<O> OnConsumeAsync(I input, CancellationToken token);

        public virtual async ValueTask<O> ConsumeAsync(I input, CancellationToken token) {
            input = await m_inputBuffer.ConsumeAsync(input, token);
            O result = await OnConsumeAsync(input, token); 
            return await m_outputBuffer.ConsumeAsync(result, token);
        }

        public async ValueTask<O> ReadAsync(CancellationToken token) {
            if (!m_outputBuffer.CanWrite)
                throw new IOException("Output buffer full.");
            else if (!m_inputBuffer.CanRead)
                throw new IOException("Buffer underflow.");

            I input = m_inputBuffer.Peek();
            O output = await OnConsumeAsync(input, token);

            m_inputBuffer.Read();
            return await m_outputBuffer.ConsumeAsync(output, token);
        }

        public async ValueTask WriteAsync(I input, CancellationToken token) {
            m_inputBuffer.Write(input);
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public O Peek()
            => m_outputBuffer.Peek();

        #endregion Encoder
    }
    #endregion Other Classes

    public static class EncoderUtils {

    }
}
