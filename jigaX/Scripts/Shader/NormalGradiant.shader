/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/


Shader "jigaX/NormalGradiant" {
	Properties {
		_ConcurrentColor ("Concurrent Color", Color) = (1,1,1,1)
		_VerticalColor ("VerticalColor", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Factor1 ("Factor1", Range(-10,10)) = 0.0
		_Factor2 ("Factor2", Range(-1,1)) = 0.0
	}

    CGINCLUDE
    #include "UnityCG.cginc"

    ENDCG
    
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200		
		// Physically based Standard lighting model, and enable shadows on all light types

		// Use shader model 3.0 target, to get nicer looking lighting

        Pass{
            Cull Off
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members angle)
            #pragma exclude_renderers d3d11 xbox360
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag            
            #pragma target 3.0
            sampler2D _MainTex;
            float4 _ConcurrentColor;
            float4 _VerticalColor;
            float _Factor1;
            float _Factor2;
            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR0;
                float angle;
                float lerpA;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                //float cameraPos = _
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex );
                float3 angle = mul (UNITY_MATRIX_MVP, float4(v.normal,0) ).xyz;
                angle = normalize( angle );
                o.angle = (angle.x + angle.y + angle.z) / 3 * _Factor1;
                o.lerpA = normalize( _WorldSpaceCameraPos - o.pos );
                
                //half3 frontVec = (0,0,1);                                   //Set front vector
                half3 frontVec = mul((half3x3)_Object2World, half3(0, 0, 1));
                half3 rotVec = mul(_Object2World, fixed4(0,0,0,1)).xyz - _WorldSpaceCameraPos;                            //Calculate current rotation vector
                
                //half grad = acos(dot(normalize(rotVec.xz), frontVec.xz));           //Calculate rotational angle                
                half grad = atan2(rotVec.x, rotVec.z);
                
                //o.lerpA = grad;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 color = lerp(_ConcurrentColor, _VerticalColor, i.angle ) * _Factor2;
                return color;//fixed4 (i.color, 1);
            }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
