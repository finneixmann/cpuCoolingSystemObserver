using System;
using System.IO.Ports;
using CoolingObserverWPF.src;

public class CoolingSystemController {
    private SerialPort port = new();
    private Controller controller;
    private bool _isConnected = false;

    public CoolingSystemController(Controller controller) {
        this.controller = controller;
        Connect();
        //SendHello();
    }

    private void Connect() {
        _isConnected = false;
        try {
            port = new SerialPort("COM3", 9600);
            port.Open();
            _isConnected = true;
            controller.view.Log("> Connected to CSCU on COM3");
        }
        catch (Exception ex) {
            controller.view.ShowMessage("Failed to connect to Cooling System Controller: " + ex.Message);
            controller.view.Log("> Failed to connect to CSCU on COM3");
        }
    }

    private void SendHello() {
        if (!_isConnected) {
            return;
        }
        port.WriteLine("H");
        Console.WriteLine("Send test message to COM3");
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
            controller.view.ShowMessage("No connection to Cooling System Controller. Please reconnect.");
            return;
        }
        port.WriteLine(active ? "SH" : "SL");
    }
}