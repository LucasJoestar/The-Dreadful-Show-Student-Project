using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TDS_ColorLevelEditor : EditorWindow 
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
    /// Created folders for color groups.
    /// </summary>
    [SerializeField] private TDS_ColorGroupFolder[] colorGroupFolders = new TDS_ColorGroupFolder[] { };

    /// <summary>
    /// Groups of loaded sprite renderers sorted by color.
    /// </summary>
    [SerializeField] private TDS_ColorGroup[] colorGroups = new TDS_ColorGroup[] { };

    /// <summary>
    /// Scrollbar of the window.
    /// </summary>
    [SerializeField] private Vector2 scrollbar = new Vector2();

    /// <summary>
    /// Indicates if the user has selected one group or more for fusion.
    /// </summary>
    [SerializeField] private bool isInFusionMode = false;
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Method to call this window from the Unity toolbar.
    /// </summary>
    [MenuItem("Window/Color Level Editor")]
    public static void CallWindow() => GetWindow<TDS_ColorLevelEditor>("Color Level Editor").Show();

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
                    TDS_ColorGroup[] _groups = colorGroups.Where(g => g.isSelected).ToArray();

                    foreach (TDS_ColorGroup _group in _groups)
                    {
                        Undo.RecordObjects(_group.Sprites.ToArray(), "fusion sprite groups color");

                        // Fusion To folder
                    }

                    colorGroups = colorGroups.Except(_groups).ToArray();
                    isInFusionMode = false;

                    Repaint();
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
                if (_previousFolder == null)
                {
                    colorGroups = colorGroups.Concat(_folder.ColorGroups).ToArray();
                    colorGroupFolders = colorGroupFolders.Where(c => c != _folder).ToArray();
                }
                else
                {
                    _previousFolder.ColorGroups = _previousFolder.ColorGroups.Concat(_folder.ColorGroups).ToList();
                    _previousFolder.Folders.Remove(_folder);
                }

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
                }
                else if (!colorGroups.Any(g => g.isSelected)) isInFusionMode = false;

                // Stock selected objects
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
                    DrawColorGroups(_folder.ColorGroups);
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
    private void DrawColorGroups(IEnumerable<TDS_ColorGroup> _colorGroups)
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
                }
                else if (!colorGroups.Any(g => g.isSelected)) isInFusionMode = false;
            }

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }
    }
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
        // Get non-assignated color groups and associate them with matching one if found
        for (int _i = 0; _i < colorGroups.Length; _i++)
        {
            colorGroups[_i].Sprites = colorGroups[_i].Sprites.Where(s => s != null).ToList();

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

                    if (colorGroups[_i].Sprites.Count == 0)
                    {
                        colorGroups = colorGroups.Where(g => g != colorGroups[_i]).ToArray();
                        Repaint();
                    }
                }
            }
        }

        // Draw toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button(new GUIContent("Create Folder", "Create a new folder to order your color groups"), EditorStyles.toolbarButton)) colorGroupFolders = colorGroupFolders.Append(new TDS_ColorGroupFolder()).ToArray();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Range", "Range the groups by name"), EditorStyles.toolbarButton))
        {
            colorGroupFolders = colorGroupFolders.OrderBy(g => g.Name).ToArray();
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

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        // Draw color groups editor
        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);

        EditorGUILayout.LabelField(new GUIContent("Level Colors", "All colors of the loaded sprites in the level"), EditorStyles.boldLabel);

        if (colorGroups.Length == 0)
        {
            GUILayout.Space(5);

            EditorGUILayout.HelpBox(new GUIContent("No sprites loaded !", "Click on the \"Load Sprites\" button to get sprites of the scene, or add some to load them next"));

            EditorGUILayout.EndScrollView();
            return;
        }

        // Draw all folders & color groups !!
        DrawColorGroupFolders(colorGroupFolders, null);
        GUILayout.Space(10);
        DrawColorGroups(colorGroups);

        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
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
    public List<SpriteRenderer> Sprites;

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
    public bool isSelected = true;

    /// <summary>
    /// Indicates if this folder is unfolded or not.
    /// </summary>
    public bool isUnfolded = true;

    /// <summary>
    /// Sprites with the color.
    /// </summary>
    public List<TDS_ColorGroupFolder> Folders;

    /// <summary>
    /// Sprites with the color.
    /// </summary>
    public List<TDS_ColorGroup> ColorGroups;
    #endregion
}
