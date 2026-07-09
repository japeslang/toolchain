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
 * This file contains the main grammar. It provides (as closely as possible) a
 * lexer grammar for the Japes Programming Language. This tokenizer produces
 * a stream of compiler tokens which can later be consumed by a parsing stage,
 * most notable the Japesc compiler.
 *
 * This grammar is unusual in that it is a context-free grammar (CFG), not a 
 * regular grammar. The way multiline comments are handled are a Dyck language
 * (that is, they are congruent to the Language of Balanced Parentheses). 
 * However, the automaton needed to do this is not that much more complex and
 * only requires a counter and an unbounded string buffer, so the actual implementation
 * is not much more burdensome compared to the things going on in the rest of the
 * language.
 * 
 * Although this is not difficult to do, the process to do so usually ends up being
 * highly implementation-defined. Instead, the implementation ends up being provided
 * by the following interface (which is provided in object-oriented languages
 * by the japes.toolchain.api.parser.BasicJapesLexer class). 
 *
 * The grammar is also targeting LL(2) unless ALL(*) becomes necessary. This is
 * to make it easily implementable by other tools (including a nascent japesc 
 * compiler-compiler) that can read g4 files. ALL(*) is a highly-optimized
 * algorithm that will require substantial effort to implement, and SLL(2)
 * will lead to faster implementation times.
 *
 * The following reference has been provided for the intrinsic operations:
 *
 * - void LexEnterMLComment() - track the opening of a multiline comment.
 *   When first invoked, it clears the internal string buffer; subsequent calls
 *   increase the nesting level. It must be matched by LexLeaveMLComment().
 *
 * - bool LexLeaveMLComment() - undo LexEnterMLComment(). If the nesting level
 *   is 0, then a compiler bug is emitted. If it is 1, true is returned;
 *   otherwise, false is returned. In any event, the nesting level is decreased.
 *
 * - void LexAppendMLComment(string) - Append text to the multiline comment buffer.
 *
 * - void LexAppendMLCommentText() - Synonymous with LexAppendMLComment(LexText()).
 *
 * - string LexRenderMLComment() - Obtain the current contents of the ML string
 *   buffer. It may only be accessed when the nesting level is 0.
 *
 * - int LexLevelMLComment() - Obtain the current nesting level of the ML 
 *   coment buffer.
 *
 * - LexConsolidateMLComment(tokenType, channel, description, expected) - If the nesting 
 *   level is 0, set the token type to tokenType, set the text to text, set the channel
 *   to channel, then pop the mode. Otherwise, append the text to the ML comment buffer.
 *
 * - LexCompilerBug2(errorSeverity, string, ...) - Explicitly raise an exception
 *   indicating that the compiler encountered a known bug. Severity options 
 *   include ERROR_WARNING, ERROR_RECOVERABLE, ERROR_FATAL. If ERROR_WARNING
 *   is used, a log message at the WARNING log level is emitted instead.
 *
 * - LexCompilerBug(string, ...) - Explicitly raise a fatal exception indicating
 *   that the compiler cannot continue due to a known bug.
 *
 * - LexBanter(string, ...) - Emit a a log message at the TRIVIA log level.
 *
 * - LexElaborate(string, ...) - Emit a log message at the INFO log level.
 *
 * - LexWarn(string, ...) - Emit a log message at the WARNING log level.
 *
 * - LexError(string, ...) - Emit an log message at the ERROR log level.
 *
 * - LexFatal(string, ...) - Emit a fatal error and raise a fatal eception.
 *
 * - LexLog(type, string, ...) - Emit a log message. May be one of LOG_TRIVIA,
 *   LOG_INFO, LOG_WARNING, LOG_ERROR, and LOG_FATAL.
 *
 * - LogInhibitError(callback, ...) - Invoke the specified function and inhibit
 *   recoverable error propagation. FATAL is still propagated and errors will 
 *   still eventually stall the pipeline. This frees the lexer to attempt
 *   subsequent parsing at the node where it is inhibited.
 *
 *	- LexText() - Obtain the current token text.
 *
 *  - LexSetText(string) - Set the current token text.
 * 
 * - LexModePush(mode) - Synonymous with pushMode(), but to account
 *	  for nomenclature differences in different host languages.
 *
 * - LexModePush(mode) - Synonymous with pushMode(), but to account
 *	  for nomenclature differences in different languages.
 *
 * - LexTokenType() - Obtain the current token type.
 *
 * - LexSetTokenType(tokenType) - Set the token type to tokenType.
 *
 * - LexTokenSetChannel(channel) - Set the current channel.
 *
 * - LexStringLeft(string, length) - Obtain (at most) length characters relative to the
 *   left of the string. If a negative index is supplied, it chops the rightmost characters
 *   instead.
 *
 * - LexStringRight(string, length) - Obtain (at most) length characters relative to the right
 *   of the string. If a negative index is supplied, it chops the leftmost characters instead.
 */

