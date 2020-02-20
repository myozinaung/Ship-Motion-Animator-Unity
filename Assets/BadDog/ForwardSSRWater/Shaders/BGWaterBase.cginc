#ifndef BADDOG_WATER_BASE
#define BADDOG_WATER_BASE

	sampler2D _MainWave;
	float4 _MainWave_ST;

	sampler2D _SecondWave;
	float4 _SecondWave_ST;

	half _MainWaveBumpScale;
	half _SecondWaveBumpScale;

	half4 _MainWaveTilingOffset;
	half4 _SecondWaveTilingOffset;
	half4 _ThirdWaveTilingOffset;

	half _WaterDepthOffset;
	half _WaterMuddyScale;
	half _WaterDistortScale;

	half4 _WaterBaseColor;
	half4 _WaterMuddyColor;

	half _SpecularIntensity;
	half _EnviromentIntensity;

	half _SSRMaxSampleCount;
	half _SSRSampleStep;
	half _SSRIntensity;

	uniform sampler2D _BGWater_GrabTexture;
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);


	#include "./BGWaterStruct.cginc"
	#include "./BGWaterSSR.cginc"
	#include "./BGWaterLighting.cginc"


	BGWaterVertexOutput VertexCommon(BGWaterVertexInput v)
	{
		BGWaterVertexOutput o;

		UNITY_INITIALIZE_OUTPUT(BGWaterVertexOutput, o);

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		// float4 worldPos2 = mul(unity_ObjectToWorld, v.vertex); // added by MYO
		// y = A*sin(wt + xpos_phase)
		
		// float elevation = 5.0 * sin(1 * _Time.y +1*dot((1, 0),worldPos2.xyz.xz); // added by MYO
		// worldPos2.y = worldPos2.y + elevation; // added by MYO
		// o.pos = mul(UNITY_MATRIX_VP, worldPos2); // added by MYO
		o.pos = UnityObjectToClipPos(v.vertex); // cmt out by MYO
	
		o.mainWaveUV = v.texcoord * _MainWaveTilingOffset.xy + _Time.r * _MainWaveTilingOffset.zw * 0.1;
		o.secondWaveUV.xy = v.texcoord * _SecondWaveTilingOffset.xy + _Time.r * _SecondWaveTilingOffset.zw * 0.1;
		o.secondWaveUV.zw = v.texcoord * _ThirdWaveTilingOffset.xy + _Time.r * _ThirdWaveTilingOffset.zw * 0.1;

		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		float3 worldNormal = UnityObjectToWorldNormal(v.normal);

		float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;

		o.worldTangentDir = float4(worldTangent, worldPos.x);
		o.worldBitangentDir = float4(worldBinormal, worldPos.y);
		o.worldNormalDir = float4(worldNormal, worldPos.z);

		UNITY_TRANSFER_FOG(o,o.pos);

		o.screenPos = ComputeScreenPos(o.pos);
		COMPUTE_EYEDEPTH(o.screenPos.z);

		o.grabPos = ComputeGrabScreenPos(o.pos);

		return o;
	}

#if defined(UNITY_PASS_FORWARDBASE)

	BGWaterVertexOutput VertexForwardBase(BGWaterVertexInput vertexInput)
	{
		BGWaterVertexOutput vertexOutput = VertexCommon(vertexInput);
		return vertexOutput;
	}

	half4 FragForwardBase(BGWaterVertexOutput vertexOutput) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(vertexOutput);

		BGLightingData lightingData = PrepareLighting(vertexOutput);

		// reflection + fog
		half3 reflection = GetReflectionWithSSR(vertexOutput, lightingData);
		UNITY_APPLY_FOG(vertexOutput.fogCoord, reflection);

		// refraction
		half4 refraction = GetRefraction(vertexOutput, lightingData);

		// final
		half3 finalColor = lerp(refraction.rgb, reflection, refraction.a);

		return half4(finalColor, 1);
	}

#endif

#endif

