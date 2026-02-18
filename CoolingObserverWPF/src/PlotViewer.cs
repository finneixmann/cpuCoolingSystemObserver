using System;
using CoolingObserverWPF;
using OxyPlot;
using OxyPlot.Series;
public class PlotViewer {
    public PlotViewer(MainWindow mainWindow) {
        PlotModel model = new PlotModel { Title = "Sine Wave" };
        model.Series.Add(new FunctionSeries(Math.Sin, 0, 10, 0.1, "sin(x)"));
        mainWindow.SetPlot(model);
    }
}