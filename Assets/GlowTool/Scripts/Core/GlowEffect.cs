using UnityEngine;

[AddComponentMenu("Effects/Sprite Glow")]
[RequireComponent(typeof(SpriteRenderer)), DisallowMultipleComponent, ExecuteInEditMode]
public class GlowEffect : MonoBehaviour
{
    #region Fields/Properties
    //bool
    [SerializeField]
    bool drawOutside = true;
    public bool DrawOutside
    { get { return drawOutside; } set { if (drawOutside != value) { drawOutside = value; SetMaterialProperties(); } } }
    //[SerializeField]
    bool enableInstancing = false;
    public bool EnableInstancing
    { get { return enableInstancing; } set { if (enableInstancing != value) { enableInstancing = value; SetMaterialProperties(); } } }
    //Color
    [SerializeField]
    Color glowColor = Color.white;
    public Color GlowColor
    { get { return glowColor; } set { if (glowColor != value) { glowColor = value; SetMaterialProperties(); } } }
    //float
    [SerializeField, Range(0f, 1f)]
    float alphaThreshold = .01f;
    public float AlphaThreshold
    { get { return alphaThreshold; } set { if (alphaThreshold != value) { alphaThreshold = value; SetMaterialProperties(); } } }

    [SerializeField, Range(1, 15)]
    float glowIntensity = 2f;
    public float GlowIntensity
    { get { return glowIntensity; } set { if (glowIntensity != value) { glowIntensity = value; SetMaterialProperties(); } } }
    //int
    [SerializeField, Range(0, 15)]
    int outlineWidth = 1;
    public int OutlineWidth
    { get { return outlineWidth; } set { if (outlineWidth != value) { outlineWidth = value; SetMaterialProperties(); } } }

    static readonly int isOutlineEnabledId = Shader.PropertyToID("_IsOutlineEnabled");
    static readonly int outlineColorId = Shader.PropertyToID("_OutlineColor");
    static readonly int outlineSizeId = Shader.PropertyToID("_OutlineSize");

    static readonly int alphaThresholdId = Shader.PropertyToID("_AlphaThreshold");
    //otter
    MaterialPropertyBlock materialProperties;
    public SpriteRenderer Renderer { get; private set; }
    #region Light
    [Space]
    [SerializeField, Header("Light")]
    bool isLight = false;
    [SerializeField]
    Light lightCustom;
    float lightInt;
    [SerializeField]
    float minInt = 3f;
    [SerializeField]
    float maxInt = 5f;
    //[SerializeField]
    //Color lightColor;
    #endregion
    #endregion

    #region Methods   

    void SetMaterialProperties()
    {
        if (!Renderer) return;

        Renderer.sharedMaterial = GlowMaterial.ShareMaterial(this);

        if (materialProperties == null)
            materialProperties = new MaterialPropertyBlock();

        materialProperties.SetColor(outlineColorId, GlowColor * GlowIntensity);
        materialProperties.SetFloat(alphaThresholdId, AlphaThreshold);
        materialProperties.SetFloat(isOutlineEnabledId, isActiveAndEnabled ? 1 : 0);
        materialProperties.SetFloat(outlineSizeId, OutlineWidth);
        Renderer.SetPropertyBlock(materialProperties);
    }

    void UpdatePropertiesInRuntime()
    {
        SetMaterialProperties();
    }
    #region Light
    void changeLightIntensity()
    {
        lightInt = UnityEngine.Random.Range(minInt, maxInt);
        lightCustom.intensity = lightInt;
        lightCustom.color = glowColor;
    }    
    #endregion
    #endregion

    #region UnityMethods
    void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }   

    void OnDisable()
    {
        SetMaterialProperties();
    }

    void OnEnable()
    {
        SetMaterialProperties();
    }

    void OnValidate()
    {
        if (!isActiveAndEnabled) return;
        SetMaterialProperties();
    }

    void Update()
    {
        if (isLight)
        {
            changeLightIntensity();
        }
    }
    #endregion
} 