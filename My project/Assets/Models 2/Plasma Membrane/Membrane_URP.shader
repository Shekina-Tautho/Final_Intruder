// Faint membrane for Unity URP (Universal Render Pipeline).
// Recreates the Blender look: nearly clear face-on, stronger toward the edges (fresnel), warm tint.
Shader "Custom/Membrane_URP"
{
    Properties
    {
        _Tint ("Tint", Color) = (1, 0.82, 0.70, 1)
        _Opacity ("Face-on Opacity", Range(0, 1)) = 0.10
        _EdgeGlow ("Edge Glow", Range(0, 2)) = 0.6
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off          // thin double-sided skin: show both shells

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Tint;
                half _Opacity;
                half _EdgeGlow;
                half _FresnelPower;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings   { float4 positionCS : SV_POSITION; float3 normalWS : TEXCOORD0; float3 viewDirWS : TEXCOORD1; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs p = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = p.positionCS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(p.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                float3 n = normalize(IN.normalWS);
                n = isFrontFace ? n : -n;                       // flip for the inner shell
                float ndv = saturate(dot(n, normalize(IN.viewDirWS)));
                float fresnel = pow(1.0 - ndv, _FresnelPower);
                half alpha = saturate(_Opacity + fresnel * _EdgeGlow);
                return half4(_Tint.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
