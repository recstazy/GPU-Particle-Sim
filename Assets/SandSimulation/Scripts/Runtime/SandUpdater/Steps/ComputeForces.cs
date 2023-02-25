using System;
using Unity.Mathematics;
using UnityEngine;

namespace SandSimulation
{
    [Serializable]
    public class ComputeForces : ComputeStep
    {
        [SerializeField]
        private bool _enabled;

        [SerializeField]
        private float2 _gravity;
        
        [SerializeField]
        private float _springK;

        [SerializeField]
        private float _dampingK;

        private NativeGrid<Cell> _cells;
        
        private static readonly int Spring = Shader.PropertyToID("_Spring");
        private static readonly int Damping = Shader.PropertyToID("_Damping");
        private static readonly int Gravity = Shader.PropertyToID("_Gravity");
        private static readonly int DeltaTime = Shader.PropertyToID("_DeltaTime");

        public override void Initialize(NativeGrid<Cell> cells)
        {
            _cells = cells;
        }

        public override void Run()
        {
            if (!_enabled) return;
            
            _shader.SetFloat(Spring, _springK);
            _shader.SetFloat(Damping, _dampingK);
            _shader.SetVector(Gravity, new float4(_gravity, 0, 0));
            _shader.SetFloat(DeltaTime, 0.02f);
            
            Dispatch(0, _cells.GridSize, 8, 8);
            Dispatch(1, _cells.GridSize, 8, 8);
        }
    }
}
