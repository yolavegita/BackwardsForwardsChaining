using BackwardsForwardsChaining.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackwardsForwardsChaining.Services
{
    /// <summary>
    /// Logger service for console
    /// </summary>
    public class ConsoleLoggerService : ILoggerService
    {

        public void Write(string str)
        {
            Console.Write(str);
        }

        public void Write(StringBuilder str)
        {
            Console.Write(str);
        }

        public void WriteLine(string str)
        {
            Console.WriteLine(str);
        }

        public void WriteLine(StringBuilder str)
        {
            Console.WriteLine(str);
        }

        public void Dispose() { }
    }
}
