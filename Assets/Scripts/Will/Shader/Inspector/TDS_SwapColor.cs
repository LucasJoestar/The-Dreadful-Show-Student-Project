using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TDS_SwapColor : MonoBehaviour
{
    [SerializeField, Space]
    bool enableSwap = true;
    [SerializeField, Space, Space, Space]
    Color blinkColor = new Color(1f, 1f, 1f, 0);
    //R
    [SerializeField, Space, Space, Space]
    Color replacementRed = new Color(1f, 0, 0, 0);
    [SerializeField, Range(0, 1)]
    float redTo = 0;
    //G
    [SerializeField, Space, Space, Space]
    Color replacementGreen = new Color(0, 1f, 0, 0);
    [SerializeField, Range(0, 1)]
    float greenTo = 0;
    //B
    [SerializeField, Space, Space, Space]
    Color replacementBlue = new Color(0, 0, 1f, 0);
    [SerializeField, Range(0, 1)]
    float blueTo = 0;
    //C
    [SerializeField, Space, Space, Space]
    Color replacementCyan = new Color(0, 1f, 1f, 0);
    [SerializeField, Range(0, 1)]
    float cyanTo = 0;    
    //M
    [SerializeField, Space, Space, Space]
    Color replacementPurple = new Color(1f, 0, 1f, 0);
    [SerializeField, Range(0, 1)]
    float purpleTo = 0;
    //Y
    [SerializeField, Space, Space, Space]
    Color replacementYellow = new Color(1f, 1f, 0, 0);
    [SerializeField, Range(0, 1)]
    float yellowTo = 0;
    //W
    [SerializeField, Space, Space, Space]
    Color replacementWhite = new Color(1f, 1f, 1f, 0);
    [SerializeField, Range(0, 1)]
    float whiteTo = 0;

    [HideInInspector]
    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return spriteRenderer;
        }
    }

    SpriteRenderer spriteRenderer;

    Material preMat;

    //static Material defaultMaterial = null;

    //public static Material DefaultMaterial
    //{
    //    get
    //    {
    //        if (defaultMaterial == null)
    //        {
    //            defaultMaterial = Resources.Load<Material>("SwapColor");
    //        }
    //        return defaultMaterial;
    //    }
    //}

    void OnEnable()
    {
        preMat = SpriteRenderer.sharedMaterial;
        //SpriteRenderer.sharedMaterial = DefaultMaterial;
        UpdateColor();
    }

    void OnDisable()
    {
        SpriteRenderer.sharedMaterial = preMat;
    }

    void OnValidate()
    {
        if (enabled)
        {
            UpdateColor();
        }
    }

    void UpdateColor()
    {
        float _valueBoolToFloat;
        if (enableSwap)
        {
            _valueBoolToFloat = 1;
        }
        else _valueBoolToFloat = 0;

        MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
        SpriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_EnableSwapColor", _valueBoolToFloat);
        //
        _mpb.SetColor("_BlinkColor", blinkColor);
        //R
        _mpb.SetColor("_ColorReplacement1", replacementRed);
        _mpb.SetFloat("_LerpValue1", redTo);
        //G
        _mpb.SetColor("_ColorReplacement2", replacementGreen);
        _mpb.SetFloat("_LerpValue2", greenTo);
        //B
        _mpb.SetColor("_ColorReplacement3", replacementBlue);
        _mpb.SetFloat("_LerpValue3", blueTo);        
        //C
        _mpb.SetColor("_ColorReplacement6", replacementCyan);
        _mpb.SetFloat("_LerpValue6", cyanTo);
        //M
        _mpb.SetColor("_ColorReplacement5", replacementPurple);
        _mpb.SetFloat("_LerpValue5", purpleTo);
        //Y
        _mpb.SetColor("_ColorReplacement4", replacementYellow);
        _mpb.SetFloat("_LerpValue4", yellowTo);
        //W
        _mpb.SetColor("_ColorReplacement7", replacementWhite);
        _mpb.SetFloat("_LerpValue7", whiteTo);
        SpriteRenderer.SetPropertyBlock(_mpb);
    }
}