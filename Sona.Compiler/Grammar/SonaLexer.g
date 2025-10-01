lexer grammar SonaLexer;

channels {
  Documentation,
  Pragma
}

fragment LOWERCASE:
  [a-z];

fragment UPPERCASE:
  [A-Z];

fragment DIGIT:
  [0-9];

fragment HEXDIGIT:
  DIGIT | [a-f] | [A-F];

INT_LITERAL:
  DIGIT+;

FLOAT_LITERAL:
  INT_LITERAL '.' DIGIT+;

EXP_LITERAL:
  (INT_LITERAL | FLOAT_LITERAL) ('E' | 'e') ('+' | '-')? INT_LITERAL;

HEX_LITERAL:
  '0x' HEXDIGIT+;

INT_SUFFIX: INT_LITERAL NAME;
FLOAT_SUFFIX: FLOAT_LITERAL NAME;
EXP_SUFFIX: EXP_LITERAL NAME;
HEX_SUFFIX: HEX_LITERAL NAME;

BEGIN_STRING:
  '"' -> pushMode(String);

BEGIN_VERBATIM_STRING:
  '@"' -> pushMode(VerbatimString);

BEGIN_INTERPOLATED_STRING:
  '$"' -> pushMode(InterpolatedString);

BEGIN_VERBATIM_INTERPOLATED_STRING:
  ('$@' | '@$') '"' -> pushMode(VerbatimInterpolatedString);

BEGIN_CHAR:
  '\'' -> pushMode(Char);

COMMENT:
  '/*' (~[*] | '*' ~[/])* '*'+ '/' -> skip;

fragment EOL:
  '\r'? ('\n' | EOF);

DOC_COMMENT:
  '///' ~[\n\r]* -> channel(Documentation);

LINE_COMMENT:
  '//' ~[\n\r]* -> skip;

fragment LINE_WHITESPACE:
  ' ' | '\t' | '\u000C';

fragment UNICODE:
  [\u00A1-\uFFFF];

WHITESPACE:
  (LINE_WHITESPACE | '\r' | '\n') -> skip;

fragment IGNORE:
  (WHITESPACE | COMMENT | LINE_COMMENT)+;

fragment ATTR_TARGET_LOCAL:
  'item' | 'type' | 'method' | 'property' | 'return' | 'param' | 'field' | 'event' | 'constructor';

fragment ATTR_TARGET_GLOBAL:
  'program' | 'assembly' | 'module' | 'entry';

fragment ATTR_TARGET_SEPARATOR:
  LINE_WHITESPACE* ':' | LINE_WHITESPACE;

BEGIN_GENERAL_LOCAL_ATTRIBUTE:
  '#:' -> mode(Directive);

BEGIN_TARGETED_LOCAL_ATTRIBUTE:
  '#' ATTR_TARGET_LOCAL ATTR_TARGET_SEPARATOR -> mode(Directive);

BEGIN_TARGETED_GLOBAL_ATTRIBUTE:
  '#' ATTR_TARGET_GLOBAL ATTR_TARGET_SEPARATOR -> mode(Directive);

BEGIN_INLINE_SOURCE:
  '#inline' LINE_WHITESPACE -> mode(InlineDirective);

BEGIN_PRAGMA:
  '#pragma' LINE_WHITESPACE -> channel(Pragma), mode(PragmaDirective);

LITERAL_NAME:
  '@' EXTENDED_NAME;

MEMBER_NAME:
  '.' NAME;

DYNAMIC_MEMBER_NAME:
  ':' NAME;

fragment TRUE_EXPR:
  'true' | '(' IGNORE TRUE_EXPR IGNORE ')';

ABSTRACT: 'abstract';
ADD: 'add';
AND: 'and';
AS: 'as';
BASE: 'base';
BREAK: 'break';
BY: 'by';
CASE: 'case';
CATCH: 'catch';
CLASS: 'class';
CONST: 'const';
CONTINUE: 'continue';
DEFAULT: 'default';
DEFER: 'defer';
DELEGATE: 'delegate';
DELETE: 'delete';
DO: 'do';
ECHO: 'echo';
ELSE: 'else';
ELSEIF: 'elseif';
END: 'end';
ENUM: 'enum';
EVENT: 'event';
EXCEPTION: 'exception';
EXPLICIT: 'explicit';
EXTENSION: 'extension';
EXTERN: 'extern';
FALSE: 'false';
FINALLY: 'finally';
FIXED: 'fixed';
FOLLOW: 'follow';
FOR: 'for';
FUNCTION: 'function';
GET: 'get';
GLOBAL: 'global';
GOTO: 'goto';
IF: 'if';
IMPLICIT: 'implicit';
IMPORT: 'import';
IN: 'in';
INCLUDE: 'include';
INHERIT: 'inherit';
INLINE: 'inline';
INTERFACE: 'interface';
INTERNAL: 'internal';
INTO: 'into';
IS: 'is';
LAZY: 'lazy';
LET: 'let';
LOCK: 'lock';
MODULE: 'module';
NAMEOF: 'nameof';
NAMESPACE: 'namespace';
NARROW: 'narrow';
NEW: 'new';
NONE: 'none';
NOT: 'not';
NULL: 'null';
ON: 'on';
OPERATOR: 'operator';
OR: 'or';
OUT: 'out';
OVERRIDE: 'override';
PACKAGE: 'package';
PARTIAL: 'partial';
PRIVATE: 'private';
PROTECTED: 'protected';
PUBLIC: 'public';
RAISE: 'raise';
RECORD: 'record';
REF: 'ref';
REMOVE: 'remove';
REPEAT: 'repeat';
REQUIRE: 'require';
RETURN: 'return';
SEALED: 'sealed';
SET: 'set';
SIZEOF: 'sizeof';
SOME: 'some';
STATIC: 'static';
STRUCT: 'struct';
SWITCH: 'switch';
THEN: 'then';
THROW: 'throw';
TRAIT: 'trait';
TRUE: 'true';
TRY: 'try';
TYPEOF: 'typeof';
UNION: 'union';
UNIT: 'unit';
UNTIL: 'until';
USE: 'use';
VAR: 'var';
VIRTUAL: 'virtual';
WHEN: 'when';
WHILE: 'while';
WHILE_TRUE_DO: 'while' IGNORE TRUE_EXPR IGNORE 'do';
WIDEN: 'widen';
WITH: 'with';
YIELD: 'yield';
YIELD_BREAK: 'yield' IGNORE 'break';
YIELD_RETURN: 'yield' IGNORE 'return';

BOOL: 'bool';
BYTE: 'byte' -> type(UINT8);
SBYTE: 'sbyte' -> type(INT8);
SHORT: 'short' -> type(INT16);
USHORT: 'ushort' -> type(UINT16);
LONG: 'long' -> type(INT64);
ULONG: 'ulong' -> type(UINT64);
INT: 'int';
INT8: 'int8';
INT16: 'int16';
INT32: 'int32';
INT64: 'int64';
INT128: 'int128';
NATIVEINT: 'nativeint';
UINT: 'uint';
UINT8: 'uint8';
UINT16: 'uint16';
UINT32: 'uint32';
UINT64: 'uint64';
UINT128: 'uint128';
UNATIVEINT: 'unativeint';
BIGINT: 'bigint';
FLOAT: 'float';
FLOAT16: 'float16';
FLOAT32: 'float32';
FLOAT64: 'float64';
HALF: 'half' -> type(FLOAT16);
SINGLE: 'single' -> type(FLOAT32);
DOUBLE: 'double' -> type(FLOAT64);
DECIMAL: 'decimal';
CHAR: 'char';
STRING: 'string';
OBJECT: 'object';
VOID: 'void';

UNDERSCORE: '_';

