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

BEGIN_INLINE_SOURCE:
  '#inline' LINE_WHITESPACE -> mode(InlineDirective);

BEGIN_PRAGMA:
  '#pragma' LINE_WHITESPACE -> channel(Pragma), mode(PragmaDirective);

LITERAL_NAME:
  '@' NAME;

fragment TRUE_EXPR:
  'true' | '(' IGNORE TRUE_EXPR IGNORE ')';

AS: 'as';
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
WHILE_TRUE_DO: 'while' IGNORE TRUE_EXPR IGNORE 'do';
FOR: 'for';
IN: 'in';
BY: 'by';
REPEAT: 'repeat';
UNTIL: 'until';

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

Directive_AS: AS -> type(AS);
Directive_RETURN: RETURN -> type(RETURN);
Directive_BREAK: BREAK -> type(BREAK);
Directive_CONTINUE: CONTINUE -> type(CONTINUE);
Directive_THROW: THROW -> type(THROW);
Directive_LET: LET -> type(LET);
Directive_VAR: VAR -> type(VAR);
Directive_CONST: CONST -> type(CONST);
Directive_FUNCTION: FUNCTION -> type(FUNCTION);
Directive_IMPORT: IMPORT -> type(IMPORT);
Directive_INCLUDE: INCLUDE -> type(INCLUDE);
Directive_REQUIRE: REQUIRE -> type(REQUIRE);
Directive_END: END -> type(END);
Directive_IF: IF -> type(IF);
Directive_THEN: THEN -> type(THEN);
Directive_ELSE: ELSE -> type(ELSE);
Directive_ELSEIF: ELSEIF -> type(ELSEIF);
Directive_DO: DO -> type(DO);
Directive_WHILE: WHILE -> type(WHILE);
Directive_WHILE_TRUE_DO: WHILE_TRUE_DO -> type(WHILE_TRUE_DO);
Directive_FOR: FOR -> type(FOR);
Directive_IN: IN -> type(IN);
Directive_BY: BY -> type(BY);
Directive_REPEAT: REPEAT -> type(REPEAT);
Directive_UNTIL: UNTIL -> type(UNTIL);

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

mode PragmaDirective;

END_PRAGMA:
  ('#' | EOL | EOF) -> channel(Pragma), mode(DEFAULT_MODE);

PragmaDirective_INT: INT -> type(INT), channel(Pragma);
PragmaDirective_FLOAT: FLOAT -> type(FLOAT), channel(Pragma);
PragmaDirective_EXP: EXP -> type(EXP), channel(Pragma);
PragmaDirective_HEX: HEX -> type(HEX), channel(Pragma);
PragmaDirective_NORMAL_STRING: NORMAL_STRING -> type(NORMAL_STRING), channel(Pragma);
PragmaDirective_VERBATIM_STRING: VERBATIM_STRING -> type(VERBATIM_STRING), channel(Pragma);
PragmaDirective_CHAR_STRING: CHAR_STRING -> type(CHAR_STRING), channel(Pragma);

PragmaDirective_COMMENT: COMMENT -> skip;
PragmaDirective_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
PragmaDirective_LINE_COMMENT: LINE_COMMENT -> skip;
PragmaDirective_WHITESPACE: (WHITESPACE | NEWLINE_ESCAPE) -> skip;

PragmaDirective_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME), channel(Pragma);

PragmaDirective_NAME: NAME -> type(NAME), channel(Pragma);

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

Verbatim_END_INTERPOLATED_STRING: '"' -> type(END_INTERPOLATED_STRING), popMode;

mode Interpolation;

