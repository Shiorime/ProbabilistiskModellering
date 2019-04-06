using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

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
                SimulationCommands simulation = new SimulationCommands(client);

                TrafficLightCommands trafficLights = new TrafficLightCommands(client);

                Console.WriteLine($"Traffic lights count: {trafficLights.GetIdCount().Content}");

                List<string> test = trafficLights.GetIdList().Content;
                List<string> test1 = trafficLights.GetControlledLanes("n0").Content;

                foreach (string nodes in test)
                {
                    Console.WriteLine(nodes);
                }
                foreach (string controlledLanes in test1)
                {
                    Console.WriteLine(controlledLanes);
                }

                Console.ReadLine();
                while (simulation.GetTime("yeet").Content < 3000)
                {
                    client.Control.SimStep();

                    trafficLights.SetRedYellowGreenState("n0", "GGGGGGGGGGGG");
                }
                
                
            });
        }
    }
}
