#ifndef TOOLBOX_COLORS_INCLUDED
#define TOOLBOX_COLORS_INCLUDED

// With thanks to https://chilliant.com/rgb2hsv.html
static const float HCLgamma = 3;
static const float HCLy0 = 100;
static const float HCLmaxL = 0.530454533953517;
static const float HCLPI = 3.1415926536;

void RGBtoHCL_float(float3 RGB, out float3 HCL)
{
    float h = 0;
    float u = min(RGB.r, min(RGB.g, RGB.b));
    float v = max(RGB.r, max(RGB.g, RGB.b));
    float q = HCLgamma / HCLy0;
    
    HCL.y = v - u;
    
    if (HCL.y != 0)
    {
        h = atan2(RGB.g - RGB.b, RGB.r - RGB.g) / HCLPI;
        q *= u / v;
    }

    q = exp(q);
    HCL.x = frac(h / 2 - min(frac(h), frac(-h)) / 6);
    HCL.y *= q;
    HCL.z = lerp(-u, v, q) / (HCLmaxL * 2);
}

void RGBtoHCL_half(half3 RGB, out half3 HCL)
{
    half h = 0;
    half u = min(RGB.r, min(RGB.g, RGB.b));
    half v = max(RGB.r, max(RGB.g, RGB.b));
    half q = HCLgamma / HCLy0;
    
    HCL.y = v - u;
    
    if (HCL.y != 0)
    {
        h = atan2(RGB.g - RGB.b, RGB.r - RGB.g) / HCLPI;
        q *= u / v;
    }

    q = exp(q);
    HCL.x = frac(h / 2 - min(frac(h), frac(-h)) / 6);
    HCL.y *= q;
    HCL.z = lerp(-u, v, q) / (HCLmaxL * 2);
}

void HCLtoRGB_float(float3 HCL, out float3 RGB)
{
    RGB = 0;
    
    if (HCL.z != 0)
    {
        float h = HCL.x;
        float c = HCL.y;
        float l = HCL.z * HCLmaxL;
        float q = exp((1 - c / (2 * l)) * (HCLgamma / HCLy0));
        float u = (2 * l - c) / (2 * q - 1);
        float v = c / q;
        float a = (h + min(frac(2 * h) / 4, frac(-2 * h) / 8)) * HCLPI * 2;
        float t;
        h *= 6;
        if(h <= 0.999)
        {
            t = tan(a);
            RGB.r = 1;
            RGB.g = t / (1 + t);
        }
        else if (h <= 1.001)
        {
            RGB.r = 1;
            RGB.g = 1;
        }
        else if (h <= 2)
        {
            t = tan(a);
            RGB.r = (1 + t) / t;
            RGB.g = 1;
        }
        else if (h <= 3)
        {
            t = tan(a);
            RGB.g = 1;
            RGB.b = 1 + t;
        }
        else if(h <= 3.999)
        {
            t = tan(a);
            RGB.g = 1 / (1 + t);
            RGB.b = 1;
        }
        else if(h <= 4.001)
        {
            RGB.g = 0;
            RGB.b = 1;
        }
        else if (h <= 5)
        {
            t = tan(a);
            RGB.r = -1 / t;
            RGB.b = 1;
        }
        else
        {
            t = tan(a);
            RGB.r = 1;
            RGB.b = -t;
        }
        
        RGB = RGB * v + u;
    }
}

void HCLtoRGB_half(half3 HCL, out half3 RGB)
{
    RGB = 0;
    
    if (HCL.z != 0)
    {
        half h = HCL.x;
        half c = HCL.y;
        half l = HCL.z * HCLmaxL;
        half q = exp((1 - c / (2 * l)) * (HCLgamma / HCLy0));
        half u = (2 * l - c) / (2 * q - 1);
        half v = c / q;
        half a = (h + min(frac(2 * h) / 4, frac(-2 * h) / 8)) * HCLPI * 2;
        half t;
        h *= 6;
        if (h <= 0.999)
        {
            t = tan(a);
            RGB.r = 1;
            RGB.g = t / (1 + t);
        }
        else if (h <= 1.001)
        {
            RGB.r = 1;
            RGB.g = 1;
        }
        else if (h <= 2)
        {
            t = tan(a);
            RGB.r = (1 + t) / t;
            RGB.g = 1;
        }
        else if (h <= 3)
        {
            t = tan(a);
            RGB.g = 1;
            RGB.b = 1 + t;
        }
        else if (h <= 3.999)
        {
            t = tan(a);
            RGB.g = 1 / (1 + t);
            RGB.b = 1;
        }
        else if (h <= 4.001)
        {
            RGB.g = 0;
            RGB.b = 1;
        }
        else if (h <= 5)
        {
            t = tan(a);
            RGB.r = -1 / t;
            RGB.b = 1;
        }
        else
        {
            t = tan(a);
            RGB.r = 1;
            RGB.b = -t;
        }
        
        RGB = RGB * v + u;
    }
}

#endif // TOOLBOX_COLORS_INCLUDED
