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

namespace japes.toolchain {
    public static class TextUtils {

        #region String Extensions

        public static string Left(this string @string, int length) {
            int strlen = @string.Length;
            if (strlen <= length || -strlen >= length)
                return @string;
            else if (length >= 0) {
                return @string.Substring(0, length);
            }
            else {
                if (strlen <= -length)
                    return @string;
                return @string.Substring(0, strlen + length);
            }
        }

        public static string Right(this string @string, int length) {
            int strlen = @string.Length;
            if (strlen <= length || -strlen >= length)
                return @string;
            else if (length >= 0) {
                return @string.Substring(strlen - length, length);
            }
            else {
                return @string.Substring(-length);
            }
        }

        #endregion String Extensions
    }
}
