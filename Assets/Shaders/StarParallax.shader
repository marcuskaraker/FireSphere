Shader "Unlit/StarParallax"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		_MidTex ("Mid Texture", 2D) = "black" {}
		_BackTex ("Back Texture", 2D) = "black" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;

                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _MidTex;
			sampler2D _BackTex;
			sampler2D _NoiseTex;
            float4 _MainTex_ST;
			float4 _MidTex_ST;
			float4 _BackTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _MidTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _BackTex);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = (tex2D(_MainTex, i.uv0) + tex2D(_MidTex, i.uv1) + tex2D(_BackTex, i.uv2)) * tex2D(_NoiseTex, i.uv0);
                return col;
            }
            ENDCG
        }
    }
}
