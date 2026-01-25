using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Sona.Compiler.Tools;
using Sona.Grammar;
using static Sona.Grammar.SonaParser;

namespace Sona.Compiler.States
{
    internal sealed class MultiFunctionState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = true;
        }

        private void OnEnter(ParserRuleContext context)
        {
            if(first)
            {
                Out.Write("let rec ");
                first = false;
            }
            else
            {
                Out.WriteLine();
                Out.Write("and ");
            }
        }

        public override void EnterFuncDecl(FuncDeclContext context)
        {
            OnEnter(context);
            EnterState<FunctionDeclState>().EnterFuncDecl(context);
        }

        public override void ExitFuncDecl(FuncDeclContext context)
        {

        }

        public override void EnterCaseFuncDecl(CaseFuncDeclContext context)
        {
            OnEnter(context);
            EnterState<CaseFunctionDeclState>().EnterCaseFuncDecl(context);
        }

        public override void ExitCaseFuncDecl(CaseFuncDeclContext context)
        {

        }

        public override void EnterMultiFuncDecl(MultiFuncDeclContext context)
        {

        }

        public override void ExitMultiFuncDecl(MultiFuncDeclContext context)
        {
            ExitState().ExitMultiFuncDecl(context);
        }
    }

    internal abstract class FunctionBaseState : NodeState, IFunctionContext
    {
        BlockFlags IBlockContext.Flags => BlockFlags.None;

        ReturnFlags IReturnableContext.Flags => ReturnFlags.None | (returnOptionType != null ? ReturnFlags.Optional : 0);

        InterruptFlags IInterruptibleContext.Flags => InterruptFlags.None;

        string? IInterruptibleContext.InterruptingVariable => null;

        ComputationFlags IComputationContext.Flags => ComputationFlags.None;

        ExpressionFlags IExpressionContext.Flags => ExpressionFlags.IsValue;

        ImplementationType? returnOptionType;

        bool IStatementContext.TrailAllowed => true;

        bool hasType;
        BindingSet bindings;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            hasType = false;
            returnOptionType = default;
            bindings = new(FindContext<IBindingContext>());
        }

        public override void EnterOptionSuffix(OptionSuffixContext context)
        {
            returnOptionType = OptionImplementationType;
        }

        public override void ExitOptionSuffix(OptionSuffixContext context)
        {

        }

        public sealed override void EnterParamList(ParamListContext context)
        {
            EnterState<ParamListState>().EnterParamList(context);
        }

        public sealed override void ExitParamList(ParamListContext context)
        {

        }

        public sealed override void EnterType(TypeContext context)
        {
            hasType = true;
            Out.WriteOperator(':');
            if(returnOptionType is { } optionType)
            {
                Out.WriteOptionType(optionType);
                Out.Write('<');
            }
            base.EnterType(context);
        }

        public sealed override void ExitType(TypeContext context)
        {
            base.ExitType(context);
            if(returnOptionType != null)
            {
                Out.Write('>');
            }
        }

        public sealed override void EnterValueBlock(ValueBlockContext context)
        {
            if(!hasType && returnOptionType is { } optionType && this is not CaseFunctionDeclState)
            {
                Out.WriteOperator(':');
                Out.Write("_ ");
                Out.WriteOptionAbbreviation(optionType);
            }
            Out.WriteOperator('=');
            Out.WriteLine('(');
            Out.EnterScope();
            EnterState<BlockState>().EnterValueBlock(context);
        }

        public sealed override void ExitValueBlock(ValueBlockContext context)
        {
            Out.ExitScope();
            Out.Write(')');
        }

        public override void EnterFuncBody(FuncBodyContext context)
        {

        }

        public override void ExitFuncBody(FuncBodyContext context)
        {

        }

        void IBindingContext.Set(string name, BindingKind kind)
        {
            bindings.Set(name, kind);
        }

        BindingKind IBindingContext.Get(string name)
        {
            return bindings.Get(name);
        }

        void IComputationContext.WriteBeginBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteBeginBlockExpression(context);
        }

        void IComputationContext.WriteEndBlockExpression(ParserRuleContext context)
        {
            Defaults.WriteEndBlockExpression(context);
        }

        void IReturnableContext.WriteEarlyReturn(ParserRuleContext context)
        {
            Defaults.WriteEarlyReturn(context);
        }

        void IReturnableContext.WriteReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteReturnStatement(context);
        }

        void IReturnableContext.WriteAfterReturnStatement(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnStatement(context);
        }

        void IReturnableContext.WriteReturnValue(bool isOption, ParserRuleContext context)
        {
            if(returnOptionType is { } optionType)
            {
                if(isOption)
                {
                    isOption = false;
                }
                else
                {
                    Out.WriteOptionSome(optionType);
                }
            }
            Defaults.WriteReturnValue(isOption, context);
        }

        void IReturnableContext.WriteAfterReturnValue(ParserRuleContext context)
        {
            Defaults.WriteAfterReturnValue(context);
        }

        void IReturnableContext.WriteEmptyReturnValue(ParserRuleContext context)
        {
            if(returnOptionType is { } optionType)
            {
                Out.WriteOptionNone(optionType);
            }
            else
            {
                Defaults.WriteEmptyReturnValue(context);
            }
        }

        void IBlockContext.WriteImplicitReturnStatement(ParserRuleContext context)
        {
            if(returnOptionType is { } optionType)
            {
                Out.WriteOptionNone(optionType);
            }
            else
            {
                Defaults.WriteImplicitReturnStatement(context);
            }
        }

        void IInterruptibleContext.WriteBreak(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteBreak(hasExpression, context);
        }

        void IInterruptibleContext.WriteContinue(bool hasExpression, ParserRuleContext context)
        {
            Defaults.WriteContinue(hasExpression, context);
        }

        void IInterruptibleContext.WriteAfterBreak(ParserRuleContext context)
        {
            Defaults.WriteAfterBreak(context);
        }

        void IInterruptibleContext.WriteAfterContinue(ParserRuleContext context)
        {
            Defaults.WriteAfterContinue(context);
        }
    }

    internal sealed class FunctionDeclState : FunctionBaseState
    {
        public override void EnterFuncDecl(FuncDeclContext context)
        {

        }

        public override void ExitFuncDecl(FuncDeclContext context)
        {
            ExitState().ExitFuncDecl(context);
        }

        public override void EnterInlineFuncDecl(InlineFuncDeclContext context)
        {

        }

        public override void ExitInlineFuncDecl(InlineFuncDeclContext context)
        {
            ExitState().ExitInlineFuncDecl(context);
        }
    }

    internal sealed class CaseFunctionDeclState : FunctionBaseState
    {
        bool firstName;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            firstName = true;
        }

        public override void EnterCaseFuncDecl(CaseFuncDeclContext context)
        {

        }

        public override void ExitCaseFuncDecl(CaseFuncDeclContext context)
        {
            ExitState().ExitCaseFuncDecl(context);
        }

        public override void EnterInlineCaseFuncDecl(InlineCaseFuncDeclContext context)
        {

        }

        public override void ExitInlineCaseFuncDecl(InlineCaseFuncDeclContext context)
        {
            ExitState().ExitInlineCaseFuncDecl(context);
        }

        public override void EnterCaseFuncName(CaseFuncNameContext context)
        {
            Out.Write("(|");
        }

        public override void EnterName(NameContext context)
        {
            Out.WriteNext('|', ref firstName);

            base.EnterName(context);
        }

        public override void EnterOptionSuffix(OptionSuffixContext context)
        {
            Out.Write("|_");

            base.EnterOptionSuffix(context);
        }

        public override void EnterFuncBody(FuncBodyContext context)
        {
            Out.Write("|)");

            base.EnterFuncBody(context);
        }
    }

    internal sealed class ParamListState : NodeState
    {
        bool first;

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            first = false;
        }

        public override void EnterParamTuple(ParamTupleContext context)
        {
            Out.Write('(');
            first = true;
        }

        public override void ExitParamTuple(ParamTupleContext context)
        {
            Out.Write(')');
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            Out.WriteNext(',', ref first);

            EnterState<DeclarationState>().EnterDeclaration(context);
        }

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void ExitParamList(ParamListContext context)
        {
            ExitState().ExitParamList(context);
        }
    }

    internal abstract class VariableDeclarationState : DeclarationState, IExpressionContext
    {
        protected bool IsConst { get; private set; }
        protected bool IsUse { get; private set; }
        protected bool IsRec { get; private set; }
        protected bool IsMutable { get; private set; }

        ExpressionFlags IExpressionContext.Flags => (FindContext<IExpressionContext>()?.Flags ?? 0) | ExpressionFlags.IsValue | (IsConst ? ExpressionFlags.IsConstant : 0);
        
        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            IsConst = false;
            IsUse = false;
            IsRec = false;
            IsMutable = false;
        }

        protected void OnEnter(ParserRuleContext context)
        {
            IsRec = LexerContext.GetState<RecursivePragma>()?.Value ?? false;
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            if(IsConst)
            {
                Out.Write("[<");
                Out.WriteCoreName("LiteralAttribute");
                Out.WriteLine(">]");
            }
        }

        public override void ExitDeclaration(DeclarationContext context)
        {

        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            switch(node.Symbol.Type)
            {
                case SonaLexer.VAR:
                    IsMutable = true;
                    break;
                case SonaLexer.USE:
                    IsUse = true;
                    break;
                case SonaLexer.CONST:
                    if(FindContext<IFunctionContext>() is DeclarationsBlockState)
                    {
                        IsConst = true;
                    }
                    else
                    {
                        Error("A `const` variable may be specified only at the package level.", node);
                    }
                    break;
            }
        }
    }

    internal class DeclarationState : NodeState
    {
        public override void EnterDeclaration(DeclarationContext context)
        {

        }

        public override void ExitDeclaration(DeclarationContext context)
        {
            ExitState().ExitDeclaration(context);
        }

        public override void EnterPattern(PatternContext context)
        {
            Out.Write('(');
            EnterState<PatternState>().EnterPattern(context);
        }

        public override void ExitPattern(PatternContext context)
        {
            Out.Write(')');
        }

        public sealed override void EnterOptionalName(OptionalNameContext context)
        {
            Out.Write('?');
        }

        public sealed override void ExitOptionalName(OptionalNameContext context)
        {

        }

        public override void EnterName(NameContext context)
        {
            StartCaptureInput(context);

            base.EnterName(context);
        }

        public override void ExitName(NameContext context)
        {
            base.ExitName(context);

            var name = StopCaptureInputIdentifier(context);

            if(FindContext<IBindingContext>() is { } binding)
            {
                binding.Set(name, BindingKind.Variable);
            }
        }

        public sealed override void EnterType(TypeContext context)
        {
            Out.WriteOperator(':');
            base.EnterType(context);
        }

        public sealed override void ExitType(TypeContext context)
        {
            base.ExitType(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);

            if(node.Symbol.Type == SonaLexer.INLINE)
            {
                Out.Write("[<");
                Out.WriteCoreName("InlineIfLambdaAttribute");
                Out.Write(">]");
            }
        }
    }

    internal sealed class SimpleDeclarationState : VariableDeclarationState
    {
        public override void EnterSimpleVariableDecl(SimpleVariableDeclContext context)
        {
            OnEnter(context);
        }

        public override void ExitSimpleVariableDecl(SimpleVariableDeclContext context)
        {
            ExitState().ExitSimpleVariableDecl(context);
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            base.EnterDeclaration(context);

            if(IsUse)
            {
                Out.Write("use ");
            }
            else
            {
                Out.Write("let ");
            }
            if(IsRec)
            {
                Out.Write("rec ");
            }
            if(IsMutable)
            {
                Out.Write("mutable ");
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            Out.WriteOperator('=');
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {

        }

        public sealed override void ExitLocalAttribute(LocalAttributeContext context)
        {
            Out.WriteLine();
            base.ExitLocalAttribute(context);
        }
    }

    internal sealed class MultiDeclarationState : VariableDeclarationState
    {
        bool anyIsFollow, anyIsNonFollow;
        ISourceCapture? capture;
        Variable currentVariable;
        readonly List<Variable> variables = new();
        readonly List<ISourceCapture> attributes = new();

        bool? _isComputation;
        bool isComputation {
            get {
                if(_isComputation is bool result)
                {
                    return result;
                }
                result = FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) ?? false;
                _isComputation = result;
                return result;
            }
        }

        protected override void Initialize(ScriptEnvironment environment, ScriptState? parent)
        {
            base.Initialize(environment, parent);

            anyIsFollow = false;
            anyIsNonFollow = false;
            capture = null;
            currentVariable = default;
            variables.Clear();
            attributes.Clear();
            _isComputation = null;
        }

        public override void EnterMultiVariableDecl(MultiVariableDeclContext context)
        {
            OnEnter(context);
        }

        public override void ExitMultiVariableDecl(MultiVariableDeclContext context)
        {
            OnExit(context);
            ExitState().ExitMultiVariableDecl(context);
        }

        public override void EnterFollowVariableDecl(FollowVariableDeclContext context)
        {
            OnEnter(context);
        }

        public override void ExitFollowVariableDecl(FollowVariableDeclContext context)
        {
            OnExit(context);
            ExitState().ExitFollowVariableDecl(context);
        }

        private void OnExit(ParserRuleContext context)
        {
            if(IsRec)
            {
                if(IsUse)
                {
                    Error("A `use` declaration is not supported together with `#pragma recursive`.", context);
                }
                if(anyIsFollow)
                {
                    Error("The use of `#pragma recursive` is not supported together with a `follow`-initialized variable.", context);
                }
                OnExitRecursive(context);
                return;
            }
            
            if(IsConst)
            {
                if(anyIsFollow)
                {
                    Error("A `const` variable cannot be initialized with `follow`.", context);
                }
                // Always recursive (using further-declared variables is prevented anyway).
                OnExitRecursive(context);
                return;
            }

            if(IsUse)
            {
                if(variables.Count > 1)
                {
                    // Multi-variable `use` is too complex
                    Error("A `use` declaration initializing multiple variables is not supported. Use multiple individual `use` declarations.", context);
                }
                else
                {
                    OnExitSingleUse(variables[0], context);
                    return;
                }
            }

            if(anyIsFollow)
            {
                if(isComputation)
                {
                    if(anyIsNonFollow || IsMutable)
                    {
                        OnExitFollowComplex(context);
                    }
                    else
                    {
                        OnExitFollowSimple(context);
                    }
                }
                else
                {
                    if(variables.Where(v => v.IsFollow).Take(2).Count() > 1)
                    {
                        // Calling `MergeSources` manually is not supported
                        Error("Multiple `follow`-initialized variables are supported only in a computation.", context);
                    }
                    OnExitTupled(context);
                }
            }
            else
            {
                OnExitTupled(context);
            }
        }

        private void OnExitTupled(ParserRuleContext context)
        {
            // Ignore `const`, `rec`, `use`, `follow` (computation).

            WriteAttributes(false);
            Out.Write("let ");
            if(IsMutable)
            {
                Out.Write("mutable ");
            }
            Out.Write('(');

            bool first = true;
            foreach(var variable in variables)
            {
                Out.WriteNext(',', ref first);
                variable.Declaration.Play(Out);
            }

            Out.Write(')');
            Out.WriteOperator('=');
            Out.EnterNestedScope();
            Out.Write('(');

            first = true;
            foreach(var variable in variables)
            {
                Out.WriteNext(',', ref first);

                if(variable.IsFollow)
                {
                    // Not in a computation (checked already)
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                }

                variable.Value.Play(Out);

                if(variable.IsFollow)
                {
                    Out.WriteAfterGlobalComputationOperator();
                }
            }

            Out.ExitNestedScope();
            Out.Write(')');
        }

        private void OnExitSingleUse(Variable variable, ParserRuleContext context)
        {
            // Ignore `rec`.

            if(variable.IsFollow && isComputation)
            {
                // Follow first, use later (`use!` has too limiting syntax)
                var name = Out.CreateTemporaryIdentifier();
                Out.Write("let! ");
                Out.WriteIdentifier(name);
                Out.WriteOperator('=');
                variable.Value.Play(Out);
                Out.WriteLine();

                Out.Write("use ");
                if(IsMutable)
                {
                    Out.Write("mutable ");
                }
                WriteAttributes(true);
                variable.Declaration.Play(Out);
                Out.WriteOperator('=');
                Out.WriteIdentifier(name);
            }
            else
            {
                Out.Write("use ");
                if(IsMutable)
                {
                    Out.Write("mutable ");
                }
                WriteAttributes(true);
                variable.Declaration.Play(Out);
                Out.WriteOperator('=');

                if(variable.IsFollow)
                {
                    Out.WriteGlobalComputationOperator("ReturnFrom");
                }

                variable.Value.Play(Out);

                if(variable.IsFollow)
                {
                    Out.WriteAfterGlobalComputationOperator();
                }
            }
        }

        private void OnExitRecursive(ParserRuleContext context)
        {
            // Ignore `use`, `follow`.

            Out.Write("let rec ");

            WriteAttributes(true);

            if(IsMutable)
            {
                // Detected by F#
                Out.Write("mutable ");
            }

            bool first = true;
            foreach(var variable in variables)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    Out.WriteLine();
                    Out.Write("and ");
                    WriteAttributes(true);
                }

                variable.Declaration.Play(Out);

                Out.WriteOperator('=');

                variable.Value.Play(Out);
            }
        }

        private void OnExitFollowSimple(ParserRuleContext context)
        {
            // Ignore `const`, `rec`, `use`, `mutable`, non-`follow`.

            bool first = true;
            foreach(var variable in variables)
            {
                if(first)
                {
                    Out.Write("let! ");
                    WriteAttributes(true);

                    first = false;
                }
                else
                {
                    Out.WriteLine();
                    Out.Write("and! ");
                    WriteAttributes(true);
                }

                Out.Write('(');
                variable.Declaration.Play(Out);
                Out.Write(')');

                Out.WriteOperator('=');

                variable.Value.Play(Out);
            }
        }

        private void OnExitFollowComplex(ParserRuleContext context)
        {
            // Ignore `const`, `rec`, `use`, non-computation.

            int numVariables = variables.Count;
            var names = new string[numVariables];

            if(anyIsNonFollow)
            {
                // Store non-followed values first

                for(int i = 0; i < numVariables; i++)
                {
                    Out.Write("let ");

                    var name = Out.CreateTemporaryIdentifier();
                    names[i] = name;

                    Out.WriteIdentifier(name);
                    Out.WriteOperator('=');
                    variables[i].Value.Play(Out);

                    Out.WriteLine();
                }

                // Follow those that should be followed

                bool first = true;
                for(int i = 0; i < numVariables; i++)
                {
                    var variable = variables[i];
                    if(!variable.IsFollow)
                    {
                        continue;
                    }

                    if(first)
                    {
                        Out.Write("let! ");
                        first = false;
                    }
                    else
                    {
                        Out.Write("and! ");
                    }

                    var name = names[i];
                    Out.WriteIdentifier(name);
                    Out.WriteOperator('=');
                    Out.WriteIdentifier(name);

                    Out.WriteLine();
                }
            }
            else
            {
                // Store and follow as a single operation

                bool first = true;
                for(int i = 0; i < numVariables; i++)
                {
                    if(first)
                    {
                        Out.Write("let! ");
                        first = false;
                    }
                    else
                    {
                        Out.Write("and! ");
                    }

                    var name = Out.CreateTemporaryIdentifier();
                    names[i] = name;

                    Out.WriteIdentifier(name);
                    Out.WriteOperator('=');
                    variables[i].Value.Play(Out);

                    Out.WriteLine();
                }
            }

            // Bind to result

            WriteAttributes(false);
            Out.Write("let ");
            if(IsMutable)
            {
                Out.Write("mutable ");
            }
            Out.Write('(');

            {
                bool first = true;
                foreach(var variable in variables)
                {
                    Out.WriteNext(',', ref first);
                    variable.Declaration.Play(Out);
                }
            }

            Out.Write(')');
            Out.WriteOperator('=');
            Out.Write('(');

            {
                bool first = true;
                foreach(var name in names)
                {
                    Out.WriteNext(',', ref first);
                    Out.WriteIdentifier(name);
                }
            }

            Out.Write(')');
        }

        private void WriteAttributes(bool inline)
        {
            if(attributes.Count == 0 && !IsConst)
            {
                return;
            }

            if(inline) Out.EnterNestedScope();

            foreach(var attr in attributes)
            {
                attr.Play(Out);
                Out.WriteLine();
            }

            if(IsConst)
            {
                Out.Write("[<");
                Out.WriteCoreName("LiteralAttribute");
                Out.Write(">]");
                Out.WriteLine();
            }

            if(inline) Out.ExitNestedScope();
        }

        public override void EnterDeclaration(DeclarationContext context)
        {
            capture = Out.StartCapture();
            currentVariable.Declaration = capture;
            base.EnterDeclaration(context);
        }

        public override void ExitDeclaration(DeclarationContext context)
        {
            base.ExitDeclaration(context);
            if(capture != null)
            {
                Out.StopCapture(capture);
            }
        }

        public override void EnterExpression(ExpressionContext context)
        {
            capture = Out.StartCapture();
            currentVariable.Value = capture;
            currentVariable.IsFollow = false;
            anyIsNonFollow = true;
            EnterState<ExpressionState>().EnterExpression(context);
        }

        public override void ExitExpression(ExpressionContext context)
        {
            if(capture != null)
            {
                Out.StopCapture(capture);
                variables.Add(currentVariable);
            }
        }

        public override void EnterFollowExpression(FollowExpressionContext context)
        {
            capture = Out.StartCapture();
            currentVariable.Value = capture;
            currentVariable.IsFollow = true;
            anyIsFollow = true;
            EnterState<FollowStatementState.Operand>().EnterFollowExpression(context);
        }

        public override void ExitFollowExpression(FollowExpressionContext context)
        {
            if(capture != null)
            {
                Out.StopCapture(capture);
                variables.Add(currentVariable);
            }
        }

        public override void EnterLocalAttribute(LocalAttributeContext context)
        {
            capture = Out.StartCapture();
            attributes.Add(capture);
            base.EnterLocalAttribute(context);
        }

        public override void ExitLocalAttribute(LocalAttributeContext context)
        {
            base.ExitLocalAttribute(context);
            if(capture != null)
            {
                Out.StopCapture(capture);
            }
        }

        record struct Variable(ISourceCapture Declaration, ISourceCapture Value, bool IsFollow);
    }
}
