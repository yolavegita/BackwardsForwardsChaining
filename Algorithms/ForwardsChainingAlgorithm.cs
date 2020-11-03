using BackwardsForwardsChaining.Abstractions;
using BackwardsForwardsChaining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackwardsForwardsChaining.Algorithms
{
    public class ForwardsChainingAlgorithm : IAlgorithm
    {
        //Services
        private readonly ILoggerService _logger;

        //Input
        private readonly IEnumerable<char> _facts;
        private readonly IEnumerable<Rule> _rules;
        private readonly char _goal;

        //Internal variables
        private List<char> _doesntHave = new List<char>();
        private List<Rule> _productions = new List<Rule>();

        public ForwardsChainingAlgorithm(IEnumerable<Rule> rules, IEnumerable<char> facts, char goal, ILoggerService logger)
        {
            _facts = facts;
            _rules = rules;
            _goal = goal;
            _logger = logger;

            //Assign numbers to rules
            var i = 1;
            foreach(var rule in _rules)
            {
                rule.Number = $"R{i}";
                rule.NumberNumeric = i;
                i++;
            }
        }

        //Executes algorithm
        public void Execute()
        {
            _logger.WriteLine("PART 1. Data");
            _logger.WriteLine("");
            _logger.WriteLine("    1) Rules");
            
            foreach(var rule in _rules)
                _logger.WriteLine($"       {rule.Number}: {rule.ToString()}");

            _logger.WriteLine($"    2) Facts {string.Join(", ", _facts)}.");
            _logger.WriteLine($"    3) Goal {_goal}.");

            _logger.WriteLine("PART 2. Trace");
            _logger.WriteLine("");
            
            bool state = ExecuteInternal();
            _logger.WriteLine("");

            _logger.WriteLine("PART 3. Results");
            _logger.Write($"    1) Goal {_goal} ");

            if(state)
            {
                if(_productions.Any())
                    _logger.Write($"achieved.");
                else
                    _logger.Write($"in facts. Empty path.");
            }
            else
            {
                _logger.Write($"not achieved.");
            }

            _logger.WriteLine("");

            if(state && _productions.Any())
            {
                _logger.Write($"    1) Path {string.Join(", ", _productions.Select(x => x.Number))}.");
            }
        }

        private bool ExecuteInternal()
        {
            bool halt;
            int index = 0;
            int iteration = 0;
            var GDB = new List<char>(_facts);
            var goal = _goal;
            var rules = new List<Rule>(_rules);

            while (true)
            {
                if (GDB.Contains(goal))
                {
                    _logger.WriteLine("      Goal achieved.");
                    return true;
                }

                halt = true;
                _logger.WriteLine($"    ITERATION {iteration + 1}");
                iteration++;

                index = 0;

                foreach (var r in rules)
                {
                    index++;
                    if (GDB.Contains(goal))
                    {
                        _logger.WriteLine("      Goal achieved.");
                        return true;
                    }

                    if (r.Flag2)
                        _logger.WriteLine($"      {r.Number}:{r.ToString()} skip, because flag 2 raised.");
                    else
                    {
                        if (r.Flag1)
                            _logger.WriteLine($"      {r.Number}:{r.ToString()} skip, because flag 1 raised.");
                        else
                        {
                            if (GDB.Contains(r.RightSide))
                            {
                                r.Flag2 = true;
                                _logger.WriteLine($"      {r.Number}:{r.ToString()} not applied, because RHS in facts. Raise flag2.");
                            }
                            else
                            {
                                if (IsInRuleLeftSide(r.LeftSide, GDB))
                                {
                                    halt = !halt;
                                    GDB.Add(r.RightSide);
                                    r.Flag1 = true;

                                    var temp = GDB.Except(_facts).ToList();
                                    _logger.WriteLine($"      {r.Number}:{r.ToString()} apply. Raise flag1. Facts {string.Join(", ", _facts.ToList())} and {string.Join(", ", temp)}.");
                                    _productions.Add(r);
                                    break;
                                }
                                else
                                {

                                    foreach (char c in r.LeftSide)
                                        if (!GDB.Contains(c))
                                            _doesntHave.Add(c);

                                    _logger.WriteLine($"      {r.Number}:{r.ToString()} not applied, because of lacking {string.Join(", ", _doesntHave.ToList())}.");
                                    _doesntHave = new List<char>();
                                }
                            }
                        }
                    }
                }

                if (halt)
                    return false;
            }
        }

        private bool IsInRuleLeftSide(IEnumerable<char> facts, IEnumerable<char> GDB)
        {
            int cnt = 0;

            foreach (char c in facts)
            {
                if (GDB.Contains(c))
                {
                    cnt++;
                }
            }

            if (cnt == facts.Count())
                return true;
            else
                return false;
        }
    }
}
