Shader "Custom/StylizedWaterWaveTrail" {
    Properties {
        _Color ("Base Color", Color) = (1, 0.5, 0.5, 1)
        _WaveAmplitude ("Wave Amplitude", Range(0.0, 1.0)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(0.0, 10.0)) = 5.0
        _WaveSpeed ("Wave Speed", Range(-5.0, 5.0)) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            half _WaveAmplitude;
            half _WaveFrequency;
            half _WaveSpeed;

            v2f vert (appdata_t v) {
                v2f o;
                float wave = sin(_Time.y * _WaveSpeed + v.vertex.x * _WaveFrequency) * _WaveAmplitude;
                v.vertex.y += wave;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Add a gradient effect based on the UV coordinates
                float gradient = i.uv.y;
                fixed4 color = lerp(fixed4(0, 0, 1, 1), _Color, gradient);
                return color;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
