Shader "add" {
	Properties {
		[HDR] _Color ("Color    ", Vector) = (1,1,1,1)
		_DepahFade ("DepahFade", Float) = 0
		_Intensity ("Intensity", Float) = 0
		_intensitypower ("intensitypower", Float) = 0
		_Opacity ("Opacity", Float) = 0
		_opacitypower ("opacitypower", Float) = 0
		_MianTex ("MianTex", 2D) = "white" {}
		_MianU ("MianU", Float) = 0
		_MianV ("MianV", Float) = 0
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