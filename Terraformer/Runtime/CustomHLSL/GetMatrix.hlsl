//UNITY_SHADER_NO_UPGRADE
#ifndef GET_MATRIX_INCLUDED
#define GET_MATRIX_INCLUDED

void GetMatrix_float(float coordinate, UnityTexture2D tex, out float4x4 Out)
{
    float4 row0 = tex2Dlod(tex, float4(coordinate, 0.0, 0.0, 0.0));
    float4 row1 = tex2Dlod(tex, float4(coordinate, 1.0 / 3.0, 0.0, 0.0));
    float4 row2 = tex2Dlod(tex, float4(coordinate, 2.0 / 3.0, 0.0, 0.0));
    float4 row3 = tex2Dlod(tex, float4(coordinate, 1.0, 0.0, 0.0));

    Out = float4x4(row0, row1, row2, row3);
}
#endif //HLSLINCLUDE_INCLUDED
