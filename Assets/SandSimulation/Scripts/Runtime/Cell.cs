using System;
using Unity.Mathematics;

namespace SandSimulation
{
    [Serializable]
    public struct Cell
    {
        public readonly int Id;
        public readonly int2 Position;

        public Cell(int id, int2 position)
        {
            Id = id;
            Position = position;
        }

        public static int GetStrideSize()
        {
            return sizeof(int) + (sizeof(int) * 2);
        }
    }
}