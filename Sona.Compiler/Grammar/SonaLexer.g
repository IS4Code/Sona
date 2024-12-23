lexer grammar SonaLexer;

channels {
  Documentation
}

fragment LOWERCASE:
  [a-z];

fragment UPPERCASE:
  [A-Z];

fragment DIGIT:
  [0-9];

fragment HEXDIGIT:
  DIGIT | [a-f] | [A-F];

INT:
  DIGIT+;

FLOAT:
  INT '.' DIGIT+;

EXP:
  (INT | FLOAT) ('E' | 'e') ('+' | '-')? INT;

HEX:
  '0x' HEXDIGIT+;

fragment ESCAPE:
  '\\' ~('\r' | '\n');

NORMAL_STRING:
  '"' (ESCAPE | ~('\\' | '"' | '\r' | '\n'))* '"';

VERBATIM_STRING:
  '@"' ('""' | ~('"'))* '"';

BEGIN_INTERPOLATED_STRING:
  '$"' -> pushMode(InterpolatedString);

BEGIN_VERBATIM_INTERPOLATED_STRING:
  ('$@' | '@$') '"' -> pushMode(VerbatimInterpolatedString);

CHAR_STRING:
  '\'' (ESCAPE | ~('\\' | '\'' | '\r' | '\n'))* '\'';

COMMENT:
  '/*' .*? '*/' -> skip;

fragment EOL:
  '\r'? ('\n' | EOF);

DOC_COMMENT:
  '///' ~('\n' | '\r')* -> channel(Documentation);

LINE_COMMENT:
  '//' ~('\n' | '\r')* -> skip;

fragment LINE_WHITESPACE:
  ' ' | '\t' | '\u000C';

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

LITERAL_NAME:
  '@' NAME;

RETURN: 'return';
BREAK: 'break';
CONTINUE: 'continue';
THROW: 'throw';
LET: 'let';
VAR: 'var';
CONST: 'const';
FUNCTION: 'function';
IMPORT: 'import';
INCLUDE: 'include';
REQUIRE: 'require';
END: 'end';
IF: 'if';
THEN: 'then';
ELSE: 'else';
ELSEIF: 'elseif';
DO: 'do';
WHILE: 'while';
WHILE_TRUE_DO: 'while' IGNORE ('true' | '(' IGNORE 'true' IGNORE ')') IGNORE 'do';
FOR: 'for';
IN: 'in';

SEMICOLON: ';';
EQ: '==';
NEQ: '!=';
NEQ_ALT: '~=';
OPENP: '(' -> pushMode(DEFAULT_MODE);
CLOSEP: ')' -> popMode;
COMMA: ',';
DOLLAR: '$';
HASH: '#';
NULL: 'null';
TRUE: 'true';
FALSE: 'false';
OPENSP: '[' -> pushMode(DEFAULT_MODE);
CLOSESP: ']' -> popMode;
OPENB: '{' -> pushMode(DEFAULT_MODE);
CLOSEB: '}' -> popMode;
PLUS: '+';
MINUS: '-';
MULTIPLY: '*';
DIVIDE: '/';
PERCENT: '%';
CONCAT: '..';
LT: '<';
LTE: '<=';
GT: '>';
GTE: '>=';
ASSIGN: '=';
AND: '&&';
OR: '||';
AND_WORD: 'and';
OR_WORD: 'or';
BIT_AND: '&';
BIT_OR: '|';
BIT_XOR: '^';
RSHIFT: '>>';
LSHIFT: '<<';
NOT: '!';
NOT_WORD: 'not';
BIT_NOT: '~';
COLON: ':';
DOT: '.';

NAME:
  (LOWERCASE | UPPERCASE | '_') (LOWERCASE | UPPERCASE | DIGIT | '_')*;

mode Directive;

fragment NEWLINE_ESCAPE: '\\' EOL;

END_DIRECTIVE:
  (':#' | '#' | EOL | EOF) -> mode(DEFAULT_MODE);

Directive_INT: INT -> type(INT);
Directive_FLOAT: FLOAT -> type(FLOAT);
Directive_EXP: EXP -> type(EXP);
Directive_HEX: HEX -> type(HEX);
Directive_NORMAL_STRING: NORMAL_STRING -> type(NORMAL_STRING);
Directive_VERBATIM_STRING: VERBATIM_STRING -> type(VERBATIM_STRING);
Directive_CHAR_STRING: CHAR_STRING -> type(CHAR_STRING);

Directive_COMMENT: COMMENT -> skip;
Directive_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
Directive_LINE_COMMENT: LINE_COMMENT -> skip;
Directive_WHITESPACE: (WHITESPACE | NEWLINE_ESCAPE) -> type(WHITESPACE);

