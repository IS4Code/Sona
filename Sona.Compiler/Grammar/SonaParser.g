parser grammar SonaParser;
options {
  tokenVocab = SonaLexer;
}

/* Blocks */

// The whole program
chunk:
  mainBlock EOF;

/*
The sequence of statements is intentionally lazy, to make the
finishing statement take as much of the block as possible.
*/

// The program's body, supporting top-level statements and conditionals
mainBlock:
  (globalAttrList (topLevelStatement | statement) ';'?)*? globalAttrList ((closingStatement | conditionalStatement) ';'? | implicitReturnStatement);

// A block that is evaluated as a whole (a function body)
valueBlock:
  (statement ';'?)*? ((closingStatement | conditionalStatement) ';'? | implicitReturnStatement);

// A block without return (no special handling when in a free-standing conditional)
freeBlock:
  (statement ';'?)*? (terminatingStatement ';'? | implicitReturnStatement);

// A block that does not return or throw at all (special handling in a final conditional)
openBlock:
  (statement ';'?)* implicitReturnStatement;

// A block that explicitly returns or throws
returnSafeBlock:
  (statement ';'?)*? (returningStatement | terminatingStatement) ';'?;

// A block that explicitly returns
returningBlock:
  (statement ';'?)*? returningStatement ';'?;

// A block that explicitly interrupts
interruptingBlock:
  (statement ';'?)*? interruptingStatement ';'?;

// A block that explicitly throws
terminatingBlock:
  (statement ';'?)*? terminatingStatement ';'?;

// A block that may or may not return or interrupt
conditionalBlock:
  (statement ';'?)*? conditionalStatement ';'?;

name:
  NAME | LITERAL_NAME;

dependentName:
  NAME | LITERAL_NAME | anyKeyword;

anyKeyword:
  'function' | 'return' | 'break' | 'and' | 'or' | 'not' | 'const' |
  'let' | 'var' | 'end' | 'throw' | 'import' | 'include' | 'require' |
  'if' | 'then' | 'else' | 'elseif' | 'do' | 'while' | 'for' | 'in' |
  'continue';

compoundName:
  name ('.' dependentName)*;

type:
  NAME;

/* Attributes */

localAttrList:
  localAttribute*;

localAttribute:
  (BEGIN_GENERAL_LOCAL_ATTRIBUTE | localAttrTarget) attrGroup (',' attrGroup)* (END_DIRECTIVE | EOF);

localAttrTarget:
  BEGIN_TARGETED_LOCAL_ATTRIBUTE;

globalAttrList:
  globalAttribute*;

globalAttribute:
  globalAttrTarget attrGroup (',' attrGroup)* (END_DIRECTIVE | EOF);

globalAttrTarget:
  BEGIN_TARGETED_GLOBAL_ATTRIBUTE;

attrGroup:
  WHITESPACE* compoundName (WHITESPACE* (attrNamedArg | attrPosArg) (WHITESPACE+ (attrNamedArg | attrPosArg))*)? WHITESPACE*;

attrPosArg:
   atomicExpr;

attrNamedArg:
  name WHITESPACE* '=' WHITESPACE* atomicExpr;

/* Statements */

// Plain statement, allowed anywhere
statement:
  variableDecl |
  multiFuncDecl |
  assignmentOrCall |
  ifStatementFree |
  doStatementFree |
  whileStatementFree;

// A statement that must be located at the end of a block
closingStatement:
  returningStatement |
  interruptingStatement |
  terminatingStatement;

// A statement that always returns and evaluating it gives the final value of the block
returningStatement:
  returnStatement |
  doStatementReturning |
  ifStatementReturning |
  ifStatementReturningTrail |
  whileStatementReturning;

returnStatement:
  'return' (exprList)?;

implicitReturnStatement:;

interruptingStatement:
  breakStatement |
  continueStatement;

breakStatement:
  'break' (exprList)?;

continueStatement:
  'continue' (exprList)?;

// A statement that may or may not return
conditionalStatement:
  doStatementConditional |
  ifStatementConditionalSimple |
  ifStatementConditionalComplex |
  whileStatementConditional;

