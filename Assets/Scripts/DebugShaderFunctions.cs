using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandGame
{
    public class DebugShaderFunctions : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private Transform _lineA;

        [SerializeField]
        private Transform _lineB;

        private void Update()
        {
            var linePos = (Vector2)_lineA.position;
            var lineN = (Vector2)_lineA.up;
            _renderer.material.SetVector("_LineA", new Vector4(linePos.x, linePos.y, lineN.x, lineN.y));

            linePos = _lineB.position;
            lineN = _lineB.up;
            
            _renderer.material.SetVector("_LineB", new Vector4(linePos.x, linePos.y, lineN.x, lineN.y));
        }

        private void OnDrawGizmos()
        {
            if (_lineA == null || _lineB == null) return;
            Gizmos.color = Color.red;
            
            Gizmos.DrawRay(_lineA.position, _lineA.right * 999);
            Gizmos.DrawRay(_lineB.position, _lineB.right * 999);
        }
    }
}
