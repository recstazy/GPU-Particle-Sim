Shader "Unlit/CellsShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_PROCEDURAL_INSTANCING_ON
            #pragma instancing_options procedural:ConfigureProcedural
            
            #include "UnityCG.cginc"
            #include "CellConfig.cginc"
            #include "SandUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _Alpha;
            float4 _Offset;
            
            StructuredBuffer<uint4> _SandIds;
            StructuredBuffer<float4> _SandSimPositions;
            StructuredBuffer<float4> _SandDebugValues;
            StructuredBuffer<CellConfig> CellDatabase;
            
            void ConfigureProcedural() {}

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float4 vertex = v.vertex;

                int id = 0;
                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                id = unity_InstanceID;
                #endif

                const float2 simPosition = _SandSimPositions[id].xy;
                const float3 position = float3(simPosition, 0);
                vertex.xyz = position + vertex.xyz * _SandParticleSize;
                
                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float2 uv = i.uv;
                
                fixed4 col = tex2D(_MainTex, uv);
                UNITY_APPLY_FOG(i.fogCoord, col);

                int id = 0;
                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                id = unity_InstanceID;
                #endif

                const uint cellId = _SandIds[id].x;
                col.rgb = ((cellId + 1) / 2);
                col.a = _Alpha * (cellId != _DefaultSandId);
                
                return col;
                
            }
            ENDCG
        }
    }
}
