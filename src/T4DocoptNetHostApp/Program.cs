using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4DocoptNetHostApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var mainArgs = new MainArgs(args);
            if (mainArgs.CmdCommand)
                Console.WriteLine("First command");
        }
    }
}
