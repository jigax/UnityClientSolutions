/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/


Shader "jigaX/SkyCircus/Wave" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        
        _X ("X", Range(0,1) ) = 0.1
        _Y ("Y", Range(0,1) ) = 0.1
        _Z ("Z", Range(0,1) ) = 0.1
        _Speed ("Speed", float ) = 0.1
        _WaveTex("Wave (Gray)", 2D) = "gradiant"{}
        _FactTex("Fact (Gray)", 2D) = "gradiant"{}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
            float4 pos : SV_POSITION;
            float4 texcoord : TEXCOORD0;
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        sampler2D _WaveTex;
        float4 _WaveTex_ST;
        sampler2D _FactTex;
        fixed _X,_Y,_Z;
        Input vert( inout appdata_full v ){
            Input o;
            o.uv = tex2D( _MainTex, v.texcoord.xy );

            v.texcoord.xy = (v.texcoord.xy * _WaveTex_ST.xy) + _WaveTex_ST.zw;

            fixed4 wave = normalize(tex2D ( _WaveTex, v.texcoord.xy ));
            fixed4 fact = normalize(tex2D ( _FactTex, v.texcoord.xy ));
            fixed result = ( wave.r -0.5) * fact.r;
            v.vertex.x += result * _X;
            v.vertex.z += result * _Z;
            v.vertex.y += result * _Y;

            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            return o; 
        }


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