// A statement that guarantees to terminate the execution without returning a value
terminatingStatement:
  throwStatement |
  doStatementTerminating |
  ifStatementTerminating |
  whileStatementTerminating;

throwStatement:
  'throw' (exprList)?;

// A sequence of statements that follows a conditionally returning block (open, closed, or conditionally returning)
conditionalTrailingStatements:
  (';'? statement)* (';'? (closingStatement | conditionalStatement) | implicitReturnStatement);

// Same but encountered in a context where never executed (follows a closed block)
ignoredTrailingStatements:
  (';'? statement)* (';'? (closingStatement | conditionalStatement) | implicitReturnStatement);

// A sequence of statements that follows a returning block (returns or throws)
returnSafeTrailingStatements:
  (';'? statement)* ';'? (returningStatement | terminatingStatement);

// Same but the statements must be open
openTrailingStatements:
  (';'? statement)* implicitReturnStatement;

if:
  'if' expression 'then';

elseif:
  'elseif' expression 'then';

else:
  'else';

// Free-standing if without returns (may throw from all branches but this is not preferred).
ifStatementFree:
  if freeBlock (elseif freeBlock)* (else freeBlock)? 'end';

// One branch is returning, the other are return-safe; the trailing statements are ignored.
ifStatementReturning:
  (
    if returningBlock (elseif returnSafeBlock)* else returnSafeBlock 'end' |
    if returnSafeBlock (elseif returnSafeBlock)* (elseif returningBlock (elseif returnSafeBlock)* else returnSafeBlock | else returningBlock) 'end'
  ) ignoredTrailingStatements;

// Else is open or missing, but the trailing statements are return-safe and so supplant it to form a returning statement.
ifStatementReturningTrail:
  (
    if returningBlock (elseif returnSafeBlock)* (else openBlock)? 'end' |
    if returnSafeBlock (elseif returnSafeBlock)* elseif returningBlock (elseif returnSafeBlock)* (else openBlock)? 'end'
  ) returnSafeTrailingStatements;

// Same but the trailing statements are open, so the whole statement is open
ifStatementConditionalSimple:
  (
    if returningBlock (elseif returnSafeBlock)* (else openBlock)? 'end' |
    if returnSafeBlock (elseif returnSafeBlock)* elseif returningBlock (elseif returnSafeBlock)* (else openBlock)? 'end'
  ) openTrailingStatements;

// Either same but the returning branch is conditional, or
// a non-else branch is open; the trailing statements are executed if no value is returned
ifStatementConditionalComplex:
  (
    if conditionalBlock (elseif returnSafeBlock)* (else openBlock)? 'end' |
    if returnSafeBlock (elseif returnSafeBlock)* elseif conditionalBlock (elseif returnSafeBlock)* (else openBlock)? 'end' |
    if (returningBlock | conditionalBlock) (elseif (returnSafeBlock | openBlock | conditionalBlock))* elseif openBlock (elseif (returnSafeBlock | openBlock | conditionalBlock))* (else (returnSafeBlock | openBlock | conditionalBlock))? 'end' |
    if openBlock (elseif (returnSafeBlock | openBlock | conditionalBlock))* (elseif (conditionalBlock | returningBlock) (elseif (returnSafeBlock | openBlock | conditionalBlock))* (else (returnSafeBlock | openBlock | conditionalBlock))? | else (returningBlock | conditionalBlock)) 'end'
  ) conditionalTrailingStatements;

// All branches are terminating; the trailing statements cannot be executed
ifStatementTerminating:
  if terminatingBlock (elseif terminatingBlock)* else terminatingBlock 'end' ignoredTrailingStatements;

doStatementFree:
  'do' freeBlock 'end';

doStatementReturning:
  'do' returningBlock 'end' ignoredTrailingStatements;

doStatementConditional:
  'do' conditionalBlock 'end' conditionalTrailingStatements;

doStatementTerminating:
  'do' terminatingBlock 'end' ignoredTrailingStatements;

while:
  'while' expression 'do';

whileStatementFree:
  while freeBlock 'end';

