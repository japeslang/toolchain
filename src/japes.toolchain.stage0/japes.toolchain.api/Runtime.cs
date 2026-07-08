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
using System.Text;

namespace japes.toolchain {

    public readonly record struct StackFramePosition {
        public readonly string? name;
        public readonly string? path;
        public readonly int line;
        public readonly int col;

        #region Constructors
        public StackFramePosition(
            [CallerMemberName] string? name = null, 
            [CallerFilePath] string? path = null,
            [CallerLineNumber] int line = int.MaxValue,
            int col = int.MaxValue) {
            this.name = name;
            this.path = path;
            this.line = line;
            this.col = col;
        }

        public StackFramePosition(StackFrame? frame) {
            if (frame == null) {
                return;
            }
            else {
                this.name = frame.GetMethod()?.Name;
                this.path = frame.GetFileName();
                this.line = frame.GetFileLineNumber()-1;
                this.col = frame.GetFileColumnNumber()-1;
            }
        }

        [StackTraceHidden]
        public static StackFramePosition FromCaller()
            => new StackFramePosition(Runtime.Caller());

        #endregion Constructors

        #region Miscellaneous

        public void ToString(StringBuilder sb) {
            sb.Append(name ?? "???");
            sb.Append('@');
            sb.Append(path ?? "???");
            if (line != int.MaxValue) {
                sb.Append(':');
                sb.Append(line);

                if (col != int.MaxValue) {
                    sb.Append(':');
                    sb.Append(col);
                }
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }

        #endregion Miscellaneous
    }

    public static class Runtime {


        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]        
        public static StackTrace StackTrace(int offset = 0, bool needFileInfo = true) {
            return new StackTrace(2 + offset, needFileInfo);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        public static StackFrame? Caller(int offset = 0)
            => StackTrace().GetFrame(offset);

        [MethodImpl(MethodImplOptions.NoInlining)]
        [StackTraceHidden]
        internal static StackFramePosition CallerPosition(int offset = 0)
            => new StackFramePosition(StackTrace().GetFrame(offset));

    }
}
