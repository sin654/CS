using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace EBookInvestigation
{
    class BookReader
    {
        private string _theBook;

        public void GetBook()
        {
            // Simulate getting a book from a URL
            using WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    _theBook = e.Result;
                    Console.WriteLine("Book downloaded successfully.");
                    GetStats();
                }
                else
                {
                    Console.WriteLine("Error downloading book: " + e.Error.Message);
                }
            };
            client.DownloadStringAsync(new Uri("https://www.gutenberg.org/files/98/98-0.txt"));
        }

        public void GetStats()
        {
            string[] words = _theBook.Split(new char[] { ' ', '\n', '\r', '\u000A',  ',', '.', ';', ':', '-', '?', '/'}, StringSplitOptions.RemoveEmptyEntries);

            string longestWord = string.Empty;
            string[] tenMostCommon = null;

            // Longest word
            Parallel.Invoke(() =>
            {
                longestWord = FindLongestWord(words);
            }, () =>
            {
                tenMostCommon = FindTenMostCommon(words);
            });

            Console.WriteLine($"Longest word {longestWord}");

            // 10 most common words
            Console.WriteLine("10 most common words:");
            foreach (var word in tenMostCommon)
            {
                Console.Write($"{word}, ");
            }
            Console.WriteLine();
        }

        private string FindLongestWord(string[] words)
        {
            var result = (from word in words
                         orderby word.Length descending
                         select word).FirstOrDefault();

            return result;
        }

        private string[] FindTenMostCommon(string[] words)
        {
            var result = from word in words
                         group word by word into g
                         orderby g.Count() descending
                         select g.Key;

            return result.Take(10).ToArray();
        }
    }
}
