Shader "Custom/DarkShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _HighlightColor("Highlight Color", Color) = (1, 1, 0, 1)
        _HighlightStrength("Highlight Strength", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                half4 _HighlightColor;
                half _HighlightStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 baseColor = tex * _BaseColor;

                // If fully transparent, skip to avoid halos
                if (baseColor.a < 0.01)
                    discard;

                // Compute strength scaled by pixel alpha so transparent pixels are unaffected
                half s = saturate(_HighlightStrength) * baseColor.a;

                // Simple linear blend between base and highlight color:
                // final = base * (1 - s) + highlight * s
                half3 finalRgb = baseColor.rgb * (1.0 - s) + _HighlightColor.rgb * s;

                return half4(finalRgb, baseColor.a);
            }
            ENDHLSL
        }
    }
}
