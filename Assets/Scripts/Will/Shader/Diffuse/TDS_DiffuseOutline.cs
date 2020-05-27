﻿using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class TDS_DiffuseOutline : MonoBehaviour
{
    [SerializeField, Space]
    bool enableOutline = true;

    [SerializeField, Space]
    bool autoChangeMaterial = true;

    [SerializeField,Space]
    Color color = Color.white;
	
    SpriteRenderer spriteRenderer;

    [HideInInspector]
    public SpriteRenderer SpriteRenderer 
    {
		get{
			if(spriteRenderer == null){
				spriteRenderer = GetComponent<SpriteRenderer>();
			}
			return spriteRenderer;
		}
	}

	[SerializeField,Range(0,15), Space]
	float outlineSize = 1;

    Color disabledColor = Color.black;
    float disabledSize = 0;

	Material preMat;

    static Material defaultMaterial = null;

    public static Material DefaultMaterial
    {
        get
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = Resources.Load<Material>("DiffuseOutline");
            }
            return defaultMaterial;
        }
    }

    public void DisableOutline()
    {
        enableOutline = false;
        MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
        SpriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_OutlineSize", disabledSize);
        _mpb.SetColor("_OutlineColor", disabledColor);        
        SpriteRenderer.SetPropertyBlock(_mpb);
    }

    public void EnableOutline()
    {
        enableOutline = true;
        MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
        SpriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_OutlineSize", outlineSize);
        _mpb.SetColor("_OutlineColor", color);
        SpriteRenderer.SetPropertyBlock(_mpb);
    }

    public void EnableOutline(Color _color,float _outlineSize)
    {
        enableOutline = true;
        color = _color;
        outlineSize = _outlineSize;
        MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
        SpriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_OutlineSize", outlineSize);
        _mpb.SetColor("_OutlineColor", color);
        SpriteRenderer.SetPropertyBlock(_mpb);
    }

    void OnEnable()
    {
		preMat = SpriteRenderer.sharedMaterial;
        if (autoChangeMaterial)
        {
            SpriteRenderer.sharedMaterial = DefaultMaterial;
        }
		UpdateOutline();
	}

	void OnDisable()
    {
		if (autoChangeMaterial) SpriteRenderer.sharedMaterial = preMat;
	}

	void OnValidate()
    {
		if(enabled)
        {
			UpdateOutline();
		}
	}

    void UpdateOutline()
    {
        if (!enableOutline) DisableOutline();
        if (enableOutline) EnableOutline();
    }
}