using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using CoolingObserverWPF.src;

public class CoolingSystemController {

    private readonly float POLLING_DELAY = 8f;
    private readonly float ANSWER_TIMEOUT = 2f;
    private LoadedTimeout loadedTimeout;
    private SerialPort port = new();
    private Controller controller;
    private bool _isConnected = false;
    private CancellationTokenSource? _pollingCts;
    private CancellationTokenSource? _timeoutCts;
    private String inputBuffer = "";


    public CoolingSystemController(Controller controller) {
        this.controller = controller;
        this.loadedTimeout = new LoadedTimeout(ANSWER_TIMEOUT);
        this.loadedTimeout.OnTimeout += OnTimeout;
        Connect();
        //SendHandshake();
        RequestCSCUState();
        StartPolling();
    }

    private void Connect() {
        _isConnected = false;
        try {
            port = new SerialPort("COM3", 9600);
            port.DataReceived += Port_DataReceived;
            port.Open();
            _isConnected = true;
            controller.view.Log("Connected to CSCU on COM3");
        }
        catch (Exception ex) {
            controller.view.ShowMessage("Failed to connect to Cooling System Controller: " + ex.Message);
            controller.view.Log("Failed to connect to CSCU on COM3");
        }
        controller.view.SetConnection(_isConnected);
    }

    void Port_DataReceived(object sender, SerialDataReceivedEventArgs e) {
        SerialPort sp = (SerialPort)sender;
        string data = sp.ReadExisting();
        inputBuffer += data;
        if (data.Contains("\n")) {
            ProcessInput();
        }
    }

    private void OnTimeout() {
        _isConnected = false;
        controller.view.SetConnection(false);
    }

    private void ProcessInput() {
        loadedTimeout.RemoveLoad();
        controller.view.Log("Received data from CSCU: " + inputBuffer);
        try {
            string[] cmds = inputBuffer.Split(';');
            if (cmds[0] == "WRT") {
                foreach (string cmd in cmds) {
                    string[] args = cmd.Split('=');
                    switch (args[0]) {
                        case "TMP": // coolant tank temperatur
                            if (int.TryParse(args[1], out int t)) {
                                controller.view.SetCoolantTankTemperature(t);
                            }
                            break;
                        case "SYM": // cscu mode
                            if (int.TryParse(args[1], out int sysMode)) {
                                controller.view.SetCSCUMode((Controller.CSCUMode)sysMode);
                            }
                            break;
                        case "LED": // led mode
                            if (int.TryParse(args[1], out int ledMode)) {
                                controller.view.SetLEDMode((Controller.LEDMode)ledMode);
                            }
                            break;
                        case "TSS": // temperature sensor status
                            if (int.TryParse(args[1], out int tss)) {
                                controller.view.SetTSS((Controller.TSS)tss);
                            }
                            break;
                        case "P1P": // pump1 power level
                            if (int.TryParse(args[1], out int p1p)) {
                                controller.view.SetPump1PowerLevel(p1p);
                            }
                            break;
                        case "P2P": // pump2 power level
                            if (int.TryParse(args[1], out int p2p)) {
                                controller.view.SetPump2PowerLevel(p2p);
                            }
                            break;
                        default:
                            // ignore
                            break;
                    }
                }
            }
        }
        catch (Exception) { }

        inputBuffer = "";
    }

    private void SendHandshake() {
        if (!_isConnected) {
            return;
        }
        port.WriteLine("H");
    }

    public void SetGreenLED(bool active) {
        if (!_isConnected) {
            controller.view.ShowMessage("No connection to Cooling System Controller. Please reconnect.");
            return;
        }
        port.WriteLine(active ? "H" : "L");
    }

    public void SetLEDStripActive(bool active) {
        if (!_isConnected) {
            controller.view.ShowMessage("No connectsstion to Cooling System Controller. Please reconnect.");
            return;
        }
        port.WriteLine(active ? "SH" : "SL");
    }

    // Send message to CSCU on COM3
    private void SendOnCOM3(String message) {
        if (!_isConnected) {
            _isConnected = false;
            controller.view.ShowMessage("No connection to Cooling System Controller. Please reconnect.");
            StopPolling();
            return;
        }
        loadedTimeout.AddLoad();
        port.WriteLine(message);
    }

    // request status update from CSCU
    private void RequestCSCUState() {
        SendOnCOM3("REQ;TMP;TSS;LED;SYM");
    }

    // Polling routine to gather information from CSCU every POLLING_DELAY seconds
    private async Task PollAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            SendOnCOM3("REQ;TMP;");
            await Task.Delay(TimeSpan.FromSeconds(POLLING_DELAY), token);
        }
    }

    // Start polling routine
    void StartPolling() {
        _pollingCts = new CancellationTokenSource();
        _ = PollAsync(_pollingCts.Token);
    }

    // Stop polling routine
    void StopPolling() {
        _pollingCts?.Cancel();
    }
}