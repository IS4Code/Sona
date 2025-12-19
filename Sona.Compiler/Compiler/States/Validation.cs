using Antlr4.Runtime;
using Sona.Compiler.States;

namespace Sona.Compiler
{
    partial class ScriptState
    {
        protected Validation Validate => new(this);

        protected readonly partial record struct Validation(ScriptState source)
        {
            public void ReturnPlacement(ParserRuleContext context)
            {
                var computationScope = source.FindContext<IComputationContext>();
                if(computationScope?.HasFlag(ComputationFlags.IsCollection) ?? false)
                {
                    const string msg = "`return` in a collection construction is not supported.";
                    if(computationScope?.HasFlag(ComputationFlags.IsComputation) != true)
                    {
                        // Can't return from a sequence
                        source.Error(msg + " Use `yield` instead.", context);
                    }
                    else
                    {
                        // No mechanism to indicate returning only
                        source.Error(msg + " Use `yield` or `yield return` instead.", context);
                    }
                }
            }

            public void YieldPlacement(ParserRuleContext context)
            {
                if(source.FindContext<IComputationContext>()?.HasAnyFlag(ComputationFlags.IsCollection | ComputationFlags.IsComputation) != true)
                {
                    source.Error("`yield` is not allowed outside a collection or computation.", context);
                }
            }

            public void YieldReturnPlacement(ParserRuleContext context)
            {
                if(source.FindContext<IComputationContext>()?.HasFlag(ComputationFlags.IsComputation) != true)
                {
                    source.Error("`yield return` is not allowed outside a computation.", context);
                }
                if(source.FindContext<IReturnableContext>()?.HasFlag(ReturnFlags.Optional) ?? false)
                {
                    source.Error("`yield return` cannot be used in an optional function.", context);
                }
            }
        }
    }
}
