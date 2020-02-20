#ifndef BADDOG_WATER_LIGHTING
#define BADDOG_WATER_LIGHTING

	half3 WaveNormal(BGWaterVertexOutput vertexOutput)
	{
		half3 waterNormal1 = tex2D(_MainWave, vertexOutput.mainWaveUV.xy).xyz;
		half3 waterNormal2 = tex2D(_MainWave, vertexOutput.secondWaveUV.xy).xyz;
		half3 waterNormal3 = tex2D(_SecondWave, vertexOutput.secondWaveUV.zw).xyz;

		half3 waterNormal = ((waterNormal1 + waterNormal2) * 0.6667 - 0.6667) * half3(_SecondWaveBumpScale, _SecondWaveBumpScale, 1);

		waterNormal3 = waterNormal3 * 2 - 1;
		waterNormal += (waterNormal3 * half3(_MainWaveBumpScale, _MainWaveBumpScale, 1));

		return normalize(mul(waterNormal, float3x3(vertexOutput.worldTangentDir.xyz, vertexOutput.worldBitangentDir.xyz, vertexOutput.worldNormalDir.xyz)));
	}

	BGLightingData PrepareLighting(BGWaterVertexOutput vertexOutput)
	{
		BGLightingData lightingData;

		lightingData.worldPos = float3(vertexOutput.worldTangentDir.w, vertexOutput.worldBitangentDir.w, vertexOutput.worldNormalDir.w);
		lightingData.worldNormal = WaveNormal(vertexOutput);
		lightingData.worldLightDir = normalize(UnityWorldSpaceLightDir(lightingData.worldPos));
		lightingData.worldViewDir = normalize(_WorldSpaceCameraPos.xyz - lightingData.worldPos);

		half3 H = normalize(lightingData.worldLightDir + lightingData.worldViewDir);

		lightingData.NoL = saturate(dot(lightingData.worldLightDir, lightingData.worldNormal));
		lightingData.NoV = saturate(dot(lightingData.worldNormal, lightingData.worldViewDir));
		lightingData.NoH = saturate(dot(lightingData.worldNormal, H));
		lightingData.LoH = saturate(dot(lightingData.worldLightDir, H));
		lightingData.R = normalize(reflect(-lightingData.worldViewDir, lightingData.worldNormal));

		lightingData.diffuseColor = _WaterBaseColor;
		lightingData.specularColor = half3(0.04, 0.04, 0.04);
		lightingData.lightColor = _LightColor0.rgb;

		lightingData.screenUV = vertexOutput.screenPos.xy / vertexOutput.screenPos.w;
		lightingData.grabUV = vertexOutput.grabPos.xy / vertexOutput.grabPos.w;

		return lightingData;
	}

	half3 IndirectDiffuse(BGLightingData lightingData)
	{
		return ShadeSH9(half4(lightingData.worldNormal, 1));
	}

	half3 Diffuse(BGLightingData lightingData)
	{
		return lightingData.lightColor * lightingData.NoL;
	}

	half3 Specular(BGLightingData lightingData)
	{
		float D = (-0.004) / (lightingData.NoH * lightingData.NoH - 1.005);
		D *= D;

		float F = FresnelTerm(lightingData.specularColor, lightingData.LoH);

		return lightingData.lightColor * D * F * UNITY_PI * _SpecularIntensity;
	}

	half3 IndirectSpecular(BGLightingData lightingData)
	{
#ifdef SHADER_API_MOBILE
		half4 probe = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, lightingData.R, 0);
#else
		half4 probe = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, lightingData.R, 0);
		probe.rgb = DecodeHDR(probe, unity_SpecCube0_HDR);
#endif

		return probe.rgb * FresnelLerpFast(lightingData.specularColor, 1, lightingData.LoH);
	}

	half4 GetSSRLighting(BGWaterVertexOutput vertexOutput, BGLightingData lightingData)
	{
		float3 uvz = GetSSRUVZ(vertexOutput, lightingData);

		half3 ssrColor = lerp(half3(0, 0, 0), tex2D(_BGWater_GrabTexture, uvz.xy) * _SSRIntensity, uvz.z > 0);

		return half4(ssrColor, uvz.z);
	}

	half3 GetReflectionWithSSR(BGWaterVertexOutput vertexOutput, BGLightingData lightingData)
	{
		half3 indirectDiffuse = IndirectDiffuse(lightingData);
		half3 diffuse = Diffuse(lightingData);
		half3 specular = Specular(lightingData);
		half3 indirectSpecular = IndirectSpecular(lightingData);

#if defined(_BGWATER_SSR_ON)
		half4 ssrLighting = GetSSRLighting(vertexOutput, lightingData);
		indirectSpecular = lerp(lerp(indirectSpecular, ssrLighting.rgb, ssrLighting.a), ssrLighting, ssrLighting.a > 0.99);
#endif

		indirectSpecular *= _EnviromentIntensity;

		return (indirectDiffuse + diffuse) * lightingData.diffuseColor + specular + indirectSpecular;
	}

	half4 GetRefraction(BGWaterVertexOutput vertexOutput, BGLightingData lightingData)
	{
		float2 screenUV = lightingData.screenUV;
		float2 grabUV = lightingData.grabUV;

		half3 worldViewDir = normalize(lightingData.worldViewDir);
		half worldViewDirY = abs(worldViewDir.y);

		float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV));
		depth = depth - vertexOutput.screenPos.z;

		half2 deltaUV = lightingData.worldNormal.xz * _WaterDistortScale * saturate(depth) * worldViewDirY / vertexOutput.screenPos.z;

		float newDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV + deltaUV));
		newDepth = newDepth - vertexOutput.screenPos.z;

		half signDepth = saturate(newDepth * 10);
		grabUV = grabUV + deltaUV * signDepth;

		depth = lerp(depth, newDepth, signDepth);

		half viewMultiplier = (worldViewDirY + _WaterMuddyScale) * _WaterDepthOffset * _WaterDepthOffset;
		depth *= viewMultiplier;

		half alpha = saturate(1 - depth);
		alpha = saturate(1.02 - pow(alpha, (dot(lightingData.worldNormal.xyz, worldViewDir) * 5 + 6)));

		half4 refraction = tex2D(_BGWater_GrabTexture, grabUV);
		refraction.rgb = lerp(refraction.rgb, refraction.rgb * _WaterMuddyColor * _WaterMuddyScale, alpha);
		refraction.a = alpha;

		return refraction;
	}

#endif

