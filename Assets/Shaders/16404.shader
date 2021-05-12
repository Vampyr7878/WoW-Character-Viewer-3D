Shader "Custom/16404"
{
	Properties
	{
		_Texture1("Texture1", 2D) = "white" {}
		_Texture2("Texture1", 2D) = "white" {}
		_Emission("Emission", 2D) = "black" {}
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCut("Alpha Cutout", Range(0,1)) = 0.0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Int) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Int) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling", Int) = 0
		[ToggleOff] _DepthTest("Depth Test", Float) = 1.0
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 0.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		ZWrite[_DepthTest]
		Blend[_SrcBlend][_DstBlend]
		Cull[_Cull]

		CGPROGRAM
			#pragma surface surfaceFunction Standard alpha:blend fullforwardshadows
			#pragma target 3.0
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF

			struct Input
			{
				float2 uv_Texture1;
				float2 uv2_Texture2;
			};

			sampler2D _Texture1;
			sampler2D _Texture2;
			fixed4 _Color;

			void surfaceFunction(Input IN, inout SurfaceOutputStandard OUT)
			{
				fixed4 color = tex2D(_Texture1, IN.uv_Texture1) * tex2D(_Texture2, IN.uv2_Texture2) * _Color;
				fixed4 emission = tex2D(_Texture2, IN.uv2_Texture2);
				OUT.Albedo = color.rgb * 2;
				OUT.Alpha = color.a;
				OUT.Metallic = 0;
				OUT.Smoothness = 0;
				OUT.Emission = emission;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