whileStatementReturning:
  WHILE_TRUE_DO returningBlock 'end' ignoredTrailingStatements;

whileStatementConditional:
  while returningBlock 'end' conditionalTrailingStatements;

whileStatementTerminating:
  WHILE_TRUE_DO freeBlock 'end' ignoredTrailingStatements;

topLevelStatement:
  importStatement |
  importTypeStatement |
  importFileStatement |
  includeStatement |
  requireStatement;

importStatement:
  'import' symbolArg;

symbolArg:
  compoundName |
  '(' symbolArg ')';

importTypeStatement:
  'import' symbolContentsArg;

symbolContentsArg:
  compoundName '.' '*' |
  '(' symbolContentsArg ')';

importFileStatement:
  'import' stringArg;

includeStatement:
  'include' stringArg;

requireStatement:
  'require' stringArg;

stringArg:
  string |
  '(' stringArg ')';


assignmentOrCall:
  name (assignment | assignmentOrCallSuffix) |
  (nestedExpr | arrayConstructor | recordConstructor | sequenceConstructor) assignmentOrCallSuffix;

assignment:
  '=' exprList;

assignmentOrCallSuffix:
  varSuffix* (callArguments+ | varSuffix assignment);

/* Declarations */

variableDecl:
  localAttrList (letDecl | varDecl | constDecl);

letDecl:
  'let' declList '=' exprList;

varDecl:
  'var' declList '=' exprList;

constDecl:
  'const' declList '=' exprList;

multiFuncDecl:
  funcDecl+;

funcDecl:
  localAttrList 'function' name funcBody;

funcBody:
  '(' paramList ')' valueBlock 'end';

paramList:
  paramTuple (';' paramTuple)*;

paramTuple:
  declList?;

declList:
  declaration (',' declaration)*;

declaration:
  localAttrList name (':' type)?;

/* Expressions */

exprList:
  expression (',' expression)*;

expression:
  negatedExpr |
  conjunctiveExpr |
  disjunctiveExpr |
  /* Require parentheses */
  //cnfExpr |
  //dnfExpr |
  outerExpr_Outer;


logicExprArg:
  outerExpr_Outer;

negatedExpr:
  'not' doubleNegation* logicExprArg;

doubleNegation:
  'not';

conjunctiveExpr:
  logicExprArg ('and' logicExprArg)* 'and' (negatedExpr | logicExprArg);

disjunctiveExpr:
  logicExprArg ('or' logicExprArg)* 'or' (negatedExpr | logicExprArg);

cnfExpr:
  disjunctiveExpr ('and' disjunctiveExpr)+;

dnfExpr:
  conjunctiveExpr ('or' conjunctiveExpr)+;

/*
// Relaxed versions

cnfExpr:
  disjunctiveExpr ('and' (logicExprArg | disjunctiveExpr))+;

dnfExpr:
  conjunctiveExpr ('or' (logicExprArg | conjunctiveExpr))+;

*/


outerExpr_Outer:
  outerExpr |
  outerExpr_Inner;

outerExpr:
  outerExpr_Inner (outerBinaryOperator outerExpr_Inner)+;

outerExpr_Inner:
  concatExpr_Outer;


concatExpr_Outer:
  concatExpr |
  concatExpr_Inner;

concatExpr:
  concatExprArg ('..' concatExprNextArg)+;

concatExprArg:
  concatExpr_Inner;

concatExprNextArg:
  concatExprArg;

concatExpr_Inner:
  bitOrExpr_Outer;


bitOrExpr_Outer:
  bitOrExpr |
  bitOrExpr_Inner;

bitOrExpr:
  bitOrExprArg ('|' bitOrExprNextArg)+;

bitOrExprArg:
  bitOrExpr_Inner;

bitOrExprNextArg:
  bitOrExprArg;

bitOrExpr_Inner:
  bitXorExpr_Outer;


bitXorExpr_Outer:
  bitXorExpr |
  bitXorExpr_Inner;

bitXorExpr:
  bitXorExprArg ('^' bitXorExprNextArg)+;

