Shader "ICE" {
	Properties {
		_Smoothness ("Smoothness", Range(0, 1)) = 1
		[HDR] _Color0 ("Color 0", Vector) = (1,1,1,1)
		_Emission_int ("Emission_int", Float) = 0
		_light_dir ("light_dir", Vector) = (1,1,1,0)
		_scale ("scale", Float) = 1
		_offset ("offset", Float) = 1
		_Opacity_int ("Opacity_int", Float) = 0.5
		_light_int ("light_int", Float) = 1
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