parser grammar SonaParser;
options {
  tokenVocab = SonaLexer;
}

/* ----------------- */
/* Blocks and trails */
/* ----------------- */

// The whole program
chunk:
  mainBlock EOF;

/*
The sequence of statements is intentionally lazy, to make the
finishing statement take as much of the block as possible.
*/

// The program's body, supporting top-level statements and conditionals
mainBlock:
  (globalAttrList (topLevelStatement | statement) ';'?)*? globalAttrList ((closingStatement | conditionalStatement) ';'? | trailingStatement);

// openBlock | terminatingBlock | interruptingBlock | returningBlock | conditionalBlock
valueBlock:
  (statement ';'?)*? ((closingStatement | conditionalStatement) ';'? | trailingStatement);
valueTrail:
  (';'? statement)*? (';'? (closingStatement | conditionalStatement) | trailingStatement);

// openBlock | terminatingBlock
freeBlock:
  (statement ';'?)*? (terminatingStatement ';'? | trailingStatement);
freeTrail:
  (';'? statement)*? (';'? terminatingStatement | trailingStatement);

// A block that does not return or throw at all (special handling in a final conditional)
openBlock:
  (statement ';'?)* trailingStatement;
openTrail:
  (';'? statement)*? trailingStatement;

// A block that explicitly returns
returningBlock:
  (statement ';'?)*? returningStatement ';'?;
returningTrail:
  (';'? statement)*? ';'? returningStatement;

// A block that explicitly throws
terminatingBlock:
  (statement ';'?)*? terminatingStatement ';'?;
terminatingTrail:
  (';'? statement)*? ';'? terminatingStatement;

// A block that explicitly interrupts
interruptingBlock:
  (statement ';'?)*? interruptingStatement ';'?;
interruptingTrail:
  (';'? statement)*? ';'? interruptingStatement;

// terminatingBlock | interruptingBlock | returningBlock
returningCoverBlock:
  (statement ';'?)*? closingStatement ';'?;
returningCoverTrail:
  (';'? statement)*? ';'? closingStatement;

// A block that may or may not return or interrupt
conditionalBlock:
  (statement ';'?)*? conditionalStatement ';'?;
conditionalTrail:
  (';'? statement)*? ';'? conditionalStatement;

// A block that may or may not interrupt
interruptibleBlock:
  (statement ';'?)*? interruptibleStatement ';'?;
interruptibleTrail:
  (';'? statement)*? ';'? interruptibleStatement;

// openBlock | terminatingBlock | interruptingBlock | interruptibleBlock | returningBlock | conditionalBlock
conditionalCoverBlock:
  (statement ';'?)*? ((closingStatement | interruptibleStatement | conditionalStatement) ';'? | trailingStatement);
// openTrail | interruptibleTrail | conditionalTrail
conditionalCoverTrail:
  (';'? statement)*? (';'? (interruptibleStatement | conditionalStatement) | trailingStatement);

// terminatingBlock | interruptingBlock
interruptingCoverBlock:
  (statement ';'?)*? (terminatingStatement | interruptingStatement) ';'?;
interruptingCoverTrail:
  (';'? statement)*? ';'? (terminatingStatement | interruptingStatement);

// openBlock | terminatingBlock | interruptingBlock | interruptibleBlock
interruptibleCoverBlock:
  (statement ';'?)*? ((terminatingStatement | interruptingStatement | interruptibleStatement) ';'? | trailingStatement);
// openTrail | interruptibleTrail
interruptibleCoverTrail:
  (';'? statement)*? (';'? interruptibleStatement | trailingStatement);

// openBlock | terminatingBlock
openCoverBlock:
  (statement ';'?)*? (terminatingStatement ';'? | trailingStatement);

// openBlock | interruptibleBlock
openToInterruptibleBlock:
  (statement ';'?)*? (interruptibleStatement ';'? | trailingStatement);

// openBlock | interruptibleBlock | conditionalBlock
openToConditionalBlock:
  (statement ';'?)*? ((interruptibleStatement | conditionalStatement) ';'? | trailingStatement);

// interruptingBlock | interruptibleBlock
interruptingToInterruptibleBlock:
  (statement ';'?)*? (interruptingStatement | interruptibleStatement) ';'?;

// returningBlock | conditionalBlock
returningToConditionalBlock:
  (statement ';'?)*? (returningStatement | conditionalStatement) ';'?;

// Same but never executed
ignoredBlock:
  (statement ';'?)*? ((closingStatement | interruptibleStatement | conditionalStatement) ';'? | trailingStatement);
ignoredTrail:
  (';'? statement)*? (';'? (closingStatement | interruptibleStatement | conditionalStatement) | trailingStatement);

// Same but empty
ignoredEmptyBlock:;
ignoredEmptyTrail:;

/* ----------- */
/* Identifiers */
/* ----------- */

name:
  NAME | LITERAL_NAME | errorUnderscoreReserved;

memberName:
  MEMBER_NAME;

dynamicMemberName:
  DYNAMIC_MEMBER_NAME;

compoundName:
  name ('.' name | memberName)*;

compoundNameGeneric:
  name genericArguments? (('.' name | memberName) genericArguments?)*;

/* ----- */
/* Types */
/* ----- */

typeArgument:
  type?;

type:
  atomicType |
  nullableType |
  conjunctionType |
  inlineSourceFree;

nullableType:
  atomicType 'or' 'null' |
  'null' 'or' (atomicType | 'else');

conjunctionType:
  atomicType ('and' atomicType)* ('and' (atomicType | 'else'));

atomicType:
  (
    typeSuffix |
    compoundNameGeneric |
    primitiveType |
    unit |
    functionType |
    nestedType |
    tupleType |
    classTupleType |
    structTupleType |
    anonymousRecordType |
    anonymousClassRecordType |
    anonymousStructRecordType
  ) typeSuffix*;

primitiveType:
  'bool' | 'int' | 'uint' |
  'int8' | 'int16' | 'int32' | 'int64' | 'int128' |
  'uint8' | 'uint16' | 'uint32' | 'uint64' | 'uint128' |
  'nativeint' | 'unativeint' | 'bigint' |
  'float' | 'float16' | 'float32' | 'float64' | 'decimal' |
  'char' | 'string' | 'object' | 'void' |
  'exception';

functionType:
  'function' ('(' paramTypesList ')')? ('as' type)?;

paramTypesList:
  paramTypesTuple (';' paramTypesTuple)*;

paramTypesTuple:
  (type | typeArgument (',' typeArgument)+)?;

nestedType:
  '(' type ')';

tupleType:
  '(' typeArgument (',' typeArgument)+ ')';

classTupleType:
  '(' 'as' 'class' ';'? typeArgument (',' typeArgument)+ ')';

structTupleType:
  '(' 'as' 'struct' ';'? typeArgument (',' typeArgument)+ ')';

anonymousRecordType:
  '{' anonymousRecordMemberDeclaration (',' anonymousRecordMemberDeclaration)* '}';

anonymousClassRecordType:
  '{' 'as' 'class' ';'? anonymousRecordMemberDeclaration (',' anonymousRecordMemberDeclaration)* '}';

anonymousStructRecordType:
  '{' 'as' 'struct' ';'? anonymousRecordMemberDeclaration (',' anonymousRecordMemberDeclaration)* '}';

anonymousRecordMemberDeclaration:
  name ('as' type)?;

typeSuffix:
  arrayTypeSuffix |
  multiArrayTypeSuffix |
  optionTypeSuffix |
  sequenceTypeSuffix;

arrayTypeSuffix: '[' ']';
multiArrayTypeSuffix: '[' ','+ ']';
optionTypeSuffix: '?';
sequenceTypeSuffix: '..';

genericArguments:
  '<' genericArgument (',' genericArgument)* '>';

genericArgument:
  typeArgument |
  measureArgument |
  literalArgument;

measureArgument:
  'unit'? measureExpression;

measureExpression:
  measureOperand (('*' | '/') measureOperand)*;

measureOperand:
  (number | compoundNameGeneric | '(' measureExpression ')') ('^' ('-')? number)?;

literalArgument:
  'const'? expression;

