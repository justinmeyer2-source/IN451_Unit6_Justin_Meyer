using System;
using System.IO;

namespace IN451_Unit6_Justin_Meyer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Clothing Store Dressing Room Simulation");
            Console.WriteLine("=======================================");
            Console.WriteLine();

            var s1 = Scenario.Scenario01();
            var s2 = Scenario.Scenario02();
            var s3 = Scenario.Scenario03();

            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
