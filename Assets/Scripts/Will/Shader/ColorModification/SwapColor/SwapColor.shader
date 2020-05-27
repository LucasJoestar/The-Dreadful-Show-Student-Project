﻿Shader "Custom/SwapColor"
{
     Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        [HideInInspector]_color("Color", Color) = (1.0,1.0,1.0,1.0)
        [HideInInspector]_Red("Filter Color 1", Color) = (1.0,0,0,1.0) //Red
        [HideInInspector]_Green("Filter Color 2", Color) = (0,1.0,0,1.0) //Green
        [HideInInspector]_Blue("Filter Color 3", Color) = (0,0,1.0,1.0) //Blue
        [HideInInspector]_Yellow("Filter Color 4", Color) = (1.0,1.0,0,1.0) //Yellow
        [HideInInspector]_Purple("Filter Color 5", Color) = (1.0,0,1.0,1.0) //Purple
        [HideInInspector]_Cyan("Filter Color 6", Color) = (0,1.0,1.0,1.0) //Cyan
        [PerRendererData]_Custom("Filter Color 7", Color) = (1.0,1.0,1.0,1.0) //Custom

        [PerRendererData]_EnableSwapColor("Enable swap",Float) = 1 //Act like boolean

        [PerRendererData]_BlinkColor("Blink color", Color) = (1.0,0,0,0)

        [PerRendererData]_ColorReplacement1("Red", Color) = (1.0,0,0,0) //Red
        [PerRendererData]_LerpValue1("Red to",Range(0,1)) = 0
        [PerRendererData]_ColorReplacement2("Green", Color) = (0,1.0,0,0) //Green
        [PerRendererData]_LerpValue2("Green to",Range(0,1)) = 0
        [PerRendererData]_ColorReplacement3("Blue", Color) = (0,0,1.0,0) //Blue
        [PerRendererData]_LerpValue3("Blue to",Range(0,1)) = 0
        [PerRendererData]_ColorReplacement4("Yellow", Color) = (1.0,1.0,0,0) //Yellow
        [PerRendererData]_LerpValue4("Yellow to",Range(0,1)) = 0
        [PerRendererData]_ColorReplacement5("Purple", Color) = (1.0,0,1.0,0) //Purple
        [PerRendererData]_LerpValue5("Purple to",Range(0,1)) = 0
        [PerRendererData]_ColorReplacement6("Cyan", Color) = (0,1.0,1.0,0) //Cyan
        [PerRendererData]_LerpValue6("Cyan to",Range(0,1)) = 0
        [PerRendererData]_ColorReplacement7("Custom", Color) = (1.0,1.0,1.0,0) //Custom
        [PerRendererData]_LerpValue7("Custom to",Range(0,1)) = 0
        [HideInInspector]_FinalColor("Final",Color) = (0,0,0,0)
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
        } 
        ZTest Always
        ZWrite Off  
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ ENABLE_SWAP
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _color;

            //MainColors
             float4 _Red, _Green, _Blue, _Yellow, _Purple, _Cyan, _Custom, _BlinkColor, _FinalColor;

            //Colors Replacement
             float4 _ColorReplacement1, _ColorReplacement2, _ColorReplacement3, _ColorReplacement4, _ColorReplacement5, _ColorReplacement6, _ColorReplacement7;

             float _LerpValue1, _LerpValue2, _LerpValue3, _LerpValue4, _LerpValue5, _LerpValue6, _LerpValue7, _LerpValue, _EnableSwapColor;

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

                 if(_color.a<=0.15)
                {
                    return half4(0,0,0,0);
                }
                   float4 transparent = float4(0,0,0,0);
                   float4 Red = lerp(_ColorReplacement1,transparent,smoothstep(0,_LerpValue1,dis(_Red)));
                   float4 Green = lerp(_ColorReplacement2,transparent,smoothstep(0,_LerpValue2,dis(_Green)));
                   float4 Blue = lerp(_ColorReplacement3,transparent,smoothstep(0,_LerpValue3,dis(_Blue)));
                   float4 Yellow = lerp(_ColorReplacement4,transparent,smoothstep(0,_LerpValue4,dis(_Yellow)));
                   float4 Purple = lerp(_ColorReplacement5,transparent,smoothstep(0,_LerpValue5,dis(_Purple)));
                   float4 Cyan = lerp(_ColorReplacement6,transparent,smoothstep(0,_LerpValue6,dis(_Cyan)));
                   float4 Custom = lerp(_ColorReplacement7,transparent,smoothstep(0,_LerpValue7,dis(_Custom)));                    

                   if(_EnableSwapColor == 1)
                   {
                       _FinalColor = _color + (Green + Red + Blue + Yellow + Purple + Cyan + Custom);
                   }
                   if(_EnableSwapColor == 0)
                   {
                        _FinalColor = _color + _BlinkColor;
                   }
                   return  _FinalColor;

                   //#ifdef ENABLE_SWAP
                        //return _color + (Green + Red + Blue + Yellow + Purple + Cyan + Custom);
                   //#else
                        //return _color + _BlinkColor;
                   //#endif
            }
            ENDCG
        }
    }
}