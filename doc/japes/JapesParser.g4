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
basic_type: identifier generics_list? pointer_declaration?;
type: type_qualifiers? basic_type;
type_qualifiers: (CONST | CONSTEXPR | VOLATILE);
pointer_declaration: STAR* array_declaration? nullability? reference_declaration?;
reference_declaration: AMP | REF_IN | REF_OUT;
array_declaration: LBRACKET COMMA* RBRACKET;
nullability: QUESTION;

constant_expression: number | boolean | string_literal;
number: integer | DECIMAL_REAL;
integer: DECIMAL_INTEGER | HEX_INTEGER | BINARY_INTEGER | OCTAL_INTEGER;
boolean: TRUE | FALSE;
string_literal: STRING_C_LIKE | STRING_VERBATIM | STRING_FORMAT | STRING_FORMAT_VERBATIM;

visibility: PRIVATE INTERNAL? | NAMESPACE? INTERNAL | PROTECTED INTERNAL? | PUBLIC ;

generics_list: GENERIC_BEGIN identifier (COMMA identifier)* RBRACKET;

template_list: TEMPLATE_BEGIN (template_element (COMMA template_element)*)? RBRACKET;
template_element: mkw_typename identifier | method_parameter | expression;

/* -- Contextual Metakeywords -- */

mkw_where: ID {LangTextIn(_localctx.Start, "where")}?;
mkw_with: ID {LangTextIn(_localctx.Start, "with")}?;
mkw_case: DEFAULT | ID {LangTextIn(_localctx.Start, "case", "where")}?;
mkw_in: ID {LangTextIn(_localctx.Start, "in")}?;
mkw_classSpec: ID {LangTextIn(_localctx.Start, "ref", "partial")}?;
mkw_cast: ID {LangTextIn(_localctx.Start, "static_cast", "dynamic_cast", "reinterpret_cast")}?;
mkw_typename: CLASS | STRUCT | INTERFACE | ID {LangTextIn(_localctx.Start, "typename")}?;
mkw_methodSpec: ID {LangTextIn(_localctx.Start, "partial")}?;
mkw_methodParamSpec: ID {LangTextIn(_localctx.Start, "params")}?;
mkw_get: ID {LangTextIn(_localctx.Start, "get")}?;
mkw_set: ID {LangTextIn(_localctx.Start, "set", "init")}?;
mkw_implicit: ID {LangTextIn(_localctx.Start, "implicit", "explicit")}?;
mkw_index: ID {LangTextIn(_localctx.Start, "index")}?;

/* -- Namespace -- */

namespace_declaration: NAMESPACE identifier SEMICOLON;
namespace_definition: NAMESPACE identifier LBRACE namespace_element* RBRACE;
namespace_element: namespace_definition | composite_definition | using_pragma;

/* -- Attributes -- */

attribute_invocation: LBRACKET identifier (LPAREN expression_list R_PARENT)? RBRACKET;

/* -- Composites -- */

composite_definition: attribute_invocation* (class_definition | enum_definition);

class_definition: visibility? mkw_classSpec* class_type unqualified_identifier generics_list? 
	class_inheritance? class_constraints* LBRACE class_body RBRACE SEMICOLON?;
class_type: (ABSTRACT | STATIC | SEALED)? CLASS | VIRTUAL? STRUCT | UNION | INTERFACE;
class_inheritance: COLON identifier (COMMA identifier)*;
class_constraints: mkw_where identifier COLON identifier | identifier LEADS_TO identifier;
class_body: (method_declaration 
	| constructor_definition | static_constructor_definition | destructor_definition
	)*;

enum_definition: visibility? ENUM identifier LBRACE RBRACE SEMICOLON?;

/* -- Fields and variables -- */

variable_definition: type variable_definition_clause 
	(COMMA variable_definition_clause)*
	;
variable_definition_clause: identifier (EQUALS expression)?;

field_definition: field_spec type variable_definition_clause
	(COMMA variable_definition_clause)*;
field_spec: attribute_invocation* visibility? STATIC?; 

property_definition: field_spec method_spec type property_body;
property_body: FAT_ARROW statement | property_block SEMICOLON?;
property_block: LBRACE property_getter? property_setter* RBRACE;
property_getter: visibility? mkw_get property_callback_body;
property_setter: visibility? mkw_set (mkw_with type)? property_callback_body;
property_callback_body: (FAT_ARROW statement)? SEMICOLON | block_statement;

/* -- Methods And Callables -- */

callable: attribute_invocation* (
	method_definition | constructor_definition 
	| static_constructor_definition | destructor_definition
	| operator_definition | indexer_definition
	);

method_definition: method_declaration ( method_body | SEMICOLON);
method_declaration: visibility? method_spec type
	identifier (generics_list | template_list)? method_parameter_list;
