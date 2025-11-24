Shader "_Shaders/FireTrail"
{
    Properties
    {
        _ColorStart("Start Color", Color) = (1,0.5,0,1)
        _ColorEnd("End Color", Color) = (1,0,0,1)
        _Alpha("Alpha", Range(0,1)) = 0.9   // Increased base alpha
        _Speed("Speed", Range(0,5)) = 2
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseStrength("Noise Strength", Range(0,1)) = 0.2  // Lowered noise effect on transparency
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float _NoiseStrength;
            float _Speed;
            fixed4 _ColorStart;
            fixed4 _ColorEnd;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.y += _Time.y * _Speed;

                // Reduced noise impact to keep it more opaque
                float noise = tex2D(_NoiseTex, uv).r * _NoiseStrength;

                fixed4 col = lerp(_ColorStart, _ColorEnd, i.uv.y + noise);

                // Multiply by base alpha only slightly reduced by noise
                col.a = _Alpha - (noise * 0.3);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
