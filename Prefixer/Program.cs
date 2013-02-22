using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prefixer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Arguments missing.");
                Console.WriteLine("Run from the command line as: Prefixer FILE_NAME [-p]");
                return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(args[0]))
                {
                    string expr = sr.ReadLine();
                    var prefixer = new Prefixer(expr);
                    string result;
                    if (args.Length == 1)
                    {
                        result = prefixer.Parse();
                    }
                    else if (args.Length == 2 && args[1] == "-r")
                    {
                        result = prefixer.Reduce();
                    }
                    else
                    {
                        Console.WriteLine("Unrecognized option.");
                        result = prefixer.Parse();
                    }
                    Console.WriteLine(expr + " becomes " + result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return;
        }

    }
}
