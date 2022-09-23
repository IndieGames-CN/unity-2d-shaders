Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        [HDR]_OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineDistance ("Outline Distance", Float) = 1
    }
    SubShader
    {
        Tags {
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize; // Vector4(1 / width, 1 / height, width, height)

            float4 _OutlineColor;
            float _OutlineDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = _OutlineDistance;
                fixed upPixel = tex2D(_MainTex, i.uv + d * float2(0, _MainTex_TexelSize.y)).a;
                fixed bottomPixel = tex2D(_MainTex, i.uv + d * float2(0, -_MainTex_TexelSize.y)).a;
                fixed leftPixel = tex2D(_MainTex, i.uv + d * float2(-_MainTex_TexelSize.x, 0)).a;
                fixed rightPixel = tex2D(_MainTex, i.uv + d * float2(_MainTex_TexelSize.x, 0)).a;

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed outline = (1 - leftPixel * upPixel * rightPixel * bottomPixel) * col.a;
                return lerp(col, _OutlineColor, outline);
            }
            ENDCG
        }
    }
}
