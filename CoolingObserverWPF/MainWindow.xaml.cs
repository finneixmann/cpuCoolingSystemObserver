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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoolingObserverWPF.src;


namespace CoolingObserverWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Controller controller;

        public MainWindow() {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            controller = new Controller(mainWindow: this);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            StartFanAnimation();
            SetRadiatorLevel(0.75f, eco: true);
            SetPump1Level(0.25f, eco: false);
        }

        private void StartFanAnimation() {
            RotateTransform rotate = new RotateTransform();
            IMG_fan_rotor.RenderTransform = rotate;
            IMG_fan_rotor.RenderTransformOrigin = new Point(0.5, 0.5);
            DoubleAnimation anim = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(1.0));
            anim.RepeatBehavior = RepeatBehavior.Forever;
            rotate.BeginAnimation(RotateTransform.AngleProperty, anim);
        }

        public void ShowMessage(string message, string title, MessageBoxImage icon = MessageBoxImage.None, MessageBoxButton button = MessageBoxButton.OK) {
            MessageBox.Show(
                message,
                title,
                button,
                icon
            );
        }

        private void Toggle_greenLED_Checked(object sender, RoutedEventArgs e) => controller.SetTestLED(true);
        private void Toggle_greenLED_Unchecked(object sender, RoutedEventArgs e) => controller.SetTestLED(false);
        private void Toggle_ledStrip_Checked(object sender, RoutedEventArgs e) => controller.SetLEDStrip(false);
        private void Toggle_ledStrip_Unchecked(object sender, RoutedEventArgs e) => controller.SetLEDStrip(false);

        public void SetRadiatorLevel(float level, bool eco) {
            TXT_radiator.Dispatcher.Invoke(() => {
                TXT_radiator.Text = $"{level * 100}%{Environment.NewLine}[{(eco ? "ECO" : "BLAST")}]{Environment.NewLine}RADIATOR";
            });
        }
        public void SetPump1Level(float level, bool eco) {
            TXT_pump1.Dispatcher.Invoke(() => {
                TXT_pump1.Text = $"{level * 100}%{Environment.NewLine}[{(eco ? "ECO" : "BLAST")}]{Environment.NewLine}PUMP 1";
            });
        }
        public void SetPump2Level(float level, bool eco) {
            TXT_pump2.Dispatcher.Invoke(() => {
                TXT_pump2.Text = $"{level * 100}%{Environment.NewLine}[{(eco ? "ECO" : "BLAST")}]{Environment.NewLine}PUMP 2";
            });
        }
        public void SetCoolantTankTemperature(int temp) {
            TXT_coolant_tank.Dispatcher.Invoke(() => {
                TXT_coolant_tank.Text = $"{temp}C{Environment.NewLine}COOLANT TANK";
            });
        }
        public void SetCpuTemp(int temp) {
            TXT_cpu.Dispatcher.Invoke(() => {
                TXT_cpu.Text = $"{(temp == -1 ? "N/A" : $"{temp}C")}{Environment.NewLine}CPU";
            });
        }


        private void TerminalInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                string command = TerminalInput.Text;
                TerminalInput.Clear();
                controller.EnterCommand(command);
                e.Handled = true; // verhindert "Ding" Sound
            }
        }

        public void Log(string message) {
            TerminalOutput.Dispatcher.Invoke(() => {
                TerminalOutput.Text += $"[{DateTime.Now:mm:ss}] {message}{Environment.NewLine}";
                TerminalOutput.ScrollToEnd();
            });
        }
    }
}
