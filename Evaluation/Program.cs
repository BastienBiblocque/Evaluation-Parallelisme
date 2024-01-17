using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

class Program
{
    static async Task Main()
    {
        Stopwatch stopwatchTotal = Stopwatch.StartNew();

        Task<int> additionTask = AddNumbersAsync(1, 3000);


        string filePath1 = "Eval_file1.txt";
        string filePath2 = "Eval_file2.txt";

        Task<int> countWordsFile1 = CountWords(filePath1);
        Task<int> countWordsFile2 = CountWords(filePath2);

        Task<int> countLorem1 = CountLorem(filePath1);
        Task<int> countLorem2 = CountLorem(filePath2);

        await additionTask;

        await Task.WhenAll(countWordsFile1, countWordsFile2, countLorem1, countLorem2);

        Console.WriteLine($"La somme des nombres de 1 à 3000 est : {additionTask.Result}");

        Console.WriteLine($"Le nombre de mot dans le fichier 1 est : {countWordsFile1.Result}");
        Console.WriteLine($"Le nombre de mot dans le fichier 2 est : {countWordsFile2.Result}");

        Console.WriteLine($"Le nombre de Lorem Ipsum dans le fichier 1 est : {countLorem1.Result}");
        Console.WriteLine($"Le nombre de Lorem Ipsum dans le fichier 2 est : {countLorem2.Result}");

        Console.WriteLine($"Total : {additionTask.Result + countWordsFile1.Result + countWordsFile2.Result + countLorem1.Result + countLorem2.Result}");

        stopwatchTotal.Stop();
        Console.WriteLine($"Temps total d'exécution : {stopwatchTotal.ElapsedMilliseconds} ms");
    }

    static async Task<int> AddNumbersAsync(int start, int end)
    {
        return await Task.Run(() =>
        {
            int sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += i;
            }
            return sum;
        });
    }

    static async Task<int> CountWords(string filePath)
    {
        string content = await File.ReadAllTextAsync(filePath);
        string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        int wordCount = 0;

        Parallel.ForEach(lines, line =>
        {
            string[] words = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            Interlocked.Add(ref wordCount, words.Length);
        });

        return wordCount;
    }

    static async Task<int> CountLorem(string filePath)
    {
        string content = await File.ReadAllTextAsync(filePath);
        string[] lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        int count = 0;

        string pattern = @"\bLorem Ipsum\b";
        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        Parallel.ForEach(lines, line =>
        {
            int lineLoremCount = regex.Matches(line).Count;
            Interlocked.Add(ref count, lineLoremCount);
        });

        return count;
    }
}
