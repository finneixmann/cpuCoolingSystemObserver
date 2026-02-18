using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoolingObserverWPF.src;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace CoolingObserverWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Controller controller;
        public PlotModel? SinModel { get; set; }
        private PlotModel? model;
        private LineSeries? lineSeriesTemp;
        private LineSeries? lineSeriesLoadP1;
        private LineSeries? lineSeriesLoadP2;
        private LineSeries? lineSeriesLoadRadiator;


        public MainWindow() {
            InitializeComponent();
            InitPlots();
            this.DataContext = this;
            this.Loaded += MainWindow_Loaded;
            controller = new Controller(mainWindow: this);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            StartFanAnimation();
            LoadComboboxes();
        }

        private void InitPlots() {
            model = new PlotModel {
                Background = OxyColor.Parse("#111111"),
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,
                IsLegendVisible = true,
            };
            var timeAxis = new DateTimeAxis {
                Position = AxisPosition.Bottom,
                AxislineColor = OxyColors.White,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColor.Parse("#222222"),
                StringFormat = "mm:ss",
            };
            var tempAxis = new LinearAxis {
                Position = AxisPosition.Left,
                Title = "Y",
                MajorStep = 10,
                MinorStep = 2,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.Gray,
                MinorGridlineColor = OxyColor.Parse("#222222"),
                AxislineColor = OxyColors.White,
                TextColor = OxyColors.White
            };
            model.Axes.Add(timeAxis);
            model.Axes.Add(tempAxis);
            var legend = new Legend {
                LegendTitle = "Legend",
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBackground = OxyColor.Parse("#111111"),
                LegendBorder = OxyColors.White
            };
            model.Legends.Add(legend);
            lineSeriesTemp = new LineSeries {
                Color = OxyColors.Red,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 1,
                Title = "T Tank"
            };
            lineSeriesLoadP1 = new LineSeries {
                Color = OxyColors.LightBlue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 1,
                Title = "P1 Load"
            };
            lineSeriesLoadP2 = new LineSeries {
                Color = OxyColors.Cyan,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 1,
                Title = "P2 Load"
            };
            lineSeriesLoadRadiator = new LineSeries {
                Color = OxyColors.Blue,
                StrokeThickness = 2,
                MarkerType = MarkerType.Circle,
                MarkerSize = 1,
                Title = "Rad Load"
            };
            model.Series.Add(lineSeriesTemp);
            model.Series.Add(lineSeriesLoadP1);
            model.Series.Add(lineSeriesLoadP2);
            model.Series.Add(lineSeriesLoadRadiator);
            SetPlot(model);

        }

        public void SetPlot(PlotModel model) {
            SinModel = model;
            this.DataContext = this;
            SinModel.InvalidatePlot(true);
        }

        private void AddTemperatureDataPoint(float temp) {
            if (lineSeriesTemp == null || model == null) return;
            if (lineSeriesTemp.Points.Count > 10 && lineSeriesTemp.Points.Count > 0) {
                lineSeriesTemp.Points.RemoveAt(0);
            }
            lineSeriesTemp.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), temp));
            model.InvalidatePlot(true);
        }

        private void AddSystemLoadDataPoint(string component, float load = 0) {
            if (lineSeriesTemp == null || model == null) return;
            LineSeries? series;
            switch (component) {
                case "pump1":
                    series = lineSeriesLoadP1;
                    break;
                case "pump2":
                    series = lineSeriesLoadP2;
                    break;
                case "radiator":
                    series = lineSeriesLoadRadiator;
                    break;
                default:
                    return;
            }
            if (series == null) return;
            if (series.Points.Count > 10 && series.Points.Count > 0) {
                series.Points.RemoveAt(0);
            }
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), load));
            model.InvalidatePlot(true);
        }

        private void StartFanAnimation() {
            RotateTransform rotate = new RotateTransform(); IMG_fan_rotor.RenderTransform = rotate; IMG_fan_rotor.RenderTransformOrigin = new Point(0.5, 0.5); DoubleAnimation anim = new DoubleAnimation(0, 360, TimeSpan.FromSeconds(1.0)); anim.RepeatBehavior = RepeatBehavior.Forever; rotate.BeginAnimation(RotateTransform.AngleProperty, anim);
        }


        public void ShowMessage(string message, string title, MessageBoxImage icon = MessageBoxImage.None, MessageBoxButton button = MessageBoxButton.OK) {
            MessageBox.Show(
                message,
                title,
                button,
                icon
            );
        }

        private void LoadComboboxes() {
            CB_ledModeSelect.ItemsSource = Enum.GetNames(typeof(Controller.LEDMode));
        }

        private void BTN_applyLedChanges_Click(object sender, RoutedEventArgs e) {
            string? selected = CB_ledModeSelect.SelectedItem as string;
            if (selected != null && Enum.TryParse(selected, out Controller.LEDMode ledMode)) {
                controller.UpdateLed(ledMode);
            }
        }

        private void BTN_generalPurposeAction_Click(object sender, RoutedEventArgs e) {
            AddTemperatureDataPoint(new Random().Next(20, 30));
            AddSystemLoadDataPoint("pump1", new Random().Next(0, 100));
            AddSystemLoadDataPoint("pump2", new Random().Next(0, 100));
            AddSystemLoadDataPoint("radiator", new Random().Next(0, 100));
        }

        private void BTN_reconnect_Click(object sender, RoutedEventArgs e) {
            controller.Reconnect();
        }

        private void Toggle_greenLED_Checked(object sender, RoutedEventArgs e) => controller.SetTestLED(true);
        private void Toggle_greenLED_Unchecked(object sender, RoutedEventArgs e) => controller.SetTestLED(false);


        public void SetRadiatorLevel(float level, bool eco) {
            TXT_radiator.Dispatcher.Invoke(() => {
                TXT_radiator.Text = $"{level}%{Environment.NewLine}[{(eco ? "ECO" : "BLAST")}]{Environment.NewLine}RADIATOR";
            });
            AddSystemLoadDataPoint("radiator", level);
        }
        public void SetPump1Level(float level, bool eco) {
            TXT_pump1.Dispatcher.Invoke(() => {
                TXT_pump1.Text = $"{level * 100}%{Environment.NewLine}[{(eco ? "ECO" : "BLAST")}]{Environment.NewLine}PUMP 1";
            });
            AddSystemLoadDataPoint("pump1", level);
        }
        public void SetPump2Level(float level, bool eco) {
            TXT_pump2.Dispatcher.Invoke(() => {
                TXT_pump2.Text = $"{level * 100}%{Environment.NewLine}[{(eco ? "ECO" : "BLAST")}]{Environment.NewLine}PUMP 2";
            });
            AddSystemLoadDataPoint("pump2", level);
        }
        public void SetCoolantTankTemperature(int temp) {
            TXT_coolant_tank.Dispatcher.Invoke(() => {
                TXT_coolant_tank.Text = $"{temp}℃{Environment.NewLine}COOLANT TANK";
            });
            AddTemperatureDataPoint(temp);
        }
        public void SetCpuTemp(int temp) {
            TXT_cpu.Dispatcher.Invoke(() => {
                TXT_cpu.Text = $"{(temp == -1 ? "N/A" : $"{temp}C")}{Environment.NewLine}CPU";
            });
        }
        public void SetCSCUMode(Controller.CSCUMode mode) {
            TXT_sysMode.Dispatcher.Invoke(() => {
                TXT_sysMode.Text = mode.ToString();
                TXT_sysMode.Foreground = mode == Controller.CSCUMode.ECO ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.SkyBlue);
            });
        }
        public void SetLEDMode(Controller.LEDMode mode) {
            TXT_ledMode.Dispatcher.Invoke(() => {
                TXT_ledMode.Text = $"MODE: {mode.ToString()}";
            });
        }
        public void SetConnection(bool isConnected) {
            TXT_connectionStatus.Dispatcher.Invoke(() => {
                TXT_connectionStatus.Text = isConnected ? "CONNECTED" : "NO CONNECTION";
                TXT_connectionStatus.Foreground = isConnected ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Red);
            });
            PNL_connectionStatus.Dispatcher.Invoke(() => {
                PNL_connectionStatus.Visibility = isConnected ? Visibility.Collapsed : Visibility.Visible;
            });

            // LED STRIP CONTROL VISIBILITY
            GRID_ledStripControl.Dispatcher.Invoke(() => {
                GRID_ledStripControl.Visibility = isConnected ? Visibility.Visible : Visibility.Hidden;
            });
            TXT_ledStripDisconnected.Dispatcher.Invoke(() => {
                TXT_ledStripDisconnected.Visibility = isConnected ? Visibility.Hidden : Visibility.Visible;
            });
        }
        public void SetTSS(Controller.TSS tss) {
            PNL_tss.Dispatcher.Invoke(() => {
                PNL_tss.Visibility = tss == Controller.TSS.OK ? Visibility.Collapsed : Visibility.Visible;
            });
            TXT_tss.Dispatcher.Invoke(() => {
                TXT_tss.Text = tss.ToString();
            });
            TXT_systemStatus.Dispatcher.Invoke(() => {
                TXT_systemStatus.Text = tss.ToString();
                TXT_systemStatus.Foreground = tss == Controller.TSS.OK ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.Red);
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
