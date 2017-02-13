using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReadIDMS
{
    internal class Program
    {
        class Station
        {
            public string Name { get; set; }
            public int MatchPriority { get; set; } = 0;
        }

        private static void Main(string[] args)
        {   
            try
            {
                var l = new List<string>() {
                    "London Kings Cross",
                    "Cross Hatch",
                    "Crossington"
                };

                var teststring = "cross";
                var pattern = $@"\b{teststring}";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);


                var result = (from s in l
                              where
                  Regex.Matches(s, pattern, RegexOptions.IgnoreCase).Count > 0
                              select new
                              {

                              });

                var matchlist = l.Where(x => Regex.Matches(x, pattern, RegexOptions.IgnoreCase).Count > 0);

                foreach (var m in matchlist)
                {
                    Console.WriteLine($"{m}");
                }

                Environment.Exit(-1);





                var fareFilename = @"s:\FareLocationsRefData.xml";
                XDocument faredoc = XDocument.Load(fareFilename);
                var stationFilename = @"s:\StationsRefData.xml";
                XDocument stationdoc = XDocument.Load(stationFilename);

                var stationq = (from station in stationdoc.Element("StationsReferenceData").Elements("Station")
                                where (string)station.Element("UnattendedTIS") == "true" &&
                                !string.IsNullOrWhiteSpace((string)station.Element("CRS")) &&
                                (string)station.Element("OJPEnabled") == "true"
                                join fare in faredoc.Element("FareLocationsReferenceData").Elements("FareLocation")
                                on (string)station.Element("Nlc") equals (string)fare.Element("Nlc")
                                where (string) fare.Element("UnattendedTIS") == "true"
                                select new
                                {
                                    crs = (string)station.Element("CRS"),
                                    nlc = (string)fare.Element("Nlc"),
                                    name = (string)fare.Element("OJPDisplayName"),
                                    unattendedTis = (bool)fare.Element("UnattendedTIS")
                                });

                var stationlist = stationq.Distinct();


                foreach (var p in stationlist)
                {
                    Console.WriteLine($"{p.crs} {p.nlc} {p.name}");
                }

            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
