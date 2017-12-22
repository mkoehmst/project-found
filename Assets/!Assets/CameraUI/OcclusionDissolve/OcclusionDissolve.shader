Shader "MK/Surface/OcclusionDissolve"
{

	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		//_Smoothness("Smoothness", 2D) = "white" {}
        //_Metallic ("Metallic", Range(0,1)) = 0.0
		_Metallic("Metallic", 2D) = "white" {}
		//_spread ("fadeSpan", Range(0,1)) = 1.0
		[Toggle] _inverse("inverse", Float) = 0
		[HideInInspector][Toggle(DISSOLVE)] _dissolve("dissolveTexture", Float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Clipping" }
        LOD 200
		//Cull Off

        CGPROGRAM

        #pragma surface surf Standard addshadow

		#pragma multi_compile __ FADE_PLANE FADE_SPHERE
		#pragma shader_feature DISSOLVE
		#include "CGIncludes/section_clipping_CS.cginc"

        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _Metallic;

        struct Input {
            float2 uv_MainTex;
			float3 worldPos;
			float4 screenPos;
        };

        //half _Smoothness;
        //float _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

			#if (FADE_PLANE || FADE_SPHERE) && DISSOLVE

			float4 fade = PLANE_FADE(IN.worldPos);

			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			screenUV *= float2(_NoiseScaleScreen*_ScreenParams.x/512,_NoiseScaleScreen*_ScreenParams.y/512);
			float f = tex2D (_Noise, screenUV).r;
			bool eval = f>=fade.a&&fade.a<1;
			if(_inverse==1) eval = !eval;
			if(eval) discard;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 metal = tex2D(_Metallic, IN.uv_MainTex);
            o.Albedo = c.rgb;
			o.Metallic = metal.r;
			o.Smoothness = metal.a;
            //o.Smoothness = _Smoothness;
            o.Alpha = c.a;

			o.Albedo *= fade.rgb*2;

			#else

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 metal = tex2D(_Metallic, IN.uv_MainTex);

            o.Albedo = c.rgb;
            o.Metallic = metal.r;
			o.Smoothness = metal.a;
           // o.Smoothness = _Smoothness;
            o.Alpha = c.a;

			#endif
        }

        ENDCG
    }
    FallBack "Standard"
}
