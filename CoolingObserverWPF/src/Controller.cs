using System;

namespace CoolingObserverWPF.src
{
    public class Controller
    {
        private CoolingSystemController coolingSystemController;
        public Controller()
        {
            Console.WriteLine("Controller initialized!");
            coolingSystemController = new CoolingSystemController();
        }

        public void SetTestLED(bool active)
        {
            coolingSystemController.SetGreenLED(active);
        }
    }
}