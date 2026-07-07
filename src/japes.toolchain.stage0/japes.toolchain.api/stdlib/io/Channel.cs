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

using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace japes.io {

    #region Interfaces

    /// <summary>
    /// An interface representing a channel.
    /// </summary>
    /// <remarks>
    /// <para>A channel is  
    /// </para>
    /// </remarks>
    public interface IChannel : IDisposable {
        #region Properties

        /// <summary>
        /// Whether or not the stream is open for reading.
        /// </summary>
        public bool IsOpen { get; }

        #endregion Properties

    }


    /// <summary>
    /// A refinement of <c>IChannel</c> that can be read.
    /// </summary>
    public interface ISource : IChannel, System.Collections.IEnumerable {
        #region @default

        public static class @default {
            public static bool __defaultTryRead(ISource @this, out object? dest) {
                try {
                    dest = @this.Read();
                    return true;
                }
                catch (Exception) {
                    dest = null;
                    return false;
                }
            }

            public static object? __defaultRead(ISource @this) {
                return @this.ReadAsync(new CancellationToken()).Result;
            }

            public static async ValueTask __defaultSinkAsync(ISource @this, ISink sink,
                CancellationToken token = default) {
                while (@this.CanRead) {
                    await sink.WriteAsync(await @this.ReadAsync(token), token);
                }
            }

            public static void __defaultSink(ISource @this, ISink sink)
                => @this.SinkAsync(sink).GetAwaiter().GetResult();

            public static System.Collections.IEnumerator __defaultGetEnumerator(ISource @this) {
                yield return @this.Read();
            }
        }

        #endregion @default

        #region Properties

        public bool CanRead { get; }

        public Type InputType
            => typeof(object);

        #endregion Properties

        #region I/O

        public bool TryRead(out object? dest)
            => @default.__defaultTryRead(this, out dest);

        public object? Read()
            => @default.__defaultRead(this);

        public void Sink(ISink sink)
            => @default.__defaultSink(this, sink);

        public ValueTask<object?> ReadAsync(CancellationToken token);

        public ValueTask SinkAsync(ISink sink,
            CancellationToken token = default)
            => @default.__defaultSinkAsync(this, sink, token);

        /* Enumeration is equivalent to constantly reading, so we'll */

        IEnumerator IEnumerable.GetEnumerator()
            => @default.__defaultGetEnumerator(this);

        public object? Peek();


        #endregion I/O


    }

    public interface ISource<I> : ISource, IEnumerable<I> {

        #region @default

        public new static class @default {
            public static bool __defaultTryRead(ISource<I> @this, out I dest) {
                try {
                    dest = @this.Read();
                    return true;
                }
                catch (Exception) {
                    dest = default!;
                    return false;
                }
            }

            public static I __defaultRead(ISource<I> @this) {
                return @this.ReadAsync(new CancellationToken()).Result;
            }

            public static async ValueTask __defaultSinkAsync(ISource<I> @this, ISink<I> sink,
                CancellationToken token = default) {
                while (@this.CanRead) {
                    await sink.WriteAsync(await @this.ReadAsync(token), token);
                }
            }

            public static void __defaultSink(ISource<I> @this, ISink<I> sink)
                => @this.SinkAsync(sink).GetAwaiter().GetResult();

            public static System.Collections.Generic.IEnumerator<I> __defaultGetEnumerator(ISource<I> src) {
                yield return src.Read();
            }

            public static async ValueTask<int> __defaultReadAsync(ISource<I> @this, 
                I[] buffer, int offset, int length, CancellationToken token) {
                int ret = 0;
                while (length > 0 && @this.CanRead) {
                    buffer[offset] = await @this.ReadAsync(token);
                    length--;
                    offset++;
                    ret++;
                }

                return ret;
            }
        }

        #endregion @default

        #region Properties

        Type ISource.InputType
            => typeof(I);

        #endregion Properties

        #region I/O

        public bool TryRead(out I dest)
            => @default.__defaultTryRead(this, out dest);

        /// <inheritdoc cref="ISource.Read()"/>
        public new I Read()
            => ReadAsync(new CancellationToken()).Result;

        object? ISource.Read()
            => Read();

        public void Sink(ISink<I> sink)
            => @default.__defaultSink(this, sink);

        public new ValueTask<I> ReadAsync(CancellationToken token);

        async ValueTask<object?> ISource.ReadAsync(CancellationToken token)
            => await ReadAsync(token);

        public ValueTask SinkAsync(ISink<I> sink, CancellationToken token)
            => @default.__defaultSinkAsync(this, sink, token);

        public ValueTask<int> ReadAsync(I[] buffer, int offset, int length,
            CancellationToken token)
            => @default.__defaultReadAsync(this, buffer, offset, length,
                token);

        IEnumerator<I> IEnumerable<I>.GetEnumerator()
            => @default.__defaultGetEnumerator(this);

        public new I Peek();

        object? ISource.Peek()
            => Peek();


        #endregion I/O
    }

    public interface ISink : IChannel {

        #region @default

        public static class @default {
            public static bool __defaultTryWrite(ISink @this, object? output) {
                try {
                    @this.Write(output);
                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }

            public static void __defaultWrite(ISink @this, object? output) {
                @this.WriteAsync(output, new CancellationToken()).
                    GetAwaiter().GetResult();
            }

            public static ValueTask __defaultSourceAsync(ISink @this, ISource source,
                CancellationToken token = default)
                => source.SinkAsync(@this, token);

            public static void __defaultSource(ISink @this, ISource source)
                => @this.SourceAsync(source).GetAwaiter().GetResult();
        }

        #endregion @default

        #region Properties

        public bool CanWrite { get; }

        public Type OutputType
            => typeof(object);

        #endregion Properties

        public bool TryWrite(object? output)
            => @default.__defaultTryWrite(this, output);

        public void Write(object? output)
            => @default.__defaultWrite(this, output);

        public void Source(ISource source)
            => @default.__defaultSource(this, source);

        public ValueTask WriteAsync(object? output, 
            CancellationToken token = default);

        public ValueTask SourceAsync(ISource source,
            CancellationToken token = default)
            => @default.__defaultSourceAsync(this, source, token);
    }

    public interface ISink<O> : ISink {

        #region @default

        public static new class @default {
            public static bool __defaultTryWrite(ISink @this, O output) {
                try {
                    @this.Write(output);
                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }

            public static void __defaultWrite(ISink @this, O output) {
                @this.WriteAsync(output, new CancellationToken()).
                    GetAwaiter().GetResult();
            }

            public static async ValueTask<int> __defaultWriteAsync(ISink<O> @this,
                O[] buffer, int offset, int length, CancellationToken token) {
                int ret = 0;
                while (length > 0 && @this.CanWrite) {
                    await @this.WriteAsync(buffer[offset], token);
                    length--;
                    offset++;
                    ret++;
                }

                return ret;
            }
        }

        #endregion @default

        #region Properties

        Type ISink.OutputType
            => typeof(O);

        #endregion Properties

        public bool TryWrite(O output)
            => @default.__defaultTryWrite(this, output);

        public void Write(O output)
            => @default.__defaultWrite(this, output);

        void ISink.Write(object? output)
            => Write((O)output!);

        public ValueTask WriteAsync(O output, 
            CancellationToken token = default);

        ValueTask ISink.WriteAsync(object? output, 
            CancellationToken token)
            => WriteAsync((O)output!, token);

        public ValueTask<int> WriteAsync(O[] buffer, int offset, int length,
            CancellationToken token = default)
            => @default.__defaultWriteAsync(this, buffer, offset, length,
                token);
    }

    #endregion Interfaces

    #region Classes

    public class SourceEnumerator : IEnumerator {
        internal ISource m_source;
        internal object? m_datum;

        #region Properties

        public object? Current
            => m_datum;

        #endregion Properties

        #region Constructors

        public SourceEnumerator(ISource source) {
            this.m_source = source;
            this.m_datum = null;
        }

        #endregion Constructors

        public void Dispose() { }

        public bool MoveNext() {
            if (!m_source.CanRead) {
                return false; 
            }

            m_datum = m_source.Read();
            return true;
        }

        public void Reset() {
            throw new NotImplementedException("Not possible.");
        }
    }

    public abstract class Source<I> : ISource<I> {
        private IBuffer<I> m_buffer;

        #region Properties
        public bool CanRead
            => m_buffer.CanRead && OnCanRead();

        public abstract bool IsOpen { get; }



        #endregion Properties

        #region Constructors

        public Source() {
            this.m_buffer = new SingleItemBuffer<I>();
        }

        public Source(IBuffer<I> buffer) {
            this.m_buffer = buffer;
        }

        public virtual void Dispose() { }

        #endregion Constructors

        #region I/O

        protected abstract bool OnCanRead();

        protected abstract ValueTask<int> OnReadAsync(IBuffer<I> buffer,
            CancellationToken token);

        public I Peek()
            => m_buffer.Peek();

        public async ValueTask<I> ReadAsync(CancellationToken token) {
            if (!m_buffer.CanRead) {
                await OnReadAsync(m_buffer, token);
            }
            return await m_buffer.ReadAsync(token);
        }

        public virtual bool TryRead(out I dest)
            => ISource<I>.@default.__defaultTryRead(this, out dest);

        public virtual I Read()
            => ISource<I>.@default.__defaultRead(this);

        #endregion I/P
    }

    public abstract class Sink<O> : ISink<O> {
        private IBuffer<O> m_buffer;

        #region Properties
        public bool CanWrite => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        protected IBuffer<O> Buffer
            => m_buffer;

        #endregion Properties

        #region Constructors

        public Sink() {
            this.m_buffer = new SingleItemBuffer<O>();
        }

        public Sink(IBuffer<O> buffer) {
            this.m_buffer = buffer;
        }

        public virtual void Dispose() { }

        #endregion Constructors

        #region I/O

        protected abstract ValueTask OnWriteASync(O output, IBuffer<O> buffer,
            CancellationToken token);

        public abstract ValueTask WriteAsync(O output, CancellationToken token);

        public bool TryWrite(O output)
            => ISink<O>.@default.__defaultTryWrite(this, output);

        public void Write(O output)
            => ISink<O>.@default.__defaultWrite(this, output);

        #endregion I/P
    }

    #endregion Classes


}
