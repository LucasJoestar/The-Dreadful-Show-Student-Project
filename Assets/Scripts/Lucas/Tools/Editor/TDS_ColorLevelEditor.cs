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
	 *	Date :			[08 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_ColorLevelEditor class.
     *	
     *	    • 
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Groups of loaded sprite renderers sorted by color.
    /// </summary>
    [SerializeField] private TDS_ColorGroup[] colorGroups = new TDS_ColorGroup[] { };

    [SerializeField] private Vector2 scrollbar = new Vector2();
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

            SpriteRenderer _newSprite = EditorGUILayout.ObjectField(null, typeof(SpriteRenderer), true, GUILayout.Width(50)) as SpriteRenderer;
            if (_newSprite != null)
            {
                TDS_ColorGroup _matching = colorGroups.Where(g => g.Sprites.Contains(_newSprite)).FirstOrDefault();
                if (_matching != null)
                {
                    if (_matching.Color != _newSprite.color)
                    {
                        _matching.Sprites.Remove(_newSprite);
                        if (_matching.Sprites.Count == 0)
                        {
                            colorGroups = colorGroups.Where(g => g != _matching).ToArray();
                            Repaint();
                        }
                    }
                }

                LoadSprite(_newSprite);
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
     *	    • 
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