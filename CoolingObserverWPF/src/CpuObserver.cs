using System;
using CoolingObserverWPF.src;
using System.Management;

public class CpuObserver {
    private Controller controller;
    public bool IsAuthorized { get; private set; } = false;
    public CpuObserver(Controller controller) {
        this.controller = controller;
        Console.WriteLine("CPU temp: ");
        TryGetCpuTemperature();
    }

    private void TryGetCpuTemperature() {
        try {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                @"root\WMI",
                "SELECT * FROM MSAcpi_ThermalZoneTemperature");

            bool found = false;
            IsAuthorized = false;

            foreach (ManagementObject obj in searcher.Get()) {
                double tempKelvin = Convert.ToDouble(obj["CurrentTemperature"]);
                double tempCelsius = (tempKelvin / 10.0) - 273.15;

                Console.WriteLine($"CPU Temperature: {tempCelsius:F1} °C");
                found = true;
                IsAuthorized = true;
            }

            if (!found) {
                Console.WriteLine("No temperature sensors found via WMI.");
            }
        }
        catch (ManagementException mex) {
            Console.WriteLine("WMI query failed: " + mex.Message);
        }
        catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
