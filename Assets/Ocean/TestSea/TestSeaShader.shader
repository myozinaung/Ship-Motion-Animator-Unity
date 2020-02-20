Shader "Custom/TestSeaShader"
{
    Properties
    {
        _Color("Colour", Color) = (0, 0, 0, 1)
        _Strength("Strength", Range(0,10)) = 1.0
        _Speed("Speed", Range(-200,200)) = 100
    }

    SubShader
    {
        Tags { "RenderType" = "transparent" }

        Pass
        {
            Cull Off

            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc

            float4 _Color;
            float _Strength;
            float _Speed;

            struct vertexInput
            {
                float4 vertex : POSITION;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
            };

            vertexOutput vertexFunc(vertexInput IN)
            {
                vertexOutput o;

                float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);
                // y = A*sin(wt + xpos_phase)
                float elevation = _Strength * sin((_Speed * _Time) + worldPos.x);

                worldPos.y = worldPos.y + elevation;

                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            float4 fragmentFunc(vertexOutput IN) : COLOR 
            {
                return _Color;
            }
            ENDCG
        }
    }
}
