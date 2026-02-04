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
using CoolingObserverWPF.src;


namespace CoolingObserverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller controller;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            controller = new Controller();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Toggle_greenLED_Checked(object sender, RoutedEventArgs e) => controller.SetTestLED(true);
        private void Toggle_greenLED_Unchecked(object sender, RoutedEventArgs e) => controller.SetTestLED(false);
        private void Toggle_ledStrip_Checked(object sender, RoutedEventArgs e) => controller.SetLEDStrip(false);
        private void Toggle_ledStrip_Unchecked(object sender, RoutedEventArgs e) => controller.SetLEDStrip(false);
    }
}
