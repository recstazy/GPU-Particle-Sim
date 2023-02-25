using System;

namespace SandSimulation
{
    public static class DisposableManager
    {
        public static event Action<IDisposable> OnTrackDisposable;

        public static void TrackDisposable(IDisposable disposable)
        {
            OnTrackDisposable?.Invoke(disposable);
        }
    }
}
