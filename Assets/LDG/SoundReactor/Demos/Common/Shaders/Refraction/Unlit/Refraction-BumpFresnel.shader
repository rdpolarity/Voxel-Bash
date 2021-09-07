Shader "Refraction/Bump Fresnel"
{
    Properties
    {
        _Color("Color", COLOR) = (1,1,1,0.5)
        _Exposure ("Exposure", Float) = 1.0
        _Distortion("Distortion", Float) = 100
        _RimColor("Rim Color", COLOR) = (1,1,1,1)
        _RimPower("Rim Power", Float) = 2
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        // Grab pixels on the screen and store into _ScreenTexture
        GrabPass
        {
            "_ScreenTexture"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
                float4 tangent  : TANGENT;
            };

            struct v2f
            {
                float4 vertex       : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 viewDirOrtho      : TEXCOORD1;
                float3 viewDirPerspective   : TEXCOORD2;
                float4 screenPos    : TEXCOORD3;
                float2 offsetScale  : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };

            sampler2D   _MainTex;
            float4      _MainTex_ST;

            sampler2D   _BumpMap;
            sampler2D   _ScreenTexture;
            float2      _ScreenTexture_TexelSize;
            float4      _Color;
            float       _Exposure;
            float       _Distortion;
            float4      _RimColor;
            float       _RimPower;
            
            v2f vert (appdata v)
            {
                v2f o;

                // calculate typical vertex and uv position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // tangent space to object space matrix
                TANGENT_SPACE_ROTATION;

                o.viewDirPerspective = mul(rotation, ObjSpaceViewDir(v.vertex));

                // calculate tangent space view dir (object space normal to tangent space)
                o.viewDirOrtho = COMPUTE_VIEW_NORMAL;

                // determine a scale that will undo the projection scale (fov can be modified without scaling the distortion)
                float undoProjection = UNITY_PI * unity_CameraProjection._m11 * 100;

                // determine a scale that can be used to adjust for different screen sizes.
                float screenScale = _ScreenParams.y * 0.002;

                // calculate normalized screen pos
                o.screenPos = ComputeGrabScreenPos(o.vertex);

                float2 texelSize = _ScreenTexture_TexelSize.xy;

                // multiply by projection up direction
                texelSize.y *= _ProjectionParams.x;

                // calculate offset scale
                o.offsetScale = texelSize * _Distortion * undoProjection * screenScale;

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 tintColor = tex2D(_MainTex, i.uv).rgb;
                float3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 viewDirOrtho = normalize(i.viewDirOrtho);
                
                float ndotv = saturate(dot(normal, viewDirOrtho));

                // compute refraction offset
                i.screenPos.xy += (normal.xy - viewDirOrtho.xy) * i.offsetScale * -cos(ndotv * UNITY_PI);

                // get screen color at offset screen pos
                float3 screenColor = tex2Dproj(_ScreenTexture, i.screenPos).xyz;

                // blend the screen color with the tint color
                fixed3 finalColor = lerp(screenColor * tintColor * _Exposure, tintColor, _Color.a)  * _Color.rgb;

                ndotv = saturate(dot(normal, normalize(i.viewDirPerspective)));

                // add fresnel to final color
                finalColor += pow(1 - ndotv, _RimPower) * _RimColor;
                
                // do fog stuff to final color
                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                return float4(finalColor, 1);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
