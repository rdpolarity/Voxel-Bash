Shader "SoundReactor/Unlit/MatCap/Texture"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection ("Reflection", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _MainTex ("Base (RGB)", 2D) = "white" {}
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
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _MatCapDiff;
            sampler2D _MatCapRefl;

            struct appdata
            {
                float3 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
            };

            struct v2f
            {
                float4 vertex       : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normal       : TEXCOORD1;

                UNITY_FOG_COORDS(2)
            };

            v2f vert(appdata v)
            {
                v2f o;

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.normal = COMPUTE_VIEW_NORMAL;

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col;

                fixed4 diff = tex2D(_MatCapDiff, i.normal.xy  * 0.5 + 0.5) * _Color * tex;
                fixed4 refl = tex2D(_MatCapRefl, i.normal.xy  * 0.5 + 0.5) * _Reflection;

                diff.rgb = diff.rgb + (diff.rgb * _EmissionStrength);

                col = lerp(diff + refl, diff * refl, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);

                // draw view space normal
                return col;
            }
            ENDCG
        }
    }
}
