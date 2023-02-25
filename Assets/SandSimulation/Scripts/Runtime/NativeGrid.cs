using System;
using Unity.Collections;
using Unity.Mathematics;

namespace SandSimulation
{
    public struct NativeGrid<T> : IDisposable where T : struct
    {
        public readonly int2 GridSize;
        public bool IsCreated => _array.IsCreated;
        public NativeArray<T> PlaneArray => _array;
        public int PlaneLength => _array.Length;

        private NativeArray<T> _array;

        public NativeGrid(int2 gridSize, Allocator allocator)
        {
            GridSize = gridSize;
            _array = new NativeArray<T>(gridSize.x * gridSize.y, allocator);
        }

        public NativeGrid(NativeGrid<T> source, Allocator allocator)
        {
            GridSize = source.GridSize;
            _array = new NativeArray<T>(source._array, allocator);
        }
        
        public NativeGrid(int gridSize, Allocator allocator, Func<int2, T> initializer) : this(gridSize, allocator)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    _array[y * GridSize.x + x] = initializer(new int2(x, y));
                }
            }
        }

        public static NativeGrid<T> GetRows(NativeGrid<T> source, int startRow, int endRow, Allocator allocator)
        {
            var gridSize = new int2(source.GridSize.x, endRow - startRow + 1);
            var grid = new NativeGrid<T>(gridSize, allocator);
            source.CopyRowsTo(grid, startRow);
            return grid;
        }

        public void CopyRowsTo(NativeGrid<T> destination, int startRow)
        {
            NativeSlice<T> arraySlice = new NativeSlice<T>(_array, 
                startRow * GridSize.x, 
                destination.GridSize.x * destination.GridSize.y);
            arraySlice.CopyTo(destination._array);
        }

        public NativeSlice<T> GetRow(int index)
        {
            return new NativeSlice<T>(_array, index * GridSize.x, GridSize.x);
        }

        public void Dispose()
        {
            if (!_array.IsCreated)
                throw new ObjectDisposedException("Native Grid wasn't been initialized before dispose");

            _array.Dispose();
        }

        public T this[int x, int y]
        {
            get => GetElement(x, y);
            set => SetElement(x, y, value);
        }

        public T this[int2 position]
        {
            get => GetElement(position.x, position.y);
            set => SetElement(position.x, position.y, value);
        }

        public bool IsInBounds(int2 position)
        {
            return position.x >= 0 && position.x < GridSize.x && position.y >= 0 && position.y < GridSize.y;
        }

        public static int2 PlaneIndexToGridPos(int planeIndex, int2 gridSize)
        {
            return new int2(planeIndex / gridSize.x, planeIndex % gridSize.x);
        }

        private T GetElement(int x, int y)
        { 
            return _array[y * GridSize.x + x];
        }

        private void SetElement(int x, int y, in T value)
        {
            _array[y * GridSize.x + x] = value;
        }
    }
}