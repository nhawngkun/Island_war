Shader "Unlit/CameraPostprocess" {
	Properties {
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_Overlap ("Overlap", 2D) = "black" {}
		_OverlapDark ("OverlapDark", 2D) = "black" {}
		_Param ("Param", Float) = 1
		_ParamDark ("Param Dark", Float) = 1
		[Toggle(_LINEAR_LIGHT_ON)] _LINEAR_LIGHT_ON ("Linear Light On", Float) = 0
		[Toggle(_ADDITIVE_ON)] _ADDITIVE_ON ("单纯叠加", Float) = 0
		[Toggle(_BRIGHTER_ON)] _BRIGHTER_ON ("叠加", Float) = 0
		[Toggle(_OVERLAP_ON)] _OVERLAP_ON ("Overlap On", Float) = 0
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