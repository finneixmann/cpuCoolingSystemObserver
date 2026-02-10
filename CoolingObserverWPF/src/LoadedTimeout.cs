using System;
using System.Threading;
using System.Threading.Tasks;

class LoadedTimeout {
    private int load = 0;
    private float ttl;
    CancellationTokenSource? tokenSource;
    public event Action? OnTimeout;
    public LoadedTimeout(float ttl) {
        this.tokenSource = new CancellationTokenSource();
        this.ttl = ttl;
    }

    public void AddLoad() {
        ++load;
        Start();
    }

    public void RemoveLoad() {
        Stop();
        if (--load > 0) {
            Start();
        }
    }

    private void Start() {
        Stop();

        tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;

        _ = RunTimerAsync(token);
    }

    private void Stop() {
        if (tokenSource != null) {
            tokenSource.Cancel();
            tokenSource.Dispose();
            tokenSource = null;
        }
    }

    private async Task RunTimerAsync(CancellationToken token) {
        try {
            await Task.Delay((int)ttl * 1000, token);
            if (!token.IsCancellationRequested) {
                OnTimeout?.Invoke();
            }
        }
        catch (TaskCanceledException) {
            // Timer wurde abgebrochen → ignorieren
        }
    }
}