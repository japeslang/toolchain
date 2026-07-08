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

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace japes.toolchain {
    public class CompilerException : Exception {

        #region Classes

        public enum Severity {
            WARNING,
            ERROR,
            FATAL
        }

        #endregion Classes

        #region Properties

        protected Type EffectiveName
            => this.GetType();

        public virtual Severity MessageSeverity
            => Severity.ERROR;

        #endregion Properties

        internal static string S_CompilerBug(string message, StackFramePosition pos)
            => $"{message} :: This is probably an oversight in the compiler implementation. Please file a bug report at https://github.com/japeslang/toolchain, and include the stack trace if available. The caller was {pos}.";

        internal static string S_CompilerBug(string message, string? callerName,
            string? fileName, int callerLine)
            => S_CompilerBug(message, new StackFramePosition(callerName, fileName, callerLine));


        internal CompilerException(StackFramePosition pos,
            string? message = null, Exception? innerException = null)
            : base(message ?? S_CompilerBug($"Somebody was naughty and did not set this string.",
                    pos), innerException) { }

        internal CompilerException(string? message = null, Exception? innerException = null,
            [CallerMemberName] string? callerName = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int callerLine = 0)
            : this(new StackFramePosition(callerName,
                file, callerLine), message, innerException) { }

        internal CompilerException(Exception? innerException, 
            [CallerMemberName] string? callerName = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int callerLine = 0) :
            this(null, innerException, callerName, file, callerLine)  { }
    }

    public class CompilerBugException : CompilerException {

        #region Constructors

        internal CompilerBugException(StackFramePosition pos, 
            string? message = null, Exception? innerException = null)
            : base(S_CompilerBug(message ?? "An unspecified compiler bug occurred.",
                pos)) { }

        internal CompilerBugException(StackFrame? pos,
            string? message = null, Exception? innerException = null)
            : this(new StackFramePosition(pos), message, innerException) { }

        internal CompilerBugException(string? message = null, 
            Exception? innerException = null,
            [CallerMemberName] string? callerName = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int callerLine = 0)
            : this(new StackFramePosition(callerName,
                file, callerLine), message, innerException) { }

        #endregion Constructors
    }

    public class CompilerIOException : CompilerException {
        #region Constructors

        public CompilerIOException(string? message = null,
            Exception? innerException = null)
            : base(message ?? "An unspecified I/O exception occurred.") { }

        public CompilerIOException(Exception? innerException)
            : this(null, innerException) { }

        #endregion Constructors
    }

    public class CompilerErrorException : CompilerException {
        public CompilerErrorException(string? message = null,
            Exception? innerException = null)
            : base(message ?? "An unspecified recoverable error occurred.") { }

        public CompilerErrorException(Exception? innerException)
            : this(null, innerException) { }
    }

    public class CompilerFatalException : CompilerException {
        public CompilerFatalException(string? message = null,
            Exception? innerException = null)
            : base(message ?? "An unspecified fatal error occurred.") { }

        public CompilerFatalException(Exception? innerException)
            : this(null, innerException) { }
    }
}
