using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
                

                

                while (true)

                    while (simulation.GetTime("yeet").Content < 3000)

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

                        await Task.Delay(1000);

                        trafficLights.SetRedYellowGreenState("n0", "GGGGGGGGGGGG");
                        await Task.Delay(1000);

                        Console.WriteLine($"Time: {simulation.GetTime("yeet").Content}");

                    }

            });
        }

        // has been tested in seperate project, we will have to test if cmd.WaitForExit() causes conflict with the ending simulation
        public void OpenSumo(string portNumber)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            //port number is passed as argument, thus enabling sumo to be opened as many times as wanted 
            cmd.StandardInput.WriteLine($"sumo-gui --remote-port {portNumber} -c cfg.sumocfg");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
    }
}
