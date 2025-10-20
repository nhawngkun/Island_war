Shader "Unlit/CommonWater" {
	Properties {
		[Header(Screen Gradient)] _NearColor ("Near", Vector) = (1,1,1,1)
		_MidColor ("Mid", Vector) = (1,1,1,1)
		_FarColor ("Far", Vector) = (1,1,1,1)
		_NearDis ("NearDis", Range(0, 1)) = 1
		_FarDis ("FarDis", Range(0, 1)) = 1
		_Weight ("Weight", Range(0, 1)) = 0.5
		[Header(Cautics Conifg)] [Toggle(_CAUTICS_ON)] _CAUTICS_ON ("CAUTICS ON", Float) = 0
		_CauticsMap ("CauticsMap", 2D) = "white" {}
		_CauticsColor ("CauticsColor", Vector) = (1,1,1,1)
		_RippleSpeed ("RippleSpeed", Float) = 1
		_Strength ("Strength", Float) = 1
		_DistanceFade ("DistanceFade", Float) = 1
		_Offset ("Offset", Float) = 1
		[Header(Deep Sea)] _DepthFade ("Depth Fade", Float) = 500
		_DepthLinearStrength ("DepthLinearStrength", Range(0, 5)) = 1
		_DeepColor ("Deep", Vector) = (1,1,1,1)
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
}