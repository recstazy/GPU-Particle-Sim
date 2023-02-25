using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace SandSimulation
{
    [Serializable]
    public class CellDatabase : IDisposable
    {
        [SerializeField]
        private CellConfig _defaultConfig;
        
        [SerializeField]
        private CellConfig[] _configs;

        private int _cacheIdOffset;
        private NativeArray<CellConfigNative> _configsNative;
        private bool _disposed;
        private static ComputeBuffer _globalShaderCellDatabaseBuffer;

        private static readonly int CellDatabaseId = Shader.PropertyToID("CellDatabase");
        private static JsonCellDatabaseProvider _jsonDatabaseProvider;
        
        public CellConfig DefaultConfig => _defaultConfig;
        public int DefaultId => DefaultConfig.Id;
        public CellConfig[] AllConfigs { get; private set; }
        public NativeArray<CellConfigNative> ConfigsNative => _configsNative;

        public void Initialize()
        {
            AllConfigs = _configs.Prepend(_defaultConfig).ToArray();
            var minId = AllConfigs.Min(x => x.Id);
            var maxId = AllConfigs.Max(x => x.Id);
            _cacheIdOffset = minId;
            _configsNative = new NativeArray<CellConfigNative>(maxId - minId + 1, Allocator.Persistent);
            
            foreach (var config in AllConfigs)
            {
                _configsNative[config.Id - _cacheIdOffset] = new CellConfigNative(config);
            }
            
            CreateGlobalShaderBuffer();
        }
        
        public void Dispose()
        {
            _configsNative.Dispose();
            _globalShaderCellDatabaseBuffer.Dispose();
            _disposed = true;
        }
        
        public CellConfigNative GetConfig(int id)
        {
            if (_disposed) return default;
            var configAtIndex = _configsNative[id - _cacheIdOffset];

            if (configAtIndex.Id.x == _defaultConfig.Id)
            {
                throw new ArgumentException($"No Id {id} found in Database");
            }

            return configAtIndex;
        }
        
        public static ICellDatabaseProvider GetProvider()
        {
            return _jsonDatabaseProvider ??= new();
        }

        public static CellDatabase Get() => GetProvider().Database;

        private void CreateGlobalShaderBuffer()
        {
            _globalShaderCellDatabaseBuffer = new ComputeBuffer(ConfigsNative.Length, CellConfigNative.GetStrideSize());
            _globalShaderCellDatabaseBuffer.SetData(_configsNative);
            Shader.SetGlobalBuffer(CellDatabaseId, _globalShaderCellDatabaseBuffer);
            Shader.SetGlobalInt("_DefaultSandId", _defaultConfig.Id);
            Shader.SetGlobalInt("_MaxSandId", _configs.Max(x => x.Id));
        }
    }
}
