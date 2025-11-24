Shader "_Shaders/IceTrail"
{
    Properties
    {
        _Color("Base Color", Color) = (0.6, 0.9, 1, 1)
        _Alpha("Alpha", Range(0,1)) = 0.7
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _NoiseSpeed("Noise Speed", Range(0,5)) = 1
        _NoiseStrength("Noise Strength", Range(0,1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NoiseTex;
            float _NoiseSpeed;
            float _NoiseStrength;
            fixed4 _Color;
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
                uv.y += _Time.y * _NoiseSpeed;
                float noise = tex2D(_NoiseTex, uv).r;
                float alpha = _Alpha - noise * _NoiseStrength;

                fixed4 col = _Color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
