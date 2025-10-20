Shader "QFX/ProjectilesFX/Trail" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Float) = 10
		_EmissiveMultiply ("Emissive Multiply", Float) = 1
		_OpacityMultiply ("Opacity Multiply", Float) = 1
		_MainTiling ("Main Tiling", Vector) = (1,1,1,1)
		_MainTexturePower ("Main Texture Power", Float) = 1
		[KeywordEnum(None,Add,Lerp)] _Blend ("Blend", Float) = 0
		_TimeScale1 ("Time Scale 1", Float) = 1
		_TimeScale2 ("Time Scale 2", Float) = 1
		[Toggle] _UseTextureMaskAlpha ("Use Texture Mask Alpha", Float) = 1
		_TextureMaskAlpha ("Texture Mask Alpha", 2D) = "white" {}
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