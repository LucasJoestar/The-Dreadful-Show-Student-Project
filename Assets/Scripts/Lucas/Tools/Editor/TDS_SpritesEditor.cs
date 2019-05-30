using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TDS_SpritesEditor : EditorWindow
{
    /* TDS_ColorLevelEditor :
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
	 *	[TO DO]
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
    /// All editor modes displayed in the toolbar.
    /// </summary>
    [SerializeField] private readonly string[] editorModes = new string[] { "General", "Color" };

    /// <summary>
    /// Index of the selected mode from <see cref="editorModes"/>.
    /// </summary>
    [SerializeField] private int selectedMode = 0;

    /// <summary>
    /// Indicates if editor manipulation should also take selected objects children sprites.
    /// </summary>
    [SerializeField] private bool doUseChildrenSprites = true;

    /// <summary>
    /// Value used to increment sprites layer.
    /// </summary>
    [SerializeField] private int layerIncrementValue = 0;

    /// <summary>
    /// Main folder containing all other folders & groups.
    /// </summary>
    [SerializeField] private TDS_ColorGroupFolder mainFolder = new TDS_ColorGroupFolder();

    /// <summary>
    /// All color groups from all folders.
    /// </summary>
    [SerializeField] private List<TDS_ColorGroup> allGroups = new List<TDS_ColorGroup>();

    /// <summary>
    /// All color group folders.
    /// </summary>
    [SerializeField] private List<TDS_ColorGroupFolder> allFolders = new List<TDS_ColorGroupFolder>();

    /// <summary>
    /// Scrollbar of the window.
    /// </summary>
    [SerializeField] private Vector2 scrollbar = new Vector2();

    /// <summary>
    /// Indicates if the user has selected one group or more for fusion.
    /// </summary>
    [SerializeField] private bool isInFusionMode = false;

    /// <summary>
    /// Selected color groups.
    /// </summary>
    [SerializeField] private Dictionary<TDS_ColorGroupFolder, List<TDS_ColorGroup>> selectedGroups = new Dictionary<TDS_ColorGroupFolder, List<TDS_ColorGroup>>();
    #endregion

    #region Methods

    #region Original Methods

    #region Menus
    /// <summary>
    /// Method to call this window from the Unity toolbar.
    /// </summary>
    [MenuItem("Window/Sprites Editor")]
    public static void CallWindow() => GetWindow<TDS_SpritesEditor>("Color Level Editor").Show();

    /// <summary>
    /// Draws this editor.
    /// </summary>
    public void DrawEditor()
    {
        selectedMode = GUILayout.Toolbar(selectedMode, editorModes, GUILayout.Height(30));

        switch (selectedMode)
        {
            case 0:
                DrawGeneralMode();
                break;

            case 1:
                DrawColorMode();
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
        if (Selection.gameObjects.Length == 0)
        {
            EditorGUILayout.HelpBox("No sprite actually selected !", MessageType.Info);
            return;
        }

        GUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);

        doUseChildrenSprites = EditorGUILayout.Toggle(new GUIContent("Use children sprites", "Should the manipulations automatically also select the selected GameObjects sprites children or not"), doUseChildrenSprites);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Increment Layer", "Increment the selected GameObjects layer by the above number"), GUILayout.Height(25), GUILayout.Width(200)))
        {
            SpriteRenderer[] _selected = null;
            if (doUseChildrenSprites)
            {
                _selected = Selection.gameObjects.ToList().SelectMany(s => s.GetComponentsInChildren<SpriteRenderer>()).ToArray();
            }
            else
            {
                _selected = Selection.gameObjects.ToList().Select(g => g.GetComponent<SpriteRenderer>()).Distinct().ToArray();
            }

            Undo.RecordObjects(_selected, "Increment Sprites layer order");

            foreach (SpriteRenderer _sprite in _selected)
            {
                _sprite.sortingOrder += layerIncrementValue;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
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
        for (int _i = 0; _i < allGroups.Count; _i++)
        {
            allGroups[_i].Sprites = allGroups[_i].Sprites.Where(s => s != null).ToList();

            SpriteRenderer[] _differents = allGroups[_i].Sprites.Where(s => s.color != allGroups[_i].Color).ToArray();

            if (_differents.Length > 0)
            {
                if (_differents.Length == allGroups[_i].Sprites.Count)
                {
                    allGroups[_i].Color = _differents[0].color;
                }

                else
                {
                    foreach (SpriteRenderer _sprite in _differents)
                    {
                        allGroups[_i].Sprites.Remove(_sprite);
                        LoadSprite(_sprite);
                    }

                    if (allGroups[_i].Sprites.Count == 0)
                    {
                        allGroups[_i].Name = "REMOVE ME";
                        Repaint();
                    }
                }
            }
        }

        // Draw toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button(new GUIContent("Create Folder", "Create a new folder to order your color groups"), EditorStyles.toolbarButton))
        {
            mainFolder.Folders = mainFolder.Folders.Append(new TDS_ColorGroupFolder()).ToList();
            allFolders.Add(mainFolder.Folders.Last());
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Range", "Range the groups by name"), EditorStyles.toolbarButton))
        {
            mainFolder.Folders = mainFolder.Folders.OrderBy(g => g.Name).ToList();
            foreach (TDS_ColorGroupFolder _folder in allFolders)
            {
                _folder.ColorGroups = _folder.ColorGroups.OrderBy(g => g.Name).ToList();
            }
        }

        if (GUILayout.Button(new GUIContent("Load Sprites", "Loads all sprites of this level"), EditorStyles.toolbarButton)) LoadSprites();

        GUILayout.Space(15);

        if (GUILayout.Button(new GUIContent("Reset", "Clear all loaded sprites and get them from zero point"), EditorStyles.toolbarButton))
        {
            if (EditorUtility.DisplayDialog("Confirm color groups reset", "Are you sure you want to reset all your color groups ? This action cannot be undone.", "Yes, I'm sure !", "I've changed my mind..."))
            {
                mainFolder = new TDS_ColorGroupFolder();
                allGroups = new List<TDS_ColorGroup>();
                allFolders = new List<TDS_ColorGroupFolder>();

                LoadSprites();
                Repaint();
            }
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        // Draw color groups editor
        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);

        EditorGUILayout.LabelField(new GUIContent("Level Colors", "All colors of the loaded sprites in the level"), EditorStyles.boldLabel);

        if ((mainFolder.ColorGroups.Count == 0) && (mainFolder.Folders.Count == 0))
        {
            GUILayout.Space(5);

            EditorGUILayout.HelpBox(new GUIContent("No sprites loaded !", "Click on the \"Load Sprites\" button to get sprites of the scene, or add some to load them next"));

            EditorGUILayout.EndScrollView();
            return;
        }

        // Draw all folders & color groups !!
        DrawColorGroupFolders(mainFolder.Folders, mainFolder);
        GUILayout.Space(10);
        DrawColorGroups(mainFolder.ColorGroups, mainFolder);

        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }
    #endregion

    #region General

    #endregion

    #region Color
    /// <summary>
    /// Loads all sprite renderers of the level.
    /// </summary>
    public void LoadSprites()
    {
        SpriteRenderer[] _sprites = FindObjectsOfType<SpriteRenderer>();

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
        TDS_ColorGroup _matching = allGroups.Where(c => c.Color == _sprite.color).FirstOrDefault();
        if (_matching == null)
        {
            mainFolder.ColorGroups.Add(new TDS_ColorGroup(_sprite.color, new SpriteRenderer[] { _sprite }));
            allGroups.Add(mainFolder.ColorGroups.Last());

            mainFolder.ColorGroups = mainFolder.ColorGroups.OrderBy(g => g.Name).ToList();
        }
        else if (!_matching.Sprites.Contains(_sprite))
        {
            _matching.Sprites.Add(_sprite);
        }
    }


    /// <summary>
    /// Draw some color group folders.
    /// </summary>
    /// <param name="_colorGroupFolders">Folders tpo draw.</param>
    private void DrawColorGroupFolders(IEnumerable<TDS_ColorGroupFolder> _colorGroupFolders, TDS_ColorGroupFolder _previousFolder)
    {
        foreach (TDS_ColorGroupFolder _folder in _colorGroupFolders)
        {
            // Draw the folder
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            // Draw fusion mode buttons
            if (isInFusionMode && !_folder.isSelected)
            {
                Color _original = GUI.color;
                GUI.color = new Color(.5f, .25f, 0, 1);

                if (GUILayout.Button("S", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    foreach (KeyValuePair<TDS_ColorGroupFolder, List<TDS_ColorGroup>> _group in selectedGroups)
                    {
                        Undo.RecordObjects(_group.Value.SelectMany(t => t.Sprites).ToArray(), "fusion sprite groups color");

                        // Fusion To folder
                        _group.Value.ForEach(g => g.isSelected = false);
                        _folder.ColorGroups.AddRange(_group.Value);

                        // Remove fusionned sprites
                        _group.Key.ColorGroups = _group.Key.ColorGroups.Except(_group.Value).ToList();
                        allGroups = allGroups.Except(_group.Value).ToList();

                        allGroups.AddRange(_folder.ColorGroups.Where(g => !allGroups.Contains(g)));

                        _group.Key.isSelected = false;
                    }

                    isInFusionMode = false;
                    Repaint();
                    return;
                }

                GUI.color = _original;
            }
            else GUILayout.Space(28);

            // Draw folder
            _folder.isUnfolded = EditorGUILayout.Foldout(_folder.isUnfolded, _folder.Name, true);
            GUILayout.FlexibleSpace();

            Color _originalColor = GUI.color;
            GUI.color = Color.red;

            if (GUILayout.Button("X", GUILayout.Width(20)) && EditorUtility.DisplayDialog("Confirm folder suppression", "Are you sure you want to delete this folder ?\nThis action cannot be undone", "Yep", "Cancel"))
            {
                _previousFolder.ColorGroups = _previousFolder.ColorGroups.Concat(_folder.ColorGroups).ToList();
                _previousFolder.Folders.Remove(_folder);

                Repaint();
            }

            GUI.color = new Color(.95f, .7f, .0f);

            if (GUILayout.Button("Edit Name")) _folder.isEditingName = !_folder.isEditingName;

            GUI.color = _originalColor;

            if (_folder.isEditingName)
            {
                _folder.Name = EditorGUILayout.TextField(_folder.Name);
            }

            GUILayout.Space(20);

            bool _selected = EditorGUILayout.Toggle(_folder.isSelected, GUILayout.Width(15));
            if (_selected != _folder.isSelected)
            {
                _folder.isSelected = _selected;
                if (_selected)
                {
                    if (!isInFusionMode) isInFusionMode = true;
                    if (selectedGroups.ContainsKey(_folder)) selectedGroups[_folder] = _folder.ColorGroups;
                    else selectedGroups.Add(_folder, _folder.ColorGroups);
                }
                else
                {
                    selectedGroups.Remove(_folder);
                    if (selectedGroups.Count == 0) isInFusionMode = false;
                }
            }

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();

            // Draw this folder color groups
            if (_folder.isUnfolded)
            {
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(38);
                EditorGUILayout.BeginVertical();

                DrawColorGroupFolders(_folder.Folders, _folder);

                if (_folder.ColorGroups.Count > 0)
                {
                    DrawColorGroups(_folder.ColorGroups, _folder);
                }
                else
                {
                    EditorGUILayout.HelpBox("There is no color group in this folder.", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// Draw some color groups.
    /// </summary>
    /// <param name="_colorGroups">Groups to draw.</param>
    /// <param name="_folder">Folder containing the group.</param>
    private void DrawColorGroups(IEnumerable<TDS_ColorGroup> _colorGroups, TDS_ColorGroupFolder _folder)
    {
        foreach (TDS_ColorGroup _colorGroup in _colorGroups)
        {
            GUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();

            // Draw fusion mode buttons
            if (isInFusionMode && !_colorGroup.isSelected)
            {
                Color _original = GUI.color;
                GUI.color = new Color(0, .75f, 0, 1);

                if (GUILayout.Button("F", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    foreach (KeyValuePair<TDS_ColorGroupFolder, List<TDS_ColorGroup>> _group in selectedGroups)
                    {
                        Undo.RecordObjects(_group.Value.SelectMany(t => t.Sprites).ToArray(), "fusion sprite groups color");

                        // Fusion To folder
                        _group.Value.ForEach(g => g.isSelected = false);
                        _folder.ColorGroups.AddRange(_group.Value);

                        foreach (SpriteRenderer _sprite in _group.Value.SelectMany(g => g.Sprites))
                        {
                            _sprite.color = _colorGroup.Color;
                            _colorGroup.Sprites.Add(_sprite);
                        }

                        // Remove fusionned sprites
                        _group.Key.ColorGroups = _group.Key.ColorGroups.Except(_group.Value).ToList();
                        allGroups = allGroups.Except(_group.Value).ToList();

                        allGroups.AddRange(_folder.ColorGroups.Where(g => !allGroups.Contains(g)));

                        _group.Key.isSelected = false;
                    }

                    isInFusionMode = false;
                    Repaint();
                    return;
                }

                GUI.color = _original;
            }
            else GUILayout.Space(28);

            // Draw color group
            _colorGroup.Name = EditorGUILayout.TextField(_colorGroup.Name);

            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            _colorGroup.Color = EditorGUILayout.ColorField(_colorGroup.Color);
            if (EditorGUI.EndChangeCheck())
            {
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
                    if (selectedGroups.ContainsKey(_folder)) selectedGroups[_folder].Add(_colorGroup);
                    else selectedGroups.Add(_folder, new List<TDS_ColorGroup>() { _colorGroup });
                }
                else
                {
                    selectedGroups[_folder].Remove(_colorGroup);
                    if (selectedGroups[_folder].Count == 0) selectedGroups.Remove(_folder);
                    if (selectedGroups.Count == 0) isInFusionMode = false;
                }
            }

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }
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
    public string Name = "New Color Group";

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
    /// <param name="_sprites">Sprites in this group.</param>
    public TDS_ColorGroup(Color _color, IEnumerable<SpriteRenderer> _sprites)
    {
        Color = _color;
        Sprites = _sprites.ToList();
    }
    #endregion
}

[Serializable]
public class TDS_ColorGroupFolder
{
    /* TDS_ColorGroupFolder :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	Class used to store multiple color groups or folder like this.
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
     *	Date :			[13 / 05 / 2019]
     *	Author :		[Guibert Lucas]
     *
     *	Changes :
     *
     *	Creation of the TDS_ColorGroupFolder class.
     *
     *	-----------------------------------
    */

    #region Fields / Properties
    /// <summary>
    /// Name of this group.
    /// </summary>
    public string Name = "New Folder";

    /// <summary>
    /// Indicates if currently editing name.
    /// </summary>
    [NonSerialized] public bool isEditingName = false;

    /// <summary>
    /// Indicates if this folder is selected or not.
    /// </summary>
    public bool isSelected = false;

    /// <summary>
    /// Indicates if this folder is unfolded or not.
    /// </summary>
    public bool isUnfolded = true;

    /// <summary>
    /// Sprites with the color.
    /// </summary>
    public List<TDS_ColorGroupFolder> Folders = new List<TDS_ColorGroupFolder>();

    /// <summary>
    /// Sprites with the color.
    /// </summary>
    public List<TDS_ColorGroup> ColorGroups = new List<TDS_ColorGroup>();
    #endregion
}
