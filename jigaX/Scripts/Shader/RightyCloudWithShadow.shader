/*-------------------------------------------------

	System Designed,
	Code Written,
	by Kunihiro Sasakawa as s2kw@jigax.jp

-------------------------------------------------*/


Shader "jigaX/SkyCircus/RightyCloudShadow" {
	Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AmbientColor ("Ambient Color", Color) = (1,1,1,1)
        _AmbientColorFact ("Ambient fact", Range(0,10) ) = 2
        _AttenFact ("Atten fact", Range(0,10) ) = 2
        _LightFact ("Light fact", Range(0,1) ) = 1
        _Alpha ("Alpha", Range(0,1) ) = 1
        _AdditionalColor("Add Color", Color) = (0,0,0,0)
	}
    SubShader {
        Tags { "RenderType" = "Opaque"}
        // This pass acts the same as the surface shader first pass.
        // Calculates per-pixel the most important directional light with shadows,
        // then per-vertex the next 4 most important lights,
        // then per-vertex spherical harmionics the rest of the lights,
        // and the ambient light value.
        Pass {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
                #pragma multi_compile_fwdbase
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                struct Input
                {
                    float4 pos : SV_POSITION;
                    float3 vlight : COLOR;
                    float4 texcoord : TEXCOORD0;
                    float3 lightDir : TEXCOORD1;
                    float3 vNormal : TEXCOORD2;
                    LIGHTING_COORDS(3,4)
                };

                fixed4 _AdditionalColor;
                float4 _MainTex_ST;
                
                Input vert(appdata_full v)
                {
                    Input o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    // Calc normal and light dir.
                    o.lightDir = normalize(ObjSpaceLightDir(v.vertex));
                    o.vNormal = normalize(v.normal).xyz;
                    // Calc spherical harmonics and vertex lights. Ripped from compiled surface shader.
                    float3 worldPos = mul(_Object2World, v.vertex).xyz;
                    float3 worldNormal = mul((float3x3)_Object2World, SCALED_NORMAL);
                    o.vlight = float3(0);
                    #ifdef LIGHTMAP_OFF
                        float3 shlight = ShadeSH9(float4(worldNormal, 1.0));
                        o.vlight = shlight;
                        #ifdef VERTEXLIGHT_ON
                            o.vlight += Shade4PointLights (
                                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                unity_4LightAtten0, worldPos, worldNormal
                                );
                        #endif // VERTEXLIGHT_ON
                    #endif // LIGHTMAP_OFF
                    TRANSFER_VERTEX_TO_FRAGMENT(o);
                    v.texcoord.xy = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
                    o.texcoord = v.texcoord;
                    
                    return o;
                }
                sampler2D _MainTex;
                fixed4 _AmbientColor;
                fixed _AmbientColorFact;
                fixed _AttenFact;
                fixed _Alpha;
                fixed _Reduct;
                fixed _LightFact;
                fixed _Y;
                float4 _LightColor0; // Contains the light color for this pass.
                half4 frag(Input IN) : COLOR
                {
                    IN.lightDir = normalize ( IN.lightDir );
                    IN.vNormal = normalize ( IN.vNormal );
                   
                    float atten = LIGHT_ATTENUATION(IN);
                    float3 color;
                    float NdotL = saturate( dot (IN.vNormal, IN.lightDir ));
                    // color = UNITY_LIGHTMODEL_AMBIENT.rgb * 2; // original
                    color = _AmbientColor.rgb * _AmbientColorFact;
                    color += IN.vlight * _LightFact;
                    color += _LightColor0.rgb * NdotL * ( atten * _AttenFact);
                    color += _AdditionalColor;
                    
                    color += tex2D( _MainTex, IN.texcoord ).rgb * _Alpha;
                    return half4(color, 1.0);
                }
               
            ENDCG
        }
 
        // Take this pass out if you don't want extra per-pixel lights.
        // Just set the other lights' Render Mode to "Not Important",
        // and they will be calculated as Spherical Harmonics or Vertex Lights in the above pass instead.
        Pass {
            Tags {"LightMode" = "ForwardAdd"}
 
            CGPROGRAM
           
                #pragma multi_compile_fwdadd
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
 
                struct Input
                {
                    float4 pos : SV_POSITION;
                    float3 lightDir : TEXCOORD1;
                    float3 vNormal : TEXCOORD2;
                };
                Input vert(appdata_full v)
                {
                    Input o;
                    o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
                    // Calc normal and light dir.
                    o.lightDir = normalize( ObjSpaceLightDir( v.vertex ));
                    o.vNormal = normalize( v.normal ).xyz;
                    // Don't need any ambient or vertex lights in here as this is just an additive pass for each extra light.
                    // Shadows won't work here, either.
                    return o;
                }
                float4 _LightColor0; // Contains the light color for this pass.
                half4 frag(Input IN) : COLOR
                {
                    IN.lightDir = normalize ( IN.lightDir );
                    IN.vNormal = normalize ( IN.vNormal );
                    float3 color;
                    float NdotL = saturate( dot (IN.vNormal, IN.lightDir ));
                    color = _LightColor0.rgb * NdotL;
                    return half4(color, 1.0f);
                }
            ENDCG
        }
    }
    FallBack "Diffuse"
}