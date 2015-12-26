/*-------------------------------------------------
	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp
-------------------------------------------------*/


Shader "jigaX/FlagWave" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        _FactX ( "X Fact", Range(0,100) ) = 0.01
        _FactY ( "Y Fact", Range(0,100) ) = 0.01
        _FactZ ( "Z Fact", Range(0,100) ) = 0.01
        _Times ( "Time", float ) = 0.0
        _Waves ( "Wave", Range(0, 100) ) = 1.0
        _Ignore( "Ignore range", Range(0,100)) = 0.0

    }


    SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 200
            Cull Off
            
            CGPROGRAM
            #pragma surface surf Lambert vertex:vert
            //#pragma multi_compile_fwdbase
            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct Input
            {
                float3 color : COLOR;
                float3 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _WaveTex_ST;
            fixed _FactX,_FactY,_FactZ,_Ignore;
            float _Times,_Waves;
            half4 _Color;

            float waveOffset( float _p ){
                float p = abs( _p );
                float sinV = cos( p * _Waves - _Times );
                float reduct = clamp ( p / _Ignore, 0, 1 );
                return sinV * reduct;
            }

            void vert( inout appdata_full v )   {
//          UNITY_INITIALIZE_OUTPUT(v2f, o); // must initialize 

                float4 vPos = v.vertex;
                vPos.w = v.vertex.w;

                vPos.x = v.vertex.x + waveOffset( v.vertex.y ) * _FactX;
                vPos.y = v.vertex.y + waveOffset( v.vertex.z ) * _FactY;
                vPos.z = v.vertex.z + waveOffset( v.vertex.x ) * _FactZ;
                v.vertex = vPos;
            }
            void surf( Input IN, inout SurfaceOutput o ){
                half4 c = tex2D (_MainTex, IN.texcoord);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }

            ENDCG
    } 
    FallBack "Diffuse"
}