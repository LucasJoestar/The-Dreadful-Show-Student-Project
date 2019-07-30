// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ChangeColor"
{
     Properties
    {
        _MainTex("Sprite", 2D) = "white" {}
        _ColorToChange("Color You Want To Change", Color) = (0,0,1,1)
        _DesiredColor("Desired Color ", Color) = (1,0,0,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Transparent+1"
        }

        Pass
        {
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile DUMMY PIXELSNAP_ON

        sampler2D _MainTex;
        float4 _ColorToChange;
        float4 _DesiredColor;

        struct Vertex
        {
            float4 vertex : POSITION;
            float2 uv_MainTex : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
        };

        struct Fragment
        {
            float4 vertex : POSITION;
            float2 uv_MainTex : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
        };

        Fragment vert(Vertex v)
        {
            Fragment o;

            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv_MainTex = v.uv_MainTex;
            o.uv2 = v.uv2;

            return o;
        }

        float4 frag(Fragment IN) : COLOR
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);

                if (c.r >= _ColorToChange.r - .2 && c.r <= _ColorToChange.r + .2
                && c.g >= _ColorToChange.g - .2 && c.g <= _ColorToChange.g + .2
                    && c.b >= _ColorToChange.b - .2 && c.b <= _ColorToChange.b + .2)
            {
                return _DesiredColor;
            }

            return c;
        }
            ENDCG
        }
    }
}
