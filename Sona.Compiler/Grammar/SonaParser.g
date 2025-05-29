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
openTrail:
  (';'? statement)*? implicitReturnStatement;

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

// A block that explicitly returns, interrupts, or throws
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

// A block that may do anything
conditionalCoverBlock:
  (statement ';'?)*? ((closingStatement | interruptibleStatement | conditionalStatement) ';'? | implicitReturnStatement);
conditionalCoverTrail:
  (openTrail | interruptibleTrail | conditionalTrail);

interruptingCoverBlock:
  terminatingBlock | interruptingBlock;
interruptingCoverTrail:
  terminatingTrail | interruptingTrail;

interruptibleCoverBlock:
  openBlock | terminatingBlock | interruptingBlock | interruptibleBlock;
interruptibleCoverTrail:
  openTrail | interruptibleTrail;

openCoverBlock:
  openBlock | terminatingBlock;

openToInterruptibleBlock:
  openBlock | interruptibleBlock;

openToConditionalBlock:
  openBlock | interruptibleBlock | conditionalBlock;

interruptingToInterruptibleBlock:
  interruptingBlock | interruptibleBlock;

returningToConditionalBlock:
  returningBlock | conditionalBlock;

// Same but never executed
ignoredBlock:
  (statement ';'?)*? ((closingStatement | interruptibleStatement | conditionalStatement) ';'? | implicitReturnStatement);
ignoredTrail:
  (';'? statement)*? (';'? (closingStatement | interruptibleStatement | conditionalStatement) | implicitReturnStatement);

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
  'bool' | 'byte' | 'sbyte' |
  'int' | 'int16' | 'int32' | 'int64' | 'int128' |
  'uint' | 'uint16' | 'uint32' | 'uint64' | 'uint128' |
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
  optionalTypeSuffix |
  sequenceTypeSuffix;

arrayTypeSuffix: '[' ','* ']';
optionalTypeSuffix: '?';
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
  WHITESPACE* compoundName (WHITESPACE* (attrNamedArg | attrPosArg) (WHITESPACE+ (attrNamedArg | attrPosArg))*)? WHITESPACE*;

attrPosArg:
   atomicExpr;

attrNamedArg:
  name WHITESPACE* '=' WHITESPACE* atomicExpr;

/* ------------------ */
/* General statements */
/* ------------------ */

// Plain statement, allowed anywhere
statement:
  variableDecl |
  multiFuncDecl |
  inlineFuncDecl |
  memberDiscard |
  memberOrAssignment |
  echoStatement |
  yieldStatement |
  yieldEachStatement |
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
  switchStatementFreeInterrupted;

// A statement that must be located at the end of a block and stops its execution
closingStatement:
  returningStatement |
  interruptingStatement |
  terminatingStatement;

// Special statements

echoStatement:
  'echo' (echoStatement_Arg | expression) (',' expression)*;

echoStatement_Arg:
  string |
  interpolatedString |
  '(' echoStatement_Arg ')';

// Simple control statements

implicitReturnStatement:;

returnStatement:
  'return' expression?;

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

// A statement that has a returning path and all other paths are closing
returningStatement:
  returnStatement |
  yieldBreakStatement |
  inlineSourceReturning |
  doStatementReturning |
  ifStatementReturning |
  ifStatementReturningTrailFromElse |
  ifStatementReturningTrail |
  whileStatementReturningTrail |
  repeatStatementReturningTrail |
  forStatementReturningTrail |
  switchStatementReturning |
  switchStatementReturningTrail;

// A statement that has interrupting paths and all other paths are terminating
interruptingStatement:
  breakStatement |
  continueStatement |
  doStatementInterrupting |
  doStatementInterruptingTrail |
  ifStatementInterrupting |
  ifStatementInterruptingTrail;

// A statement that has interrupting paths and all other paths are open or terminating
interruptibleStatement:
  doStatementInterruptible |
  ifStatementInterruptible;

// A statement that has returning paths and open paths
conditionalStatement:
  doStatementConditional |
  ifStatementConditional |
  whileStatementConditional |
  repeatStatementConditional |
  forStatementConditional |
  switchStatementConditional;

// A statement that only has terminating paths
terminatingStatement:
  throwStatement |
  inlineSourceTerminating |
  doStatementTerminating |
  ifStatementTerminating |
  whileStatementTerminating |
  repeatStatementTerminating |
  switchStatementTerminating |
  switchStatementTerminatingInterrupted;

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
  switchStatementConditional;

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

else:
  'else';

