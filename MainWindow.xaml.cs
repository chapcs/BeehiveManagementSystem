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

namespace BeehiveManagementSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public static class HoneyVault
    {
        const float NECTAR_CONVERSION_RATIO = .19f;
        const float LOW_LEVEL_WARNING = 10f;

        private static float honey = 25f;
        private static float nectar = 100f;

        static string StatusReport
        {
            get
            {
                string warning = "";
                if (honey < LOW_LEVEL_WARNING)
                    warning = "LOW HONEY - ADD MANUFACTURER\n";
                else if (nectar < LOW_LEVEL_WARNING)
                    warning += "LOW NECTAR - ADD COLLECTOR\n";
                return $"Honey: {honey}\nNectar: {nectar}\n" + warning;
            }
        }

        public static void ConvertNectarToHoney(float amount)
        {
            if (amount <= nectar)
                amount = nectar;
            else
                nectar -= amount;
            honey += (amount * NECTAR_CONVERSION_RATIO);
        }

        public static bool ConsumeHoney(float amount)
        {
            if (amount >= honey)
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
            if (amount > 0)
                nectar += amount;
        }
    }

    public class Bee
    {
        public string Job { get; }

        public Bee(string job)
        {
            Job = job;
        }

        public virtual float CostPerShift { get; } 

        public void WorkTheNextShift(float honeyconsumed)
        {
            if (HoneyVault.ConsumeHoney(honeyconsumed))
                DoJob();
            else
                return;
        }

        public virtual void DoJob()
        {

        }

        //here to add new workers to the Bee[] array in class Queen
        public void AddWorker()
        {

        }
    }

    public class Queen : Bee //still need to start off with 3 
    {
        private Bee[] workers;
        public float eggs;
        public float unassignedWorkers;

        const float EGGS_PER_SHIFT = 0.45f;
        const float HONEY_PER_UNASSIGNED_WORKER = 0.5f;

        public Queen() : base("Queen")
        {
            AssignBee("Honey Manufacturer");
            AssignBee("Nectar Collector");
            AssignBee("Egg Care");
        }

        public override float CostPerShift { get { return 2.15f; } }

        public void AssignBee(string job)
        {
            switch (job)
            {
                case "Honey Manufacturer":
                    AddWorker(new HoneyManufacturer(this));
                    break;
                case "Nectar Collector":
                    AddWorker(new NectarCollector(this));
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

        public override void DoJob()
        {
            eggs += EGGS_PER_SHIFT;
            foreach (var worker in workers)
            {
                base.WorkTheNextShift(CostPerShift);
            }
            HoneyVault.ConsumeHoney(HONEY_PER_UNASSIGNED_WORKER * workers.Length);//does this really do what we are wanting it to
            UpdateStatusReport();
        }

        public void CareForEggs(float eggsToConvert)
        {
            if (eggs >= eggsToConvert)
            {
                eggs -= eggsToConvert;
                unassignedWorkers += eggsToConvert;
            }
            else
                return;
        }

        private string UpdateStatusReport() //needs reconfigured and reports to be sent out for each method
        {
            HoneyVault.StatusReport;
        }
    }


    public class HoneyManufacturer : Bee
    {
        public HoneyManufacturer() : base("Honey Manufacturer")
        {

        }

        public override float CostPerShift { get { return 1.7f; } }

        public override void DoJob()
        {
            base.DoJob();
        }
    }

    public class NectarCollector : Bee
    {
        public NectarCollector() : base("Nectar Collector")
        {

        }

        public override float CostPerShift { get { return 1.95f; } }

        public override void DoJob()
        {
            base.DoJob();
        }
    }

    public class EggCare : Bee
    {
        public EggCare() : base("Egg Care")
        {

        }

        public override float CostPerShift { get { return 1.35f; } }

        public override void DoJob()
        {
            base.DoJob(); //call queens CareForEggs
        }
    }
}
