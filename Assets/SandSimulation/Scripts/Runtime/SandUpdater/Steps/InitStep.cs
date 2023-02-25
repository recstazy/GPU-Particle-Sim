using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SandSimulation
{
    [Serializable]
    public class InitStep : ComputeStep
    {
        [SerializeField]
        private float _cellSize = 1f;
        
        [SerializeField]
        private float _particleSize = 1f;
        
        [SerializeField]
        private float _maxRandomizePosition;

        private NativeGrid<Cell> _cells;
        
        private static readonly int SandGridWidth = Shader.PropertyToID("_SandGridWidth");
        private static readonly int SandGridHeight = Shader.PropertyToID("_SandGridHeight");
        private static readonly int SandCellSize = Shader.PropertyToID("_SandCellSize");
        private static readonly int SandParticleSize = Shader.PropertyToID("_SandParticleSize");
        private static readonly int SandIds = Shader.PropertyToID("_SandIds");
        private static readonly int SandSimPositions = Shader.PropertyToID("_SandSimPositions");
        private static readonly int SandVelocities = Shader.PropertyToID("_SandVelocities");
        private static readonly int SandForces = Shader.PropertyToID("_SandForces");
        private static readonly int SandDebugValues = Shader.PropertyToID("_SandDebugValues");

        private float ParticleR => _particleSize * 0.5f;
        private float2 ParticleHalfSize => new float2(ParticleR, ParticleR);
        
        public override void Initialize(NativeGrid<Cell> cells)
        {
            _cells = cells;
            
            Shader.SetGlobalInt(SandGridWidth, cells.GridSize.x);
            Shader.SetGlobalInt(SandGridHeight, cells.GridSize.y);
            Shader.SetGlobalFloat(SandCellSize, _cellSize);
            Shader.SetGlobalFloat(SandParticleSize, _particleSize);

            var ids = CreateArray(cells.PlaneArray
                .Select(x => new uint4((uint)x.Id, 0, 0, 0)));
            var idsBuffer = CreateBuffer(ids);
            Shader.SetGlobalBuffer(SandIds, idsBuffer);
            
            var positions = CreateArray(cells.PlaneArray
                .Select(x => new float4(CreateSimPos(x), 0f, 0f)));
            var positionsBuffer = CreateBuffer(positions);
            Shader.SetGlobalBuffer(SandSimPositions, positionsBuffer);

            var velocities = CreateArray(Enumerable.Repeat(new float4(), cells.PlaneLength));
            var velocitiesBuffer = CreateBuffer(velocities);
            Shader.SetGlobalBuffer(SandVelocities, velocitiesBuffer);
            
            var forces = CreateArray(Enumerable.Repeat(new float4(), cells.PlaneLength));
            var forcesBuffer = CreateBuffer(forces);
            Shader.SetGlobalBuffer(SandForces, forcesBuffer);
            
            var debugValues = CreateArray(Enumerable.Repeat(new float4(), cells.PlaneLength));
            var debugValuesBuffer = CreateBuffer(debugValues);
            Shader.SetGlobalBuffer(SandDebugValues, debugValuesBuffer);
        }

        public override void Run() {}

        private float2 CreateSimPos(Cell cell)
        {
            var database = CellDatabase.Get();
            var defaultId = database.DefaultConfig.Id;
            var wallId = database.AllConfigs.First(x => x.Id != defaultId && x.IsStatic).Id;
            return cell.Id == defaultId || cell.Id == wallId 
                ? ToSimPos(cell.Position) 
                : RandomizePosition(ToSimPos(cell.Position));
        }
        
        private float2 RandomizePosition(float2 position)
        {
            var onCircle = Random.insideUnitCircle;
            var newPos = position + new float2(onCircle) * _maxRandomizePosition;
            newPos = math.clamp(newPos, ParticleHalfSize, _cells.GridSize - ParticleHalfSize);
            return newPos;
        }

        private float2 ToSimPos(int2 gridPos)
        {
            return gridPos + ParticleHalfSize;
        }
    }
}
