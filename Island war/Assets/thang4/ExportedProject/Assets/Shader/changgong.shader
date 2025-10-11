Shader "changgong" {
	Properties {
		_miantex ("miantex", 2D) = "white" {}
		_intensity ("intensity", Float) = 1
		_DepahFade ("DepahFade", Float) = 0
		_opacity ("opacity", Float) = 1
		_ero ("ero", 2D) = "white" {}
		_au ("au", Float) = 0
		_M_U ("M_U", Float) = 0
		_softdove ("softdove", Range(0, 1.05)) = 0.5
		_av ("av", Float) = 0
		_M_v ("M_v", Float) = 0
		_opacpower ("opacpower", Float) = 0
		_intenpowert ("intenpowert", Float) = 0
		[HDR] _Color ("Color ", Vector) = (1,1,1,1)
		_a_mask ("a_mask", 2D) = "white" {}
		_diss ("diss", 2D) = "white" {}
		_d_u ("d_u", Float) = 0
		_d_v ("d_v", Float) = 0
		_d_intensity ("d_intensity", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
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

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}