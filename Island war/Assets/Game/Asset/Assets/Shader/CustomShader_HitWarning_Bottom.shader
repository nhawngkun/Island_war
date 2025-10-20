Shader "CustomShader/HitWarning_Bottom" {
	Properties {
		_Color ("颜色", Vector) = (1,1,1,1)
		[KeywordEnum(Rect, Circle, Sector)] _Mode ("形状", Float) = 0
		_Width ("边宽", Range(0, 0.5)) = 1
		[Header(Gradiant)] _GradiantRange ("渐变范围", Range(0, 1)) = 1
		_GradiantSmooth ("边缘软硬", Range(0, 5)) = 1
		[Space(15)] [Header(Sector)] _Radian ("Angle", Range(0, 1)) = 0
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
}