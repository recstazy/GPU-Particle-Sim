using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SandSimulation
{
    public class SandSimulation : MonoBehaviour
    {
        [SerializeField]
        private Object _sandModelObj;
        
        [SerializeField]
        private InitStep _initStep;
        
        [SerializeField]
        private PixelizeParticles _pixelizeStep;

        [SerializeField]
        private ComputeForces _computeForces;

        private ISandModel _sandModel;

        private float _lastTime;
        public static float DeltaTime { get; private set; }

        private void Awake()
        {
            _sandModel = (ISandModel)_sandModelObj;
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            yield return null;
            var cells = _sandModel.Cells;

            foreach (var step in EnumerateAllSteps())
            {
                step.Initialize(cells);
            }
            
            yield return UpdateLoop();
        }

        private void OnDestroy()
        {
            foreach (var step in EnumerateAllSteps())
            {
                step.Dispose();
            }
        }

        private IEnumerator UpdateLoop()
        {
            _lastTime = Time.time;
            
            while (true)
            {
                DeltaTime = Time.time - _lastTime;
                _lastTime = Time.time;
                
                foreach (var step in EnumerateAllSteps())
                {
                    step.Run();
                }
                
                yield return null;
            }
        }

        private IEnumerable<ComputeStep> EnumerateAllSteps()
        {
            yield return _initStep;
            yield return _pixelizeStep;
            yield return _computeForces;
        }
    }
}
