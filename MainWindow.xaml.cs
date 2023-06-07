using System;
using System.Collections.Generic;
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
        public Bee()
        {

        }
    }
}
