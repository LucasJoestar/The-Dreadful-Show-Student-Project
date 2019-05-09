using System;
using System.Collections;
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
            colorGroups = colorGroups.Append(new TDS_ColorGroup(_sprite.color, new SpriteRenderer[] { _sprite })).ToArray();
        }
        else if (!_matching.Sprites.Contains(_sprite))
        {
            _matching.Sprites.Add(_sprite);
        }
    }
	#endregion

	#region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        LoadSprites();
    }

    // Implement your own editor GUI here
    private void OnGUI()
    {
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

        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("Level Colors", "All colors of the loaded sprites in the level"), EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Load Sprites", "Loads all sprites of this level"))) LoadSprites();
        if (GUILayout.Button(new GUIContent("Reset", "Clear all loaded sprites and get them from zero point")))
        {
            colorGroups = new TDS_ColorGroup[] { };
            LoadSprites();
            Repaint();
        }

        EditorGUILayout.EndHorizontal();

        if (colorGroups.Length == 0)
        {
            GUILayout.Space(5);

            EditorGUILayout.HelpBox(new GUIContent("No sprites loaded !", "Click on the \"Load Sprites\" button to get sprites of the scene, or add some to load them next"));

            EditorGUILayout.EndScrollView();
            return;
        }

        foreach (TDS_ColorGroup _colorGroup in colorGroups)
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

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

            if (isInFusionMode && !_colorGroup.isSelected)
            {
                Color _original = GUI.color;
                GUI.color = new Color(0, .75f, 0, 1);

                if (GUILayout.Button("F", GUILayout.Width(25)))
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
            else
            {
                GUILayout.Space(25);
            }

            EditorGUILayout.EndHorizontal();
        }

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