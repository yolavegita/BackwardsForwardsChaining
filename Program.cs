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

            //Execute
            var logger = new ConsoleLoggerService();
            IAlgorithm algorithm = new ForwardsChainingAlgorithm(rules, facts, goal, logger);
            algorithm.Execute();

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
