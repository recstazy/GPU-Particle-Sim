using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace SandSimulation
{
    [Serializable]
    public class CellConfig
    {
        [SerializeField, Min(0)]
        private int _id;

        [SerializeField]
        private bool _isStatic;
        
        [SerializeField]
        private Vector2 _textureOffset;

        [SerializeField]
        private string _name;

        public int Id => _id;
        public Vector2 TextureOffset => _textureOffset;
        public string Name => _name;
        public bool IsStatic => _isStatic;
    }

    public struct CellConfigNative
    {
        public readonly int2 Id; // x - id, y - isStatic
        public readonly float2 TextureOffset;

        public CellConfigNative(CellConfig config)
        {
            Id = new int2(config.Id, config.IsStatic ? 1 : 0);
            TextureOffset = config.TextureOffset;
        }

        public static int GetStrideSize()
        {
            return Marshal.SizeOf<CellConfigNative>();
        }
    }
}
