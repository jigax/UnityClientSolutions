/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/


Shader "jigaX/ForceBoxelAndGradiant" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SnapVal("Threashold", Range(0,1)) = 1
		_IsInModel( "In Model", Range(0,1) ) = 0
		_MaxVal("max", float) = 1.0
		_MinVal("min", float) = 1.0
	}
	SubShader {
		Pass{
			Tags { "RenderType"="Opaque" }
			LOD 200
			
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members depth)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float _SnapVal;
			uniform bool _IsInModel;
			uniform float4 _LightColor0;
			uniform float4 _Color;
			float _MaxVal;
			float _MinVal;

        		struct v2f {
        			float4 position : SV_POSITION;
        			fixed4 color    : COLOR;
        			float2 uv       : TEXCOORD0;
					float4 modelVertPos;
        		};
			
			v2f vert( appdata_full v:POSITION ) : SV_POSITION{
				float4 tmpV = v.vertex;
				v2f o;
				if( _IsInModel == 0 ){
					if( _SnapVal != 0.0 ){
						float v = _SnapVal * 0.01;
						float x = tmpV.x - ( tmpV.x % ( v ) );
						float y = tmpV.y - ( tmpV.y % ( v ) );
						float z = tmpV.z - ( tmpV.z % ( v ) );
						float w = tmpV.w - ( tmpV.w % ( v ) );
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
				o.position = tmpV;
				float4 vertexPos = v.vertex;
				o.modelVertPos = vertexPos;

				// diffuse color
				float4 normalDirection = mul( UNITY_MATRIX_MVP, float4( v.normal, 0.0) );
				float3 surfaceNormal = normalize( normalDirection );
				float3 lightDirectionNormal = normalize(float3(_WorldSpaceLightPos0.xyz));
				
				o.color = v.color;//v.color;
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				return o; 
			}
			float4 frag(v2f i) : COLOR {
				float b = i.modelVertPos.z - 0.1;
				b = clamp( _MaxVal, _MinVal, b );
				b += 0.1;
				float2 one = float2(1.0,1.0);
				float4 c = float4( one - _ScreenParams.xy/ _ScreenParams.zw ,b, 1.0 );
				return c;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
