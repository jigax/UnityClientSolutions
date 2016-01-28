Shader "jigaX/ForceBoxel" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SnapVal("Threashold", Range(0,1)) = 1
		_IsInModel( "In Model", Range(0,1) ) = 0
	}
	SubShader {
		Pass{
			Tags { "RenderType"="Opaque" }
			LOD 200
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float _SnapVal;
			uniform bool _IsInModel;
			uniform sampler2D _MainTex;
			uniform float4 _LightColor0;
			uniform float4 _Color;
        		struct v2f {
        			float4 position : SV_POSITION;
        			fixed4 color    : COLOR;
        			float2 uv       : TEXCOORD0;
        		};
			
			v2f vert( appdata_full appdata ){
				float4 tmpV = appdata.vertex;
				if( _IsInModel == 0 ){
					if( _SnapVal != 0.0 ){
						float val = _SnapVal * 0.01;
						float x = tmpV.x - ( tmpV.x % ( val ) );
						float y = tmpV.y - ( tmpV.y % ( val ) );
						float z = tmpV.z - ( tmpV.z % ( val ) );
						float w = tmpV.w - ( tmpV.w % ( val ) );
						tmpV = float4( x, y, z, w );
					}
					tmpV = mul ( UNITY_MATRIX_MV, tmpV ); 		
				}else{
					tmpV = mul ( UNITY_MATRIX_MV, tmpV );
					if( _SnapVal != 0.0 ){
						float x = tmpV.x - ( tmpV.x % _SnapVal );
						float y = tmpV.y - ( tmpV.y % _SnapVal );
						float z = tmpV.z - ( tmpV.z % _SnapVal );
						float w = tmpV.w - ( tmpV.w % _SnapVal );
						tmpV = float4( x, y, z, w );
					}
				}
				tmpV = mul (UNITY_MATRIX_P, tmpV);
				v2f o;
				o.position = tmpV;

				// diffuse color
				float4 normalDirection = mul( UNITY_MATRIX_MVP, float4( appdata.normal, 0.0) );
				float3 surfaceNormal = normalize( normalDirection );
				float3 lightDirectionNormal = normalize(float3(_WorldSpaceLightPos0.xyz));
				float halfLambert = max(0.0, dot(surfaceNormal.xy,lightDirectionNormal.xy)) * 0.5 + 0.5;
				float3 diffuseReflection = float3(_LightColor0.xyz) * float3(_Color.rgb) * halfLambert * halfLambert;
				float4 color = float4(diffuseReflection, 1.0);
				
				o.color = color;//v.color;
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, appdata.texcoord);
				
				return o; 
			}
			float4 frag(v2f i) : COLOR {
        			return i.color;
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
