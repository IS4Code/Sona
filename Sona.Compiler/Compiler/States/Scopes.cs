namespace IS4.Sona.Compiler.States
{
    internal interface IReturnScope
    {
        string? VariableName { get; }
    }

    internal interface IFunctionScope
    {
        void WriteBegin();
        void WriteEnd();
    }
}
