Shader "UI/2DFoam" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		_Range ("Y Gradient", Float) = 1
		_sinWeight ("Sin Weight", Range(0, 5)) = 0.5
		_texWeight ("Texture Weight", Range(0, 5)) = 0.5
		_WaveWidth ("Wave Width", Float) = 10
		_WaveDensity ("Wave Density", Float) = 5
		_Speed ("Speed", Float) = 1
		_DistortStrength ("Distort Strength", Float) = 25
		_DistortDensity ("Distort Density", Range(0, 5)) = 0.5
		_FoamTex ("FoamTex", 2D) = "white" {}
		_WaveTilling ("Foam Tilling", Float) = 1
		_NoiseSpeed ("Texture Speed", Float) = 1
		_NoiseStrength ("Noise Strength", Float) = 1
		_NoiseTilling ("Noise Tilling", Float) = 1
		_YFactor ("Factor Y", Float) = 1
		_YBase ("Base Y", Float) = 1
		[HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector] _ColorMask ("Color Mask", Float) = 15
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
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, float2(input.uv.x, input.uv.y)) * _Color;
			}

			ENDHLSL
		}
	}
}