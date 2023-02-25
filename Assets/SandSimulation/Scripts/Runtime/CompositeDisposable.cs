using System;
using System.Collections.Generic;
using UnityEngine;

namespace SandSimulation
{
    public class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();

        public void Add(IDisposable disposable) => _disposables.Add(disposable);
        
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }
    }

    public class DisposableRenderTexture : IDisposable
    {
        private readonly RenderTexture _texture;

        public DisposableRenderTexture(RenderTexture texture)
        {
            _texture = texture;
        }
        
        public void Dispose()
        {
            _texture.Release();
        }
    }

    public static class DisposableExtensions
    {
        public static T AddTo<T>(this T disposable, CompositeDisposable container) where T : IDisposable
        {
            container.Add(disposable);
            return disposable;
        }

        public static RenderTexture AddTo(this RenderTexture tex, CompositeDisposable disposable)
        {
            disposable.Add(new DisposableRenderTexture(tex));
            return tex;
        }
    }
}
