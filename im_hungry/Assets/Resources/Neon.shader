Shader "_Shaders/NeonTrailAdvanced"
{
    Properties
    {
        _Color("Neon Color", Color) = (0,1,1,1)
        _GlowIntensity("Glow Intensity", Range(0,5)) = 2
        _PulseSpeed("Pulse Speed", Range(0,10)) = 2
        _Alpha("Alpha", Range(0,1)) = 1
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

            fixed4 _Color;
            float _GlowIntensity;
            float _PulseSpeed;
            float _Alpha;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Pulsing factor
                float pulse = 0.5 + 0.5 * sin(_Time.y * _PulseSpeed);

                // Glow factor based on distance from center (v.uv)
                float glow = exp(-10 * abs(i.uv.y - 0.5));

                fixed3 col = _Color.rgb * (_GlowIntensity * glow * pulse + 1);
                return fixed4(col, _Alpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
