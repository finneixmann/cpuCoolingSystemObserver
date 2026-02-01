using System;
using System.IO.Ports;

public class CoolingSystemController
{
    private SerialPort port = new();

    public CoolingSystemController()
    {
        Connect();
        SendHello();
    }

    private void Connect()
    {
        port = new SerialPort("COM3", 9600);
        port.Open();
    }

    private void SendHello()
    {
        port.WriteLine("H");
        Console.WriteLine("Send test message to COM3");
    }

    public void SetGreenLED(bool active)
    {
        port.WriteLine(active ? "H" : "L");
    }
}