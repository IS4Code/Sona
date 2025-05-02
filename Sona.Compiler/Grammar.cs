namespace Sona.Grammar
{
    partial class SonaParser
    {
        internal bool combinedOperator {
            get {
                var stream = TokenStream;
                return stream.LT(-1).StopIndex + 1 == stream.LT(1).StartIndex;
            }
        }
    }
}
