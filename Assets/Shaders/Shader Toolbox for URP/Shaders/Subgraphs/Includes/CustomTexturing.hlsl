#ifndef TOOLBOX_TEXTURING_INCLUDED
#define TOOLBOX_TEXTURING_INCLUDED

// Take a 2D seed value and output a random 2D vector.
inline float2 hash2D2D_float(float2 s)
{
    return frac(sin(fmod(float2(dot(s, float2(127.1f, 311.7f)), dot(s, float2(269.5f, 183.3f))), 3.14159f)) * 43758.5453f);
}

inline half2 hash2D2D_half(half2 s)
{
    return frac(sin(fmod(half2(dot(s, half2(127.1h, 311.7h)), dot(s, half2(269.5h, 183.3h))), 3.14159h)) * 43758.5453h);
}

// Take a 2D seed value and output a random 2D vector - this version works with custom function node.
void randomValue2D_float(float2 Seed, out float2 Rand)
{
    Rand = hash2D2D_float(Seed);
}

void randomValue2D_half(half2 Seed, out half2 Rand)
{
    Rand = hash2D2D_half(Seed);
}

/*
void SeparateTextureSampler_half(UnityTexture2D TextureIn, out Texture2D TextureOut, SamplerState SamplerOut)
{
    TextureOut = TextureIn.tex;
    SamplerOut = TextureIn.samplerstate;
}

void SeparateTextureSampler_float(UnityTexture2D TextureIn, out Texture2D TextureOut, SamplerState SamplerOut)
{
    SeparateTextureSampler_half(TextureIn, TextureOut, SamplerOut);
}
*/

// Sample a Texture2D and supply DX, DY gradients for choosing mipmap levels.
void SampleTexture2DGrad_float(UnityTexture2D Texture, UnitySamplerState Sampler, float2 UV, float DX, float DY, out float4 Color)
{
    Color = SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV, DX, DY);
}

void SampleTexture2DGrad_half(UnityTexture2D Texture, UnitySamplerState Sampler, half2 UV, half DX, half DY, out half4 Color)
{
    Color = SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV, DX, DY);
}

// Based on https://www.reddit.com/r/Unity3D/comments/dhr5g2/i_made_a_stochastic_texture_sampling_shader/
void InternalStochasticTexturing_float(UnityTexture2D Texture, SamplerState Sampler, float2 UV, out float4 Color)
{
    float4x3 BW_vx;

    float2 skewUV = mul(float2x2(1.0f, 0.0f, -0.57735027f, 1.15470054f), UV * 3.464f);

    float2 vxID = float2(floor(skewUV));
    float3 barycentric = float3(frac(skewUV), 0);
    barycentric.z = 1.0 - barycentric.x - barycentric.y;

    BW_vx = ((barycentric.z > 0) ?
		float4x3(float3(vxID, 0), float3(vxID + float2(0, 1), 0), float3(vxID + float2(1, 0), 0), barycentric.zyx) :
		float4x3(float3(vxID + float2(1, 1), 0), float3(vxID + float2(1, 0), 0), float3(vxID + float2(0, 1), 0), float3(-barycentric.z, 1.0 - barycentric.y, 1.0 - barycentric.x)));
 
	// Calculate derivatives to avoid triangular grid artifacts.
    float2 dx = ddx(UV);
    float2 dy = ddy(UV);
 
	// Blend samples with calculated weights.
    Color = mul(SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV + hash2D2D_float(BW_vx[0].xy), dx, dy), BW_vx[3].x) +
			mul(SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV + hash2D2D_float(BW_vx[1].xy), dx, dy), BW_vx[3].y) +
			mul(SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV + hash2D2D_float(BW_vx[2].xy), dx, dy), BW_vx[3].z);
}

void StochasticTexturing_float(UnityTexture2D Texture, UnitySamplerState Sampler, float2 UV, out float4 Color)
{
    InternalStochasticTexturing_float(Texture, Sampler.samplerstate, UV, Color);
}

void StochasticTexturing_float(UnityTexture2D Texture, float2 UV, out float4 Color)
{
    InternalStochasticTexturing_float(Texture, Texture.samplerstate, UV, Color);
}

