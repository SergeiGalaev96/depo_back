using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Depository.RateExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
           
            string strConnection = builder.GetSection("Link").Value;

            Console.WriteLine(strConnection);
        }
    }
}
