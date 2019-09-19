using System;
using System.Runtime.Serialization.Json;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace testJson
{
    class Program
    {

        static void Main(string[] args)
        {

            IConfigurationBuilder builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine("Game on...");

            //assign variables based on config file
            string inputpath = configuration["LogFilePath"];
            string archivepath = configuration["LogFileArchivePath"];
            string Outputfile = configuration["OutputFile"];
            string RefreshAll = configuration["RefreshAll"];
            string site = configuration["site"];
            string url = configuration["EndpointUrl"];
            string key = configuration["PrimaryKey"];


            //Loop through log file directory and pass each filename into parser
            string[] fileEntries = Directory.GetFiles(inputpath);
            int ictr = 0;
            foreach (string fileName in fileEntries)
            {
               string inputfile_in = inputpath + Path.GetFileName(fileName);   //file to read
                ictr++;
                Console.WriteLine("Processing file " + ictr.ToString() + " of " + fileEntries.Length.ToString());
                FileParser fp = new FileParser(inputfile_in, url, key, archivepath);

            }

            //Console.WriteLine(sr.ReadToEnd());
            Console.WriteLine("Done Processing Files");
            Console.ReadLine();

            //TODO: Add logic to copy log files to ADLS



        }
    }
}
