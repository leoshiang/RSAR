namespace RSAR.Models
{
    public class OutputOptions
    {
        public bool FileName { get; set; }
        public bool MatchedCount { get; set; }
        public int[] MatchedGroups { get; set; }
        public bool MatchedText { get; set; }
        public string TextTransform { get; set; }
    }
}