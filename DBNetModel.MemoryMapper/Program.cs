using System;
using System.IO;
using System.Linq;
using DBNetModel.DNA;
using JetBrains.Annotations;

namespace DBNetModel.MemoryMapper
{
    [UsedImplicitly]
    internal class Program
    {
        private const int MaxAddress = 1000;
        private const string OutputFile = "memory-map.md";

        private static string GetNames(int address)
        {
            var vars = Tokenizer.SystemVariables
                .Where(v => v.Address == address)
                .Select(v => v.Name)
                .ToList();

            return vars.Any() ? string.Join(", ", vars) : "Empty";
        }

        private static void Main()
        {
            var outputWriter = new StreamWriter("memory-map.md", false);
            var maxValue = Tokenizer.SystemVariables.Max(v => v.Address);
            var inUse = Tokenizer.SystemVariables.Select(v => v.Address).Distinct().Count();

            Console.WriteLine("DB .Net Memory Map");
            Console.WriteLine("=======================================");
            Console.WriteLine($"Output File            : {OutputFile}");
            Console.WriteLine($"Maximum Address In Use : {maxValue}");
            Console.WriteLine($"Maximum Address        : {MaxAddress}");
            Console.WriteLine($"Addresses in Use       : {inUse}");

            outputWriter.WriteLine("# Memory Map");
            outputWriter.WriteLine();

            outputWriter.WriteLine("This is the current memory allocation:");
            outputWriter.WriteLine();

            outputWriter.Write("xxx | 0-99");
            for (var i = 1; i * 100 < MaxAddress; i++)
            {
                outputWriter.Write($" | {i * 100}-{i * 100 + 99}");
            }

            outputWriter.WriteLine();

            outputWriter.Write("--- | ----");
            for (var i = 1; i * 100 < MaxAddress; i++)
            {
                var len = $"{i * 100}-{i * 100 + 99}".Length;
                outputWriter.Write($" | {new string('-', len)}");
            }

            outputWriter.WriteLine();

            for (var i = 0; i < 100; i++)
            {
                outputWriter.Write($"x{i:00} | ");
                outputWriter.Write(GetNames(i));
                for (var j = 1; j * 100 < MaxAddress; j++)
                {
                    outputWriter.Write($" | {GetNames(j * 100 + i)}");
                }
                outputWriter.WriteLine();
            }

            outputWriter.WriteLine();

            outputWriter.Close();

            Console.WriteLine();
            Console.Write("Press enter to close...");
            Console.ReadLine();
        }
    }
}