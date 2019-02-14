using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dolphin.IO.File;
using Newtonsoft.Json;
using RSAR.Models;

namespace RSAR
{
    internal class Program
    {
        private static Config Config;

        public static void Main(string[] args)
        {
            try
            {
                Config = GetConfig(args);
                var files = SearchFiles(Config);
                ProcessFiles(files);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured when processing files:" + e.Message);
                Environment.Exit(-1);
            }
        }

        private static void GenerateOutput(string file, MatchCollection matches)
        {
            if (Config.Output.FileName) Console.WriteLine(file);
            if (Config.Output.MatchedCount) Console.WriteLine(matches.Count);
            if (!Config.Output.MatchedText) return;
            foreach (Match match in matches)
            {
                if (Config.Output.MatchedGroups.Length == 0)
                {
                    Console.WriteLine(match.Value);
                    continue;
                }

                for (var i = 1; i < match.Groups.Count; i++)
                {
                    var group = match.Groups[i];
                    if (Config.Output.MatchedGroups.Contains(i))
                    {
                        var output = group.Value;
                        if (Config.Output.TextTransform == "U")
                        {
                            output = output.ToUpper();
                        }

                        if (Config.Output.TextTransform == "L")
                        {
                            output = output.ToLower();
                        }

                        Console.Write(output);
                    }
                }

                Console.WriteLine();
            }
        }

        private static Config GetConfig(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid arguments.");
                PrintUsage();
                Environment.Exit(-1);
            }

            var optionsFileName = args[0];

            if (!File.Exists(optionsFileName))
            {
                Console.WriteLine($"File {optionsFileName} not found!");
                Environment.Exit(-1);
            }

            var contents = File.ReadAllText(optionsFileName);
            return JsonConvert.DeserializeObject<Config>(contents);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: Grape");
        }

        private static void ProcessFiles(IEnumerable<string> files)
        {
            var regexOptions = GetOptions(Config.Search);
            foreach (var file in files)
            {
                var fileContents = File.ReadAllText(file);
                var matches = Regex.Matches(fileContents, Config.Search.Regex, regexOptions);
                if (matches.Count == 0) continue;
                GenerateOutput(file, matches);
            }
        }

        private static RegexOptions GetOptions(SearchOptions searchOptions)
        {
            var result = RegexOptions.None;

            if (searchOptions.IgnoreCase)
            {
                result = result | RegexOptions.IgnoreCase;
            }

            return result;
        }

        private static IEnumerable<string> SearchFiles(Config config)
        {
            var enumerator = new FileEnumerator(new NativeFileSystem());
            var filter = new SearchFilter()
                .IncludeDirectories(config.Search.IncludedDirectories)
                .IncludeExtensions(config.Search.IncludedExtensions)
                .ExcludeDirectories(config.Search.ExcludedDirectories)
                .ExcludeExtensions(config.Search.ExcludedExtensions);
            return enumerator.FindFiles(config.Search.RootDirectory, filter);
        }
    }
}