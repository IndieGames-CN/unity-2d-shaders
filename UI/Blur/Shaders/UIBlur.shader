Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 127)) = 1.0
    }
    SubShader
    {
        Cull Off
        ZWrite Off

        CGINCLUDE
        #include "UnityCG.cginc"
        
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float2 uv[5] : TEXCOORD0;
        };

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        fixed _BlurSize;

        v2f vert_hor (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);

            half2 uv = v.uv;
            o.uv[0] = uv;
            o.uv[1] = uv + half2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[2] = uv - half2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
            o.uv[3] = uv + half2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
            o.uv[4] = uv - half2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;

            return o;
        }

        v2f vert_ver (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);

            half2 uv = v.uv;
            o.uv[0] = uv;
            o.uv[1] = uv + half2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[2] = uv - half2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
            o.uv[3] = uv + half2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
            o.uv[4] = uv - half2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;

            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            half weight[3] = {0.4026, 0.2442, 0.0545};

            fixed3 color = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
            color += tex2D(_MainTex, i.uv[1]).rgb * weight[1];
            color += tex2D(_MainTex, i.uv[2]).rgb * weight[1];
            color += tex2D(_MainTex, i.uv[3]).rgb * weight[2];
            color += tex2D(_MainTex, i.uv[4]).rgb * weight[2];

            return fixed4(color, 1.0);
        }
        ENDCG

        Pass
        {
            Name "Blur Horizontal"
            CGPROGRAM
            #pragma vertex vert_hor
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Name "Blur Vertical"
            CGPROGRAM
            #pragma vertex vert_ver
            #pragma fragment frag
            ENDCG
        }
    }

    Fallback Off
}
