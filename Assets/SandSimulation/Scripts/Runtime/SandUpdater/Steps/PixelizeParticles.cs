using System;
using Unity.Mathematics;
using UnityEngine;

namespace SandSimulation
{
    [Serializable]
    public class PixelizeParticles : ComputeStep
    {
        private int2 _gridSize;
        private const int StepsCount = 6;
        
        private static readonly int ParticlesInCells = Shader.PropertyToID("_ParticlesInCells");
        private static readonly int ParticlesInCellsTemp = Shader.PropertyToID("_ParticlesInCellsTemp");

        private const int ClearStep = 0;
        private const int BlitStep = 1;
        private const int RgbaStepsStartIndex = 2;
        
        public override void Initialize(NativeGrid<Cell> cells)
        {
            _gridSize = cells.GridSize;

            var particlesInCells = CreateDefaultBuffer<int4>(cells.GridSize);
            var particlesInCellsTemp = CreateDefaultBuffer<int4>(cells.GridSize);
            
            for (int i = 0; i < StepsCount; i++)
            {
                _shader.SetBuffer(i,ParticlesInCellsTemp, particlesInCellsTemp);
            }
            
            Shader.SetGlobalBuffer(ParticlesInCells, particlesInCells);
        }

        public override void Run()
        {
            Dispatch(ClearStep, _gridSize, 8, 8);
            
            for (int i = 0; i < 4; i++)
            {
                int index = RgbaStepsStartIndex + i;
                Dispatch(index, _gridSize, 8, 8);
                Dispatch(BlitStep, _gridSize, 8, 8);
            }
        }
    }
}
