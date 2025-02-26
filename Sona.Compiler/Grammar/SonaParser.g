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
closingBlock:
  (statement ';'?)*? closingStatement ';'?;
closingTrail:
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
fullBlock:
  (statement ';'?)*? ((closingStatement | interruptibleStatement | conditionalStatement) ';'? | implicitReturnStatement);
fullTrail:
  (';'? statement)*? (';'? (closingStatement | interruptibleStatement | conditionalStatement) | implicitReturnStatement);

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

/* ----- */
/* Types */
/* ----- */

type:
  compoundName;

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
  memberOrAssignment |
  memberDiscard |
  echoStatement |
  inlineSourceFree |
  ifStatementFree |
  doStatementFree |
  whileStatementFree |
  whileStatementFreeInterrupted |
  repeatStatementFree |
  repeatStatementFreeInterrupted |
  forStatementFree |
  forStatementFreeInterrupted;

// A statement that must be located at the end of a block and stops its execution
closingStatement:
  returningStatement |
  interruptingStatement |
  terminatingStatement;

// Special statements

echoStatement:
  'echo' expression (',' expression)*;

// Simple control statements

implicitReturnStatement:;

returnStatement:
  'return' expression?;

breakStatement:
  'break' expression?;

continueStatement:
  'continue' expression?;

throwStatement:
  'throw' expression?;

// A statement that has a returning path and all other paths are closing
returningStatement:
  returnStatement |
  inlineSourceReturning |
  doStatementReturning |
  ifStatementReturning |
  ifStatementReturningTrailFromElse |
  ifStatementReturningTrail |
  whileStatementReturningTrail |
  repeatStatementReturningTrail |
  forStatementReturningTrail;

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
  forStatementConditional;

// A statement that only has terminating paths
terminatingStatement:
  throwStatement |
  inlineSourceTerminating |
  doStatementTerminating |
  ifStatementTerminating |
  whileStatementTerminating |
  repeatStatementTerminating;

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
  forStatementConditional;

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
  'do' interruptibleBlock 'end' (terminatingTrail | interruptingTrail);

doStatementInterruptible:
  // Body is interruptible, trail is open or interruptible
  'do' interruptibleBlock 'end' (openTrail | interruptibleTrail);

doStatementReturning:
  // Body returns, trail is ignored
  'do' returningBlock 'end' (ignoredEmptyTrail | ignoredTrail) |
  // Body is interruptible but trail returns
  'do' interruptibleBlock 'end' returningTrail |
  // Body is conditional but trail closes
  'do' conditionalBlock 'end' closingTrail;

doStatementConditional:
  // Body is conditional, trail is open, interruptible or conditional
  'do' conditionalBlock 'end' (openTrail | interruptibleTrail | conditionalTrail) |
  // Body is interrutible but trail is conditional
  'do' interruptibleBlock 'end' conditionalBlock;

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

ifStatementInterrupting:
  // `if` branch is interrupting, other are terminating or interrupting, trail is ignored
  if interruptingBlock (elseif (terminatingBlock | interruptingBlock))* else (terminatingBlock | interruptingBlock) 'end' ignoredTrail |
  // `if` branch is terminating... (find interrupting branch)
  if terminatingBlock (elseif terminatingBlock)* (
    // `elseif` branch is interrupting, other are terminating or interrupting
    elseif interruptingBlock (elseif (terminatingBlock | interruptingBlock))* else (terminatingBlock | interruptingBlock) |
    // `else` branch is interrupting
    else interruptingBlock
    // Trail is ignored
  ) 'end' ignoredTrail;

