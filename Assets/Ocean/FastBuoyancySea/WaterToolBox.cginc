#ifndef WATER_TOOLBOX
#define WATER_TOOLBOX

#include "UnityCG.cginc"

inline half3 PerPixelNormal(sampler2D bumpMap, half4 coords, half3 vertexNormal, half bumpStrength) 
{
	half3 bump = (UnpackNormal(tex2D(bumpMap, coords.xy)) + UnpackNormal(tex2D(bumpMap, coords.zw))) * 0.5;
	half3 worldNormal = vertexNormal + bump.xxy * bumpStrength * half3(1,0,1);
	return normalize(worldNormal);
} 

inline half Fresnel(half3 viewVector, half3 worldNormal, half bias, half power)
{
	half facing =  clamp(1.0-max(dot(-viewVector, worldNormal), 0.0), 0.0,1.0);	
	half refl2Refr = saturate(bias+(1.0-bias) * pow(facing,power));	
	return refl2Refr;	
}

inline void ComputeScreenAndGrabPassPos (float4 pos, out float4 screenPos, out float4 grabPassPos) 
{
	#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
	#else
		float scale = 1.0f;
	#endif
	
	screenPos = ComputeScreenPos(pos); 
	grabPassPos.xy = ( float2( pos.x, pos.y*scale ) + pos.w ) * 0.5;
	grabPassPos.zw = pos.zw;
}

#define NB_WAVE 200 // number of wave components
float4 waves_p[NB_WAVE];
float4 waves_d[NB_WAVE];

// Main equation of the regular sine wave component
inline float evaluateWave(float4 wave_param, float4 wave_dir, float2 pos, float t)
{
	// wave_param.x = frequency (Omega)
	// wave_param.y = amplitude (Zeta_a)
	// wave_param.z = wave number (K)
	// wave_param.w = phase
	return wave_param.y * sin(wave_param.x*t + wave_param.w - wave_param.z*dot(wave_dir.xy, pos));
	// Zeta = Zeta_a*sin(Omega*t + Phase - K(XY_dir*XY_pos)) // XY_pos >> pos.xz
}

inline float waterHeight(half2 p)
{
	// p *= _WaveLengthInverse;
	// float v = (cos(_Time.y * _Periode + p.x) + sin(_Time.y * _Periode + p.y)) * _Intensity * 0.25;
	// float v = 5*sin(1.0*_Time.y + 0 - 0.05*dot((1,1),p.xy));
	// return v;

	float height = 0.0;
	for(int i = 0; i < NB_WAVE; i++)
		height += evaluateWave(waves_p[i], waves_d[i], p.xy, _Time.y);
	return height;
}

inline float4 WaterHeightAt(float4 pos)
{
	pos.y = 5;
	return pos;
}

#endif