Shader "Fx_bend_02" {
	Properties {
		[HDR] _Color ("Color    ", Vector) = (1,1,1,1)
		_GradientTex ("GradientTex", 2D) = "white" {}
		_DepahFade ("DepahFade", Float) = 0
		_Intensity ("Intensity", Float) = 1
		_intensitypower ("intensitypower", Float) = 1
		_Opacity ("Opacity", Float) = 1
		_opacitypower ("opacitypower", Float) = 1
		_MianTex ("MianTex", 2D) = "white" {}
		_MianU ("MianU", Float) = 0.1
		_MianV ("MianV", Float) = 0.1
		_A1 ("A1", 2D) = "white" {}
		_A1U ("A1U", Float) = 0
		_AIV ("AIV", Float) = 0
		_SoftDissolve ("SoftDissolve", 2D) = "white" {}
		_Dissolvesoft ("Dissolvesoft", Float) = 0.5
		_DissolveIntensity ("DissolveIntensity", Range(0, 1.05)) = 0.5
		[Toggle(_DISSOLVE_ON)] _dissolve ("dissolve", Float) = 0
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