/* ---------- */
/* Attributes */
/* ---------- */

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
  WHITESPACE* (compoundName | compiledNameAttr) (WHITESPACE* (attrNamedArg | attrPosArg) (WHITESPACE+ (attrNamedArg | attrPosArg))*)? WHITESPACE*;

compiledNameAttr:
  anyStringArg;

attrPosArg:
   unaryExpr;

attrNamedArg:
  name WHITESPACE* '=' WHITESPACE* unaryExpr;

/* ------------------ */
/* General statements */
/* ------------------ */

// Plain statement, allowed anywhere
statement:
  variableDecl |
  multiFuncDecl |
  inlineFuncDecl |
  inlineCaseFuncDecl |
  memberDiscard |
  memberOrAssignment |
  echoStatement |
  yieldStatement |
  yieldEachStatement |
  followDiscardStatement |
  followStatement |
  inlineSourceFree |
  ifStatementFree |
  doStatementFree |
  whileStatementFree |
  whileStatementFreeInterrupted |
  repeatStatementFree |
  repeatStatementFreeInterrupted |
  forStatementFree |
  forStatementFreeInterrupted |
  switchStatementFree |
  switchStatementFreeInterrupted |
  tryFinallyStatementFree |
  tryCatchFinallyStatementFree |
  tryCatchStatementFree;

// A statement that must be located at the end of a block and stops its execution
closingStatement:
  returningStatement |
  interruptingStatement |
  terminatingStatement;

// Special statements

echoStatement:
  'echo' (anyStringArg | expression) (',' expression)*;

// Simple control statements

implicitReturnStatement:;

returnStatement:
  'return' expression?;

returnOptionStatement:
  'return' atomicExpr '?';

yieldStatement:
  'yield' (expression | errorMissingExpression);

yieldEachStatement:
  'yield' spreadExpression;

yieldBreakStatement:
  YIELD_BREAK expression?;

breakStatement:
  'break' expression?;

continueStatement:
  'continue' expression?;

throwStatement:
  'throw' expression?;

withStatement:
  'with' (expression | errorMissingExpression) valueTrail;

followDiscardStatement:
  'follow' memberDiscard;

followStatement:
  'follow' (expression | errorMissingExpression);

followWithTrailing:
  FOLLOW_WITH (expression | errorMissingExpression) freeTrail;

followWithTerminating:
  FOLLOW_WITH (expression | errorMissingExpression) terminatingTrail;

followWithInterrupting:
  FOLLOW_WITH (expression | errorMissingExpression) interruptingTrail;

followWithInterruptible:
  FOLLOW_WITH (expression | errorMissingExpression) interruptibleTrail;

followWithReturning:
  FOLLOW_WITH (expression | errorMissingExpression) returningTrail;

followWithConditional:
  FOLLOW_WITH (expression | errorMissingExpression) conditionalTrail;

// A free statement that must be at the end of a block
trailingStatement:
  followWithTrailing ';'? |
  implicitReturnStatement;

// A statement that has a returning path and all other paths are closing
returningStatement:
  returnOptionStatement |
  returnStatement |
  yieldBreakStatement |
  withStatement |
  followWithReturning |
  inlineSourceReturning |
  doStatementReturning |
  ifStatementReturning |
  ifStatementReturningTrailFromElse |
  ifStatementReturningTrail |
  whileStatementReturningTrail |
  repeatStatementReturningTrail |
  forStatementReturningTrail |
  switchStatementReturning |
  switchStatementReturningTrail |
  tryFinallyStatementReturning |
  tryFinallyStatementReturningTrail |
  tryCatchFinallyStatementReturning |
  tryCatchFinallyStatementReturningTrail |
  tryCatchStatementReturning |
  tryCatchStatementReturningTrail;

// A statement that has interrupting paths and all other paths are terminating
interruptingStatement:
  breakStatement |
  continueStatement |
  followWithInterrupting |
  doStatementInterrupting |
  doStatementInterruptingTrail |
  ifStatementInterrupting |
  ifStatementInterruptingTrail;

// A statement that has interrupting paths and all other paths are open or terminating
interruptibleStatement:
  followWithInterruptible |
  doStatementInterruptible |
  ifStatementInterruptible;

// A statement that has returning paths and open paths
conditionalStatement:
  followWithConditional |
  doStatementConditional |
  ifStatementConditional |
  whileStatementConditional |
  repeatStatementConditional |
  forStatementConditional |
  switchStatementConditional |
  tryFinallyStatementConditional |
  tryCatchFinallyStatementConditional |
  tryCatchStatementConditional;

// A statement that only has terminating paths
terminatingStatement:
  throwStatement |
  followWithTerminating |
  inlineSourceTerminating |
  doStatementTerminating |
  ifStatementTerminating |
  whileStatementTerminating |
  repeatStatementTerminating |
  switchStatementTerminating |
  switchStatementTerminatingInterrupted |
  tryFinallyStatementTerminating |
  tryCatchFinallyStatementTerminating |
  tryCatchStatementTerminating;

// A statement that can be used in special expressions
// This includes all statements that contain blocks,
// but do no have any requirements on their trails.
expressionStatement:
  doStatementTerminating |
  doStatementFree |
  doStatementInterrupting |
  doStatementInterruptible |
  doStatementReturning |
  doStatementConditional |
  ifStatementTerminating |
  ifStatementFree |
  ifStatementInterrupting |
  ifStatementInterruptible |
  ifStatementReturning |
  ifStatementConditional |
  whileStatementTerminating |
  whileStatementFree |
  whileStatementFreeInterrupted |
  whileStatementConditional |
  repeatStatementTerminating |
  repeatStatementFree |
  repeatStatementFreeInterrupted |
  repeatStatementConditional |
  forStatementFree |
  forStatementFreeInterrupted |
  forStatementConditional |
  switchStatementTerminating |
  switchStatementTerminatingInterrupted |
  switchStatementFree |
  switchStatementFreeInterrupted |
  switchStatementReturning |
  switchStatementConditional |
  tryFinallyStatementTerminating |
  tryCatchFinallyStatementTerminating |
  tryCatchStatementTerminating |
  tryFinallyStatementFree |
  tryCatchFinallyStatementFree |
  tryCatchStatementFree |
  tryFinallyStatementConditional |
  tryCatchFinallyStatementConditional |
  tryCatchStatementConditional;

/* ---------------- */
/* Block statements */
/* ---------------- */

// `do`

doStatementFree:
  'do' freeBlock 'end';

doStatementTerminating:
  'do' terminatingBlock 'end' (ignoredEmptyTrail | ignoredTrail);

doStatementInterrupting:
  // Body interrupts, trail is ignored
  'do' interruptingBlock 'end' (ignoredEmptyTrail | ignoredTrail);

doStatementInterruptingTrail:
  // Body is interruptible but trail terminates or interrupts
  'do' interruptibleBlock 'end' interruptingCoverTrail;

doStatementInterruptible:
  // Body is interruptible, trail is open or interruptible
  'do' interruptibleBlock 'end' interruptibleCoverTrail;

doStatementReturning:
  // Body returns, trail is ignored
  'do' returningBlock 'end' (ignoredEmptyTrail | ignoredTrail) |
  // Body is interruptible but trail returns
  'do' interruptibleBlock 'end' returningTrail |
  // Body is conditional but trail closes
  'do' conditionalBlock 'end' returningCoverTrail;

doStatementConditional:
  // Body is conditional, trail is open, interruptible or conditional
  'do' conditionalBlock 'end' conditionalCoverTrail |
  // Body is interrutible but trail is conditional
  'do' interruptibleBlock 'end' conditionalTrail;

// `if`

if:
  'if' expression 'then';

elseif:
  'elseif' expression 'then';

caseIf:
  'if' 'let' pattern '=' expression whenClause? 'then';

caseElseif:
  'elseif' 'let' pattern '=' expression whenClause? 'then';

else:
  'else';

// Free-standing (may throw from all branches but this is not preferred).
ifStatementFree:
  (caseIf | if) freeBlock ((caseElseif | elseif) freeBlock)* (else freeBlock)? 'end';

ifStatementTerminating:
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* else terminatingBlock 'end' ignoredTrail;

// The following rules are auto-generated.

