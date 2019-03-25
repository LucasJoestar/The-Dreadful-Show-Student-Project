using System.Collections.Generic;
using UnityEngine;

public class GlowMaterial : Material
{
    #region F/P
    //bool
    public bool DrawOutside { get { return IsKeywordEnabled(outsideMaterialKeyword); } }
    public bool InstancingEnabled { get { return enableInstancing; } }
    //List
    static List<GlowMaterial> allMaterials = new List<GlowMaterial>();
    //string
    const string outlineShaderName = "Sprites/Outline";
    const string outsideMaterialKeyword = "SPRITE_OUTLINE_OUTSIDE";
    //Shader
    static readonly Shader outlineShader = Shader.Find(outlineShaderName);
    //Texture
    public Texture SpriteTexture { get { return mainTexture; } }
    #endregion

    #region Constructor
    public GlowMaterial (Texture _spriteTexture, bool _drawOutside = false, bool _instancingEnabled = false) : base(outlineShader)
    {
        if (!outlineShader) Debug.LogError($"{outlineShaderName}shader not found. Check in Shader folder if you find the shader SpriteOutline.");
        mainTexture = _spriteTexture;
        if (_drawOutside) EnableKeyword(outsideMaterialKeyword);
        if (_instancingEnabled) enableInstancing = true;
    }
    #endregion

    #region Meths
    public static Material ShareMaterial (GlowEffect _glowEffect)
    {
        for (int i = 0; i < allMaterials.Count; i++)
        {
            if (allMaterials[i].SpriteTexture == _glowEffect.Renderer.sprite.texture &&
                allMaterials[i].DrawOutside == _glowEffect.DrawOutside &&
                allMaterials[i].InstancingEnabled == _glowEffect.EnableInstancing)
                return allMaterials[i];
        }
        GlowMaterial material = new GlowMaterial(_glowEffect.Renderer.sprite.texture, _glowEffect.DrawOutside, _glowEffect.EnableInstancing);
        material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
        allMaterials.Add(material);
        return material;
    }
    #endregion
}