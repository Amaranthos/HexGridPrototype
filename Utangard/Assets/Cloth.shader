// Upgrade NOTE: replaced 'PositionFog()' with multiply of UNITY_MATRIX_MVP by position
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'

// Shader "Custom/Cloth" {
// 	Properties {
// 		_Color ("Color", Color) = (1,1,1,1)
// 		_MainTex ("Albedo (RGB)", 2D) = "white" {}
// 		_Glossiness ("Smoothness", Range(0,1)) = 0.5
// 		_Metallic ("Metallic", Range(0,1)) = 0.0
// 	}
// 	SubShader {
// 		Tags { "RenderType"="Opaque" }
// 		LOD 200
// 		Cull Off
		
// 		CGPROGRAM
// 		// Physically based Standard lighting model, and enable shadows on all light types
// 		#pragma surface surf Standard fullforwardshadows

// 		// Use shader model 3.0 target, to get nicer looking lighting
// 		#pragma target 3.0

// 		sampler2D _MainTex;


// 		struct Input {
// 			float2 uv_MainTex;
// 		};

// 		half _Glossiness;
// 		half _Metallic;
// 		fixed4 _Color;

// 		void surf (Input IN, inout SurfaceOutputStandard o) {
// 			// Albedo comes from a texture tinted by color
// 			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
// 			o.Albedo = c.rgb;
// 			// Metallic and smoothness come from slider variables
// 			o.Metallic = _Metallic;
// 			o.Smoothness = _Glossiness;
// 			o.Alpha = c.a;
// 		}
// 		ENDCG
// 	} 
// 	FallBack "Diffuse"
// }
// Shader "DoubleSided" {
// 	Properties {
// 		_Color ("Main Color", Color) = (1,1,1,1)
// 		_MainTex ("Base (RGB)", 2D) = "white" {}
// 		//_BumpMap ("Bump (RGB) Illumin (A)", 2D) = "bump" {}
// 	}
// 	SubShader {    
// 		//UsePass "Self-Illumin/VertexLit/BASE"
// 		//UsePass "Bumped Diffuse/PPL"
	   
// 		// Ambient pass
// 		Pass {
// 			Name "BASE"
// 			Tags {"LightMode" = "Always" /* Upgrade NOTE: changed from PixelOrNone to Always */}
// 			Color [_PPLAmbient]
// 			SetTexture [_BumpMap] {
// 				constantColor (.5,.5,.5)
// 				combine constant lerp (texture) previous
// 			}
// 			SetTexture [_MainTex] {
// 				constantColor [_Color]
// 				Combine texture * previous DOUBLE, texture*constant
// 			}
// 		}
   
// 		// Vertex lights
// 		Pass {
// 			Name "BASE"
// 			Tags {"LightMode" = "Vertex"}
// 			Material {
// 				Diffuse [_Color]
// 				Emission [_PPLAmbient]
// 				Shininess [_Shininess]
// 				Specular [_SpecColor]
// 			}
// 			SeparateSpecular On
// 			Lighting On
// 			Cull Off
// 			SetTexture [_BumpMap] {
// 				constantColor (.5,.5,.5)
// 				combine constant lerp (texture) previous
// 			}
// 			SetTexture [_MainTex] {
// 				Combine texture * previous DOUBLE, texture*primary
// 			}
// 		}
// 	}
// 	FallBack "Diffuse", 1
// }
Shader "Cloth" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
 
Category {
    /* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
    Fog { Color [_AddFog] }
   
    #warning Upgrade NOTE: SubShader commented out; uses Unity 2.x per-pixel lighting. You should rewrite shader into a Surface Shader.
/*SubShader {
        // Ambient pass
        Pass {
            Name "BASE"
            Tags {"LightMode" = "Always" /* Upgrade NOTE: changed from PixelOrNone to Always */}
            Cull Off
            Color [_PPLAmbient]
            SetTexture [_MainTex] {constantColor [_Color] Combine texture * primary DOUBLE, texture * constant}
        }
        // Vertex lights
        // Pass {
        //  Name "BASE"
        //  Tags {"LightMode" = "Vertex"}
        //  Lighting On
        //  Material {
        //      Diffuse [_Color]
        //      Emission [_PPLAmbient]
        //  }
        //  SetTexture [_MainTex] { constantColor [_Color] Combine texture * primary DOUBLE, texture * constant}
        // }
        // Pixel lights front
        Pass {
            Name "PPL"
            Tags { "LightMode" = "Pixel" }
CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members uv,normal,lightDir)
#pragma exclude_renderers d3d11 xbox360
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_builtin
#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
#include "AutoLight.cginc"
 
struct v2f {
    float4 pos : SV_POSITION;
    LIGHTING_COORDS
    float2  uv;
    float3  normal;
    float3  lightDir;
};
 
uniform float4 _MainTex_ST;
 
v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.normal = v.normal;
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.lightDir = ObjSpaceLightDir( v.vertex );
    TRANSFER_VERTEX_TO_FRAGMENT(o);
    return o;
}
 
uniform sampler2D _MainTex;
 
float4 frag (v2f i) : COLOR
{
    // The eternal tradeoff: do we normalize the normal?
    //float3 normal = normalize(i.normal);
    float3 normal = i.normal;
       
    half4 texcol = tex2D( _MainTex, i.uv );
   
    return DiffuseLight( i.lightDir, normal, texcol, LIGHT_ATTENUATION(i) );
}
ENDCG
        }
            // Pixel lights back
        Pass {
            Name "PPL"
            Cull Front
            Tags { "LightMode" = "Pixel" }
CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members uv,normal,lightDir)
#pragma exclude_renderers d3d11 xbox360
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_builtin
#pragma fragmentoption ARB_fog_exp2
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
#include "AutoLight.cginc"
 
struct v2f {
    float4 pos : SV_POSITION;
    LIGHTING_COORDS
    float2  uv;
    float3  normal;
    float3  lightDir;
};
 
uniform float4 _MainTex_ST;
 
v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.normal = -v.normal;
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.lightDir = ObjSpaceLightDir( v.vertex );
    TRANSFER_VERTEX_TO_FRAGMENT(o);
    return o;
}
 
uniform sampler2D _MainTex;
 
float4 frag (v2f i) : COLOR
{
    // The eternal tradeoff: do we normalize the normal?
    //float3 normal = normalize(i.normal);
    float3 normal = i.normal;
       
    half4 texcol = tex2D( _MainTex, i.uv );
   
    return DiffuseLight( i.lightDir, normal, texcol, LIGHT_ATTENUATION(i) );
}
ENDCG
        }
    }*/
}
 
Fallback "VertexLit", 2
 
}