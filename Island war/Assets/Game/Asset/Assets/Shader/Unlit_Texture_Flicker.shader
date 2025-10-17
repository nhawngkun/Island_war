Shader "Unlit/Texture_Flicker" {
	Properties {
		[Header(Main)] _Tint ("Tint", Vector) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[Header(Light)] [Toggle(_HANDLE_LIGHT)] _HANDLE_LIGHT ("Handle Light", Range(0, 1)) = 0
		_LambertInterval ("Lambert interval", Range(0, 0.5)) = 0.5
		_ShadowAttenuation ("Shadow Attenuation", Range(1, 10)) = 3
		_Diffuse ("Diffuse", Range(0, 1)) = 0.22
		[Header(Flicker)] [Toggle(_SELF_FLICKER)] _SELF_FLICKER ("Enable Flicker", Range(0, 1)) = 0
		_Speed ("Speed", Range(0, 5)) = 2.35
		_Min ("Min", Range(0, 1)) = 0.75
		_Max ("Max", Range(0, 1)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_MatrixMVP;

			struct Vertex_Stage_Input
			{
				float3 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixMVP, float4(input.pos, 1.0));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, float2(input.uv.x, input.uv.y));
			}

			ENDHLSL
		}
	}
}