bitXorExprArg:
  bitXorExpr_Inner;

bitXorExprNextArg:
  bitXorExprArg;

bitXorExpr_Inner:
  bitAndExpr_Outer;


bitAndExpr_Outer:
  bitAndExpr |
  bitAndExpr_Inner;

bitAndExpr:
  bitAndExprArg ('&' bitAndExprNextArg)+;

bitAndExprArg:
  bitAndExpr_Inner;

bitAndExprNextArg:
  bitAndExprArg;

bitAndExpr_Inner:
  bitShiftExpr_Outer;


bitShiftExpr_Outer:
  bitShiftExpr |
  bitShiftExpr_Inner;

bitShiftExpr:
  bitShiftExprArg (bitShiftLeftExprNextArg | bitShiftRightExprNextArg)+;

bitShiftExprArg:
  bitShiftExpr_Inner;

bitShiftLeftExprNextArg:
  '<<' bitShiftExprArg;

bitShiftRightExprNextArg:
  '>>' bitShiftExprArg;

bitShiftExpr_Inner:
  innerExpr;


innerExpr:
  atomicExpr (innerBinaryOperator atomicExpr)*;

atomicExpr:
  simpleExpr |
  funcExpr |
  assignmentOrValue |
  hashExpr |
  notExpr |
  unaryOperator atomicExpr;

simpleExpr:
  primitiveExpr |
  interpolatedString |
  verbatimInterpolatedString;

nestedExpr:
  '(' exprList ')';

primitiveExpr:
  'null' | 'false' | 'true' | number | string;

funcExpr:
  'function' name? funcBody;

assignmentOrValue:
  name (assignment | assignmentOrValueSuffix) |
  (nestedExpr | arrayConstructor | recordConstructor | sequenceConstructor) assignmentOrValueSuffix;

assignmentOrValueSuffix:
  varSuffix* (callArguments+ | varSuffix assignment)?;

hashExpr:
  '#' atomicExpr;

notExpr:
  '!' atomicExpr;

varSuffix:
  callArguments* (indexAccess | memberAccess | dynamicMemberAccess);

indexAccess:
  '[' exprList ']';
  
memberAccess:
  '.' dependentName;

dynamicMemberAccess:
  ':' dependentName;

callArguments:
  '(' callArgList ')' |
  recordConstructor |
  sequenceConstructor |
  string |
  interpolatedString;

callArgList:
  callArgTuple (';' callArgTuple)*;

callArgTuple:
  exprList?;

recordConstructor:
  '{' recordField (',' recordField)* '}';

recordField:
  name '=' expression;

arrayConstructor:
  '[' (arrayElement (',' arrayElement)*)? ']';

arrayElement:
  spreadExpression |
  expression;

sequenceConstructor:
  '{' (sequenceElement (',' sequenceElement)*)? '}';

sequenceElement:
  '[' exprList ']' '=' expression |
  spreadExpression |
  expression;

spreadExpression:
  '..' expression;

/* Interpolated strings */

interpolatedString:
  BEGIN_INTERPOLATED_STRING interpStrComponent* END_INTERPOLATED_STRING;

verbatimInterpolatedString:
  BEGIN_VERBATIM_INTERPOLATED_STRING interpStrComponent* END_INTERPOLATED_STRING;

interpStrComponent:
  interpStrPart |
  interpStrPercent |
  interpStrExpression;

interpStrPart: INTERP_PART;
interpStrPercent: '%';
interpStrAlignment: INTERP_ALIGNMENT;
interpStrFormat: INTERP_FORMAT;

interpStrExpression:
  '{' expression interpStrAlignment? interpStrFormat? '}';

/* Operators */

innerBinaryOperator:
  '+' | '-' | '*' | '/' | '%';

outerBinaryOperator:
  '<' | '<=' | '>' | '>=' | '==' | '!=' | '~=' |
  '&&' | '||';

unaryOperator:
  '+' | '-' | '~';

number:
  INT | FLOAT | EXP | HEX;

string:
  NORMAL_STRING | VERBATIM_STRING | CHAR_STRING;