SEMICOLON: ';';
EQ: '==';
NEQ: '!=';
NEQ_ALT: '~=';
COMMA: ',';
DOLLAR: '$';
HASH: '#';
OPENP: '(' -> pushMode(DEFAULT_MODE);
CLOSEP: ')' -> popMode;
OPENSP: '[' -> pushMode(DEFAULT_MODE);
CLOSESP: ']' -> popMode;
OPENB: '{' -> pushMode(DEFAULT_MODE);
CLOSEB: '}' -> popMode;
PLUS: '+';
MINUS: '-';
ASTERISK: '*';
SLASH: '/';
PERCENT: '%';
CONCAT: '..';
LT: '<';
LTE: '<=';
GT: '>';
ASSIGN: '=';
QUESTION: '?';
DOUBLE_AND: '&&';
DOUBLE_OR: '||';
SINGLE_AND: '&';
SINGLE_OR: '|';
SINGLE_XOR: '^';
LSHIFT: '<<';
EXCLAMATION: '!';
TILDE: '~';
COLON: ':';
DOT: '.';

NAME:
  (LOWERCASE | UPPERCASE | '_' | UNICODE) (LOWERCASE | UPPERCASE | DIGIT | '_' | UNICODE)*;

fragment EXTENDED_NAME:
  (LOWERCASE | UPPERCASE | '_' | UNICODE) (LOWERCASE | UPPERCASE | DIGIT | '_' | '\'' | UNICODE)*;

mode Directive;

fragment NEWLINE_ESCAPE: '\\' EOL;

END_DIRECTIVE:
  (':#' | '#' | EOL | EOF) -> mode(DEFAULT_MODE);

Directive_INT_LITERAL: INT_LITERAL -> type(INT_LITERAL);
Directive_FLOAT_LITERAL: FLOAT_LITERAL -> type(FLOAT_LITERAL);
Directive_EXP_LITERAL: EXP_LITERAL -> type(EXP_LITERAL);
Directive_HEX_LITERAL: HEX_LITERAL -> type(HEX_LITERAL);
Directive_INT_SUFFIX: INT_SUFFIX -> type(INT_SUFFIX);
Directive_FLOAT_SUFFIX: FLOAT_SUFFIX -> type(FLOAT_SUFFIX);
Directive_EXP_SUFFIX: EXP_SUFFIX -> type(EXP_SUFFIX);
Directive_HEX_SUFFIX: HEX_SUFFIX -> type(HEX_SUFFIX);
Directive_BEGIN_STRING: BEGIN_STRING -> type(BEGIN_STRING), pushMode(String);
Directive_BEGIN_VERBATIM_STRING: BEGIN_VERBATIM_STRING -> type(BEGIN_VERBATIM_STRING), pushMode(VerbatimString);
Directive_BEGIN_CHAR: BEGIN_CHAR -> type(BEGIN_CHAR), pushMode(Char);

Directive_COMMENT: COMMENT -> skip;
Directive_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
Directive_LINE_COMMENT: LINE_COMMENT -> skip;
Directive_WHITESPACE: (WHITESPACE | NEWLINE_ESCAPE) -> type(WHITESPACE);

Directive_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME);
Directive_MEMBER_NAME: MEMBER_NAME -> type(MEMBER_NAME);
Directive_DYNAMIC_MEMBER_NAME: DYNAMIC_MEMBER_NAME -> type(DYNAMIC_MEMBER_NAME);

Directive_ABSTRACT: ABSTRACT -> type(ABSTRACT);
Directive_ADD: ADD -> type(ADD);
Directive_AND: AND -> type(AND);
Directive_AS: AS -> type(AS);
Directive_BASE: BASE -> type(BASE);
Directive_BREAK: BREAK -> type(BREAK);
Directive_BY: BY -> type(BY);
Directive_CASE: CASE -> type(CASE);
Directive_CATCH: CATCH -> type(CATCH);
Directive_CLASS: CLASS -> type(CLASS);
Directive_CONST: CONST -> type(CONST);
Directive_CONTINUE: CONTINUE -> type(CONTINUE);
Directive_DEFAULT: DEFAULT -> type(DEFAULT);
Directive_DEFER: DEFER -> type(DEFER);
Directive_DELEGATE: DELEGATE -> type(DELEGATE);
Directive_DELETE: DELETE -> type(DELETE);
Directive_DO: DO -> type(DO);
Directive_ECHO: ECHO -> type(ECHO);
Directive_ELSE: ELSE -> type(ELSE);
Directive_ELSEIF: ELSEIF -> type(ELSEIF);
Directive_END: END -> type(END);
Directive_ENUM: ENUM -> type(ENUM);
Directive_EVENT: EVENT -> type(EVENT);
Directive_EXCEPTION: EXCEPTION -> type(EXCEPTION);
Directive_EXPLICIT: EXPLICIT -> type(EXPLICIT);
Directive_EXTENSION: EXTENSION -> type(EXTENSION);
Directive_EXTERN: EXTERN -> type(EXTERN);
Directive_FALSE: FALSE -> type(FALSE);
Directive_FINALLY: FINALLY -> type(FINALLY);
Directive_FIXED: FIXED -> type(FIXED);
Directive_FOLLOW: FOLLOW -> type(FOLLOW);
Directive_FOR: FOR -> type(FOR);
Directive_FUNCTION: FUNCTION -> type(FUNCTION);
Directive_GET: GET -> type(GET);
Directive_GLOBAL: GLOBAL -> type(GLOBAL);
Directive_GOTO: GOTO -> type(GOTO);
Directive_IF: IF -> type(IF);
Directive_IMPLICIT: IMPLICIT -> type(IMPLICIT);
Directive_IMPORT: IMPORT -> type(IMPORT);
Directive_IN: IN -> type(IN);
Directive_INCLUDE: INCLUDE -> type(INCLUDE);
Directive_INHERIT: INHERIT -> type(INHERIT);
Directive_INLINE: INLINE -> type(INLINE);
Directive_INTERFACE: INTERFACE -> type(INTERFACE);
Directive_INTERNAL: INTERNAL -> type(INTERNAL);
Directive_INTO: INTO -> type(INTO);
Directive_IS: IS -> type(IS);
Directive_LAZY: LAZY -> type(LAZY);
Directive_LET: LET -> type(LET);
Directive_LOCK: LOCK -> type(LOCK);
Directive_MODULE: MODULE -> type(MODULE);
Directive_NAMEOF: NAMEOF -> type(NAMEOF);
Directive_NAMESPACE: NAMESPACE -> type(NAMESPACE);
Directive_NARROW: NARROW -> type(NARROW);
Directive_NEW: NEW -> type(NEW);
Directive_NONE: NONE -> type(NONE);
Directive_NOT: NOT -> type(NOT);
Directive_NULL: NULL -> type(NULL);
Directive_ON: ON -> type(ON);
Directive_OPERATOR: OPERATOR -> type(OPERATOR);
Directive_OR: OR -> type(OR);
Directive_OUT: OUT -> type(OUT);
Directive_OVERRIDE: OVERRIDE -> type(OVERRIDE);
Directive_PACKAGE: PACKAGE -> type(PACKAGE);
Directive_PARTIAL: PARTIAL -> type(PARTIAL);
Directive_PRIVATE: PRIVATE -> type(PRIVATE);
Directive_PROTECTED: PROTECTED -> type(PROTECTED);
Directive_PUBLIC: PUBLIC -> type(PUBLIC);
Directive_RAISE: RAISE -> type(RAISE);
Directive_RECORD: RECORD -> type(RECORD);
Directive_REF: REF -> type(REF);
Directive_REMOVE: REMOVE -> type(REMOVE);
Directive_REPEAT: REPEAT -> type(REPEAT);
Directive_REQUIRE: REQUIRE -> type(REQUIRE);
Directive_RETURN: RETURN -> type(RETURN);
Directive_SEALED: SEALED -> type(SEALED);
Directive_SET: SET -> type(SET);
Directive_SIZEOF: SIZEOF -> type(SIZEOF);
Directive_SOME: SOME -> type(SOME);
Directive_STATIC: STATIC -> type(STATIC);
Directive_STRUCT: STRUCT -> type(STRUCT);
Directive_SWITCH: SWITCH -> type(SWITCH);
Directive_THEN: THEN -> type(THEN);
Directive_THROW: THROW -> type(THROW);
Directive_TRAIT: TRAIT -> type(TRAIT);
Directive_TRUE: TRUE -> type(TRUE);
Directive_TRY: TRY -> type(TRY);
Directive_TYPEOF: TYPEOF -> type(TYPEOF);
Directive_UNION: UNION -> type(UNION);
Directive_UNIT: UNIT -> type(UNIT);
Directive_UNTIL: UNTIL -> type(UNTIL);
Directive_USE: USE -> type(USE);
Directive_VAR: VAR -> type(VAR);
Directive_VIRTUAL: VIRTUAL -> type(VIRTUAL);
Directive_WHEN: WHEN -> type(WHEN);
Directive_WHILE: WHILE -> type(WHILE);
Directive_WHILE_TRUE_DO: WHILE_TRUE_DO -> type(WHILE_TRUE_DO);
Directive_WIDEN: WIDEN -> type(WIDEN);
Directive_WITH: WITH -> type(WITH);
Directive_YIELD: YIELD -> type(YIELD);
Directive_YIELD_BREAK: YIELD_BREAK -> type(YIELD_BREAK);
Directive_YIELD_RETURN: YIELD_RETURN -> type(YIELD_RETURN);

