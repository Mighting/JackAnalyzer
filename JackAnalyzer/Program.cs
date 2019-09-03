using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JackAnalyzer
{
    class Program
    {
        public static string[] splitPath;
        static void Main(string[] args)
        {
            string[] fileToRead = Directory.GetFiles(Console.ReadLine(), "*.jack");
            splitPath = fileToRead[0].Split('\\');
            string fileToWrite = splitPath[splitPath.Length - 2];

            for (int i = 0; i < fileToRead.Length; i++)
            {
                CompilationEngine compilationEngine = new CompilationEngine(fileToRead,fileToWrite);
                compilationEngine.compileClass();
            }
        }
    }
}
