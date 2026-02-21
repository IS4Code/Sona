// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Sona.Grammar {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="SonaParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface ISonaParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.chunk"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterChunk([NotNull] SonaParser.ChunkContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.chunk"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitChunk([NotNull] SonaParser.ChunkContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.namespaceSection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamespaceSection([NotNull] SonaParser.NamespaceSectionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.namespaceSection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamespaceSection([NotNull] SonaParser.NamespaceSectionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.packageSection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPackageSection([NotNull] SonaParser.PackageSectionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.packageSection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPackageSection([NotNull] SonaParser.PackageSectionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.mainBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMainBlock([NotNull] SonaParser.MainBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.mainBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMainBlock([NotNull] SonaParser.MainBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.valueBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValueBlock([NotNull] SonaParser.ValueBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.valueBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValueBlock([NotNull] SonaParser.ValueBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.valueTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValueTrail([NotNull] SonaParser.ValueTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.valueTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValueTrail([NotNull] SonaParser.ValueTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.freeBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFreeBlock([NotNull] SonaParser.FreeBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.freeBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFreeBlock([NotNull] SonaParser.FreeBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.freeTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFreeTrail([NotNull] SonaParser.FreeTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.freeTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFreeTrail([NotNull] SonaParser.FreeTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpenBlock([NotNull] SonaParser.OpenBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpenBlock([NotNull] SonaParser.OpenBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpenTrail([NotNull] SonaParser.OpenTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpenTrail([NotNull] SonaParser.OpenTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturningBlock([NotNull] SonaParser.ReturningBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturningBlock([NotNull] SonaParser.ReturningBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturningTrail([NotNull] SonaParser.ReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturningTrail([NotNull] SonaParser.ReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.terminatingBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTerminatingBlock([NotNull] SonaParser.TerminatingBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.terminatingBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTerminatingBlock([NotNull] SonaParser.TerminatingBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.terminatingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTerminatingTrail([NotNull] SonaParser.TerminatingTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.terminatingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTerminatingTrail([NotNull] SonaParser.TerminatingTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptingBlock([NotNull] SonaParser.InterruptingBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptingBlock([NotNull] SonaParser.InterruptingBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptingTrail([NotNull] SonaParser.InterruptingTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptingTrail([NotNull] SonaParser.InterruptingTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturningCoverBlock([NotNull] SonaParser.ReturningCoverBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturningCoverBlock([NotNull] SonaParser.ReturningCoverBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturningCoverTrail([NotNull] SonaParser.ReturningCoverTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturningCoverTrail([NotNull] SonaParser.ReturningCoverTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalBlock([NotNull] SonaParser.ConditionalBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalBlock([NotNull] SonaParser.ConditionalBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalTrail([NotNull] SonaParser.ConditionalTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalTrail([NotNull] SonaParser.ConditionalTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptibleBlock([NotNull] SonaParser.InterruptibleBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptibleBlock([NotNull] SonaParser.InterruptibleBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptibleTrail([NotNull] SonaParser.InterruptibleTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptibleTrail([NotNull] SonaParser.InterruptibleTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalCoverBlock([NotNull] SonaParser.ConditionalCoverBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalCoverBlock([NotNull] SonaParser.ConditionalCoverBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalCoverTrail([NotNull] SonaParser.ConditionalCoverTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalCoverTrail([NotNull] SonaParser.ConditionalCoverTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptingCoverBlock([NotNull] SonaParser.InterruptingCoverBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptingCoverBlock([NotNull] SonaParser.InterruptingCoverBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptingCoverTrail([NotNull] SonaParser.InterruptingCoverTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptingCoverTrail([NotNull] SonaParser.InterruptingCoverTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptibleCoverBlock([NotNull] SonaParser.InterruptibleCoverBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptibleCoverBlock([NotNull] SonaParser.InterruptibleCoverBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptibleCoverTrail([NotNull] SonaParser.InterruptibleCoverTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleCoverTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptibleCoverTrail([NotNull] SonaParser.InterruptibleCoverTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpenCoverBlock([NotNull] SonaParser.OpenCoverBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openCoverBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpenCoverBlock([NotNull] SonaParser.OpenCoverBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openToInterruptibleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpenToInterruptibleBlock([NotNull] SonaParser.OpenToInterruptibleBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openToInterruptibleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpenToInterruptibleBlock([NotNull] SonaParser.OpenToInterruptibleBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openToConditionalBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOpenToConditionalBlock([NotNull] SonaParser.OpenToConditionalBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openToConditionalBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOpenToConditionalBlock([NotNull] SonaParser.OpenToConditionalBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingToInterruptibleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptingToInterruptibleBlock([NotNull] SonaParser.InterruptingToInterruptibleBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingToInterruptibleBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptingToInterruptibleBlock([NotNull] SonaParser.InterruptingToInterruptibleBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningToConditionalBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturningToConditionalBlock([NotNull] SonaParser.ReturningToConditionalBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningToConditionalBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturningToConditionalBlock([NotNull] SonaParser.ReturningToConditionalBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIgnoredBlock([NotNull] SonaParser.IgnoredBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIgnoredBlock([NotNull] SonaParser.IgnoredBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIgnoredTrail([NotNull] SonaParser.IgnoredTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIgnoredTrail([NotNull] SonaParser.IgnoredTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredEmptyBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIgnoredEmptyBlock([NotNull] SonaParser.IgnoredEmptyBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredEmptyBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIgnoredEmptyBlock([NotNull] SonaParser.IgnoredEmptyBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredEmptyTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIgnoredEmptyTrail([NotNull] SonaParser.IgnoredEmptyTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredEmptyTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIgnoredEmptyTrail([NotNull] SonaParser.IgnoredEmptyTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterName([NotNull] SonaParser.NameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitName([NotNull] SonaParser.NameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberName([NotNull] SonaParser.MemberNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberName([NotNull] SonaParser.MemberNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.dynamicMemberName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDynamicMemberName([NotNull] SonaParser.DynamicMemberNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.dynamicMemberName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDynamicMemberName([NotNull] SonaParser.DynamicMemberNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.compoundName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompoundName([NotNull] SonaParser.CompoundNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.compoundName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompoundName([NotNull] SonaParser.CompoundNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.compoundNameGeneric"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompoundNameGeneric([NotNull] SonaParser.CompoundNameGenericContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.compoundNameGeneric"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompoundNameGeneric([NotNull] SonaParser.CompoundNameGenericContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typeArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypeArgument([NotNull] SonaParser.TypeArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typeArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypeArgument([NotNull] SonaParser.TypeArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] SonaParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] SonaParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nullableType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullableType([NotNull] SonaParser.NullableTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nullableType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullableType([NotNull] SonaParser.NullableTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conjunctionType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConjunctionType([NotNull] SonaParser.ConjunctionTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conjunctionType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConjunctionType([NotNull] SonaParser.ConjunctionTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtomicType([NotNull] SonaParser.AtomicTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtomicType([NotNull] SonaParser.AtomicTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.primitiveType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimitiveType([NotNull] SonaParser.PrimitiveTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.primitiveType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimitiveType([NotNull] SonaParser.PrimitiveTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.functionType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionType([NotNull] SonaParser.FunctionTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.functionType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionType([NotNull] SonaParser.FunctionTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramTypesList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParamTypesList([NotNull] SonaParser.ParamTypesListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramTypesList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParamTypesList([NotNull] SonaParser.ParamTypesListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramTypesTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParamTypesTuple([NotNull] SonaParser.ParamTypesTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramTypesTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParamTypesTuple([NotNull] SonaParser.ParamTypesTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNestedType([NotNull] SonaParser.NestedTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNestedType([NotNull] SonaParser.NestedTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tupleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTupleType([NotNull] SonaParser.TupleTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tupleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTupleType([NotNull] SonaParser.TupleTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.classTupleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassTupleType([NotNull] SonaParser.ClassTupleTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.classTupleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassTupleType([NotNull] SonaParser.ClassTupleTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.structTupleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructTupleType([NotNull] SonaParser.StructTupleTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.structTupleType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructTupleType([NotNull] SonaParser.StructTupleTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousRecordType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousRecordType([NotNull] SonaParser.AnonymousRecordTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousRecordType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousRecordType([NotNull] SonaParser.AnonymousRecordTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousClassRecordType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousClassRecordType([NotNull] SonaParser.AnonymousClassRecordTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousClassRecordType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousClassRecordType([NotNull] SonaParser.AnonymousClassRecordTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousStructRecordType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousStructRecordType([NotNull] SonaParser.AnonymousStructRecordTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousStructRecordType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousStructRecordType([NotNull] SonaParser.AnonymousStructRecordTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousRecordMemberDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousRecordMemberDeclaration([NotNull] SonaParser.AnonymousRecordMemberDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousRecordMemberDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousRecordMemberDeclaration([NotNull] SonaParser.AnonymousRecordMemberDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypeSuffix([NotNull] SonaParser.TypeSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypeSuffix([NotNull] SonaParser.TypeSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.arrayTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayTypeSuffix([NotNull] SonaParser.ArrayTypeSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.arrayTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayTypeSuffix([NotNull] SonaParser.ArrayTypeSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.multiArrayTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiArrayTypeSuffix([NotNull] SonaParser.MultiArrayTypeSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.multiArrayTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiArrayTypeSuffix([NotNull] SonaParser.MultiArrayTypeSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionTypeSuffix([NotNull] SonaParser.OptionTypeSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionTypeSuffix([NotNull] SonaParser.OptionTypeSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.sequenceTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSequenceTypeSuffix([NotNull] SonaParser.SequenceTypeSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.sequenceTypeSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSequenceTypeSuffix([NotNull] SonaParser.SequenceTypeSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.genericArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGenericArguments([NotNull] SonaParser.GenericArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.genericArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGenericArguments([NotNull] SonaParser.GenericArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.genericArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGenericArgument([NotNull] SonaParser.GenericArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.genericArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGenericArgument([NotNull] SonaParser.GenericArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.measureArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMeasureArgument([NotNull] SonaParser.MeasureArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.measureArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMeasureArgument([NotNull] SonaParser.MeasureArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.measureExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMeasureExpression([NotNull] SonaParser.MeasureExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.measureExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMeasureExpression([NotNull] SonaParser.MeasureExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.measureOperand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMeasureOperand([NotNull] SonaParser.MeasureOperandContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.measureOperand"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMeasureOperand([NotNull] SonaParser.MeasureOperandContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.literalArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteralArgument([NotNull] SonaParser.LiteralArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.literalArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteralArgument([NotNull] SonaParser.LiteralArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.localAttrList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLocalAttrList([NotNull] SonaParser.LocalAttrListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.localAttrList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLocalAttrList([NotNull] SonaParser.LocalAttrListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.localAttribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLocalAttribute([NotNull] SonaParser.LocalAttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.localAttribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLocalAttribute([NotNull] SonaParser.LocalAttributeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.localAttrTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLocalAttrTarget([NotNull] SonaParser.LocalAttrTargetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.localAttrTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLocalAttrTarget([NotNull] SonaParser.LocalAttrTargetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.globalAttrList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGlobalAttrList([NotNull] SonaParser.GlobalAttrListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.globalAttrList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGlobalAttrList([NotNull] SonaParser.GlobalAttrListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.globalAttribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGlobalAttribute([NotNull] SonaParser.GlobalAttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.globalAttribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGlobalAttribute([NotNull] SonaParser.GlobalAttributeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.globalAttrTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGlobalAttrTarget([NotNull] SonaParser.GlobalAttrTargetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.globalAttrTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGlobalAttrTarget([NotNull] SonaParser.GlobalAttrTargetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.attrGroup"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttrGroup([NotNull] SonaParser.AttrGroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.attrGroup"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttrGroup([NotNull] SonaParser.AttrGroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.compiledNameAttr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompiledNameAttr([NotNull] SonaParser.CompiledNameAttrContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.compiledNameAttr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompiledNameAttr([NotNull] SonaParser.CompiledNameAttrContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.attrPosArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttrPosArg([NotNull] SonaParser.AttrPosArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.attrPosArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttrPosArg([NotNull] SonaParser.AttrPosArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.attrNamedArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttrNamedArg([NotNull] SonaParser.AttrNamedArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.attrNamedArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttrNamedArg([NotNull] SonaParser.AttrNamedArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] SonaParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] SonaParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.closingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClosingStatement([NotNull] SonaParser.ClosingStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.closingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClosingStatement([NotNull] SonaParser.ClosingStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.echoStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEchoStatement([NotNull] SonaParser.EchoStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.echoStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEchoStatement([NotNull] SonaParser.EchoStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.implicitReturnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterImplicitReturnStatement([NotNull] SonaParser.ImplicitReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.implicitReturnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitImplicitReturnStatement([NotNull] SonaParser.ImplicitReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] SonaParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] SonaParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returnOptionStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnOptionStatement([NotNull] SonaParser.ReturnOptionStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returnOptionStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnOptionStatement([NotNull] SonaParser.ReturnOptionStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returnFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnFollowStatement([NotNull] SonaParser.ReturnFollowStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returnFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnFollowStatement([NotNull] SonaParser.ReturnFollowStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldStatement([NotNull] SonaParser.YieldStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldStatement([NotNull] SonaParser.YieldStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldFollowStatement([NotNull] SonaParser.YieldFollowStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldFollowStatement([NotNull] SonaParser.YieldFollowStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldEachStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldEachStatement([NotNull] SonaParser.YieldEachStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldEachStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldEachStatement([NotNull] SonaParser.YieldEachStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldBreakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldBreakStatement([NotNull] SonaParser.YieldBreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldBreakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldBreakStatement([NotNull] SonaParser.YieldBreakStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldReturnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldReturnStatement([NotNull] SonaParser.YieldReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldReturnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldReturnStatement([NotNull] SonaParser.YieldReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldReturnFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldReturnFollowStatement([NotNull] SonaParser.YieldReturnFollowStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldReturnFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldReturnFollowStatement([NotNull] SonaParser.YieldReturnFollowStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakStatement([NotNull] SonaParser.BreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakStatement([NotNull] SonaParser.BreakStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.continueStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContinueStatement([NotNull] SonaParser.ContinueStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.continueStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContinueStatement([NotNull] SonaParser.ContinueStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.continueFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContinueFollowStatement([NotNull] SonaParser.ContinueFollowStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.continueFollowStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContinueFollowStatement([NotNull] SonaParser.ContinueFollowStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.throwStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterThrowStatement([NotNull] SonaParser.ThrowStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.throwStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitThrowStatement([NotNull] SonaParser.ThrowStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.withDefaultArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWithDefaultArgument([NotNull] SonaParser.WithDefaultArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.withDefaultArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWithDefaultArgument([NotNull] SonaParser.WithDefaultArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.withDefaultSequenceArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWithDefaultSequenceArgument([NotNull] SonaParser.WithDefaultSequenceArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.withDefaultSequenceArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWithDefaultSequenceArgument([NotNull] SonaParser.WithDefaultSequenceArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.with_Argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWith_Argument([NotNull] SonaParser.With_ArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.with_Argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWith_Argument([NotNull] SonaParser.With_ArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.withStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWithStatement([NotNull] SonaParser.WithStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.withStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWithStatement([NotNull] SonaParser.WithStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowStatement([NotNull] SonaParser.FollowStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowStatement([NotNull] SonaParser.FollowStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithTrailing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowWithTrailing([NotNull] SonaParser.FollowWithTrailingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithTrailing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowWithTrailing([NotNull] SonaParser.FollowWithTrailingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowWithTerminating([NotNull] SonaParser.FollowWithTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowWithTerminating([NotNull] SonaParser.FollowWithTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowWithInterrupting([NotNull] SonaParser.FollowWithInterruptingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowWithInterrupting([NotNull] SonaParser.FollowWithInterruptingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowWithInterruptible([NotNull] SonaParser.FollowWithInterruptibleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowWithInterruptible([NotNull] SonaParser.FollowWithInterruptibleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowWithReturning([NotNull] SonaParser.FollowWithReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowWithReturning([NotNull] SonaParser.FollowWithReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowWithConditional([NotNull] SonaParser.FollowWithConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowWithConditional([NotNull] SonaParser.FollowWithConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithTrailing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldWithTrailing([NotNull] SonaParser.YieldWithTrailingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithTrailing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldWithTrailing([NotNull] SonaParser.YieldWithTrailingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldWithTerminating([NotNull] SonaParser.YieldWithTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldWithTerminating([NotNull] SonaParser.YieldWithTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldWithInterrupting([NotNull] SonaParser.YieldWithInterruptingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldWithInterrupting([NotNull] SonaParser.YieldWithInterruptingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldWithInterruptible([NotNull] SonaParser.YieldWithInterruptibleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldWithInterruptible([NotNull] SonaParser.YieldWithInterruptibleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldWithReturning([NotNull] SonaParser.YieldWithReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldWithReturning([NotNull] SonaParser.YieldWithReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterYieldWithConditional([NotNull] SonaParser.YieldWithConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitYieldWithConditional([NotNull] SonaParser.YieldWithConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.trailingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTrailingStatement([NotNull] SonaParser.TrailingStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.trailingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTrailingStatement([NotNull] SonaParser.TrailingStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturningStatement([NotNull] SonaParser.ReturningStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturningStatement([NotNull] SonaParser.ReturningStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptingStatement([NotNull] SonaParser.InterruptingStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptingStatement([NotNull] SonaParser.InterruptingStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterruptibleStatement([NotNull] SonaParser.InterruptibleStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterruptibleStatement([NotNull] SonaParser.InterruptibleStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalStatement([NotNull] SonaParser.ConditionalStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalStatement([NotNull] SonaParser.ConditionalStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.terminatingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTerminatingStatement([NotNull] SonaParser.TerminatingStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.terminatingStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTerminatingStatement([NotNull] SonaParser.TerminatingStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.expressionStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpressionStatement([NotNull] SonaParser.ExpressionStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.expressionStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpressionStatement([NotNull] SonaParser.ExpressionStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredTrail_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIgnoredTrail_Group([NotNull] SonaParser.IgnoredTrail_GroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredTrail_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIgnoredTrail_Group([NotNull] SonaParser.IgnoredTrail_GroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementFree([NotNull] SonaParser.DoStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementFree([NotNull] SonaParser.DoStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementTerminating([NotNull] SonaParser.DoStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementTerminating([NotNull] SonaParser.DoStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementInterrupting([NotNull] SonaParser.DoStatementInterruptingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementInterrupting([NotNull] SonaParser.DoStatementInterruptingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementInterruptingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementInterruptingTrail([NotNull] SonaParser.DoStatementInterruptingTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementInterruptingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementInterruptingTrail([NotNull] SonaParser.DoStatementInterruptingTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementInterruptible([NotNull] SonaParser.DoStatementInterruptibleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementInterruptible([NotNull] SonaParser.DoStatementInterruptibleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementReturning([NotNull] SonaParser.DoStatementReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementReturning([NotNull] SonaParser.DoStatementReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementReturningTrail([NotNull] SonaParser.DoStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementReturningTrail([NotNull] SonaParser.DoStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoStatementConditional([NotNull] SonaParser.DoStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoStatementConditional([NotNull] SonaParser.DoStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.if_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIf_Group([NotNull] SonaParser.If_GroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.if_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIf_Group([NotNull] SonaParser.If_GroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.elseif_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElseif_Group([NotNull] SonaParser.Elseif_GroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.elseif_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElseif_Group([NotNull] SonaParser.Elseif_GroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIf([NotNull] SonaParser.IfContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.if"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIf([NotNull] SonaParser.IfContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.elseif"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElseif([NotNull] SonaParser.ElseifContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.elseif"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElseif([NotNull] SonaParser.ElseifContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseIf"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseIf([NotNull] SonaParser.CaseIfContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseIf"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseIf([NotNull] SonaParser.CaseIfContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseElseif"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseElseif([NotNull] SonaParser.CaseElseifContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseElseif"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseElseif([NotNull] SonaParser.CaseElseifContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.else"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElse([NotNull] SonaParser.ElseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.else"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElse([NotNull] SonaParser.ElseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementFree([NotNull] SonaParser.IfStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementFree([NotNull] SonaParser.IfStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementTerminating([NotNull] SonaParser.IfStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementTerminating([NotNull] SonaParser.IfStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementInterrupting([NotNull] SonaParser.IfStatementInterruptingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementInterrupting"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementInterrupting([NotNull] SonaParser.IfStatementInterruptingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementInterruptingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementInterruptingTrail([NotNull] SonaParser.IfStatementInterruptingTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementInterruptingTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementInterruptingTrail([NotNull] SonaParser.IfStatementInterruptingTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementInterruptible([NotNull] SonaParser.IfStatementInterruptibleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementInterruptible"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementInterruptible([NotNull] SonaParser.IfStatementInterruptibleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementReturning([NotNull] SonaParser.IfStatementReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementReturning([NotNull] SonaParser.IfStatementReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementReturningTrailFromElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementReturningTrailFromElse([NotNull] SonaParser.IfStatementReturningTrailFromElseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementReturningTrailFromElse"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementReturningTrailFromElse([NotNull] SonaParser.IfStatementReturningTrailFromElseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementReturningTrail([NotNull] SonaParser.IfStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementReturningTrail([NotNull] SonaParser.IfStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatementConditional([NotNull] SonaParser.IfStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatementConditional([NotNull] SonaParser.IfStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhile([NotNull] SonaParser.WhileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.while"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhile([NotNull] SonaParser.WhileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseWhile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseWhile([NotNull] SonaParser.CaseWhileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseWhile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseWhile([NotNull] SonaParser.CaseWhileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileTrue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileTrue([NotNull] SonaParser.WhileTrueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileTrue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileTrue([NotNull] SonaParser.WhileTrueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatementFree([NotNull] SonaParser.WhileStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatementFree([NotNull] SonaParser.WhileStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatementFreeInterrupted([NotNull] SonaParser.WhileStatementFreeInterruptedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatementFreeInterrupted([NotNull] SonaParser.WhileStatementFreeInterruptedContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatementTerminating([NotNull] SonaParser.WhileStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatementTerminating([NotNull] SonaParser.WhileStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatementReturningTrail([NotNull] SonaParser.WhileStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatementReturningTrail([NotNull] SonaParser.WhileStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatementConditional([NotNull] SonaParser.WhileStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatementConditional([NotNull] SonaParser.WhileStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.until"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUntil([NotNull] SonaParser.UntilContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.until"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUntil([NotNull] SonaParser.UntilContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatementFree([NotNull] SonaParser.RepeatStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatementFree([NotNull] SonaParser.RepeatStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatementFreeInterrupted([NotNull] SonaParser.RepeatStatementFreeInterruptedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatementFreeInterrupted([NotNull] SonaParser.RepeatStatementFreeInterruptedContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatementTerminating([NotNull] SonaParser.RepeatStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatementTerminating([NotNull] SonaParser.RepeatStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatementReturningTrail([NotNull] SonaParser.RepeatStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatementReturningTrail([NotNull] SonaParser.RepeatStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRepeatStatementConditional([NotNull] SonaParser.RepeatStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRepeatStatementConditional([NotNull] SonaParser.RepeatStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.for"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFor([NotNull] SonaParser.ForContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.for"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFor([NotNull] SonaParser.ForContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forSimple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForSimple([NotNull] SonaParser.ForSimpleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forSimple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForSimple([NotNull] SonaParser.ForSimpleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forSimpleStep"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForSimpleStep([NotNull] SonaParser.ForSimpleStepContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forSimpleStep"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForSimpleStep([NotNull] SonaParser.ForSimpleStepContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForRange([NotNull] SonaParser.ForRangeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForRange([NotNull] SonaParser.ForRangeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRangeStep"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForRangeStep([NotNull] SonaParser.ForRangeStepContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRangeStep"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForRangeStep([NotNull] SonaParser.ForRangeStepContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRangePrimitiveStep"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForRangePrimitiveStep([NotNull] SonaParser.ForRangePrimitiveStepContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRangePrimitiveStep"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForRangePrimitiveStep([NotNull] SonaParser.ForRangePrimitiveStepContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRangeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForRangeExpression([NotNull] SonaParser.ForRangeExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRangeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForRangeExpression([NotNull] SonaParser.ForRangeExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatementFree([NotNull] SonaParser.ForStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatementFree([NotNull] SonaParser.ForStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatementFreeInterrupted([NotNull] SonaParser.ForStatementFreeInterruptedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatementFreeInterrupted([NotNull] SonaParser.ForStatementFreeInterruptedContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatementReturningTrail([NotNull] SonaParser.ForStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatementReturningTrail([NotNull] SonaParser.ForStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatementConditional([NotNull] SonaParser.ForStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatementConditional([NotNull] SonaParser.ForStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitch([NotNull] SonaParser.SwitchContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitch([NotNull] SonaParser.SwitchContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.case"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCase([NotNull] SonaParser.CaseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.case"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCase([NotNull] SonaParser.CaseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whenClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhenClause([NotNull] SonaParser.WhenClauseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whenClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhenClause([NotNull] SonaParser.WhenClauseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementFree([NotNull] SonaParser.SwitchStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementFree([NotNull] SonaParser.SwitchStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementFreeInterrupted([NotNull] SonaParser.SwitchStatementFreeInterruptedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementFreeInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementFreeInterrupted([NotNull] SonaParser.SwitchStatementFreeInterruptedContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementTerminating([NotNull] SonaParser.SwitchStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementTerminating([NotNull] SonaParser.SwitchStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementTerminatingInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementTerminatingInterrupted([NotNull] SonaParser.SwitchStatementTerminatingInterruptedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementTerminatingInterrupted"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementTerminatingInterrupted([NotNull] SonaParser.SwitchStatementTerminatingInterruptedContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementReturning([NotNull] SonaParser.SwitchStatementReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementReturning([NotNull] SonaParser.SwitchStatementReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementReturningTrail([NotNull] SonaParser.SwitchStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementReturningTrail([NotNull] SonaParser.SwitchStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatementConditional([NotNull] SonaParser.SwitchStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatementConditional([NotNull] SonaParser.SwitchStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.try"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTry([NotNull] SonaParser.TryContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.try"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTry([NotNull] SonaParser.TryContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.finallyBranch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFinallyBranch([NotNull] SonaParser.FinallyBranchContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.finallyBranch"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFinallyBranch([NotNull] SonaParser.FinallyBranchContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.catch_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCatch_Group([NotNull] SonaParser.Catch_GroupContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.catch_Group"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCatch_Group([NotNull] SonaParser.Catch_GroupContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.catchAsCase"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCatchAsCase([NotNull] SonaParser.CatchAsCaseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.catchAsCase"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCatchAsCase([NotNull] SonaParser.CatchAsCaseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.catchCase"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCatchCase([NotNull] SonaParser.CatchCaseContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.catchCase"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCatchCase([NotNull] SonaParser.CatchCaseContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchStatementFree([NotNull] SonaParser.TryCatchStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchStatementFree([NotNull] SonaParser.TryCatchStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryFinallyStatementFree([NotNull] SonaParser.TryFinallyStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryFinallyStatementFree([NotNull] SonaParser.TryFinallyStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchFinallyStatementFree([NotNull] SonaParser.TryCatchFinallyStatementFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchFinallyStatementFree([NotNull] SonaParser.TryCatchFinallyStatementFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchStatementTerminating([NotNull] SonaParser.TryCatchStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchStatementTerminating([NotNull] SonaParser.TryCatchStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchStatementConditional([NotNull] SonaParser.TryCatchStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchStatementConditional([NotNull] SonaParser.TryCatchStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchStatementReturning([NotNull] SonaParser.TryCatchStatementReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchStatementReturning([NotNull] SonaParser.TryCatchStatementReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchStatementReturningTrail([NotNull] SonaParser.TryCatchStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchStatementReturningTrail([NotNull] SonaParser.TryCatchStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryFinallyStatementTerminating([NotNull] SonaParser.TryFinallyStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryFinallyStatementTerminating([NotNull] SonaParser.TryFinallyStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryFinallyStatementConditional([NotNull] SonaParser.TryFinallyStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryFinallyStatementConditional([NotNull] SonaParser.TryFinallyStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryFinallyStatementReturning([NotNull] SonaParser.TryFinallyStatementReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryFinallyStatementReturning([NotNull] SonaParser.TryFinallyStatementReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryFinallyStatementReturningTrail([NotNull] SonaParser.TryFinallyStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryFinallyStatementReturningTrail([NotNull] SonaParser.TryFinallyStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchFinallyStatementTerminating([NotNull] SonaParser.TryCatchFinallyStatementTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchFinallyStatementTerminating([NotNull] SonaParser.TryCatchFinallyStatementTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchFinallyStatementConditional([NotNull] SonaParser.TryCatchFinallyStatementConditionalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementConditional"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchFinallyStatementConditional([NotNull] SonaParser.TryCatchFinallyStatementConditionalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchFinallyStatementReturning([NotNull] SonaParser.TryCatchFinallyStatementReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchFinallyStatementReturning([NotNull] SonaParser.TryCatchFinallyStatementReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTryCatchFinallyStatementReturningTrail([NotNull] SonaParser.TryCatchFinallyStatementReturningTrailContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturningTrail"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTryCatchFinallyStatementReturningTrail([NotNull] SonaParser.TryCatchFinallyStatementReturningTrailContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.topLevelStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTopLevelStatement([NotNull] SonaParser.TopLevelStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.topLevelStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTopLevelStatement([NotNull] SonaParser.TopLevelStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.importStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterImportStatement([NotNull] SonaParser.ImportStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.importStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitImportStatement([NotNull] SonaParser.ImportStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.importTypeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterImportTypeStatement([NotNull] SonaParser.ImportTypeStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.importTypeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitImportTypeStatement([NotNull] SonaParser.ImportTypeStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.symbolContentsArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSymbolContentsArg([NotNull] SonaParser.SymbolContentsArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.symbolContentsArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSymbolContentsArg([NotNull] SonaParser.SymbolContentsArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.importFileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterImportFileStatement([NotNull] SonaParser.ImportFileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.importFileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitImportFileStatement([NotNull] SonaParser.ImportFileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.includeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIncludeStatement([NotNull] SonaParser.IncludeStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.includeStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIncludeStatement([NotNull] SonaParser.IncludeStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.requireStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRequireStatement([NotNull] SonaParser.RequireStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.requireStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRequireStatement([NotNull] SonaParser.RequireStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.packageStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPackageStatement([NotNull] SonaParser.PackageStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.packageStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPackageStatement([NotNull] SonaParser.PackageStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleVariableDecl([NotNull] SonaParser.SimpleVariableDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleVariableDecl([NotNull] SonaParser.SimpleVariableDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowVariableDecl([NotNull] SonaParser.FollowVariableDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowVariableDecl([NotNull] SonaParser.FollowVariableDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.multiVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiVariableDecl([NotNull] SonaParser.MultiVariableDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.multiVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiVariableDecl([NotNull] SonaParser.MultiVariableDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.variableDecl_Prefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableDecl_Prefix([NotNull] SonaParser.VariableDecl_PrefixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.variableDecl_Prefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableDecl_Prefix([NotNull] SonaParser.VariableDecl_PrefixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.lazyVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLazyVariableDecl([NotNull] SonaParser.LazyVariableDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.lazyVariableDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLazyVariableDecl([NotNull] SonaParser.LazyVariableDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.multiFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiFuncDecl([NotNull] SonaParser.MultiFuncDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.multiFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiFuncDecl([NotNull] SonaParser.MultiFuncDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.funcDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncDecl([NotNull] SonaParser.FuncDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.funcDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncDecl([NotNull] SonaParser.FuncDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineFuncDecl([NotNull] SonaParser.InlineFuncDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineFuncDecl([NotNull] SonaParser.InlineFuncDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseFuncDecl([NotNull] SonaParser.CaseFuncDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseFuncDecl([NotNull] SonaParser.CaseFuncDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineCaseFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineCaseFuncDecl([NotNull] SonaParser.InlineCaseFuncDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineCaseFuncDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineCaseFuncDecl([NotNull] SonaParser.InlineCaseFuncDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseFuncName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseFuncName([NotNull] SonaParser.CaseFuncNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseFuncName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseFuncName([NotNull] SonaParser.CaseFuncNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.funcBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncBody([NotNull] SonaParser.FuncBodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.funcBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncBody([NotNull] SonaParser.FuncBodyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParamList([NotNull] SonaParser.ParamListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParamList([NotNull] SonaParser.ParamListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParamTuple([NotNull] SonaParser.ParamTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParamTuple([NotNull] SonaParser.ParamTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclaration([NotNull] SonaParser.DeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclaration([NotNull] SonaParser.DeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionalName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionalName([NotNull] SonaParser.OptionalNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionalName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionalName([NotNull] SonaParser.OptionalNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberOrAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberOrAssignment([NotNull] SonaParser.MemberOrAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberOrAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberOrAssignment([NotNull] SonaParser.MemberOrAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment([NotNull] SonaParser.AssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment([NotNull] SonaParser.AssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberDiscard"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberDiscard([NotNull] SonaParser.MemberDiscardContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberDiscard"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberDiscard([NotNull] SonaParser.MemberDiscardContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followAssignmentStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowAssignmentStatement([NotNull] SonaParser.FollowAssignmentStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followAssignmentStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowAssignmentStatement([NotNull] SonaParser.FollowAssignmentStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.patternArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPatternArgument([NotNull] SonaParser.PatternArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.patternArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPatternArgument([NotNull] SonaParser.PatternArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.pattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPattern([NotNull] SonaParser.PatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.pattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPattern([NotNull] SonaParser.PatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicPattern([NotNull] SonaParser.LogicPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicPattern([NotNull] SonaParser.LogicPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern_AndPrefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicPattern_AndPrefix([NotNull] SonaParser.LogicPattern_AndPrefixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern_AndPrefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicPattern_AndPrefix([NotNull] SonaParser.LogicPattern_AndPrefixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern_AndSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicPattern_AndSuffix([NotNull] SonaParser.LogicPattern_AndSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern_AndSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicPattern_AndSuffix([NotNull] SonaParser.LogicPattern_AndSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern_Argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicPattern_Argument([NotNull] SonaParser.LogicPattern_ArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern_Argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicPattern_Argument([NotNull] SonaParser.LogicPattern_ArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryLogicPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryLogicPattern([NotNull] SonaParser.UnaryLogicPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryLogicPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryLogicPattern([NotNull] SonaParser.UnaryLogicPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryPattern([NotNull] SonaParser.UnaryPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryPattern([NotNull] SonaParser.UnaryPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypePattern([NotNull] SonaParser.TypePatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypePattern([NotNull] SonaParser.TypePatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePattern_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypePattern_Contents([NotNull] SonaParser.TypePattern_ContentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePattern_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypePattern_Contents([NotNull] SonaParser.TypePattern_ContentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePatternExplicit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypePatternExplicit([NotNull] SonaParser.TypePatternExplicitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePatternExplicit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypePatternExplicit([NotNull] SonaParser.TypePatternExplicitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePatternImplicit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypePatternImplicit([NotNull] SonaParser.TypePatternImplicitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePatternImplicit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypePatternImplicit([NotNull] SonaParser.TypePatternImplicitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.notNullPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNotNullPattern([NotNull] SonaParser.NotNullPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.notNullPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNotNullPattern([NotNull] SonaParser.NotNullPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicNotNullPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtomicNotNullPattern([NotNull] SonaParser.AtomicNotNullPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicNotNullPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtomicNotNullPattern([NotNull] SonaParser.AtomicNotNullPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nullPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullPattern([NotNull] SonaParser.NullPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nullPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullPattern([NotNull] SonaParser.NullPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nullArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullArg([NotNull] SonaParser.NullArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nullArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullArg([NotNull] SonaParser.NullArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.annotationPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnnotationPattern([NotNull] SonaParser.AnnotationPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.annotationPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnnotationPattern([NotNull] SonaParser.AnnotationPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtomicPattern([NotNull] SonaParser.AtomicPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtomicPattern([NotNull] SonaParser.AtomicPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNestedPattern([NotNull] SonaParser.NestedPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNestedPattern([NotNull] SonaParser.NestedPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.somePattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSomePattern([NotNull] SonaParser.SomePatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.somePattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSomePattern([NotNull] SonaParser.SomePatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleNamedPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleNamedPattern([NotNull] SonaParser.SimpleNamedPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleNamedPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleNamedPattern([NotNull] SonaParser.SimpleNamedPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.namedPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamedPattern([NotNull] SonaParser.NamedPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.namedPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamedPattern([NotNull] SonaParser.NamedPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberPattern([NotNull] SonaParser.MemberPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberPattern([NotNull] SonaParser.MemberPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.relationalPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationalPattern([NotNull] SonaParser.RelationalPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.relationalPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationalPattern([NotNull] SonaParser.RelationalPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.regexPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRegexPattern([NotNull] SonaParser.RegexPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.regexPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRegexPattern([NotNull] SonaParser.RegexPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.regexGroupStart"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRegexGroupStart([NotNull] SonaParser.RegexGroupStartContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.regexGroupStart"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRegexGroupStart([NotNull] SonaParser.RegexGroupStartContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.patternArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPatternArguments([NotNull] SonaParser.PatternArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.patternArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPatternArguments([NotNull] SonaParser.PatternArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simplePatternArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimplePatternArgument([NotNull] SonaParser.SimplePatternArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simplePatternArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimplePatternArgument([NotNull] SonaParser.SimplePatternArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.emptyPatternArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEmptyPatternArgTuple([NotNull] SonaParser.EmptyPatternArgTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.emptyPatternArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEmptyPatternArgTuple([NotNull] SonaParser.EmptyPatternArgTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.lastPatternArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLastPatternArgTuple([NotNull] SonaParser.LastPatternArgTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.lastPatternArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLastPatternArgTuple([NotNull] SonaParser.LastPatternArgTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.patternArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPatternArgTuple([NotNull] SonaParser.PatternArgTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.patternArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPatternArgTuple([NotNull] SonaParser.PatternArgTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.basicConstructPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBasicConstructPattern([NotNull] SonaParser.BasicConstructPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.basicConstructPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBasicConstructPattern([NotNull] SonaParser.BasicConstructPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fullConstructPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFullConstructPattern([NotNull] SonaParser.FullConstructPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fullConstructPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFullConstructPattern([NotNull] SonaParser.FullConstructPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.emptyFieldAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEmptyFieldAssignment([NotNull] SonaParser.EmptyFieldAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.emptyFieldAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEmptyFieldAssignment([NotNull] SonaParser.EmptyFieldAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fieldRelation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFieldRelation([NotNull] SonaParser.FieldRelationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fieldRelation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFieldRelation([NotNull] SonaParser.FieldRelationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.recordConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRecordConstructorPattern([NotNull] SonaParser.RecordConstructorPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.recordConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRecordConstructorPattern([NotNull] SonaParser.RecordConstructorPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.arrayConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayConstructorPattern([NotNull] SonaParser.ArrayConstructorPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.arrayConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayConstructorPattern([NotNull] SonaParser.ArrayConstructorPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTupleConstructorPattern([NotNull] SonaParser.TupleConstructorPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTupleConstructorPattern([NotNull] SonaParser.TupleConstructorPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.explicitTupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExplicitTupleConstructorPattern([NotNull] SonaParser.ExplicitTupleConstructorPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.explicitTupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExplicitTupleConstructorPattern([NotNull] SonaParser.ExplicitTupleConstructorPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.classTupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassTupleConstructorPattern([NotNull] SonaParser.ClassTupleConstructorPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.classTupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassTupleConstructorPattern([NotNull] SonaParser.ClassTupleConstructorPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.structTupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructTupleConstructorPattern([NotNull] SonaParser.StructTupleConstructorPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.structTupleConstructorPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructTupleConstructorPattern([NotNull] SonaParser.StructTupleConstructorPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tuplePattern_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTuplePattern_Contents([NotNull] SonaParser.TuplePattern_ContentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tuplePattern_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTuplePattern_Contents([NotNull] SonaParser.TuplePattern_ContentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberTestPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberTestPattern([NotNull] SonaParser.MemberTestPatternContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberTestPattern"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberTestPattern([NotNull] SonaParser.MemberTestPatternContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] SonaParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] SonaParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicLogicExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtomicLogicExpr([NotNull] SonaParser.AtomicLogicExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicLogicExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtomicLogicExpr([NotNull] SonaParser.AtomicLogicExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicExpr([NotNull] SonaParser.LogicExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicExpr([NotNull] SonaParser.LogicExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineIfExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineIfExpr([NotNull] SonaParser.InlineIfExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineIfExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineIfExpr([NotNull] SonaParser.InlineIfExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.booleanExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBooleanExpr([NotNull] SonaParser.BooleanExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.booleanExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBooleanExpr([NotNull] SonaParser.BooleanExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.relationalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationalExpr([NotNull] SonaParser.RelationalExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.relationalExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationalExpr([NotNull] SonaParser.RelationalExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.coalesceExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCoalesceExpr([NotNull] SonaParser.CoalesceExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.coalesceExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCoalesceExpr([NotNull] SonaParser.CoalesceExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.concatExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConcatExpr([NotNull] SonaParser.ConcatExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.concatExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConcatExpr([NotNull] SonaParser.ConcatExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.concatExpr_Inner"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConcatExpr_Inner([NotNull] SonaParser.ConcatExpr_InnerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.concatExpr_Inner"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConcatExpr_Inner([NotNull] SonaParser.ConcatExpr_InnerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitOrExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBitOrExpr([NotNull] SonaParser.BitOrExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitOrExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBitOrExpr([NotNull] SonaParser.BitOrExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitXorExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBitXorExpr([NotNull] SonaParser.BitXorExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitXorExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBitXorExpr([NotNull] SonaParser.BitXorExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitAndExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBitAndExpr([NotNull] SonaParser.BitAndExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitAndExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBitAndExpr([NotNull] SonaParser.BitAndExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitShiftExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBitShiftExpr([NotNull] SonaParser.BitShiftExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitShiftExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBitShiftExpr([NotNull] SonaParser.BitShiftExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.innerExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInnerExpr([NotNull] SonaParser.InnerExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.innerExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInnerExpr([NotNull] SonaParser.InnerExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.annotationExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnnotationExpr([NotNull] SonaParser.AnnotationExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.annotationExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnnotationExpr([NotNull] SonaParser.AnnotationExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryExpr([NotNull] SonaParser.UnaryExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryExpr([NotNull] SonaParser.UnaryExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAtomicExpr([NotNull] SonaParser.AtomicExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAtomicExpr([NotNull] SonaParser.AtomicExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.hashExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterHashExpr([NotNull] SonaParser.HashExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.hashExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitHashExpr([NotNull] SonaParser.HashExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.notExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNotExpr([NotNull] SonaParser.NotExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.notExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNotExpr([NotNull] SonaParser.NotExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryNumberConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryNumberConvertExpr([NotNull] SonaParser.UnaryNumberConvertExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryNumberConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryNumberConvertExpr([NotNull] SonaParser.UnaryNumberConvertExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryCharConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryCharConvertExpr([NotNull] SonaParser.UnaryCharConvertExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryCharConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryCharConvertExpr([NotNull] SonaParser.UnaryCharConvertExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryConvertExpr([NotNull] SonaParser.UnaryConvertExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryConvertExpr([NotNull] SonaParser.UnaryConvertExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleExpr([NotNull] SonaParser.SimpleExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleExpr([NotNull] SonaParser.SimpleExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNestedExpr([NotNull] SonaParser.NestedExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNestedExpr([NotNull] SonaParser.NestedExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNestedAssignment([NotNull] SonaParser.NestedAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNestedAssignment([NotNull] SonaParser.NestedAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.primitiveExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimitiveExpr([NotNull] SonaParser.PrimitiveExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.primitiveExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimitiveExpr([NotNull] SonaParser.PrimitiveExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.namedValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNamedValue([NotNull] SonaParser.NamedValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.namedValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNamedValue([NotNull] SonaParser.NamedValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.funcExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncExpr([NotNull] SonaParser.FuncExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.funcExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncExpr([NotNull] SonaParser.FuncExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseFuncRefExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseFuncRefExpr([NotNull] SonaParser.CaseFuncRefExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseFuncRefExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseFuncRefExpr([NotNull] SonaParser.CaseFuncRefExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineExpr([NotNull] SonaParser.InlineExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineExpr([NotNull] SonaParser.InlineExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.basicConstructExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBasicConstructExpr([NotNull] SonaParser.BasicConstructExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.basicConstructExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBasicConstructExpr([NotNull] SonaParser.BasicConstructExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fullConstructExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFullConstructExpr([NotNull] SonaParser.FullConstructExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fullConstructExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFullConstructExpr([NotNull] SonaParser.FullConstructExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.numberArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumberArg([NotNull] SonaParser.NumberArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.numberArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumberArg([NotNull] SonaParser.NumberArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.stringArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStringArg([NotNull] SonaParser.StringArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.stringArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStringArg([NotNull] SonaParser.StringArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anyStringArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnyStringArg([NotNull] SonaParser.AnyStringArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anyStringArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnyStringArg([NotNull] SonaParser.AnyStringArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.charArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCharArg([NotNull] SonaParser.CharArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.charArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCharArg([NotNull] SonaParser.CharArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.symbolArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSymbolArg([NotNull] SonaParser.SymbolArgContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.symbolArg"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSymbolArg([NotNull] SonaParser.SymbolArgContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberExpr([NotNull] SonaParser.MemberExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberExpr([NotNull] SonaParser.MemberExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr_Standalone"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberExpr_Standalone([NotNull] SonaParser.MemberExpr_StandaloneContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr_Standalone"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberExpr_Standalone([NotNull] SonaParser.MemberExpr_StandaloneContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr_Prefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberExpr_Prefix([NotNull] SonaParser.MemberExpr_PrefixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr_Prefix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberExpr_Prefix([NotNull] SonaParser.MemberExpr_PrefixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr_Suffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberExpr_Suffix([NotNull] SonaParser.MemberExpr_SuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr_Suffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberExpr_Suffix([NotNull] SonaParser.MemberExpr_SuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.primitiveTypeMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimitiveTypeMemberAccess([NotNull] SonaParser.PrimitiveTypeMemberAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.primitiveTypeMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimitiveTypeMemberAccess([NotNull] SonaParser.PrimitiveTypeMemberAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.altMemberExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAltMemberExpr([NotNull] SonaParser.AltMemberExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.altMemberExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAltMemberExpr([NotNull] SonaParser.AltMemberExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.altMemberExpr_Suffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAltMemberExpr_Suffix([NotNull] SonaParser.AltMemberExpr_SuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.altMemberExpr_Suffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAltMemberExpr_Suffix([NotNull] SonaParser.AltMemberExpr_SuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalMember"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionalMember([NotNull] SonaParser.ConditionalMemberContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalMember"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionalMember([NotNull] SonaParser.ConditionalMemberContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constrainedMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstrainedMemberAccess([NotNull] SonaParser.ConstrainedMemberAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constrainedMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstrainedMemberAccess([NotNull] SonaParser.ConstrainedMemberAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constrainedFunctionAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstrainedFunctionAccess([NotNull] SonaParser.ConstrainedFunctionAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constrainedFunctionAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstrainedFunctionAccess([NotNull] SonaParser.ConstrainedFunctionAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constrainedPropertyAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstrainedPropertyAccess([NotNull] SonaParser.ConstrainedPropertyAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constrainedPropertyAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstrainedPropertyAccess([NotNull] SonaParser.ConstrainedPropertyAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.indexAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIndexAccess([NotNull] SonaParser.IndexAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.indexAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIndexAccess([NotNull] SonaParser.IndexAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberAccess([NotNull] SonaParser.MemberAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberAccess([NotNull] SonaParser.MemberAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.dynamicMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDynamicMemberAccess([NotNull] SonaParser.DynamicMemberAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.dynamicMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDynamicMemberAccess([NotNull] SonaParser.DynamicMemberAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.dynamicExprMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDynamicExprMemberAccess([NotNull] SonaParser.DynamicExprMemberAccessContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.dynamicExprMemberAccess"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDynamicExprMemberAccess([NotNull] SonaParser.DynamicExprMemberAccessContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberNumberConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberNumberConvertExpr([NotNull] SonaParser.MemberNumberConvertExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberNumberConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberNumberConvertExpr([NotNull] SonaParser.MemberNumberConvertExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberCharConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberCharConvertExpr([NotNull] SonaParser.MemberCharConvertExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberCharConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberCharConvertExpr([NotNull] SonaParser.MemberCharConvertExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberConvertExpr([NotNull] SonaParser.MemberConvertExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberConvertExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberConvertExpr([NotNull] SonaParser.MemberConvertExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.convertOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConvertOperator([NotNull] SonaParser.ConvertOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.convertOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConvertOperator([NotNull] SonaParser.ConvertOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionSuffix([NotNull] SonaParser.OptionSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionSuffix([NotNull] SonaParser.OptionSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberTypeConstructExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberTypeConstructExpr([NotNull] SonaParser.MemberTypeConstructExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberTypeConstructExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberTypeConstructExpr([NotNull] SonaParser.MemberTypeConstructExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberNewExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemberNewExpr([NotNull] SonaParser.MemberNewExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberNewExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemberNewExpr([NotNull] SonaParser.MemberNewExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constructArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstructArguments([NotNull] SonaParser.ConstructArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constructArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstructArguments([NotNull] SonaParser.ConstructArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constructCallArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstructCallArgTuple([NotNull] SonaParser.ConstructCallArgTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constructCallArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstructCallArgTuple([NotNull] SonaParser.ConstructCallArgTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fieldAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFieldAssignment([NotNull] SonaParser.FieldAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fieldAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFieldAssignment([NotNull] SonaParser.FieldAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionalFieldAssignmentExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOptionalFieldAssignmentExpr([NotNull] SonaParser.OptionalFieldAssignmentExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionalFieldAssignmentExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOptionalFieldAssignmentExpr([NotNull] SonaParser.OptionalFieldAssignmentExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.callArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCallArguments([NotNull] SonaParser.CallArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.callArguments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCallArguments([NotNull] SonaParser.CallArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleCallArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleCallArgument([NotNull] SonaParser.SimpleCallArgumentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleCallArgument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleCallArgument([NotNull] SonaParser.SimpleCallArgumentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.callArgList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCallArgList([NotNull] SonaParser.CallArgListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.callArgList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCallArgList([NotNull] SonaParser.CallArgListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleCallArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleCallArgTuple([NotNull] SonaParser.SimpleCallArgTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleCallArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleCallArgTuple([NotNull] SonaParser.SimpleCallArgTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.complexCallArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexCallArgTuple([NotNull] SonaParser.ComplexCallArgTupleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.complexCallArgTuple"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexCallArgTuple([NotNull] SonaParser.ComplexCallArgTupleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.recordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRecordConstructor([NotNull] SonaParser.RecordConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.recordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRecordConstructor([NotNull] SonaParser.RecordConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousRecordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousRecordConstructor([NotNull] SonaParser.AnonymousRecordConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousRecordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousRecordConstructor([NotNull] SonaParser.AnonymousRecordConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousClassRecordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousClassRecordConstructor([NotNull] SonaParser.AnonymousClassRecordConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousClassRecordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousClassRecordConstructor([NotNull] SonaParser.AnonymousClassRecordConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousStructRecordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnonymousStructRecordConstructor([NotNull] SonaParser.AnonymousStructRecordConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousStructRecordConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnonymousStructRecordConstructor([NotNull] SonaParser.AnonymousStructRecordConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.recordConstructor_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRecordConstructor_Contents([NotNull] SonaParser.RecordConstructor_ContentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.recordConstructor_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRecordConstructor_Contents([NotNull] SonaParser.RecordConstructor_ContentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.arrayConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayConstructor([NotNull] SonaParser.ArrayConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.arrayConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayConstructor([NotNull] SonaParser.ArrayConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.sequenceConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSequenceConstructor([NotNull] SonaParser.SequenceConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.sequenceConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSequenceConstructor([NotNull] SonaParser.SequenceConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.computationSequenceConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComputationSequenceConstructor([NotNull] SonaParser.ComputationSequenceConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.computationSequenceConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComputationSequenceConstructor([NotNull] SonaParser.ComputationSequenceConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.collectionElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCollectionElement([NotNull] SonaParser.CollectionElementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.collectionElement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCollectionElement([NotNull] SonaParser.CollectionElementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.collectionFieldExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCollectionFieldExpression([NotNull] SonaParser.CollectionFieldExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.collectionFieldExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCollectionFieldExpression([NotNull] SonaParser.CollectionFieldExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTupleConstructor([NotNull] SonaParser.TupleConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTupleConstructor([NotNull] SonaParser.TupleConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.explicitTupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExplicitTupleConstructor([NotNull] SonaParser.ExplicitTupleConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.explicitTupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExplicitTupleConstructor([NotNull] SonaParser.ExplicitTupleConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.classTupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassTupleConstructor([NotNull] SonaParser.ClassTupleConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.classTupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassTupleConstructor([NotNull] SonaParser.ClassTupleConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.structTupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructTupleConstructor([NotNull] SonaParser.StructTupleConstructorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.structTupleConstructor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructTupleConstructor([NotNull] SonaParser.StructTupleConstructorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleTupleContents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleTupleContents([NotNull] SonaParser.SimpleTupleContentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleTupleContents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleTupleContents([NotNull] SonaParser.SimpleTupleContentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.complexTupleContents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexTupleContents([NotNull] SonaParser.ComplexTupleContentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.complexTupleContents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexTupleContents([NotNull] SonaParser.ComplexTupleContentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.spreadExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSpreadExpression([NotNull] SonaParser.SpreadExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.spreadExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSpreadExpression([NotNull] SonaParser.SpreadExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowExpression([NotNull] SonaParser.FollowExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowExpression([NotNull] SonaParser.FollowExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followExpression_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFollowExpression_Contents([NotNull] SonaParser.FollowExpression_ContentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followExpression_Contents"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFollowExpression_Contents([NotNull] SonaParser.FollowExpression_ContentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.spreadFollowExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSpreadFollowExpression([NotNull] SonaParser.SpreadFollowExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.spreadFollowExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSpreadFollowExpression([NotNull] SonaParser.SpreadFollowExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpolatedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpolatedString([NotNull] SonaParser.InterpolatedStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpolatedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpolatedString([NotNull] SonaParser.InterpolatedStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.plainInterpolatedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPlainInterpolatedString([NotNull] SonaParser.PlainInterpolatedStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.plainInterpolatedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPlainInterpolatedString([NotNull] SonaParser.PlainInterpolatedStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.verbatimInterpolatedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVerbatimInterpolatedString([NotNull] SonaParser.VerbatimInterpolatedStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.verbatimInterpolatedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVerbatimInterpolatedString([NotNull] SonaParser.VerbatimInterpolatedStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrComponent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrComponent([NotNull] SonaParser.InterpStrComponentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrComponent"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrComponent([NotNull] SonaParser.InterpStrComponentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrAlignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrAlignment([NotNull] SonaParser.InterpStrAlignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrAlignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrAlignment([NotNull] SonaParser.InterpStrAlignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrGeneralFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrGeneralFormat([NotNull] SonaParser.InterpStrGeneralFormatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrGeneralFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrGeneralFormat([NotNull] SonaParser.InterpStrGeneralFormatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrStandardFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrStandardFormat([NotNull] SonaParser.InterpStrStandardFormatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrStandardFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrStandardFormat([NotNull] SonaParser.InterpStrStandardFormatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrCustomFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrCustomFormat([NotNull] SonaParser.InterpStrCustomFormatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrCustomFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrCustomFormat([NotNull] SonaParser.InterpStrCustomFormatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrNumberFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrNumberFormat([NotNull] SonaParser.InterpStrNumberFormatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrNumberFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrNumberFormat([NotNull] SonaParser.InterpStrNumberFormatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrComponentFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrComponentFormat([NotNull] SonaParser.InterpStrComponentFormatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrComponentFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrComponentFormat([NotNull] SonaParser.InterpStrComponentFormatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInterpStrExpression([NotNull] SonaParser.InterpStrExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInterpStrExpression([NotNull] SonaParser.InterpStrExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFree([NotNull] SonaParser.InlineSourceFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFree([NotNull] SonaParser.InlineSourceFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceReturning([NotNull] SonaParser.InlineSourceReturningContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceReturning"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceReturning([NotNull] SonaParser.InlineSourceReturningContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceTerminating([NotNull] SonaParser.InlineSourceTerminatingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceTerminating"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceTerminating([NotNull] SonaParser.InlineSourceTerminatingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceLanguage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceLanguage([NotNull] SonaParser.InlineSourceLanguageContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceLanguage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceLanguage([NotNull] SonaParser.InlineSourceLanguageContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSharp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSharp([NotNull] SonaParser.InlineSourceFSharpContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSharp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSharp([NotNull] SonaParser.InlineSourceFSharpContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceJavaScript"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceJavaScript([NotNull] SonaParser.InlineSourceJavaScriptContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceJavaScript"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceJavaScript([NotNull] SonaParser.InlineSourceJavaScriptContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSFirstLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSFirstLine([NotNull] SonaParser.InlineSourceFSFirstLineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSFirstLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSFirstLine([NotNull] SonaParser.InlineSourceFSFirstLineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSLine([NotNull] SonaParser.InlineSourceFSLineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSLine([NotNull] SonaParser.InlineSourceFSLineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSNewLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSNewLine([NotNull] SonaParser.InlineSourceFSNewLineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSNewLine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSNewLine([NotNull] SonaParser.InlineSourceFSNewLineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSIndentation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSIndentation([NotNull] SonaParser.InlineSourceFSIndentationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSIndentation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSIndentation([NotNull] SonaParser.InlineSourceFSIndentationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSToken"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSToken([NotNull] SonaParser.InlineSourceFSTokenContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSToken"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSToken([NotNull] SonaParser.InlineSourceFSTokenContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSPart"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSPart([NotNull] SonaParser.InlineSourceFSPartContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSPart"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSPart([NotNull] SonaParser.InlineSourceFSPartContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSDirective"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSDirective([NotNull] SonaParser.InlineSourceFSDirectiveContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSDirective"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSDirective([NotNull] SonaParser.InlineSourceFSDirectiveContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSLineComment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSLineComment([NotNull] SonaParser.InlineSourceFSLineCommentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSLineComment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSLineComment([NotNull] SonaParser.InlineSourceFSLineCommentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSBlockComment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSBlockComment([NotNull] SonaParser.InlineSourceFSBlockCommentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSBlockComment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSBlockComment([NotNull] SonaParser.InlineSourceFSBlockCommentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSLineCutComment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSLineCutComment([NotNull] SonaParser.InlineSourceFSLineCutCommentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSLineCutComment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSLineCutComment([NotNull] SonaParser.InlineSourceFSLineCutCommentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSWhitespace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineSourceFSWhitespace([NotNull] SonaParser.InlineSourceFSWhitespaceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSWhitespace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineSourceFSWhitespace([NotNull] SonaParser.InlineSourceFSWhitespaceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryOperator([NotNull] SonaParser.UnaryOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryOperator([NotNull] SonaParser.UnaryOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.relationalOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationalOperator([NotNull] SonaParser.RelationalOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.relationalOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationalOperator([NotNull] SonaParser.RelationalOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenGTE"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTokenGTE([NotNull] SonaParser.TokenGTEContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenGTE"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTokenGTE([NotNull] SonaParser.TokenGTEContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenLSHIFT"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTokenLSHIFT([NotNull] SonaParser.TokenLSHIFTContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenLSHIFT"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTokenLSHIFT([NotNull] SonaParser.TokenLSHIFTContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenRSHIFT"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTokenRSHIFT([NotNull] SonaParser.TokenRSHIFTContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenRSHIFT"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTokenRSHIFT([NotNull] SonaParser.TokenRSHIFTContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenDOUBLEQUESTION"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTokenDOUBLEQUESTION([NotNull] SonaParser.TokenDOUBLEQUESTIONContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenDOUBLEQUESTION"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTokenDOUBLEQUESTION([NotNull] SonaParser.TokenDOUBLEQUESTIONContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumber([NotNull] SonaParser.NumberContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumber([NotNull] SonaParser.NumberContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.numberToken"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumberToken([NotNull] SonaParser.NumberTokenContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.numberToken"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumberToken([NotNull] SonaParser.NumberTokenContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.char"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterChar([NotNull] SonaParser.CharContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.char"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitChar([NotNull] SonaParser.CharContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.charToken"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCharToken([NotNull] SonaParser.CharTokenContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.charToken"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCharToken([NotNull] SonaParser.CharTokenContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterString([NotNull] SonaParser.StringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.string"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitString([NotNull] SonaParser.StringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnit([NotNull] SonaParser.UnitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnit([NotNull] SonaParser.UnitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorMissingExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterErrorMissingExpression([NotNull] SonaParser.ErrorMissingExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorMissingExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitErrorMissingExpression([NotNull] SonaParser.ErrorMissingExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedFollow"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterErrorUnsupportedFollow([NotNull] SonaParser.ErrorUnsupportedFollowContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedFollow"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitErrorUnsupportedFollow([NotNull] SonaParser.ErrorUnsupportedFollowContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedNumberSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterErrorUnsupportedNumberSuffix([NotNull] SonaParser.ErrorUnsupportedNumberSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedNumberSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitErrorUnsupportedNumberSuffix([NotNull] SonaParser.ErrorUnsupportedNumberSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedEndCharSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterErrorUnsupportedEndCharSuffix([NotNull] SonaParser.ErrorUnsupportedEndCharSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedEndCharSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitErrorUnsupportedEndCharSuffix([NotNull] SonaParser.ErrorUnsupportedEndCharSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedEndStringSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterErrorUnsupportedEndStringSuffix([NotNull] SonaParser.ErrorUnsupportedEndStringSuffixContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedEndStringSuffix"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitErrorUnsupportedEndStringSuffix([NotNull] SonaParser.ErrorUnsupportedEndStringSuffixContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnderscoreReserved"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterErrorUnderscoreReserved([NotNull] SonaParser.ErrorUnderscoreReservedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnderscoreReserved"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitErrorUnderscoreReserved([NotNull] SonaParser.ErrorUnderscoreReservedContext context);
}
} // namespace Sona.Grammar