method_spec: (STATIC | ABSTRACT | OVERRIDE | SEALED)* mkw_methodSpec*;
method_parameter: mkw_methodParamSpec* type identifier (EQUALS expression)?;
method_parameter_list: LPAREN (method_parameter (COMMA method_parameter)*)? RPAREN;
method_body: block_statement | FAT_ARROW statement | DEFAULT;

constructor_definition: visibility? CTOR? (identifier | THIS) method_parameter_list 
	constructor_preamble? method_body
	;
constructor_preamble: COLON constructor_preinit (COMMA constructor_preinit)*;
constructor_preinit: (THIS | BASE | identifier) LPAREN 
	(expression (COMMA expression)*)? RPAREN
	;

static_constructor_definition: STATIC (identifier | THIS)? block_statement;
destructor_definition: VIRTUAL? TILDE (identifier | THIS) method_parameter_list
	method_body
	;

operator_definition: visibility? method_spec  
	(casting_operator_declaration | other_operator_declaration)
	method_body
	;
casting_operator_declaration: mkw_implicit OPERATOR type method_parameter_list;
other_operator_declaration: type OPERATOR overloadable_operator;
overloadable_operator: 
	| RIGHT_ARROW
	| FMA | EXPONENT
	| STAR | SLASH | PERCENT
	| PLUS | MINUS
	| AMP | OR | XOR 
	| LROT | LSHIFT | RROT | RSHIFT
	| EQ | NE | LT | LE | GT | GE | CMP | NCMP
	| MATCH | NMATCH | TILDE | NCONG
	| LAND | LOR | LXOR | LEADS_TO
	| TRUE | FALSE 
	;

indexer_definition: visibility? method_spec type 
	LBRACKET (method_parameter (COMMA method_parameter)*)? BRACKET
	property_body
	;

/* -- Statements -- */

statement: block_statement | conditional_statement | single_statement? SEMICOLON;
block_statement: LBRACE statement* RBRACE;
conditional_statement: if_statement 
	| while_statement | do_while_statement
	| for_statement | try_statement | using_statement 
	;

if_statement: (IF | UNLESS) statement (ELSE statement);
while_statement: (WHILE | UNTIL) LPAREN expression RPAREN statement;

do_while_statement: DO statement (WHILE | UNTIL) SEMICOLON;
for_statement: FOR LPAREN (for_loop | foreach_loop) R_PAREN statement;
for_loop: 
	for_init SEMICOLON
	expression_list SEMICOLON
	expression_list
	;
for_init: (variable_definition | expression);
foreach_loop: type? identifier (COLON | mkw_in) expression 
	foreach_with? foreach_index?
	;
foreach_with: mkw_with type? identifier;
foreach_index: mkw_index variable_definition;
expression_list: expression (COMMA expression)*;

try_statement: TRY (USING for_init)? statement
	(CATCH LPAREN type identifier? RPAREN statement)*
	(FINALLY statement)?
	;

using_statement: USING using_disposable | using_pragma;
using_disposable: LPAREN (variable_definition) RPAREN statement;
using_pragma: using_namespace | using_alias;
using_namespace: (NAMESPACE | STATIC)? identifier; 
using_alias: unqualified_identifier EQUALS identifier;

return_statement: (RETURN | THROW) expression;
single_statement: variable_definition | expression | return_statement;

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
relational_expression: match_expression (( EQ | NE | REQ | RNE | CMP | NCMP
	| MATCH | NMATCH | LT | LE | GT | GE
	) match_expression)*; 
bitand_expression: relational_expression (AMP relational_expression)*;
bitor_expression: bitand_expression ((OR | XOR) bitand_expression)*;
logand_expression: bitor_expression (LAND bitor_expression)*;
logor_expression: logand_expression ((LOR | LXOR) logand_expression)*;
coalesce_expression: logor_expression (COALESCE logor_expression)*;
leadsto_expression: coalesce_expression (LEADS_TO leadsto_expression)?;
ternary_expression: leadsto_expression ((QUESTION | NQUESTION) ternary_expression COLON ternary_expression)?; 
pipeline_expression: ternary_expression ((PIPE_IN | PIPE_OUT) ternary_expression)*;
assignment_expression: pipeline_expression ((ASSIGN
	| ADD_ASSIGN | SUB_ASSIGN | MUL_ASSIGN | DIV_ASSIGN | MOD_ASSIGN
	| LROT_ASSIGN | LSHIFT_ASSIGN | RROT_ASSIGN | RSHIFT_ASSIGN 
	| LAND_ASSIGN | AND_ASSIGN | LOR_ASSIGN | OR_ASSIGN | LXOR_ASSIGN | XOR_ASSIGN
	) expression)?;
expression: assignment_expression ;