Directive_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME);

Directive_SEMICOLON: SEMICOLON -> type(SEMICOLON);
Directive_EQ: EQ -> type(EQ);
Directive_NEQ: NEQ -> type(NEQ);
Directive_NEQ_ALT: NEQ_ALT -> type(NEQ_ALT);
Directive_OPENP: OPENP -> type(OPENP), pushMode(DEFAULT_MODE);
Directive_CLOSEP: CLOSEP -> type(CLOSEP), popMode;
Directive_COMMA: COMMA ((LINE_WHITESPACE | NEWLINE_ESCAPE)* EOL)? -> type(COMMA);
Directive_DOLLAR: DOLLAR -> type(DOLLAR);
// no Directive_HASH
Directive_NULL: NULL -> type(NULL);
Directive_TRUE: TRUE -> type(TRUE);
Directive_FALSE: FALSE -> type(FALSE);
Directive_OPENSP: OPENSP -> type(OPENSP), pushMode(DEFAULT_MODE);
Directive_CLOSESP: CLOSESP -> type(CLOSESP), popMode;
Directive_OPENB: OPENB -> type(OPENB), pushMode(DEFAULT_MODE);
Directive_CLOSEB: CLOSEB -> type(CLOSEB), popMode;
Directive_PLUS: PLUS -> type(PLUS);
Directive_MINUS: MINUS -> type(MINUS);
Directive_MULTIPLY: MULTIPLY -> type(MULTIPLY);
Directive_DIVIDE: DIVIDE -> type(DIVIDE);
Directive_PERCENT: PERCENT -> type(PERCENT);
Directive_CONCAT: CONCAT -> type(CONCAT);
Directive_LT: LT -> type(LT);
Directive_LTE: LTE -> type(LTE);
Directive_GT: GT -> type(GT);
Directive_GTE: GTE -> type(GTE);
Directive_ASSIGN: ASSIGN -> type(ASSIGN);
Directive_AND: AND -> type(AND);
Directive_OR: OR -> type(OR);
Directive_AND_WORD: AND_WORD -> type(AND_WORD);
Directive_OR_WORD: OR_WORD -> type(OR_WORD);
Directive_BIT_AND: BIT_AND -> type(BIT_AND);
Directive_BIT_OR: BIT_OR -> type(BIT_OR);
Directive_BIT_XOR: BIT_XOR -> type(BIT_XOR);
Directive_RSHIFT: RSHIFT -> type(RSHIFT);
Directive_LSHIFT: LSHIFT -> type(LSHIFT);
Directive_NOT: NOT -> type(NOT);
Directive_NOT_WORD: NOT_WORD -> type(NOT_WORD);
Directive_BIT_NOT: BIT_NOT -> type(BIT_NOT);
Directive_COLON: COLON -> type(COLON);
Directive_DOT: DOT -> type(DOT);

Directive_NAME: NAME -> type(NAME);

mode InterpolatedString;

fragment INTERP_ESCAPE:
  '\\' ~('%' | '{' | '}' | '\r' | '\n');

INTERP_PART:
  (INTERP_ESCAPE | '{{' | '}}' | ~('\\' | '"' | '{' | '}' | '%' | '\r' | '\n'))+;

InterpolatedString_OPENB: OPENB -> type(OPENB), pushMode(Interpolation);
InterpolatedString_PERCENT: ('\\%' | PERCENT) -> type(PERCENT);

END_INTERPOLATED_STRING: '"' -> popMode;

mode VerbatimInterpolatedString;

Verbatim_INTERP_PART:
  ('{{' | '}}' | '""' | ~('"' | '{' | '}' | '%'))+ -> type(INTERP_PART);

VerbatimInterpolatedString_OPENB: OPENB -> type(OPENB), pushMode(Interpolation);
VerbatimInterpolatedString_PERCENT: PERCENT -> type(PERCENT);

Verbatim_END_INTERPOLATED_STRING: '"' -> popMode;

mode Interpolation;

INTERP_FORMAT:
  ':' ~('}')*;

INTERP_ALIGNMENT:
  ',' WHITESPACE* INT WHITESPACE*;

Interpolation_INT: INT -> type(INT);
Interpolation_FLOAT: FLOAT -> type(FLOAT);
Interpolation_EXP: EXP -> type(EXP);
Interpolation_HEX: HEX -> type(HEX);
Interpolation_CHAR_STRING: CHAR_STRING -> type(CHAR_STRING);

