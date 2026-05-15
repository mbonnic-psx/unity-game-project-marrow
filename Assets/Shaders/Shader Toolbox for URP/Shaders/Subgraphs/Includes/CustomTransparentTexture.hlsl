#ifndef TOOLBOX_TRANSPARENT_TEXTURE_INCLUDED
#define TOOLBOX_TRANSPARENT_TEXTURE_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

TEXTURE2D(_CameraTransparentTexture);

// Importing Core.hlsl causes problems, so I just duplicate the required bits here.
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
    #define TransformStereoUV(u) (u+unity_StereoEyeIndex)*0.5
#else
    #define TransformStereoUV(u) u
#endif

void SampleTransparentTexture_float(float2 UV, out float3 Color)
{
    UV.x = TransformStereoUV(UV.x);
    Color = SAMPLE_TEXTURE2D(_CameraTransparentTexture, sampler_LinearClamp, UV).rgb;
}

void SampleTransparentTexture_half(half2 UV, out half3 Color)
{
    UV.x = TransformStereoUV(UV.x);
    Color = SAMPLE_TEXTURE2D(_CameraTransparentTexture, sampler_LinearClamp, UV).rgb;
}

#endif // TOOLBOX_TRANSPARENT_TEXTURE_INCLUDED
