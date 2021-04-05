namespace T4DocoptNetHostApp
{
    using System;

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