Interpolation_COMMENT: COMMENT -> skip;
Interpolation_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
Interpolation_LINE_COMMENT: LINE_COMMENT -> skip;

Interpolation_WHITESPACE: WHITESPACE -> skip;

Interpolation_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME);
Interpolation_RETURN: RETURN -> type(RETURN);
Interpolation_BREAK: BREAK -> type(BREAK);
Interpolation_CONTINUE: CONTINUE -> type(CONTINUE);
Interpolation_THROW: THROW -> type(THROW);
Interpolation_LET: LET -> type(LET);
Interpolation_VAR: VAR -> type(VAR);
Interpolation_CONST: CONST -> type(CONST);
Interpolation_FUNCTION: FUNCTION -> type(FUNCTION);
Interpolation_IMPORT: IMPORT -> type(IMPORT);
Interpolation_INCLUDE: INCLUDE -> type(INCLUDE);
Interpolation_REQUIRE: REQUIRE -> type(REQUIRE);
Interpolation_END: END -> type(END);
Interpolation_IF: IF -> type(IF);
Interpolation_THEN: THEN -> type(THEN);
Interpolation_ELSE: ELSE -> type(ELSE);
Interpolation_ELSEIF: ELSEIF -> type(ELSEIF);
Interpolation_DO: DO -> type(DO);
Interpolation_WHILE: WHILE -> type(WHILE);
Interpolation_WHILE_TRUE_DO: WHILE_TRUE_DO -> type(WHILE_TRUE_DO);
Interpolation_FOR: FOR -> type(FOR);
Interpolation_IN: IN -> type(IN);

Interpolation_SEMICOLON: SEMICOLON -> type(SEMICOLON);
Interpolation_EQ: EQ -> type(EQ);
Interpolation_NEQ: NEQ -> type(NEQ);
Interpolation_NEQ_ALT: NEQ_ALT -> type(NEQ_ALT);
Interpolation_OPENP: OPENP -> type(OPENP), pushMode(DEFAULT_MODE);
Interpolation_CLOSEP: CLOSEP -> type(CLOSEP), popMode;
// no Interpolation_COMMA
Interpolation_DOLLAR: DOLLAR -> type(DOLLAR);
Interpolation_HASH: HASH -> type(HASH);
Interpolation_NULL: NULL -> type(NULL);
Interpolation_TRUE: TRUE -> type(TRUE);
Interpolation_FALSE: FALSE -> type(FALSE);
Interpolation_OPENSP: OPENSP -> type(OPENSP), pushMode(DEFAULT_MODE);
Interpolation_CLOSESP: CLOSESP -> type(CLOSESP), popMode;
Interpolation_OPENB: OPENB -> type(OPENB), pushMode(DEFAULT_MODE);
Interpolation_CLOSEB: CLOSEB -> type(CLOSEB), popMode;
Interpolation_PLUS: PLUS -> type(PLUS);
Interpolation_MINUS: MINUS -> type(MINUS);
Interpolation_MULTIPLY: MULTIPLY -> type(MULTIPLY);
Interpolation_DIVIDE: DIVIDE -> type(DIVIDE);
Interpolation_PERCENT: PERCENT -> type(PERCENT);
Interpolation_CONCAT: CONCAT -> type(CONCAT);
Interpolation_LT: LT -> type(LT);
Interpolation_LTE: LTE -> type(LTE);
Interpolation_GT: GT -> type(GT);
Interpolation_GTE: GTE -> type(GTE);
Interpolation_ASSIGN: ASSIGN -> type(ASSIGN);
Interpolation_AND: AND -> type(AND);
Interpolation_OR: OR -> type(OR);
Interpolation_AND_WORD: AND_WORD -> type(AND_WORD);
Interpolation_OR_WORD: OR_WORD -> type(OR_WORD);
Interpolation_BIT_AND: BIT_AND -> type(BIT_AND);
Interpolation_BIT_OR: BIT_OR -> type(BIT_OR);
Interpolation_BIT_XOR: BIT_XOR -> type(BIT_XOR);
Interpolation_RSHIFT: RSHIFT -> type(RSHIFT);
Interpolation_LSHIFT: LSHIFT -> type(LSHIFT);
Interpolation_NOT: NOT -> type(NOT);
Interpolation_NOT_WORD: NOT_WORD -> type(NOT_WORD);
Interpolation_BIT_NOT: BIT_NOT -> type(BIT_NOT);
// no Interpolation_COLON
Interpolation_DOT: DOT -> type(DOT);

Interpolation_NAME: NAME -> type(NAME);

mode Empty;

Empty_REST: (~('\u0000')+ | '\u0000'+) -> skip;
