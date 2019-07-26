Shader "Custom/DiffuseOutline"
{
    Properties
    {
        [HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        //
        [PerRendererData] _OutlineSize ("Outline", Float) = 0
        [PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
        //
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
  SubShader
    {
		Tags 
        { 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
		
		UsePass "Custom/Diffuse"
        UsePass "Custom/Outline"
	} 
    Fallback "Custom/Outline"
}