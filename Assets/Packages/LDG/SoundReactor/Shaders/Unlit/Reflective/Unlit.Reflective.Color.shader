Shader "SoundReactor/Unlit/Reflective/Color"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Reflection ("Reflection", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0
        _Cube ("Reflection", Cube) = "black" {}
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

            samplerCUBE _Cube;

            struct appdata
            {
                float4 vertex   : POSITION;
                float3 normal   : NORMAL;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float3 refl     : TEXCOORD0;

                UNITY_FOG_COORDS(1)
            };

            v2f vert(appdata v)
            {
                v2f o;

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);

                float3x3 rotation = float3x3(normalize(unity_ObjectToWorld[0].xyz), normalize(unity_ObjectToWorld[1].xyz), normalize(unity_ObjectToWorld[2].xyz));

                // view direction in world space derrived from subtracting camera pos from world space vertex.
                float3 view = UnityWorldSpaceViewDir(mul(rotation, v.vertex.xyz));

                // reflect([world space view direction], [world space normal])
                o.refl = reflect(-view, mul(rotation, v.normal));

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                float4 diff = _Color;
                float4 refl = texCUBE(_Cube, i.refl) * _Reflection;

                diff.rgb = diff.rgb + (diff.rgb * _EmissionStrength);

                fixed4 col = lerp(diff + refl, diff * refl, _Metallic);

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
