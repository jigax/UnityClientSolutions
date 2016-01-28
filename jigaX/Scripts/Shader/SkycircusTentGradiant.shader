/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/


Shader "SkyCircus/TentGradiant" {
	Properties {
		_TopColor ("U + Color", Color) = (1,1,1,1)
		_BotColor ("U - Color", Color) = (1,1,1,1)
		_LeftColor ("V + Color", Color) = (1,1,1,1)
		_RightColor ("V - Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Factor ("Factor", Float) = 0.5
        _TimeFactor( "Time Factor", Float ) = 0
        _UorV ("Use U vector?", Range(0,1) ) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True"}
		LOD 200
        Material {
            Diffuse (1,1,1,1)
            Ambient (1,1,1,1)
            Specular (0,0,0,0)
        }
        Lighting Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting// fullforwardshadows
        #include "UnityCG.cginc" 
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		sampler2D _MainTex;
        fixed4 _TopColor;
        fixed4 _BotColor;
        fixed4 _LeftColor;
        fixed4 _RightColor;
        float _TimeFactor;
        fixed _Factor;


        fixed4 LightingNoLighting( SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo; 
            c.a = s.Alpha;
            return c;
        }

        struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
        fixed _UorV;
        float cut( float val ){
            val = val + _Time.x * _TimeFactor;
            if( val > 1 ){
                return val - 1;
            }
            return val;
        }

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
            float2 uv = float2(
                cut( IN.uv_MainTex.x ),
                cut( IN.uv_MainTex.y )                
            );
			fixed4 c = tex2D (_MainTex, uv );
            if( _UorV >= 0.5 ){
    			o.Albedo = c.rgb + lerp( _TopColor.rgb, _BotColor.rgb, uv.x ) * _Factor;
            }else{
                o.Albedo = c.rgb + lerp( _LeftColor.rgb, _RightColor.rgb, uv.y ) * _Factor;                
            }
			o.Alpha = 1;
		}
		ENDCG
        
        
        
        
	} 
	FallBack "Diffuse"
}
