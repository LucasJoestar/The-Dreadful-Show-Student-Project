using UnityEngine;

[ExecuteInEditMode]
public class TDS_SwapColor : MonoBehaviour
{
    #region Fields / Properties
    bool enableSwap = true;
    [Space] public bool EnableSwap = true;
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
    //Custom
    [SerializeField, Space, Space, Space]
    Color eyedropperCustom = new Color(1f, 1f, 1f, 0);
    [SerializeField]
    Color replacementCustom = new Color(1f, 1f, 1f, 0);
    [SerializeField, Range(0, 1)]
    float customTo = 0;

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

    static Material defaultMaterial = null;

    public static Material DefaultMaterial
    {
        get
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = Resources.Load<Material>("SwapColor");
            }
            return defaultMaterial;
        }
    }
    #endregion

    #region Methods
    #region Original Methods 
    void UpdateColor()
    {
        float _valueBoolToFloat;
        if (EnableSwap)
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
        //Custom
        _mpb.SetColor("_Custom", eyedropperCustom);
        _mpb.SetColor("_ColorReplacement7", replacementCustom);
        _mpb.SetFloat("_LerpValue7", customTo);
        SpriteRenderer.SetPropertyBlock(_mpb);
    }

    void UpdateColor(Color _replacementRed, float _redTo,
                     Color _replacementGreen, float greenTo,
                     Color _replacementBlue, float blueTo)
    {
        float _valueBoolToFloat;
        if (EnableSwap)
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
        _mpb.SetColor("_ColorReplacement7", replacementCustom);
        _mpb.SetFloat("_LerpValue7", customTo);
        SpriteRenderer.SetPropertyBlock(_mpb);
    }

    #endregion

    #region Shader Methods
    void OnEnable()
    {
        preMat = SpriteRenderer.sharedMaterial;
        SpriteRenderer.sharedMaterial = DefaultMaterial;
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
    #endregion

    private void Update()
    {
        if (enableSwap != EnableSwap)
        {
            enableSwap = EnableSwap;
            UpdateColor();
        }
    }
    #endregion
}