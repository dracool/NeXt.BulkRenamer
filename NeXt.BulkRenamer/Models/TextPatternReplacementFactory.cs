namespace NeXt.BulkRenamer.Models
{
    internal class TextPatternReplacementFactory : ITextReplacementFactory
    {
        public ITextReplacement Create(string pattern)
        {
            return new TextPatternReplacement(pattern);
        }
    }
}
