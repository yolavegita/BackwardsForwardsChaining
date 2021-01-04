using BackwardsForwardsChaining.Abstractions;
using BackwardsForwardsChaining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackwardsForwardsChaining.Algorithms
{
    public class BackwardsChainingAlgorithm : IAlgorithm
    {
        //Services
        private readonly ILoggerService _logger;

        //Input
        private readonly IEnumerable<char> _facts;
        private readonly IEnumerable<Rule> _rules;
        private readonly char _goal;

        //Internal variables
        private List<char> _derivedFacts = new List<char>();
        private List<Rule> _productions = new List<Rule>();
        private List<char> _goals = new List<char>();
        private int _level;
        private int _counter;

        public BackwardsChainingAlgorithm(IEnumerable<Rule> rules, IEnumerable<char> facts, char goal, ILoggerService logger)
        {
            _facts = facts;
            _rules = rules;
            _goal = goal;
            _logger = logger;

            //Assign numbers to rules
            var i = 1;
            foreach (var rule in _rules)
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

            foreach (var rule in _rules)
                _logger.WriteLine($"       {rule.Number}: {rule.ToString()}");

            _logger.WriteLine($"    2) Facts {string.Join(", ", _facts)}.");
            _logger.WriteLine($"    3) Goal {_goal}.");
            _logger.WriteLine("");

            _logger.WriteLine("PART 2. Trace");
            _logger.WriteLine("");

            _level = 0;
            _counter = 0;

            bool state = ExecuteInternal(_goal);
            _logger.WriteLine("");

            _logger.WriteLine("PART 3. Results");
            _logger.Write($"    1) Goal {_goal} ");

            if (state)
            {
                if (_productions.Any())
                    _logger.Write($"achieved.");
                else
                    _logger.Write($"in facts. Empty path.");
            }
            else
            {
                _logger.Write($"not achieved.");
            }

            _logger.WriteLine("");

            if (state && _productions.Any())
                _logger.WriteLine($"    1) Path {string.Join(", ", _productions.Select(x => x.Number))}.");
        }

        bool initial;
        List<char> ongoingGoals = new List<char>();

        /// <summary>
        /// Internal BC recursive algorithm
        /// </summary>
        /// <param name="goal">Goal</param>
        /// <returns>True if goal achieved, false if not</returns>
        private bool ExecuteInternal(char goal)
        {
            initial = false;

            if (!_goals.Contains(goal))
            {
                if (_facts.Contains(goal))
                {
                    _logger.Write($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. Fact (initial), as facts are {string.Join(", ", _facts)}.");
                    _level--;
                    _counter++;
                    initial = true;

                    if (_goal != goal)
                        _logger.Write(" Back, OK.");
                    else
                        _logger.Write(" OK.");

                    _logger.WriteLine("");

                    return true;
                }
                else if (_derivedFacts.Contains(goal))
                {
                    _logger.Write($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. Fact (earlier inferred), as facts {string.Join(", ", _facts)} and {string.Join(", ", _derivedFacts)}.");
                    _level--;
                    _counter++;

                    if (_goal != goal)
                        _logger.Write(" Back, OK.");
                    else
                        _logger.Write(" OK.");

                    _logger.WriteLine("");

                    return true;
                }
                else
                {
                    _goals.Add(goal);
                    foreach (var rule in _rules)
                    {
                        if (rule.RightSide.Equals(goal))
                        {
                            _logger.WriteLine($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. Find {rule.Number}:{rule.ToString()}. New goals {string.Join(",", rule.LeftSide)}.");
                            ongoingGoals.AddRange(rule.LeftSide);
                            _counter++;
                            _level++;

                            bool usable = true;
                            List<char> temp_derived_facts = new List<char>();
                            List<Rule> temp_productions = new List<Rule>();
                            temp_derived_facts.AddRange(_derivedFacts);
                            temp_productions.AddRange(_productions);

                            foreach (char fact in rule.LeftSide)
                            {
                                if (!ExecuteInternal(fact))
                                {
                                    usable = false;
                                    _derivedFacts = temp_derived_facts;
                                    _productions = temp_productions;
                                    break;
                                }
                            }

                            if (usable)
                            {
                                _derivedFacts.Add(rule.RightSide);
                                _productions.Add(rule);
                                _goals.Remove(goal);
                                ongoingGoals.Remove(goal);
                                _logger.Write($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. Fact (presently inferred). Facts {string.Join(", ", _facts)} and {string.Join(", ", _derivedFacts)}.");

                                if (_goal != goal)
                                {
                                    //TODO: Sample 2 - C buvo anksciau, tai nereikia back-tracking.
                                    //TODO: Reikia prideti salyga (apgalvoti).

                                    if (!initial)
                                    {
                                        _level--;
                                        _logger.Write(" Back, OK.");
                                    }
                                    else
                                        initial = false;
                                }
                                else
                                    _logger.Write(" OK.");

                                _logger.WriteLine("");
                                _counter++;
                                return true;
                            }
                        }
                    }

                    if(!_rules.Any(x => x.RightSide == goal))
                        _logger.WriteLine($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. No rules. Back, FAIL.");
                    else
                        _logger.WriteLine($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. No more rules. Back, FAIL.");

                    _level--;
                    _counter++;
                    _goals.Remove(goal);
                    ongoingGoals.Remove(goal);
                    return false;
                }
            }
            else
            {
                _logger.WriteLine($"  {string.Format("{0, 2}", _counter + 1)}) {RepeatSymbol('-', _level)}Goal {goal}. Cycle. Back, FAIL.");
                _level--;
                _counter++;
                return false;
            }
        }

        /// <summary>
        /// Repeats the symbol amount of times and forms a string
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="amount">Amount of times to repeat symbol</param>
        /// <returns>String composed of symbol repeated amount of times</returns>
        private string RepeatSymbol(char symbol, int amount)
        {
            var str = new StringBuilder();
            for (int i = 0; i < amount; i++)
                str.Append(symbol);

            return str.ToString();
        }

        public void Dispose()
        {
            _logger?.Dispose();
        }
    }
}
