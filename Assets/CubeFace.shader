Shader "Custom/DoubleSidedShader"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1) // Color for front face
        _EmissionColor ("Emission Color", Color) = (0,0,0,1) // Emission color
        _BackColor ("Back Face Color", Color) = (0,0,1,1) // Color for back face
        _BorderThickness ("Border Thickness", Range(0.0, 0.5)) = 0.05 // Thickness of the border
        _BorderColor ("Border Color", Color) = (0,0,0,1) // Color of the border
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off // Disable culling to render both sides of the faces

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _EmissionColor;
            fixed4 _BackColor;
            fixed4 _BorderColor;
            float _BorderThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize the normal vector
                float3 worldNormal = normalize(i.normal);

                // Check if the face is front or back
                float frontFace = dot(worldNormal, float3(0, 0, 1)) > 0 ? 1 : 0;

                // Calculate UV distance from the edges
                float2 uv = i.uv;
                float distToEdge = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));

                // Smooth border transition
                float borderBlend = smoothstep(_BorderThickness - 0.01, _BorderThickness, distToEdge);

                // Determine the base color (front or back)
                fixed4 faceColor = frontFace > 0 ? _Color : _BackColor;

                // Add emission to the face color
                faceColor.rgb += _EmissionColor.rgb;

                // Blend between the border and the face color
                return lerp(_BorderColor, faceColor, borderBlend);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
