using System;
using System.IO;
using System.Linq;

namespace sharp_objects
{
    class Program
    {
        static void Main(string[] args)
        {
            string operation = null;
            string source = null;
            string destination = null;
            if (args.Length < 1 || args.Length > 3) {
                PrintUsage();
                return;
            }
            else if (args.Length == 3) {
                operation = args[0];
                source = args[1];
                destination = args[2];
            }
            else if (args.Length == 2) {
                if (new[] { "encrypt", "decrypt" }.Any(x => x == args[0])) {
                    operation = args[0];
                    source = args[1];
                }
                else {
                    source = args[0];
                    destination = args[1];
                }
            }
            else {
                source = args[0];
            }

            operation = operation ?? "encrypt";
            FileInfo sourceFile = new FileInfo(source);

            Stream output = null;
            if (destination == null) {
                output = Console.OpenStandardOutput();
            }
            else {
                FileInfo destFile = new FileInfo(destination);
                output = destFile.OpenWrite();
            }

            Console.WriteLine($"We will {operation.ToUpper()} the file {source} and output to {destination ?? "stdout"}");
        }

        static void PrintUsage() {
            Console.WriteLine("Usage: dotnet run [operation] <source> [destination]");
            Console.WriteLine();
            Console.WriteLine("operation:  'encrypt' or 'decrypt' (defaults to encrypt)");
            Console.WriteLine("source:  path to source file");
            Console.WriteLine("destination:  path to output (defaults to stdout)");
        }
    }
}
