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

using System.Runtime.CompilerServices;

namespace japes.io {

    public record struct LineFilePosition {
        public readonly string? path;
        public readonly int line;

        public LineFilePosition(
            [CallerFilePath] string? path = null,
            int line = 0) {
            this.path = path;
            this.line = line;
        }
    }
}
