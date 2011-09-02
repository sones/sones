using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using sones.GraphDS.GraphDSRESTClient;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphQL.Result;

namespace SimpleImportBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Use: SimpleImportBenchmark.exe ${Schema GraphQL file path} ${Data XML file path} ${IsVERBOSE}");
                Console.WriteLine("E.G.: SimpleImportBenchmark.exe C:\\data\\schema.gql C:\\data\\text.xml True");
            }
            else
            {
                string schemaFilePath = args[0];
                string dataFilePath = args[1];
                string isVerbose = args[2];

                //Get the current path
                string execPath = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                int fileNameBeginsIdx = execPath.LastIndexOf("/");
                execPath = execPath.Substring(0, fileNameBeginsIdx);
                Uri execUri = new Uri(execPath);

                //Startup the sones GraphDB in a seperate process
                string sonesGraphDBStarter = execUri.LocalPath + Path.DirectorySeparatorChar + "sonesGraphDBStarter.exe";

                Console.WriteLine("===  Startup ===");
                Console.WriteLine("Starting: " + sonesGraphDBStarter);
                Process.Start(sonesGraphDBStarter);

                //Wait a moment to give the process a chance to get started -- So wait 10s
                for (int i = 0; i < 100; i++)
                {
                    Console.Write(".");
                    Thread.Sleep(100);
                }

                Console.WriteLine();

                //Read the configuration to determine the port, user name and password
                string sonesGraphDBStarterConfig = sonesGraphDBStarter + ".config";

                Dictionary<string, string> config = new Dictionary<string,string>();
                config.Add("userName",ReadConfig(sonesGraphDBStarterConfig, "Username"));
                config.Add("password",ReadConfig(sonesGraphDBStarterConfig, "Password"));
                config.Add("port",ReadConfig(sonesGraphDBStarterConfig, "ListeningPort"));


                Console.WriteLine("=== Configuration === \n" +
                                  "User: " + config["userName"] + "\n" +
                                  "Password: " + config["password"] + "\n" +
                                  "Port:" + config["port"]); 



                Console.WriteLine("=== Import ===");

                //Connect to the local sones GraphDB instance to start the import
                GraphDS_RESTClient client = new GraphDS_RESTClient("localhost", config["userName"], config["password"], UInt32.Parse(config["port"]));
                
                TransactionToken tx = null;
                SecurityToken sec = null;
                
                client.Query(sec, tx, "IMPORT FROM 'file:\\\\" + schemaFilePath + "' FORMAT gql", "sones.gql");

                string importQuery = null;

                if (isVerbose.Equals("True"))
                {
                    importQuery = "IMPORT FROM 'file:\\\\" + dataFilePath + "' FORMAT fastimport VERBOSITY full";
                }
                else
                {
                    importQuery = "IMPORT FROM 'file:\\\\" + dataFilePath + "' FORMAT fastimport";
                }

                Stopwatch sw = new Stopwatch();

                Console.WriteLine("Start: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                sw.Start();

                QueryResult rs = client.Query(sec, tx, importQuery, "sones.gql");

                sw.Stop();

                Console.WriteLine(rs.Error.Message);

                Console.WriteLine("Stop: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Elapsed: " + sw.ElapsedMilliseconds + " ms");
        }
        
        }


        private static String ReadConfig(string configFilePath, string settingName)
        {
            string result = null;

            XmlTextReader configReader = new XmlTextReader(configFilePath);

            while (configReader.Read())
            {
                if (configReader.Name.Equals("sones.sonesGraphDBStarter.Properties.Settings"))
                {
                    while (configReader.Read())
                    {
                        if (configReader.Name.Equals("setting"))
                        {
                            if (configReader.HasAttributes)
                            {
                                string attribute = configReader.GetAttribute("name");

                                if (attribute.Equals(settingName))
                                {
                                    while (configReader.Read())
                                    {
                                        if (configReader.Name.Equals("value"))
                                        {
                                            configReader.Read();
                                            result = configReader.ReadContentAsString();
                                            configReader.Close();

                                            return result;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            configReader.Close();
            return result;
        }

    }
}