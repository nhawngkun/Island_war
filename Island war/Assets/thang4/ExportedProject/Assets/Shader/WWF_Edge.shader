Shader "WWF/Edge" {
	Properties {
		_dis ("dis", 2D) = "white" {}
		_mask ("mask", 2D) = "white" {}
		_Color ("Color", Vector) = (0.5,0.5,0.5,1)
		_Niuqu ("Niuqu", 2D) = "white" {}
		_Niuqu_U_copy ("Niuqu_U_copy", Float) = -0.3
		_Niuqu_V_copy ("Niuqu_V_copy", Float) = 0
		_Niuqu_U ("Niuqu_U", Float) = 0
		_Niuqu_V ("Niuqu_V", Float) = 0
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

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ShaderForgeMaterialInspector"
}