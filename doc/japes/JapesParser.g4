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

options {
	superClass = BasicJapesParser;
}


/* -- Translation unit -- */
translation_unit: tu_element*;
tu_element: namespace_declaration | namespace_definition | composite_definition;

/* -- Forward Nonterminals -- */

unqualified_identifier : ID | ID_AT;
identifier: unqualified_identifier (DOT unqualified_identifier)*;
type: identifier generics_list? pointer_declaration?;
pointer_declaration: STAR* reference_declaration?;
reference_declaration: AMP | REF_IN | REF_OUT;

constant_expression: number | boolean | string_literal;
number: integer | DECIMAL_REAL;
integer: DECIMAL_INTEGER | HEX_INTEGER | BINARY_INTEGER | OCTAL_INTEGER;
boolean: TRUE | FALSE;
string_literal: STRING_C_LIKE | STRING_VERBATIM;

visibility: PRIVATE INTERNAL? | INTERNAL | PROTECTED INTERNAL? | PUBLIC | PACKAGE;

generics_list: GENERIC_BEGIN identifier (COMMA identifier)* RBRACKET;

/* -- Contextual Metakeywords -- */

mkw_where: ID {LangTextIn(_localctx.Start, "where")}?;
mkw_case: DEFAULT | ID {LangTextIn(_localctx.Start, "case", "where")}?;
mkw_in: ID {LangTextIn(_localctx.Start, "in")}?;
mkw_classSpec: ID {LangTextIn(_localctx.Start, "ref", "partial")}?;
mkw_cast: ID {LangTextIn(_localctx.Start, "static_cast", "dynamic_cast", "reinterpret_cast")}?;

/* -- Namespace -- */

namespace_declaration: NAMESPACE identifier SEMICOLON;
namespace_definition: NAMESPACE identifier LBRACE namespace_element* RBRACE;
namespace_element: namespace_definition | composite_definition;

/* -- Composites -- */

composite_definition: class_definition | enum_definition;

class_definition: visibility? mkw_classSpec* class_type unqualified_identifier generics_list? 
	class_inheritance? class_constraints* LBRACE class_body RBRACE SEMICOLON?;
class_type: (ABSTRACT | STATIC)? CLASS | VIRTUAL? STRUCT | UNION | INTERFACE;
class_inheritance: COLON identifier (COMMA identifier)*;
class_constraints: mkw_where identifier COLON identifier | identifier LEADS_TO identifier;
class_body: /* empty */;

enum_definition: visibility? ENUM identifier LBRACE RBRACE SEMICOLON?;

/* -- Expression :: Operators -- */

primary_expression: identifier | constant_expression | LPAREN expression RPAREN
	| mkw_cast TEMPLATE_BEGIN type RBRACKET LPAREN expression RPAREN
	;

prefix_expression: (REF_IN | REF_OUT)? (STAR | AMP | INC | DEC | TILDE | BANG | MATCH | NMATCH | MINUS | PLUS)* primary_expression;  
postfix_expression: prefix_expression
	| prefix_expression (INC | DEC)
	| prefix_expression LPAREN (expression (COMMA expression )*) RPAREN
	| prefix_expression LBRACKET (expression (COMMA expression )*) RBRACKET
	; 
member_expression: postfix_expression ((DOT | ARROW) postfix_expression)*;
exponential_expression: member_expression EXPONENT exponential_expression;
fma_expression: exponential_expression (FMA exponential_expression)*;
multiplicative_expression: fma_expression (( STAR | SLASH | PERCENT ) fma_expression)*;
additive_expression: multiplicative_expression (( PLUS | MINUS ) multiplicative_expression)*;
shift_expression: additive_expression ((LROT | LSHIFT | RROT | RSHIFT) additive_expression)*;
match_expression: shift_expression ((TILDE | NCONG) shift_expression)*; 
relational_expression: match_expression (( EQ | NE | REQ | RNE | CMP | NCMP | MATCH | NMATCH) match_expression)*; 
bitand_expression: relational_expression (AND relational_expression)*;
bitor_expression: bitand_expression ((OR | XOR) bitand_expression)*;
logand_expression: bitor_expression (LAND bitor_expression)*;
logor_expression: logand_expression ((LOR | LXOR) logand_expression)*;
coalesce_expression: logor_expression (COALESCE logor_expression)*;
leadsto_expression: coalesce_expression (LEADS_TO leadsto_expression)?;
ternary_expression: leadsto_expression ((QUESTION | NQUESTION) ternary_expression COLON ternary_expression); 
pipeline_expression: ternary_expression ((PIPE_IN | PIPE_OUT) ternary_expression)*;
assignment_expression: pipeline_expression ((ASSIGN
	| ADD_ASSIGN | SUB_ASSIGN | MUL_ASSIGN | DIV_ASSIGN | MOD_ASSIGN
	| LROT_ASSIGN | LSHIFT_ASSIGN | RROT_ASSIGN | RSHIFT_ASSIGN 
	| LAND_ASSIGN | AND_ASSIGN | LOR_ASSIGN | OR_ASSIGN | LXOR_ASSIGN | XOR_ASSIGN
	) expression)?;
expression: assignment_expression ;