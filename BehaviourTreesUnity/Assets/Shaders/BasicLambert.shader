Shader "Custom/BasicLambert"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags { "Queue" = "Geometry" }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf BasicLambert
		//#pragma target 3.0

		half4 LightingBasicLambert(SurfaceOutput s, half3 lightDir, half atten) {

		half NdotL = dot(s.Normal, lightDir);
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
		c.a = s.Alpha;
		return c;
}

		float4 _Color;

	struct Input {

		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {

		o.Albedo = _Color.rgb;
	}
		ENDCG
	}
		FallBack "Diffuse"
}
