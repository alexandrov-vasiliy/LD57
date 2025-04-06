Shader "Custom/CRTGlitchWithNoise_NoColorDistortion_Fisheye" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _GlitchIntensity("Glitch Intensity", Range(0,1)) = 0.5
        _NoiseIntensity("Noise Intensity", Range(0,1)) = 0.5
        _FisheyeIntensity("Fisheye Intensity", Range(0,1)) = 0.3
    }
    SubShader{
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass{
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float _GlitchIntensity;
            float _NoiseIntensity;
            float _FisheyeIntensity;
            
            // Функция генерации шума на основе координат
            float rand(float2 co){
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }
            
            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Применяем эффект "рыбьего глаза"
                float2 centeredUV = uv - 0.5;
                float r = length(centeredUV);
                if(r > 0.0) {
                    // Чем больше _FisheyeIntensity, тем сильнее эффект.
                    // Используем нелинейное масштабирование расстояния.
                    float rDistorted = pow(r, 1.0 - _FisheyeIntensity);
                    centeredUV = (centeredUV / r) * rDistorted;
                }
                uv = centeredUV + 0.5;
                
                // Применяем смещение для глитч эффекта
                uv.x += (sin(uv.y * 50.0 + _Time.y * 10.0) * _GlitchIntensity * 0.01);
                
                fixed4 col = tex2D(_MainTex, uv);
                
                // Добавляем эффект шума
                float noise = rand(uv + _Time.y);
                col.rgb += (noise - 0.5) * _NoiseIntensity * 0.2;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