ifStatementInterrupting:
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    else interruptingCoverBlock 'end' ignoredTrail |
    else terminatingBlock 'end' ignoredTrail 
  ) |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    else interruptingBlock 'end' ignoredTrail |
    (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
      else interruptingCoverBlock 'end' ignoredTrail |
      else terminatingBlock 'end' ignoredTrail 
    ) 
  );

ifStatementInterruptingTrail:
  (caseIf | if) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 'end' interruptingCoverTrail |
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    (else openToInterruptibleBlock)? |
    (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
  ) 'end' interruptingCoverTrail |
  (caseIf | if) openBlock ((caseElseif | elseif) openCoverBlock)* (
    else interruptingToInterruptibleBlock |
    (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
  ) 'end' interruptingCoverTrail |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    else interruptibleBlock |
    (caseElseif | elseif) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
      (else openToInterruptibleBlock)? |
      (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
    ) |
    (caseElseif | elseif) openBlock ((caseElseif | elseif) openCoverBlock)* (
      else interruptingToInterruptibleBlock |
      (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
    ) 
  ) 'end' interruptingCoverTrail;

ifStatementInterruptible:
  (caseIf | if) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 'end' interruptibleCoverTrail |
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    (else openToInterruptibleBlock)? |
    (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
  ) 'end' interruptibleCoverTrail |
  (caseIf | if) openBlock ((caseElseif | elseif) openCoverBlock)* (
    else interruptingToInterruptibleBlock |
    (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
  ) 'end' interruptibleCoverTrail |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    else interruptibleBlock |
    (caseElseif | elseif) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
      (else openToInterruptibleBlock)? |
      (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
    ) |
    (caseElseif | elseif) openBlock ((caseElseif | elseif) openCoverBlock)* (
      else interruptingToInterruptibleBlock |
      (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (else interruptibleCoverBlock)? 
    ) 
  ) 'end' interruptibleCoverTrail;

ifStatementReturning:
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    else returningBlock 'end' ignoredTrail |
    (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
      else returningCoverBlock 'end' ignoredTrail |
      else terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  (caseIf | if) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
    else returningCoverBlock 'end' ignoredTrail |
    else terminatingBlock 'end' ignoredTrail 
  ) |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    else returningBlock 'end' ignoredTrail |
    (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
      else returningBlock 'end' ignoredTrail |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        else returningCoverBlock 'end' ignoredTrail |
        else terminatingBlock 'end' ignoredTrail 
      ) 
    ) |
    (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
      else returningCoverBlock 'end' ignoredTrail |
      else terminatingBlock 'end' ignoredTrail 
    ) 
  );

