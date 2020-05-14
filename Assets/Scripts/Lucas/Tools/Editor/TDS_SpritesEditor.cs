using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TDS_SpritesEditor : EditorWindow
{
    /* TDS_SpritesEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Window allowing to dynamically change all scene sprites color, with groups.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[09 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Implemented a system to fusion groups.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[08 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_ColorLevelEditor class.
     *	
     *	    • Created base system to change color by groups.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Indicates if editor manipulation should also take selected objects children sprites.
    /// </summary>
    [SerializeField] private bool doUseChildrenSprites = true;

    /// <summary>
    /// Value used to increment sprites layer.
    /// </summary>
    [SerializeField] private int layerIncrementValue = 0;

    /// <summary>
    /// The index of the selected color group.
    /// </summary>
    [SerializeField] private int selectedGroup = -1;

    /// <summary>
    /// Scrollbar of the window.
    /// </summary>
    [SerializeField] private Vector2 scrollbar = new Vector2();

    /// <summary>
    /// Indicates if the user has selected one group or more for fusion.
    /// </summary>
    [SerializeField] private bool isInFusionMode = false;

    /// <summary>
    /// Name of the material to replace.
    /// </summary>
    [SerializeField] private string materialName = string.Empty;

    /// <summary>
    /// Material used to replace another material in the scene.
    /// </summary>
    [SerializeField] private Material material = null;

    /// <summary>
    /// Groups of loaded sprite renderers sorted by color.
    /// </summary>
    [SerializeField] private TDS_ColorGroup[] colorGroups = new TDS_ColorGroup[] { };


    private readonly GUIContent[] modes =   new GUIContent[]
                                            {
                                                new GUIContent("Color Management", "Manages all scene sprites color"),
                                                new GUIContent("Sprite Creator", "Create a single sprite from multiple ones")
                                            };

    [SerializeField] private int modeIndex = 0;

    [SerializeField] private string savePath = "Assets/";

    [SerializeField] private Transform modelRoot = null;

    private int spriteCreatorMessageID = 0;
    #endregion

    #region Methods

    #region Original Methods

    #region Menus
    /// <summary>
    /// Method to call this window from the Unity toolbar.
    /// </summary>
    [MenuItem("Window/Sprites Editor")]
    public static void CallWindow() => GetWindow<TDS_SpritesEditor>("Sprites Editor").Show();

    /// <summary>
    /// Draws this editor.
    /// </summary>
    public void DrawEditor()
    {
        DrawToolbar();

        DrawGeneralMode();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        modeIndex = GUILayout.Toolbar(modeIndex, modes, GUILayout.Height(25), GUILayout.ExpandWidth(true));

        switch (modeIndex)
        {
            case 0:
                DrawColorMode();
                break;

            case 1:
                DrawSpriteCreator();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Draws the general mode.
    /// </summary>
    public void DrawGeneralMode()
    {
        GUILayout.Space(15);

        EditorGUILayout.LabelField(new GUIContent("General", "General Sprites Edition Section"), EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);
        if (GUILayout.Button(new GUIContent("Replace Sprite Materials", "Replace all sprites default material by the one interacting with light"), GUILayout.Width(250)))
        {
            if (material == null)
            {
                Debug.Log("The selected material is null !");
                return;
            }

            SpriteRenderer[] _sprites = FindObjectsOfType<SpriteRenderer>();
            Undo.RecordObjects(_sprites, "Replace sprites material");

            foreach (SpriteRenderer _sprite in _sprites)
            {
                if (_sprite.sharedMaterial.name == materialName)
                {
                    _sprite.sharedMaterial = material;
                }
                else Debug.Log("??? => " + _sprite.sharedMaterial.name);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);

        materialName = EditorGUILayout.TextField("Material Name to Replace", materialName, GUILayout.Width(250));
        material = (Material)EditorGUILayout.ObjectField("Material to use", material, typeof(Material), true, GUILayout.Width(250));

        EditorGUILayout.EndHorizontal();

        if (Selection.gameObjects.Length == 0)
        {
            EditorGUILayout.HelpBox("No sprite actually selected !", MessageType.Info);
            return;
        }

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);

        doUseChildrenSprites = EditorGUILayout.Toggle(new GUIContent("Use children sprites", "Should the manipulations automatically also select the selected GameObjects sprites children or not"), doUseChildrenSprites);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Increment Layer", "Increment the selected GameObjects layer by the above number"), GUILayout.Height(25), GUILayout.Width(200)))
        {
            SpriteRenderer[] _sprites = null;
            SpriteMask[] _masks = null;
            if (doUseChildrenSprites)
            {
                _sprites = Selection.gameObjects.ToList().SelectMany(s => s.GetComponentsInChildren<SpriteRenderer>()).ToArray();
                _masks = Selection.gameObjects.ToList().SelectMany(s => s.GetComponentsInChildren<SpriteMask>()).ToArray();
            }
            else
            {
                _sprites = Selection.gameObjects.ToList().Select(g => g.GetComponent<SpriteRenderer>()).Distinct().ToArray();
                _masks = Selection.gameObjects.ToList().Select(s => s.GetComponent<SpriteMask>()).Distinct().ToArray();
            }

            if (_sprites != null)
            {
                Undo.RecordObjects(_sprites, "Increment Sprites layer order");

                foreach (SpriteRenderer _sprite in _sprites)
                {
                    _sprite.sortingOrder += layerIncrementValue;
                }
            }

            if (_masks != null)
            {
                Undo.RecordObjects(_masks, "Increment Sprites layer order");

                foreach (SpriteMask _mask in _masks)
                {
                    _mask.backSortingOrder += layerIncrementValue;
                    _mask.frontSortingOrder += layerIncrementValue;
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);

        if ((Selection.gameObjects.Length > 0) && GUILayout.Button("Debug Sprite(s) Group(s)", GUILayout.Width(175)))
        {
            SpriteRenderer[] _sprites = null;
            if (doUseChildrenSprites) _sprites = Selection.gameObjects.ToList().SelectMany(s => s.GetComponentsInChildren<SpriteRenderer>()).ToArray();
            else _sprites = Selection.gameObjects.ToList().Select(g => g.GetComponent<SpriteRenderer>()).Distinct().ToArray();

            if (_sprites != null)
            {
                foreach (SpriteRenderer _sprite in _sprites)
                {
                    Debug.Log(_sprite.name + " => " + colorGroups.Where(g => g.Sprites.Contains(_sprite)).First().Name);
                }
            }
        }

        GUILayout.FlexibleSpace();

        layerIncrementValue = EditorGUILayout.IntField(new GUIContent("Layer Increment value", "Value used to increment selected objects layer"), layerIncrementValue, GUILayout.Width(200));

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the color mode.
    /// </summary>
    public void DrawColorMode()
    {
        // Get non-assignated color groups and associate them with matching one if found
        for (int _i = 0; _i < colorGroups.Length; _i++)
        {
            colorGroups[_i].Sprites = colorGroups[_i].Sprites.Where(s => (s != null) && s.enabled).ToList();

            SpriteRenderer[] _differents = colorGroups[_i].Sprites.Where(s => s.color != colorGroups[_i].Color).ToArray();

            if (_differents.Length > 0)
            {
                if (_differents.Length == colorGroups[_i].Sprites.Count)
                {
                    colorGroups[_i].Color = _differents[0].color;
                }

                else
                {
                    foreach (SpriteRenderer _sprite in _differents)
                    {
                        colorGroups[_i].Sprites.Remove(_sprite);
                        LoadSprite(_sprite);
                    }
                }
            }
            if (colorGroups[_i].Sprites.Count == 0)
            {
                colorGroups = colorGroups.Where(g => g != colorGroups[_i]).ToArray();
                Repaint();
            }
        }

        // Draw color groups editor
        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);
        GUILayout.Space(5);

        EditorGUILayout.LabelField(new GUIContent("Level Colors", "All colors of the loaded sprites in the level"), EditorStyles.boldLabel);

        if (colorGroups.Length == 0)
        {
            GUILayout.Space(5);

            EditorGUILayout.HelpBox(new GUIContent("No sprites loaded !", "Click on the \"Load Sprites\" button to get sprites of the scene, or add some to load them next"));

            EditorGUILayout.EndScrollView();
            return;
        }

        // Draw all folders & color groups !!
        DrawColorGroups();

        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }

    public void DrawSpriteCreator()
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField(new GUIContent("Sprite Creator", "Create single sprites from multiple ones !"), EditorStyles.boldLabel);

        float _maxWidth = (Screen.width / 2f) - 10;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(_maxWidth));

        modelRoot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Model Root"), modelRoot, typeof(Transform), true);
        EditorGUILayout.HelpBox("Model to create sprite from all its renderer in children", MessageType.Info);

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(_maxWidth));

        savePath = EditorGUILayout.TextField(new GUIContent("Save Path", "Path to save newly created sprite"), savePath);
        EditorGUILayout.HelpBox("You can copy a folder path with \"Context click/ Copy path\"", MessageType.Info);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create Sprite", GUILayout.MaxWidth(200)))
        {
            if (modelRoot)
            {
                spriteCreatorMessageID = 0;

                SpriteRenderer[] _sprites = modelRoot.GetComponentsInChildren<SpriteRenderer>();
                if (_sprites.Length > 0)
                {
                    var targetTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false, false);
                    targetTexture.filterMode = FilterMode.Point;
                    var targetPixels = targetTexture.GetPixels();
                    for (int i = 0; i < targetPixels.Length; i++) targetPixels[i] = Color.clear;// default pixels are not set
                    var targetWidth = targetTexture.width;

                    for (int i = 0; i < _sprites.Length; i++)
                    {
                        // Get sprites bounding volume
                        Vector2 _min = Vector2.zero;
                        Vector2 _max = Vector2.zero;

                        Vector2[] _sMin = new Vector2[_sprites.Length];
                        Vector2[] _sMax = new Vector2[_sprites.Length];

                        Comparison<SpriteRenderer> _comparer = (x, y) => x.sortingOrder.CompareTo(y.sortingOrder);
                        Array.Sort(_sprites, _comparer);

                        _min = _sprites[0].bounds.min;
                        _max = _sprites[0].bounds.max;

                        for (int _i = 1; _i < _sprites.Length; _i++)
                        {
                            _sMin[_i] = _sprites[_i].bounds.min;
                            _sMax[_i] = _sprites[_i].bounds.max;

                            _min.x = Mathf.Min(_min.x, _sMin[_i].x);
                            _min.y = Mathf.Min(_min.y, _sMin[_i].y);
                            _max.x = Mathf.Max(_max.x, _sMax[_i].x);
                            _max.y = Mathf.Max(_max.y, _sMax[_i].y);
                        }

                        Vector2 _size = new Vector2((int)(_max.x - _min.x), (int)(_max.y - _min.y)) * 100;
                        Texture2D _texture = new Texture2D((int)_size.x, (int)_size.y, TextureFormat.RGBA32, false, false);

                        Color[] _pixels = _texture.GetPixels();
                        for (int _i = 0; _i < _pixels.Length; _i++)
                            _pixels[_i] = Color.clear;

                        // Paint all sprites on the texture
                        for (int _i = 0; _i < _sprites.Length; _i++)
                        {
                            SpriteRenderer _sprite = _sprites[_i];
                            Vector2 _scale = _sprite.transform.localScale;
                            Rect _rect = _sprite.sprite.textureRect;
                            Texture2D _spriteTexture = _sprite.sprite.texture;

                            int _x = 0;
                            for (float _w = 0; _w < _rect.width; _w += 1 / _scale.x)
                            {
                                int _y = 0;
                                for (float _h = 0; _h < _rect.height; _h += 1 / _scale.y)
                                {
                                    Color _pixel = _spriteTexture.GetPixel((int)_rect.x + (int)_w, (int)_rect.y + (int)_h);
                                    _pixel *= _sprite.color;

                                    if (_pixel.a > 0)
                                    {
                                        Vector2Int _index = new Vector2Int((int)(_sMin[_i].x - _min.x) + _x,
                                                                           (int)(_sMin[_i].y - _min.y) + _y);

                                        // Blend colors
                                        Color _originalPixel = _texture.GetPixel(_index.x, _index.y);
                                        _pixel = _originalPixel * (1 - _pixel.a) + _pixel;

                                        _texture.SetPixel(_index.x, _index.y, _pixel);
                                    }

                                    _y++;
                                }

                                _x++;
                            }
                        }
                        _texture.Apply(false, true);

                        SpriteRenderer _newSprite = new GameObject("SPRITE").AddComponent<SpriteRenderer>();
                        _newSprite.transform.position = modelRoot.transform.position;
                        _newSprite.material = _sprites[0].sharedMaterial;
                        _newSprite.sprite = Sprite.Create(_texture, new Rect(0, 0, _size.x, _size.y), Vector2.zero);

                        return;

                        // --------------------
                        var sr = _sprites[i];
                        var position = (Vector2)sr.transform.localPosition - sr.sprite.pivot;
                        var p = new Vector2Int((int)position.x, (int)position.y);
                        var sourceWidth = sr.sprite.texture.width;
                        // if read/write is not enabled on texture (under Advanced) then this next line throws an error
                        // no way to check this without Try/Catch :(
                        var sourcePixels = sr.sprite.texture.GetPixels();
                        for (int j = 0; j < sourcePixels.Length; j++)
                        {
                            var source = sourcePixels[j];
                            var x = (j % sourceWidth) + p.x;
                            var y = (j / sourceWidth) + p.y;
                            var index = x + y * targetWidth;
                            if (index > 0 && index < targetPixels.Length)
                            {
                                var target = targetPixels[index];
                                if (target.a > 0)
                                {
                                    // alpha blend when we've already written to the target
                                    float sourceAlpha = source.a;
                                    float invSourceAlpha = 1f - source.a;
                                    float alpha = sourceAlpha + invSourceAlpha * target.a;
                                    Color result = (source * sourceAlpha + target * target.a * invSourceAlpha) / alpha;
                                    result.a = alpha;
                                    source = result;
                                }
                                targetPixels[index] = source;
                            }
                        }

                        targetTexture.SetPixels(targetPixels);
                        targetTexture.Apply(false, true);// read/write is disabled in 2nd param to free up memory

                        SpriteRenderer _spriteRenderer = new GameObject("SPRITE").AddComponent<SpriteRenderer>();
                        _spriteRenderer.transform.position = modelRoot.transform.position;
                        _spriteRenderer.sprite = Sprite.Create(targetTexture, new Rect(new Vector2(), new Vector2(1024, 1024)), new Vector2(), 1, 0, SpriteMeshType.FullRect);
                    }
                }
                else
                    spriteCreatorMessageID = 2;
            }
            else
                spriteCreatorMessageID = 1;
        }

        switch (spriteCreatorMessageID)
        {
            case 0:
                // Do nothing
                break;

            case 1:
                EditorGUILayout.HelpBox("You must select a model root to create sprite from !", MessageType.Error);
                break;

            case 2:
                EditorGUILayout.HelpBox("There must be at least one SpriteRenderer in a child GameObject of the root !", MessageType.Error);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Draws the toolbar.
    /// </summary>
    public void DrawToolbar()
    {
        // Draw toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Range", "Range the groups by name"), EditorStyles.toolbarButton))
        {
            colorGroups = colorGroups.OrderBy(g => g.Color.grayscale).ToArray();
            colorGroups = colorGroups.OrderBy(g => g.Name).ToArray();
        }

        if (GUILayout.Button(new GUIContent("Load Sprites", "Loads all sprites of this level"), EditorStyles.toolbarButton)) LoadSprites();

        GUILayout.Space(15);

        if (GUILayout.Button(new GUIContent("Reset", "Clear all loaded sprites and get them from zero point"), EditorStyles.toolbarButton))
        {
            if (EditorUtility.DisplayDialog("Confirm color groups reset", "Are you sure you want to reset all your color groups ? This action cannot be undone.", "Yes, I'm sure !", "I've changed my mind..."))
            {
                colorGroups = new TDS_ColorGroup[] { };
                LoadSprites();
                Repaint();
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button(new GUIContent("Load", "Load saved groups if existing"), EditorStyles.toolbarButton))
        {
            if (EditorUtility.DisplayDialog("Confirm color group load", "Are you sure you want to load saved groups ? Your saved groups will be erased. This action cannot be undone.", "Yes, I'm sure !", "I've changed my mind..."))
            {
                LoadGroups();
            }
        }
        if (GUILayout.Button(new GUIContent("Save", "Save color groups"), EditorStyles.toolbarButton))
        {
            SaveGroups();
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Color
    /// <summary>
    /// Loads all sprite renderers of the level.
    /// </summary>
    public void LoadSprites()
    {
        SpriteRenderer[] _sprites = FindObjectsOfType<SpriteRenderer>().Where(s => s.enabled).ToArray();

        foreach (SpriteRenderer _sprite in _sprites)
        {
            LoadSprite(_sprite);
        }
    }

    /// <summary>
    /// Loads a sprite renderer.
    /// </summary>
    /// <param name="_sprite">Sprite to load.</param>
    private void LoadSprite(SpriteRenderer _sprite)
    {
        TDS_ColorGroup _matching = colorGroups.Where(c => c.Color == _sprite.color).FirstOrDefault();
        if (_matching == null)
        {
            colorGroups = colorGroups.Append(new TDS_ColorGroup(_sprite.color, new SpriteRenderer[] { _sprite })).OrderBy(g => g.Name).ToArray();
        }
        else if (!_matching.Sprites.Contains(_sprite))
        {
            _matching.Sprites.Add(_sprite);
        }
    }

    /// <summary>
    /// Draw some color groups.
    /// </summary>
    /// <param name="_colorGroups">Groups to draw.</param>
    private void DrawColorGroups()
    {
        TDS_ColorGroup _colorGroup;

        for (int _i = 0; _i < colorGroups.Length; _i++)
        {
            _colorGroup = colorGroups[_i];

            if (selectedGroup == _i) GUI.backgroundColor = new Color(.7f, .7f, .7f);
            else GUI.backgroundColor = Color.white;

            GUILayout.Space(3);
            EditorGUILayout.BeginHorizontal(selectedGroup == _i ? "HelpBox" : EditorStyles.inspectorDefaultMargins);

            // Draw fusion mode buttons
            if (!_colorGroup.isSelected && (isInFusionMode || (Selection.gameObjects.Length > 0)))
            {
                Color _original = GUI.color;
                GUI.color = isInFusionMode ? new Color(0, .75f, 0, 1) : new Color(.5f, .5f, .8f, 1);

                if (GUILayout.Button("F", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    if (isInFusionMode)
                    {
                        TDS_ColorGroup[] _groups = colorGroups.Where(g => g.isSelected).ToArray();

                        foreach (TDS_ColorGroup _group in _groups)
                        {
                            Undo.RecordObjects(_group.Sprites.ToArray(), "fusion sprite groups color");

                            foreach (SpriteRenderer _sprite in _group.Sprites)
                            {
                                _sprite.color = _colorGroup.Color;
                            }
                            _colorGroup.Sprites.AddRange(_group.Sprites);
                        }

                        colorGroups = colorGroups.Except(_groups).ToArray();
                        isInFusionMode = false;

                        Repaint();
                    }
                    else
                    {
                        SpriteRenderer[] _sprites = Selection.gameObjects.ToList().SelectMany(s => doUseChildrenSprites ? s.GetComponentsInChildren<SpriteRenderer>() : s.GetComponents<SpriteRenderer>()).ToArray();

                        Undo.RecordObjects(_sprites, "fusion sprite groups color");

                        foreach (SpriteRenderer _sprite in _sprites)
                        {
                            _sprite.color = _colorGroup.Color;
                        }

                        _colorGroup.Sprites.AddRange(_sprites);
                    }
                }

                GUI.color = _original;
            }
            else GUILayout.Space(28);

            // Draw color group
            EditorGUI.BeginChangeCheck();
            _colorGroup.Name = EditorGUILayout.TextField(_colorGroup.Name, GUILayout.Width(250));
            if (EditorGUI.EndChangeCheck()) selectedGroup = _i;

            if (GUILayout.Button($"({_colorGroup.Sprites.Count})", EditorStyles.label, GUILayout.Width(50)))
            {
                foreach (SpriteRenderer _sprite in _colorGroup.Sprites)
                {
                    Debug.Log($"CG \"{_colorGroup.Name}\" => {_sprite.name}");
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            _colorGroup.Color = EditorGUILayout.ColorField(_colorGroup.Color);
            if (EditorGUI.EndChangeCheck())
            {
                selectedGroup = _i;

                Undo.RecordObjects(_colorGroup.Sprites.ToArray(), "change sprites group color");
                _colorGroup.Sprites.ForEach(s => s.color = _colorGroup.Color);
            }

            bool _selected = EditorGUILayout.Toggle(_colorGroup.isSelected, GUILayout.Width(15));
            if (_selected != _colorGroup.isSelected)
            {
                _colorGroup.isSelected = _selected;
                if (_selected)
                {
                    if (!isInFusionMode) isInFusionMode = true;
                }
                else if (!colorGroups.Any(g => g.isSelected)) isInFusionMode = false;
            }

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Loads saved groups.
    /// </summary>
    private void LoadGroups()
    {
        Directory.CreateDirectory(Application.dataPath + "/Resources/Editor/");

        string _path = Application.dataPath + "/Resources/Editor/" + EditorSceneManager.GetActiveScene().name + ".txt";

        // If the file doesn't exist, return
        if (!File.Exists(_path)) return;

        string[] _groups = File.ReadAllLines(_path).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        string _groupName = string.Empty;
        string[] _groupColor = new string[] { };

        colorGroups = new TDS_ColorGroup[_groups.Length];

        for (int _i = 0; _i < _groups.Length; _i++)
        {
            _groupName = _groups[_i].Split('|')[0];
            _groupColor = _groups[_i].Split('|')[1].Split('#');

            colorGroups[_i] = new TDS_ColorGroup(new Color(float.Parse(_groupColor[0]), float.Parse(_groupColor[1]), float.Parse(_groupColor[2]), float.Parse(_groupColor[3])), _groupName);
        }

        LoadSprites();
    }

    /// <summary>
    /// Save groups.
    /// </summary>
    private void SaveGroups()
    {
        string _file = string.Empty;
        foreach (TDS_ColorGroup _group in colorGroups)
        {
            _file += $"{_group.Name}|{_group.Color.r}#{_group.Color.g}#{_group.Color.b}#{_group.Color.a}\n";
        }
        Directory.CreateDirectory(Application.dataPath + "/Resources/Editor/");
        File.WriteAllText(Application.dataPath + "/Resources/Editor/" + EditorSceneManager.GetActiveScene().name + ".txt", _file);
    }
    #endregion

    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Load sprites on enable
        LoadSprites();
    }

    // Implement your own editor GUI here
    private void OnGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}

[Serializable]
public class TDS_ColorGroup
{
    /* TDS_ColorGroup :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	Class used to store a color associated with sprites colored by. Used by the TDS_ColorLevelEditor window.
     *
     *	#####################
     *	####### TO DO #######
     *	#####################
     *
     *	...
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *
     *	Date :			[08 / 05 / 2019]
     *	Author :		[Guibert Lucas]
     *
     *	Changes :
     *
     *	Creation of the TDS_ColorGroup class.
     *	
     *	    • Created the whole class with 3 constructors.
     *
     *	-----------------------------------
    */

    #region Fields / Properties
    /// <summary>
    /// Name of this group.
    /// </summary>
    public string Name = "? New Color Group ?";

    /// <summary>
    /// Color associated with the class.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Sprites with the color.
    /// </summary>
    public List<SpriteRenderer> Sprites = new List<SpriteRenderer>();

    /// <summary>
    /// Is this group selected in the window or not.
    /// </summary>
    public bool isSelected = false;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new Color Group.
    /// </summary>
    public TDS_ColorGroup()
    {
        Color = Color.white;
        Sprites = new List<SpriteRenderer>();
    }

    /// <summary>
    /// Creates a new Color Group.
    /// </summary>
    /// <param name="_color">Color of the group.</param>
    public TDS_ColorGroup(Color _color)
    {
        Color = _color;
        Sprites = new List<SpriteRenderer>();
    }

    /// <summary>
    /// Creates a new Color Group.
    /// </summary>
    /// <param name="_color">Color of the group.</param>
    /// <param name="_name">Name of the group.</param>
    public TDS_ColorGroup(Color _color, string _name)
    {
        Color = _color;
        Name = _name;
        Sprites = new List<SpriteRenderer>();
    }

    /// <summary>
    /// Creates a new Color Group.
    /// </summary>
    /// <param name="_color">Color of the group.</param>
    /// <param name="_sprites">Sprites in this group.</param>
    public TDS_ColorGroup(Color _color, IEnumerable<SpriteRenderer> _sprites)
    {
        Color = _color;
        Sprites = _sprites.ToList();
    }
    #endregion
}
