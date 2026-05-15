#ifndef TOOLBOX_RANDOM_INCLUDED
#define TOOLBOX_RANDOM_INCLUDED

// Based on work from: https://www.shadertoy.com/view/4djSRW
// Some values have been tweaked to suit different use cases.
// Also added 4D -> 2D and 4D -> 3D functions.

// Hash without Sine
// MIT License...
/* Copyright (c)2014 David Hoskins.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

void Hash1Dto1D_float(float In, float Min, float Max, out float Out)
{
    float p = frac(In * 49.458);
    p *= p + 33.33;
    p *= p + p;
    
    Out = lerp(Min, Max, frac(p));
}

void Hash1Dto2D_float(float In, float2 Min, float2 Max, out float2 Out)
{
    float3 p3 = frac(In * float3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xx + p3.yz) * p3.zy));
}

void Hash1Dto3D_float(float In, float3 Min, float3 Max, out float3 Out)
{
    float3 p3 = frac(In * float3(49.458, 21.7678, 1.26385));
    p3 += dot(In, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xxy + p3.yzz) * p3.zyx));
}

void Hash1Dto4D_float(float In, float4 Min, float4 Max, out float4 Out)
{
    float4 p4 = frac(In * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash2Dto1D_float(float2 In, float Min, float Max, out float Out)
{
    float3 p3 = frac(float3(In.xyx) * 49.458);
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.x + p3.y) * p3.z));
}

void Hash2Dto2D_float(float2 In, float2 Min, float2 Max, out float2 Out)
{
    float3 p3 = frac(float3(In.xyx) * float3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xx + p3.yz) * p3.zy));
}

void Hash2Dto3D_float(float2 In, float3 Min, float3 Max, out float3 Out)
{
    float3 p3 = frac(float3(In.xyx) * float3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yxz + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xxy + p3.yzz) * p3.zyx));
}

void Hash2Dto4D_float(float2 In, float4 Min, float4 Max, out float4 Out)
{
    float4 p4 = frac(float4(In.xyxy) * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash3Dto1D_float(float3 In, float Min, float Max, out float Out)
{
    float3 p3 = frac(In * 49.458);
    p3 += dot(p3, p3.zyx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.x + p3.y) * p3.z));
}

void Hash3Dto2D_float(float3 In, float2 Min, float2 Max, out float2 Out)
{
    float3 p3 = frac(In * float3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xx + p3.yz) * p3.zy));
}

void Hash3Dto3D_float(float3 In, float3 Min, float3 Max, out float3 Out)
{
    float3 p3 = frac(In * float3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yxz + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xxy + p3.yxx) * p3.zyx));
}

void Hash3Dto4D_float(float3 In, float4 Min, float4 Max, out float4 Out)
{
    float4 p4 = frac(float4(In.xyzx) * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash4Dto1D_float(float4 In, float Min, float Max, out float Out)
{
    float4 p4 = frac(In * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.x + p4.y) * (p4.z + p4.w)));
}

void Hash4Dto2D_float(float4 In, float2 Min, float2 Max, out float2 Out)
{
    float4 p4 = frac(In * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.yxzw + 33.33);
    
    Out = lerp(Min, Max, frac(p4.yz * p4.wx));
}

void Hash4Dto3D_float(float4 In, float3 Min, float3 Max, out float3 Out)
{
    float4 p4 = frac(In * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.xywz + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xyw + p4.zwx) * p4.yxz));
}

void Hash4Dto4D_float(float4 In, float4 Min, float4 Max, out float4 Out)
{
    float4 p4 = frac(In * float4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash1Dto1D_half(half In, half Min, half Max, out half Out)
{
    half p = frac(In * .1031);
    p *= p + 33.33;
    p *= p + p;
    
    Out = lerp(Min, Max, frac(p));
}

void Hash1Dto2D_half(half In, half2 Min, half2 Max, out half2 Out)
{
    half3 p3 = frac(In * half3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xx + p3.yz) * p3.zy));
}

void Hash1Dto3D_half(half In, half3 Min, half3 Max, out half3 Out)
{
    half3 p3 = frac(In * half3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xxy + p3.yzz) * p3.zyx));
}

void Hash1Dto4D_half(half In, half4 Min, half4 Max, out half4 Out)
{
    half4 p4 = frac(In * half4(.1031, .1030, .0973, .1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash2Dto1D_half(half2 In, half Min, half Max, out half Out)
{
    half3 p3 = frac(half3(In.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.x + p3.y) * p3.z));
}

void Hash2Dto2D_half(half2 In, half2 Min, half2 Max, out half2 Out)
{
    half3 p3 = frac(half3(In.xyx) * half3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xx + p3.yz) * p3.zy));
}

void Hash2Dto3D_half(half2 In, half3 Min, half3 Max, out half3 Out)
{
    half3 p3 = frac(half3(In.xyx) * half3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yxz + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xxy + p3.yzz) * p3.zyx));
}

void Hash2Dto4D_half(half2 In, half4 Min, half4 Max, out half4 Out)
{
    half4 p4 = frac(half4(In.xyxy) * half4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash3Dto1D_half(half3 In, half Min, half Max, out half Out)
{
    half3 p3 = frac(In * 49.458);
    p3 += dot(p3, p3.zyx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.x + p3.y) * p3.z));
}

void Hash3Dto2D_half(half3 In, half2 Min, half2 Max, out half2 Out)
{
    half3 p3 = frac(In * half3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yzx + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xx + p3.yz) * p3.zy));
}

void Hash3Dto3D_half(half3 In, half3 Min, half3 Max, out half3 Out)
{
    half3 p3 = frac(In * half3(49.458, 21.7678, 1.26385));
    p3 += dot(p3, p3.yxz + 33.33);
    
    Out = lerp(Min, Max, frac((p3.xxy + p3.yxx) * p3.zyx));
}

void Hash3Dto4D_half(half3 In, half4 Min, half4 Max, out half4 Out)
{
    half4 p4 = frac(half4(In.xyzx) * half4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

void Hash4Dto1D_half(half4 In, half Min, half Max, out half Out)
{
    half4 p4 = frac(In * half4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    
    Out = lerp(Min, Max, frac((p4.x + p4.y) * (p4.z + p4.w)));
}

void Hash4Dto2D_half(half4 In, half2 Min, half2 Max, out half2 Out)
{
    half4 p4 = frac(In * half4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.yxzw + 33.33);
    
    Out = lerp(Min, Max, frac(p4.yz * p4.wx));
}

void Hash4Dto3D_half(half4 In, half3 Min, half3 Max, out half3 Out)
{
    half4 p4 = frac(In * half4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.xywz + 33.33);
    
    Out = lerp(Min, Max, frac((p4.xyw + p4.zwx) * p4.yxz));
}

void Hash4Dto4D_half(half4 In, half4 Min, half4 Max, out half4 Out)
{
    half4 p4 = frac(In * half4(49.458, 21.7678, 1.26385, 37.1099));
    p4 += dot(p4, p4.wzxy + 33.33);
    Out = lerp(Min, Max, frac((p4.xxyz + p4.yzzw) * p4.zywx));
}

#endif // TOOLBOX_RANDOM_INCLUDED