// Free-standing (may throw from all branches but this is not preferred).
ifStatementFree:
  if freeBlock (elseif freeBlock)* (else freeBlock)? 'end';

ifStatementTerminating:
  if terminatingBlock (elseif terminatingBlock)* else terminatingBlock 'end' ignoredTrail;

// The following rules are auto-generated.

ifStatementInterrupting:
  if terminatingBlock (elseif terminatingBlock)* (
    elseif interruptingBlock (elseif interruptingCoverBlock)* else interruptingCoverBlock 'end' ignoredTrail |
    else interruptingBlock 'end' ignoredTrail 
  ) |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    else interruptingCoverBlock 'end' ignoredTrail 
  );

ifStatementInterruptingTrail:
  if terminatingBlock (elseif terminatingBlock)* (
    elseif openBlock (elseif openCoverBlock)* (
      elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
      else interruptingToInterruptibleBlock 
    ) |
    elseif interruptingBlock (elseif interruptingCoverBlock)* (
      elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
      (else openToInterruptibleBlock)? 
    ) |
    elseif interruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    else interruptibleBlock 
  ) 'end' interruptingCoverTrail |
  if openBlock (elseif openCoverBlock)* (
    elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    else interruptingToInterruptibleBlock 
  ) 'end' interruptingCoverTrail |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    (else openToInterruptibleBlock)? 
  ) 'end' interruptingCoverTrail |
  if interruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? 'end' interruptingCoverTrail;

ifStatementInterruptible:
  if terminatingBlock (elseif terminatingBlock)* (
    elseif openBlock (elseif openCoverBlock)* (
      elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
      else interruptingToInterruptibleBlock 
    ) |
    elseif interruptingBlock (elseif interruptingCoverBlock)* (
      elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
      (else openToInterruptibleBlock)? 
    ) |
    elseif interruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    else interruptibleBlock 
  ) 'end' interruptibleCoverTrail |
  if openBlock (elseif openCoverBlock)* (
    elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    else interruptingToInterruptibleBlock 
  ) 'end' interruptibleCoverTrail |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? |
    (else openToInterruptibleBlock)? 
  ) 'end' interruptibleCoverTrail |
  if interruptibleBlock (elseif interruptibleCoverBlock)* (else interruptibleCoverBlock)? 'end' interruptibleCoverTrail;

ifStatementReturning:
  if terminatingBlock (elseif terminatingBlock)* (
    elseif returningBlock (elseif returningCoverBlock)* else returningCoverBlock 'end' ignoredTrail |
    elseif interruptingBlock (elseif interruptingCoverBlock)* (
      elseif returningBlock (elseif returningCoverBlock)* else returningCoverBlock 'end' ignoredTrail |
      else returningBlock 'end' ignoredTrail 
    ) |
    else returningBlock 'end' ignoredTrail 
  ) |
  if returningBlock (elseif returningCoverBlock)* (
    else returningCoverBlock 'end' ignoredTrail 
  ) |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    elseif returningBlock (elseif returningCoverBlock)* else returningCoverBlock 'end' ignoredTrail |
    else returningBlock 'end' ignoredTrail 
  );

ifStatementReturningTrailFromElse:
  if terminatingBlock (elseif terminatingBlock)* (
    (
      elseif returningBlock (elseif returningCoverBlock)* else openToConditionalBlock |
      elseif interruptingBlock (elseif interruptingCoverBlock)* (
        elseif returningBlock (elseif returningCoverBlock)* else openToConditionalBlock |
        else conditionalBlock 
      ) |
      else interruptibleBlock |
      else conditionalBlock 
    ) 'end' returningTrail |
    (
      elseif returningBlock (elseif returningCoverBlock)* else openToConditionalBlock |
      elseif interruptingBlock (elseif interruptingCoverBlock)* (
        elseif returningBlock (elseif returningCoverBlock)* else openToConditionalBlock |
        else conditionalBlock 
      ) |
      else conditionalBlock 
    ) 'end' interruptingCoverTrail 
  ) |
  if returningBlock (elseif returningCoverBlock)* else openToConditionalBlock 'end' returningCoverTrail |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    (
      elseif returningBlock (elseif returningCoverBlock)* else openToConditionalBlock |
      else conditionalBlock 
    ) 'end' returningTrail |
    (
      elseif returningBlock (elseif returningCoverBlock)* else openToConditionalBlock |
      else conditionalBlock 
    ) 'end' interruptingCoverTrail 
  );

