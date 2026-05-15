#ifndef TOOLBOX_LIGHTING_INCLUDED
#define TOOLBOX_LIGHTING_INCLUDED

// Required to avoid errors when using max 1 shadow cascade.
#ifndef SHADERGRAPH_PREVIEW
	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
	#if (SHADERPASS != SHADERPASS_FORWARD)
		#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
	#endif
#endif

void AmbientLight_float(float3 NormalWS, out float3 AmbientLight)
{
#ifdef SHADERGRAPH_PREVIEW
    AmbientLight = 0.1f;
#else
    AmbientLight = SampleSH(NormalWS);
#endif
}

void AmbientLight_half(half3 NormalWS, out half3 AmbientLight)
{
#ifdef SHADERGRAPH_PREVIEW
    AmbientLight = 0.1h;
#else
    AmbientLight = SampleSH(NormalWS);
#endif
}

// Get parameters from the main light - usually the scene's primary directional light.
void MainLight_float(float3 PositionWS, 
    out float3 Direction, out float3 Color, out float DistanceAttenuation, out float ShadowAttenuation)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = normalize(float3(1.0f, 1.0f, 0.0f));
        Color = 1.0f;
        DistanceAttenuation = 1.0f;
        ShadowAttenuation = 1.0f;
    #else
        Light mainLight = GetMainLight();
        Direction = normalize(mainLight.direction);
        Color = mainLight.color;
        DistanceAttenuation = mainLight.distanceAttenuation;

        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
		    float4 shadowCoord = ComputeScreenPos(TransformWorldToHClip(PositionWS));
		#else
		    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
		#endif
		ShadowAttenuation = MainLightShadow(shadowCoord, PositionWS, float4(1, 1, 1, 1), _MainLightOcclusionProbes);
    #endif
}

void MainLight_half(half3 PositionWS, 
    out half3 Direction, out half3 Color, out half DistanceAttenuation, out half ShadowAttenuation)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = normalize(half3(1.0f, 1.0f, 0.0f));
        Color = 1.0f;
        DistanceAttenuation = 1.0f;
        ShadowAttenuation = 1.0f;
    #else
        Light mainLight = GetMainLight();
        Direction = normalize(mainLight.direction);
        Color = mainLight.color;
        DistanceAttenuation = mainLight.distanceAttenuation;

        #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
		    half4 shadowCoord = ComputeScreenPos(TransformWorldToHClip(PositionWS));
		#else
		    half4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
		#endif
		ShadowAttenuation = MainLightShadow(shadowCoord, PositionWS, half4(1, 1, 1, 1), _MainLightOcclusionProbes);
    #endif
}

// Modify the world normals according to a normal map.
void ApplyNormalMap_float(float3 NormalSample, float3 WorldNormal, float4 WorldTangent, out float3 OutNormal)
{
    float3 binormal = cross(WorldNormal, WorldTangent.xyz);

    OutNormal = normalize(
        NormalSample.x * WorldTangent.xyz +
        NormalSample.y * binormal +
        NormalSample.z * WorldNormal
    );
}

void ApplyNormalMap_half(half3 NormalSample, half3 WorldNormal, half4 WorldTangent, out half3 OutNormal)
{
    half3 binormal = cross(WorldNormal, WorldTangent.xyz) * (WorldTangent.w * unity_WorldTransformParams.w);

    OutNormal = normalize(
        NormalSample.x * WorldTangent.xyz +
        NormalSample.y * binormal +
        NormalSample.z * WorldNormal
    );
}

void ApplyTriplanarTint_float(float4 InColor, float3 Normal, float DirBlend, float3 ColorX, float3 ColorY, float3 ColorZ, float Strength, out float4 OutColor)
{
    float3 blend = pow(abs(Normal), DirBlend);
    blend /= dot(blend, 1.0f);
    
    float3 triplanarColor = blend.x * ColorX + blend.y * ColorY + blend.z * ColorZ;
    
    OutColor.rgb = lerp(InColor.rgb, triplanarColor, Strength);
    OutColor.a = InColor.a;
}

void ApplyTriplanarTint_half(half4 InColor, half3 Normal, half DirBlend, half3 ColorX, half3 ColorY, half3 ColorZ, half Strength, out half4 OutColor)
{
    half3 blend = pow(abs(Normal), DirBlend);
    blend /= dot(blend, 1.0h);
    
    half3 triplanarColor = blend.x * ColorX + blend.y * ColorY + blend.z * ColorZ;
    
    OutColor.rgb = lerp(InColor.rgb, triplanarColor, Strength);
    OutColor.a = InColor.a;
}

#endif // TOOLBOX_LIGHTING_INCLUDED
