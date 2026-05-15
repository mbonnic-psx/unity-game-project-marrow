#ifndef TOOLBOX_FOG_INCLUDED
#define TOOLBOX_FOG_INCLUDED

void ApplyFog_float(float4 InColor, float3 PositionWS, out float4 FogColor)
{
    float4 color = unity_FogColor;
    
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
    float viewZ = -TransformWorldToView(PositionWS).z;
    float nearZ0ToFarZ = max(viewZ - _ProjectionParams.y, 0);
    // ComputeFogFactorZ0ToFar returns the fog "occlusion" (0 for full fog and 1 for no fog) so this has to be inverted for density.
    float density = 1.0f - ComputeFogIntensity(ComputeFogFactorZ0ToFar(nearZ0ToFarZ));
#else
    float density = 0.0f;
#endif
    
    FogColor = lerp(InColor, unity_FogColor, density);
}

void ApplyFog_half(half4 InColor, half3 PositionWS, out half4 FogColor)
{
    half4 color = unity_FogColor;
    
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
    half viewZ = -TransformWorldToView(PositionWS).z;
    half nearZ0ToFarZ = max(viewZ - _ProjectionParams.y, 0);
    // ComputeFogFactorZ0ToFar returns the fog "occlusion" (0 for full fog and 1 for no fog) so this has to be inverted for density.
    half density = 1.0f - ComputeFogIntensity(ComputeFogFactorZ0ToFar(nearZ0ToFarZ));
#else
    half density = 0.0f;
#endif
    
    FogColor = lerp(InColor, unity_FogColor, density);
}

// Based on the 'portal cards' from Abzu: https://www.youtube.com/watch?v=l9NX06mvp2E&t=1077s
void PortalCard_float(float2 UV, float IntersectionDistance, float3 PositionWS, float2 FadeThresholds, float2 DistanceThresholds, float2 EdgeFade, float4 FogStartColor, float4 FogEndColor, out float4 OutColor)
{
    float portalStrength = smoothstep(FadeThresholds.x, FadeThresholds.y, IntersectionDistance);
    OutColor.rgb = lerp(FogStartColor, FogEndColor, portalStrength);
    
    float distanceFromCard = distance(PositionWS, _WorldSpaceCameraPos);
    float2 edgeAlpha = smoothstep(0.0f, EdgeFade, 1.0f - abs(UV - 0.5f) * 2.0f);
    float cardAlpha = smoothstep(DistanceThresholds.x, DistanceThresholds.y, distanceFromCard);
    OutColor.a = edgeAlpha.x * edgeAlpha.y * cardAlpha;

}

void PortalCard_half(half2 UV, half IntersectionDistance, half3 PositionWS, half2 FadeThresholds, half2 DistanceThresholds, half2 EdgeFade, half4 FogStartColor, half4 FogEndColor, out half4 OutColor)
{
    half portalStrength = smoothstep(FadeThresholds.x, FadeThresholds.y, IntersectionDistance);
    OutColor.rgb = lerp(FogStartColor, FogEndColor, portalStrength);
    
    half distanceFromCard = distance(PositionWS, _WorldSpaceCameraPos);
    half2 edgeAlpha = smoothstep(0.0f, EdgeFade, 1.0f - abs(UV - 0.5f) * 2.0f);
    half cardAlpha = smoothstep(DistanceThresholds.x, DistanceThresholds.y, distanceFromCard);
    OutColor.a = edgeAlpha.x * edgeAlpha.y * cardAlpha;
}

#endif // TOOLBOX_FOG_INCLUDED
