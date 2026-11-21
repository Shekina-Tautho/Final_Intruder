// Faint membrane for Unity's Built-in Render Pipeline.
// Same look as Membrane_URP.shader. Use this one if your project does NOT use URP/HDRP.
Shader "Custom/Membrane_BuiltIn"
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
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Tint;
            half _Opacity;
            half _EdgeGlow;
            half _FresnelPower;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; float3 normalWS : TEXCOORD0; float3 viewDirWS : TEXCOORD1; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.viewDirWS = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
                float3 n = normalize(i.normalWS) * (facing > 0 ? 1 : -1);   // flip for the inner shell
                float ndv = saturate(dot(n, normalize(i.viewDirWS)));
                float fresnel = pow(1.0 - ndv, _FresnelPower);
                half alpha = saturate(_Opacity + fresnel * _EdgeGlow);
                return fixed4(_Tint.rgb, alpha);
            }
            ENDCG
        }
    }
}
