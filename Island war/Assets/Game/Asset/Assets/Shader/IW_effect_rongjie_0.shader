Shader "IW/effect/rongjie" {
	Properties {
		_Color ("Color", Vector) = (0.5,0.5,0.5,0.5)
		_LineWidth ("Burn Line Width", Range(0, 0.5)) = 0.1
		[HDR] _BumpFirstColor ("Burn First Color", Vector) = (0.5,0.5,0.5,1)
		[HDR] _BumpSecondColor ("Burn Second Color", Vector) = (0.5,0.5,0.5,1)
		_MainTex ("Main Tex", 2D) = "white" {}
		_BurnMap ("Burn Map", 2D) = "while" {}
		[Enum(Additive,1,AlphaBlend,11)] _BlendMode ("Blend Mode", Float) = 11
		[KeywordEnum(Off, Front, Back)] _CullMode ("CullMode", Float) = 2
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
	Fallback "VertexLit"
}