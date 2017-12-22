Shader "MK/Surface/Standard[Surface]"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
		_ParallaxMap ("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


		// Blending state
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

CGINCLUDE
	// You may define one of these to expressly specify it.
	#define UNITY_BRDF_PBS BRDF1_Unity_PBS
	// #define UNITY_BRDF_PBS BRDF2_Unity_PBS
	// #define UNITY_BRDF_PBS BRDF3_Unity_PBS

	// You can reduce the time to compile by constraining the usage of eash features.
	// Corresponding shader_feature pragma should be disabled.
	// #define _NORMALMAP 1
	// #define _ALPHATEST_ON 1
	//#define _EMISSION 1
	// #define _METALLICGLOSSMAP 1
	// #define _DETAIL_MULX2 1
ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
		LOD 300

		// It seems Blend command is getting overridden later
		// in the processing of  Surface shader.
		// Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]

	CGPROGRAM
		//#pragma surface surf Standard addshadow
		#pragma multi_compile __ FADE_PLANE FADE_SPHERE
		#pragma shader_feature DISSOLVE
		#include "CGIncludes/section_clipping_CS.cginc"

		#pragma target 3.0
		// TEMPORARY: GLES2.0 temporarily disabled to prevent errors spam on devices without textureCubeLodEXT
		#pragma exclude_renderers gles


		#pragma shader_feature _NORMALMAP
		// #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma shader_feature _ALPHATEST_ON
		//#pragma shader_feature _EMISSION
		#pragma shader_feature _METALLICGLOSSMAP
		#pragma shader_feature ___ _DETAIL_MULX2
		#pragma shader_feature _PARALLAXMAP

		// may not need these (not sure)
		// #pragma multi_compile_fwdbase
		// #pragma multi_compile_fog

		#pragma surface surf Standard addshadow vertex:vert finalcolor:final fullforwardshadows // Opaque or Cutout
		// #pragma surface surf Standard vertex:vert finalcolor:final fullforwardshadows alpha:fade // Fade
		// #pragma surface surf Standard vertex:vert finalcolor:final fullforwardshadows alpha:premul // Transparent





		sampler2D _MainTex;
		sampler2D _Metallic;

		struct InputFT {
			float2 uv_MainTex;
			float3 worldPos;
			float4 screenPos;
		};

		//half _Smoothness;
		//float _Metallic;
		fixed4 _Color;

		void surf(InputFT IN, inout SurfaceOutputStandard o)
		{
			#if (FADE_PLANE || FADE_SPHERE) && DISSOLVE

			float4 fade = PLANE_FADE(IN.worldPos);

			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			screenUV *= float2(_NoiseScaleScreen*_ScreenParams.x / 512,_NoiseScaleScreen*_ScreenParams.y / 512);
			float f = tex2D(_Noise, screenUV).r;
			bool eval = f >= fade.a&&fade.a<1;
			if (_inverse == 1) eval = !eval;
			if (eval) discard;

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 metal = tex2D(_Metallic, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Metallic = metal.r;
			o.Smoothness = metal.a;
			//o.Smoothness = _Smoothness;
			o.Alpha = c.a;

			o.Albedo *= fade.rgb * 2;

			#else

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 metal = tex2D(_Metallic, IN.uv_MainTex);

			o.Albedo = c.rgb;
			o.Metallic = metal.r;
			o.Smoothness = metal.a;
			// o.Smoothness = _Smoothness;
			o.Alpha = c.a;

			#endif
		}

		#include "CGIncludes/StandardSurface.cginc"

	ENDCG

		// For some reason SHADOWCASTER works. Not ShadowCaster.
		// UsePass "Standard/ShadowCaster"
		UsePass "Standard/SHADOWCASTER"
	}

	FallBack Off
	CustomEditor "StandardShaderGUI"
}
