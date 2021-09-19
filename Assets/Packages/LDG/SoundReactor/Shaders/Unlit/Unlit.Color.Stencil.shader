﻿Shader "SoundReactor/Unlit/Color Stencil"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _EmissionStrength("Emission Strength", Range(0, 8)) = 0.0

        _Stencil("Stencil ID [0;255]", Int) = 0
        _ReadMask("ReadMask [0;255]", Int) = 255
        _WriteMask("WriteMask [0;255]", Int) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Int) = 6
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Operation", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail("Stencil ZFail", Int) = 0
    }

    SubShader
    {
        Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }

        ZTest Less

        Pass
        {
            Stencil
            {
                Ref[_Stencil]
                ReadMask[_ReadMask]
                WriteMask[_WriteMask]
                Comp[_StencilComp]
                Pass[_StencilOp]
                Fail[_StencilFail]
                ZFail[_StencilZFail]
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _EmissionStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _Color * i.color;

                return fixed4(col.rgb + (col.rgb * _EmissionStrength), col.a);
            }

            ENDCG
        }
    }
}