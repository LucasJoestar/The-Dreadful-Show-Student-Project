Shader "Custom/SwapColor"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}


        [HideInInspector]_color("Color", Color) = (1.0,1.0,1.0,1.0)

        [HideInInspector]_Red("Filter Color 1", Color) = (1.0,0,0,1.0) //Red
        [HideInInspector]_Green("Filter Color 2", Color) = (0,1.0,0,1.0) //Green
        [HideInInspector]_Blue("Filter Color 3", Color) = (0,0,1.0,1.0) //Blue
        [HideInInspector]_Yellow("Filter Color 4", Color) = (1.0,1.0,0,1.0) //Yellow
        [HideInInspector]_Purple("Filter Color 5", Color) = (1.0,0,1.0,1.0) //Purple
        [HideInInspector]_Cyan("Filter Color 6", Color) = (0,1.0,1.0,1.0) //Cyan
        [HideInInspector]_White("Filter Color 7", Color) = (1.0,1.0,1.0,1.0) //Cyan


        _ColorReplacement1("Red", Color) = (1.0,0,0,0) //Red
        _LerpValue1("_LerpValue1",Range(0,1)) = 0
        _ColorReplacement2("Green", Color) = (0,1.0,0,0) //Green
        _LerpValue2("_LerpValue2",Range(0,1)) = 0
        _ColorReplacement3("Blue", Color) = (0,0,1.0,0) //Blue
        _LerpValue3("_LerpValue3",Range(0,1)) = 0
        _ColorReplacement4("Yellow", Color) = (1.0,1.0,0,0) //Yellow
        _LerpValue4("_LerpValue4",Range(0,1)) = 0
        _ColorReplacement5("Purple", Color) = (1.0,0,1.0,0) //Purple
        _LerpValue5("_LerpValue5",Range(0,1)) = 0
        _ColorReplacement6("Cyan", Color) = (0,1.0,1.0,0) //Cyan
        _LerpValue6("_LerpValue5",Range(0,1)) = 0
        _ColorReplacement7("White", Color) = (1.0,1.0,1.0,0) //White
        _LerpValue7("_LerpValue6",Range(0,1)) = 0
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
        } 
        ZWrite Off  
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            fixed4 _color;

            //MainColors
            uniform float4 _Red;
            uniform float4 _Green;
            uniform float4 _Blue;
            uniform float4 _Yellow;
            uniform float4 _Purple;
            uniform float4 _Cyan;
            uniform float4 _White;

            //Colors Replacement
            uniform float4 _ColorReplacement1;
            uniform float4 _ColorReplacement2;
            uniform float4 _ColorReplacement3;
            uniform float4 _ColorReplacement4;
            uniform float4 _ColorReplacement5;
            uniform float4 _ColorReplacement6;
            uniform float4 _ColorReplacement7;



            uniform float _LerpValue1;
            uniform float _LerpValue2;
            uniform float _LerpValue3;
            uniform float _LerpValue4;
            uniform float _LerpValue5;
            uniform float _LerpValue6;
            uniform float _LerpValue7;

            float _LerpValue;


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

            half dis(float4 c){
            half result = (pow(_color.r - c.r,2.0)+pow(_color.g - c.g,2.0)+pow(_color.b - c.b,2.0));
            return result;
            }

            fixed4 frag (v2f i) : Color
            {
                 _color = tex2D(_MainTex, i.uv);

                 if(_color.a<=0.15){
                    return half4(0,0,0,0);
                }

                   float4 transparent = float4(0,0,0,0);
                   float4 Red = lerp(_ColorReplacement1,transparent,smoothstep(0,_LerpValue1,dis(_Red)));
                   float4 Green = lerp(_ColorReplacement2,transparent,smoothstep(0,_LerpValue2,dis(_Green)));
                   float4 Blue = lerp(_ColorReplacement3,transparent,smoothstep(0,_LerpValue3,dis(_Blue)));
                   float4 Yellow = lerp(_ColorReplacement4,transparent,smoothstep(0,_LerpValue4,dis(_Yellow)));
                   float4 Purple = lerp(_ColorReplacement5,transparent,smoothstep(0,_LerpValue5,dis(_Purple)));
                   float4 Cyan = lerp(_ColorReplacement6,transparent,smoothstep(0,_LerpValue6,dis(_Cyan)));
                   float4 White = lerp(_ColorReplacement7,transparent,smoothstep(0,_LerpValue7,dis(_White)));

                   return _color + (Green + Red + Blue + Yellow + Purple + Cyan + White);
            }
            ENDCG
        }
    }
}