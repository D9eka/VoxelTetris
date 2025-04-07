Shader "UI/BlurUnlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 blur = _BlurSize / _ScreenParams.xy;
                fixed4 col = tex2D(_MainTex, uv) * 0.36;
                col += tex2D(_MainTex, uv + blur) * 0.18;
                col += tex2D(_MainTex, uv - blur) * 0.18;
                col += tex2D(_MainTex, uv + blur.yx) * 0.14;
                col += tex2D(_MainTex, uv - blur.yx) * 0.14;
                return col;
            }
            ENDCG
        }
    }
}
