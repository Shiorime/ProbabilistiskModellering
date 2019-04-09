using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Commands;

namespace ProbabilistiskModellering
{
    delegate double FitnessFunction(Program pg, string attribute, string filePath);
    class Program 
    {
        Random random = new Random();

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            await Task.Run(async () =>
            {
                Program pg = new Program();

                TraCIClient client = new TraCIClient();
                SimulationCommands simulation = new SimulationCommands(client);
                TrafficLightCommands trafficLights = new TrafficLightCommands(client);
                Func<string> RedGreen;
                Action<string> write = Console.WriteLine;

                RedGreen = pg.GenerateRandomRedYellowGreenState;

                string sumoOutputFilePath = "out.xml";
                DNA<string> genome = new DNA<string>(3600, RedGreen, true);

                for (int i = 0; i < genome.genes.Length; i++)
                    write(genome.genes[i]);

                pg.OpenSumo(1000, "out.xml");

                Console.Write("Insert Port Number: ");
                int port = int.Parse(Console.ReadLine());
                await client.ConnectAsync("127.0.0.1", port);

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

                genome.FitnessFunction = 0;
                //write(pg.CalculateFitnessFunction(pg, "timeLoss", sumoOutputFilePath).ToString());

                Console.ReadLine();
            });
        }

        public string GenerateRandomRedYellowGreenState()
        {
            string randomState = string.Empty;
            int result = 0;
            for (int i = 0; i < 11; i++)
            {
                result = random.Next(0, 2);
                if (result == 0)
                {
                    randomState = randomState + "r";
                }
                else if (result == 1)
                {
                    randomState = randomState + "G";
                }
            }
            return randomState;
        }

        public double CalculateFitnessFunction(Program pg, string attribute, string filePath)
        {
            string[] timeLossArray = pg.GetSpecificXMLAttributeFromFile(attribute, filePath);
            int cars = timeLossArray.Count();
            double timeLossSum = 0.0;
            for (int i = 0; i < cars; i++)
            {
                timeLossSum += double.Parse(timeLossArray[i], CultureInfo.InvariantCulture);
            }
            return timeLossSum;
        }

        // has been tested in seperate project, we will have to test if cmd.WaitForExit() causes conflict with the ending simulation
        public void OpenSumo(int portNumber, string outputFile)
        {
            string yeet = $"sumo --remote-port {portNumber} -c cfg.sumocfg -W true --tripinfo-output {outputFile}";
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            //port number is passed as argument, thus enabling sumo to be opened as many times as wanted 
            cmd.StandardInput.WriteLine(yeet);
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
