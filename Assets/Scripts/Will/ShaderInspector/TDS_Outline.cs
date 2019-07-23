using UnityEngine;

[ExecuteInEditMode]
public class TDS_Outline : MonoBehaviour
{
    [SerializeField, Space]
    bool enableOutline = true;
    [SerializeField,Space]
    Color color = Color.white;

	[SerializeField, Space]
    SpriteRenderer spriteRenderer;

	public SpriteRenderer SpriteRenderer{
		get{
			if(spriteRenderer == null){
				spriteRenderer = GetComponent<SpriteRenderer>();
			}
			return spriteRenderer;
		}
	}
	[SerializeField,Range(0,15), Space]
	float outlineSize = 1;

	Material preMat;

    static Material defaultMaterial = null;

    public static Material DefaultMaterial
    {
        get
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = Resources.Load<Material>("Outline");
            }
            return defaultMaterial;
        }
    }

    public void DisableOutline()
    {
        if(enableOutline)enableOutline = false;
        color = Color.black;
        outlineSize = 0;
    }

    void OnEnable() {
		preMat = SpriteRenderer.sharedMaterial;
		SpriteRenderer.sharedMaterial = DefaultMaterial;
		UpdateOutline(outlineSize);
	}

	void OnDisable() {
		SpriteRenderer.sharedMaterial = preMat;
	}

	void OnValidate(){
		if(enabled){
			UpdateOutline(outlineSize);
		}
	}

    void UpdateOutline(float outline)
    {
        if (!enableOutline) DisableOutline();
        MaterialPropertyBlock _mpb = new MaterialPropertyBlock();
        SpriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_OutlineSize", outline);
        _mpb.SetColor("_OutlineColor", color);
        SpriteRenderer.SetPropertyBlock(_mpb);
    }

}