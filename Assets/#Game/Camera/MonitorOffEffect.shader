Shader "Custom/MonitorOffEdgesToCenter"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {} // ваша RenderTexture
        _Off ("Off Amount", Range(0,1)) = 0         // параметр эффекта: 0 – экран включён, 1 – экран выключен
        _EdgeSmooth ("Edge Smoothness", Range(0, 0.1)) = 0.01 // для небольшого сглаживания границы (можно уменьшить для максимально резкого эффекта)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float _Off;
            float _EdgeSmooth;
            float4 _MainTex_ST;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем исходный цвет из RenderTexture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Вычисляем расстояние от центра экрана (0.5, 0.5)
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                
                // Максимальное расстояние в UV (от центра до угла) ~0.7071
                float maxDist = 0.7071;
                
                // Вычисляем порог: при _Off = 0 порог равен maxDist (то есть экран полностью активен),
                // а при _Off = 1 порог равен 0 (все затемнено).
                float threshold = lerp(maxDist, 0.0, _Off);
                
                // Создаем маску: для пикселей, находящихся дальше от центра, чем threshold, устанавливаем значение 1 (выключено).
                // Используем smoothstep для небольшого сглаживания границы; если нужен максимально резкий эффект, можно заменить на step().
                float mask = smoothstep(threshold - _EdgeSmooth, threshold + _EdgeSmooth, dist);
                
                // Применяем маску: пиксели вне области (mask = 1) становятся черными, внутри остаются неизменными.
                col.rgb *= (1.0 - mask);
                
                return col;
            }
            ENDCG
        }
    }
}
