using System;
using System.Collections.Generic;
using UnityEditor;

namespace SandSimulation.EditorScripts
{
    [InitializeOnLoad]
    public static class EditorDisposableManager
    {
        private static List<IDisposable> _disposables = new();
        
        static EditorDisposableManager()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeChanged;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeReload;
            DisposableManager.OnTrackDisposable += TrackDisposable;
        }

        private static void TrackDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        private static void HandlePlayModeChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.ExitingEditMode)
            {
                Dispose();
            }
        }

        private static void HandleBeforeReload()
        {
            Dispose();
        }

        private static void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }

            _disposables.Clear();
        }
    }
}
