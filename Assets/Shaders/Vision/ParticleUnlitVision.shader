// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Particles/Detective Unlit"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Mask("Stencil visibility", Int) = 1
		_EmissionMultiply("Emission mult", Float) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Stencil {
				Ref[_Mask]
				Comp equal
			}

			CGPROGRAM
			#pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"
			float _EmissionMultiply;
			struct Input
			{
				float2 uv_MainTex;
				fixed4 color;
			};

			void vert(inout appdata_full v, out Input o)
			{
				v.vertex = UnityFlipSprite(v.vertex, _Flip);

				#if defined(PIXELSNAP_ON)
				v.vertex = UnityPixelSnap(v.vertex);
				#endif

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = v.color * _Color * _RendererColor;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
				o.Albedo = c.rgb * c.a;
				o.Emission = o.Albedo * _EmissionMultiply;
				o.Alpha = c.a;
			}
			ENDCG
		}

			Fallback "Transparent/VertexLit"
}
