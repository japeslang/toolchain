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

using Antlr4.Runtime;
using japes.io;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace japes.toolchain.parser {


    public abstract class Lexer : Antlr4.Runtime.Lexer {
        private IBuffer<char> m_inputBuffer;
        private IBuffer<IToken> m_outputBuffer;

        InlineTextAccumulator m_docComment;

        #region ANTLR4 Interface

        #region Multiline Comments

        /* Here, we will define a common interface for *all* environments. 
         * The most restrictive
         */
        protected void LexEnterMLComment() {
            m_docComment.Enter();
        }

        protected void LexAppendMLComment(string text)
            => m_docComment.Append(text);

        protected void LexLeaveMLComment() {
            m_docComment.Leave();
        }

        #endregion Multiline Comments

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexCompilerBug(string? message = null, 
            Exception? innerException = null) {
            throw new CompilerBugException(Runtime.Caller(1), message, innerException);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexCompilerBug(Exception? innerException = null) {
            throw new CompilerBugException(Runtime.Caller(1), null, innerException);
        }

        #endregion ANTLR4 Interface



        #region Properties

        public bool CanWrite => throw new NotImplementedException();

        public bool CanRead => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        #endregion Properties

        #region Constructors

        protected Lexer(IBuffer<char> inputBuffer, IBuffer<IToken> outputBuffer) 
            : base(null) { }

        #endregion Constructors

        #region Flow Control

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void CompilerBug(string? message = null,
            Exception? innerException = null) {
            throw new CompilerBugException(Runtime.Caller(), message, innerException);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void CompilerBug(Exception? innerException = null) {
            throw new CompilerBugException(Runtime.Caller(), null, innerException);
        }

        protected void Error(string? message, Exception? innerException = null)
    => throw new CompilerErrorException(message, innerException);

        protected void Error(Exception? innerException = null)
            => throw new CompilerErrorException(innerException);

        protected void Fatal(string? message, Exception? innerException = null)
            => throw new CompilerFatalException(message, innerException);

        protected void Fatal(Exception? innerException = null)
            => throw new CompilerFatalException(innerException);

        #endregion Flow Control

        public ValueTask<IToken> ConsumeAsync(char input, CancellationToken token = default) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public IToken Peek() {
            throw new NotImplementedException();
        }

        public ValueTask<IToken> ReadAsync(CancellationToken token) {
            throw new NotImplementedException();
        }

        public ValueTask WriteAsync(char output, CancellationToken token = default) {
            throw new NotImplementedException();
        }
    }
}
