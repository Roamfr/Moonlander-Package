#define BLEND_NORMAL 0
#define BLEND_ADD 1
#define BLEND_ADDSUBTRACT 2
#define BLEND_SUBTRACT 3
#define BLEND_MULTIPLY 4
#define BLEND_DIVIDE 5
#define BLEND_SCREEN 6
#define BLEND_OVERLAY 7
#define BLEND_MIN 8
#define BLEND_MAX 9
#define BLEND_SOFTLIGHT 10
#define BLEND_HARDLIGHT 11

typedef int blend_mode;

float4 blend(float4 bg, float4 fg, blend_mode mode, float opacity);

float4 blend(float4 bg, float4 fg, blend_mode mode, float opacity)
{
    const float4 invertedFr = 1.0f - fg;
    const float4 invertedBc = 1.0f - bg;
    
    float4 blended;
    switch (mode)
    {
        case BLEND_NORMAL:
            blended = fg;
            break;
        case BLEND_ADD:
            blended = fg + bg;
            break;
        case BLEND_ADDSUBTRACT:
            blended = float4(clamp(fg.rgb + bg.rgb - 0.5, 0, 1), bg.a);
            break;
        case BLEND_SUBTRACT:
            blended = float4(saturate(bg.rgb - fg.rgb), bg.a); // Subtracting alpha would result in transparent if both are full alpha.
            break;
        case BLEND_MULTIPLY:
            blended = fg * bg;
            break;
        case BLEND_DIVIDE:
            blended = fg / bg;
            break;
        case BLEND_SCREEN:
            blended = 1.0f - invertedFr * invertedBc;
            break;
        case BLEND_OVERLAY:
            //blended = fg < 0.5 ? 2.0 * fg * bg : 1.0 - 2.0 * invertedFr * invertedBc;
            blended = (fg < 0.5) ? 2.0 * bg * fg : 1.0 - 2.0 * (1.0 - bg) * (1.0 - fg);
            break;
        case BLEND_MIN:
            blended = min(fg, bg);
            break;
        case BLEND_MAX:
            blended = max(fg, bg);
            break;
    case BLEND_SOFTLIGHT:
        blended = bg < 0.5 ?
            fg - (1.0 - 2.0 * bg) * fg * (1.0 - fg) :
                fg < 0.25 ?
                    fg + (2.0 * bg - 1.0) * fg * ((16.0 * fg - 12.0) * fg + 3.0) :
                        fg + (2.0 * bg - 1.0) * (sqrt(fg) - fg);
        
        break;
    case BLEND_HARDLIGHT:
        blended = bg < 0.5 ? 2.0 * fg * bg : 1.0 - 2.0 * (1 - fg) * (1 - bg);
        break;
        default:
            blended = fg;
            break;
    }

    return  saturate(lerp(bg, blended, opacity));
}