// Not valid in non-string format.
fragment INTERP_NON_DELIMITER:
  ~([{}()"'\][\\\r\n]);
fragment INTERP_NON_DELIMITER_WHITESPACE:
  ~([{}()"'\][\\\r\n] | ' ' | '\t' | '\u000C');
fragment INTERP_NON_DELIMITER_ALPHA:
  ~([{}()"'\][\\\r\n] | [a-zA-Z]);
fragment INTERP_NON_DELIMITER_WHITESPACE_ALPHA:
  ~([{}()"'\][\\\r\n] | ' ' | '\t' | '\u000C' | [a-zA-Z_]);
fragment INTERP_NON_DELIMITER_WHITESPACE_ALPHANUMERIC:
  ~([{}()"'\][\\\r\n] | ' ' | '\t' | '\u000C' | [a-zA-Z_] | [0-9]);

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

// A valid string (must be a single letter).
INTERP_FORMAT_CUSTOM:
  ':' WHITESPACE* (NORMAL_STRING | VERBATIM_STRING);
  
// A valid character (must be a letter).
INTERP_FORMAT_STANDARD:
  ':' WHITESPACE* CHAR_STRING;

INTERP_FORMAT_NUMBER:
  ':' (STANDARD_NUM_FORMAT | CUSTOM_NUM_FORMAT) -> mode(InterpolationEnd);

INTERP_FORMAT_COMPONENTS:
  COLON -> skip, mode(InterpolationComponents);

fragment STANDARD_NUM_FORMAT:
  [bBcCdDeEfFgGnNpPrRxX] '0'* [1-9]? [0-9];

fragment CUSTOM_NUM_FORMAT:
  CUSTOM_NUM_SECTION (';' CUSTOM_NUM_SECTION (';' CUSTOM_NUM_SECTION)?)?;

fragment CUSTOM_NUM_LITERAL:
  '\\' ('\\' | INTERP_NON_DELIMITER) | LINE_WHITESPACE | [-+];

fragment CUSTOM_NUM_SECTION:
  CUSTOM_NUM_LITERAL* (
    ([%\u2030] CUSTOM_NUM_LITERAL*)? CUSTOM_NUM_DIGITS |
    CUSTOM_NUM_DIGITS CUSTOM_NUM_LITERAL* [%\u2030]
  ) CUSTOM_NUM_LITERAL*;

fragment CUSTOM_NUM_DIGITS:
  // Preceding # groups
  (('#' CUSTOM_NUM_LITERAL*)+ ',' CUSTOM_NUM_LITERAL*)*
  (
    // Preceding mixed or 0 groups
    (('#' CUSTOM_NUM_LITERAL*)+ ('0' CUSTOM_NUM_LITERAL*)+ ',' CUSTOM_NUM_LITERAL*)?
    (('0' CUSTOM_NUM_LITERAL*)+ ',' CUSTOM_NUM_LITERAL*)*
    // Unit digit
    '0' | '#'
  )
  (
    // Decimal point
    CUSTOM_NUM_LITERAL* ','*
    (
      ',' |
      '.'
      // Decimal digits
      CUSTOM_NUM_LITERAL*
      (
        // Tenth digit
        '#' | '0'
        // 0 decimal digits
        (CUSTOM_NUM_LITERAL* '0')*
      )
      // # decimal digits
      (CUSTOM_NUM_LITERAL* '#')*
    )
  )?
  // Exponent
  (CUSTOM_NUM_LITERAL* [eE] [-+]? '0'+)?;

INTERP_ALIGNMENT:
  ',' WHITESPACE* ('+' | '-')? INT;

Interpolation_INT: INT -> type(INT);
Interpolation_FLOAT: FLOAT -> type(FLOAT);
Interpolation_EXP: EXP -> type(EXP);
Interpolation_HEX: HEX -> type(HEX);
Interpolation_NORMAL_STRING: NORMAL_STRING -> type(NORMAL_STRING);
Interpolation_VERBATIM_STRING: VERBATIM_STRING -> type(VERBATIM_STRING);
Interpolation_BEGIN_INTERPOLATED_STRING: BEGIN_INTERPOLATED_STRING -> type(BEGIN_INTERPOLATED_STRING), pushMode(InterpolatedString);
Interpolation_BEGIN_VERBATIM_INTERPOLATED_STRING: BEGIN_VERBATIM_INTERPOLATED_STRING -> type(BEGIN_VERBATIM_INTERPOLATED_STRING), pushMode(VerbatimInterpolatedString);
Interpolation_CHAR_STRING: CHAR_STRING -> type(CHAR_STRING);

Interpolation_COMMENT: COMMENT -> skip;
Interpolation_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
Interpolation_LINE_COMMENT: LINE_COMMENT -> skip;

Interpolation_WHITESPACE: WHITESPACE -> skip;

Interpolation_LITERAL_NAME: LITERAL_NAME -> type(LITERAL_NAME);

Interpolation_AS: AS -> type(AS);
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
Interpolation_BY: BY -> type(BY);
Interpolation_REPEAT: REPEAT -> type(REPEAT);
Interpolation_UNTIL: UNTIL -> type(UNTIL);

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

mode InterpolationComponents;

INTERP_COMPONENTS_PART_SHORT:
  [a-zA-Z];

INTERP_COMPONENTS_PART_LONG:
  '\\' ('\\' | INTERP_NON_DELIMITER) |
  '%' [a-zA-Z] | COLON | DIVIDE |
  'a'+ | 'b'+ | 'c'+ | 'd'+ | 'e'+ | 'f'+ | 'g'+ |
  'h'+ | 'i'+ | 'j'+ | 'k'+ | 'l'+ | 'm'+ | 'n'+ |
  'o'+ | 'p'+ | 'q'+ | 'r'+ | 's'+ | 't'+ | 'u'+ |
  'v'+ | 'w'+ | 'x'+ | 'y'+ | 'z'+ |
  'A'+ | 'B'+ | 'C'+ | 'D'+ | 'E'+ | 'F'+ | 'G'+ |
  'H'+ | 'I'+ | 'J'+ | 'K'+ | 'L'+ | 'M'+ | 'N'+ |
  'O'+ | 'P'+ | 'Q'+ | 'R'+ | 'S'+ | 'T'+ | 'U'+ |
  'V'+ | 'W'+ | 'X'+ | 'Y'+ | 'Z'+;

InterpolationComponents_CLOSEB: CLOSEB -> type(CLOSEB), popMode;

mode InterpolationEnd;

InterpolationEnd_CLOSEB: CLOSEB -> type(CLOSEB), popMode;

mode Empty;

ERROR: (~('\u0000')+ | '\u0000'+);

mode InlineDirective;

InlineDirective_NORMAL_STRING: NORMAL_STRING -> type(NORMAL_STRING), mode(FS);
InlineDirective_VERBATIM_STRING: VERBATIM_STRING -> type(VERBATIM_STRING), mode(FS);

InlineDirective_COMMENT: COMMENT -> skip;
InlineDirective_DOC_COMMENT: DOC_COMMENT -> channel(Documentation);
InlineDirective_LINE_COMMENT: LINE_COMMENT -> skip;
InlineDirective_WHITESPACE: (WHITESPACE | NEWLINE_ESCAPE) -> type(WHITESPACE);

mode FS;

/* F# lite mode */

fragment FS_ESCAPE:
  '\\' .;

// Special-handled tokens that do not form delimiters in code and comments
fragment FS_TOKEN:
  '(*)' |
  '@"' ('""' | ~('"'))* '"' |
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

END_INLINE_SOURCE:
  EOL '#endinline' ' '? -> mode(Directive);

FS_DIRECTIVE:
  EOL '#' ('endinline' ~[ \r\n])?;

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
