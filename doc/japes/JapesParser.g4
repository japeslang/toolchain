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

parser grammar JapesParser;

/* -- Translation unit -- */
translation_unit: tu_element*;
tu_element: namespace_declaration | namespace_definition | composite_definition;

/* Forward Nonterminals */

unqualified_identifier : ID | ID_AT;
identifier: unqualified_identifier (DOT unqualified_identifier)*;
type: identifier generics_list?;

visibility: PRIVATE INTERNAL? | INTERNAL | PROTECTED INTERNAL? | PUBLIC;

generics_list: GENERIC_BEGIN identifier (COMMA identifier)* RBRACKET;

/* -- Namespace -- */

namespace_declaration: NAMESPACE identifier SEMICOLON;
namespace_definition: NAMESPACE identifier LBRACE namespace_element* RBRACE;
namespace_element: namespace_definition | composite_definition;

/* -- Composites -- */

composite_definition: class_definition | enum_definition;

class_definition: visibility? class_specifiers class_type generics_list class_inheritance? class_constraints LBRACE class_body RBRACE SEMICOLON?;
class_specifiers: STATIC? PARTIAL?;
class_type: CLASS | VIRTUAL? STRUCT | UNION | INTERFACE;
class_inheritance: COLON identifier (COMMA identifier)*;
class_constraints: WHERE identifier COLON identifier;
class_body: /* empty */;

enum_definition: visibility? ENUM identifier LBRACE RBRACE SEMICOLON?;