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

/**
 * @file JapesLexer.g4
 * @since 2026/07/06
 * 
 * This file contains the main grammar. It will provide everything but rules
 * for the comments, which whill change depending on the implementation. To
 * accomodate this, you need to provide the lexer grammar `JapesLexerImpl`.
 * Without it, you cannot nest multiline comments. The JapesLexerImpl grammar
 * defines the following terminals:
 *
 * - DOCCOMMENT_MULTI - Multiline ('/**') comments.
 * - DOCCOMMENT_MACRO - Macro-style comments ('#doccomment'/'#comment')
 *
 * These comments require a mode switch because they maintain a counter so that
 * nested comments. Regular comments will have been stripped out of the sources
 * by J4 by this point; however, we keep the doc comments so that we can 
 * extract them at parse time. It is frequently annoying when they are not;
 * 
 */

lexer grammar JapesLexer;

@members {
	private int m_docCommentNesting = 0;
	private System.Text.StringBuilder m_docCommentAggregator = new System.Text.StringBuilder();
}

/* == Tokens == */

/* -- Comments --  */

DOCCOMMENT_SINGLE: '///'~[\r\n]*;

/* -- Keywords :: Branch -- */

IF: 'if';
UNLESS: 'unless';
ELSE: 'else';
WHILE: 'while';
UNTIL: 'until';
DO: 'do';
FOR: 'for';
SWITCH: 'switch';
CASE: 'case';

/* -- Keywords :: Types */

CLASS: 'class';
STRUCT: 'struct';
INTERFACE: 'interface';
ENUM: 'enum';
UNION: 'union';
VIRTUAL: 'virtual';
ABSTRACT: 'abstract';
OVERRIDE: 'override';
STATIC: 'static';

/* -- Punctuators -- */

COMMA: ',';
DOT: '.';
SEMICOLON: ';';
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
LBRACKET: '[';
RBRACKET: ']';
GENERIC_BEGIN: '$[';
TEMPLATE_BEGIN: '![';
FAT_ARROW: '=>';

/* Primary Expressions */

ID : [A-Za-z_][0-9A-Za-z_]*;
ID_AT : '@' [A-Za-z_][0-9A-Za-z_]*;

DECIMAL_INTEGER: '0' | [+-]?[1-9][0-9]*;
DECIMAL_REAL: [0-9]*[.][0-9]+([Ee][+-]?[0-9]+)?;
HEX_INTEGER: '0x' [0-9ABCDEFabcdef_]*[0-9ABCDEFabcdef];
OCTAL_INTEGER: '0' [0-7_]*[0-7];
BINARY_INTEGER: '0b' [01_]*[01];
