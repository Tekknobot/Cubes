Shader "Custom/GlassShader"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1, 1, 1, 0.5)
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.9
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _FresnelPower ("Fresnel Power", Range(1.0, 5.0)) = 2.0
        _Refraction ("Refraction Index", Range(0.0, 1.0)) = 0.02
        _EnvironmentMap ("Environment Map", Cube) = "" {}
        _MainTex ("Base Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable proper alpha blending
        ZWrite Off // Disable writing to the depth buffer for correct transparency
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            sampler2D _MainTex;
            samplerCUBE _EnvironmentMap;
            float4 _Color;
            float _Smoothness;
            float _Metallic;
            float _FresnelPower;
            float _Refraction;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = normalize(mul((float3x3)unity_WorldToObject, v.normal));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample base texture color
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                // Fresnel effect
                float fresnel = pow(1.0 - saturate(dot(i.viewDir, i.worldNormal)), _FresnelPower);

                // Refraction and reflection
                float3 refractionDir = refract(-i.viewDir, i.worldNormal, _Refraction);
                float3 reflectionDir = reflect(-i.viewDir, i.worldNormal);

                // Environment map sampling
                float4 refractionColor = texCUBE(_EnvironmentMap, refractionDir);
                float4 reflectionColor = texCUBE(_EnvironmentMap, reflectionDir);

                // Combine refraction, reflection, and base texture
                float4 envColor = lerp(refractionColor, reflectionColor, fresnel);
                float4 finalColor = lerp(envColor, baseColor, _Color.a);

                // Apply texture transparency (alpha channel from _MainTex)
                finalColor.rgb *= _Color.rgb; // Apply tint
                finalColor.a *= baseColor.a; // Combine texture alpha with base alpha

                return finalColor;
            }
            ENDCG
        }
    }

    FallBack "Transparent/Diffuse"
}
