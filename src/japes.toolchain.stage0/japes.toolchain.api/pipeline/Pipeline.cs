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

/* WARNING: ALL MEMBERS IN THIS API ARE SUBJECT TO MAJOR REVISION UNTIL BOOTSTRAPPED. */

using japes.io;
using System.Runtime.CompilerServices;

namespace japes.toolchain.pipeline {

    public abstract class Pipeline : IEncoder {
        #region Classes

        public abstract class Stage {

            private Pipeline? m_parent;

            #region Properties

            protected Pipeline Parent
                => m_parent!;

            public Type InputType
                => Unsafe.As<ISource>(this).InputType;

            public Type OutputType
                => Unsafe.As<ISink>(this).OutputType;

            #endregion Properties

            #region Constructors

            internal Stage() { }

            internal void M_Imbue(Pipeline pipeline) {
                if (m_parent != null)
                    throw new InvalidOperationException("Pipeline stage already imbued.");

                m_parent = pipeline;
            }

            #endregion Constructors

            #region I/O

            public abstract ValueTask SinkAsync(ISink sink,
                CancellationToken token = default);

            public abstract ValueTask SourceAsync(ISource source,
                CancellationToken token = default);

            public object? Peek()
                => M_Peek();

            internal abstract object? M_Peek();

            #endregion I/O
        }

        public abstract class Stage<I, O> : Stage, japes.io.IEncoder<I, O> {
            private IBuffer<I> m_inputBuffer;
            private IBuffer<O> m_outputBuffer;

            #region Properties

            public bool CanWrite => m_inputBuffer.CanWrite;

            public bool CanRead => m_outputBuffer.CanRead;

            public bool IsOpen => true;

            #endregion Properties

            #region Constructors

            public Stage() : base() {
                this.m_inputBuffer = new SingleItemBuffer<I>();
                this.m_outputBuffer = new SingleItemBuffer<O>();
            }

            public Stage(IBuffer<I> inputBuffer, IBuffer<O> outputBuffer) {
                this.m_inputBuffer = inputBuffer;
                this.m_outputBuffer = outputBuffer;
            }

            public virtual void Dispose() { }

            #endregion Constructors

            protected abstract ValueTask<O> OnConsumeAsync(I input, CancellationToken token);

            public virtual async ValueTask<O> ConsumeAsync(I input, CancellationToken token) {
                input = await m_inputBuffer.ConsumeAsync(input, token);
                O result = await OnConsumeAsync(input, token);
                return await m_outputBuffer.ConsumeAsync(result, token);
            }



            public new O Peek()
                => m_outputBuffer.Peek();

            internal override object? M_Peek()
                => m_outputBuffer.Peek();

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

            public virtual ValueTask SinkAsync(ISink<O> sink,
                CancellationToken token = default)
                => m_outputBuffer.SinkAsync(sink, token);


            public override ValueTask SinkAsync(ISink sink, CancellationToken token = default) {
                if (sink is ISink<O>) {
                    return SinkAsync((ISink<O>)sink, token);
                }
                else {
                    throw new InvalidOperationException("Sink is incompatible with this stage.");
                }
            }


            public ValueTask WriteAsync(I input, CancellationToken token)
                => m_inputBuffer.WriteAsync(input, token);

            public virtual ValueTask SourceAsync(ISource<I> source,
                CancellationToken token = default)
                => m_inputBuffer.SourceAsync(source);

            public override ValueTask SourceAsync(ISource source, CancellationToken token = default) {
                if (source is ISource<I>) {
                    return SourceAsync((ISource<I>)source, token);
                }
                else {
                    throw new InvalidOperationException("Source is incompatible with this stage.");
                }
            }

        }

        #endregion Classes

        #region Properties

        public abstract IEnumerable<Stage> Stages { get; }

        public abstract Type InputType { get; }

        public abstract Type OutputType { get; }

        public abstract bool CanRead { get; }

        public bool CanWrite { get; }

        public bool IsOpen => true;

        #endregion Properties

        #region Constructors

        protected Pipeline() : base() { }

        #endregion Constructors

        #region I/O

        public abstract ValueTask ConsumeAsync(ISource source, ISink sink,
            CancellationToken token = default);

        public virtual void Consume(ISource source, ISink sink)
            => ConsumeAsync(source, sink).GetAwaiter().GetResult();

        #endregion I/O

        #region Other Operations

        public abstract void Add(Stage stage);

        public ValueTask<object?> ConsumeAsync(object? input, CancellationToken token = default) {
            throw new NotImplementedException();
        }

        public ValueTask<object?> ReadAsync(CancellationToken token) {
            throw new NotImplementedException();
        }

        public abstract object? Peek();

        public ValueTask WriteAsync(object? output, CancellationToken token = default) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        #endregion Other Operations
    }
}
