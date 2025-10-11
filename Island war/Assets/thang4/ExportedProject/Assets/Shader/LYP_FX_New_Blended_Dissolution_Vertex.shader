Shader "LYP_FX/New/Blended_Dissolution_Vertex" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_Light ("Light", Float) = 1
		[HDR] _Color1 ("Color1", Vector) = (1,0.5471421,0,1)
		[HDR] _Color2 ("Color2", Vector) = (1,0.2712016,0,1)
		[HDR] _Color3 ("Color3", Vector) = (0.1037736,0.08566216,0.08566216,1)
		[HideInInspector] _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
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
	//CustomEditor "ShaderForgeMaterialInspector"
}