Directive_BOOL: BOOL -> type(BOOL);
Directive_BYTE: BYTE -> type(UINT8);
Directive_SBYTE: SBYTE -> type(INT8);
Directive_SHORT: SHORT -> type(INT16);
Directive_USHORT: USHORT -> type(UINT16);
Directive_LONG: LONG -> type(INT64);
Directive_ULONG: ULONG -> type(UINT64);
Directive_INT: INT -> type(INT);
Directive_INT8: INT8 -> type(INT8);
Directive_INT16: INT16 -> type(INT16);
Directive_INT32: INT32 -> type(INT32);
Directive_INT64: INT64 -> type(INT64);
Directive_INT128: INT128 -> type(INT128);
Directive_NATIVEINT: NATIVEINT -> type(NATIVEINT);
Directive_UINT: UINT -> type(UINT);
Directive_UINT8: UINT8 -> type(UINT8);
Directive_UINT16: UINT16 -> type(UINT16);
Directive_UINT32: UINT32 -> type(UINT32);
Directive_UINT64: UINT64 -> type(UINT64);
Directive_UINT128: UINT128 -> type(UINT128);
Directive_UNATIVEINT: UNATIVEINT -> type(UNATIVEINT);
Directive_BIGINT: BIGINT -> type(BIGINT);
Directive_FLOAT: FLOAT -> type(FLOAT);
Directive_FLOAT16: FLOAT16 -> type(FLOAT16);
Directive_FLOAT32: FLOAT32 -> type(FLOAT32);
Directive_FLOAT64: FLOAT64 -> type(FLOAT64);
Directive_HALF: HALF -> type(FLOAT16);
Directive_SINGLE: SINGLE -> type(FLOAT32);
Directive_DOUBLE: DOUBLE -> type(FLOAT64);
Directive_DECIMAL: DECIMAL -> type(DECIMAL);
Directive_CHAR: CHAR -> type(CHAR);
Directive_STRING: STRING -> type(STRING);
Directive_OBJECT: OBJECT -> type(OBJECT);
Directive_VOID: VOID -> type(VOID);

Directive_UNDERSCORE: UNDERSCORE -> type(UNDERSCORE);

Directive_SEMICOLON: SEMICOLON -> type(SEMICOLON);
Directive_EQ: EQ -> type(EQ);
Directive_NEQ: NEQ -> type(NEQ);
Directive_NEQ_ALT: NEQ_ALT -> type(NEQ_ALT);
Directive_COMMA: COMMA ((LINE_WHITESPACE | NEWLINE_ESCAPE)* EOL)? -> type(COMMA);
Directive_DOLLAR: DOLLAR -> type(DOLLAR);
// no Directive_HASH
Directive_OPENP: OPENP -> type(OPENP), pushMode(DEFAULT_MODE);
Directive_CLOSEP: CLOSEP -> type(CLOSEP), popMode;
Directive_OPENSP: OPENSP -> type(OPENSP), pushMode(DEFAULT_MODE);
Directive_CLOSESP: CLOSESP -> type(CLOSESP), popMode;
Directive_OPENB: OPENB -> type(OPENB), pushMode(DEFAULT_MODE);
Directive_CLOSEB: CLOSEB -> type(CLOSEB), popMode;
Directive_PLUS: PLUS -> type(PLUS);
Directive_MINUS: MINUS -> type(MINUS);
Directive_ASTERISK: ASTERISK -> type(ASTERISK);
Directive_SLASH: SLASH -> type(SLASH);
Directive_PERCENT: PERCENT -> type(PERCENT);
Directive_CONCAT: CONCAT -> type(CONCAT);
Directive_LT: LT -> type(LT);
Directive_LTE: LTE -> type(LTE);
Directive_GT: GT -> type(GT);
Directive_ASSIGN: ASSIGN -> type(ASSIGN);
Directive_QUESTION: QUESTION -> type(QUESTION);
Directive_DOUBLE_AND: DOUBLE_AND -> type(DOUBLE_AND);
Directive_DOUBLE_OR: DOUBLE_OR -> type(DOUBLE_OR);
Directive_SINGLE_AND: SINGLE_AND -> type(SINGLE_AND);
Directive_SINGLE_OR: SINGLE_OR -> type(SINGLE_OR);
Directive_SINGLE_XOR: SINGLE_XOR -> type(SINGLE_XOR);
Directive_LSHIFT: LSHIFT -> type(LSHIFT);
Directive_EXCLAMATION: EXCLAMATION -> type(EXCLAMATION);
Directive_TILDE: TILDE -> type(TILDE);
Directive_COLON: COLON -> type(COLON);
Directive_DOT: DOT -> type(DOT);

Directive_NAME: NAME -> type(NAME);

mode PragmaDirective;

END_PRAGMA:
  ('#' | EOL | EOF) -> channel(Pragma), mode(DEFAULT_MODE);

PragmaDirective_INT_LITERAL: INT_LITERAL -> type(INT_LITERAL), channel(Pragma);
PragmaDirective_FLOAT_LITERAL: FLOAT_LITERAL -> type(FLOAT_LITERAL), channel(Pragma);
PragmaDirective_EXP_LITERAL: EXP_LITERAL -> type(EXP_LITERAL), channel(Pragma);
PragmaDirective_HEX_LITERAL: HEX_LITERAL -> type(HEX_LITERAL), channel(Pragma);
PragmaDirective_STRING_LITERAL: STRING_LITERAL -> type(STRING_LITERAL), channel(Pragma);
PragmaDirective_VERBATIM_STRING_LITERAL: VERBATIM_STRING_LITERAL -> type(VERBATIM_STRING_LITERAL), channel(Pragma);
PragmaDirective_CHAR_LITERAL: CHAR_LITERAL -> type(CHAR_LITERAL), channel(Pragma);

PragmaDirective_COMMENT: COMMENT -> skip;
PragmaDirective_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
PragmaDirective_LINE_COMMENT: LINE_COMMENT -> skip;
PragmaDirective_WHITESPACE: (WHITESPACE | NEWLINE_ESCAPE) -> skip;

PragmaDirective_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME), channel(Pragma);

PragmaDirective_NAME: NAME -> type(NAME), channel(Pragma);

mode Char;

