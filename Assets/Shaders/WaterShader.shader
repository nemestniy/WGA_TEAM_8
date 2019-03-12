// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Water"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_WaterTex("Water", 2D) = "white" {}
		_WaterColor ("WaterColor", Color) = (0.3529, 0.69, 1, 1)
		[PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
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
        Blend One OneMinusSrcAlpha
        
        Stencil {
		    Ref 1
		    Comp equal
		}

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing
        #pragma multi_compile _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

		sampler2D _MaskTex, _WaterTex;
		float4 _WaterColor;
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
			float4 maskColor = tex2D(_MaskTex, IN.uv_MainTex).rgba;
			//clip(0.4f - length(maskColor - _WaterColor));
			float d1 = sin(IN.uv_MainTex * 900 * float2(0.2352f, 0.654323f) + float2(_Time.y, _Time.y) * 5 * -0.4564f) * 0.2f;
			float d2 = cos(IN.uv_MainTex.yx * 900 * float2(0.63243f, 0.324235f) + float2(_Time.y, _Time.y) * 5 * 0.516f) * 0.2f;
			fixed4 c1 = tex2D(_WaterTex, IN.uv_MainTex * 50 + d1);
			fixed4 c2 = tex2D(_WaterTex, IN.uv_MainTex * 50 + d2);
			fixed4 c = (c1 + c2) * 0.5f;
			o.Alpha = length(c.rgb) * (1 - saturate(length(maskColor - _WaterColor) * 2));
			o.Albedo = o.Alpha * c;
			
        }
        ENDCG
    }

Fallback "Transparent/VertexLit"
}
