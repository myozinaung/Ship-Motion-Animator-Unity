#ifndef BADDOG_WATER_STRUCT
#define BADDOG_WATER_STRUCT

	struct BGWaterVertexInput
	{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
		float4 tangent: TANGENT;
		float2 texcoord : TEXCOORD0;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct BGWaterVertexOutput
	{
		UNITY_POSITION(pos);

		float2 mainWaveUV : TEXCOORD0;
		float4 secondWaveUV : TEXCOORD1;

		float4 worldTangentDir : TEXCOORD2;
		float4 worldBitangentDir : TEXCOORD3;
		float4 worldNormalDir : TEXCOORD4;

        float4 screenPos : TEXCOORD5;
        float4 grabPos : TEXCOORD6;

		UNITY_FOG_COORDS(7)

		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct BGLightingData
	{
		float3 worldPos;
		float3 worldNormal;
		float3 worldLightDir;
		float3 worldViewDir;

		half NoL;
		half NoV;
		half NoH;
		half LoH;

		half3 R;

		half3 diffuseColor;
		half3 specularColor;
		half3 lightColor;

		float2 screenUV;
		float2 grabUV;
	};

#endif

