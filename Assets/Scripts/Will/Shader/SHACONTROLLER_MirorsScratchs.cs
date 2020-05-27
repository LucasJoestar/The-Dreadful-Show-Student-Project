﻿using UnityEngine;

[ExecuteInEditMode]
public class SHACONTROLLER_MirorsScratchs : MonoBehaviour
{
    #region F/P
    [SerializeField]
    Shader curentShader;

    [SerializeField, Range(0, 1), Space(20)]
    float oldFilmEffectAmount = .5f;
    [SerializeField]
    Color sepiaColor = Color.white;

    [SerializeField, Space(20)]
    Texture2D vignetteTexture;
    [SerializeField, Range(0, 1), Space(5)]
    float vignetteAmount = .5f;

    [SerializeField, Space(20)]
    Texture2D layer1Texture;

    [SerializeField, Space(20)]
    Texture2D layer2Texture;

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

            if (layer1Texture)
            {
                ScreenMat.SetTexture("_ScratchesTex", layer1Texture);
            }

            if (layer2Texture)
            {
                ScreenMat.SetTexture("_DustTex", layer2Texture);
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

    void Update()
    {
        vignetteAmount = Mathf.Clamp01(vignetteAmount);
        oldFilmEffectAmount = Mathf.Clamp(oldFilmEffectAmount, 0f, 1.5f);
        randomValue = Random.Range(-1f, 1f);
    }
    #endregion
    #endregion
}
