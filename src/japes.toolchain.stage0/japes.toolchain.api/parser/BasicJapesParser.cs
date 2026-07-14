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

namespace japes.toolchain.parser {
    public abstract  class BasicJapesParser : Parser {

        #region ANTLR4 Interface

        public bool LangTextIn(IToken src, params string[] candidates) {

            string text = src.Text;
            foreach (string candidate in candidates) {
                if (text.Equals(candidate))
                    return true;
            }

            return false;
        }

        #endregion ANTLR4 Interface

        #region Constructors

        protected BasicJapesParser(IBuffer<char> inputBuffer, IBuffer<IToken> outputBuffer)
            : base(null) { }

        protected BasicJapesParser(ITokenStream stdin, TextWriter stdout, TextWriter stderr)
            : base(stdin, stdout, stderr) {

        }

        #endregion Constructors
    }
}