void InternalStochasticTexturing_half(UnityTexture2D Texture, SamplerState Sampler, half2 UV, out half4 Color)
{
    half4x3 BW_vx;

    half2 skewUV = mul(half2x2(1.0h, 0.0h, -0.57735027h, 1.15470054h), UV * 3.464h);

    half2 vxID = half2(floor(skewUV));
    half3 barycentric = half3(frac(skewUV), 0);
    barycentric.z = 1.0 - barycentric.x - barycentric.y;

    BW_vx = ((barycentric.z > 0) ?
		half4x3(half3(vxID, 0), half3(vxID + half2(0, 1), 0), half3(vxID + half2(1, 0), 0), barycentric.zyx) :
		half4x3(half3(vxID + half2(1, 1), 0), half3(vxID + half2(1, 0), 0), half3(vxID + half2(0, 1), 0), half3(-barycentric.z, 1.0 - barycentric.y, 1.0 - barycentric.x)));
 
	// Calculate derivatives to avoid triangular grid artifacts.
    half2 dx = ddx(UV);
    half2 dy = ddy(UV);
 
	// Blend samples with calculated weights.
    Color = mul(SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV + hash2D2D_half(BW_vx[0].xy), dx, dy), BW_vx[3].x) +
			mul(SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV + hash2D2D_half(BW_vx[1].xy), dx, dy), BW_vx[3].y) +
			mul(SAMPLE_TEXTURE2D_GRAD(Texture, Sampler, UV + hash2D2D_half(BW_vx[2].xy), dx, dy), BW_vx[3].z);
}

void StochasticTexturing_half(UnityTexture2D Texture, UnitySamplerState Sampler, half2 UV, out half4 Color)
{
    InternalStochasticTexturing_half(Texture, Sampler.samplerstate, UV, Color);
}

void StochasticTexturing_half(UnityTexture2D Texture, half2 UV, out half4 Color)
{
    InternalStochasticTexturing_half(Texture, Texture.samplerstate, UV, Color);
}

// Based on code by Tyler Glaiel here: https://www.shadertoy.com/view/csX3RH
void InternalSampleTexture2DAA_float(UnityTexture2D Texture, SamplerState Sampler, float2 UV, out float4 Color)
{
    float2 textureSize = Texture.texelSize.zw;
    float2 textureUVs = UV * textureSize;
    float2 seam = floor(textureUVs + 0.5f);
    textureUVs = (textureUVs - seam) / fwidth(textureUVs) + seam;
    textureUVs = clamp(textureUVs, seam - 0.5f, seam + 0.5f);
    
    Color = SAMPLE_TEXTURE2D(Texture, Sampler, textureUVs / textureSize);
}

void InternalSampleTexture2DAA_half(UnityTexture2D Texture, SamplerState Sampler, half2 UV, out half4 Color)
{
    half2 textureSize = Texture.texelSize.zw;
    half2 textureUVs = UV * textureSize;
    half2 seam = floor(textureUVs + 0.5f);
    textureUVs = (textureUVs - seam) / fwidth(textureUVs) + seam;
    textureUVs = clamp(textureUVs, seam - 0.5f, seam + 0.5f);
    
    Color = SAMPLE_TEXTURE2D(Texture, Sampler, textureUVs / textureSize);
}

void SampleTexture2DAA_float(UnityTexture2D Texture, UnitySamplerState Sampler, float2 UV, out float4 Color)
{
    InternalSampleTexture2DAA_float(Texture, Sampler.samplerstate, UV, Color);
}

void SampleTexture2DAA_half(UnityTexture2D Texture, UnitySamplerState Sampler, half2 UV, out half4 Color)
{
    InternalSampleTexture2DAA_half(Texture, Sampler.samplerstate, UV, Color);
}

void SampleTexture2DAA_float(UnityTexture2D Texture, float2 UV, out float4 Color)
{
    InternalSampleTexture2DAA_float(Texture, Texture.samplerstate, UV, Color);
}

void SampleTexture2DAA_half(UnityTexture2D Texture, half2 UV, out half4 Color)
{
    InternalSampleTexture2DAA_half(Texture, Texture.samplerstate, UV, Color);
}

#endif // TOOLBOX_TEXTURING_INCLUDED
