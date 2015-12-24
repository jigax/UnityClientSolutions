/*-------------------------------------------------
	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp
-------------------------------------------------*/


Shader "jigaX/SkyCircus/Wave" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        _X ("X", Range(0,1) ) = 0.1
        _Y ("Y", Range(0,1) ) = 0.1
        _Z ("Z", Range(0,1) ) = 0.1
        _Speed ("Speed", float ) = 0.1
        _WaveTex("Wave (Gray)", 2D) = "gradiant"{}
        _FactTex("Fact (Gray)", 2D) = "gradiant"{}
	}
	SubShader {
        Pass{
		Tags { "RenderType"="Opaque"}
		LOD 400
		Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
        };

		fixed4 _Color;
		sampler2D _MainTex;
        sampler2D _WaveTex;
        float4 _WaveTex_ST;
        sampler2D _FactTex;
        fixed _X,_Y,_Z;
        
        Input vert( appdata_full v ){
            Input o;
            o.uv0 = tex2D( _MainTex, v.texcoord.xy );

            v.texcoord.xy = (v.texcoord.xy * _WaveTex_ST.xy) + _WaveTex_ST.zw;

            fixed4 wave = normalize(tex2D ( _WaveTex, v.texcoord.xy ));
            fixed4 fact = normalize(tex2D ( _FactTex, v.texcoord.xy ));
            fixed result = ( wave.r - 0.5 ) * fact.r;
            v.vertex.x += result * _X;
            v.vertex.z += result * _Z;
            v.vertex.y += result * _Y;

            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            return o; 
        }

        fixed4 frag( Input IN ) : SV_Target{
            float4 c = tex2D ( _MainTex, IN.uv0) * _Color;
            return c;
        }
		// void surf (Input IN, inout SurfaceOutputStandard o) {
		// 	//Albedo comes from a texture tinted by color
		// 	fixed4 c = tex2D (_MainTex, IN.uv0) * _Color;
		// 	o.Albedo = c.rgb;
		// }
		ENDCG
        }
	} 
	FallBack "Diffuse"
}