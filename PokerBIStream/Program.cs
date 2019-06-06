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
            //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);

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

          
            //int isGame = 0;
            Games g = new Games();  //TODO: remove later. this is only so code below doesn't fail


            //Loop through log file directory and pass each filename into parser
            string[] fileEntries = Directory.GetFiles(inputpath);
            foreach (string fileName in fileEntries)
            {
               string inputfile_in = inputpath + Path.GetFileName(fileName);   //file to read

                FileParser fp = new FileParser(inputfile_in, url, key);


            }


            
            //Console.WriteLine(sr.ReadToEnd());

            Console.ReadLine();



        }
    }
}
