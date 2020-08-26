using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace House_Size
{
    public class State
    {
        public string Name { get; set; }
        public int Pop { get; set; }
        public string Ab { get; set; }
        public int Reps { get; set; }
        public double standardQuota { get; set; }
        public int lowerQuota { get; set; }
        public int upperQuota { get; set; }

    }

    class Program
    {
        static void print(IEnumerable<State> records)
        {
            foreach (var state in records)
            {
                Console.WriteLine(state.Ab + ": " + state.Reps + " (" + state.Name + ")");
            }
        }

        static void huntingtonHillMethod(string path, IEnumerable<State> record, List<State> records, int size)
        {
            int remainingSeats = size;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                record = csv.GetRecords<State>();
                records = record.ToList<State>();
                int total = 0;
                for (int i = 0; i < records.Count; i++)
                {
                    total += records[i].Pop;
                }
                var standardDivisor = total / size;
                for (int i = 0; i < records.Count; i++)
                {
                    records[i].standardQuota = records[i].Pop / standardDivisor;
                    records[i].lowerQuota = (int)Math.Floor(records[i].standardQuota);
                    records[i].upperQuota = (int)Math.Ceiling(records[i].standardQuota);
                    records[i].Reps = 1;
                    remainingSeats -= 1;
                }

                while (remainingSeats > 0)
                {
                    double max = 0;
                    int maxIndex = -1;
                    for (int i = 0; i < records.Count; i++)
                    {
                        var numSeats = records[i].Reps;
                        var div = records[i].Pop / Math.Sqrt(numSeats * (numSeats + 1));
                        if (div > max)
                        {
                            max = div;
                            maxIndex = i;
                        }
                    }
                    records[maxIndex].Reps++;
                    remainingSeats--;
                }
            }
            print(records);
        }

        static void currentRule(int num)
        {
            IEnumerable<State> record = null;
            List<State> records = null;
            int size = 0;
            Console.WriteLine("Please enter a size for the house, greater than the number of states. Current is 435.");
            string s = Console.ReadLine();
            Console.WriteLine("\n");
            size = Int32.Parse(s);
            string path = null;
            if (num == 1)
            {
                path = "pop50.csv";
                
            }
            if (num == 2)
            {
                path = "popDC.csv";
            }
            if (num == 3)
            {
                path = "popPR.csv";
            }
            if (num == 4)
            {
                path = "popDCPR.csv";
            }
            if (num == 5)
            {
                path = "popAll.csv";
            }

            huntingtonHillMethod(path, record, records, size);
            if (num == 5)
            {
                Console.WriteLine("\nTerritories:");
                Console.WriteLine("Bermuda");
                Console.WriteLine("Greenland");
                Console.WriteLine("Labrador (Includes northern Quebec)");
                Console.WriteLine("Borealia");
                Console.WriteLine("Yukon");
                Console.WriteLine("Nunavut");
            }
        }

        static void Main(string[] args)
        {       
            int check = 0;
            
            while (check == 0)
            {
                string option = "0";
                Console.WriteLine("0. To quit\n1. For 50 States\n2. For 50 States + DC\n3. For 50 States + PR\n4. For 50 States + DC + PR\n5. For hypothetical American Empire");
                option = Console.ReadLine();
                int numOption = Int32.Parse(option);
                if (numOption < 1 || numOption > 5)
                {
                    check = 1;
                }
                else
                {
                    currentRule(numOption);
                }
                Console.WriteLine("\n");
            }
        }
    }
}