ifStatementReturningTrailFromElse:
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    (
      else conditionalBlock |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 
    ) 'end' interruptingCoverTrail |
    (
      else conditionalBlock |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 
    ) 'end' returningTrail 
  ) |
  (caseIf | if) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 'end' returningCoverTrail |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    (
      else conditionalBlock |
      (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
        else conditionalBlock |
        (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 
    ) 'end' interruptingCoverTrail |
    (
      else (conditionalBlock | interruptibleBlock) |
      (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
        else conditionalBlock |
        (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* else openToConditionalBlock 
    ) 'end' returningTrail 
  );

ifStatementReturningTrail:
  (caseIf | if) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 'end' returningCoverTrail |
  (caseIf | if) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
    (
      else returningToConditionalBlock |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' interruptingCoverTrail |
    (
      (else (returningBlock | conditionalBlock | terminatingBlock))? |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' returningTrail 
  ) |
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    (
      else conditionalBlock |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      (else conditionalBlock)? |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' returningTrail 
  ) |
  (caseIf | if) openBlock ((caseElseif | elseif) openCoverBlock)* (
    (
      else returningToConditionalBlock |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      else returningToConditionalBlock |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' returningTrail 
  ) |
  (caseIf | if) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
    (else openToConditionalBlock)? |
    (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
  ) 'end' returningCoverTrail |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    (
      else conditionalBlock |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
        else conditionalBlock |
        (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
          (else openToConditionalBlock)? |
          (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) openBlock ((caseElseif | elseif) openCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      else (conditionalBlock | interruptibleBlock) |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
        (else conditionalBlock)? |
        (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
          (else openToConditionalBlock)? |
          (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) openBlock ((caseElseif | elseif) openCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' returningTrail 
  );

ifStatementConditional:
  (caseIf | if) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 'end' conditionalCoverTrail |
  (caseIf | if) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
    (
      (else (returningBlock | conditionalBlock | terminatingBlock))? |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' conditionalTrail |
    (
      else returningToConditionalBlock |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' interruptibleCoverTrail 
  ) |
  (caseIf | if) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
    (
      (else conditionalBlock)? |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' conditionalTrail |
    (
      else conditionalBlock |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) |
  (caseIf | if) openBlock ((caseElseif | elseif) openCoverBlock)* (
    (
      else returningToConditionalBlock |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' conditionalTrail |
    (
      else returningToConditionalBlock |
      (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) |
  (caseIf | if) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
    (else openToConditionalBlock)? |
    (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
  ) 'end' conditionalCoverTrail |
  (caseIf | if) terminatingBlock ((caseElseif | elseif) terminatingBlock)* (
    (
      else (conditionalBlock | interruptibleBlock) |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
        (else conditionalBlock)? |
        (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
          (else openToConditionalBlock)? |
          (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) openBlock ((caseElseif | elseif) openCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' conditionalTrail |
    (
      else conditionalBlock |
      (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (caseElseif | elseif) interruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      (caseElseif | elseif) interruptingBlock ((caseElseif | elseif) interruptingCoverBlock)* (
        else conditionalBlock |
        (caseElseif | elseif) conditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) openToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
          (else openToConditionalBlock)? |
          (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) openBlock ((caseElseif | elseif) openCoverBlock)* (
        else returningToConditionalBlock |
        (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (caseElseif | elseif) interruptingToInterruptibleBlock ((caseElseif | elseif) interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          (caseElseif | elseif) returningToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      (caseElseif | elseif) returningBlock ((caseElseif | elseif) returningCoverBlock)* (
        (else openToConditionalBlock)? |
        (caseElseif | elseif) openToConditionalBlock ((caseElseif | elseif) conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) 
    ) 'end' interruptibleCoverTrail 
  );

// `while`

while:
  'while' expression 'do';

whileTrue:
  WHILE_TRUE_DO;

whileStatementFree:
  while freeBlock 'end';

whileStatementFreeInterrupted:
  // Body may interrupt
  (whileTrue | while) interruptingToInterruptibleBlock 'end';

// `while true do` with interrupting body could be terminating too,
// but we would need to guarantee that only `continue` appears.
whileStatementTerminating:
  // Body either throws or keeps looping forever
  whileTrue freeBlock 'end' ignoredTrail;

/*
// `while true do` could be returning but since a returning block
// may also interrupt, we cannot guarantee that it won't interrupt here.
whileStatementReturning:
  // Body may return, trail is ignored
  whileTrue returningToConditionalBlock 'end' ignoredTrail;
*/

whileStatementReturningTrail:
  // Body is returning or conditional, but the trail is closing
  (whileTrue | while) returningToConditionalBlock 'end' returningCoverTrail;

whileStatementConditional:
  // Body is returning or conditional, trail is open, interruptible, or conditional
  (whileTrue | while) returningToConditionalBlock 'end' conditionalCoverTrail;

// `repeat`

until:
  'until' expression;

repeatStatementFree:
  'repeat' freeBlock until;

repeatStatementFreeInterrupted:
  // Body may interrupt
  'repeat' interruptingToInterruptibleBlock until;

repeatStatementTerminating:
  // Body always throws and condition is never checked
  'repeat' terminatingBlock until ignoredTrail;

/*
// Same issue as with returning `while true do`.
repeatStatementReturning:
  // Body may return, trail is ignored
  'repeat' returningBlock until ignoredTrail;
*/

repeatStatementReturningTrail:
  // Body is returning or conditional, but the trail is closing
  'repeat' returningToConditionalBlock until returningCoverTrail;

repeatStatementConditional:
  // Body is returning or conditional, trail is open, interruptible, or conditional
  'repeat' returningToConditionalBlock until conditionalCoverTrail;

// `for`

for:
  // Step is a primitive expression - can be evaluated out of order
  forRangePrimitiveStep |
  // Step is provided
  forRangeStep |
  // Step is not provided
  forRange |
  // Arbitrary collection with step (error for now)
  forSimpleStep |
  // Arbitrary collection
  forSimple;

forSimple:
  'for' declaration 'in' forRangeExpression 'do';

forSimpleStep:
  'for' declaration 'in' forRangeExpression 'by' expression 'do';

forRange:
  'for' declaration 'in' forRangeExpression '..' forRangeExpression 'do';

forRangeStep:
  'for' declaration 'in' forRangeExpression '..' forRangeExpression 'by' expression 'do';

forRangePrimitiveStep:
  'for' declaration 'in' forRangeExpression '..' forRangeExpression 'by' primitiveExpr 'do';

forRangeExpression:
  concatExpr_Inner;

forStatementFree:
  for freeBlock 'end';

forStatementFreeInterrupted:
  // Body may interrupt
  for interruptingToInterruptibleBlock 'end';

forStatementReturningTrail:
  // Body is returning or conditional, but the trail is closing
  for returningToConditionalBlock 'end' returningCoverTrail;

forStatementConditional:
  // Body is returning or conditional, trail is open, interruptible, or conditional
  for returningToConditionalBlock 'end' conditionalCoverTrail;

// `switch`

switch:
  'switch' expression;

case:
  'case' (pattern whenClause? | whenClause) 'do';

whenClause:
  'when' expression;

// Free-standing (may throw from all branches but this is not preferred).
// Presence of at least one case/else is ensured in code.
switchStatementFree:
  switch (case freeBlock)* (else freeBlock)? 'end';

// Free-standing (may interrupt in all branches but this is not preferred)
switchStatementFreeInterrupted:
  switch (case freeBlock)* (
    case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    else interruptingToInterruptibleBlock
  ) 'end';

// Having no branch is technically invalid, but is handled through code.
switchStatementTerminating:
  switch (case terminatingBlock)* (else terminatingBlock)? 'end' ignoredTrail;

// An "interrupted" branch means another must be entered, so such a branch
// does not affect the statement's category.
switchStatementTerminatingInterrupted:
  switch (case terminatingBlock)* (
    case interruptingBlock (case interruptingCoverBlock)* (else interruptingCoverBlock)? |
    else interruptingBlock
  ) 'end' ignoredTrail;

// The following rules are auto-generated.

switchStatementReturning:
  switch (
    case interruptingBlock (case interruptingCoverBlock)* (
      case returningBlock (case returningCoverBlock)* (
        'end' ignoredTrail |
        else returningCoverBlock 'end' ignoredTrail |
        else terminatingBlock 'end' ignoredTrail 
      ) |
      else returningBlock 'end' ignoredTrail 
    ) |
    case returningBlock (case returningCoverBlock)* (
      'end' ignoredTrail |
      else returningCoverBlock 'end' ignoredTrail |
      else terminatingBlock 'end' ignoredTrail 
    ) |
    case terminatingBlock (case terminatingBlock)* (
      case interruptingBlock (case interruptingCoverBlock)* (
        case returningBlock (case returningCoverBlock)* (
          'end' ignoredTrail |
          else returningCoverBlock 'end' ignoredTrail |
          else terminatingBlock 'end' ignoredTrail 
        ) |
        else returningBlock 'end' ignoredTrail 
      ) |
      case returningBlock (case returningCoverBlock)* (
        'end' ignoredTrail |
        else returningCoverBlock 'end' ignoredTrail |
        else terminatingBlock 'end' ignoredTrail 
      ) |
      else returningBlock 'end' ignoredTrail 
    ) |
    else returningBlock 'end' ignoredTrail 
  );

switchStatementReturningTrail:
  switch (
    (
      else conditionalBlock |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case openToInterruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case terminatingBlock (case terminatingBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case openToInterruptibleBlock (case interruptibleCoverBlock)* (
            else returningToConditionalBlock |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningBlock (case returningCoverBlock)* (
            else openToConditionalBlock |
            case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            else returningToConditionalBlock |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      else (conditionalBlock | interruptibleBlock) |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case openToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case terminatingBlock (case terminatingBlock)* (
        else (conditionalBlock | interruptibleBlock) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case openToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else (returningBlock | conditionalBlock | terminatingBlock))? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningBlock (case returningCoverBlock)* (
            else openToConditionalBlock |
            case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else (returningBlock | conditionalBlock | terminatingBlock))? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) 
    ) 'end' returningTrail 
  );

switchStatementConditional:
  switch (
    (
      else (conditionalBlock | interruptibleBlock) |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        (else (returningBlock | conditionalBlock | terminatingBlock))? |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case openToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case terminatingBlock (case terminatingBlock)* (
        else (conditionalBlock | interruptibleBlock) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          (else (returningBlock | conditionalBlock | terminatingBlock))? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case openToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else (returningBlock | conditionalBlock | terminatingBlock))? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningBlock (case returningCoverBlock)* (
            else openToConditionalBlock |
            case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else (returningBlock | conditionalBlock | terminatingBlock))? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) 
    ) 'end' conditionalTrail |
    (
      else conditionalBlock |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case openToInterruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case terminatingBlock (case terminatingBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case openToInterruptibleBlock (case interruptibleCoverBlock)* (
            else returningToConditionalBlock |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningBlock (case returningCoverBlock)* (
            else openToConditionalBlock |
            case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            else returningToConditionalBlock |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) 
        ) |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) 
      ) 
    ) 'end' interruptibleCoverTrail 
  );

// `try`

try:
  'try';

finallyBranch:
  'finally';

catchCase:
  'catch' (pattern whenClause? | whenClause) 'do';

// Free-standing (may throw from all branches but this is not preferred).
// Branches may be interrupting but this is not currently supported.
tryCatchStatementFree:
  try interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* 'end';

tryFinallyStatementFree:
  try interruptibleCoverBlock finallyBranch interruptibleCoverBlock 'end';

tryCatchFinallyStatementFree:
  try interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)+ finallyBranch interruptibleCoverBlock 'end';

/*
// Ignoring interrupting
tryCatchStatementTerminating:
  try terminatingBlock ((case | catchCase) terminatingBlock)* 'end' ignoredTrail;

// At least one block must be terminating.
tryFinallyStatementTerminating:
  try (
    terminatingBlock finallyBranch interruptibleCoverBlock |
    interruptibleCoverBlock finallyBranch terminatingBlock
  ) 'end' ignoredTrail;

tryCatchFinallyStatementTerminating:
  try (
    terminatingBlock ((case | catchCase) terminatingBlock)+ finallyBranch interruptibleCoverBlock |
    interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)+ finallyBranch terminatingBlock
  ) 'end' ignoredTrail;
*/

// The following rules are auto-generated.

tryCatchStatementTerminating:
  try interruptingBlock (
    (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* 'end' ignoredTrail |
    'end' ignoredTrail 
  ) |
  try terminatingBlock (
    (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* 'end' ignoredTrail |
    (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* 'end' ignoredTrail |
      'end' ignoredTrail 
    ) |
    'end' ignoredTrail 
  ) ;

tryCatchStatementConditional:
  try conditionalBlock (
    |
    (case | catchCase) conditionalCoverBlock ((case | catchCase) conditionalCoverBlock)* 
  ) 'end' conditionalCoverTrail |
  try interruptibleBlock (
    (
      |
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
    ) 'end' interruptibleCoverTrail 
  ) |
  try interruptingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
    ) 'end' interruptibleCoverTrail 
  ) |
  try openBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) 
      ) 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) |
  try returningBlock (
    (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
    (case | catchCase) returningCoverBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
  ) 'end' conditionalCoverTrail |
  try terminatingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
          ) |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
          ) 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) ;



tryCatchStatementReturning:
  try interruptingBlock (
    (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail |
    (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail 
  ) |
  try returningBlock (
    (case | catchCase) returningCoverBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail |
    'end' ignoredTrail 
  ) |
  try terminatingBlock (
    (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail |
    (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail |
    (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* 'end' ignoredTrail 
    ) 
  ) ;


tryCatchStatementReturningTrail:
  try conditionalBlock (
    |
    (case | catchCase) conditionalCoverBlock ((case | catchCase) conditionalCoverBlock)* 
  ) 'end' returningCoverTrail |
  try interruptibleBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
    ) 'end' interruptingCoverTrail |
    (
      |
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' returningTrail 
  ) |
  try interruptingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
    ) 'end' returningTrail 
  ) |
  try openBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) 
      ) 
    ) 'end' returningTrail 
  ) |
  try returningBlock (
    (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
    (case | catchCase) returningCoverBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
  ) 'end' returningCoverTrail |
  try terminatingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
          ) |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
          ) 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* 
      ) 
    ) 'end' returningTrail 
  ) ;


tryFinallyStatementTerminating:
  try openToInterruptibleBlock (
    finallyBranch interruptingBlock 'end' ignoredTrail |
    finallyBranch terminatingBlock 'end' ignoredTrail 
  ) |
  try interruptingCoverBlock (
    finallyBranch interruptibleBlock 'end' ignoredTrail |
    finallyBranch interruptingBlock 'end' ignoredTrail |
    finallyBranch openBlock 'end' ignoredTrail |
    finallyBranch terminatingBlock 'end' ignoredTrail 
  ) ;

tryFinallyStatementConditional:
  try conditionalBlock finallyBranch openToConditionalBlock 'end' conditionalCoverTrail |
  try interruptibleBlock (
    finallyBranch openToConditionalBlock 'end' conditionalTrail |
    finallyBranch conditionalBlock 'end' interruptibleCoverTrail 
  ) |
  try openBlock (
    finallyBranch (conditionalBlock | interruptibleBlock) 'end' conditionalTrail |
    finallyBranch conditionalBlock 'end' interruptibleCoverTrail 
  ) ;



tryFinallyStatementReturning:
  try conditionalBlock (
    finallyBranch interruptingBlock 'end' ignoredTrail |
    finallyBranch returningBlock 'end' ignoredTrail |
    finallyBranch terminatingBlock 'end' ignoredTrail 
  ) |
  try openToInterruptibleBlock (
    finallyBranch returningBlock 'end' ignoredTrail 
  ) |
  try interruptingCoverBlock (
    finallyBranch conditionalBlock 'end' ignoredTrail |
    finallyBranch returningBlock 'end' ignoredTrail 
  ) |
  try returningBlock (
    finallyBranch conditionalBlock 'end' ignoredTrail |
    finallyBranch interruptibleBlock 'end' ignoredTrail |
    finallyBranch interruptingBlock 'end' ignoredTrail |
    finallyBranch openBlock 'end' ignoredTrail |
    finallyBranch returningBlock 'end' ignoredTrail |
    finallyBranch terminatingBlock 'end' ignoredTrail 
  ) ;


tryFinallyStatementReturningTrail:
  try conditionalBlock finallyBranch openToConditionalBlock 'end' returningCoverTrail |
  try interruptibleBlock (
    finallyBranch conditionalBlock 'end' interruptingCoverTrail |
    finallyBranch openToConditionalBlock 'end' returningTrail 
  ) |
  try openBlock (
    finallyBranch conditionalBlock 'end' interruptingCoverTrail |
    finallyBranch (conditionalBlock | interruptibleBlock) 'end' returningTrail 
  ) ;


tryCatchFinallyStatementTerminating:
  try interruptibleBlock (
    (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  try interruptingBlock (
    (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch interruptibleBlock 'end' ignoredTrail |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch openBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  try openBlock (
    (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  try terminatingBlock (
    (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch interruptibleBlock 'end' ignoredTrail |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch openBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch interruptibleBlock 'end' ignoredTrail |
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch openBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch interruptibleBlock 'end' ignoredTrail |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch openBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) ;

tryCatchFinallyStatementConditional:
  try conditionalBlock (case | catchCase) conditionalCoverBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 'end' conditionalCoverTrail |
  try interruptibleBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) |
  try interruptingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
    ) 'end' interruptibleCoverTrail 
  ) |
  try openBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch (conditionalBlock | interruptibleBlock) |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) |
  try returningBlock (
    (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
    (case | catchCase) returningCoverBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
  ) 'end' conditionalCoverTrail |
  try terminatingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch (conditionalBlock | interruptibleBlock) |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch openToConditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          finallyBranch (conditionalBlock | interruptibleBlock) |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch openToConditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' conditionalTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch conditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch conditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' interruptibleCoverTrail 
  ) ;



tryCatchFinallyStatementReturning:
  try conditionalBlock (
    (case | catchCase) conditionalCoverBlock ((case | catchCase) conditionalCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  try interruptibleBlock (
    (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) 
  ) |
  try interruptingBlock (
    (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (
        (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch conditionalBlock 'end' ignoredTrail |
        finallyBranch interruptibleBlock 'end' ignoredTrail |
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch openBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch conditionalBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (
      (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch conditionalBlock 'end' ignoredTrail |
      finallyBranch interruptibleBlock 'end' ignoredTrail |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch openBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  try openBlock (
    (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) 
  ) |
  try returningBlock (
    (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) returningCoverBlock ((case | catchCase) returningCoverBlock)* (
      (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch conditionalBlock 'end' ignoredTrail |
      finallyBranch interruptibleBlock 'end' ignoredTrail |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch openBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) 
  ) |
  try terminatingBlock (
    (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* (
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (
        (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch conditionalBlock 'end' ignoredTrail |
        finallyBranch interruptibleBlock 'end' ignoredTrail |
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch openBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch conditionalBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (
      (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch conditionalBlock 'end' ignoredTrail |
      finallyBranch interruptibleBlock 'end' ignoredTrail |
      finallyBranch interruptingBlock 'end' ignoredTrail |
      finallyBranch openBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail |
      finallyBranch terminatingBlock 'end' ignoredTrail 
    ) |
    (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* (
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
            finallyBranch interruptingBlock 'end' ignoredTrail |
            finallyBranch returningBlock 'end' ignoredTrail |
            finallyBranch terminatingBlock 'end' ignoredTrail 
          ) |
          finallyBranch returningBlock 'end' ignoredTrail 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (
          (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
            finallyBranch interruptingBlock 'end' ignoredTrail |
            finallyBranch returningBlock 'end' ignoredTrail |
            finallyBranch terminatingBlock 'end' ignoredTrail 
          ) |
          finallyBranch conditionalBlock 'end' ignoredTrail |
          finallyBranch interruptibleBlock 'end' ignoredTrail |
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch openBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch conditionalBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
            finallyBranch interruptingBlock 'end' ignoredTrail |
            finallyBranch returningBlock 'end' ignoredTrail |
            finallyBranch terminatingBlock 'end' ignoredTrail 
          ) |
          finallyBranch returningBlock 'end' ignoredTrail 
        ) |
        finallyBranch returningBlock 'end' ignoredTrail 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (
        (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* (
          finallyBranch interruptingBlock 'end' ignoredTrail |
          finallyBranch returningBlock 'end' ignoredTrail |
          finallyBranch terminatingBlock 'end' ignoredTrail 
        ) |
        finallyBranch conditionalBlock 'end' ignoredTrail |
        finallyBranch interruptibleBlock 'end' ignoredTrail |
        finallyBranch interruptingBlock 'end' ignoredTrail |
        finallyBranch openBlock 'end' ignoredTrail |
        finallyBranch returningBlock 'end' ignoredTrail |
        finallyBranch terminatingBlock 'end' ignoredTrail 
      ) |
      finallyBranch conditionalBlock 'end' ignoredTrail |
      finallyBranch returningBlock 'end' ignoredTrail 
    ) 
  ) ;


tryCatchFinallyStatementReturningTrail:
  try conditionalBlock (case | catchCase) conditionalCoverBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 'end' returningCoverTrail |
  try interruptibleBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleCoverBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' returningTrail 
  ) |
  try interruptingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingCoverBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
    ) 'end' returningTrail 
  ) |
  try openBlock (
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openCoverBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch (conditionalBlock | interruptibleBlock) |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) 
    ) 'end' returningTrail 
  ) |
  try returningBlock (
    (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
    (case | catchCase) returningCoverBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
  ) 'end' returningCoverTrail |
  try terminatingBlock (
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch conditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch conditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          finallyBranch conditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch conditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' interruptingCoverTrail |
    (
      (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
        finallyBranch openToConditionalBlock |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) |
      (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
        finallyBranch (conditionalBlock | interruptibleBlock) |
        (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) 
      ) |
      (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
      (case | catchCase) terminatingBlock ((case | catchCase) terminatingBlock)* (
        (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
        (case | catchCase) interruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
          finallyBranch openToConditionalBlock |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) interruptingBlock ((case | catchCase) interruptingCoverBlock)* (
          (case | catchCase) conditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) openToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch openToConditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) |
          (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
        ) |
        (case | catchCase) openBlock ((case | catchCase) openCoverBlock)* (
          finallyBranch (conditionalBlock | interruptibleBlock) |
          (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock |
          (case | catchCase) interruptingToInterruptibleBlock ((case | catchCase) interruptibleCoverBlock)* (
            finallyBranch openToConditionalBlock |
            (case | catchCase) returningToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
          ) 
        ) |
        (case | catchCase) returningBlock ((case | catchCase) returningCoverBlock)* (case | catchCase) openToConditionalBlock ((case | catchCase) conditionalCoverBlock)* finallyBranch openToConditionalBlock 
      ) 
    ) 'end' returningTrail 
  ) ;

/* -------------------- */
/* Top-level statements */
/* -------------------- */

topLevelStatement:
  importStatement |
  importTypeStatement |
  importFileStatement |
  includeStatement |
  requireStatement |
  packageStatement;

importStatement:
  'import' symbolArg;

importTypeStatement:
  'import' symbolContentsArg;

symbolContentsArg:
  compoundNameGeneric '.' '*' |
  '(' symbolContentsArg ')';

importFileStatement:
  'import' stringArg;

includeStatement:
  'include' stringArg;

requireStatement:
  'require' stringArg;

packageStatement:
  localAttrList 'package' name mainBlock 'end';

/* ---------------------------- */
/* Declarations and assignments */
/* ---------------------------- */

variableDecl:
  localAttrList (letDecl | varDecl | constDecl | useDecl | useVarDecl) |
  // Attributes are captured inside
  lazyDecl;

letDecl:
  'let' (multiDeclAssignment | declaration '=' expression);

varDecl:
  'var' (multiDeclAssignment | declaration '=' expression);

constDecl:
  'const' (multiDeclAssignment | declaration '=' expression);

useDecl:
  'use' (multiDeclAssignment | declaration '=' expression);

useVarDecl:
  'use' 'var' (multiDeclAssignment | declaration '=' expression);

lazyDecl:
  localAttrList 'lazy' declaration '=' expression (',' declaration '=' expression)*;

multiDeclAssignment:
  declaration '=' expression (',' declaration '=' expression)+;

multiFuncDecl:
  (funcDecl | caseFuncDecl)+;

funcDecl:
  localAttrList 'function' name optionSuffix? funcBody;

inlineFuncDecl:
  localAttrList 'inline' 'function' name optionSuffix? funcBody;

caseFuncDecl:
  localAttrList 'case' 'function' caseFuncName optionSuffix? funcBody;

inlineCaseFuncDecl:
  localAttrList 'inline' 'case' 'function' caseFuncName optionSuffix? funcBody;

caseFuncName:
  name | '(' name ('or' name)* ')';

funcBody:
  paramList ('as' type ';'?)? valueBlock 'end';

paramList:
  ('(' paramTuple (';' paramTuple)* ')')+;

paramTuple:
  (declaration (',' declaration)*)?;

declaration:
  localAttrList 'inline'? ((optionalName | name) ('as' type)? | pattern);

optionalName:
  name '?';

memberOrAssignment:
  (altMemberExpr | memberExpr) assignment?;

assignment:
  '=' expression;

memberDiscard:
  (altMemberExpr | memberExpr) '!';

/* -------- */
/* Patterns */
/* -------- */

patternArgument:
  pattern?;

pattern:
  logicPattern |
  inlineSourceFree;

logicPattern:
  logicPattern_AndPrefix logicPattern_AndSuffix? |
  logicPattern_Argument (
    logicPattern_AndSuffix |
    ('or' logicPattern_Argument)* (
        'or' unaryPattern
    )
  )? |
  unaryPattern;

// Arbitrarily nested pattern consisting only of `and`.
logicPattern_AndPrefix:
  '('
    (
      logicPattern_AndPrefix logicPattern_AndSuffix? |
      logicPattern_Argument logicPattern_AndSuffix
    )
  ')';

logicPattern_AndSuffix:
  ('and' (logicPattern_AndPrefix | logicPattern_Argument))*
  'and' (
    logicPattern_AndPrefix |
    unaryPattern
  );

logicPattern_Argument:
  '(' unaryLogicPattern ')' |
  atomicNotNullPattern |
  typePattern;

unaryLogicPattern:
  '(' unaryLogicPattern ')' |
  notNullPattern;

unaryPattern:
  unaryLogicPattern |
  atomicNotNullPattern |
  typePattern;

typePattern:
  typePattern_Contents |
  annotationPattern;

typePattern_Contents:
  '(' typePattern_Contents ')' |
  'is' (
    '<' typeArgument '>' typePatternExplicit? |
    typePatternImplicit
  );

typePatternExplicit:
  annotationPattern;

typePatternImplicit:
  annotationPattern;

notNullPattern:
  'not' nullArg;

atomicNotNullPattern:
  '!' nullArg;

nullPattern:
  nullArg;

nullArg:
  'null' |
  '(' nullArg ')';

annotationPattern:
  atomicPattern (
    'as' type
  )*;

atomicPattern:
  nullPattern |
  primitiveExpr |
  unit ('(' ')')? |
  unaryNumberConvertExpr |
  unaryCharConvertExpr |
  somePattern |
  relationalPattern |
  regexPattern |
  fullConstructPattern |
  memberTestPattern |
  namedPattern |
  memberPattern |
  nestedPattern;

nestedPattern:
  '(' pattern ')';

somePattern:
  'some' (
    '(' patternArgument ')' |
    unaryPattern
  );

namedPattern:
  compoundName;

memberPattern:
  compoundName (
    simplePatternArgument |
    patternArguments
  );

relationalPattern:
  relationalOperator unaryPattern;

regexPattern:
  '/' (regexGroupStart | ~BEGIN_REGEX_GROUP)*? '/' ({combinedOperator}? NAME)? ({combinedOperator}? '-' {combinedOperator}? NAME)?;

regexGroupStart:
  BEGIN_REGEX_GROUP patternArgument '}';

// Calls

patternArguments:
  ('(' (emptyPatternArgTuple | patternArgTuple) (';' (emptyPatternArgTuple | patternArgTuple))* ')')+;

simplePatternArgument:
  basicConstructPattern |
  char |
  string;

emptyPatternArgTuple:;

patternArgTuple:
  (fieldAssignment pattern | fieldRelation | patternArgument) (',' (fieldAssignment pattern | fieldRelation | patternArgument))*;

// Records, collections, and tuples

basicConstructPattern:
  arrayConstructorPattern |
  recordConstructorPattern |
  explicitTupleConstructorPattern |
  classTupleConstructorPattern |
  structTupleConstructorPattern;

fullConstructPattern:
  basicConstructPattern |
  tupleConstructorPattern;

emptyFieldAssignment:
  name;

fieldRelation:
  name relationalPattern;

recordConstructorPattern:
  '{' (emptyFieldAssignment ',')* (fieldAssignment pattern | fieldRelation) (',' (fieldAssignment pattern | fieldRelation | emptyFieldAssignment))* '}';

arrayConstructorPattern:
  '[' (
    pattern |
    patternArgument (',' patternArgument)+
   )? ']';

tupleConstructorPattern:
  '(' tuplePattern_Contents ')';

explicitTupleConstructorPattern:
  '(' 'as' 'new' ';'? tuplePattern_Contents ')';

classTupleConstructorPattern:
  '(' 'as' 'class' ';'? tuplePattern_Contents ')';

structTupleConstructorPattern:
  '(' 'as' 'struct' ';'? tuplePattern_Contents ')';

tuplePattern_Contents:
  patternArgument (',' patternArgument)+;

memberTestPattern:
  'with' recordConstructorPattern;

/* ----------- */
/* Expressions */
/* ----------- */

expression:
  atomicLogicExpr |
  logicExpr |
  inlineSourceFree;

atomicLogicExpr:
  'not'* ('not' booleanExpr | inlineIfExpr);

logicExpr:
  booleanExpr (
    (
      'and' (booleanExpr 'and')* |
      'or' (booleanExpr 'or')*
    ) (booleanExpr | atomicLogicExpr)
  )?;

inlineIfExpr:
  (caseIf | if) expression ((caseElseif | elseif) expression)* else (booleanExpr | atomicLogicExpr);

booleanExpr:
  relationalExpr (('&&' | '||') relationalExpr)*;

relationalExpr:
  coalesceExpr (relationalOperator coalesceExpr)*;

coalesceExpr:
  concatExpr (tokenDOUBLEQUESTION concatExpr)*;

concatExpr:
  concatExpr_Inner ('..' concatExpr_Inner)*;

concatExpr_Inner: bitOrExpr;

bitOrExpr:
  bitXorExpr ('|' bitXorExpr)*;

bitXorExpr:
  bitAndExpr ('^' bitAndExpr)*;

bitAndExpr:
  bitShiftExpr ('&' bitShiftExpr)*;

bitShiftExpr:
  innerExpr (('<<' | tokenRSHIFT) innerExpr)*;

innerExpr:
  annotationExpr (('+' | '-' | '*' | '/' | '%') annotationExpr)*;

annotationExpr:
  unaryExpr (
    'as' type |
    'with' (
      recordConstructor |
      anonymousRecordConstructor |
      anonymousClassRecordConstructor |
      anonymousStructRecordConstructor
    )
  )*;

unaryExpr:
  (unaryOperator | errorUnsupportedFollow)* (
    atomicExpr |
    hashExpr |
    notExpr |
    unaryNumberConvertExpr |
    unaryCharConvertExpr |
    unaryConvertExpr |
    unit ('(' ')')?
  );

atomicExpr:
  altMemberExpr |
  memberExpr |
  simpleExpr |
  fullConstructExpr |
  funcExpr |
  caseFuncRefExpr |
  inlineExpr;

hashExpr:
  '#' unaryExpr;

notExpr:
  '!' unaryExpr;

unaryNumberConvertExpr:
  primitiveType numberArg;

unaryCharConvertExpr:
  primitiveType charArg;

unaryConvertExpr:
  convertOperator unaryExpr;

simpleExpr:
  primitiveExpr |
  interpolatedString;

nestedExpr:
  '(' expression ')';

nestedAssignment:
  '(' (altMemberExpr | memberExpr) assignment ')';

primitiveExpr:
  namedValue | number | string | char;

namedValue:
  'null' | 'false' | 'true' | 'none' | 'default';

funcExpr:
  'function' name? optionSuffix? funcBody;

caseFuncRefExpr:
  'case' (
    compoundNameGeneric ('.' caseFuncName | memberName) |
    caseFuncName
  ) optionSuffix?;

inlineExpr:
  'inline' expressionStatement;

basicConstructExpr:
  arrayConstructor |
  recordConstructor |
  anonymousRecordConstructor |
  anonymousClassRecordConstructor |
  anonymousStructRecordConstructor |
  sequenceConstructor |
  explicitTupleConstructor |
  classTupleConstructor |
  structTupleConstructor;

fullConstructExpr:
  basicConstructExpr |
  tupleConstructor;

numberArg:
  ('+' | '-')* (
    numberToken |
    '(' numberArg ')'
  );

stringArg:
  string |
  '(' stringArg ')';

anyStringArg:
  string |
  interpolatedString |
  '(' anyStringArg ')';

charArg:
  charToken |
  '(' charArg ')';

symbolArg:
  compoundName |
  '(' symbolArg ')';

// Member access

memberExpr:
  (memberExpr_Standalone | memberExpr_Prefix memberExpr_Suffix)
    memberExpr_Suffix*;

memberExpr_Standalone:
  name genericArguments? simpleCallArgument?? |
  memberTypeConstructExpr |
  memberNewExpr |
  memberNumberConvertExpr |
  memberCharConvertExpr |
  memberConvertExpr |
  primitiveTypeMemberAccess simpleCallArgument?? |
  nestedExpr |
  nestedAssignment;

memberExpr_Prefix:
  simpleExpr |
  fullConstructExpr |
  errorUnsupportedFollow nestedExpr;

memberExpr_Suffix:
  callArguments |
  indexAccess |
  (memberAccess | dynamicMemberAccess) simpleCallArgument?? |
  dynamicExprMemberAccess;

primitiveTypeMemberAccess:
  primitiveType memberAccess;

altMemberExpr:
  (memberExpr_Standalone | memberExpr_Prefix)
    memberExpr_Suffix* (conditionalMember memberExpr_Suffix | conditionalMember? altMemberExpr_Suffix) (conditionalMember? (memberExpr_Suffix | altMemberExpr_Suffix))*;

altMemberExpr_Suffix:
  constrainedMemberAccess |
  constrainedFunctionAccess |
  constrainedPropertyAccess;

conditionalMember:
  '?' {combinedOperator}?;

constrainedMemberAccess:
  '.' '(' name ')' callArguments?;

constrainedFunctionAccess:
  '.' '(' name 'as' functionType ')' callArguments?;

constrainedPropertyAccess:
  '.' '(' name 'as' type ')';

indexAccess:
  '[' (expression (',' expression)* | errorMissingExpression) ']';
  
memberAccess:
  ('.' name | memberName) genericArguments?;

dynamicMemberAccess:
  ':' name | dynamicMemberName;

dynamicExprMemberAccess:
  ':' nestedExpr;

memberNumberConvertExpr:
  primitiveType '(' numberArg ')';

memberCharConvertExpr:
  primitiveType '(' charArg ')';

memberConvertExpr:
  convertOperator '(' expression ')';

convertOperator:
  (
    (
      'some' | 'enum' | 'implicit' | 'explicit' | 'new' | 'widen' | 'narrow'
    ) ('<' genericArgument '>')? |
    'unit' '<' genericArgument '>' |
    primitiveType
  ) optionSuffix?;

optionSuffix:
  '?';

memberTypeConstructExpr:
  primitiveType optionSuffix? constructArguments;

memberNewExpr:
  'new' ('<' type '>')? optionSuffix? constructArguments;

// Must indicate construction by having no arguments, at least two arguments, or at least one named argument.
constructArguments:
  '(' (constructCallArgTuple | complexCallArgTuple) ')';

constructCallArgTuple:
  (
    (
      expression ',' expression |
      (expression ',')* (optionalFieldAssignmentExpr | fieldAssignment expression)
    ) (',' (optionalFieldAssignmentExpr | fieldAssignment? expression))*
  )?;

fieldAssignment:
  name '=';

optionalFieldAssignmentExpr:
  name '=' atomicExpr '?';

// Calls

callArguments:
  '(' callArgList ')';

simpleCallArgument:
  basicConstructExpr |
  char |
  string |
  interpolatedString;

callArgList:
  (simpleCallArgTuple | complexCallArgTuple) (';' (simpleCallArgTuple | complexCallArgTuple))*;

simpleCallArgTuple:
  (
    (optionalFieldAssignmentExpr | fieldAssignment? expression)
    (',' (optionalFieldAssignmentExpr | fieldAssignment? expression))*
  )?;

complexCallArgTuple:
  (fieldAssignment? expression ',')* spreadExpression (',' (spreadExpression | fieldAssignment? expression))*;

// Records, collections, and tuples

recordConstructor:
  '{' recordConstructor_Contents '}';

anonymousRecordConstructor:
  '{' 'as' 'new' ';'? recordConstructor_Contents '}';

anonymousClassRecordConstructor:
  '{' 'as' 'class' ';'? recordConstructor_Contents '}';

anonymousStructRecordConstructor:
  '{' 'as' 'struct' ';'? recordConstructor_Contents '}';

recordConstructor_Contents:
  (emptyFieldAssignment ',')* (fieldAssignment expression) (',' (fieldAssignment expression | emptyFieldAssignment))*;

arrayConstructor:
  '[' (simpleCollectionContents | complexCollectionContents)? ']';

sequenceConstructor:
  '{' (simpleCollectionContents | complexCollectionContents)? '}';

simpleCollectionContents:
  // Each element is one value
  simpleCollectionElement (',' simpleCollectionElement)*;

complexCollectionContents:
  // There is a complex element
  (
    // At the beginning
    complexCollectionElement |
    // In the middle
    simpleCollectionElement (',' simpleCollectionElement)* ',' complexCollectionElement
  ) (',' (simpleCollectionElement | complexCollectionElement))*;

simpleCollectionElement:
  collectionFieldExpression |
  expression;

collectionFieldExpression:
  '[' expression ']' assignment;

complexCollectionElement:
  spreadExpression |
  expressionStatement;

tupleConstructor:
  '(' (simpleTupleContents | complexTupleContents) ')';

explicitTupleConstructor:
  '(' 'as' 'new' ';'? (simpleTupleContents | complexTupleContents) ')';

classTupleConstructor:
  '(' 'as' 'class' ';'? (simpleTupleContents | complexTupleContents) ')';

structTupleConstructor:
  '(' 'as' 'struct' ';'? (simpleTupleContents | complexTupleContents) ')';

simpleTupleContents:
  expression (',' expression)+;

complexTupleContents:
  (expression ',')* spreadExpression (',' (spreadExpression | expression))*;

spreadExpression:
  '..' concatExpr_Inner;

/* -------------------- */
/* Interpolated strings */
/* -------------------- */

interpolatedString:
  plainInterpolatedString |
  verbatimInterpolatedString;

plainInterpolatedString:
  BEGIN_INTERPOLATED_STRING interpStrComponent* (END_STRING | errorUnsupportedEndStringSuffix);

verbatimInterpolatedString:
  BEGIN_VERBATIM_INTERPOLATED_STRING interpStrComponent* (END_STRING | errorUnsupportedEndStringSuffix);

interpStrComponent:
  STRING_PART |
  '%' |
  LITERAL_NEWLINE |
  LITERAL_ESCAPE_NEWLINE |
  interpStrExpression;

interpStrAlignment: INTERP_ALIGNMENT;
interpStrGeneralFormat: INTERP_FORMAT_GENERAL | INTERP_FORMAT_BEGIN_COMPONENTS INTERP_COMPONENTS_PART_SHORT;
interpStrStandardFormat: INTERP_FORMAT_STANDARD;
interpStrCustomFormat: INTERP_FORMAT_CUSTOM;
interpStrNumberFormat: INTERP_FORMAT_NUMBER;
interpStrComponentFormat:
  INTERP_FORMAT_BEGIN_COMPONENTS
  ((INTERP_COMPONENTS_PART_LONG | INTERP_COMPONENTS_PART_SHORT (INTERP_COMPONENTS_PART_SHORT | INTERP_COMPONENTS_PART_LONG))
  (INTERP_COMPONENTS_PART_SHORT | INTERP_COMPONENTS_PART_LONG)*)?;

interpStrExpression:
  '{' expression ((interpStrAlignment? (interpStrStandardFormat | interpStrCustomFormat | interpStrNumberFormat | interpStrComponentFormat)?) | interpStrGeneralFormat) '}';

/* ---------------- */
/* Inline F# source */
/* ---------------- */

inlineSourceFree:
  BEGIN_INLINE_SOURCE WHITESPACE* inlineSourceLanguage
    (inlineSourceFSharp | inlineSourceJavaScript | ERROR*)
  END_INLINE_SOURCE WHITESPACE* (END_DIRECTIVE | EOF);

inlineSourceReturning:
  BEGIN_INLINE_SOURCE WHITESPACE* inlineSourceLanguage
    (inlineSourceFSharp | inlineSourceJavaScript | ERROR*)
  END_INLINE_SOURCE WHITESPACE* 'return' WHITESPACE* (END_DIRECTIVE | EOF);

inlineSourceTerminating:
  BEGIN_INLINE_SOURCE WHITESPACE* inlineSourceLanguage
    (inlineSourceFSharp | inlineSourceJavaScript | ERROR*)
  END_INLINE_SOURCE WHITESPACE* 'throw' WHITESPACE* (END_DIRECTIVE | EOF);

inlineSourceLanguage:
  STRING_LITERAL | VERBATIM_STRING_LITERAL;

inlineSourceFSharp:
  inlineSourceFSFirstLine inlineSourceFSLine*;

inlineSourceJavaScript:
  JS_PART+;

inlineSourceFSFirstLine:
  // Skip any whitespace before indented token
  ((FS_WHITESPACE | FS_COMMENT | FS_BEGIN_BLOCK_COMMENT | FS_END_BLOCK_COMMENT)* inlineSourceFSToken inlineSourceFSPart*)? inlineSourceFSWhitespace* inlineSourceFSLineCutComment?;

inlineSourceFSLine:
  (
    // Directive includes newline
    inlineSourceFSDirective inlineSourceFSPart* |
    inlineSourceFSNewLine (inlineSourceFSIndentation inlineSourceFSToken inlineSourceFSPart*)?
  ) inlineSourceFSWhitespace* inlineSourceFSLineCutComment?;

inlineSourceFSNewLine:
  FS_EOL;

inlineSourceFSIndentation:
  (FS_WHITESPACE | FS_COMMENT | FS_BEGIN_BLOCK_COMMENT | FS_END_BLOCK_COMMENT)*;

inlineSourceFSToken:
  FS_PART;

inlineSourceFSPart:
  FS_PART | FS_WHITESPACE | inlineSourceFSLineComment;

inlineSourceFSDirective:
  FS_DIRECTIVE;

inlineSourceFSLineComment:
  FS_COMMENT | FS_BEGIN_BLOCK_COMMENT inlineSourceFSLineComment* FS_END_BLOCK_COMMENT; 

inlineSourceFSBlockComment:
  FS_BEGIN_BLOCK_COMMENT (FS_COMMENT | FS_EOL | inlineSourceFSBlockComment)* FS_END_BLOCK_COMMENT;

inlineSourceFSLineCutComment:
  FS_BEGIN_BLOCK_COMMENT inlineSourceFSWhitespace* inlineSourceFSLineCutComment?;

inlineSourceFSWhitespace:
  FS_WHITESPACE | FS_COMMENT | inlineSourceFSBlockComment;

/* --------- */
/* Operators */
/* --------- */

unaryOperator:
  '+' | '-' | '~';

relationalOperator:
  '<' | '<=' | '>' | tokenGTE | '==' | '!=';

tokenGTE:
  '>' {combinedOperator}? '=';

tokenRSHIFT:
  '>' {combinedOperator}? '>';

tokenDOUBLEQUESTION:
  '?' {combinedOperator}? '?';

/* ---------- */
/* Primitives */
/* ---------- */

number:
  numberToken | errorUnsupportedNumberSuffix;

numberToken:
  INT_LITERAL | FLOAT_LITERAL | EXP_LITERAL | HEX_LITERAL;

char:
  BEGIN_CHAR CHAR_PART (END_CHAR | errorUnsupportedEndCharSuffix);

charToken:
  BEGIN_CHAR CHAR_PART END_CHAR;

string:
  (BEGIN_STRING | BEGIN_VERBATIM_STRING) (STRING_PART | LITERAL_NEWLINE | LITERAL_ESCAPE_NEWLINE)* (END_STRING | errorUnsupportedEndStringSuffix);

unit:
  'unit';

/* ------------ */
/* Error states */
/* ------------ */

errorMissingExpression:;

errorUnsupportedFollow:
  'follow';

errorUnsupportedNumberSuffix:
  INT_SUFFIX | FLOAT_SUFFIX | EXP_SUFFIX | HEX_SUFFIX;

errorUnsupportedEndCharSuffix:
  END_CHAR_SUFFIX;

errorUnsupportedEndStringSuffix:
  END_STRING_SUFFIX;

errorUnderscoreReserved:
  UNDERSCORE;
