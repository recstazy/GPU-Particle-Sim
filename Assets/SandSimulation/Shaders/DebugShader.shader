// Upgrade NOTE: replaced 'defined VISUALIZE_PIXELIZE_STEP_ON' with 'defined (VISUALIZE_PIXELIZE_STEP_ON)'

Shader "Unlit/DebugShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
        _DebugValueChannel("Debug Value Channel", Range(-1, 3)) = 0
        
        [Toggle(GRAYSCALE_ON)]
        GrayscaleOn ("Grayscale", float) = 0
        
        [Toggle(PIXELIZE_STEP_ON)]
        PixelizeStepOn ("Pixelize Step", float) = 0
        
        [Toggle(DEBUG_VALUES_ON)]
        DebugValuesOn ("Debug Values On", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma shader_feature PIXELIZE_STEP_ON
            #pragma shader_feature DEBUG_VALUES_ON
            #pragma shader_feature GRAYSCALE_ON

            #include "UnityCG.cginc"
            #include "SandUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            StructuredBuffer<int4> _ParticlesInCells;
            StructuredBuffer<uint4> _SandIds;
            StructuredBuffer<float4> _SandDebugValues;

            int _DebugValueChannel;
            float _Alpha;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 VisualizePixelizeStep(float2 uv)
            {
                const uint2 gridPos = UvToGridPos(uv);
                const int4 indicesInCell = _ParticlesInCells[ToPlaneIndex(gridPos)];
                return indicesInCell;
            }

            float4 SampleDebugValues(float2 uv)
            {
                const uint2 gridPos = UvToGridPos(uv);
                return _SandDebugValues[ToPlaneIndex(gridPos)];
            }

            void FilterChannels(inout float4 col)
            {
                if (_DebugValueChannel >= 0)
                {
                    col =
                        (_DebugValueChannel == 0) * float4(col.r, 0, 0, 1) +
                        (_DebugValueChannel == 1) * float4(0, col.g, 0, 1) +
                        (_DebugValueChannel == 2) * float4(0, 0, col.b, 1) +
                        (_DebugValueChannel == 3) * float4(col.a, col.a, col.a, 1);
                }
                else
                {
                    col.a = 1;
                }
            }

            void MakeGrayscale(inout float4 col)
            {
                const float avg = (col.r + col.g + col.b + col.a) / 4;
                col = float4(avg, avg, avg, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float2 uv = i.uv;
                float4 col = 0;

                #ifdef PIXELIZE_STEP_ON
                col = VisualizePixelizeStep(uv);
                #endif

                #ifdef DEBUG_VALUES_ON
                col = SampleDebugValues(uv);
                #endif

                FilterChannels(col);
                col = clamp(col, 0, 1);

                #ifdef GRAYSCALE_ON
                MakeGrayscale(col);
                #endif

                col.a *= _Alpha;
                
                return col;
            }
            
            ENDCG
        }
    }
}