ifStatementReturningTrail:
  if terminatingBlock (elseif terminatingBlock)* (
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openBlock (elseif openCoverBlock)* (
        elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else returningToConditionalBlock)? 
        ) |
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif interruptingBlock (elseif interruptingCoverBlock)* (
        elseif returningBlock (elseif returningCoverBlock)* (
          elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else openToConditionalBlock)? 
        ) |
        elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else returningToConditionalBlock)? 
        ) |
        elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else conditionalBlock)? 
      ) |
      elseif interruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else returningToConditionalBlock)? 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else interruptibleBlock |
      else conditionalBlock 
    ) 'end' returningTrail |
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openBlock (elseif openCoverBlock)* (
        elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          else returningToConditionalBlock 
        ) |
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif interruptingBlock (elseif interruptingCoverBlock)* (
        elseif returningBlock (elseif returningCoverBlock)* (
          elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else openToConditionalBlock)? 
        ) |
        elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          else returningToConditionalBlock 
        ) |
        elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else conditionalBlock 
      ) |
      elseif interruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else conditionalBlock 
    ) 'end' interruptingCoverTrail 
  ) |
  if returningBlock (elseif returningCoverBlock)* (
    elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
    (else openToConditionalBlock)? 
  ) 'end' returningCoverTrail |
  if openBlock (elseif openCoverBlock)* (
    (
      elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else returningToConditionalBlock)? 
      ) |
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else returningToConditionalBlock 
    ) 'end' returningTrail |
    (
      elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else returningToConditionalBlock 
    ) 'end' interruptingCoverTrail 
  ) |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else returningToConditionalBlock)? 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (else conditionalBlock)? 
    ) 'end' returningTrail |
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else conditionalBlock 
    ) 'end' interruptingCoverTrail 
  ) |
  if interruptibleBlock (elseif interruptibleCoverBlock)* (
    (
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (else returningToConditionalBlock)? 
    ) 'end' returningTrail |
    (
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else returningToConditionalBlock 
    ) 'end' interruptingCoverTrail 
  ) |
  if conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? 'end' returningCoverTrail;

ifStatementConditional:
  if terminatingBlock (elseif terminatingBlock)* (
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openBlock (elseif openCoverBlock)* (
        elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          else returningToConditionalBlock 
        ) |
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif interruptingBlock (elseif interruptingCoverBlock)* (
        elseif returningBlock (elseif returningCoverBlock)* (
          elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else openToConditionalBlock)? 
        ) |
        elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          else returningToConditionalBlock 
        ) |
        elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else conditionalBlock 
      ) |
      elseif interruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else conditionalBlock 
    ) 'end' interruptibleCoverTrail |
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openBlock (elseif openCoverBlock)* (
        elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else returningToConditionalBlock)? 
        ) |
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif interruptingBlock (elseif interruptingCoverBlock)* (
        elseif returningBlock (elseif returningCoverBlock)* (
          elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else openToConditionalBlock)? 
        ) |
        elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
          elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
          (else returningToConditionalBlock)? 
        ) |
        elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else conditionalBlock)? 
      ) |
      elseif interruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else returningToConditionalBlock)? 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else interruptibleBlock |
      else conditionalBlock 
    ) 'end' conditionalTrail 
  ) |
  if returningBlock (elseif returningCoverBlock)* (
    elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
    (else openToConditionalBlock)? 
  ) 'end' conditionalCoverTrail |
  if openBlock (elseif openCoverBlock)* (
    (
      elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else returningToConditionalBlock 
    ) 'end' interruptibleCoverTrail |
    (
      elseif interruptingToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else returningToConditionalBlock)? 
      ) |
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else returningToConditionalBlock 
    ) 'end' conditionalTrail 
  ) |
  if interruptingBlock (elseif interruptingCoverBlock)* (
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        else returningToConditionalBlock 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else conditionalBlock 
    ) 'end' interruptibleCoverTrail |
    (
      elseif returningBlock (elseif returningCoverBlock)* (
        elseif openToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else openToConditionalBlock)? 
      ) |
      elseif openToInterruptibleBlock (elseif interruptibleCoverBlock)* (
        elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
        (else returningToConditionalBlock)? 
      ) |
      elseif conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (else conditionalBlock)? 
    ) 'end' conditionalTrail 
  ) |
  if interruptibleBlock (elseif interruptibleCoverBlock)* (
    (
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      else returningToConditionalBlock 
    ) 'end' interruptibleCoverTrail |
    (
      elseif returningToConditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? |
      (else returningToConditionalBlock)? 
    ) 'end' conditionalTrail 
  ) |
  if conditionalBlock (elseif conditionalCoverBlock)* (else conditionalCoverBlock)? 'end' conditionalCoverTrail;

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
  // Body may returns, trail is ignored
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
  'case' (pattern whenClause? | emptyPattern whenClause) 'do';

