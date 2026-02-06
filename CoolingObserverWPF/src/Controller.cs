using System;
using System.Security.RightsManagement;

namespace CoolingObserverWPF.src {
    public class Controller {
        private CoolingSystemController coolingSystemController;
        private CpuObserver cpuObserver;
        public View view;

        public Controller(MainWindow mainWindow) {
            this.view = new View(mainWindow);
            this.cpuObserver = new CpuObserver(this);
            this.coolingSystemController = new CoolingSystemController(controller: this);

            if (!cpuObserver.IsAuthorized) {
                view.SetCpuTemp(-1);
            }
        }

        public void SetTestLED(bool active) {
            coolingSystemController.SetGreenLED(active);
        }

        public void SetLEDStrip(bool active) {
            coolingSystemController.SetLEDStripActive(active);
        }

        public void EnterCommand(string cmd) {
            view.Log($"[USER] > {cmd}");
        }

        public class View {
            private MainWindow mainWindow;
            public View(MainWindow mainWindow) {
                this.mainWindow = mainWindow;
            }

            public void ShowMessage(string message) {
                mainWindow.ShowMessage(message, "Information");
            }

            public void Log(string message) {
                mainWindow.Log(message.Trim());
            }

            public void SetRadiatorLevel(float level, bool eco) => mainWindow.SetRadiatorLevel(level, eco);
            public void SetPump1Level(float level, bool eco) => mainWindow.SetPump1Level(level, eco);
            public void SetPump2Level(float level, bool eco) => mainWindow.SetPump2Level(level, eco);
            public void SetCoolantTankTemperature(int temp) => mainWindow.SetCoolantTankTemperature(temp);
            public void SetCpuTemp(int temp) => mainWindow.SetCpuTemp(temp);
        }
    }
}