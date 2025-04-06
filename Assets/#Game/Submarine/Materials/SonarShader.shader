Shader "Custom/SonarShader"
{
    Properties
    {
        _Center ("Center", Vector) = (0, 0, 0, 0)
        _TimeValue ("Time", Float) = 0
        _RotationSpeed ("Rotation Speed", Float) = 1.0
        _LineWidth ("Line Width", Float) = 0.05
        _HitCount ("Hit Count", Float) = 0
        [HideInInspector]_HitPoints("Hit Points", Vector) = (0, 0, 0, 0)
        _Color ("Line Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            float4 _Color;
            float4 _Center;
            float _TimeValue;
            float _RotationSpeed;
            float _LineWidth;
            float _HitCount;
            float4 _HitPoints[50];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 worldPos = i.worldPos.xy;      // Изменено с xz на xy
                float2 center = _Center.xy;
                float2 dir = worldPos - center;

                // Угол текущего пикселя
                float angle = atan2(dir.y, dir.x);
                angle = fmod(angle + 6.2831853, 6.2831853); // от 0 до 2π

                // Угол вращающейся линии
                float scanAngle = fmod(_TimeValue * _RotationSpeed, 6.2831853);
                float diff = abs(angle - scanAngle);
                diff = min(diff, 6.2831853 - diff); // циклическая разница

                float scanLine = smoothstep(_LineWidth, 0.0, diff);
                

                float alpha = scanLine;
                return float4(_Color.rgb, alpha * _Color.a); // белая линия + следы
            }
            ENDCG
        }
    }

    FallBack "Unlit/Transparent"
}
