using UnityEngine;

[ExecuteInEditMode]
public class SHACONTROLLER_OldMovieEffect : MonoBehaviour
{
    #region F/P
    [SerializeField]
        Shader curentShader;

    [SerializeField, Range(0,1), Space(20)]
        float oldFilmEffectAmount = .5f;
    [SerializeField]
        Color sepiaColor = Color.white;

    [SerializeField,Space(20)]
        Texture2D vignetteTexture;
    [SerializeField, Range(0,1), Space(5)]
        float vignetteAmount = .5f;

    [SerializeField, Space(20)]
        Texture2D scratchesTexture;
    [SerializeField, Range(0,10), Space(5)]
        float scratchesYSpeed = 10.0f;
    [SerializeField, Range(0, 10)]
        float scratchesXSpeed = 10.0f;

    [SerializeField, Space(20)]
        Texture2D dustTexture;
    [SerializeField, Range(0, 10), Space(5)]
        float dustYSpeed = 10.0f;
    [SerializeField, Range(0, 10)]
        float dustXSpeed = 10.0f;

    Material screenMat;
    float randomValue;

    Material ScreenMat
    {
        get
        {
            if (screenMat == null)
            {
                screenMat = new Material(curentShader);
                screenMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return screenMat;
        }
    }
    #endregion

    #region Methods
    #region ShaderController
    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curentShader != null)
        {
            ScreenMat.SetColor("_SepiaColor", sepiaColor);
            ScreenMat.SetFloat("_VignetteAmount", vignetteAmount);
            ScreenMat.SetFloat("_EffectAmount", oldFilmEffectAmount);

            if (vignetteTexture)
            {
                ScreenMat.SetTexture("_VignetteTex", vignetteTexture);
            }

            if (scratchesTexture)
            {
                ScreenMat.SetTexture("_ScratchesTex", scratchesTexture);
                ScreenMat.SetFloat("_ScratchesYSpeed", scratchesYSpeed);
                ScreenMat.SetFloat("_ScratchesXSpeed", scratchesXSpeed);
            }

            if (dustTexture)
            {
                ScreenMat.SetTexture("_DustTex", dustTexture);
                ScreenMat.SetFloat("_dustYSpeed", dustYSpeed);
                ScreenMat.SetFloat("_dustXSpeed", dustXSpeed);
                ScreenMat.SetFloat("_RandomValue", randomValue);
            }

            Graphics.Blit(sourceTexture, destTexture, ScreenMat);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
    #endregion

    #region UnityMethods
    void OnDisable()
    {
        if (screenMat)
        {
            DestroyImmediate(screenMat);
        }
    }

    //void Start()
    //{
    //    if (!SystemInfo.supportsImageEffects)
    //    {
    //        enabled = false;
    //        return;
    //    }

    //    if (!curentShader && !curentShader.isSupported)
    //    {
    //        enabled = false;
    //    }
    //}

    void Update()
    {
        vignetteAmount = Mathf.Clamp01(vignetteAmount);
        oldFilmEffectAmount = Mathf.Clamp(oldFilmEffectAmount, 0f, 1.5f);
        randomValue = Random.Range(-1f, 1f);
    }
    #endregion
    #endregion
}