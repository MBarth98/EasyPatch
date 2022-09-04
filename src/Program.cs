namespace Global;

using System;

using Global.Patcher;

public class Program
{

    public static void Main(string[] args)
    {
        Console.WriteLine($"Program args [{(args.Length > 0 ? args[1..args.Length].Aggregate(args[0], (a, b) => $"{a}, {b}") : "none")}]");


        var update1 = Writer.CreatePatch("tests/resources/text/plain/v1/", "tests/resources/text/plain/v2/");
        
        if (update1.result.HasValue)
        {
            foreach (var a in update1.result.Value.sections)
            {
                Console.WriteLine(a.ToString());
            }
        }
//
        //var update2 = Writer.CreatePatch("tests/resources/text/plain2.txt", "tests/resources/text/plain3.txt");


          /*
                _old -> 104 195 166 108 108 111 
                _mid -> 104 101 108 108 111 
                _end -> 104 101 108 108 111 32 119 111 114 108 100 33

            _old_mid -> ... 101 108 ... 111
            _mid_end -> ... ... ... ... ... 32 119 111 114 108 100 33 
            _old_end -> ... 101 108 ... 111 32 119 111 114 108 100 33            

            _old_mid: 
                > offset 1 = 101 108
                > offset 4 = 111

            _mid_end:
                > offset 5 = 32 119 111 114 108 100 33

            _old_end:
                > offset 1 = 101 108
                > offset 4 = 111 32 119 111 114 108 100 33 
        */
    }
}
