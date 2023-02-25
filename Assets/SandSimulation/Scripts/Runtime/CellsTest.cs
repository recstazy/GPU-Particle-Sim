using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SandSimulation
{
    public class CellsTest : MonoBehaviour, ISandModel
    {
        [SerializeField]
        private int _gridSize;

        [SerializeField]
        [Range(0f, 1f)]
        private float _spawnProbability = 1f;

        private NativeGrid<Cell> _grid;
        public NativeGrid<Cell> Cells => _grid;
        
        private void Awake()
        {
            var database = CellDatabase.Get();
            _grid = new NativeGrid<Cell>(_gridSize, Allocator.Persistent, 
                position => new Cell(GenerateRandomId(position), position));
        }

        private void OnDestroy()
        {
            _grid.Dispose();
        }

        private int GenerateRandomId(int2 position)
        {
            var database = CellDatabase.Get();
            var defaultId = database.DefaultId;
            var validIds = database.AllConfigs
                .Where(x => x.Id != defaultId && !x.IsStatic)
                .Select(x => x.Id)
                .ToArray();

            var chance = Random.Range(0f, 1f);
            return chance <= _spawnProbability
                ? GetRandom(validIds)
                : defaultId;
        }

        private T GetRandom<T>(T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }
}
