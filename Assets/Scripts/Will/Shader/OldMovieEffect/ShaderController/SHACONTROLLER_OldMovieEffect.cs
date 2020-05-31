using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("_Old Movie Effect")]
public class SHACONTROLLER_OldMovieEffect : MonoBehaviour
{
    #region F/P
    [SerializeField]
    Shader _shader;

    [SerializeField, Range(0, 1), Space(20)]
    float oldFilmEffectAmount = .5f;
    [SerializeField]
    Color sepiaColor = Color.white;

    [SerializeField, Space(20)]
    Texture2D vignetteTexture;
    [SerializeField, Range(0, 1), Space(5)]
    float vignetteAmount = .5f;

    [SerializeField, Space(20)]
    Texture2D scratchesTexture;
    [SerializeField, Range(0, 10), Space(5)]
    float scratchesYSpeed = 10.0f;
    [SerializeField, Range(0, 10)]
    float scratchesXSpeed = 10.0f;

    [SerializeField, Space(20)]
    Texture2D dustTexture;
    [SerializeField, Range(0, 10), Space(5)]
    float dustYSpeed = 10.0f;
    [SerializeField, Range(0, 10)]
    float dustXSpeed = 10.0f;
    //[SerializeField, Range(0, 30), Space(5)]
    //int fpsEffect = 14;
    [SerializeField, Range(0, 1), Space(20)]
    float flickIntensity = .5f;

    Material screenMat;
    float randomValue = 1;
    #endregion

    #region Methods
    #region ShaderController
    //void Flick()
    //{
    //    if (randomValue > 0f) randomValue = flickIntensity;
    //    else randomValue = flickIntensity * -1f;
    //}
    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destinationTexture)
    {
            if(screenMat == null)
            {
                screenMat = new Material(_shader);
                screenMat.hideFlags = HideFlags.DontSave;
            }
                screenMat.SetColor("_SepiaColor", sepiaColor);
                screenMat.SetFloat("_VignetteAmount", vignetteAmount);
                screenMat.SetFloat("_EffectAmount", oldFilmEffectAmount);

            if (vignetteTexture)
            {
            screenMat.SetTexture("_VignetteTex", vignetteTexture);
            }

            if (scratchesTexture)
            {
                screenMat.SetTexture("_ScratchesTex", scratchesTexture);
                screenMat.SetFloat("_ScratchesYSpeed", scratchesYSpeed);
                screenMat.SetFloat("_ScratchesXSpeed", scratchesXSpeed);
            }

            if (dustTexture)
            {
                screenMat.SetTexture("_DustTex", dustTexture);
                screenMat.SetFloat("_dustYSpeed", dustYSpeed);
                screenMat.SetFloat("_dustXSpeed", dustXSpeed);
                screenMat.SetFloat("_RandomValue", randomValue);
            }

            Graphics.Blit(sourceTexture, destinationTexture, screenMat);        
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
    //    InvokeRepeating("Flick", 1, (fpsEffect*0.024f)/60f);
    //}
     void FixedUpdate()
    {
        randomValue = Random.Range(flickIntensity * -1f, flickIntensity);
    }
    void Update()
    {
        vignetteAmount = Mathf.Clamp01(vignetteAmount);
        oldFilmEffectAmount = Mathf.Clamp(oldFilmEffectAmount, 0f, 1.5f);
        //randomValue = Random.Range(-1f, 1f);
    }
    #endregion
    #endregion
}