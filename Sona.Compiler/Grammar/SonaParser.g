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
  (globalAttrList (topLevelStatement | statement) ';'?)*? globalAttrList ((closingStatement | conditionallyReturningStatement) ';'? | implicitReturnStatement);

// A block that is evaluated as a whole (a function body)
valueBlock:
  (statement ';'?)*? ((closingStatement | conditionallyReturningStatement) ';'? | implicitReturnStatement);

// A block without return (no special handling when in a free-standing conditional)
valuelessBlock:
  (statement ';'?)*? (terminatingStatement ';'? | implicitReturnStatement);

// A block that does not return or throw at all (special handling in a final conditional)
openBlock:
  (statement ';'?)* implicitReturnStatement;

// A block that explicitly returns or throws
closedBlock:
  (statement ';'?)*? closingStatement ';'?;

// A block that explicitly returns
returningBlock:
  (statement ';'?)*? returningStatement ';'?;

// A block that explicitly throws
terminatingBlock:
  (statement ';'?)*? terminatingStatement ';'?;

// A block that may or may not return
controlBlock:
  (statement ';'?)*? conditionallyReturningStatement ';'?;

name:
  NAME | LITERAL_NAME;

dependentName:
  NAME | LITERAL_NAME | anyKeyword;

anyKeyword:
  'function' | 'return' | 'break' | 'and' | 'or' | 'not' | 'const' |
  'let' | 'var' | 'end' | 'throw' | 'import' | 'include' | 'require' |
  'if' | 'then' | 'else' | 'elseif' | 'do' | 'while' | 'for' | 'in';

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
  ifStatement |
  doStatement |
  whileStatement;

// A statement that must be located at the end of a block
closingStatement:
  returningStatement |
  terminatingStatement;

// A statement that always returns and evaluating it gives the final value of the block
returningStatement:
  returnStatement |
  ifStatementReturningClosed |
  ifStatementReturningClosedTrail |
  doStatementReturning |
  whileStatementReturningClosed;

// A statement that may or may not return
conditionallyReturningStatement:
  ifStatementReturningOpenSimple |
  ifStatementReturningOpenComplex |
  doStatementConditionallyReturning |
  whileStatementReturningOpen;

returnStatement:
  'return' (exprList)?;

implicitReturnStatement:;

// A statement that guarantees to terminate the execution without returning a value
terminatingStatement:
  throwStatement |
  ifStatementTerminating |
  doStatementTerminating |
  whileStatementTerminating;

throwStatement:
  'throw' (exprList)?;

// A sequence of statements that ends a returning block
finalStatements:
  (';'? statement)* (';'? (closingStatement | conditionallyReturningStatement) | implicitReturnStatement);

// Same but the statements must be closed
closedFinalStatements:
  (';'? statement)* ';'? closingStatement;

// Same but the statements must be open
openFinalStatements:
  (';'? statement)* implicitReturnStatement;

// A sequence of statements that will never be executed
ignoredStatements:
  (';'? statement)* (';'? (closingStatement | conditionallyReturningStatement) | implicitReturnStatement);

if:
  'if' expression 'then';

elseif:
  'elseif' expression 'then';

else:
  'else';

// Free-standing if without returns (but may secretly throw)
ifStatement:
  if valuelessBlock (elseif valuelessBlock)* (else valuelessBlock)? 'end';

// One branch is returning, the other are closed; the following statements are unused
ifStatementReturningClosed:
  (
    if returningBlock (elseif closedBlock)* else closedBlock 'end' |
    if closedBlock (elseif closedBlock)* (elseif returningBlock (elseif closedBlock)* else closedBlock | else returningBlock) 'end'
  ) ignoredStatements;

// The following statements are closing and so can become a part of else to form a closing statement.
ifStatementReturningClosedTrail:
  (
    if returningBlock (elseif closedBlock)* (else openBlock)? 'end' |
    if closedBlock (elseif closedBlock)* elseif returningBlock (elseif closedBlock)* (else openBlock)? 'end'
  ) closedFinalStatements;

// Same but the following statements are open, so the whole statement is open
ifStatementReturningOpenSimple:
  (
    if returningBlock (elseif closedBlock)* (else openBlock)? 'end' |
    if closedBlock (elseif closedBlock)* elseif returningBlock (elseif closedBlock)* (else openBlock)? 'end'
  ) openFinalStatements;

// Either same but the returning branch is conditional, or
// a non-else branch is open; the following statements are executed if no value is returned
ifStatementReturningOpenComplex:
  (
    if controlBlock (elseif closedBlock)* (else openBlock)? 'end' |
    if closedBlock (elseif closedBlock)* elseif controlBlock (elseif closedBlock)* (else openBlock)? 'end' |
    if (returningBlock | controlBlock) (elseif (closedBlock | openBlock | controlBlock))* elseif openBlock (elseif (closedBlock | openBlock | controlBlock))* (else (closedBlock | openBlock | controlBlock))? 'end' |
    if openBlock (elseif (closedBlock | openBlock | controlBlock))* (elseif (controlBlock | returningBlock) (elseif (closedBlock | openBlock | controlBlock))* (else (closedBlock | openBlock | controlBlock))? | else (returningBlock | controlBlock)) 'end'
  ) finalStatements;

// All branches are terminating; the following statements cannot be executed
ifStatementTerminating:
  if terminatingBlock (elseif terminatingBlock)* else terminatingBlock 'end' ignoredStatements;

doStatement:
  'do' valuelessBlock 'end';

doStatementReturning:
  'do' returningBlock 'end' ignoredStatements;

doStatementConditionallyReturning:
  'do' controlBlock 'end' finalStatements;

doStatementTerminating:
  'do' terminatingBlock 'end' ignoredStatements;

while:
  'while' expression 'do';

whileStatement:
  while valuelessBlock 'end';

whileStatementReturningClosed:
  WHILE_TRUE_DO returningBlock 'end' ignoredStatements;

whileStatementReturningOpen:
  while returningBlock 'end' finalStatements;

whileStatementTerminating:
  WHILE_TRUE_DO valuelessBlock 'end' ignoredStatements;

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
