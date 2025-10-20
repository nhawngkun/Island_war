Shader "VFX/Noise" {
	Properties {
		[Enum(Add,1,AlphaBlend,10)] _Float3 ("选择模式", Float) = 1
		_Cutoff ("Mask Clip Value", Float) = 0.5
		[HDR] _Color0 ("颜色", Vector) = (0,0,0,0)
		_main_tex ("主帖图", 2D) = "white" {}
		_tillig ("平铺度", Vector) = (0,0,0,0)
		_NIOSE ("扭曲噪波", 2D) = "white" {}
		tilii02 ("平铺度", Vector) = (0,0,0,0)
		_Float0 ("扭曲强度", Float) = 0
		_Float1 ("扭曲U", Float) = 0
		_Float2 ("扭曲V", Float) = 0
		_mask_tex ("遮罩", 2D) = "white" {}
		[HideInInspector] _texcoord ("", 2D) = "white" {}
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
	//CustomEditor "ASEMaterialInspector"
}