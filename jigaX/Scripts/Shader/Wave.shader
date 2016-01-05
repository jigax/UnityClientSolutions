/*-------------------------------------------------
    System Designed,
    Code Written,
    by Kunihiro Sasakawa as s2kw@jigax.jp
-------------------------------------------------*/


Shader "jigaX/FlagWave" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _EmmisionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionColorFact  ("Emission Color Fact", Range(1,10)) = 1.0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        _FactX ( "X Fact", Range(0,10) ) = 0.01
        _FactY ( "Y Fact", Range(0,10) ) = 0.01
        _FactZ ( "Z Fact", Range(0,10) ) = 0.01
        _Times ( "Time", float ) = 0.0
        _Waves ( "Wave", Range(0, 100) ) = 1.0
        _Ignore( "Ignore range", Range(0,100)) = 0.0

    }
    SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 200
            Cull Off
            
            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows vertex:vert
            //#pragma multi_compile_fwdbase
            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "jigaX.cginc"
            struct Input
            {
                float3 color : COLOR;
                float2 uv_MainTex : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _WaveTex_ST;
            fixed _FactX,_FactY,_FactZ;
            float _Times,_Waves,_Ignore;

            void vert( inout appdata_full v )   {
//          UNITY_INITIALIZE_OUTPUT(v2f, o); // must initialize 
                float3 fact = ( _FactX,_FactY,_FactZ );
                v.vertex = vert_wave( v.vertex, _Waves, _Times, _Ignore, fact );
            }

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            fixed4 _EmmisionColor;
            fixed _EmissionColorFact;

            void surf( Input IN, inout SurfaceOutputStandard o ){
                fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color + _EmmisionColor * _EmissionColorFact;
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }

            ENDCG
    } 
    FallBack "Diffuse"
}