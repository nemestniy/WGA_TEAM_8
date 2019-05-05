Shader "Custom/MaskFakeEffect"
 {
	Properties 
	{
	}
	SubShader 
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True"}
		Zwrite off
		Blend Zero OneMinusSrcAlpha 

		Lighting Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0



		struct Input
		{
			float2 uv_MainTex;
			float4 colorAlpha;  
		};

		void vert(inout appdata_full v, out Input o)
		{

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.colorAlpha = v.color.rgba;
		}


		void surf(Input IN, inout SurfaceOutput o) 
		{
			o.Emission = float3(0, 0, 0);
			o.Alpha = IN.colorAlpha.a;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
