using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reapportionment
{
    public class State
    {
        public string Name { get; set; }
        public int Pop { get; set; }
        public string Ab { get; set; }
        public int Reps { get; set; }
        public double StandardQuota { get; set; }
        public int LowerQuota { get; set; }
        public int UpperQuota { get; set; }
        public int CurrentReps { get; set; }

    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            optionDropdown.Items.Add("50 States");
            optionDropdown.Items.Add("50 States + DC");
            optionDropdown.Items.Add("50 States + PR");
            optionDropdown.Items.Add("50 States + DC + PR");
            dateDropdown.Items.Add("2020");
            dateDropdown.Items.Add("2010");
            dateDropdown.Items.Add("2000");
            textBox.IsReadOnly = true;
            info.IsReadOnly = true;
        }

        private void HuntingtonHillMethod(string path, IEnumerable<State> record, List<State> records, int size)
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
                    records[i].StandardQuota = records[i].Pop / standardDivisor;
                    records[i].LowerQuota = (int)Math.Floor(records[i].StandardQuota);
                    records[i].UpperQuota = (int)Math.Ceiling(records[i].StandardQuota);
                    records[i].Reps = 1;
                    remainingSeats--;
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
            Print(records);
        }
        private void Print(IEnumerable<State> records)
        {
            textBox.Text = "";
            //textBox.Text = string.Format("{0,-30} {1,-20} {2,-20}\n", "State:", "Current Reps:", "Future Reps:");
            textBox.Text = "State:\t\tCurrent Reps:\tFuture Reps:\tState Name:\n";
            foreach (var state in records)
            {
                //textBox.Text += string.Format("{0,-30} {1,-20} {2,-20}\n", state.Name, state.CurrentReps, state.Reps);
                textBox.Text += state.Ab + "\t\t" + state.CurrentReps + "\t\t" + state.Reps + "\t\t" + state.Name + "\n";
            }
        }

        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = 0;
                IEnumerable<State> record = null;
                List<State> records = null;
                string path = "";

                string date = dateDropdown.SelectedItem.ToString();

                if (optionDropdown.SelectedItem.ToString() == "50 States")
                {
                    path = "CSV/pop50" + date + ".csv";
                }
                else if (optionDropdown.SelectedItem.ToString() == "50 States + DC")
                {
                    path = "CSV/popDC" + date + ".csv";
                }
                else if (optionDropdown.SelectedItem.ToString() == "50 States + PR")
                {
                    path = "CSV/popPR" + date + ".csv";
                }
                else
                {
                    path = "CSV/popDCPR" + date + ".csv";
                }
                if (!int.TryParse(sizeBox.Text, out size))
                {
                    size = 435;
                }
                HuntingtonHillMethod(path, record, records, size);
            }
            catch(NullReferenceException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
