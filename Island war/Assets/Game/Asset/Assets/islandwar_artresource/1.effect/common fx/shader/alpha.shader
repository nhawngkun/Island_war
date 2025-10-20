Shader "YY/Alpha" {
	Properties {
		[HDR] _MainColor ("MainColor", Vector) = (1,1,1,1)
		_MainTex ("MainTex", 2D) = "white" {}
		[Enum(Material,0,Particle,1)] _UVMode ("UVMode", Float) = 0
		_U ("U", Float) = 0
		_V ("V", Float) = 0
		_Mask ("Mask", 2D) = "white" {}
		[Toggle(_RA_ON)] _RA ("R&A", Float) = 0
		_Alpha ("Alpha", Float) = 1
		_DistortValue ("DistortValue", Float) = 0
		_DistortTex ("DistortTex", 2D) = "white" {}
		_DistortU ("DistortU", Float) = 0
		_DistortV ("DistortV", Float) = 0
		[Enum(Material,0,Particle,1)] _DossolveMode ("DossolveMode", Float) = 0
		[HDR] _EdgeColor ("EdgeColor", Vector) = (1,1,1,1)
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_Dissolve ("Dissolve", Range(0, 1)) = 0
		_EdgeWidth ("EdgeWidth", Range(0, 1)) = 0
		_Hardness ("Hardness", Range(0, 1)) = 0
		_OffsetTex ("OffsetTex", 2D) = "white" {}
		_Offset ("Offset", Vector) = (0,0,0,0)
		_OffsetU ("OffsetU", Float) = 0
		_OffsetV ("OffsetV", Float) = 0
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