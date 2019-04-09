using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;
using System.Globalization;

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
                Program pg = new Program();
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
                
                while (simulation.GetTime("yeet").Content < 3000)
                {
                    client.Control.SimStep();
                    trafficLights.SetRedYellowGreenState("n0", "GGGGGGGGGGGG");

                }
                client.Control.Close();

                await Task.Delay(100);
                string[] timeLoss = pg.GetSpecificXMLAttributeFromFile("timeLoss", @"C:\Users\Lasse\Documents\GitHub\ProbabilistiskModellering\SUMOFiles\out.xml");
                int cars = timeLoss.Count();
                double eo = 0.0;
                for (int i = 0; i < cars; i++)
                {
                    eo += double.Parse(timeLoss[i], CultureInfo.InvariantCulture);
                }

                Console.WriteLine($"{eo / cars}");
                Console.ReadLine();
            });
        }

        // has been tested in seperate project, we will have to test if cmd.WaitForExit() causes conflict with the ending simulation
        public void OpenSumo(string portNumber, string outputFile)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            //port number is passed as argument, thus enabling sumo to be opened as many times as wanted 
            cmd.StandardInput.WriteLine($"sumo --remote-port {portNumber} -c cfg.sumocfg -W true --tripinfo-output out.xml");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }

        public string[] GetSpecificXMLAttributeFromFile(string attribute, string filePath)
        {
            //https://stackoverflow.com/questions/933687/read-xml-attribute-using-xmldocument
            XmlDocument xmlDoc = new XmlDocument(); /* Create new XmlDocument */
            xmlDoc.Load(filePath);                  /* Load the xml file from filePath */
            XmlNodeList list = xmlDoc.GetElementsByTagName("tripinfo"); /* Find elements with interval. Put it into an array/list */
            string[] finalArray = new string[list.Count];

            for(int i = 0; i < list.Count; ++i)
            {
                finalArray[i] = list[i].Attributes[$"{attribute}"].Value;
            }

            return finalArray;
        }
    }

}
