using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace SandSimulation
{
    [Serializable]
    public abstract class ComputeStep : IDisposable
    {
        [SerializeField]
        protected ComputeShader _shader;

        protected readonly CompositeDisposable Disposer = new();

        public abstract void Initialize(NativeGrid<Cell> cells);
        public abstract void Run();
        
        public void Dispose()
        {
            Disposer?.Dispose();
        }

        protected NativeArray<T> CreateArray<T>(IEnumerable<T> collection, Allocator allocator = Allocator.Persistent) where T : struct
        {
            return new NativeArray<T>(collection.ToArray(), allocator).AddTo(Disposer);
        }

        protected ComputeBuffer CreateBuffer<T>(NativeArray<T> array) where T : struct
        {
            var stride = Marshal.SizeOf<T>();
            var buffer = new ComputeBuffer(array.Length, stride).AddTo(Disposer);
            buffer.SetData(array);
            return buffer;
        }

        protected ComputeBuffer CreateDefaultBuffer<T>(int2 gridSize) where T : struct
        {
            return CreateDefaultBuffer<T>(gridSize.x * gridSize.y);
        }

        protected ComputeBuffer CreateDefaultBuffer<T>(int length) where T : struct
        {
            var array = CreateArray(Enumerable.Repeat(new T(), length));
            return CreateBuffer(array);
        }

        protected RenderTexture CreateTexture(int2 size, GraphicsFormat format, bool randomWrite = true)
        {
            var tex = new RenderTexture(size.x, size.y, 1, format);
            tex.enableRandomWrite = randomWrite;
            tex.AddTo(Disposer);
            return tex;
        }

        protected void GetComputeXY(int2 gridSize, int numThreadsX, int numThreadsY, out int x, out int y)
        {
            x = gridSize.x / numThreadsX;
            y = gridSize.y / numThreadsY;
        }

        protected void Dispatch(int kernel, int2 gridSize, int numThreadsX, int numThreadsY)
        {
            _shader.Dispatch(kernel, gridSize.x / numThreadsX, gridSize.y / numThreadsY, 1);
        }
    }
}
