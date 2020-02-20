#ifndef BADDOG_WATER_SSR
#define BADDOG_WATER_SSR

	float UVJitter(in float2 uv)
	{
		return frac((52.9829189 * frac(dot(uv, float2(0.06711056, 0.00583715)))));
	}

	void SSRRayConvert(float3 worldPos, out float4 clipPos, out float3 screenPos, out float2 grabPos)
	{
		clipPos = UnityWorldToClipPos(worldPos);
		float k = ((1.0) / (clipPos.w));

		screenPos.xy = ComputeScreenPos(clipPos).xy * k;
		screenPos.z = k;

		grabPos = ComputeGrabScreenPos(clipPos).xy * k;
	}

	float3 SSRRayMarch(BGWaterVertexOutput vertexOutput, BGLightingData lightingData)
	{
		float4 startClipPos;
		float3 startScreenPos;
		float2 startGrabPos;

		SSRRayConvert(lightingData.worldPos, startClipPos, startScreenPos, startGrabPos);

		float4 endClipPos;
		float3 endScreenPos;
		float2 endGrabPos;

		SSRRayConvert(lightingData.worldPos + lightingData.R, endClipPos, endScreenPos, endGrabPos);

		if (((endClipPos.w) < (startClipPos.w)))
		{
			return float3(0, 0, 0);
		}

		float3 screenDir = endScreenPos - startScreenPos;
		float2 grabDir = endGrabPos - startGrabPos;

		float screenDirX = abs(screenDir.x);
		float screenDirY = abs(screenDir.y);

		float dirMultiplier = lerp( 1 / (_ScreenParams.y * screenDirY), 1 / (_ScreenParams.x * screenDirX), screenDirX > screenDirY ) * _SSRSampleStep;

		screenDir *= dirMultiplier;
		grabDir *= dirMultiplier;

		half lastRayDepth = startClipPos.w;

		half sampleCount = 1 + UVJitter(vertexOutput.pos) * 0.1;

#if defined (SHADER_API_OPENGL) || defined (SHADER_API_D3D11) || defined (SHADER_API_D3D12)
		[unroll(64)]
#else
		UNITY_LOOP
#endif
		for(int i = 0; i < _SSRMaxSampleCount; i++)
		{
			float3 screenMatchUVZ = startScreenPos + screenDir * sampleCount;

			if((screenMatchUVZ.x <= 0) || (screenMatchUVZ.x >= 1) || (screenMatchUVZ.y <= 0) || (screenMatchUVZ.y >= 1))
			{
				break;
			}

			float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenMatchUVZ.xy));
			half rayDepth = 1.0 / screenMatchUVZ.z;
			half deltaDepth = rayDepth - sceneDepth;

			if((deltaDepth > 0) && (sceneDepth > startClipPos.w) && (deltaDepth < (abs(rayDepth - lastRayDepth) * 2)))
			{
				return float3(startGrabPos + grabDir * sampleCount, 1);
			}

			lastRayDepth = rayDepth;
			sampleCount += 1;
		}

		float4 farClipPos;
		float3 farScreenPos;
		float2 farGrabPos;

		SSRRayConvert(lightingData.worldPos + lightingData.R * 100000, farClipPos, farScreenPos, farGrabPos);

		if((farScreenPos.x > 0) && (farScreenPos.x < 1) && (farScreenPos.y > 0) && (farScreenPos.y < 1))
		{
			float farDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, farScreenPos.xy));

			if(farDepth > startClipPos.w)
			{
				return float3(farGrabPos, 1);
			}
		}

		return float3(0, 0, 0);
	}

	float3 GetSSRUVZ(BGWaterVertexOutput vertexOutput, BGLightingData lightingData)
	{
		#if defined(UNITY_SINGLE_PASS_STEREO)
			half ssrWeight = 1;

			half NoV = lightingData.NoV * 2;
			ssrWeight *= (1 - NoV * NoV);
		#else
			float screenUV = lightingData.screenUV * 2 - 1;
			screenUV *= screenUV;

			half ssrWeight = saturate(1 - dot(screenUV, screenUV));

			half NoV = lightingData.NoV * 2.5;
			ssrWeight *= (1 - NoV * NoV);
		#endif

		if (ssrWeight > 0.005)
		{
			float3 uvz = SSRRayMarch(vertexOutput, lightingData);
			uvz.z *= ssrWeight;
			return uvz;
		}

		return float3(0, 0, 0);
	}

#endif

