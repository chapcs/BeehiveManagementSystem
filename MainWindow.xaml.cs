using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using BeehiveManagementSystem;

namespace BeehiveManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queen queen = new Queen();
        public MainWindow()
        {
            InitializeComponent();
            statusReport.Text = queen.StatusReport;
        }

        private void WorkShift_Click(object sender, RoutedEventArgs e)
        {
            queen.WorkTheNextShift();
            statusReport.Text = queen.StatusReport;
        }

        private void AssignJob_Click(object sender, RoutedEventArgs e)
        {
            queen.AssignBee(jobSelector.Text);
            statusReport.Text = queen.StatusReport;
        }
    }

    static class HoneyVault
    {
        public const float NECTAR_CONVERSION_RATIO = .19f;
        public const float LOW_LEVEL_WARNING = 10f;

        private static float honey = 25f;
        private static float nectar = 100f;

        public static string StatusReport
        {
            get
            {
                string warning = "";
                if (honey < LOW_LEVEL_WARNING)
                    warning += "LOW HONEY - ADD MANUFACTURER\n";
                else if (nectar < LOW_LEVEL_WARNING)
                    warning += "LOW NECTAR - ADD COLLECTOR\n";
                return $"Honey: {honey:0.0}\nNectar: {nectar:0.0}\n" + warning;
            }
        }

        public static void ConvertNectarToHoney(float amount)
        {
            if (amount > nectar)
                amount = nectar;
            nectar -= amount;
            honey += (amount * NECTAR_CONVERSION_RATIO);
        }

        public static bool ConsumeHoney(float amount)
        {
            if (honey >= amount)
            {
                honey -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CollectNectar(float amount)
        {
            if (amount > 0f)
                nectar += amount;
        }
    }

    public class Bee
    {
        public virtual float CostPerShift { get; }
        public string Job { get; private set; }
        public Bee(string job)
        {
            Job = job;
        }

        public void WorkTheNextShift()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
                DoJob();
        }

        protected virtual void DoJob() { }
    }

    public class Queen : Bee
    {
        public const float EGGS_PER_SHIFT = 0.45f;
        public const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;

        //forgot to generate the array here with Bee[0]
        private Bee[] workers = new Bee[0]; 
        public float eggs = 0;
        public float unassignedWorkers = 3;

        public string StatusReport { get; private set; }
        public override float CostPerShift { get { return 2.15f; } }

        public Queen() : base("Queen")
        {
            AssignBee("Honey Manufacturer");
            AssignBee("Nectar Collector");
            AssignBee("Egg Care");
        }

        //forgot this method almost entirely and put it in the Bee class
        private void AddWorker(Bee worker) 
        {
            if (unassignedWorkers >= 1)
            {
                unassignedWorkers--;
                Array.Resize(ref workers, workers.Length + 1);
                workers[workers.Length - 1] = worker;
            }
        }

        public void AssignBee(string job)
        {
            switch (job)
            {
                case "Honey Manufacturer":
                    AddWorker(new HoneyManufacturer());
                    break;
                case "Nectar Collector":
                    AddWorker(new NectarCollector());
                    break;
                case "Egg Care":
                    AddWorker(new EggCare(this));
                    break;
                default:
                    Console.WriteLine("This should never be called");
                    break;
            }
            UpdateStatusReport();
        }

        protected override void DoJob()
        {
            eggs += EGGS_PER_SHIFT;
            foreach (var worker in workers)
            {
                worker.WorkTheNextShift();
            }
            HoneyVault.ConsumeHoney(HONEY_PER_UNASSIGNED_WORKER * unassignedWorkers);//had to changed this to unassignedWorkers, originally a mistype with workers.Length
            UpdateStatusReport();
        }

        public void CareForEggs(float eggsToConvert)
        {
            if (eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorkers += eggsToConvert;
            }
        }

        private void UpdateStatusReport()
        {
            StatusReport = $"Vault report: \n{HoneyVault.StatusReport}\n" +
                $"\nEgg count: {eggs}\nUnassigned Workers: {unassignedWorkers}\n" +
                $"{WorkerStatus("Honey Manufacturer")}\n{WorkerStatus("Nectar Collector")}\n{WorkerStatus("Egg Care")}\n" +
                $"TOTAL WORKERS: {workers.Length}";
        }

        public string WorkerStatus(string job)
        {
            int count = 0;
            foreach (var worker in workers)
                if (worker.Job == job) count++;
            string s = "s";
            if (count == 1) s = "";
            return $"{count} {job} bee{s}";
        }
    }


    public class HoneyManufacturer : Bee
    {
        public const float NECTAR_PROCESSED_PER_SHIFT = 33.15f;
        public HoneyManufacturer() : base("Honey Manufacturer") { }
        public override float CostPerShift { get { return 1.7f; } }

        protected override void DoJob()
        {
            HoneyVault.ConvertNectarToHoney(NECTAR_PROCESSED_PER_SHIFT);
        }
    }

    public class NectarCollector : Bee
    {
        public const float NECTAR_COLLECTED_PER_SHIFT = 33.25f;
        public NectarCollector() : base("Nectar Collector") { }
        public override float CostPerShift { get { return 1.95f; } }

        protected override void DoJob()
        {
            HoneyVault.CollectNectar(NECTAR_COLLECTED_PER_SHIFT);
        }
    }

    public class EggCare : Bee
    {
        public const float CARE_PER_SHIFT = 0.15f;
        private Queen queen;
        public EggCare(Queen queen) : base("Egg Care")
        {
            this.queen = queen;
        }
        public override float CostPerShift { get { return 1.35f; } }

        protected override void DoJob()
        {
            queen.CareForEggs(CARE_PER_SHIFT);
        }
    }
}
