﻿/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/


Shader "jigaX/SkyCircus/RightyCloud" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Ramp ("Ramp Map", 2D ) = "white" {}
        _RambertFact ("Rambert fact", Range(0,1) ) = 0.5
        _Alpha("Alpha", Range(0,1)) = 1
        _AdditionalColor("Add Color", Color) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
        Lighting On
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Common
        float _RambertFact;
        fixed _Alpha;
        sampler2D _Ramp;
        fixed4 _AdditionalColor;
        half4 LightingCommon (SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            half diff = NdotL * _RambertFact + _RambertFact;
            //half3 ramp = tex2D (_Ramp, float2( diff )).rgb; // original
            half4 c;
            // c.rgb = s.Albedo * _LightColor0.rgb * ramp * atten; // original
            c.rgb = ( s.Albedo * _LightColor0.rgb * diff ) + _AdditionalColor;
            c.a = s.Alpha;
            return c;
        }
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
			//o.Alpha = _Alpha;
            return;
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color; //original
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}