using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using KeywordSuggestor.Bing;

namespace KeywordSuggestor
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length == 0)
            //{
            //    Console.WriteLine("Correct Usage : KeywordSuggestor.exe filename.txt");
            //}
            
            //var fileName = args[0];
            const string Filename = @"C:\newsitem1.txt";
            const long totalNumberOfdocs = 50000000000; // Fifty Billion
            var searchEngine = new BingSearch();
            //var result = searchEngine.Search("google");
            var contents = File.ReadAllText(Filename);
            contents = contents.Replace("\r", "").Replace("\n", "");
            var exludedWords = File.ReadAllText("ExcludedWords.txt")
                .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var allwords = contents.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var distinctWords = allwords.Distinct().Select(x => x.ToLower());
            var filteredWords = distinctWords.Where(x => !exludedWords.Contains(x.ToLower())).ToList();
            var dictionaryHits = new Dictionary<string, long>();
            var options = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            Parallel.ForEach(filteredWords, options,
                word =>
                {
                    var result = searchEngine.Search(word);
                    dictionaryHits.Add(result.SearchTerm, result.Total);
                });

            var dictTFIDF = new Dictionary<string, double>();
            Parallel.ForEach(dictionaryHits, options,
                element =>
                {
                    var tf = allwords.Count(x => x.ToLower() == element.Key);
                    var idf = Math.Log((totalNumberOfdocs / element.Value), 2);
                    var tf_idf = tf * idf;
                    dictTFIDF.Add(element.Key, tf_idf);
                });

            foreach (var element in dictTFIDF)
            {
                Console.WriteLine(string.Format("{0:25}{1}", element.Key, element.Value));
            }

            Console.ReadKey();
        }
    }
}
