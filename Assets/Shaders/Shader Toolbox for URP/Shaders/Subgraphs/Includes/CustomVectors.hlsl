#ifndef TOOLBOX_VECTORS_INCLUDED
#define TOOLBOX_VECTORS_INCLUDED

void FindPerpendicularVectorsSafe_float(float3 InVector, out float3 OutVector1, out float3 OutVector2)
{
    if (length(InVector) < 1e-06)
    {
        InVector += float3(1.0f, 0.0f, 0.0f);
    }
    
    float x = abs(InVector.z) * sign(sign(InVector.x) + 0.5f);
    float y = abs(InVector.z) * sign(sign(InVector.y) + 0.5f);
    float z = -(abs(InVector.x) + abs(InVector.y) * sign(sign(InVector.z)) + 0.5f);

    OutVector1 = normalize(float3(x, y, z));
    OutVector2 = normalize(cross(InVector, OutVector1));
}

void FindPerpendicularVectorsSafe_half(half3 InVector, out half3 OutVector1, out half3 OutVector2)
{
    if (length(InVector) < 1e-06)
    {
        InVector += half3(1.0f, 0.0f, 0.0f);
    }
    
    half x = abs(InVector.z) * sign(sign(InVector.x) + 0.5f);
    half y = abs(InVector.z) * sign(sign(InVector.y) + 0.5f);
    half z = -(abs(InVector.x) + abs(InVector.y) * sign(sign(InVector.z) + 0.5f));

    OutVector1 = normalize(half3(x, y, z));
    OutVector2 = normalize(cross(InVector, OutVector1));
}

void FindPerpendicularVectorsUnsafe_float(float3 InVector, out float3 OutVector1, out float3 OutVector2)
{
    float x = abs(InVector.z) * sign(InVector.x);
    float y = abs(InVector.z) * sign(InVector.y);
    float z = -(abs(InVector.x) + abs(InVector.y) * sign(InVector.z));

    OutVector1 = normalize(float3(x, y, z));
    OutVector2 = normalize(cross(InVector, OutVector1));
}

void FindPerpendicularVectorsUnsafe_half(half3 InVector, out half3 OutVector1, out half3 OutVector2)
{
    half x = abs(InVector.z) * sign(InVector.x);
    half y = abs(InVector.z) * sign(InVector.y);
    half z = -(abs(InVector.x) + abs(InVector.y) * sign(InVector.z));

    OutVector1 = normalize(half3(x, y, z));
    OutVector2 = normalize(cross(InVector, OutVector1));
}

void FindPerpendicularVectors_float(float3 InVector, float Mode, out float3 OutVector1, out float3 OutVector2)
{
    if (Mode > 0.5f)
    {
        FindPerpendicularVectorsSafe_float(InVector, OutVector1, OutVector2);
    }
    else
    {
        FindPerpendicularVectorsUnsafe_float(InVector, OutVector1, OutVector2);
    }
}

void FindPerpendicularVectors_half(half3 InVector, half Mode, out half3 OutVector1, out half3 OutVector2)
{
    if (Mode > 0.5f)
    {
        FindPerpendicularVectorsSafe_half(InVector, OutVector1, OutVector2);
    }
    else
    {
        FindPerpendicularVectorsUnsafe_half(InVector, OutVector1, OutVector2);
    }
}

void Refract_float(float3 ViewWS, float3 NormalWS, float RefractiveIndex, out float3 RefractedVector)
{
    RefractedVector = refract(ViewWS, NormalWS, -1.0f / RefractiveIndex);
}

void Refract_half(half3 InVector, half3 NormalWS, half RefractiveIndex, out half3 RefractedVector)
{
    RefractedVector = refract(InVector, NormalWS, -1.0f / RefractiveIndex);
}

void Shear_float(float2 Pos, float ShearX, float ShearY, out float2 Out)
{
    float2x2 shearMat = float2x2(1.0f, ShearX, ShearY, 1.0f);

    Out = mul(shearMat, Pos);
}

void Shear_half(half2 Pos, half ShearX, half ShearY, out half2 Out)
{
    half2x2 shearMat = half2x2(1.0f, ShearX, ShearY, 1.0f);

    Out = mul(shearMat, Pos);
}

#endif // TOOLBOX_VECTORS_INCLUDED