ifStatementInterruptingTrail:
  // `if` branch is interrupting... (find open path)
  if interruptingBlock (elseif (terminatingBlock | interruptingBlock))* (
    // `elseif` branch is open or interruptible, other are non-returning
    elseif (openBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is open or interruptible
    (else (openBlock | interruptibleBlock))?
    // But the trail is terminating or interrupting
  ) 'end' (terminatingTrail | interruptingTrail) |
  // `if` branch is interruptible, other are non-returning, but the trail is terminating or interrupting
  if interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? 'end' (terminatingTrail | interruptingTrail) |
  // `if` branch is open or terminating... (find interrupting path)
  if (openBlock | terminatingBlock) (elseif (openBlock | terminatingBlock))* (
    // `elseif` branch is interrupting or interruptible, other are non-returning
    elseif (interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is interrupting or interruptible
    else (interruptingBlock | interruptibleBlock)
    // But the trail is terminating or interrupting
  ) 'end' (terminatingTrail | interruptingTrail);

ifStatementInterruptible:
  // `if` branch is interrupting... (find open path)
  if interruptingBlock (elseif (terminatingBlock | interruptingBlock))* (
    // `elseif` branch is open or interruptible, other are non-returning
    elseif (openBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is open or interruptible
    (else (openBlock | interruptibleBlock))?
    // Trail is open or interruptible
  ) 'end' (openTrail | interruptibleTrail) |
  // `if` branch is terminating... (find open and interrupting path)
  if terminatingBlock (elseif terminatingBlock)* (
    // `elseif` branch is interrupting... (find open path)
    elseif interruptingBlock (elseif (terminatingBlock | interruptingBlock))* (
      // `elseif` branch is open or interruptible, other are non-returning
      elseif (openBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
      // `else` branch is open or interruptible
      (else (openBlock | interruptibleBlock))?
    ) |
    // `elseif` branch is open... (find interrupting path)
    elseif openBlock (elseif (openBlock | terminatingBlock))* (
      // `elseif` branch is interrupting or interruptible, other are non-returning
      elseif (interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
      // `else` branch is interrupting or interruptible
      else (interruptingBlock | interruptibleBlock)
    ) |
    // `elseif` branch is interruptible, other are non-returning
    elseif interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is interruptible
    else interruptibleBlock
    // Trail is open or interruptible
  ) 'end' (openTrail | interruptibleTrail) |
  // `if` branch is open... (find interrupting path)
  if openBlock (elseif (openBlock | terminatingBlock))* (
    // `elseif` branch is interrupting or interruptible, other are non-returning
    elseif (interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is interrupting or interruptible
    else (interruptingBlock | interruptibleBlock)
    // Trail is open or interruptible
  ) 'end' (openTrail | interruptibleTrail) |
  // `if` branch is interruptible, other are non-returning, trail is open or interruptible
  if interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? 'end' (openTrail | interruptibleTrail);

ifStatementReturning:
  // `if` branch is returning, other are closing, trail is ignored
  if returningBlock (elseif closingBlock)* else closingBlock 'end' ignoredTrail |
  // `if` branch is closing but non-returning... (find returning path)
  if (terminatingBlock | interruptingBlock) (elseif (terminatingBlock | interruptingBlock))* (
    // `elseif` branch is returning, other are closing
    elseif returningBlock (elseif closingBlock)* else closingBlock |
    // `else` branch is returning
    else returningBlock
    // Trail is ignored
  ) ignoredTrail;

ifStatementReturningTrailFromElse:
  // `if` branch is returning... (require open `else`)
  if returningBlock (elseif closingBlock)* (
    // `else` branch is open
    (else openBlock)?
    // But the trail is closing
  ) 'end' closingTrail |
  // `if` branch is terminating... (find returning path)
  if terminatingBlock (elseif terminatingBlock)* (
    // `elseif` branch is returning, other are returning or terminating, `else` is open
    elseif returningBlock (elseif closingBlock)* (else openBlock)?
    // But the trail is closing
  ) 'end' closingTrail;

ifStatementReturningTrail:
  // `if` branch is returning... (find open path)
  if returningBlock (elseif closingBlock)* (
    // `elseif` branch is open or interruptible or conditional, other are anything
    elseif (openBlock | interruptibleBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
    // `else` branch is open or interruptible or conditional
    (else (openBlock | interruptibleBlock | conditionalBlock))?
    // But the trail is closing
  ) 'end' closingTrail |
  // `if` branch is interruptible, other are non-returning, but the trail is returning
  if interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? 'end' returningTrail |
  // `if` branch is conditional, other are anything, but the trail is closing
  if conditionalBlock (elseif fullBlock)* (else fullBlock)? 'end' closingTrail |
  // `if` branch is non-returning... (find returning path)
  if (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (
    // `elseif` branch is returning or conditional, other are anything
    elseif (returningBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
    // `else` branch is returning or conditional
    else (returningBlock | conditionalBlock)
    // But the trail is closing
  ) 'end' closingTrail |
  // `if` branch is interrupting... (find open path)
  if interruptingBlock (elseif (terminatingBlock | interruptingBlock))* (
    // `elseif` branch is open or interruptible, other are non-returning
    elseif (openBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is open or interruptible
    (else (openBlock | interruptibleBlock))?
    // But the trail is returning
  ) 'end' returningTrail |
  // `if` branch is interruptible... (find returning path)
  if interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (
    // `elseif` branch is returning or conditional, other are anything
    elseif (returningBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
    // `else` branch is returning or conditional
    else (returningBlock | conditionalBlock)
    // But the trail is closing
  ) 'end' closingTrail;

ifStatementConditional:
  // `if` branch is returning... (find open path)
  if returningBlock (elseif closingBlock)* (
    // `elseif` branch is open or interruptible or conditional, other are anything
    elseif (openBlock | interruptibleBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
    // `else` branch is open or interruptible or conditional
    (else (openBlock | interruptibleBlock | conditionalBlock))?
    // Trail is open or interruptible or conditional
  ) 'end' (openTrail | interruptibleTrail | conditionalTrail) |
  // `if` branch is conditional, other are anything, trail is open or interruptible or conditional
  if conditionalBlock (elseif fullBlock)* (else fullBlock)? 'end' (openTrail | interruptibleTrail | conditionalTrail) |
  // `if` branch is closing but non-returning... (find open and returning path)
  if (terminatingBlock | interruptingBlock) (elseif (terminatingBlock | interruptingBlock))* (
    // `elseif` branch is returning... (find open path)
    elseif returningBlock (elseif closingBlock)* (
      // `elseif` branch is open or conditional, other are anything
      elseif (openBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
      // `else` branch is open or conditional
      (else (openBlock | conditionalBlock))?
    ) |
    // `elseif` branch is open... (find returning path)
    elseif openBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (
      // `elseif` branch is returning or conditional, other are anything
      elseif (returningBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
      // `else` branch is returning or conditional
      else (returningBlock | conditionalBlock)
    ) |
    // `elseif` branch is conditional, other are anything
    elseif conditionalBlock (elseif fullBlock)* (else fullBlock)? |
    // `else` branch is conditional
    else conditionalBlock
    // Trail is open or interruptible or conditional
  ) 'end' (openTrail | interruptibleTrail | conditionalTrail) |
  // `if` branch is non-returning... (find returning path)
  if (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (
    // `elseif` branch is returning or conditional, other are anything
    elseif (returningBlock | conditionalBlock) (elseif fullBlock)* (else fullBlock)? |
    // `else` branch is returning or conditional
    else (returningBlock | conditionalBlock)
    // Trail is open or interruptible or conditional
  ) 'end' (openTrail | interruptibleTrail | conditionalTrail) |
  // Copied from ifStatementInterruptible
  // `if` branch is interrupting... (find open path)
  if interruptingBlock (elseif (terminatingBlock | interruptingBlock))* (
    // `elseif` branch is open or interruptible, other are non-returning
    elseif (openBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is open or interruptible
    (else (openBlock | interruptibleBlock))?
    // But the trail is conditional
  ) 'end' conditionalTrail |
  // `if` branch is terminating... (find open and interrupting path)
  if terminatingBlock (elseif terminatingBlock)* (
    // `elseif` branch is interrupting... (find open path)
    elseif interruptingBlock (elseif (terminatingBlock | interruptingBlock))* (
      // `elseif` branch is open or interruptible, other are non-returning
      elseif (openBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
      // `else` branch is open or interruptible
      (else (openBlock | interruptibleBlock))?
    ) |
    // `elseif` branch is open... (find interrupting path)
    elseif openBlock (elseif (openBlock | terminatingBlock))* (
      // `elseif` branch is interrupting or interruptible, other are non-returning
      elseif (interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
      // `else` branch is interrupting or interruptible
      else (interruptingBlock | interruptibleBlock)
    ) |
    // `elseif` branch is interruptible, other are non-returning
    elseif interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is interruptible
    else interruptibleBlock
    // But the trail is conditional
  ) 'end' conditionalTrail |
  // `if` branch is open... (find interrupting path)
  if openBlock (elseif (openBlock | terminatingBlock))* (
    // `elseif` branch is interrupting or interruptible, other are non-returning
    elseif (interruptingBlock | interruptibleBlock) (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? |
    // `else` branch is interrupting or interruptible
    else (interruptingBlock | interruptibleBlock)
    // But the trail is conditional
  ) 'end' conditionalTrail |
  // `if` branch is interruptible, other are non-returning, but the trail is conditional
  if interruptibleBlock (elseif (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (else (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))? 'end' conditionalTrail;

// `while`

while:
  'while' expression 'do';

whileTrue:
  WHILE_TRUE_DO;

whileStatementFree:
  while freeBlock 'end';

whileStatementFreeInterrupted:
  // Body may interrupt
  (whileTrue | while) (interruptingBlock | interruptibleBlock) 'end';

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
  whileTrue (returningBlock | conditionalBlock) 'end' ignoredTrail;
*/

whileStatementReturningTrail:
  // Body is returning or conditional, but the trail is closing
  (whileTrue | while) (returningBlock | conditionalBlock) 'end' closingTrail;

whileStatementConditional:
  // Body is returning or conditional, trail is open, interruptible, or conditional
  (whileTrue | while) (returningBlock | conditionalBlock) 'end' (openTrail | interruptibleTrail | conditionalTrail);

// `repeat`

until:
  'until' expression;

repeatStatementFree:
  'repeat' freeBlock until;

repeatStatementFreeInterrupted:
  // Body may interrupt
  'repeat' (interruptingBlock | interruptibleBlock) until;

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
  'repeat' (returningBlock | conditionalBlock) until closingTrail;

repeatStatementConditional:
  // Body is returning or conditional, trail is open, interruptible, or conditional
  'repeat' (returningBlock | conditionalBlock) until (openTrail | interruptibleTrail | conditionalTrail);

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
  for (interruptingBlock | interruptibleBlock) 'end';

forStatementReturningTrail:
  // Body is returning or conditional, but the trail is closing
  for (returningBlock | conditionalBlock) 'end' closingTrail;

forStatementConditional:
  // Body is returning or conditional, trail is open, interruptible, or conditional
  for (returningBlock | conditionalBlock) 'end' (openTrail | interruptibleTrail | conditionalTrail);

// `switch`

switch:
  'switch' expression;

case:
  'case' pattern whenClause? 'do';

whenClause:
  'when' expression;

// Free-standing (may throw from all branches but this is not preferred).
// Presence of at least one case/else is ensured in code.
switchStatementFree:
  switch (case freeBlock)* (else freeBlock)? 'end';

// Free-standing (may interrupt in all branches but this is not preferred)
switchStatementFreeInterrupted:
  switch (case freeBlock)* (
    // `case` must interrupt
    case (interruptingBlock | interruptibleBlock) (case (freeBlock | interruptingBlock | interruptibleBlock))* (else (freeBlock | interruptingBlock | interruptibleBlock))? |
    // `else` must interrupt
    else (interruptingBlock | interruptibleBlock)
  ) 'end';

switchStatementTerminating:
  // At least one branch must handle the value
  switch (case terminatingBlock)* (else terminatingBlock)? 'end' ignoredTrail;

// An "interrupted" branch means another must be entered, so such a branch
// does not affect the statement's category.
switchStatementTerminatingInterrupted:
  switch (case terminatingBlock)* (
    // `case` interrupts, the rest terminates or interrupts
    case interruptingBlock (case (terminatingBlock | interruptingBlock))* (else (terminatingBlock | interruptingBlock))? |
    // `else` interrupts
    else interruptingBlock
  ) 'end' ignoredTrail;

switchStatementReturning:
  switch (case (terminatingBlock | interruptingBlock))* (
    // `case` is returning, the rest is closing
    case returningBlock (case closingBlock)* (else closingBlock)? |
    // `else` is returning
    else returningBlock
    // Trail is ignored
  ) ignoredTrail;

switchStatementReturningTrail:
  switch (case (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (
    // `case` is returning or conditional, others are anything
    case (returningBlock | conditionalBlock) (case fullBlock)* (else fullBlock)? |
    // `else` is returning or conditional
    else (returningBlock | conditionalBlock)
    // But the trail is closing
  ) 'end' closingTrail;

switchStatementConditional:
  switch (case (terminatingBlock | interruptingBlock))* (
    // `case` is returning... (find open path)
    case returningBlock (case closingBlock)* (
      // `case` is open or conditional, other are anything
      case (openBlock | interruptibleBlock | conditionalBlock) (case fullBlock)* (else fullBlock)? |
      // `else` is open or conditional
      (else (openBlock | interruptibleBlock | conditionalBlock))?
    ) |
    // `case` is open... (find returning path)
    case (openBlock | interruptibleBlock) (case (openBlock | terminatingBlock | interruptingBlock | interruptibleBlock))* (
      // `case` is returning or conditional, other are anything
      case (returningBlock | conditionalBlock) (case fullBlock)* (else fullBlock)? |
      // `else` is returning or conditional
      else (returningBlock | conditionalBlock)
    ) |
    // `case` is conditional, other are anything
    case conditionalBlock (case fullBlock)* (else fullBlock)? |
    // `else` is conditional
    else conditionalBlock
    // Trail is open or interruptible or conditional
  ) 'end' (openTrail | interruptibleTrail | conditionalTrail);

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

funcBody:
  '(' paramList ')' ('as' type)? valueBlock 'end';

paramList:
  paramTuple (';' paramTuple)*;

paramTuple:
  (declaration (',' declaration)*)?;

declaration:
  localAttrList name ('as' type)?;

memberOrAssignment:
  memberExpr assignment?;

assignment:
  '=' expression;

memberDiscard:
  memberExpr '!';

/* -------- */
/* Patterns */
/* -------- */

pattern:
  primitiveExpr;

/* ----------- */
/* Expressions */
/* ----------- */

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
  constructExpr |
  funcExpr |
  inlineExpr |
  memberExpr |
  hashExpr |
  notExpr |
  unaryOperator atomicExpr;

hashExpr:
  '#' atomicExpr;

notExpr:
  '!' atomicExpr;

simpleExpr:
  primitiveExpr |
  interpolatedString |
  verbatimInterpolatedString;

nestedExpr:
  '(' expression ')';

nestedAssignment:
  '(' memberExpr assignment ')';

primitiveExpr:
  namedValue | number | string;

namedValue:
  'null' | 'false' | 'true';

funcExpr:
  'function' name? funcBody;

inlineExpr:
  'inline' expressionStatement;

constructExpr:
  arrayConstructor |
  recordConstructor |
  anonymousRecordConstructor |
  anonymousClassRecordConstructor |
  anonymousStructRecordConstructor |
  sequenceConstructor |
  tupleConstructor |
  explicitTupleConstructor |
  classTupleConstructor |
  structTupleConstructor;

// Member access

memberExpr:
  (
    name simpleCallArgument?? |
    nestedExpr |
    nestedAssignment
  ) memberExpr_Suffix? |
  (
    simpleExpr |
    constructExpr
  ) memberExpr_Suffix;

memberExpr_Suffix:
  (
    callArguments |
    indexAccess |
    (memberAccess | dynamicMemberAccess) simpleCallArgument??
  )+;

indexAccess:
  '[' (expression (',' expression)* | errorMissingExpression) ']';
  
memberAccess:
  '.' name | memberName;

dynamicMemberAccess:
  ':' name | dynamicMemberName;

// Function calls

callArguments:
  '(' callArgList ')';

simpleCallArgument:
  constructExpr |
  string |
  interpolatedString |
  verbatimInterpolatedString;

callArgList:
  callArgTuple (';' callArgTuple)*;

callArgTuple:
  (expression (',' expression)*)?;

// Records, collections, and tuples

recordConstructor:
  '{' recordField (',' recordField)* '}';

anonymousRecordConstructor:
  '{' 'as' 'new' ';'? recordField (',' recordField)* '}';

anonymousClassRecordConstructor:
  '{' 'as' 'class' ';'? recordField (',' recordField)* '}';

anonymousStructRecordConstructor:
  '{' 'as' 'struct' ';'? recordField (',' recordField)* '}';

recordField:
  name '=' expression;

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
  '(' expression ',' expression (',' expression)* ')';

explicitTupleConstructor:
  '(' 'as' 'new' ';'? expression ',' expression (',' expression)* ')';

classTupleConstructor:
  '(' 'as' 'class' ';'? expression ',' expression (',' expression)* ')';

structTupleConstructor:
  '(' 'as' 'struct' ';'? expression ',' expression (',' expression)* ')';

spreadExpression:
  '..' concatExpr_Inner;

/* -------------------- */
/* Interpolated strings */
/* -------------------- */

interpolatedString:
  BEGIN_INTERPOLATED_STRING interpStrComponent* (END_INTERPOLATED_STRING | errorUnsupportedEndInterpolatedStringSuffix);

verbatimInterpolatedString:
  BEGIN_VERBATIM_INTERPOLATED_STRING interpStrComponent* (END_INTERPOLATED_STRING | errorUnsupportedEndInterpolatedStringSuffix);

interpStrComponent:
  interpStrPart |
  interpStrPercent |
  interpStrExpression;

interpStrPart: INTERP_PART;
interpStrPercent: '%';
interpStrAlignment: INTERP_ALIGNMENT;
interpStrGeneralFormat: INTERP_FORMAT_GENERAL | INTERP_COMPONENTS_PART_SHORT;
interpStrStandardFormat: INTERP_FORMAT_STANDARD;
interpStrCustomFormat: INTERP_FORMAT_CUSTOM;
interpStrNumberFormat: INTERP_FORMAT_NUMBER;
interpStrComponentFormat:
  INTERP_COMPONENTS_PART_LONG (INTERP_COMPONENTS_PART_SHORT | INTERP_COMPONENTS_PART_LONG)* |
  INTERP_COMPONENTS_PART_SHORT (INTERP_COMPONENTS_PART_SHORT | INTERP_COMPONENTS_PART_LONG)+;

interpStrExpression:
  '{' expression ((interpStrAlignment? (interpStrStandardFormat | interpStrCustomFormat | interpStrNumberFormat | interpStrComponentFormat)?) | interpStrGeneralFormat) '}';

/* ---------------- */
/* Inline F# source */
/* ---------------- */

inlineSourceFree:
  BEGIN_INLINE_SOURCE WHITESPACE* string inlineSourceFirstLine inlineSourceLine*
  END_INLINE_SOURCE WHITESPACE* END_DIRECTIVE;

inlineSourceReturning:
  BEGIN_INLINE_SOURCE WHITESPACE* string inlineSourceFirstLine inlineSourceLine*
  END_INLINE_SOURCE WHITESPACE* 'return' WHITESPACE* END_DIRECTIVE;

inlineSourceTerminating:
  BEGIN_INLINE_SOURCE WHITESPACE* string inlineSourceFirstLine inlineSourceLine*
  END_INLINE_SOURCE WHITESPACE* 'throw' WHITESPACE* END_DIRECTIVE;

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

innerBinaryOperator:
  '+' | '-' | '*' | '/' | '%';

outerBinaryOperator:
  '<' | '<=' | '>' | '>=' | '==' | '!=' | '~=' |
  '&&' | '||';

unaryOperator:
  '+' | '-' | '~';

/* ---------- */
/* Primitives */
/* ---------- */

number:
  INT | FLOAT | EXP | HEX | errorUnsupportedNumberSuffix;

string:
  NORMAL_STRING | VERBATIM_STRING | CHAR_STRING | errorUnsupportedStringSuffix;

/* ------------ */
/* Error states */
/* ------------ */

errorMissingExpression:;

errorUnsupportedNumberSuffix:
  INT_SUFFIX | FLOAT_SUFFIX | EXP_SUFFIX | HEX_SUFFIX;

errorUnsupportedStringSuffix:
  NORMAL_STRING_SUFFIX | VERBATIM_STRING_SUFFIX | CHAR_STRING_SUFFIX;

errorUnsupportedEndInterpolatedStringSuffix:
  END_INTERPOLATED_STRING_SUFFIX;

errorUnderscoreReserved:
  UNDERSCORE;