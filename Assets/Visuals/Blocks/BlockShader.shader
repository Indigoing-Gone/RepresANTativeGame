Shader "Unlit/BlockShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SubTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Cutout" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

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
            sampler2D _SubTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 texCol = tex2D(_MainTex, i.uv);
                fixed4 col = pow(tex2D(_SubTex, i.uv), 2.2);
                col.a = texCol.a;
                return col;
            }
            ENDCG
        }
    }
}
