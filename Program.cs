using BackwardsForwardsChaining.Abstractions;
using BackwardsForwardsChaining.Algorithms;
using BackwardsForwardsChaining.Models;
using BackwardsForwardsChaining.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackwardsForwardsChaining
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggerService logger = null;
            IAlgorithm algorithm = null;

            //Read file
            Console.WriteLine("Please provide a file name:");
            var fileName = Console.ReadLine();
            if (String.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Please provide a file name.");
                return;
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File {fileName} is not found.");
                Console.ReadKey();
                return;
            }

            var (rules, facts, goal) = ReadFromFile(fileName);

            //Choose output method
            Console.WriteLine("Please choose output method by typing:");
            Console.WriteLine("1. Console");
            Console.WriteLine("2. File");

            var outputSelection = Convert.ToChar(Console.ReadLine());
            if(outputSelection != '1' && outputSelection != '2')
            {
                Console.WriteLine($"{outputSelection} is not a valid output method. Please try again.");
                Console.ReadKey();
                return;
            }

            switch (outputSelection)
            {
                case '1':
                    logger = new ConsoleLoggerService();
                    break;
                case '2':
                    Console.WriteLine("Please provide output file name:");
                    var outputFileName = Console.ReadLine();
                    if (String.IsNullOrEmpty(outputFileName))
                    {
                        Console.WriteLine("Please provide a file name.");
                        return;
                    }

                    logger = new FileLoggerService(outputFileName);
                    break;
            }

            //Choose algorithm
            Console.WriteLine("Please choose the algorithm:");
            Console.WriteLine("1. Forwards chaining");
            Console.WriteLine("2. Backwards chaining");

            var algorithmSelection = Convert.ToChar(Console.ReadLine());
            if (algorithmSelection != '1' && outputSelection != '2')
            {
                Console.WriteLine($"{algorithmSelection} is not a valid output method. Please try again.");
                Console.ReadKey();
                return;
            }

            switch (algorithmSelection)
            {
                case '1':
                    algorithm = new ForwardsChainingAlgorithm(rules, facts, goal, logger);
                    break;
                case '2':
                    algorithm = new BackwardsChainingAlgorithm(rules, facts, goal, logger);
                    break;
            }

            //Execute
            algorithm.Execute();
            algorithm.Dispose();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Reads input from file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static (IEnumerable<Rule>, IEnumerable<char>, char) ReadFromFile(string fileName)
        {
            string line;

            var rules = new List<Rule>();
            var goal = '0';
            var facts = new List<char>();

            using (StreamReader file = new StreamReader(fileName))
            {
                //Skip 3 lines
                file.ReadLine();
                file.ReadLine();
                file.ReadLine();

                //Read rules
                while ((line = file.ReadLine()) != "")
                    rules.Add(new Rule()
                    {
                        LeftSide = line.Substring(1, line.Length - (line.Length - (line.IndexOf("//") - 1))).Trim().Split(' ').Select(x => x[0]),
                        RightSide = line[0]
                    });

                //Read facts
                file.ReadLine();
                line = file.ReadLine();
                facts = line.Split(' ').Select(x => x[0]).ToList();

                //Read goal
                file.ReadLine();
                file.ReadLine();
                line = file.ReadLine();
                goal = line[0];
            }

            return (rules, facts, goal);
        }
    }
}
