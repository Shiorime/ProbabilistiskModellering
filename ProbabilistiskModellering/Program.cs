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

                TrafficLightCommands trafficLights = new TrafficLightCommands(client);

                Console.WriteLine($"Traffic lights count: {trafficLights.GetIdCount().Content}");

                List<string> test = trafficLights.GetIdList().Content;
                List<string> test1 = trafficLights.GetControlledLanes("n0").Content;

                Console.WriteLine(test.ElementAt(0));
                Console.WriteLine(test1.ElementAt(0));

                Console.ReadLine();

                while (true)
                {
                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("0");
                    trafficLights.SetRedYellowGreenState("n0", "Grrrrrrrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("1");
                    trafficLights.SetRedYellowGreenState("n0", "rGrrrrrrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("2");
                    trafficLights.SetRedYellowGreenState("n0", "rrGrrrrrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("3");
                    trafficLights.SetRedYellowGreenState("n0", "rrrGrrrrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("4");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrGrrrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("5");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrrGrrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("6");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrrrGrrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("7");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrrrrGrrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("8");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrrrrrGrr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("9");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrrrrrrGr");
                    await Task.Delay(1000);

                    Console.WriteLine("Taking a step");
                    client.Control.SimStep();
                    Console.WriteLine("10");
                    trafficLights.SetRedYellowGreenState("n0", "rrrrrrrrrrG");
                    await Task.Delay(1000);

                    await Task.Delay(100);
                }

            });
        }
    }
}
