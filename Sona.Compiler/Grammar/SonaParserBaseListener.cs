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
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ISonaParserListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class SonaParserBaseListener : ISonaParserListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.chunk"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterChunk([NotNull] SonaParser.ChunkContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.chunk"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitChunk([NotNull] SonaParser.ChunkContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.namespaceSection"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamespaceSection([NotNull] SonaParser.NamespaceSectionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.namespaceSection"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamespaceSection([NotNull] SonaParser.NamespaceSectionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.packageSection"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPackageSection([NotNull] SonaParser.PackageSectionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.packageSection"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPackageSection([NotNull] SonaParser.PackageSectionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.mainBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMainBlock([NotNull] SonaParser.MainBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.mainBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMainBlock([NotNull] SonaParser.MainBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.valueBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterValueBlock([NotNull] SonaParser.ValueBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.valueBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitValueBlock([NotNull] SonaParser.ValueBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.valueTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterValueTrail([NotNull] SonaParser.ValueTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.valueTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitValueTrail([NotNull] SonaParser.ValueTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.freeBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFreeBlock([NotNull] SonaParser.FreeBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.freeBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFreeBlock([NotNull] SonaParser.FreeBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.freeTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFreeTrail([NotNull] SonaParser.FreeTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.freeTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFreeTrail([NotNull] SonaParser.FreeTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOpenBlock([NotNull] SonaParser.OpenBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOpenBlock([NotNull] SonaParser.OpenBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOpenTrail([NotNull] SonaParser.OpenTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOpenTrail([NotNull] SonaParser.OpenTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturningBlock([NotNull] SonaParser.ReturningBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturningBlock([NotNull] SonaParser.ReturningBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturningTrail([NotNull] SonaParser.ReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturningTrail([NotNull] SonaParser.ReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.terminatingBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminatingBlock([NotNull] SonaParser.TerminatingBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.terminatingBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminatingBlock([NotNull] SonaParser.TerminatingBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.terminatingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminatingTrail([NotNull] SonaParser.TerminatingTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.terminatingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminatingTrail([NotNull] SonaParser.TerminatingTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptingBlock([NotNull] SonaParser.InterruptingBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptingBlock([NotNull] SonaParser.InterruptingBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptingTrail([NotNull] SonaParser.InterruptingTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptingTrail([NotNull] SonaParser.InterruptingTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturningCoverBlock([NotNull] SonaParser.ReturningCoverBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturningCoverBlock([NotNull] SonaParser.ReturningCoverBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturningCoverTrail([NotNull] SonaParser.ReturningCoverTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturningCoverTrail([NotNull] SonaParser.ReturningCoverTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalBlock([NotNull] SonaParser.ConditionalBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalBlock([NotNull] SonaParser.ConditionalBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalTrail([NotNull] SonaParser.ConditionalTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalTrail([NotNull] SonaParser.ConditionalTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptibleBlock([NotNull] SonaParser.InterruptibleBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptibleBlock([NotNull] SonaParser.InterruptibleBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptibleTrail([NotNull] SonaParser.InterruptibleTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptibleTrail([NotNull] SonaParser.InterruptibleTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalCoverBlock([NotNull] SonaParser.ConditionalCoverBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalCoverBlock([NotNull] SonaParser.ConditionalCoverBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalCoverTrail([NotNull] SonaParser.ConditionalCoverTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalCoverTrail([NotNull] SonaParser.ConditionalCoverTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptingCoverBlock([NotNull] SonaParser.InterruptingCoverBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptingCoverBlock([NotNull] SonaParser.InterruptingCoverBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptingCoverTrail([NotNull] SonaParser.InterruptingCoverTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptingCoverTrail([NotNull] SonaParser.InterruptingCoverTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptibleCoverBlock([NotNull] SonaParser.InterruptibleCoverBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptibleCoverBlock([NotNull] SonaParser.InterruptibleCoverBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptibleCoverTrail([NotNull] SonaParser.InterruptibleCoverTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleCoverTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptibleCoverTrail([NotNull] SonaParser.InterruptibleCoverTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOpenCoverBlock([NotNull] SonaParser.OpenCoverBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openCoverBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOpenCoverBlock([NotNull] SonaParser.OpenCoverBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openToInterruptibleBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOpenToInterruptibleBlock([NotNull] SonaParser.OpenToInterruptibleBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openToInterruptibleBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOpenToInterruptibleBlock([NotNull] SonaParser.OpenToInterruptibleBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.openToConditionalBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOpenToConditionalBlock([NotNull] SonaParser.OpenToConditionalBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.openToConditionalBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOpenToConditionalBlock([NotNull] SonaParser.OpenToConditionalBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingToInterruptibleBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptingToInterruptibleBlock([NotNull] SonaParser.InterruptingToInterruptibleBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingToInterruptibleBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptingToInterruptibleBlock([NotNull] SonaParser.InterruptingToInterruptibleBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningToConditionalBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturningToConditionalBlock([NotNull] SonaParser.ReturningToConditionalBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningToConditionalBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturningToConditionalBlock([NotNull] SonaParser.ReturningToConditionalBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIgnoredBlock([NotNull] SonaParser.IgnoredBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIgnoredBlock([NotNull] SonaParser.IgnoredBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIgnoredTrail([NotNull] SonaParser.IgnoredTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIgnoredTrail([NotNull] SonaParser.IgnoredTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredEmptyBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIgnoredEmptyBlock([NotNull] SonaParser.IgnoredEmptyBlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredEmptyBlock"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIgnoredEmptyBlock([NotNull] SonaParser.IgnoredEmptyBlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredEmptyTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIgnoredEmptyTrail([NotNull] SonaParser.IgnoredEmptyTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredEmptyTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIgnoredEmptyTrail([NotNull] SonaParser.IgnoredEmptyTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.name"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterName([NotNull] SonaParser.NameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.name"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitName([NotNull] SonaParser.NameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberName([NotNull] SonaParser.MemberNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberName([NotNull] SonaParser.MemberNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.dynamicMemberName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDynamicMemberName([NotNull] SonaParser.DynamicMemberNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.dynamicMemberName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDynamicMemberName([NotNull] SonaParser.DynamicMemberNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.compoundName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompoundName([NotNull] SonaParser.CompoundNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.compoundName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompoundName([NotNull] SonaParser.CompoundNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.compoundNameGeneric"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompoundNameGeneric([NotNull] SonaParser.CompoundNameGenericContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.compoundNameGeneric"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompoundNameGeneric([NotNull] SonaParser.CompoundNameGenericContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typeArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypeArgument([NotNull] SonaParser.TypeArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typeArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypeArgument([NotNull] SonaParser.TypeArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.type"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterType([NotNull] SonaParser.TypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.type"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitType([NotNull] SonaParser.TypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nullableType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNullableType([NotNull] SonaParser.NullableTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nullableType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNullableType([NotNull] SonaParser.NullableTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conjunctionType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConjunctionType([NotNull] SonaParser.ConjunctionTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conjunctionType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConjunctionType([NotNull] SonaParser.ConjunctionTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtomicType([NotNull] SonaParser.AtomicTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtomicType([NotNull] SonaParser.AtomicTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.primitiveType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimitiveType([NotNull] SonaParser.PrimitiveTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.primitiveType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimitiveType([NotNull] SonaParser.PrimitiveTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.functionType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFunctionType([NotNull] SonaParser.FunctionTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.functionType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFunctionType([NotNull] SonaParser.FunctionTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramTypesList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParamTypesList([NotNull] SonaParser.ParamTypesListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramTypesList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParamTypesList([NotNull] SonaParser.ParamTypesListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramTypesTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParamTypesTuple([NotNull] SonaParser.ParamTypesTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramTypesTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParamTypesTuple([NotNull] SonaParser.ParamTypesTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNestedType([NotNull] SonaParser.NestedTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNestedType([NotNull] SonaParser.NestedTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tupleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTupleType([NotNull] SonaParser.TupleTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tupleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTupleType([NotNull] SonaParser.TupleTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.classTupleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterClassTupleType([NotNull] SonaParser.ClassTupleTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.classTupleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitClassTupleType([NotNull] SonaParser.ClassTupleTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.structTupleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStructTupleType([NotNull] SonaParser.StructTupleTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.structTupleType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStructTupleType([NotNull] SonaParser.StructTupleTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousRecordType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousRecordType([NotNull] SonaParser.AnonymousRecordTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousRecordType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousRecordType([NotNull] SonaParser.AnonymousRecordTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousClassRecordType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousClassRecordType([NotNull] SonaParser.AnonymousClassRecordTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousClassRecordType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousClassRecordType([NotNull] SonaParser.AnonymousClassRecordTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousStructRecordType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousStructRecordType([NotNull] SonaParser.AnonymousStructRecordTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousStructRecordType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousStructRecordType([NotNull] SonaParser.AnonymousStructRecordTypeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousRecordMemberDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousRecordMemberDeclaration([NotNull] SonaParser.AnonymousRecordMemberDeclarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousRecordMemberDeclaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousRecordMemberDeclaration([NotNull] SonaParser.AnonymousRecordMemberDeclarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypeSuffix([NotNull] SonaParser.TypeSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypeSuffix([NotNull] SonaParser.TypeSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.arrayTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrayTypeSuffix([NotNull] SonaParser.ArrayTypeSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.arrayTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrayTypeSuffix([NotNull] SonaParser.ArrayTypeSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.multiArrayTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMultiArrayTypeSuffix([NotNull] SonaParser.MultiArrayTypeSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.multiArrayTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMultiArrayTypeSuffix([NotNull] SonaParser.MultiArrayTypeSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptionTypeSuffix([NotNull] SonaParser.OptionTypeSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptionTypeSuffix([NotNull] SonaParser.OptionTypeSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.sequenceTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSequenceTypeSuffix([NotNull] SonaParser.SequenceTypeSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.sequenceTypeSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSequenceTypeSuffix([NotNull] SonaParser.SequenceTypeSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.genericArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGenericArguments([NotNull] SonaParser.GenericArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.genericArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGenericArguments([NotNull] SonaParser.GenericArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.genericArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGenericArgument([NotNull] SonaParser.GenericArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.genericArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGenericArgument([NotNull] SonaParser.GenericArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.measureArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMeasureArgument([NotNull] SonaParser.MeasureArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.measureArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMeasureArgument([NotNull] SonaParser.MeasureArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.measureExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMeasureExpression([NotNull] SonaParser.MeasureExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.measureExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMeasureExpression([NotNull] SonaParser.MeasureExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.measureOperand"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMeasureOperand([NotNull] SonaParser.MeasureOperandContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.measureOperand"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMeasureOperand([NotNull] SonaParser.MeasureOperandContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.literalArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLiteralArgument([NotNull] SonaParser.LiteralArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.literalArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLiteralArgument([NotNull] SonaParser.LiteralArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.localAttrList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLocalAttrList([NotNull] SonaParser.LocalAttrListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.localAttrList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLocalAttrList([NotNull] SonaParser.LocalAttrListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.localAttribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLocalAttribute([NotNull] SonaParser.LocalAttributeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.localAttribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLocalAttribute([NotNull] SonaParser.LocalAttributeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.localAttrTarget"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLocalAttrTarget([NotNull] SonaParser.LocalAttrTargetContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.localAttrTarget"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLocalAttrTarget([NotNull] SonaParser.LocalAttrTargetContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.globalAttrList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGlobalAttrList([NotNull] SonaParser.GlobalAttrListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.globalAttrList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGlobalAttrList([NotNull] SonaParser.GlobalAttrListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.globalAttribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGlobalAttribute([NotNull] SonaParser.GlobalAttributeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.globalAttribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGlobalAttribute([NotNull] SonaParser.GlobalAttributeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.globalAttrTarget"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGlobalAttrTarget([NotNull] SonaParser.GlobalAttrTargetContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.globalAttrTarget"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGlobalAttrTarget([NotNull] SonaParser.GlobalAttrTargetContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.attrGroup"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttrGroup([NotNull] SonaParser.AttrGroupContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.attrGroup"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttrGroup([NotNull] SonaParser.AttrGroupContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.compiledNameAttr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCompiledNameAttr([NotNull] SonaParser.CompiledNameAttrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.compiledNameAttr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCompiledNameAttr([NotNull] SonaParser.CompiledNameAttrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.attrPosArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttrPosArg([NotNull] SonaParser.AttrPosArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.attrPosArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttrPosArg([NotNull] SonaParser.AttrPosArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.attrNamedArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttrNamedArg([NotNull] SonaParser.AttrNamedArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.attrNamedArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttrNamedArg([NotNull] SonaParser.AttrNamedArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStatement([NotNull] SonaParser.StatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.statement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStatement([NotNull] SonaParser.StatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.closingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterClosingStatement([NotNull] SonaParser.ClosingStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.closingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitClosingStatement([NotNull] SonaParser.ClosingStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.echoStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEchoStatement([NotNull] SonaParser.EchoStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.echoStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEchoStatement([NotNull] SonaParser.EchoStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.implicitReturnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterImplicitReturnStatement([NotNull] SonaParser.ImplicitReturnStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.implicitReturnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitImplicitReturnStatement([NotNull] SonaParser.ImplicitReturnStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturnStatement([NotNull] SonaParser.ReturnStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturnStatement([NotNull] SonaParser.ReturnStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returnOptionStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturnOptionStatement([NotNull] SonaParser.ReturnOptionStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returnOptionStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturnOptionStatement([NotNull] SonaParser.ReturnOptionStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returnFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturnFollowStatement([NotNull] SonaParser.ReturnFollowStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returnFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturnFollowStatement([NotNull] SonaParser.ReturnFollowStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldStatement([NotNull] SonaParser.YieldStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldStatement([NotNull] SonaParser.YieldStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldFollowStatement([NotNull] SonaParser.YieldFollowStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldFollowStatement([NotNull] SonaParser.YieldFollowStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldEachStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldEachStatement([NotNull] SonaParser.YieldEachStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldEachStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldEachStatement([NotNull] SonaParser.YieldEachStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldBreakStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldBreakStatement([NotNull] SonaParser.YieldBreakStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldBreakStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldBreakStatement([NotNull] SonaParser.YieldBreakStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldReturnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldReturnStatement([NotNull] SonaParser.YieldReturnStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldReturnStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldReturnStatement([NotNull] SonaParser.YieldReturnStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldReturnFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldReturnFollowStatement([NotNull] SonaParser.YieldReturnFollowStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldReturnFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldReturnFollowStatement([NotNull] SonaParser.YieldReturnFollowStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.breakStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBreakStatement([NotNull] SonaParser.BreakStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.breakStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBreakStatement([NotNull] SonaParser.BreakStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.continueStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterContinueStatement([NotNull] SonaParser.ContinueStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.continueStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitContinueStatement([NotNull] SonaParser.ContinueStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.continueFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterContinueFollowStatement([NotNull] SonaParser.ContinueFollowStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.continueFollowStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitContinueFollowStatement([NotNull] SonaParser.ContinueFollowStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.throwStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterThrowStatement([NotNull] SonaParser.ThrowStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.throwStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitThrowStatement([NotNull] SonaParser.ThrowStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.withDefaultArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWithDefaultArgument([NotNull] SonaParser.WithDefaultArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.withDefaultArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWithDefaultArgument([NotNull] SonaParser.WithDefaultArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.withDefaultSequenceArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWithDefaultSequenceArgument([NotNull] SonaParser.WithDefaultSequenceArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.withDefaultSequenceArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWithDefaultSequenceArgument([NotNull] SonaParser.WithDefaultSequenceArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.with_Argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWith_Argument([NotNull] SonaParser.With_ArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.with_Argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWith_Argument([NotNull] SonaParser.With_ArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.withStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWithStatement([NotNull] SonaParser.WithStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.withStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWithStatement([NotNull] SonaParser.WithStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowStatement([NotNull] SonaParser.FollowStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowStatement([NotNull] SonaParser.FollowStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithTrailing"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowWithTrailing([NotNull] SonaParser.FollowWithTrailingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithTrailing"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowWithTrailing([NotNull] SonaParser.FollowWithTrailingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowWithTerminating([NotNull] SonaParser.FollowWithTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowWithTerminating([NotNull] SonaParser.FollowWithTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowWithInterrupting([NotNull] SonaParser.FollowWithInterruptingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowWithInterrupting([NotNull] SonaParser.FollowWithInterruptingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowWithInterruptible([NotNull] SonaParser.FollowWithInterruptibleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowWithInterruptible([NotNull] SonaParser.FollowWithInterruptibleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowWithReturning([NotNull] SonaParser.FollowWithReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowWithReturning([NotNull] SonaParser.FollowWithReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followWithConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowWithConditional([NotNull] SonaParser.FollowWithConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followWithConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowWithConditional([NotNull] SonaParser.FollowWithConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithTrailing"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldWithTrailing([NotNull] SonaParser.YieldWithTrailingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithTrailing"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldWithTrailing([NotNull] SonaParser.YieldWithTrailingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldWithTerminating([NotNull] SonaParser.YieldWithTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldWithTerminating([NotNull] SonaParser.YieldWithTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldWithInterrupting([NotNull] SonaParser.YieldWithInterruptingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldWithInterrupting([NotNull] SonaParser.YieldWithInterruptingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldWithInterruptible([NotNull] SonaParser.YieldWithInterruptibleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldWithInterruptible([NotNull] SonaParser.YieldWithInterruptibleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldWithReturning([NotNull] SonaParser.YieldWithReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldWithReturning([NotNull] SonaParser.YieldWithReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.yieldWithConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterYieldWithConditional([NotNull] SonaParser.YieldWithConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.yieldWithConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitYieldWithConditional([NotNull] SonaParser.YieldWithConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.trailingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTrailingStatement([NotNull] SonaParser.TrailingStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.trailingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTrailingStatement([NotNull] SonaParser.TrailingStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.returningStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterReturningStatement([NotNull] SonaParser.ReturningStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.returningStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitReturningStatement([NotNull] SonaParser.ReturningStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptingStatement([NotNull] SonaParser.InterruptingStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptingStatement([NotNull] SonaParser.InterruptingStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interruptibleStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterruptibleStatement([NotNull] SonaParser.InterruptibleStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interruptibleStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterruptibleStatement([NotNull] SonaParser.InterruptibleStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalStatement([NotNull] SonaParser.ConditionalStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalStatement([NotNull] SonaParser.ConditionalStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.terminatingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminatingStatement([NotNull] SonaParser.TerminatingStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.terminatingStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminatingStatement([NotNull] SonaParser.TerminatingStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.expressionStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpressionStatement([NotNull] SonaParser.ExpressionStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.expressionStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpressionStatement([NotNull] SonaParser.ExpressionStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ignoredTrail_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIgnoredTrail_Group([NotNull] SonaParser.IgnoredTrail_GroupContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ignoredTrail_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIgnoredTrail_Group([NotNull] SonaParser.IgnoredTrail_GroupContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementFree([NotNull] SonaParser.DoStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementFree([NotNull] SonaParser.DoStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementTerminating([NotNull] SonaParser.DoStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementTerminating([NotNull] SonaParser.DoStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementInterrupting([NotNull] SonaParser.DoStatementInterruptingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementInterrupting([NotNull] SonaParser.DoStatementInterruptingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementInterruptingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementInterruptingTrail([NotNull] SonaParser.DoStatementInterruptingTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementInterruptingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementInterruptingTrail([NotNull] SonaParser.DoStatementInterruptingTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementInterruptible([NotNull] SonaParser.DoStatementInterruptibleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementInterruptible([NotNull] SonaParser.DoStatementInterruptibleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementReturning([NotNull] SonaParser.DoStatementReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementReturning([NotNull] SonaParser.DoStatementReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementReturningTrail([NotNull] SonaParser.DoStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementReturningTrail([NotNull] SonaParser.DoStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.doStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDoStatementConditional([NotNull] SonaParser.DoStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.doStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDoStatementConditional([NotNull] SonaParser.DoStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.if_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIf_Group([NotNull] SonaParser.If_GroupContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.if_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIf_Group([NotNull] SonaParser.If_GroupContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.elseif_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterElseif_Group([NotNull] SonaParser.Elseif_GroupContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.elseif_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitElseif_Group([NotNull] SonaParser.Elseif_GroupContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.if"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIf([NotNull] SonaParser.IfContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.if"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIf([NotNull] SonaParser.IfContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.elseif"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterElseif([NotNull] SonaParser.ElseifContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.elseif"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitElseif([NotNull] SonaParser.ElseifContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseIf"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCaseIf([NotNull] SonaParser.CaseIfContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseIf"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCaseIf([NotNull] SonaParser.CaseIfContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseElseif"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCaseElseif([NotNull] SonaParser.CaseElseifContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseElseif"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCaseElseif([NotNull] SonaParser.CaseElseifContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.else"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterElse([NotNull] SonaParser.ElseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.else"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitElse([NotNull] SonaParser.ElseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementFree([NotNull] SonaParser.IfStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementFree([NotNull] SonaParser.IfStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementTerminating([NotNull] SonaParser.IfStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementTerminating([NotNull] SonaParser.IfStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementInterrupting([NotNull] SonaParser.IfStatementInterruptingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementInterrupting"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementInterrupting([NotNull] SonaParser.IfStatementInterruptingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementInterruptingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementInterruptingTrail([NotNull] SonaParser.IfStatementInterruptingTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementInterruptingTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementInterruptingTrail([NotNull] SonaParser.IfStatementInterruptingTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementInterruptible([NotNull] SonaParser.IfStatementInterruptibleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementInterruptible"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementInterruptible([NotNull] SonaParser.IfStatementInterruptibleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementReturning([NotNull] SonaParser.IfStatementReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementReturning([NotNull] SonaParser.IfStatementReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementReturningTrailFromElse"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementReturningTrailFromElse([NotNull] SonaParser.IfStatementReturningTrailFromElseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementReturningTrailFromElse"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementReturningTrailFromElse([NotNull] SonaParser.IfStatementReturningTrailFromElseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementReturningTrail([NotNull] SonaParser.IfStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementReturningTrail([NotNull] SonaParser.IfStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.ifStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfStatementConditional([NotNull] SonaParser.IfStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.ifStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfStatementConditional([NotNull] SonaParser.IfStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.while"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhile([NotNull] SonaParser.WhileContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.while"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhile([NotNull] SonaParser.WhileContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseWhile"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCaseWhile([NotNull] SonaParser.CaseWhileContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseWhile"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCaseWhile([NotNull] SonaParser.CaseWhileContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileTrue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileTrue([NotNull] SonaParser.WhileTrueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileTrue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileTrue([NotNull] SonaParser.WhileTrueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileStatementFree([NotNull] SonaParser.WhileStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileStatementFree([NotNull] SonaParser.WhileStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileStatementFreeInterrupted([NotNull] SonaParser.WhileStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileStatementFreeInterrupted([NotNull] SonaParser.WhileStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileStatementTerminating([NotNull] SonaParser.WhileStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileStatementTerminating([NotNull] SonaParser.WhileStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileStatementReturningTrail([NotNull] SonaParser.WhileStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileStatementReturningTrail([NotNull] SonaParser.WhileStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whileStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhileStatementConditional([NotNull] SonaParser.WhileStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whileStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhileStatementConditional([NotNull] SonaParser.WhileStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.until"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUntil([NotNull] SonaParser.UntilContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.until"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUntil([NotNull] SonaParser.UntilContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeatStatementFree([NotNull] SonaParser.RepeatStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeatStatementFree([NotNull] SonaParser.RepeatStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeatStatementFreeInterrupted([NotNull] SonaParser.RepeatStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeatStatementFreeInterrupted([NotNull] SonaParser.RepeatStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeatStatementTerminating([NotNull] SonaParser.RepeatStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeatStatementTerminating([NotNull] SonaParser.RepeatStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeatStatementReturningTrail([NotNull] SonaParser.RepeatStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeatStatementReturningTrail([NotNull] SonaParser.RepeatStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.repeatStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRepeatStatementConditional([NotNull] SonaParser.RepeatStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.repeatStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRepeatStatementConditional([NotNull] SonaParser.RepeatStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.for"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFor([NotNull] SonaParser.ForContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.for"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFor([NotNull] SonaParser.ForContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forSimple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForSimple([NotNull] SonaParser.ForSimpleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forSimple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForSimple([NotNull] SonaParser.ForSimpleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forSimpleStep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForSimpleStep([NotNull] SonaParser.ForSimpleStepContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forSimpleStep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForSimpleStep([NotNull] SonaParser.ForSimpleStepContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRange"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForRange([NotNull] SonaParser.ForRangeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRange"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForRange([NotNull] SonaParser.ForRangeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRangeStep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForRangeStep([NotNull] SonaParser.ForRangeStepContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRangeStep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForRangeStep([NotNull] SonaParser.ForRangeStepContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRangePrimitiveStep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForRangePrimitiveStep([NotNull] SonaParser.ForRangePrimitiveStepContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRangePrimitiveStep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForRangePrimitiveStep([NotNull] SonaParser.ForRangePrimitiveStepContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forRangeExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForRangeExpression([NotNull] SonaParser.ForRangeExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forRangeExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForRangeExpression([NotNull] SonaParser.ForRangeExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForStatementFree([NotNull] SonaParser.ForStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForStatementFree([NotNull] SonaParser.ForStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForStatementFreeInterrupted([NotNull] SonaParser.ForStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForStatementFreeInterrupted([NotNull] SonaParser.ForStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForStatementReturningTrail([NotNull] SonaParser.ForStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForStatementReturningTrail([NotNull] SonaParser.ForStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.forStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForStatementConditional([NotNull] SonaParser.ForStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.forStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForStatementConditional([NotNull] SonaParser.ForStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switch"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitch([NotNull] SonaParser.SwitchContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switch"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitch([NotNull] SonaParser.SwitchContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.case"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCase([NotNull] SonaParser.CaseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.case"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCase([NotNull] SonaParser.CaseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.whenClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhenClause([NotNull] SonaParser.WhenClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.whenClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhenClause([NotNull] SonaParser.WhenClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementFree([NotNull] SonaParser.SwitchStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementFree([NotNull] SonaParser.SwitchStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementFreeInterrupted([NotNull] SonaParser.SwitchStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementFreeInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementFreeInterrupted([NotNull] SonaParser.SwitchStatementFreeInterruptedContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementTerminating([NotNull] SonaParser.SwitchStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementTerminating([NotNull] SonaParser.SwitchStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementTerminatingInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementTerminatingInterrupted([NotNull] SonaParser.SwitchStatementTerminatingInterruptedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementTerminatingInterrupted"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementTerminatingInterrupted([NotNull] SonaParser.SwitchStatementTerminatingInterruptedContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementReturning([NotNull] SonaParser.SwitchStatementReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementReturning([NotNull] SonaParser.SwitchStatementReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementReturningTrail([NotNull] SonaParser.SwitchStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementReturningTrail([NotNull] SonaParser.SwitchStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.switchStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSwitchStatementConditional([NotNull] SonaParser.SwitchStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.switchStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSwitchStatementConditional([NotNull] SonaParser.SwitchStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.try"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTry([NotNull] SonaParser.TryContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.try"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTry([NotNull] SonaParser.TryContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.finallyBranch"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFinallyBranch([NotNull] SonaParser.FinallyBranchContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.finallyBranch"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFinallyBranch([NotNull] SonaParser.FinallyBranchContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.catch_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCatch_Group([NotNull] SonaParser.Catch_GroupContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.catch_Group"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCatch_Group([NotNull] SonaParser.Catch_GroupContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.catchAsCase"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCatchAsCase([NotNull] SonaParser.CatchAsCaseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.catchAsCase"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCatchAsCase([NotNull] SonaParser.CatchAsCaseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.catchCase"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCatchCase([NotNull] SonaParser.CatchCaseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.catchCase"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCatchCase([NotNull] SonaParser.CatchCaseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchStatementFree([NotNull] SonaParser.TryCatchStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchStatementFree([NotNull] SonaParser.TryCatchStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryFinallyStatementFree([NotNull] SonaParser.TryFinallyStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryFinallyStatementFree([NotNull] SonaParser.TryFinallyStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchFinallyStatementFree([NotNull] SonaParser.TryCatchFinallyStatementFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchFinallyStatementFree([NotNull] SonaParser.TryCatchFinallyStatementFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchStatementTerminating([NotNull] SonaParser.TryCatchStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchStatementTerminating([NotNull] SonaParser.TryCatchStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchStatementConditional([NotNull] SonaParser.TryCatchStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchStatementConditional([NotNull] SonaParser.TryCatchStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchStatementReturning([NotNull] SonaParser.TryCatchStatementReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchStatementReturning([NotNull] SonaParser.TryCatchStatementReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchStatementReturningTrail([NotNull] SonaParser.TryCatchStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchStatementReturningTrail([NotNull] SonaParser.TryCatchStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryFinallyStatementTerminating([NotNull] SonaParser.TryFinallyStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryFinallyStatementTerminating([NotNull] SonaParser.TryFinallyStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryFinallyStatementConditional([NotNull] SonaParser.TryFinallyStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryFinallyStatementConditional([NotNull] SonaParser.TryFinallyStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryFinallyStatementReturning([NotNull] SonaParser.TryFinallyStatementReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryFinallyStatementReturning([NotNull] SonaParser.TryFinallyStatementReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryFinallyStatementReturningTrail([NotNull] SonaParser.TryFinallyStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryFinallyStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryFinallyStatementReturningTrail([NotNull] SonaParser.TryFinallyStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchFinallyStatementTerminating([NotNull] SonaParser.TryCatchFinallyStatementTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchFinallyStatementTerminating([NotNull] SonaParser.TryCatchFinallyStatementTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchFinallyStatementConditional([NotNull] SonaParser.TryCatchFinallyStatementConditionalContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementConditional"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchFinallyStatementConditional([NotNull] SonaParser.TryCatchFinallyStatementConditionalContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchFinallyStatementReturning([NotNull] SonaParser.TryCatchFinallyStatementReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchFinallyStatementReturning([NotNull] SonaParser.TryCatchFinallyStatementReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTryCatchFinallyStatementReturningTrail([NotNull] SonaParser.TryCatchFinallyStatementReturningTrailContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tryCatchFinallyStatementReturningTrail"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTryCatchFinallyStatementReturningTrail([NotNull] SonaParser.TryCatchFinallyStatementReturningTrailContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.topLevelStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTopLevelStatement([NotNull] SonaParser.TopLevelStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.topLevelStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTopLevelStatement([NotNull] SonaParser.TopLevelStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.importStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterImportStatement([NotNull] SonaParser.ImportStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.importStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitImportStatement([NotNull] SonaParser.ImportStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.importTypeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterImportTypeStatement([NotNull] SonaParser.ImportTypeStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.importTypeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitImportTypeStatement([NotNull] SonaParser.ImportTypeStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.symbolContentsArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSymbolContentsArg([NotNull] SonaParser.SymbolContentsArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.symbolContentsArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSymbolContentsArg([NotNull] SonaParser.SymbolContentsArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.importFileStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterImportFileStatement([NotNull] SonaParser.ImportFileStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.importFileStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitImportFileStatement([NotNull] SonaParser.ImportFileStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.includeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIncludeStatement([NotNull] SonaParser.IncludeStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.includeStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIncludeStatement([NotNull] SonaParser.IncludeStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.requireStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRequireStatement([NotNull] SonaParser.RequireStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.requireStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRequireStatement([NotNull] SonaParser.RequireStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.packageStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPackageStatement([NotNull] SonaParser.PackageStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.packageStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPackageStatement([NotNull] SonaParser.PackageStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleVariableDecl([NotNull] SonaParser.SimpleVariableDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleVariableDecl([NotNull] SonaParser.SimpleVariableDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowVariableDecl([NotNull] SonaParser.FollowVariableDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowVariableDecl([NotNull] SonaParser.FollowVariableDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.multiVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMultiVariableDecl([NotNull] SonaParser.MultiVariableDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.multiVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMultiVariableDecl([NotNull] SonaParser.MultiVariableDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.variableDecl_Prefix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariableDecl_Prefix([NotNull] SonaParser.VariableDecl_PrefixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.variableDecl_Prefix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariableDecl_Prefix([NotNull] SonaParser.VariableDecl_PrefixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.lazyVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLazyVariableDecl([NotNull] SonaParser.LazyVariableDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.lazyVariableDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLazyVariableDecl([NotNull] SonaParser.LazyVariableDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.multiFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMultiFuncDecl([NotNull] SonaParser.MultiFuncDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.multiFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMultiFuncDecl([NotNull] SonaParser.MultiFuncDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.funcDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFuncDecl([NotNull] SonaParser.FuncDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.funcDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFuncDecl([NotNull] SonaParser.FuncDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineFuncDecl([NotNull] SonaParser.InlineFuncDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineFuncDecl([NotNull] SonaParser.InlineFuncDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCaseFuncDecl([NotNull] SonaParser.CaseFuncDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCaseFuncDecl([NotNull] SonaParser.CaseFuncDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineCaseFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineCaseFuncDecl([NotNull] SonaParser.InlineCaseFuncDeclContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineCaseFuncDecl"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineCaseFuncDecl([NotNull] SonaParser.InlineCaseFuncDeclContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseFuncName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCaseFuncName([NotNull] SonaParser.CaseFuncNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseFuncName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCaseFuncName([NotNull] SonaParser.CaseFuncNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.funcBody"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFuncBody([NotNull] SonaParser.FuncBodyContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.funcBody"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFuncBody([NotNull] SonaParser.FuncBodyContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParamList([NotNull] SonaParser.ParamListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParamList([NotNull] SonaParser.ParamListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.paramTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParamTuple([NotNull] SonaParser.ParamTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.paramTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParamTuple([NotNull] SonaParser.ParamTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDeclaration([NotNull] SonaParser.DeclarationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.declaration"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDeclaration([NotNull] SonaParser.DeclarationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionalName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptionalName([NotNull] SonaParser.OptionalNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionalName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptionalName([NotNull] SonaParser.OptionalNameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberOrAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberOrAssignment([NotNull] SonaParser.MemberOrAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberOrAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberOrAssignment([NotNull] SonaParser.MemberOrAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.assignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssignment([NotNull] SonaParser.AssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.assignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssignment([NotNull] SonaParser.AssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberDiscard"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberDiscard([NotNull] SonaParser.MemberDiscardContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberDiscard"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberDiscard([NotNull] SonaParser.MemberDiscardContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followAssignmentStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowAssignmentStatement([NotNull] SonaParser.FollowAssignmentStatementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followAssignmentStatement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowAssignmentStatement([NotNull] SonaParser.FollowAssignmentStatementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.patternArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPatternArgument([NotNull] SonaParser.PatternArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.patternArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPatternArgument([NotNull] SonaParser.PatternArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.pattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPattern([NotNull] SonaParser.PatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.pattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPattern([NotNull] SonaParser.PatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicPattern([NotNull] SonaParser.LogicPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicPattern([NotNull] SonaParser.LogicPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern_AndPrefix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicPattern_AndPrefix([NotNull] SonaParser.LogicPattern_AndPrefixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern_AndPrefix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicPattern_AndPrefix([NotNull] SonaParser.LogicPattern_AndPrefixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern_AndSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicPattern_AndSuffix([NotNull] SonaParser.LogicPattern_AndSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern_AndSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicPattern_AndSuffix([NotNull] SonaParser.LogicPattern_AndSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicPattern_Argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicPattern_Argument([NotNull] SonaParser.LogicPattern_ArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicPattern_Argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicPattern_Argument([NotNull] SonaParser.LogicPattern_ArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryLogicPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryLogicPattern([NotNull] SonaParser.UnaryLogicPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryLogicPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryLogicPattern([NotNull] SonaParser.UnaryLogicPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryPattern([NotNull] SonaParser.UnaryPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryPattern([NotNull] SonaParser.UnaryPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypePattern([NotNull] SonaParser.TypePatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypePattern([NotNull] SonaParser.TypePatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePattern_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypePattern_Contents([NotNull] SonaParser.TypePattern_ContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePattern_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypePattern_Contents([NotNull] SonaParser.TypePattern_ContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePatternExplicit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypePatternExplicit([NotNull] SonaParser.TypePatternExplicitContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePatternExplicit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypePatternExplicit([NotNull] SonaParser.TypePatternExplicitContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.typePatternImplicit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypePatternImplicit([NotNull] SonaParser.TypePatternImplicitContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.typePatternImplicit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypePatternImplicit([NotNull] SonaParser.TypePatternImplicitContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.notNullPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNotNullPattern([NotNull] SonaParser.NotNullPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.notNullPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNotNullPattern([NotNull] SonaParser.NotNullPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicNotNullPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtomicNotNullPattern([NotNull] SonaParser.AtomicNotNullPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicNotNullPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtomicNotNullPattern([NotNull] SonaParser.AtomicNotNullPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nullPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNullPattern([NotNull] SonaParser.NullPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nullPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNullPattern([NotNull] SonaParser.NullPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nullArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNullArg([NotNull] SonaParser.NullArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nullArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNullArg([NotNull] SonaParser.NullArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.annotationPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnnotationPattern([NotNull] SonaParser.AnnotationPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.annotationPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnnotationPattern([NotNull] SonaParser.AnnotationPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtomicPattern([NotNull] SonaParser.AtomicPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtomicPattern([NotNull] SonaParser.AtomicPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNestedPattern([NotNull] SonaParser.NestedPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNestedPattern([NotNull] SonaParser.NestedPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.somePattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSomePattern([NotNull] SonaParser.SomePatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.somePattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSomePattern([NotNull] SonaParser.SomePatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleNamedPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleNamedPattern([NotNull] SonaParser.SimpleNamedPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleNamedPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleNamedPattern([NotNull] SonaParser.SimpleNamedPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.namedPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamedPattern([NotNull] SonaParser.NamedPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.namedPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamedPattern([NotNull] SonaParser.NamedPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberPattern([NotNull] SonaParser.MemberPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberPattern([NotNull] SonaParser.MemberPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.relationalPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRelationalPattern([NotNull] SonaParser.RelationalPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.relationalPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRelationalPattern([NotNull] SonaParser.RelationalPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.regexPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRegexPattern([NotNull] SonaParser.RegexPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.regexPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRegexPattern([NotNull] SonaParser.RegexPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.regexGroupStart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRegexGroupStart([NotNull] SonaParser.RegexGroupStartContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.regexGroupStart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRegexGroupStart([NotNull] SonaParser.RegexGroupStartContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.patternArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPatternArguments([NotNull] SonaParser.PatternArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.patternArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPatternArguments([NotNull] SonaParser.PatternArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simplePatternArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimplePatternArgument([NotNull] SonaParser.SimplePatternArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simplePatternArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimplePatternArgument([NotNull] SonaParser.SimplePatternArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.emptyPatternArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEmptyPatternArgTuple([NotNull] SonaParser.EmptyPatternArgTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.emptyPatternArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEmptyPatternArgTuple([NotNull] SonaParser.EmptyPatternArgTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.lastPatternArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLastPatternArgTuple([NotNull] SonaParser.LastPatternArgTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.lastPatternArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLastPatternArgTuple([NotNull] SonaParser.LastPatternArgTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.patternArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPatternArgTuple([NotNull] SonaParser.PatternArgTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.patternArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPatternArgTuple([NotNull] SonaParser.PatternArgTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.basicConstructPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBasicConstructPattern([NotNull] SonaParser.BasicConstructPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.basicConstructPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBasicConstructPattern([NotNull] SonaParser.BasicConstructPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fullConstructPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFullConstructPattern([NotNull] SonaParser.FullConstructPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fullConstructPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFullConstructPattern([NotNull] SonaParser.FullConstructPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.emptyFieldAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEmptyFieldAssignment([NotNull] SonaParser.EmptyFieldAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.emptyFieldAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEmptyFieldAssignment([NotNull] SonaParser.EmptyFieldAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fieldRelation"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldRelation([NotNull] SonaParser.FieldRelationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fieldRelation"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldRelation([NotNull] SonaParser.FieldRelationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.recordConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRecordConstructorPattern([NotNull] SonaParser.RecordConstructorPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.recordConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRecordConstructorPattern([NotNull] SonaParser.RecordConstructorPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.arrayConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrayConstructorPattern([NotNull] SonaParser.ArrayConstructorPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.arrayConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrayConstructorPattern([NotNull] SonaParser.ArrayConstructorPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTupleConstructorPattern([NotNull] SonaParser.TupleConstructorPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTupleConstructorPattern([NotNull] SonaParser.TupleConstructorPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.explicitTupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExplicitTupleConstructorPattern([NotNull] SonaParser.ExplicitTupleConstructorPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.explicitTupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExplicitTupleConstructorPattern([NotNull] SonaParser.ExplicitTupleConstructorPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.classTupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterClassTupleConstructorPattern([NotNull] SonaParser.ClassTupleConstructorPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.classTupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitClassTupleConstructorPattern([NotNull] SonaParser.ClassTupleConstructorPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.structTupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStructTupleConstructorPattern([NotNull] SonaParser.StructTupleConstructorPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.structTupleConstructorPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStructTupleConstructorPattern([NotNull] SonaParser.StructTupleConstructorPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tuplePattern_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTuplePattern_Contents([NotNull] SonaParser.TuplePattern_ContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tuplePattern_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTuplePattern_Contents([NotNull] SonaParser.TuplePattern_ContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberTestPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberTestPattern([NotNull] SonaParser.MemberTestPatternContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberTestPattern"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberTestPattern([NotNull] SonaParser.MemberTestPatternContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] SonaParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] SonaParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicLogicExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtomicLogicExpr([NotNull] SonaParser.AtomicLogicExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicLogicExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtomicLogicExpr([NotNull] SonaParser.AtomicLogicExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.logicExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicExpr([NotNull] SonaParser.LogicExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.logicExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicExpr([NotNull] SonaParser.LogicExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineIfExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineIfExpr([NotNull] SonaParser.InlineIfExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineIfExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineIfExpr([NotNull] SonaParser.InlineIfExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.booleanExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBooleanExpr([NotNull] SonaParser.BooleanExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.booleanExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBooleanExpr([NotNull] SonaParser.BooleanExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.relationalExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRelationalExpr([NotNull] SonaParser.RelationalExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.relationalExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRelationalExpr([NotNull] SonaParser.RelationalExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.coalesceExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCoalesceExpr([NotNull] SonaParser.CoalesceExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.coalesceExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCoalesceExpr([NotNull] SonaParser.CoalesceExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.concatExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConcatExpr([NotNull] SonaParser.ConcatExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.concatExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConcatExpr([NotNull] SonaParser.ConcatExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.concatExpr_Inner"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConcatExpr_Inner([NotNull] SonaParser.ConcatExpr_InnerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.concatExpr_Inner"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConcatExpr_Inner([NotNull] SonaParser.ConcatExpr_InnerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitOrExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBitOrExpr([NotNull] SonaParser.BitOrExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitOrExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBitOrExpr([NotNull] SonaParser.BitOrExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitXorExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBitXorExpr([NotNull] SonaParser.BitXorExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitXorExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBitXorExpr([NotNull] SonaParser.BitXorExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitAndExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBitAndExpr([NotNull] SonaParser.BitAndExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitAndExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBitAndExpr([NotNull] SonaParser.BitAndExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.bitShiftExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBitShiftExpr([NotNull] SonaParser.BitShiftExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.bitShiftExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBitShiftExpr([NotNull] SonaParser.BitShiftExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.innerExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInnerExpr([NotNull] SonaParser.InnerExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.innerExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInnerExpr([NotNull] SonaParser.InnerExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.annotationExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnnotationExpr([NotNull] SonaParser.AnnotationExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.annotationExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnnotationExpr([NotNull] SonaParser.AnnotationExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryExpr([NotNull] SonaParser.UnaryExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryExpr([NotNull] SonaParser.UnaryExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.atomicExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAtomicExpr([NotNull] SonaParser.AtomicExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.atomicExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAtomicExpr([NotNull] SonaParser.AtomicExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.hashExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterHashExpr([NotNull] SonaParser.HashExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.hashExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitHashExpr([NotNull] SonaParser.HashExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.notExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNotExpr([NotNull] SonaParser.NotExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.notExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNotExpr([NotNull] SonaParser.NotExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryNumberConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryNumberConvertExpr([NotNull] SonaParser.UnaryNumberConvertExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryNumberConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryNumberConvertExpr([NotNull] SonaParser.UnaryNumberConvertExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryCharConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryCharConvertExpr([NotNull] SonaParser.UnaryCharConvertExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryCharConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryCharConvertExpr([NotNull] SonaParser.UnaryCharConvertExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryConvertExpr([NotNull] SonaParser.UnaryConvertExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryConvertExpr([NotNull] SonaParser.UnaryConvertExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleExpr([NotNull] SonaParser.SimpleExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleExpr([NotNull] SonaParser.SimpleExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNestedExpr([NotNull] SonaParser.NestedExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNestedExpr([NotNull] SonaParser.NestedExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.nestedAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNestedAssignment([NotNull] SonaParser.NestedAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.nestedAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNestedAssignment([NotNull] SonaParser.NestedAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.primitiveExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimitiveExpr([NotNull] SonaParser.PrimitiveExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.primitiveExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimitiveExpr([NotNull] SonaParser.PrimitiveExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.namedValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamedValue([NotNull] SonaParser.NamedValueContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.namedValue"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamedValue([NotNull] SonaParser.NamedValueContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.funcExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFuncExpr([NotNull] SonaParser.FuncExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.funcExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFuncExpr([NotNull] SonaParser.FuncExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.caseFuncRefExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCaseFuncRefExpr([NotNull] SonaParser.CaseFuncRefExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.caseFuncRefExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCaseFuncRefExpr([NotNull] SonaParser.CaseFuncRefExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineExpr([NotNull] SonaParser.InlineExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineExpr([NotNull] SonaParser.InlineExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.basicConstructExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBasicConstructExpr([NotNull] SonaParser.BasicConstructExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.basicConstructExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBasicConstructExpr([NotNull] SonaParser.BasicConstructExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fullConstructExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFullConstructExpr([NotNull] SonaParser.FullConstructExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fullConstructExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFullConstructExpr([NotNull] SonaParser.FullConstructExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.numberArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumberArg([NotNull] SonaParser.NumberArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.numberArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumberArg([NotNull] SonaParser.NumberArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.stringArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStringArg([NotNull] SonaParser.StringArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.stringArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStringArg([NotNull] SonaParser.StringArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anyStringArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnyStringArg([NotNull] SonaParser.AnyStringArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anyStringArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnyStringArg([NotNull] SonaParser.AnyStringArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.charArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCharArg([NotNull] SonaParser.CharArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.charArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCharArg([NotNull] SonaParser.CharArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.symbolArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSymbolArg([NotNull] SonaParser.SymbolArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.symbolArg"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSymbolArg([NotNull] SonaParser.SymbolArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberExpr([NotNull] SonaParser.MemberExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberExpr([NotNull] SonaParser.MemberExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr_Standalone"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberExpr_Standalone([NotNull] SonaParser.MemberExpr_StandaloneContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr_Standalone"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberExpr_Standalone([NotNull] SonaParser.MemberExpr_StandaloneContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr_Prefix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberExpr_Prefix([NotNull] SonaParser.MemberExpr_PrefixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr_Prefix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberExpr_Prefix([NotNull] SonaParser.MemberExpr_PrefixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberExpr_Suffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberExpr_Suffix([NotNull] SonaParser.MemberExpr_SuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberExpr_Suffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberExpr_Suffix([NotNull] SonaParser.MemberExpr_SuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.primitiveTypeMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimitiveTypeMemberAccess([NotNull] SonaParser.PrimitiveTypeMemberAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.primitiveTypeMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimitiveTypeMemberAccess([NotNull] SonaParser.PrimitiveTypeMemberAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.altMemberExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAltMemberExpr([NotNull] SonaParser.AltMemberExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.altMemberExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAltMemberExpr([NotNull] SonaParser.AltMemberExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.altMemberExpr_Suffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAltMemberExpr_Suffix([NotNull] SonaParser.AltMemberExpr_SuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.altMemberExpr_Suffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAltMemberExpr_Suffix([NotNull] SonaParser.AltMemberExpr_SuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.conditionalMember"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConditionalMember([NotNull] SonaParser.ConditionalMemberContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.conditionalMember"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConditionalMember([NotNull] SonaParser.ConditionalMemberContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constrainedMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstrainedMemberAccess([NotNull] SonaParser.ConstrainedMemberAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constrainedMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstrainedMemberAccess([NotNull] SonaParser.ConstrainedMemberAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constrainedFunctionAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstrainedFunctionAccess([NotNull] SonaParser.ConstrainedFunctionAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constrainedFunctionAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstrainedFunctionAccess([NotNull] SonaParser.ConstrainedFunctionAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constrainedPropertyAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstrainedPropertyAccess([NotNull] SonaParser.ConstrainedPropertyAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constrainedPropertyAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstrainedPropertyAccess([NotNull] SonaParser.ConstrainedPropertyAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.indexAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIndexAccess([NotNull] SonaParser.IndexAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.indexAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIndexAccess([NotNull] SonaParser.IndexAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberAccess([NotNull] SonaParser.MemberAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberAccess([NotNull] SonaParser.MemberAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.dynamicMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDynamicMemberAccess([NotNull] SonaParser.DynamicMemberAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.dynamicMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDynamicMemberAccess([NotNull] SonaParser.DynamicMemberAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.dynamicExprMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDynamicExprMemberAccess([NotNull] SonaParser.DynamicExprMemberAccessContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.dynamicExprMemberAccess"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDynamicExprMemberAccess([NotNull] SonaParser.DynamicExprMemberAccessContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberNumberConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberNumberConvertExpr([NotNull] SonaParser.MemberNumberConvertExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberNumberConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberNumberConvertExpr([NotNull] SonaParser.MemberNumberConvertExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberCharConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberCharConvertExpr([NotNull] SonaParser.MemberCharConvertExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberCharConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberCharConvertExpr([NotNull] SonaParser.MemberCharConvertExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberConvertExpr([NotNull] SonaParser.MemberConvertExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberConvertExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberConvertExpr([NotNull] SonaParser.MemberConvertExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.convertOperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConvertOperator([NotNull] SonaParser.ConvertOperatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.convertOperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConvertOperator([NotNull] SonaParser.ConvertOperatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptionSuffix([NotNull] SonaParser.OptionSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptionSuffix([NotNull] SonaParser.OptionSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberTypeConstructExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberTypeConstructExpr([NotNull] SonaParser.MemberTypeConstructExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberTypeConstructExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberTypeConstructExpr([NotNull] SonaParser.MemberTypeConstructExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.memberNewExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMemberNewExpr([NotNull] SonaParser.MemberNewExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.memberNewExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMemberNewExpr([NotNull] SonaParser.MemberNewExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constructArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstructArguments([NotNull] SonaParser.ConstructArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constructArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstructArguments([NotNull] SonaParser.ConstructArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.constructCallArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterConstructCallArgTuple([NotNull] SonaParser.ConstructCallArgTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.constructCallArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitConstructCallArgTuple([NotNull] SonaParser.ConstructCallArgTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.fieldAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldAssignment([NotNull] SonaParser.FieldAssignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.fieldAssignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldAssignment([NotNull] SonaParser.FieldAssignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.optionalFieldAssignmentExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptionalFieldAssignmentExpr([NotNull] SonaParser.OptionalFieldAssignmentExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.optionalFieldAssignmentExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptionalFieldAssignmentExpr([NotNull] SonaParser.OptionalFieldAssignmentExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.callArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCallArguments([NotNull] SonaParser.CallArgumentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.callArguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCallArguments([NotNull] SonaParser.CallArgumentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleCallArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleCallArgument([NotNull] SonaParser.SimpleCallArgumentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleCallArgument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleCallArgument([NotNull] SonaParser.SimpleCallArgumentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.callArgList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCallArgList([NotNull] SonaParser.CallArgListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.callArgList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCallArgList([NotNull] SonaParser.CallArgListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleCallArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleCallArgTuple([NotNull] SonaParser.SimpleCallArgTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleCallArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleCallArgTuple([NotNull] SonaParser.SimpleCallArgTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.complexCallArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComplexCallArgTuple([NotNull] SonaParser.ComplexCallArgTupleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.complexCallArgTuple"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComplexCallArgTuple([NotNull] SonaParser.ComplexCallArgTupleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.recordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRecordConstructor([NotNull] SonaParser.RecordConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.recordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRecordConstructor([NotNull] SonaParser.RecordConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousRecordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousRecordConstructor([NotNull] SonaParser.AnonymousRecordConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousRecordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousRecordConstructor([NotNull] SonaParser.AnonymousRecordConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousClassRecordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousClassRecordConstructor([NotNull] SonaParser.AnonymousClassRecordConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousClassRecordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousClassRecordConstructor([NotNull] SonaParser.AnonymousClassRecordConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.anonymousStructRecordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAnonymousStructRecordConstructor([NotNull] SonaParser.AnonymousStructRecordConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.anonymousStructRecordConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAnonymousStructRecordConstructor([NotNull] SonaParser.AnonymousStructRecordConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.recordConstructor_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRecordConstructor_Contents([NotNull] SonaParser.RecordConstructor_ContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.recordConstructor_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRecordConstructor_Contents([NotNull] SonaParser.RecordConstructor_ContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.arrayConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrayConstructor([NotNull] SonaParser.ArrayConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.arrayConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrayConstructor([NotNull] SonaParser.ArrayConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.sequenceConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSequenceConstructor([NotNull] SonaParser.SequenceConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.sequenceConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSequenceConstructor([NotNull] SonaParser.SequenceConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.computationSequenceConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComputationSequenceConstructor([NotNull] SonaParser.ComputationSequenceConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.computationSequenceConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComputationSequenceConstructor([NotNull] SonaParser.ComputationSequenceConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.collectionElement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCollectionElement([NotNull] SonaParser.CollectionElementContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.collectionElement"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCollectionElement([NotNull] SonaParser.CollectionElementContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.collectionFieldExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCollectionFieldExpression([NotNull] SonaParser.CollectionFieldExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.collectionFieldExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCollectionFieldExpression([NotNull] SonaParser.CollectionFieldExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTupleConstructor([NotNull] SonaParser.TupleConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTupleConstructor([NotNull] SonaParser.TupleConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.explicitTupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExplicitTupleConstructor([NotNull] SonaParser.ExplicitTupleConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.explicitTupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExplicitTupleConstructor([NotNull] SonaParser.ExplicitTupleConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.classTupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterClassTupleConstructor([NotNull] SonaParser.ClassTupleConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.classTupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitClassTupleConstructor([NotNull] SonaParser.ClassTupleConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.structTupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStructTupleConstructor([NotNull] SonaParser.StructTupleConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.structTupleConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStructTupleConstructor([NotNull] SonaParser.StructTupleConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.simpleTupleContents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleTupleContents([NotNull] SonaParser.SimpleTupleContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.simpleTupleContents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleTupleContents([NotNull] SonaParser.SimpleTupleContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.complexTupleContents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterComplexTupleContents([NotNull] SonaParser.ComplexTupleContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.complexTupleContents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitComplexTupleContents([NotNull] SonaParser.ComplexTupleContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.spreadExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSpreadExpression([NotNull] SonaParser.SpreadExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.spreadExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSpreadExpression([NotNull] SonaParser.SpreadExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowExpression([NotNull] SonaParser.FollowExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowExpression([NotNull] SonaParser.FollowExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.followExpression_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFollowExpression_Contents([NotNull] SonaParser.FollowExpression_ContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.followExpression_Contents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFollowExpression_Contents([NotNull] SonaParser.FollowExpression_ContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.spreadFollowExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSpreadFollowExpression([NotNull] SonaParser.SpreadFollowExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.spreadFollowExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSpreadFollowExpression([NotNull] SonaParser.SpreadFollowExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpolatedString"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpolatedString([NotNull] SonaParser.InterpolatedStringContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpolatedString"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpolatedString([NotNull] SonaParser.InterpolatedStringContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.plainInterpolatedString"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPlainInterpolatedString([NotNull] SonaParser.PlainInterpolatedStringContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.plainInterpolatedString"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPlainInterpolatedString([NotNull] SonaParser.PlainInterpolatedStringContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.verbatimInterpolatedString"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVerbatimInterpolatedString([NotNull] SonaParser.VerbatimInterpolatedStringContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.verbatimInterpolatedString"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVerbatimInterpolatedString([NotNull] SonaParser.VerbatimInterpolatedStringContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrComponent"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrComponent([NotNull] SonaParser.InterpStrComponentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrComponent"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrComponent([NotNull] SonaParser.InterpStrComponentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrAlignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrAlignment([NotNull] SonaParser.InterpStrAlignmentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrAlignment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrAlignment([NotNull] SonaParser.InterpStrAlignmentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrGeneralFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrGeneralFormat([NotNull] SonaParser.InterpStrGeneralFormatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrGeneralFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrGeneralFormat([NotNull] SonaParser.InterpStrGeneralFormatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrStandardFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrStandardFormat([NotNull] SonaParser.InterpStrStandardFormatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrStandardFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrStandardFormat([NotNull] SonaParser.InterpStrStandardFormatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrCustomFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrCustomFormat([NotNull] SonaParser.InterpStrCustomFormatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrCustomFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrCustomFormat([NotNull] SonaParser.InterpStrCustomFormatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrNumberFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrNumberFormat([NotNull] SonaParser.InterpStrNumberFormatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrNumberFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrNumberFormat([NotNull] SonaParser.InterpStrNumberFormatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrComponentFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrComponentFormat([NotNull] SonaParser.InterpStrComponentFormatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrComponentFormat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrComponentFormat([NotNull] SonaParser.InterpStrComponentFormatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.interpStrExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterpStrExpression([NotNull] SonaParser.InterpStrExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.interpStrExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterpStrExpression([NotNull] SonaParser.InterpStrExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFree([NotNull] SonaParser.InlineSourceFreeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFree"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFree([NotNull] SonaParser.InlineSourceFreeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceReturning([NotNull] SonaParser.InlineSourceReturningContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceReturning"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceReturning([NotNull] SonaParser.InlineSourceReturningContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceTerminating([NotNull] SonaParser.InlineSourceTerminatingContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceTerminating"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceTerminating([NotNull] SonaParser.InlineSourceTerminatingContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceLanguage"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceLanguage([NotNull] SonaParser.InlineSourceLanguageContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceLanguage"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceLanguage([NotNull] SonaParser.InlineSourceLanguageContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSharp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSharp([NotNull] SonaParser.InlineSourceFSharpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSharp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSharp([NotNull] SonaParser.InlineSourceFSharpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceJavaScript"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceJavaScript([NotNull] SonaParser.InlineSourceJavaScriptContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceJavaScript"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceJavaScript([NotNull] SonaParser.InlineSourceJavaScriptContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSFirstLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSFirstLine([NotNull] SonaParser.InlineSourceFSFirstLineContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSFirstLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSFirstLine([NotNull] SonaParser.InlineSourceFSFirstLineContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSLine([NotNull] SonaParser.InlineSourceFSLineContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSLine([NotNull] SonaParser.InlineSourceFSLineContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSNewLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSNewLine([NotNull] SonaParser.InlineSourceFSNewLineContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSNewLine"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSNewLine([NotNull] SonaParser.InlineSourceFSNewLineContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSIndentation"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSIndentation([NotNull] SonaParser.InlineSourceFSIndentationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSIndentation"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSIndentation([NotNull] SonaParser.InlineSourceFSIndentationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSToken"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSToken([NotNull] SonaParser.InlineSourceFSTokenContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSToken"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSToken([NotNull] SonaParser.InlineSourceFSTokenContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSPart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSPart([NotNull] SonaParser.InlineSourceFSPartContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSPart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSPart([NotNull] SonaParser.InlineSourceFSPartContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSDirective"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSDirective([NotNull] SonaParser.InlineSourceFSDirectiveContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSDirective"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSDirective([NotNull] SonaParser.InlineSourceFSDirectiveContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSLineComment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSLineComment([NotNull] SonaParser.InlineSourceFSLineCommentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSLineComment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSLineComment([NotNull] SonaParser.InlineSourceFSLineCommentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSBlockComment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSBlockComment([NotNull] SonaParser.InlineSourceFSBlockCommentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSBlockComment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSBlockComment([NotNull] SonaParser.InlineSourceFSBlockCommentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSLineCutComment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSLineCutComment([NotNull] SonaParser.InlineSourceFSLineCutCommentContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSLineCutComment"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSLineCutComment([NotNull] SonaParser.InlineSourceFSLineCutCommentContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.inlineSourceFSWhitespace"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInlineSourceFSWhitespace([NotNull] SonaParser.InlineSourceFSWhitespaceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.inlineSourceFSWhitespace"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInlineSourceFSWhitespace([NotNull] SonaParser.InlineSourceFSWhitespaceContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unaryOperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryOperator([NotNull] SonaParser.UnaryOperatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unaryOperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryOperator([NotNull] SonaParser.UnaryOperatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.relationalOperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRelationalOperator([NotNull] SonaParser.RelationalOperatorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.relationalOperator"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRelationalOperator([NotNull] SonaParser.RelationalOperatorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenGTE"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTokenGTE([NotNull] SonaParser.TokenGTEContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenGTE"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTokenGTE([NotNull] SonaParser.TokenGTEContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenLSHIFT"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTokenLSHIFT([NotNull] SonaParser.TokenLSHIFTContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenLSHIFT"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTokenLSHIFT([NotNull] SonaParser.TokenLSHIFTContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenRSHIFT"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTokenRSHIFT([NotNull] SonaParser.TokenRSHIFTContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenRSHIFT"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTokenRSHIFT([NotNull] SonaParser.TokenRSHIFTContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.tokenDOUBLEQUESTION"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTokenDOUBLEQUESTION([NotNull] SonaParser.TokenDOUBLEQUESTIONContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.tokenDOUBLEQUESTION"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTokenDOUBLEQUESTION([NotNull] SonaParser.TokenDOUBLEQUESTIONContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.number"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumber([NotNull] SonaParser.NumberContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.number"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumber([NotNull] SonaParser.NumberContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.numberToken"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumberToken([NotNull] SonaParser.NumberTokenContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.numberToken"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumberToken([NotNull] SonaParser.NumberTokenContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.char"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterChar([NotNull] SonaParser.CharContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.char"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitChar([NotNull] SonaParser.CharContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.charToken"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCharToken([NotNull] SonaParser.CharTokenContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.charToken"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCharToken([NotNull] SonaParser.CharTokenContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.string"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterString([NotNull] SonaParser.StringContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.string"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitString([NotNull] SonaParser.StringContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.unit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnit([NotNull] SonaParser.UnitContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.unit"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnit([NotNull] SonaParser.UnitContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorMissingExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterErrorMissingExpression([NotNull] SonaParser.ErrorMissingExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorMissingExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitErrorMissingExpression([NotNull] SonaParser.ErrorMissingExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedFollow"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterErrorUnsupportedFollow([NotNull] SonaParser.ErrorUnsupportedFollowContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedFollow"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitErrorUnsupportedFollow([NotNull] SonaParser.ErrorUnsupportedFollowContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedNumberSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterErrorUnsupportedNumberSuffix([NotNull] SonaParser.ErrorUnsupportedNumberSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedNumberSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitErrorUnsupportedNumberSuffix([NotNull] SonaParser.ErrorUnsupportedNumberSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedEndCharSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterErrorUnsupportedEndCharSuffix([NotNull] SonaParser.ErrorUnsupportedEndCharSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedEndCharSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitErrorUnsupportedEndCharSuffix([NotNull] SonaParser.ErrorUnsupportedEndCharSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnsupportedEndStringSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterErrorUnsupportedEndStringSuffix([NotNull] SonaParser.ErrorUnsupportedEndStringSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnsupportedEndStringSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitErrorUnsupportedEndStringSuffix([NotNull] SonaParser.ErrorUnsupportedEndStringSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="SonaParser.errorUnderscoreReserved"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterErrorUnderscoreReserved([NotNull] SonaParser.ErrorUnderscoreReservedContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="SonaParser.errorUnderscoreReserved"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitErrorUnderscoreReserved([NotNull] SonaParser.ErrorUnderscoreReservedContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace Sona.Grammar
