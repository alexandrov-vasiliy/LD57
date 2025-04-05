Shader "Custom/SonarRevealSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Center ("Scan Center", Vector) = (0,0,0,0)
        _ScanAngle ("Scan Angle", Float) = 0
        _ScanWidth ("Scan Width", Float) = 0.2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Center;
            float _ScanAngle;
            float _ScanWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 world = i.worldPos.xy;
                float2 center = _Center.xy;
                float2 dir = world - center;

                float angle = atan2(dir.y, dir.x);
                angle = fmod(angle + 6.2831853, 6.2831853); // 0–2π

                float diff = abs(angle - _ScanAngle);
                diff = min(diff, 6.2831853 - diff);

                // если пиксель находится в зоне сканирования
                float reveal = smoothstep(_ScanWidth, 0.0, diff);

                float4 col = tex2D(_MainTex, i.uv);
                col.a *= reveal;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}