Shader "SoundReactor/Unlit/MatCap/Color"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection ("Reflection", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _MatCapDiff ("Diffuse", 2D) = "black" {}
        _MatCapRefl ("Reflection", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float4 _Color;
            float _Reflection;
            float _Metallic;
            float _EmissionStrength;

            sampler2D _MatCapDiff;
            sampler2D _MatCapRefl;

            struct appdata
            {
                float3 vertex   : POSITION;
                float3 normal   : NORMAL;
            };

            struct v2f
            {
                float4 vertex       : SV_POSITION;
                float3 normal       : TEXCOORD0;

                UNITY_FOG_COORDS(1)
            };

            v2f vert(appdata v)
            {
                v2f o;

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = COMPUTE_VIEW_NORMAL;

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                fixed4 diff = tex2D(_MatCapDiff, i.normal.xy  * 0.5 + 0.5) * _Color;
                fixed4 refl = tex2D(_MatCapRefl, i.normal.xy  * 0.5 + 0.5) * _Reflection;

                diff.rgb = diff.rgb + (diff.rgb * _EmissionStrength);

                fixed4 col = lerp(diff + refl, diff * refl, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);

                // draw view space normal
                return col;
            }
            ENDCG
        }
    }
}
