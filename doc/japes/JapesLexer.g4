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

lexer grammar JapesLexer;

/* Primary Expressions */

ID : [A-Za-z_][0-9A-Za-z_]*
ID_AT : '@' [A-Za-z_][0-9A-Za-z_]*;

DECIMAL_INTEGER : 0 | [+-]?[1-9][0-9]*;
DECIMAL_REAL : [0-9]*[.][0-9]+([Ee][+-]?[0-9]+);
HEX_INTEGER : '0x' [0-9ABCDEFabcdef_]*[0-9ABCDEFabcdef];
OCTAL_INTEGER : '0' [0-7_]*[0-7];
BINARY_INTEGER : '0b' [01_]*[01];

