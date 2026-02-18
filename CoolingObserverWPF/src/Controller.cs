using System;
using System.Security.RightsManagement;

namespace CoolingObserverWPF.src {
    public class Controller {
        private CoolingSystemController coolingSystemController;
        private CpuObserver cpuObserver;
        private PlotViewer plotViewer;
        public View view;
        public enum CSCUMode {
            ECO = 0,
            BLAST = 1
        }
        public enum TSS {
            OK = 0,
            DISCONNECTED = 1,
            ERROR = 2,
        }
        public enum LEDMode {
            OFF = 0,
            BOUNCE1 = 1,
            LOOP1 = 2
        }

        public CSCUMode cscuMode = CSCUMode.ECO;

        public Controller(MainWindow mainWindow) {
            this.view = new View(mainWindow, this);
            this.cpuObserver = new CpuObserver(this);
            this.coolingSystemController = new CoolingSystemController(controller: this);
            this.plotViewer = new PlotViewer(mainWindow);

            if (!cpuObserver.IsAuthorized) {
                view.SetCpuTemp(-1);
            }
        }

        public void Reconnect() {
            this.cpuObserver = new CpuObserver(this);
            this.coolingSystemController = new CoolingSystemController(controller: this);
        }

        public void UpdateLed(LEDMode ledMode) {
            coolingSystemController.SetLED(ledMode);
        }

        public void SetTestLED(bool active) {
            coolingSystemController.SetGreenLED(active);
        }

        public void EnterCommand(string cmd) {
            coolingSystemController.SendOnCOM3(cmd);
            view.Log($"[USER] > {cmd}");
        }

        public class View {
            private MainWindow mainWindow;
            private Controller controller;
            public View(MainWindow mainWindow, Controller controller) {
                this.mainWindow = mainWindow;
                this.controller = controller;
            }

            public void ShowMessage(string message) {
                mainWindow.ShowMessage(message, "Information");
            }

            public void Log(string message) {
                mainWindow.Log(message.Trim());
            }

            public void SetRadiatorLevel(float level) => mainWindow.SetRadiatorLevel(level, eco: controller.cscuMode == CSCUMode.ECO);
            public void SetCoolantTankTemperature(int temp) => mainWindow.SetCoolantTankTemperature(temp);
            public void SetCSCUMode(CSCUMode mode) => mainWindow.SetCSCUMode(mode);
            public void SetTSS(TSS tss) => mainWindow.SetTSS(tss);
            public void SetLEDMode(LEDMode mode) => mainWindow.SetLEDMode(mode);
            public void SetCpuTemp(int temp) => mainWindow.SetCpuTemp(temp);
            public void SetConnection(bool isConnected) => mainWindow.SetConnection(isConnected);
            public void SetPump1PowerLevel(int level) => mainWindow.SetPump1Level(level, eco: controller.cscuMode == CSCUMode.ECO);
            public void SetPump2PowerLevel(int level) => mainWindow.SetPump2Level(level, eco: controller.cscuMode == CSCUMode.ECO);
        }
    }
}