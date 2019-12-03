using System;

namespace Silkroad.ConsoleExtensions
{
    public class ConsoleRider
    {
        public static void Start()
        {
            while (true)
            {
                Console.Write("> ");
                
                var line = Console.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    continue;
                }
                
                // TODO: Implement
            }
        }
    }
}
