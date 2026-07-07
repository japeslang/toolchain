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


namespace japes.io {

    #region Interfaces

    public interface ITextChannel : IChannel {
        #region Properties

        public long Line { get; }

        public long Column { get; }

        #endregion Properties
    }

    public interface ITextSource : ISource<char>, ITextChannel {

    }

    public interface ITextSink : ISink<char>, ITextChannel { }

    #endregion Interfaces

    #region Classes

    public abstract class TextSource : Source<char>, ITextSource {

        #region Properties

        public long Line => throw new NotImplementedException();

        public long Column => throw new NotImplementedException();

        #endregion Properties

        #region Constructors

        public TextSource() : base() { }

        public TextSource(IBuffer<char> buffer)
            : base(buffer) { }

        #endregion Constructors

    }

    public class NETTextSource : TextSource {
        private System.IO.TextReader m_reader;
        private bool m_isOpen;

        #region Properties

        public override bool IsOpen => m_isOpen;

        #endregion Properties

        #region Constructors

        public NETTextSource(System.IO.TextReader reader) {
            this.m_reader = reader;
        }

        public NETTextSource(System.IO.TextReader reader,
            IBuffer<char> buffer) : base(buffer) {
            this.m_reader = reader;
        }

        public override void Dispose() {
            base.Dispose();
            m_reader?.Dispose();
        }

        #endregion Constructors

        #region I/O

        protected override bool OnCanRead()
            => m_isOpen;

        protected async override ValueTask<int> OnReadAsync(IBuffer<char> buffer, 
            CancellationToken token = default) {
            int remain = buffer.Remaining;
            int ret = 0;
            char[] buf = new char[remain];

            ret = await m_reader.ReadAsync(buf, token);
            await buffer.WriteAsync(buf, 0, ret, token);
            return ret;
        }

        #endregion I/O
    }


    #endregion Classes
}
