Shader "SoundReactor/Unlit/MatCap/Bumped"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection("Reflection", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Bump", 2D) = "normal" {}
        _MatCapDiff("Diffuse", 2D) = "black" {}
        _MatCapRefl("Reflection", 2D) = "black" {}
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
            sampler2D _BumpMap;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _MatCapDiff;
            sampler2D _MatCapRefl;

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;

                float3 tangent  : TEXCOORD1;
                float3 binormal : TEXCOORD2;
                float3 normal   : TEXCOORD3;

                UNITY_FOG_COORDS(4)
            };

            v2f vert(appdata v)
            {
                v2f o;

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // object space to tangent space rotation
                TANGENT_SPACE_ROTATION;

                // tangent space to view space rotation
                rotation = mul((float3x3)UNITY_MATRIX_IT_MV, transpose(rotation));
                
                o.tangent = rotation[0];
                o.binormal = rotation[1];
                o.normal = rotation[2];

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                // get tangent space normal
                float3 tNormal = UnpackNormal(tex2D(_BumpMap, i.uv));

                // rotate tangent space normal into view space and decode
                float3 vNormal = normalize(float3(dot(i.tangent, tNormal), dot(i.binormal, tNormal), dot(i.normal, tNormal))) * 0.5 + 0.5;

                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col;

                fixed4 diff = tex2D(_MatCapDiff, vNormal.xy) * _Color * tex;
                fixed4 refl = tex2D(_MatCapRefl, vNormal.xy) * _Reflection;

                diff.rgb = diff.rgb + (diff.rgb * _EmissionStrength);

                col = lerp(diff + refl, diff * refl, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
