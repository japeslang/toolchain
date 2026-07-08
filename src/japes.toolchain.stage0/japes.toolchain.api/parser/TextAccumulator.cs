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

using System.Text;

namespace japes.toolchain.parser {


    public struct InlineTextAccumulator {
        private StringBuilder m_data;
        private string? m_name;
        private int m_depth;

        #region Constructors

        public InlineTextAccumulator(string? name = null) {
            this.m_data = new StringBuilder();
            this.m_name = name ?? "<anon>";
        }

        #endregion Constructors

        public void Enter() {
            if (m_depth == 0)
                m_data.Clear();
            m_depth++;
        }

        public void Append(string text)
            => m_data.Append(text);

        public void Leave() {
            /* */
            if (m_depth == 0) {
                throw new CompilerFatalException(
                    $"Text accumulator {m_name} cannot be ended because it is outside of a transaction. This is possibly a bug in the compiler. Please file a bug report ");
            }
        }
    }
}