fragment ESCAPE_BASIC:
  '\\' ([abfnrtv\\"'] | DIGIT DIGIT DIGIT | 'x' HEXDIGIT HEXDIGIT | 'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT | 'U' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT);

CHAR_PART: ~['\\\r\n] | ESCAPE_BASIC;
CHAR_BAD_TWO_CHARACTERS: CHAR_PART CHAR_PART -> type(ERROR);
LITERAL_NEWLINE: EOL;
LITERAL_ESCAPE_NEWLINE: NEWLINE_ESCAPE;
LITERAL_BAD_ESCAPE: '\\' . -> type(ERROR);

END_CHAR: '\'' -> popMode;
END_CHAR_SUFFIX: END_CHAR NAME -> popMode;

mode String;

STRING_PART: (~["\\\r\n] | ESCAPE_BASIC)+;
String_LITERAL_NEWLINE: LITERAL_NEWLINE -> type(LITERAL_NEWLINE);
String_LITERAL_ESCAPE_NEWLINE: LITERAL_ESCAPE_NEWLINE -> type(LITERAL_ESCAPE_NEWLINE);
String_LITERAL_BAD_ESCAPE: LITERAL_BAD_ESCAPE -> type(ERROR);

END_STRING: '"' -> popMode;
END_STRING_SUFFIX: END_STRING NAME -> popMode;

mode VerbatimString;

Verbatim_STRING_PART: ('""' | ~'"')+ -> type(STRING_PART);

Verbatim_END_STRING: END_STRING -> type(END_STRING), popMode;
Verbatim_END_STRING_SUFFIX: END_STRING_SUFFIX -> type(END_STRING_SUFFIX), popMode;

mode InterpolatedString;

Interpolated_STRING_PART: (ESCAPE_BASIC | '{{' | '}}' | ~[\\"{}%\r\n])+ -> type(STRING_PART);
InterpolatedString_LITERAL_NEWLINE: LITERAL_NEWLINE -> type(LITERAL_NEWLINE);
InterpolatedString_LITERAL_ESCAPE_NEWLINE: LITERAL_ESCAPE_NEWLINE -> type(LITERAL_ESCAPE_NEWLINE);
InterpolatedString_LITERAL_BAD_ESCAPE: LITERAL_BAD_ESCAPE -> type(ERROR);

InterpolatedString_OPENB: OPENB -> type(OPENB), pushMode(Interpolation);
InterpolatedString_PERCENT: PERCENT -> type(PERCENT);

Interpolated_END_STRING: END_STRING -> type(END_STRING), popMode;
Interpolated_END_STRING_SUFFIX: END_STRING_SUFFIX -> type(END_STRING_SUFFIX), popMode;

mode VerbatimInterpolatedString;

VerbatimInterpolated_STRING_PART: ('{{' | '}}' | '""' | ~["{}%])+ -> type(STRING_PART);

VerbatimInterpolatedString_OPENB: OPENB -> type(OPENB), pushMode(Interpolation);
VerbatimInterpolatedString_PERCENT: PERCENT -> type(PERCENT);

VerbatimInterpolated_END_STRING: END_STRING -> type(END_STRING), popMode;
VerbatimInterpolated_END_STRING_SUFFIX: END_STRING_SUFFIX -> type(END_STRING_SUFFIX), popMode;

mode Interpolation;

// Not valid in non-string format.
fragment INTERP_NON_DELIMITER:
  ~[{}()"'\][\\\r\n];
fragment INTERP_NON_DELIMITER_ALPHA:
  ~[{}()"'\][\\\r\na-zA-Z_];

// F# format specifiers with at least one optional component used.
INTERP_FORMAT_GENERAL:
  ':'
  (
    GENERAL_FLAGS GENERAL_WIDTH? GENERAL_PRECISION? |
    GENERAL_WIDTH GENERAL_PRECISION? |
    GENERAL_PRECISION
  )
  [a-zA-Z] -> mode(InterpolationEnd);

fragment GENERAL_FLAGS:
  [-+0 #]+;

fragment GENERAL_WIDTH:
  [1-9][0-9]*;

fragment GENERAL_PRECISION:
  '.' INTERP_NON_DELIMITER_ALPHA*;

// A valid string (must not be a single letter).
INTERP_FORMAT_CUSTOM:
  ':' WHITESPACE* (STRING_LITERAL | VERBATIM_STRING_LITERAL);
  
// A valid character (must be a letter).
INTERP_FORMAT_STANDARD:
  ':' WHITESPACE* CHAR_LITERAL;

INTERP_FORMAT_NUMBER:
  ':' (STANDARD_NUM_FORMAT | CUSTOM_NUM_FORMAT) -> mode(InterpolationEnd);

INTERP_FORMAT_BEGIN_COMPONENTS:
  COLON -> mode(InterpolationComponents);

fragment INTERP_FORMAT_ESCAPE:
  '\\' ('\\' | INTERP_NON_DELIMITER) | '\'' ~['\\\r\n]* '\'';

fragment INTERP_FORMAT_LITERAL:
  LINE_WHITESPACE | [-+_];

fragment STANDARD_NUM_FORMAT:
  [bBcCdDeEfFgGnNpPrRxX] '0'* [1-9]? [0-9];

fragment CUSTOM_NUM_FORMAT:
  CUSTOM_NUM_SECTION (';' CUSTOM_NUM_SECTION (';' CUSTOM_NUM_SECTION)?)?;

fragment CUSTOM_NUM_LITERAL:
  (INTERP_FORMAT_ESCAPE | INTERP_FORMAT_LITERAL)*;

fragment CUSTOM_NUM_SECTION:
  CUSTOM_NUM_LITERAL (
    [%\u2030] CUSTOM_NUM_LITERAL CUSTOM_NUM_DIGITS |
    CUSTOM_NUM_DIGITS (CUSTOM_NUM_LITERAL [%\u2030])?
  ) CUSTOM_NUM_LITERAL;

fragment CUSTOM_NUM_DIGITS:
  // Preceding # groups
  (('#' CUSTOM_NUM_LITERAL)+ ',' CUSTOM_NUM_LITERAL)*
  (
    // Preceding mixed or 0 groups
    (('#' CUSTOM_NUM_LITERAL)+ ('0' CUSTOM_NUM_LITERAL)+ ',' CUSTOM_NUM_LITERAL)?
    // Preceding 0 groups
    (('0' CUSTOM_NUM_LITERAL)+ ',' CUSTOM_NUM_LITERAL)*
    // Core digits
    ('0' CUSTOM_NUM_LITERAL)* '0' |
    ('#' CUSTOM_NUM_LITERAL)* '#' (CUSTOM_NUM_LITERAL ('0' CUSTOM_NUM_LITERAL)* '0')?
  )
  (
    // Decimal point
    CUSTOM_NUM_LITERAL ','*
    (
      ',' |
      '.'
      // Decimal digits
      CUSTOM_NUM_LITERAL
      (
        // Tenth digit
        '#' | '0'
        // 0 decimal digits
        (CUSTOM_NUM_LITERAL '0')*
      )
      // # decimal digits
      (CUSTOM_NUM_LITERAL '#')*
    )
  )?
  // Exponent
  (CUSTOM_NUM_LITERAL [eE] [-+]? '0'+)?;

INTERP_ALIGNMENT:
  ',' WHITESPACE* ('+' | '-')? INT_LITERAL;

Interpolation_INT_LITERAL: INT_LITERAL -> type(INT_LITERAL);
Interpolation_FLOAT_LITERAL: FLOAT_LITERAL -> type(FLOAT_LITERAL);
Interpolation_EXP_LITERAL: EXP_LITERAL -> type(EXP_LITERAL);
Interpolation_HEX_LITERAL: HEX_LITERAL -> type(HEX_LITERAL);
Interpolation_INT_SUFFIX: INT_SUFFIX -> type(INT_SUFFIX);
Interpolation_FLOAT_SUFFIX: FLOAT_SUFFIX -> type(FLOAT_SUFFIX);
Interpolation_EXP_SUFFIX: EXP_SUFFIX -> type(EXP_SUFFIX);
Interpolation_HEX_SUFFIX: HEX_SUFFIX -> type(HEX_SUFFIX);
Interpolation_BEGIN_STRING: BEGIN_STRING -> type(BEGIN_STRING), pushMode(String);
Interpolation_BEGIN_VERBATIM_STRING: BEGIN_VERBATIM_STRING -> type(BEGIN_VERBATIM_STRING), pushMode(VerbatimString);
Interpolation_BEGIN_INTERPOLATED_STRING: BEGIN_INTERPOLATED_STRING -> type(BEGIN_INTERPOLATED_STRING), pushMode(InterpolatedString);
Interpolation_BEGIN_VERBATIM_INTERPOLATED_STRING: BEGIN_VERBATIM_INTERPOLATED_STRING -> type(BEGIN_VERBATIM_INTERPOLATED_STRING), pushMode(VerbatimInterpolatedString);
Interpolation_BEGIN_CHAR: BEGIN_CHAR -> type(BEGIN_CHAR), pushMode(Char);

Interpolation_COMMENT: COMMENT -> skip;
Interpolation_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
Interpolation_LINE_COMMENT: LINE_COMMENT -> skip;

Interpolation_WHITESPACE: WHITESPACE -> skip;

Interpolation_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME);
Interpolation_MEMBER_NAME: MEMBER_NAME -> type(MEMBER_NAME);
// no Interpolation_DYNAMIC_MEMBER_NAME

Interpolation_ABSTRACT: ABSTRACT -> type(ABSTRACT);
Interpolation_ADD: ADD -> type(ADD);
Interpolation_AND: AND -> type(AND);
Interpolation_AS: AS -> type(AS);
Interpolation_BASE: BASE -> type(BASE);
Interpolation_BREAK: BREAK -> type(BREAK);
Interpolation_BY: BY -> type(BY);
Interpolation_CASE: CASE -> type(CASE);
Interpolation_CATCH: CATCH -> type(CATCH);
Interpolation_CLASS: CLASS -> type(CLASS);
Interpolation_CONST: CONST -> type(CONST);
Interpolation_CONTINUE: CONTINUE -> type(CONTINUE);
Interpolation_DEFAULT: DEFAULT -> type(DEFAULT);
Interpolation_DEFER: DEFER -> type(DEFER);
Interpolation_DELEGATE: DELEGATE -> type(DELEGATE);
Interpolation_DELETE: DELETE -> type(DELETE);
Interpolation_DO: DO -> type(DO);
Interpolation_ECHO: ECHO -> type(ECHO);
Interpolation_ELSE: ELSE -> type(ELSE);
Interpolation_ELSEIF: ELSEIF -> type(ELSEIF);
Interpolation_END: END -> type(END);
Interpolation_ENUM: ENUM -> type(ENUM);
Interpolation_EVENT: EVENT -> type(EVENT);
Interpolation_EXCEPTION: EXCEPTION -> type(EXCEPTION);
Interpolation_EXPLICIT: EXPLICIT -> type(EXPLICIT);
Interpolation_EXTENSION: EXTENSION -> type(EXTENSION);
Interpolation_EXTERN: EXTERN -> type(EXTERN);
Interpolation_FALSE: FALSE -> type(FALSE);
Interpolation_FINALLY: FINALLY -> type(FINALLY);
Interpolation_FIXED: FIXED -> type(FIXED);
Interpolation_FOLLOW: FOLLOW -> type(FOLLOW);
Interpolation_FOR: FOR -> type(FOR);
Interpolation_FUNCTION: FUNCTION -> type(FUNCTION);
Interpolation_GET: GET -> type(GET);
Interpolation_GLOBAL: GLOBAL -> type(GLOBAL);
Interpolation_GOTO: GOTO -> type(GOTO);
Interpolation_IF: IF -> type(IF);
Interpolation_IMPLICIT: IMPLICIT -> type(IMPLICIT);
Interpolation_IMPORT: IMPORT -> type(IMPORT);
Interpolation_IN: IN -> type(IN);
Interpolation_INCLUDE: INCLUDE -> type(INCLUDE);
Interpolation_INHERIT: INHERIT -> type(INHERIT);
Interpolation_INLINE: INLINE -> type(INLINE);
Interpolation_INTERFACE: INTERFACE -> type(INTERFACE);
Interpolation_INTERNAL: INTERNAL -> type(INTERNAL);
Interpolation_INTO: INTO -> type(INTO);
Interpolation_IS: IS -> type(IS);
Interpolation_LAZY: LAZY -> type(LAZY);
Interpolation_LET: LET -> type(LET);
Interpolation_LOCK: LOCK -> type(LOCK);
Interpolation_MODULE: MODULE -> type(MODULE);
Interpolation_NAMEOF: NAMEOF -> type(NAMEOF);
Interpolation_NAMESPACE: NAMESPACE -> type(NAMESPACE);
Interpolation_NARROW: NARROW -> type(NARROW);
Interpolation_NEW: NEW -> type(NEW);
Interpolation_NONE: NONE -> type(NONE);
Interpolation_NOT: NOT -> type(NOT);
Interpolation_NULL: NULL -> type(NULL);
Interpolation_ON: ON -> type(ON);
Interpolation_OPERATOR: OPERATOR -> type(OPERATOR);
Interpolation_OR: OR -> type(OR);
Interpolation_OUT: OUT -> type(OUT);
Interpolation_OVERRIDE: OVERRIDE -> type(OVERRIDE);
Interpolation_PACKAGE: PACKAGE -> type(PACKAGE);
Interpolation_PARTIAL: PARTIAL -> type(PARTIAL);
Interpolation_PRIVATE: PRIVATE -> type(PRIVATE);
Interpolation_PROTECTED: PROTECTED -> type(PROTECTED);
Interpolation_PUBLIC: PUBLIC -> type(PUBLIC);
Interpolation_RAISE: RAISE -> type(RAISE);
Interpolation_RECORD: RECORD -> type(RECORD);
Interpolation_REF: REF -> type(REF);
Interpolation_REMOVE: REMOVE -> type(REMOVE);
Interpolation_REPEAT: REPEAT -> type(REPEAT);
Interpolation_REQUIRE: REQUIRE -> type(REQUIRE);
Interpolation_RETURN: RETURN -> type(RETURN);
Interpolation_SEALED: SEALED -> type(SEALED);
Interpolation_SET: SET -> type(SET);
Interpolation_SIZEOF: SIZEOF -> type(SIZEOF);
Interpolation_SOME: SOME -> type(SOME);
Interpolation_STATIC: STATIC -> type(STATIC);
Interpolation_STRUCT: STRUCT -> type(STRUCT);
Interpolation_SWITCH: SWITCH -> type(SWITCH);
Interpolation_THEN: THEN -> type(THEN);
Interpolation_THROW: THROW -> type(THROW);
Interpolation_TRAIT: TRAIT -> type(TRAIT);
Interpolation_TRUE: TRUE -> type(TRUE);
Interpolation_TRY: TRY -> type(TRY);
Interpolation_TYPEOF: TYPEOF -> type(TYPEOF);
Interpolation_UNION: UNION -> type(UNION);
Interpolation_UNIT: UNIT -> type(UNIT);
Interpolation_UNTIL: UNTIL -> type(UNTIL);
Interpolation_USE: USE -> type(USE);
Interpolation_VAR: VAR -> type(VAR);
Interpolation_VIRTUAL: VIRTUAL -> type(VIRTUAL);
Interpolation_WHEN: WHEN -> type(WHEN);
Interpolation_WHILE: WHILE -> type(WHILE);
Interpolation_WHILE_TRUE_DO: WHILE_TRUE_DO -> type(WHILE_TRUE_DO);
Interpolation_WIDEN: WIDEN -> type(WIDEN);
Interpolation_WITH: WITH -> type(WITH);
Interpolation_YIELD: YIELD -> type(YIELD);
Interpolation_YIELD_BREAK: YIELD_BREAK -> type(YIELD_BREAK);
Interpolation_YIELD_RETURN: YIELD_RETURN -> type(YIELD_RETURN);

Interpolation_BOOL: BOOL -> type(BOOL);
Interpolation_BYTE: BYTE -> type(UINT8);
Interpolation_SBYTE: SBYTE -> type(INT8);
Interpolation_SHORT: SHORT -> type(INT16);
Interpolation_USHORT: USHORT -> type(UINT16);
Interpolation_LONG: LONG -> type(INT64);
Interpolation_ULONG: ULONG -> type(UINT64);
Interpolation_INT: INT -> type(INT);
Interpolation_INT8: INT8 -> type(INT8);
Interpolation_INT16: INT16 -> type(INT16);
Interpolation_INT32: INT32 -> type(INT32);
Interpolation_INT64: INT64 -> type(INT64);
Interpolation_INT128: INT128 -> type(INT128);
Interpolation_NATIVEINT: NATIVEINT -> type(NATIVEINT);
Interpolation_UINT: UINT -> type(UINT);
Interpolation_UINT8: UINT8 -> type(UINT8);
Interpolation_UINT16: UINT16 -> type(UINT16);
Interpolation_UINT32: UINT32 -> type(UINT32);
Interpolation_UINT64: UINT64 -> type(UINT64);
Interpolation_UINT128: UINT128 -> type(UINT128);
Interpolation_UNATIVEINT: UNATIVEINT -> type(UNATIVEINT);
Interpolation_BIGINT: BIGINT -> type(BIGINT);
Interpolation_FLOAT: FLOAT -> type(FLOAT);
Interpolation_FLOAT16: FLOAT32 -> type(FLOAT16);
Interpolation_FLOAT32: FLOAT32 -> type(FLOAT32);
Interpolation_FLOAT64: FLOAT64 -> type(FLOAT64);
Interpolation_HALF: HALF -> type(FLOAT16);
Interpolation_SINGLE: SINGLE -> type(FLOAT32);
Interpolation_DOUBLE: DOUBLE -> type(FLOAT64);
Interpolation_DECIMAL: DECIMAL -> type(DECIMAL);
Interpolation_CHAR: CHAR -> type(CHAR);
Interpolation_STRING: STRING -> type(STRING);
Interpolation_OBJECT: OBJECT -> type(OBJECT);
Interpolation_VOID: VOID -> type(VOID);

Interpolation_UNDERSCORE: UNDERSCORE -> type(UNDERSCORE);

Interpolation_SEMICOLON: SEMICOLON -> type(SEMICOLON);
Interpolation_EQ: EQ -> type(EQ);
Interpolation_NEQ: NEQ -> type(NEQ);
Interpolation_NEQ_ALT: NEQ_ALT -> type(NEQ_ALT);
// no Interpolation_COMMA
Interpolation_DOLLAR: DOLLAR -> type(DOLLAR);
Interpolation_HASH: HASH -> type(HASH);
Interpolation_OPENP: OPENP -> type(OPENP), pushMode(DEFAULT_MODE);
Interpolation_CLOSEP: CLOSEP -> type(CLOSEP), popMode;
Interpolation_OPENSP: OPENSP -> type(OPENSP), pushMode(DEFAULT_MODE);
Interpolation_CLOSESP: CLOSESP -> type(CLOSESP), popMode;
Interpolation_OPENB: OPENB -> type(OPENB), pushMode(DEFAULT_MODE);
Interpolation_CLOSEB: CLOSEB -> type(CLOSEB), popMode;
Interpolation_PLUS: PLUS -> type(PLUS);
Interpolation_MINUS: MINUS -> type(MINUS);
Interpolation_ASTERISK: ASTERISK -> type(ASTERISK);
Interpolation_SLASH: SLASH -> type(SLASH);
Interpolation_PERCENT: PERCENT -> type(PERCENT);
Interpolation_CONCAT: CONCAT -> type(CONCAT);
Interpolation_LT: LT -> type(LT);
Interpolation_LTE: LTE -> type(LTE);
Interpolation_GT: GT -> type(GT);
Interpolation_ASSIGN: ASSIGN -> type(ASSIGN);
Interpolation_QUESTION: QUESTION -> type(QUESTION);
Interpolation_DOUBLE_AND: DOUBLE_AND -> type(DOUBLE_AND);
Interpolation_DOUBLE_OR: DOUBLE_OR -> type(DOUBLE_OR);
Interpolation_SINGLE_AND: SINGLE_AND -> type(SINGLE_AND);
Interpolation_SINGLE_OR: SINGLE_OR -> type(SINGLE_OR);
Interpolation_SINGLE_XOR: SINGLE_XOR -> type(SINGLE_XOR);
Interpolation_LSHIFT: LSHIFT -> type(LSHIFT);
Interpolation_EXCLAMATION: EXCLAMATION -> type(EXCLAMATION);
Interpolation_TILDE: TILDE -> type(TILDE);
// no Interpolation_COLON
Interpolation_DOT: DOT -> type(DOT);

Interpolation_NAME: NAME -> type(NAME);

mode InterpolationComponents;

INTERP_COMPONENTS_PART_SHORT:
  [a-zA-Z];

INTERP_COMPONENTS_PART_LONG:
  INTERP_FORMAT_ESCAPE | (INTERP_FORMAT_LITERAL)+ |
  '%' [a-zA-Z] | COLON | SLASH |
  'a'+ | 'b'+ | 'c'+ | 'd'+ | 'e'+ | 'f'+ | 'g'+ |
  'h'+ | 'i'+ | 'j'+ | 'k'+ | 'l'+ | 'm'+ | 'n'+ |
  'o'+ | 'p'+ | 'q'+ | 'r'+ | 's'+ | 't'+ | 'u'+ |
  'v'+ | 'w'+ | 'x'+ | 'y'+ | 'z'+ |
  'A'+ | 'B'+ | 'C'+ | 'D'+ | 'E'+ | 'F'+ | 'G'+ |
  'H'+ | 'I'+ | 'J'+ | 'K'+ | 'L'+ | 'M'+ | 'N'+ |
  'O'+ | 'P'+ | 'Q'+ | 'R'+ | 'S'+ | 'T'+ | 'U'+ |
  'V'+ | 'W'+ | 'X'+ | 'Y'+ | 'Z'+;

InterpolationComponents_CLOSEB: CLOSEB -> type(CLOSEB), popMode;

InterpolationComponents_ERROR: . -> type(ERROR);

mode InterpolationEnd;

InterpolationEnd_CLOSEB: CLOSEB -> type(CLOSEB), popMode;

mode Empty;

// Matches everything with highest priority
ERROR: (~'\u0000'+ | '\u0000'+);

// Helper tokens that are never matched:

STRING_LITERAL:
  BEGIN_STRING STRING_PART* END_STRING;

VERBATIM_STRING_LITERAL:
  BEGIN_VERBATIM_STRING Verbatim_STRING_PART* Verbatim_END_STRING;

CHAR_LITERAL:
  BEGIN_CHAR CHAR_PART END_CHAR;

GTE: '>=';
RSHIFT: '>>';
DOUBLE_QUESTION: '??';

mode InlineDirective;

InlineDirective_FS_LITERAL: '"' [fF] ('#' | [sS] [hH] [aA] [rR] [pP]) '"' -> type(STRING_LITERAL), mode(FS);
InlineDirective_VERBATIM_FS_LITERAL: '@' InlineDirective_FS_LITERAL -> type(VERBATIM_STRING_LITERAL), mode(FS);

InlineDirective_JS_LITERAL: '"' [jJ] ([sS] | [aA] [vV] [aA] [sS] [cC] [rR] [iI] [pP] [tT]) '"' -> type(STRING_LITERAL), mode(JSBegin);
InlineDirective_VERBATIM_JS_LITERAL: '@' InlineDirective_JS_LITERAL -> type(VERBATIM_STRING_LITERAL), mode(JSBegin);

InlineDirective_STRING_LITERAL: STRING_LITERAL -> type(STRING_LITERAL), mode(InlineUnknown);
InlineDirective_VERBATIM_STRING_LITERAL: VERBATIM_STRING_LITERAL -> type(VERBATIM_STRING_LITERAL), mode(InlineUnknown);

InlineDirective_COMMENT: COMMENT -> skip;
InlineDirective_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
InlineDirective_LINE_COMMENT: LINE_COMMENT -> skip;
InlineDirective_WHITESPACE: (WHITESPACE | NEWLINE_ESCAPE) -> type(WHITESPACE);

mode InlineUnknown;

/* Unrecognized language */

END_INLINE_SOURCE:
  '#endinline' ' '? -> mode(Directive);

InlineUnknown_ERROR:
  ('#' | ~[#]+) -> type(ERROR);

mode FS;

/* F# lite mode */

fragment FS_ESCAPE:
  '\\' .;

// Special-handled tokens that do not form delimiters in code and comments
fragment FS_TOKEN:
  '(*)' |
  '@"' ('""' | ~'"')* '"' |
  '"""' (~'"' | '"' ~'"' | '""' ~'"')* '"""' |
  '"' (FS_ESCAPE | ~["\\])* '"' |
  '\'' '\\'? ["{}] '\'';

FS_BEGIN_TRIPLE_INTERPOLATED_STRING_N:
  '$$$$"""' -> type(ERROR);
 
FS_BEGIN_TRIPLE_INTERPOLATED_STRING_3:
  '$$$"""' -> type(FS_PART), pushMode(FSTripleInterpolatedString3);
 
FS_BEGIN_TRIPLE_INTERPOLATED_STRING_2:
  '$$"""' -> type(FS_PART), pushMode(FSTripleInterpolatedString2);
 
FS_BEGIN_TRIPLE_INTERPOLATED_STRING_1:
  '$"""' -> type(FS_PART), pushMode(FSTripleInterpolatedString1);

FS_BEGIN_INTERPOLATED_STRING:
  '$"' -> type(FS_PART), pushMode(FSInterpolatedString);

FS_BEGIN_VERBATIM_INTERPOLATED_STRING:
  ('$@' | '@$') '"' -> type(FS_PART), pushMode(FSVerbatimInterpolatedString);

// Parts of regular source code
FS_PART:
  FS_TOKEN |
  '``' (~[`\r\n] | '`' ~[`\r\n])* '``' |
  ~[("'@$/` \r\n{}]+ |
  ~[ \r\n];

FS_COMMENT:
  '//' ~[\n\r]*;

FS_WHITESPACE:
  ' '+;

FS_END_INLINE_SOURCE:
  EOL END_INLINE_SOURCE -> type(END_INLINE_SOURCE), mode(Directive);

FS_DIRECTIVE:
  EOL '#' (LOWERCASE | UPPERCASE | DIGIT | '_' | '\'' | UNICODE)*;

FS_EOL:
  EOL;

FS_BEGIN_BLOCK_COMMENT:
  '(*' -> pushMode(FSComment);

mode FSComment;

FS_END_BLOCK_COMMENT:
  '*)' -> popMode;

FSComment_PART:
  (FS_TOKEN | ~[("'@*\r\n]+ | ~[\r\n]) -> type(FS_COMMENT);

FSComment_EOL:
  FS_EOL -> type(FS_EOL);

FSComment_BEGIN_BLOCK_COMMENT:
  FS_BEGIN_BLOCK_COMMENT -> pushMode(FSComment);

mode FSInterpolatedString;

FSInterpolatedString_PART:
  (('{{' | '\\' ~'{' | ~["{\\])+ | '\\') -> type(FS_PART);

FS_END_INTERPOLATED_STRING:
  '"' -> type(FS_PART), popMode;

FS_BEGIN_INTERPOLATION:
  OPENB -> type(FS_PART), pushMode(FSInterpolation);

mode FSVerbatimInterpolatedString;

FSVerbatimInterpolatedString_PART:
  ('{{' | '""' | ["{])+ -> type(FS_PART);

FSVerbatim_END_INTERPOLATED_STRING:
  '"' -> type(FS_PART), popMode;

FSVerbatim_BEGIN_INTERPOLATION:
  OPENB -> type(FS_PART), pushMode(FSInterpolation);

mode FSTripleInterpolatedString1;

FSTripleInterpolatedString_PART_1:
  ('{{' | '"' ~["{] | '""' ~["{] | ["{])+ -> type(FS_PART);

FSTriple_END_INTERPOLATED_STRING_1:
  '"""' -> type(FS_PART), popMode;

FSTriple_BEGIN_INTERPOLATION_1:
  ('"' '"'?)? '{' -> type(FS_PART), pushMode(FSInterpolation);

mode FSTripleInterpolatedString2;

FSTripleInterpolatedString_PART_2:
  (('"' ~["{] | '""' ~["{] | ["{])+ | '{') -> type(FS_PART);

FSTriple_END_INTERPOLATED_STRING_2:
  '"""' -> type(FS_PART), popMode;

FSTriple_BEGIN_INTERPOLATION_2:
  ('"' '"'?)? '{{' '{'* -> type(FS_PART), pushMode(FSInterpolation), pushMode(FSInterpolation);

mode FSTripleInterpolatedString3;
// $$$$""" not supported

FSTripleInterpolatedString_PART_3:
  (('"' ~["{] | '""' ~["{] | ["{])+ | '{' '{'?) -> type(FS_PART);

FSTriple_END_INTERPOLATED_STRING_3:
  '"""' -> type(FS_PART), popMode;

FSTriple_BEGIN_INTERPOLATION_3:
  ('"' '"'?)? '{{{' '{'* -> type(FS_PART), pushMode(FSInterpolation), pushMode(FSInterpolation), pushMode(FSInterpolation);

mode FSInterpolation;

FSInterpolation_OPENB:
  OPENB -> type(FS_PART), pushMode(FSInterpolation);

FSInterpolation_CLOSEB:
  CLOSEB -> type(FS_PART), popMode;

FSInterpolation_BEGIN_TRIPLE_INTERPOLATED_STRING_N:
  FS_BEGIN_TRIPLE_INTERPOLATED_STRING_N -> type(FS_BEGIN_TRIPLE_INTERPOLATED_STRING_N);
 
FSInterpolation_BEGIN_TRIPLE_INTERPOLATED_STRING_3:
  FS_BEGIN_TRIPLE_INTERPOLATED_STRING_3 -> type(FS_PART), pushMode(FSTripleInterpolatedString3);
 
FSInterpolation_BEGIN_TRIPLE_INTERPOLATED_STRING_2:
  FS_BEGIN_TRIPLE_INTERPOLATED_STRING_2 -> type(FS_PART), pushMode(FSTripleInterpolatedString2);
 
FSInterpolation_BEGIN_TRIPLE_INTERPOLATED_STRING_1:
  FS_BEGIN_TRIPLE_INTERPOLATED_STRING_1 -> type(FS_PART), pushMode(FSTripleInterpolatedString1);

FSInterpolation_BEGIN_INTERPOLATED_STRING:
  FS_BEGIN_INTERPOLATED_STRING -> type(FS_PART), pushMode(FSInterpolatedString);

FSInterpolation_BEGIN_VERBATIM_INTERPOLATED_STRING:
  FS_BEGIN_VERBATIM_INTERPOLATED_STRING -> type(FS_PART), pushMode(FSVerbatimInterpolatedString);

FSInterpolation_PART: FS_PART -> type(FS_PART);
FSInterpolation_WHITESPACE: FS_WHITESPACE -> type(FS_WHITESPACE);
FSInterpolation_END_INLINE_SOURCE: END_INLINE_SOURCE -> type(ERROR);
FSInterpolation_DIRECTIVE: FS_DIRECTIVE -> type(FS_DIRECTIVE);

FSInterpolation_EOL: FS_EOL -> type(FS_EOL);

FSInterpolation_BEGIN_BLOCK_COMMENT:
  FS_BEGIN_BLOCK_COMMENT -> type(FS_PART), pushMode(FSComment);

mode JS;

/* JavaScript lite mode */

fragment JS_ESCAPE:
  '\\' .;

fragment JS_EOL:
  [\n\r\u2028\u2029]+;

fragment JS_KEYWORD:
  // Ignoring true, false, null, super, this because they form a single expression
  'break' | 'case' | 'catch' | 'class' | 'const' | 'continue' |
  'debugger' | 'default' | 'delete' | 'do' | 'else' | 'export' |
  'extends' | 'finally' | 'for' | 'function' | 'if' | 'import' |
  'in' | 'instanceof' | 'new' | 'return' | 'switch' | 'throw' |
  'try' | 'typeof' | 'var' | 'void' | 'while' | 'with' | 'enum';

fragment JS_SOFT_KEYWORD:
  // Not reserved in certain situations
  'let' | 'static' | 'yield' | 'await' | 'implements' |
  'interface' | 'package' | 'private' | 'protected' |
  'public' | 'of';

fragment JS_BEGIN_REGEXP:
  // Must appear in combination with constructs that do not end an expression
  JS_WHITESPACE* (('++' | '--') JS_WHITESPACE*)* '/';

fragment JS_COMMENT_CONTENT:
  // Already after `/`
  '/' ~[\n\r\u2028\u2029]* |
  '*' (~[*] | '*' ~[/])* '*'+ '/';

// There are no parser-specific rules for JS; only JS_PART and END_INLINE_SOURCE must be emitted.

JS_WHITESPACE:
  // Priority over JS_PART_BEGIN_REGEXP to prevent `//` being matched
  (
    [ \t\u000B\f\u00A0\uFEFF]+ |
    // No need to take semicolon insertion into account
    JS_EOL |
    '/' JS_COMMENT_CONTENT
  ) -> type(JS_PART);

JS_BEGIN_TEMPLATE:
  '`' -> type(JS_PART), pushMode(JSTemplate);

JS_BEGIN_BLOCK_REGEXP:
  '{' JS_BEGIN_REGEXP -> type(JS_PART), pushMode(JSBlock), pushMode(JSRegexpBegin);

JS_BEGIN_BLOCK:
  '{' -> type(JS_PART), pushMode(JSBlock);

JS_END_BLOCK:
  // When not in block, abandon lexing
  '}' -> type(JS_PART), mode(InlineUnknown);

JS_PART_BEGIN_REGEXP:
  // An operator or keyword followed by / must indicate a regexp start
  (~[\])}a-zA-Z0-9_$. \t\u000B\f\n\r\u0080-\uFFFF] | JS_KEYWORD) JS_BEGIN_REGEXP -> type(JS_PART), pushMode(JSRegexpBegin);

JS_END_INLINE_SOURCE:
  END_INLINE_SOURCE -> type(END_INLINE_SOURCE), mode(Directive);

JS_AMBIGUOUS:
  // Interpretation depends on the parser, better leave as error token (if what follows is not a comment)
  ('++' | '--' | JS_SOFT_KEYWORD) JS_WHITESPACE '/' ~[/*];

JS_PART:
  // Optimistically any non-whitespace/EOL Unicode character is either an identifier part or is illegal
  // A member can be a keyword (includes #endinline but JS_END_INLINE_SOURCE has priority)
  ([.#] JS_WHITESPACE*)? [a-zA-Z0-9_$\u0080-\u2027\u202A-\uFEFE\uFF00-\uFFFF]+ |
  '"' (JS_ESCAPE | ~["\\])* '"' |
  '\'' (JS_ESCAPE | ~['\\])*  '\'' |
  .;

mode JSBegin;

// Only at the beginning, same tokens as above but also recognizes JS_BEGIN_REGEXP

JSBegin_WHITESPACE: JS_WHITESPACE -> mode(JS), type(JS_PART);
JSBegin_BEGIN_TEMPLATE: JS_BEGIN_TEMPLATE -> mode(JS), type(JS_PART), pushMode(JSTemplate);
JSBegin_BEGIN_BLOCK_REGEXP: JS_BEGIN_BLOCK_REGEXP -> mode(JS), type(JS_PART), pushMode(JSBlock), pushMode(JSRegexpBegin);
JSBegin_BEGIN_BLOCK: JS_BEGIN_BLOCK -> mode(JS), type(JS_PART), pushMode(JSBlock);
JSBegin_END_BLOCK: JS_END_BLOCK -> type(JS_PART), mode(InlineUnknown);
JSBegin_PART_BEGIN_REGEXP: JS_PART_BEGIN_REGEXP -> mode(JS), type(JS_PART), pushMode(JSRegexpBegin);
JSBegin_BEGIN_REGEXP: JS_BEGIN_REGEXP -> mode(JS), type(JS_PART), pushMode(JSRegexpBegin);
JSBegin_END_INLINE_SOURCE: JS_END_INLINE_SOURCE -> type(END_INLINE_SOURCE), mode(Directive);
JSBegin_AMBIGUOUS: JS_AMBIGUOUS -> mode(JS), type(JS_AMBIGUOUS);
JSBegin_PART: JS_PART -> mode(JS), type(JS_PART);

mode JSBlock;

// Same as above but inside {...}, so #endinline is not recognized

JSBlock_WHITESPACE: JS_WHITESPACE -> type(JS_PART);
JSBlock_BEGIN_TEMPLATE: JS_BEGIN_TEMPLATE -> type(JS_PART), pushMode(JSTemplate);
JSBlock_BEGIN_BLOCK_REGEXP: JS_BEGIN_BLOCK_REGEXP -> type(JS_PART), pushMode(JSBlock), pushMode(JSRegexpBegin);
JSBlock_BEGIN_BLOCK: JS_BEGIN_BLOCK -> type(JS_PART), pushMode(JSBlock);
JSBlock_END_BLOCK: JS_END_BLOCK -> type(JS_PART), popMode;
JSBlock_PART_BEGIN_REGEXP: JS_PART_BEGIN_REGEXP -> type(JS_PART), pushMode(JSRegexpBegin);
JSBlock_BEGIN_REGEXP: JS_BEGIN_REGEXP -> type(JS_PART), pushMode(JSRegexpBegin);
JSBlock_AMBIGUOUS: JS_AMBIGUOUS -> type(JS_AMBIGUOUS);
JSBlock_PART: JS_PART -> type(JS_PART);

mode JSTemplate;

JSTemplate_BEGIN_BLOCK:
  '${' -> type(JS_PART), pushMode(JSBlock);

JS_END_TEMPLATE:
  '`' -> popMode;

JSTemplate_PART:
  (~[`$\\]+ | '$' | JS_ESCAPE) -> type(JS_PART);

mode JSRegexp;

JS_END_REGEXP: (SLASH | JS_EOL) -> type(JS_PART), popMode;

JSRegexp_PART:
  (
    '[' ('^'? ']')? (~[\]\\] | '\\' .)* ']' |
    '\\'? ~[\n\r\u2028\u2029]
  ) -> type(JS_PART);

mode JSRegexpBegin;

// Same as above but this might actually be a comment

JSRegexpBegin_END_REGEXP: (JS_COMMENT_CONTENT | JS_EOL) -> type(JS_PART), popMode;

JSRegexpBegin_PART: JSRegexp_PART -> mode(JSRegexp), type(JS_PART);