lexer grammar JapesLexer;

options {
	superClass = BasicJapesLexer;
}

/* == Tokens == */

/* Forward declarations of meta-tokens used to transmit trivia */

tokens {
	DOCCOMMENT_MACRO,
	COMMENT_MACRO,
	DOCCOMMENT_ML,
	COMMENT_ML
}

/* -- Comments --  */

DOCCOMMENT_SINGLE: '///'~[\r\n]* {
	LexBanter("Consuming DOCCOMMENT_SINGLE");

	LexTextSet(LexTextRight(-3));

	LexTokenSetChannel(CHANNEL_DOCCOMMENTS);
};

COMMENT_SINGLE: '//'~[\r\n]* {
	LexBanter("Consuming COMMENT_SINGLE");

	LexTextSet(LexTextRight(-2));

	LexTokenSetChannel(CHANNEL_TRIVIA);
};

START_DOCCOMMENT_ML: '/**' {
	LexModePush(MODE_DOCCOMMENT_ML);
	LexEnterMLComment();
};

START_COMMENT_ML: '/*' {
	LexModePush(MODE_COMMENT_ML);
	LexEnterMLComment();
};

START_DOCCOMMENT_MACRO: '#doccomment' {
	LexModePush(MODE_DOCCOMMENT_MACRO);
	LexEnterMLComment();
};

START_COMMENT_MACRO: '#comment' {
	LexModePush(MODE_COMMENT_MACRO);
	LexEnterMLComment();
};

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
DECIMAL_REAL: [0-9]*[.][0-9]+([Ee][+-]?[0-9]+)? | [1-9][0-9]*[Ee][+-]?[0-9]+;
HEX_INTEGER: '0x' [0-9ABCDEFabcdef_]*[0-9ABCDEFabcdef];
OCTAL_INTEGER: 0' [0-7_]*[0-7];
BINARY_INTEGER: '0b' [01_]*[01];

/* == MODE 1: MODE_DOCCOMMENT_ML == */

mode MODE_DOCCOMMENT_ML;

NESTED_START_DOCCOMMENT_ML: '/**' {
	LexEnterMLComment();
	LexAppendMLCommentText();
};

CONTENT_DOCCOMMENT_ML: . {
	LexAppendMLCommentText();
};

END_DOCCOMMENT_ML: '*/' {
	LexConsolidateMLComment(DOCCOMMENT_ML, CHANNEL_DOCCOMMENTS, "multi-line comment", "*/");
	LexModePop();
};

/* == MODE 2: MODE_COMMENT_ML == */

mode MODE_COMMENT_ML;

NESTED_START_COMMENT_ML: '/*' {
	LexEnterMLComment();
	LexAppendMLCommentText();
};

CONTENT_COMMENT_ML: . {
	LexAppendMLCommentText();
};

END_COMMENT_ML: '*/' {
	LexConsolidateMLComment(COMMENT_ML, CHANNEL_TRIVIA, "multi-line comment", "*/");
	LexModePop();
};

/* == MODE 3: MODE_DOCCOMMENT_MACRO == */
mode MODE_DOCCOMMENT_MACRO;

NESTED_START_DOCCOMMENT_MACRO: '#doccomment' | '#comment' {
	LexEnterMLComment();
	LexAppendMLCommentText();
};

CONTENT_DOCCOMMENT_MACRO: . {
	LexAppendMLCommentText();
};

END_DOCCOMMENT_MACRO: '#endcomment' {
	LexConsolidateMLComment(DOCCOMMENT_MACRO, CHANNEL_DOCCOMMENTS, "macro multi-line comment", "#endcomment");
	LexModePop();
};

/* == MODE 4: MODE_COMMENT_MACRO == */
mode MODE_COMMENT_MACRO;

NESTED_START_COMMENT_MACRO: '#doccomment' | '#comment' {
	LexEnterMLComment();
	LexAppendMLCommentText();
};

CONTENT_COMMENT_MACRO: . {
	LexAppendMLCommentText();
};

END_COMMENT_MACRO: '#endcomment' {
	LexConsolidateMLComment(COMMENT_MACRO, CHANNEL_TRIVIA, "macro multi-line comment", "#endcomment");
	LexModePop();
};