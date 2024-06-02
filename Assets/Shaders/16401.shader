Shader "Custom/16401"
{
	Properties
	{
		_Texture1("Texture1", 2D) = "white" {}
		_Texture2("Texture2", 2D) = "white" {}
		_Emission("Emission", 2D) = "black" {}
		_Color("Color", Color) = (1,1,1,1)
		_AlphaCut("Alpha Cutout", Range(0,1)) = 0.0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcColorBlend("Source Color Blend", Int) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstColorBlend("Destination Color Blend", Int) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcAlphaBlend("Source Alpha Blend", Int) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstAlphaBlend("Destination Alpha Blend", Int) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling", Int) = 0
		[ToggleOff] _DepthTest("Depth Test", Float) = 1.0
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 0.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 0.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
		LOD 300
		ZWrite[_DepthTest]
		Blend[_SrcColorBlend][_DstColorBlend],[_SrcAlphaBlend][_DstAlphaBlend]
		Cull[_Cull]

		CGPROGRAM
			#pragma surface surfaceFunction Standard fullforwardshadows keepalpha alphatest:_AlphaCut
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
			int _SrcColorBlend;
			int _DstColorBlend;

			void surfaceFunction(Input IN, inout SurfaceOutputStandard OUT)
			{
				fixed4 color = tex2D(_Texture1, IN.uv_Texture1) * _Color;
				color *= tex2D(_Texture2, IN.uv2_Texture2);
				OUT.Albedo = color.rgb;
				OUT.Emission = color;
				fixed4 alpha = tex2D(_Texture1, IN.uv_Texture1) * _Color;
				alpha *= tex2D(_Texture2, IN.uv2_Texture2);
				if (_SrcColorBlend == 5 && _DstColorBlend == 1)
				{
					OUT.Alpha = alpha.a * (alpha.r * 0.299f + alpha.g * 0.587f + alpha.b * 0.114f);
				}
				else
				{
					OUT.Alpha = alpha.a;
				}
				OUT.Metallic = 0;
				OUT.Smoothness = 0;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
