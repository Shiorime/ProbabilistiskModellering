using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingConnected.TraCI.NET;

namespace ProbabilistiskModellering
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            await Task.Run(async () =>
            {
                var client = new TraCIClient();
                Console.Write("Insert Port Number: ");
                int port = int.Parse(Console.ReadLine());
                await client.ConnectAsync("127.0.0.1", port);
                
                while (true)
                {
                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    //await Task.Delay(1);
                }
            });
        }
    }
}
