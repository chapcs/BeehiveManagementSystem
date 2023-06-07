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

        //lets each Bee subclass define the amount of honey it consumes each shift
        public virtual string CostPerShift { get; } 

        //pass consumed honey to the method in honeyvault, if true then this method will call DoJob
        public void WorkTheNextShift(float honeyconsumed)
        {
            if (HoneyVault.ConsumeHoney(honeyconsumed))
                DoJob();
            else
                return;
        }

        //don't know if this is supposed to be in this exact spot
        public void DoJob()
        {

        }
    }

    public class Queen : Bee
    {
        public Queen() : base("Queen")
        {

        }
    }

    //each of the subclasses will include overrides to DoJob and CostPerShift

    public class HoneyManufacturer : Bee
    {
        public HoneyManufacturer() : base("Honey Manufacturer")
        {

        }
    }

    public class NectarCollector : Bee
    {
        public NectarCollector() : base("Nectar Collector")
        {

        }
    }

    public class EggCare : Bee
    {
        public EggCare() : base("Egg Care")
        {

        }
    }
}
