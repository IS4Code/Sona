parser grammar SonaParser;
options {
  tokenVocab = SonaLexer;
}

chunk:
  mainBlock EOF;

subBlock:
  (statement (';')?)* (blockFinalStatement (';')? | implicitReturnStatement);

funcBlock:
  (statement (';')?)* ((funcFinalStatement | blockFinalStatement) (';')? | implicitReturnStatement);

mainBlock:
  (globalAttrList (topLevelStatement | statement) (';')?)* globalAttrList ((funcFinalStatement | blockFinalStatement) (';')? | implicitReturnStatement);

name:
  NAME | LITERAL_NAME;

dependentName:
  NAME | LITERAL_NAME | anyKeyword;

anyKeyword:
  'function' | 'return' | 'break' | 'and' | 'or' | 'not' |
  'let' | 'var' | 'end' | 'throw' | 'import' | 'include' | 'require';

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

statement:
  variableDecl |
  multiFuncDecl |
  assignmentOrCall;

blockFinalStatement:
  throwStatement;

throwStatement:
  'throw' (exprList)?;

funcFinalStatement:
  returnStatement;

returnStatement:
  'return' (exprList)?;

implicitReturnStatement:;

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
  localAttrList (letDecl | varDecl);

letDecl:
  'let' declList '=' exprList;

varDecl:
  'var' declList '=' exprList;

multiFuncDecl:
  funcDecl+;

funcDecl:
	localAttrList 'function' name funcBody;

funcBody:
  '(' paramList ')' funcBlock 'end';

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
  'not' ('not' doubleNegation)* logicExprArg;

doubleNegation:;

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
  bitShiftExprArg (bitShiftBinaryOperator bitShiftExprArg)+;

bitShiftExprArg:
  bitShiftExpr_Inner;

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

bitShiftBinaryOperator:
  '>>' | '<<';

unaryOperator:
  '+' | '-' | '~';

number:
  INT | FLOAT | EXP | HEX;

string:
  NORMAL_STRING | VERBATIM_STRING | CHAR_STRING;
