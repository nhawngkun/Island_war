Shader "Blend_Shader" {
	Properties {
		_Main_Tex ("Main_Tex", 2D) = "white" {}
		[HDR] _Main_Color ("Main_Color", Vector) = (0,0,0,0)
		_Emission_int ("Emission_int", Float) = 0
		_Opacity_int ("Opacity_int", Float) = 0
		_Power_int ("Power_int", Float) = 0
		_Main_UV ("Main_UV", Vector) = (0,0,0,0)
		_Smoothstep_int ("Smooth step_int", Float) = 0
		_RJ_1 ("RJ_1", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] _tex4coord2 ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
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

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}