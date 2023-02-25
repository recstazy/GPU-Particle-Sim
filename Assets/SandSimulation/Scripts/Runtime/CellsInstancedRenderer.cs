using UnityEngine;
using UnityEngine.Rendering;

namespace SandSimulation
{
    [RequireComponent(typeof(ISandModel))]
    public class CellsInstancedRenderer : MonoBehaviour
    {
        [SerializeField]
        private Mesh _cellMesh;

        [SerializeField]
        private Material _cellMaterial;

        [SerializeField]
        private Vector2 _offset;

        [SerializeField]
        private float _scale = 1f;
        
        private Material _materialInstance;
        private Bounds _bounds;
        private static readonly int CellsToRenderId = Shader.PropertyToID("_CellsToRender");
        private static readonly int CellSizeId = Shader.PropertyToID("_CellSize");
        private CompositeDisposable _renderDisposer = new();
        private ISandModel _model;
        private static readonly int Offset = Shader.PropertyToID("_Offset");

        public Bounds Bounds => _bounds;

        private void Awake()
        {
            _model = GetComponent<ISandModel>();
        }

        private void Update()
        {
            RenderCells();
        }

        private void RenderCells()
        {
            float cellSize = _scale / _model.Cells.GridSize.x;
            _bounds = new Bounds(Vector3.zero, new Vector3(_model.Cells.GridSize.x * cellSize, _model.Cells.GridSize.y * cellSize, 0f));
            EnsureMaterial();
            _renderDisposer?.Dispose();
            _renderDisposer = new CompositeDisposable();

            _materialInstance.SetFloat(CellSizeId, cellSize);
            _materialInstance.SetVector(Offset, _offset);
            
            Graphics.DrawMeshInstancedProcedural(_cellMesh, 0, _materialInstance, Bounds, 
                _model.Cells.PlaneLength, null, ShadowCastingMode.Off, false);
        }

        private void OnDestroy()
        {
            _renderDisposer?.Dispose();
        }

        private void EnsureMaterial()
        {
            if (_materialInstance != null) return;
            _materialInstance = Instantiate(_cellMaterial);
        }
    }
}
