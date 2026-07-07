using japes.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace japes.io {

    public interface IBuffer<T> : IEncoder<T, T> {
        #region Properties
        public int Capacity { get; }

        public int Available { get; }

        public int Remaining
            => Capacity - Available;

        bool IChannel.IsOpen
            => true;

        bool ISource.CanRead
            => Available > 0;

        bool ISink.CanWrite
            => Available < Capacity;

        #endregion Properties

        #region I/O

        public new ValueTask<T> ConsumeAsync(T input, CancellationToken token)
            => new ValueTask<T>(this.Consume(input));

        ValueTask<T> IEncoder<T, T>.ConsumeAsync(T input, CancellationToken token)
            => ConsumeAsync(input, token);

        public new T Consume(T input) {
            /* We have something in the buffer, read it in the hope that we 
             * will make room. 
             */
            if (this.CanRead) {
                T ret = this.Read();
                this.Write(input);
                return ret;
            }
            /* Otherwise, this is a pass-through */
            else {
                return input;
            }
        }

        T IEncoder<T, T>.Consume(T input)
            => Consume(input);

        public new ValueTask<T> ReadAsync(CancellationToken token)
            => new ValueTask<T>(Read());

        ValueTask<T> ISource<T>.ReadAsync(CancellationToken token)
            => ReadAsync(token);

        public new T Read();

        T ISource<T>.Read()
            => Read();

        public new ValueTask WriteAsync(T input, CancellationToken token) {
            this.Write(input);
            return ValueTask.CompletedTask;
        }


        ValueTask ISink<T>.WriteAsync(T output, CancellationToken token)
            => WriteAsync(output, token);

        public new void Write(T input);

        void ISink<T>.Write(T o)
            => Write(o);


        #endregion I/O
    }

    public struct SingleItemBuffer<T> : IBuffer<T> {
        private T m_value;
        private bool m_valid;

        public int Capacity => 1;

        public int Available => m_valid ? 1 : 0;

        public void Dispose() { }

        public T Peek() {
            if (m_valid)
                return m_value;
            throw new IOException("Nothing to peek at.");
        }

        public T Read() {
            if (m_valid) {
                T ret = m_value;
                /* Clear out what we got so we don't leak state */
                m_value = default!; /* Clear out what we got sowe don't */
                m_valid = false;
                return ret;
            }
            else {
                throw new IOException("Buffer underrun.");
            }
        }


        public void Write(T input) {
            if (m_valid)
                throw new IOException("Buffer full.");
            m_value = input;
            m_valid = true;
        }
    }
}
