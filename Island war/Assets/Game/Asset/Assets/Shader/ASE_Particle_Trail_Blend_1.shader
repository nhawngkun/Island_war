Shader "ASE/Particle_Trail_Blend" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		[HDR] _MainTexture ("Main Texture", 2D) = "white" {}
		[HDR] _Tint ("Tint", Vector) = (1,1,1,0)
		_MinSpeedU ("Min Speed U", Float) = 0
		_MinSpeedV ("Min Speed V", Float) = 0
		[HDR] _DissloveTexture ("Disslove Texture", 2D) = "white" {}
		_DissloveSpeedV ("Disslove Speed V", Float) = 0
		_DissloveSpeedU ("Disslove Speed U", Float) = 0
		_Intensity ("Intensity", Float) = 0.1
		_Gardient ("Gardient", Float) = 0.74
		_Mask ("Mask", 2D) = "white" {}
		[HideInInspector] _texcoord ("", 2D) = "white" {}
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
	//CustomEditor "ASEMaterialInspector"
}