namespace RSAR.Models
{
    public class SearchOptions
    {
        public string[] ExcludedDirectories { get; set; }
        public string[] ExcludedExtensions { get; set; }
        public string[] IncludedDirectories { get; set; }
        public string[] IncludedExtensions { get; set; }
        public OutputOptions Output { get; set; }
        public string RootDirectory { get; set; }
        public string Regex { get; set; }
        public bool IgnoreCase { get; set; }
    }
}