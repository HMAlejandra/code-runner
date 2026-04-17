Shader "Custom/AuroraShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _ColorA ("Color Verde", Color) = (0.1, 1.0, 0.4, 0.6)
        _ColorB ("Color Azul", Color) = (0.1, 0.4, 1.0, 0.5)
        _ColorC ("Color Morado", Color) = (0.6, 0.1, 1.0, 0.4)
        _WaveSpeed ("Wave Speed", Float) = 0.3
        _WaveScale ("Wave Scale", Float) = 2.0
        _Intensity ("Intensity", Float) = 1.5
        _TintColor ("Tint Color", Color) = (0.1, 1.0, 0.4, 1.0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _ColorA, _ColorB, _ColorC, _TintColor;
            float _WaveSpeed, _WaveScale, _Intensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y;

                // Ondas animadas
                float2 uv = i.uv;
                uv.x += sin(uv.y * _WaveScale + t * _WaveSpeed) * 0.05;
                uv.y += cos(uv.x * _WaveScale * 0.7 + t * _WaveSpeed * 0.5) * 0.03;

                // Ruido para textura organica
                float noise1 = tex2D(_NoiseTex, uv + float2(t * 0.02, 0)).r;
                float noise2 = tex2D(_NoiseTex, uv * 1.5 + float2(0, t * 0.015)).g;
                float combined = noise1 * noise2;

                // Mezcla de colores
                float cycle = frac(t * 0.2);
                fixed4 col;
                if (cycle < 0.33)
                    col = lerp(_ColorA, _ColorB, cycle * 3.0);
                else if (cycle < 0.66)
                    col = lerp(_ColorB, _ColorC, (cycle - 0.33) * 3.0);
                else
                    col = lerp(_ColorC, _ColorA, (cycle - 0.66) * 3.0);

                // Forma de cortina (mas brillante en el centro)
                float curtain = smoothstep(0.0, 0.3, uv.y) * smoothstep(1.0, 0.6, uv.y);
                col.a = combined * curtain * _Intensity * col.a;

                return col * _TintColor;
            }
            ENDCG
        }
    }
}
