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
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using japes.io;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace japes.toolchain.parser {


    public abstract class BasicJapesLexer : Antlr4.Runtime.Lexer {
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

        protected void LexAppendMLCommentText()
            => m_docComment.Append(this.Text);

        protected bool LexLeaveMLComment() {
            m_docComment.Leave();
            return m_docComment.Depth == 0;
        }

        protected void LexConsolidateMLComment(int tokenType, int channel,
            string description, string expected) {

            try {
                if (LexLeaveMLComment()) {
                    this.Text = this.m_docComment.ToString();
                    this.Type = tokenType;
                    this.Channel = channel;
                    this.LexModePop();
                }
                else {
                    LexAppendMLCommentText();
                }
            }
            catch (CompilerFatalException exception) {
                LexFatal("Cannot consolidate {0}, probably because a terminal \"{1}\" is missing.",
                    description, expected);
            }
        }

        #endregion Multiline Comments

        #region Error reporting

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexCompilerBug(string message, params object?[] args) {
            throw new CompilerBugException(Runtime.Caller(),
                String.Format(message, args));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexBanter(string message, params object?[] args) {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexElaborate(string message, params object?[] args) {
            
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexWarn(string message, params object?[] args) {
;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexError(string message, params object?[] args) {
            throw new CompilerErrorException(String.Format(message, args));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        protected void LexFatal(string message, params object?[] args) {
            throw new CompilerFatalException(String.Format(message, args));
        }

        #endregion Error Reporting

        #region String Maniplation

        protected string LexStringLeft(string @string, int length)
            => @string.Left(length);

        protected string LexStringRight(string @string, int length)
            => @string.Right(length);

        #endregion String Manipulation

        #region Token Manipulation

        protected string LexText()
            => this.Text;

        protected string LexTextSet(string value, params object?[] args) {
            if ((args?.Length ?? 0) > 0) {
                value = String.Format(value, args!);
            }
            return this.Text = value;
        }

        protected string LexTextLeft(int length)
            => LexStringLeft(this.Text, length);

        protected string LexTextRight(int length)
            => LexStringRight(this.Text, length);

        protected int LexTokenChannel()
            => this.Channel;

        protected void LexTokenSetChannel(int channel)
            => this.Channel = channel;

        #endregion Token Manipulation

        #region Mode Settings

        protected void LexModePush(int mode)
            => this.PushMode(mode);

        protected int LexModePop()
            => this.PopMode();

        #endregion Mode Settings

        #endregion ANTLR4 Interface

        #region Constants

        #region Channels

        public const int CHANNEL_DEFAULT = 0;

        public const int CHANNEL_TRIVIA = 1;

        public const int CHANNEL_DOCCOMMENTS = 2;

        #endregion Channels

        #region Modes

        public const int MODE_DOCCOMMENT_ML = 1;

        public const int MODE_COMMENT_ML = 2;

        public const int MODE_DOCCOMMENT_MACRO = 3;

        public const int MODE_COMMENT_MACRO = 4;

        public const int MODE_VERBATIM_BLOCK = 5;

        public const int MODE_STRING_FORMAT = 6;

        public const int MODE_STRING_FORMAT_VERBATIM = 7;

        #endregion Mode

        #endregion Constants

        #region Properties

        public bool CanWrite => throw new NotImplementedException();

        public bool CanRead => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        #endregion Properties

        #region Constructors

        protected BasicJapesLexer(IBuffer<char> inputBuffer, IBuffer<IToken> outputBuffer) 
            : base(null) { }

        protected BasicJapesLexer(ICharStream stdin, TextWriter stdout, TextWriter stderr) 
            : base(stdin, stdout, stderr) {

        }

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
