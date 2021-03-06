﻿// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Fog"
{
    Properties
    {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "white" {}
		_FogTex("Fog", 2D) = "white" {}
		_DetailTex("Details", 2D) = "white" {}
		_NoiseTimeMult("Noise animation speed multiplier", Float) = 0.05
		_NoiseMult("Noise offset multiplier", Float) = 0.05
		_FogTimeMult("Fog animation speed multiplier", Float) = 1
		_FogMult("Fog rotation offset multiplier", Float) = 1
		_FogTimeMult2("Fog movement multiplier", Float) = 1
		_DetailTimeMult("Detail animation speed multiplier", Float) = 1
		_DetailMult("Detail rotation offset multiplier", Float) = 1
		_DetailTimeMult2("Detail movement multiplier", Float) = 1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha DstAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

		sampler2D _NoiseTex,_FogTex, _DetailTex;
		float4 _NoiseTex_ST, _FogTex_ST, _DetailTex_ST;
		float _NoiseMult, _FogMult, _DetailMult;
		float _NoiseTimeMult, _FogTimeMult, _DetailTimeMult;
		float _FogTimeMult2, _DetailTimeMult2;
        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };

        void vert (inout appdata_full v, out Input o)
        {
            v.vertex = UnityFlipSprite(v.vertex, _Flip);

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap (v.vertex);
            #endif

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color * _RendererColor;
        }		
        void surf (Input IN, inout SurfaceOutput o)
        {
			float _PI = 3.14159265359f;
			float _PI2 = 3.14159265359f * 2;

			float d1 = IN.uv_MainTex * 15 * float2(0.2352f, 0.654323f) + float2(_Time.y, _Time.y) * _NoiseTimeMult * -0.4564f;
			float d2 = IN.uv_MainTex.yx * 15 * float2(0.63243f, 0.324235f) + float2(_Time.y, _Time.y) * _NoiseTimeMult * 0.516f;
			float4 maskColor = tex2D(_NoiseTex, (IN.uv_MainTex) *_NoiseTex_ST.xy +  float2(d1, d2) * _NoiseMult).rgba;

			float angle = (1 + maskColor.r * 5 + _Time.y * _FogTimeMult) % _PI2;
			float angle2 = (1 + maskColor.g * 5 + _Time.y * _DetailTimeMult) % _PI2;
			float2 delta = (1 + maskColor.b) * (cos(angle), sin(angle));
			float2 delta2 = (1 + maskColor.a) * (cos(angle), sin(angle2));

			float c1 =  pow(tex2D(_FogTex, (IN.uv_MainTex ) * _FogTex_ST + delta * _FogMult + _Time.x * _FogTimeMult2).a, 2);
			float c2 = pow(tex2D(_DetailTex, (IN.uv_MainTex ) * _DetailTex_ST + delta2 * _DetailMult + _Time.x * _DetailTimeMult2).a, 2);
			o.Alpha = c1 * 0.1f;
			o.Albedo = c2 * float3(0, 1, 0.8);
			o.Emission = float3(1, 1, 1);
			
        }
        ENDCG
    }

Fallback "Transparent/VertexLit"
}