whenClause:
  'when' expression;

emptyPattern:;

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
    else returningBlock 'end' ignoredTrail |
    case terminatingBlock (case terminatingBlock)* (
      else returningBlock 'end' ignoredTrail |
      case returningBlock (case returningCoverBlock)* (else returningCoverBlock)? 'end' ignoredTrail |
      case interruptingBlock (case interruptingCoverBlock)* (
        else returningBlock 'end' ignoredTrail |
        case returningBlock (case returningCoverBlock)* (else returningCoverBlock)? 'end' ignoredTrail 
      ) 
    ) |
    case returningBlock (case returningCoverBlock)* (else returningCoverBlock)? 'end' ignoredTrail |
    case interruptingBlock (case interruptingCoverBlock)* (
      else returningBlock 'end' ignoredTrail |
      case returningBlock (case returningCoverBlock)* (else returningCoverBlock)? 'end' ignoredTrail 
    ) 
  );

switchStatementReturningTrail:
  switch (
    (
      else interruptibleBlock |
      else conditionalBlock |
      case terminatingBlock (case terminatingBlock)* (
        else interruptibleBlock |
        else conditionalBlock |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else returningToConditionalBlock)? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case returningBlock (case returningCoverBlock)* (
            else openToConditionalBlock |
            case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case openToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else returningToConditionalBlock)? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          (else returningToConditionalBlock)? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else returningToConditionalBlock)? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case openToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else returningToConditionalBlock)? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        (else returningToConditionalBlock)? |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' returningTrail |
    (
      else conditionalBlock |
      case terminatingBlock (case terminatingBlock)* (
        else conditionalBlock |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            else returningToConditionalBlock |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case conditionalBlock (case conditionalCoverBlock)* (
            else conditionalCoverBlock |
            case conditionalCoverBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (
          else conditionalCoverBlock |
          case conditionalCoverBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' interruptingCoverTrail 
  );

switchStatementConditional:
  switch (
    (
      else conditionalBlock |
      case terminatingBlock (case terminatingBlock)* (
        else conditionalBlock |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            else returningToConditionalBlock |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case conditionalBlock (case conditionalCoverBlock)* (
            else conditionalCoverBlock |
            case conditionalCoverBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          else returningToConditionalBlock |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case conditionalBlock (case conditionalCoverBlock)* (
          else conditionalCoverBlock |
          case conditionalCoverBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        else returningToConditionalBlock |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' interruptibleCoverTrail |
    (
      else interruptibleBlock |
      else conditionalBlock |
      case terminatingBlock (case terminatingBlock)* (
        else interruptibleBlock |
        else conditionalBlock |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case openBlock (case openCoverBlock)* (
          else returningToConditionalBlock |
          case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else returningToConditionalBlock)? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptingBlock (case interruptingCoverBlock)* (
          else conditionalBlock |
          case returningBlock (case returningCoverBlock)* (
            else openToConditionalBlock |
            case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case openToInterruptibleBlock (case interruptibleCoverBlock)* (
            (else returningToConditionalBlock)? |
            case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
          ) |
          case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case interruptibleBlock (case interruptibleCoverBlock)* (
          (else returningToConditionalBlock)? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case returningBlock (case returningCoverBlock)* (
        else openToConditionalBlock |
        case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case openBlock (case openCoverBlock)* (
        else returningToConditionalBlock |
        case interruptingToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else returningToConditionalBlock)? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptingBlock (case interruptingCoverBlock)* (
        else conditionalBlock |
        case returningBlock (case returningCoverBlock)* (
          else openToConditionalBlock |
          case openToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case openToInterruptibleBlock (case interruptibleCoverBlock)* (
          (else returningToConditionalBlock)? |
          case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
        ) |
        case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case interruptibleBlock (case interruptibleCoverBlock)* (
        (else returningToConditionalBlock)? |
        case returningToConditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
      ) |
      case conditionalBlock (case conditionalCoverBlock)* (else conditionalCoverBlock)? 
    ) 'end' conditionalTrail 
  );

/* -------------------- */
/* Top-level statements */
/* -------------------- */

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
  compoundNameGeneric '.' '*' |
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

/* ---------------------------- */
/* Declarations and assignments */
/* ---------------------------- */

variableDecl:
  localAttrList (letDecl | varDecl | constDecl | useDecl | useVarDecl);

letDecl:
  'let' declaration '=' expression;

varDecl:
  'var' declaration '=' expression;

constDecl:
  'const' declaration '=' expression;

useDecl:
  'use' declaration '=' expression;

useVarDecl:
  'use' 'var' declaration '=' expression;

multiFuncDecl:
  funcDecl+;

funcDecl:
  localAttrList 'function' name funcBody;

inlineFuncDecl:
  localAttrList 'inline' 'function' name funcBody;

funcBody:
  '(' paramList ')' ('as' type ';'?)? valueBlock 'end';

paramList:
  paramTuple (';' paramTuple)*;

paramTuple:
  (declaration (',' declaration)*)?;

declaration:
  localAttrList name ('as' type)?;

memberOrAssignment:
  (altMemberExpr | memberExpr) assignment?;

assignment:
  '=' expression;

memberDiscard:
  (altMemberExpr | memberExpr) '!';

/* -------- */
/* Patterns */
/* -------- */

pattern:
  primitiveExpr |
  nestedPattern;

nestedPattern:
  '(' pattern ')';

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
  'if' expression 'then' expression ('elseif' expression 'then' expression)* 'else' (booleanExpr | atomicLogicExpr);

booleanExpr:
  relationalExpr (('&&' | '||') relationalExpr)*;

relationalExpr:
  coalesceExpr (('<' | '<=' | '>' | '>' {combinedOperator}? '=' | '==' | '!=' | '~=') coalesceExpr)*;

coalesceExpr:
  concatExpr ('?' {combinedOperator}? '?' concatExpr)*;

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
  innerExpr (('<<' | '>' {combinedOperator}? '>') innerExpr)*;

innerExpr:
  annotationExpr (('+' | '-' | '*' | '/' | '%') annotationExpr)*;

annotationExpr:
  atomicExpr (
    'as' type |
    'with' (
      recordConstructor |
      anonymousRecordConstructor |
      anonymousClassRecordConstructor |
      anonymousStructRecordConstructor
    )
  )*;

atomicExpr:
  unaryOperator* (
    altMemberExpr |
    memberExpr |
    simpleExpr |
    fullConstructExpr |
    funcExpr |
    inlineExpr |
    hashExpr |
    notExpr |
    atomicNumberConvertExpr |
    atomicConvertExpr |
    unit
  );

hashExpr:
  '#' atomicExpr;

notExpr:
  '!' atomicExpr;

atomicNumberConvertExpr:
  primitiveType numberConvertExpr_Arg;

numberConvertExpr_Arg:
  ('+' | '-')* (
    numberToken |
    '(' numberConvertExpr_Arg ')'
  );

atomicConvertExpr:
  convertOperator atomicExpr;

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
  'function' name? funcBody;

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

// Member access

memberExpr:
  (memberExpr_Standalone | memberExpr_Prefix memberExpr_Suffix)
    memberExpr_Suffix*;

memberExpr_Standalone:
  name genericArguments? simpleCallArgument?? |
  memberTypeConstructExpr |
  memberNewExpr |
  primitiveTypeMemberAccess simpleCallArgument?? |
  nestedExpr |
  nestedAssignment;

memberExpr_Prefix:
  simpleExpr |
  fullConstructExpr |
  memberNumberConvertExpr |
  memberConvertExpr;

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
  primitiveType '(' numberConvertExpr_Arg ')';

memberConvertExpr:
  convertOperator '(' expression ')';

convertOperator:
  (
    (
      'some' | 'enum' | 'implicit' | 'explicit' | 'new' | 'widen' | 'narrow' | 'unit'
    ) ('<' genericArgument '>')? |
    primitiveType
  ) optionSuffix?;

optionSuffix:
  '?';

memberTypeConstructExpr:
  primitiveType optionSuffix? constructArguments;

memberNewExpr:
  'new' ('<' type '>')? optionSuffix? constructArguments;

// Must indicate construction by having no arguments, at least two arguments, or one named argument.
constructArguments:
  '(' (constructCallArgTuple | complexCallArgTuple) ')';

constructCallArgTuple:
  ((expression ',' expression | (expression ',')* fieldAssignment) (',' (fieldAssignment | expression))*)?;

fieldAssignment:
  name '=' expression;

// Function calls

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
  ((fieldAssignment | expression) (',' (fieldAssignment | expression))*)?;

complexCallArgTuple:
  ((fieldAssignment | expression) ',')* spreadExpression (',' (spreadExpression | fieldAssignment | expression))*;

// Records, collections, and tuples

recordConstructor:
  '{' fieldAssignment (',' fieldAssignment)* '}';

anonymousRecordConstructor:
  '{' 'as' 'new' ';'? fieldAssignment (',' fieldAssignment)* '}';

anonymousClassRecordConstructor:
  '{' 'as' 'class' ';'? fieldAssignment (',' fieldAssignment)* '}';

anonymousStructRecordConstructor:
  '{' 'as' 'struct' ';'? fieldAssignment (',' fieldAssignment)* '}';

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
  BEGIN_INLINE_SOURCE WHITESPACE* inlineSourceLanguage inlineSourceFirstLine inlineSourceLine*
  END_INLINE_SOURCE WHITESPACE* (END_DIRECTIVE | EOF);

inlineSourceReturning:
  BEGIN_INLINE_SOURCE WHITESPACE* inlineSourceLanguage inlineSourceFirstLine inlineSourceLine*
  END_INLINE_SOURCE WHITESPACE* 'return' WHITESPACE* (END_DIRECTIVE | EOF);

inlineSourceTerminating:
  BEGIN_INLINE_SOURCE WHITESPACE* inlineSourceLanguage inlineSourceFirstLine inlineSourceLine*
  END_INLINE_SOURCE WHITESPACE* 'throw' WHITESPACE* (END_DIRECTIVE | EOF);

inlineSourceLanguage:
  STRING_LITERAL | VERBATIM_STRING_LITERAL;

inlineSourceFirstLine:
  // Skip any whitespace before indented token
  ((FS_WHITESPACE | FS_COMMENT | FS_BEGIN_BLOCK_COMMENT | FS_END_BLOCK_COMMENT)* inlineSourceToken inlineSourcePart*)? inlineSourceWhitespace* inlineSourceLineCutComment?;

inlineSourceLine:
  (
    // Directive includes newline
    inlineSourceDirective inlineSourcePart* |
    inlineSourceNewLine (inlineSourceIndentation inlineSourceToken inlineSourcePart*)?
  ) inlineSourceWhitespace* inlineSourceLineCutComment?;

inlineSourceNewLine:
  FS_EOL;

inlineSourceIndentation:
  (FS_WHITESPACE | FS_COMMENT | FS_BEGIN_BLOCK_COMMENT | FS_END_BLOCK_COMMENT)*;

inlineSourceToken:
  FS_PART;

inlineSourcePart:
  FS_PART | FS_WHITESPACE | inlineSourceLineComment;

inlineSourceDirective:
  FS_DIRECTIVE;

inlineSourceLineComment:
  FS_COMMENT | FS_BEGIN_BLOCK_COMMENT inlineSourceLineComment* FS_END_BLOCK_COMMENT; 

inlineSourceBlockComment:
  FS_BEGIN_BLOCK_COMMENT (FS_COMMENT | FS_EOL | inlineSourceBlockComment)* FS_END_BLOCK_COMMENT;

inlineSourceLineCutComment:
  FS_BEGIN_BLOCK_COMMENT inlineSourceWhitespace* inlineSourceLineCutComment?;

inlineSourceWhitespace:
  FS_WHITESPACE | FS_COMMENT | inlineSourceBlockComment;

/* --------- */
/* Operators */
/* --------- */

unaryOperator:
  '+' | '-' | '~';

/* ---------- */
/* Primitives */
/* ---------- */

number:
  numberToken | errorUnsupportedNumberSuffix;

numberToken:
  INT_LITERAL | FLOAT_LITERAL | EXP_LITERAL | HEX_LITERAL;

char:
  BEGIN_CHAR CHAR_PART (END_CHAR | errorUnsupportedEndCharSuffix);

string:
  (BEGIN_STRING | BEGIN_VERBATIM_STRING) (STRING_PART | LITERAL_NEWLINE | LITERAL_ESCAPE_NEWLINE)* (END_STRING | errorUnsupportedEndStringSuffix);

unit:
  'unit';

/* ------------ */
/* Error states */
/* ------------ */

errorMissingExpression:;

errorUnsupportedNumberSuffix:
  INT_SUFFIX | FLOAT_SUFFIX | EXP_SUFFIX | HEX_SUFFIX;

errorUnsupportedEndCharSuffix:
  END_CHAR_SUFFIX;

errorUnsupportedEndStringSuffix:
  END_STRING_SUFFIX;

errorUnderscoreReserved:
  UNDERSCORE;
