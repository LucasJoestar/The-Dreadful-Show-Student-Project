using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Editor class containing multiple static methods around the multi-tags system.
/// </summary>
public static class MultiTagsUtility
{
    /* MultiTagsUtility :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Script to stock multiple utilities static members around the multi-tags system in one script.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     * 
     *      • Add a little arrow next to the drawn tags to enable / disable
     *  multi lines for drawing tags. In other words, to fold / unfold tags.
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[24 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Created a context menu on tag right click, allowing to change its color
     *	with the ColorPicker class window.
     *	
     *	Source for this : https://github.com/Unity-Technologies/UnityCsReference/blob/2018.3/Editor/Mono/GUI/ColorPicker.cs
     *	
     *	    • Modified the Tag field to display the list of available tags to add in a popup.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[05 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Created methods to draw tags in the editor, with or without a button
     *	left to each of them.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[22 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Moved the Char variable used to separate tags to the newly created MultiTags class.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[03 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    We can now create and remove tags from the project with the
     *	AddTag & RemoveTag methods.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[30 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the MultiTagsUtility class.
     *	
     *	    • Added a Char variable used to separate tags witht he multi-tags system
     *	from the Unity one.
     *	
     *	    • Creation of methods to get all this project tags using the Unity system
     *	or the multi one, witht he GetTags & GetUnityTags methods.
	 *
	 *	-----------------------------------
	*/

    #region Methods
    /// <summary>
    /// Adds a tag to this Unity project.
    /// </summary>
    /// <param name="_tag">New tag to add.</param>
    public static void AddTag(string _tag) => InternalEditorUtility.AddTag(_tag);

    /// <summary>
    /// Removes a tag from this Unity project.
    /// </summary>
    /// <param name="_tag">Tag to remove.</param>
    public static void RemoveTag(string _tag) => InternalEditorUtility.RemoveTag(_tag);


    /// <summary>
    /// Get the scriptable object containing all this project tags.
    /// </summary>
    public static TagsSO GetTagsAsset()
    {
        TagsSO _tagsSO = Resources.Load<TagsSO>(MultiTags.TAGS_SO_PATH);
        if (_tagsSO) return _tagsSO;
        return CreateTagsAsset();
    }

    /// <summary>
    /// Creates the tags scriptable object used to save & load project tags.
    /// </summary>
    public static TagsSO CreateTagsAsset()
    {
        // Creates the scriptable object to write
        TagsSO _reference = ScriptableObject.CreateInstance<TagsSO>();

        // Creates the default folders & write the asset on disk
        Directory.CreateDirectory(Application.dataPath + "/Resources/" + Path.GetDirectoryName(MultiTags.TAGS_SO_PATH));
        AssetDatabase.CreateAsset(_reference, "Assets/Resources/" + MultiTags.TAGS_SO_PATH + ".asset");
        AssetDatabase.SaveAssets();

        return _reference;
    }


    /// <summary>
    /// Get all this project tags, using the multi-tags system.
    /// </summary>
    /// <returns>Returns a string array of all tags, using the multi-tags system, of this proejct.</returns>
    public static string[] GetTags() { return GetUnityTags().SelectMany(t => t.Split(MultiTags.TagSeparator)).Distinct().ToArray(); }

    /// <summary>
    /// Get all tags of a game object.
    /// </summary>
    /// <param name="_gameObject">Game object to get tags from.</param>
    /// <returns>Returns an array of all tags from this game object.</returns>
    public static Tag[] GetTags(GameObject _gameObject)
    {
        // Get scriptable object containing all project tags
        TagsSO _source = GetTagsAsset();

        // Get game object tags
        List<string> _gameObjectTags = _gameObject.GetTags().ToList();
        List<Tag> _tags = new List<Tag>();

        // Get tags from custom tags
        foreach (Tag _tag in _source.Tags)
        {
            if (_gameObjectTags.Contains(_tag.Name))
            {
                _tags.Add(_tag);
                _gameObjectTags.Remove(_tag.Name);

                if (_gameObjectTags.Count == 0) break;
            }
        }

        // Get tags from Unity built-in tags
        if (_gameObjectTags.Count > 0)
        {
            foreach (Tag _tag in _source.UnityBuiltInTags)
            {
                if (_gameObjectTags.Contains(_tag.Name))
                {
                    _tags.Add(_tag);
                    _gameObjectTags.Remove(_tag.Name);

                    if (_gameObjectTags.Count == 0) break;
                }
            }
        }

        // Return object tags
        return _tags.ToArray();
    }

    /// <summary>
    /// Get a Tag object from an given tag name.
    /// </summary>
    /// <param name="_tagName">Tag name to get Tag object from.</param>
    /// <returns>Returns the Tag object from this tag name.</returns>
    public static Tag GetTag(string _tagName)
    {
        // Get scriptable object containing all project tags
        TagsSO _source = GetTagsAsset();

        // Try to get corresponding tag from custom tags
        foreach (Tag _tag in _source.Tags)
        {
            if (_tagName == _tag.Name) return _tag;
        }

        // Try to get corresponding tag from built-in tags
        foreach (Tag _tag in _source.UnityBuiltInTags)
        {
            if (_tagName == _tag.Name) return _tag;
        }

        // Return given tag does not exist, return null
        return null;
    }

    /// <summary>
    /// Get all Tag objects from an array of tag names.
    /// </summary>
    /// <param name="_tagNames">Tag names to get Tag objects from.</param>
    /// <returns>Returns an array of all Tag objects from these tag names.</returns>
    public static Tag[] GetTags(string[] _tagNames)
    {
        // Get scriptable object containing all project tags
        TagsSO _source = GetTagsAsset();

        List<Tag> _tags = new List<Tag>();
        List<string> _tagsNamesList = _tagNames.ToList();

        // Get tags from custom tags
        foreach (Tag _tag in _source.Tags)
        {
            if (_tagNames.Contains(_tag.Name))
            {
                _tags.Add(_tag);
                _tagsNamesList.Remove(_tag.Name);

                if (_tagsNamesList.Count == 0) break;
            }
        }

        // Get tags from Unity built-in tags
        if (_tagsNamesList.Count > 0)
        {
            foreach (Tag _tag in _source.UnityBuiltInTags)
            {
                if (_tagNames.Contains(_tag.Name))
                {
                    _tags.Add(_tag);
                    _tagsNamesList.Remove(_tag.Name);

                    if (_tagsNamesList.Count == 0) break;
                }
            }
        }

        // Return object tags
        return _tags.ToArray();
    }

    /// <summary>
    /// Get all this project Unity tags.
    /// </summary>
    /// <returns>Returns a string array of all Unity tags from this project.</returns>
    public static string[] GetUnityTags() { return InternalEditorUtility.tags; }


    /// <summary>
    /// Call this method to draw tags in an editor.
    /// </summary>
    /// <param name="_tags">Tags to draw.</param>
    public static void DrawTags(Tag[] _tags)
    {
        // Get current event
        Event _event = Event.current;

        // Get CN Counter Badge style
        GUIStyle _cnCountBadge = new GUIStyle("CN CountBadge");
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        Vector2 _cnCounterBadgeCaclSize = _cnCountBadge.CalcSize(GUIContent.none);

        // Get original GUI colors
        Color _guiOriginalColor = GUI.color;
        Color _boldLabelOriginalColor = _boldLabel.normal.textColor;

        EditorGUILayout.BeginHorizontal();

        // Get rect from the actual position in layout
        Rect _lastRect = GUILayoutUtility.GetRect(new GUIContent(""), _boldLabel);
        _lastRect.width = 0;
        _lastRect.height = 0;
        _lastRect.x = EditorStyles.inspectorDefaultMargins.padding.left;
        float _xStartPos = _lastRect.x;
        float _yTotalHeight = _lastRect.height;

        // Draws tags
        foreach (Tag _tag in _tags)
        {
            GUI.color = _tag.Color;
            if (GUI.color.grayscale < .5f) _boldLabel.normal.textColor = Color.white;
            else _boldLabel.normal.textColor = Color.black;

            Rect _badgeRect = new Rect(_lastRect.position, new Vector2(_boldLabel.CalcSize(new GUIContent(_tag.Name)).x + 7, _cnCounterBadgeCaclSize.y));
            _badgeRect.position = new Vector2(_lastRect.xMax + (_lastRect.xMax > _xStartPos ? 5 : 0), _lastRect.y);

            if (_badgeRect.xMax > (Screen.width - 10) && _badgeRect.xMin > 15)
            {
                _badgeRect.x = _xStartPos;
                _badgeRect.y += _badgeRect.height + 7;
                _yTotalHeight += _badgeRect.height + 7;
            }

            // Update last rect
            _lastRect = _badgeRect;

            // Creates the box for the whole tag area
            GUI.Box(new Rect(_badgeRect.x, _badgeRect.y - 1, _badgeRect.width + 2, _badgeRect.height), string.Empty, _cnCountBadge);

            // Set GUI color as white to display tag name properly
            GUI.color = Color.white;

            // Label to display the tag name
            GUI.Label(new Rect(_badgeRect.x + 4, _badgeRect.y, _badgeRect.width, _badgeRect.height), _tag.Name, _boldLabel);

            // Create menu context on right click
            if ((_event.type == EventType.ContextClick) && _badgeRect.Contains(_event.mousePosition))
            {
                GenericMenu _menu = new GenericMenu();
                _menu.AddItem(new GUIContent("Change Color", "Change the color of this Tag"), false, ColorPickerMenu, _tag);

                _menu.ShowAsContext();
            }
        }

        EditorGUILayout.EndHorizontal();

        // Update layout position
        GUILayoutUtility.GetRect(0, _yTotalHeight);

        // Set back original GUI colors
        GUI.color = _guiOriginalColor;
        _boldLabel.normal.textColor = _boldLabelOriginalColor;
    }

    /// <summary>
    /// Call this method to draw tags in an editor, with a little button left to each of them.
    /// </summary>
    /// <param name="_tags">Tags to draw.</param>
    /// <param name="_callback">Method to call back on tag left button click.</param>
    /// <returns>Returns true if the user clicked on a tag left button, false otherwise.</returns>
    public static bool DrawTags(Tag[] _tags, Action<Tag> _callback)
    {
        // Get current event
        Event _event = Event.current;

        // Get CN Counter Badge & Ol Minus styles
        GUIStyle _cnCountBadge = new GUIStyle("CN CountBadge");
        GUIStyle _olMinus = new GUIStyle("OL Minus");
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        Vector2 _olMinusCaclSize = _olMinus.CalcSize(GUIContent.none);
        Vector2 _cnCounterBadgeCaclSize = _cnCountBadge.CalcSize(GUIContent.none);

        // Get original GUI colors
        Color _guiOriginalColor = GUI.color;
        Color _boldLabelOriginalColor = _boldLabel.normal.textColor;

        EditorGUILayout.BeginHorizontal();

        // Get rect from the actual position in layout
        Rect _lastRect = GUILayoutUtility.GetRect(new GUIContent(""), _boldLabel);
        _lastRect.width = 0;
        _lastRect.height = 0;
        _lastRect.x = EditorStyles.inspectorDefaultMargins.padding.left;
        float _xStartPos = _lastRect.x;
        float _yTotalHeight = _lastRect.height;

        // Boolean indicating if user clicked on a button
        bool _hasClick = false;

        // Draws tags
        foreach (Tag _tag in _tags)
        {
            GUI.color = _tag.Color;
            if (GUI.color.grayscale < .5f) _boldLabel.normal.textColor = Color.white;
            else _boldLabel.normal.textColor = Color.black;

            Rect _labelRect = new Rect(_lastRect.position, new Vector2(_boldLabel.CalcSize(new GUIContent(_tag.Name)).x + 3, _cnCounterBadgeCaclSize.y));
            _labelRect.position = new Vector2(_lastRect.xMax + (_lastRect.xMax > _xStartPos ? 5 : 0), _lastRect.y);

            if ((_labelRect.xMax + _olMinusCaclSize.x) > (Screen.width - 10) && _labelRect.xMin > 15)
            {
                _labelRect.x = _xStartPos;
                _labelRect.y += _labelRect.height + 7;
                _yTotalHeight += _labelRect.height + 7;
            }

            Rect _badgeRect = new Rect(_labelRect.position, _labelRect.size + new Vector2(_olMinusCaclSize.x, 0));

            // Update last rect
            _lastRect = _badgeRect;

            // Creates the box for the whole tag area
            GUI.Box(new Rect(_badgeRect.x, _badgeRect.y - 1, _badgeRect.width, _badgeRect.height), string.Empty, _cnCountBadge);

            // Draw button left to the tag
            if (GUI.Button(new Rect(_badgeRect.position, _olMinusCaclSize), GUIContent.none, _olMinus) && (_event.button == 0))
            {
                _callback.Invoke(_tag);
                _hasClick = true;
            }

            // Set GUI color as white to display tag name properly
            GUI.color = Color.white;

            // Label to display the tag name
            GUI.Label(new Rect(_labelRect.position + new Vector2(_olMinusCaclSize.x - 2, 0), _labelRect.size), _tag.Name, _boldLabel);

            GUILayout.Space(_olMinusCaclSize.x);

            // Create menu context on right click
            if ((_event.type == EventType.ContextClick) && _badgeRect.Contains(_event.mousePosition))
            {
                GenericMenu _menu = new GenericMenu();
                _menu.AddItem(new GUIContent("Change Color", "Change the color of this Tag"), false, ColorPickerMenu, _tag);

                _menu.ShowAsContext();
            }
        }

        EditorGUILayout.EndHorizontal();

        // Update layout position
        GUILayoutUtility.GetRect(0, _yTotalHeight);

        // Set back original GUI colors
        GUI.color = _guiOriginalColor;
        _boldLabel.normal.textColor = _boldLabelOriginalColor;

        return _hasClick;
    }

    /// <summary>
    /// Draws a tag field in the editor.
    /// </summary>
    /// <param name="_text">Index of the tag to display in the tag field.</param>
    /// <param name="_notDisplayedTags">Tags to not display in the list of tags that can be added.</param>
    /// <param name="_callback">Method to call back when a tag is selected.</param>
    /// <returns>Returns new string value entered by the user in the field.</returns>
    public static int TagField(int _index, Tag[] _notDisplayedTags, Action<string> _callback)
    {
        // Get all tags that can be added
        string[] _allTags = GetTagsAsset().UnityBuiltInTags.Select(t => t.Name).Concat(GetTagsAsset().Tags.Select(t => t.Name)).Except(_notDisplayedTags.Select(t => t.Name)).ToArray();
        _allTags[0] = "Select new Tag";

        // Set index as last element index if exceeding array count
        if (_index >= _allTags.Length) _index = _allTags.Length - 1;

        // Get selected tag name
        string _text = _allTags[_index];

        // Get CN Counter Badge & Ol Plus styles
        GUIStyle _cnCountBadge = new GUIStyle("CN CountBadge");
        GUIStyle _olPlus = new GUIStyle("OL Plus");

        Vector2 _olMinusCaclSize = _olPlus.CalcSize(GUIContent.none);
        Vector2 _cnCounterBadgeCaclSize = _cnCountBadge.CalcSize(GUIContent.none);

        // Get rect from the actual position in layout
        Rect _fieldRect = GUILayoutUtility.GetRect(new GUIContent(""), EditorStyles.boldLabel);

        // Get the x size of the text area, with a minmum value
        float _xTextSize = EditorStyles.boldLabel.CalcSize(new GUIContent(_text)).x + 5;
        if (_xTextSize < 50) _xTextSize = 50;

        Rect _textRect = new Rect(new Vector2(_fieldRect.position.x + 6, _fieldRect.position.y + 1), new Vector2(_xTextSize, _cnCounterBadgeCaclSize.y));
        Rect _badgeRect = new Rect(_fieldRect.position, _textRect.size + new Vector2(_olMinusCaclSize.x + 8, 0));

        // Get if the given text does match to an existing tag name
        // If so, draw the field in green color, or in red in no tag with such name is found
        Tag _tag = GetTag(_text);

        Color _originalColor = GUI.color;
        //GUI.color = (_text == _allTags[0]) ? Color.white : Color.green;

        // Creates the box for the whole tag area
        GUI.Box(new Rect(_badgeRect.x, _badgeRect.y - 1, _badgeRect.width, _badgeRect.height), string.Empty, _cnCountBadge);

        // Set GUI original color back
        GUI.color = _originalColor;

        // Text field to select a tag
        _index = EditorGUI.Popup(new Rect(_textRect.position, _textRect.size), _index, _allTags, EditorStyles.boldLabel);

        // Draw button right to the tag field to select it
        if (GUI.Button(new Rect(new Vector2(_badgeRect.position.x + _textRect.size.x + 6, _badgeRect.position.y), _olMinusCaclSize), GUIContent.none, _olPlus) && (_tag != null))
        {
            _callback.Invoke(_text);
            _text = string.Empty;
        }

        return _index;
    }


    /// <summary>
    /// Removes a tag from all game objects in a given array.
    /// </summary>
    /// <param name="_tagName">Name of the tag to remove.</param>
    /// <param name="_gameObjects">Array of game objects to remove tag from.</param>
    public static void RemoveTagFromGameObjects(string _tagName, GameObject[] _gameObjects)
    {
        // Get object having specified tag
        _gameObjects = _gameObjects.Where(g => g.GetTags().Contains(_tagName)).ToArray();

        // Remove tag from all objects
        foreach (GameObject _object in _gameObjects)
        {
            string _newTag = string.Empty;

            if (_object.tag != _tagName)
            {
                if (_object.tag.Contains(MultiTags.TagSeparator + _tagName + MultiTags.TagSeparator))
                {
                    _newTag = _object.tag.Replace(MultiTags.TagSeparator + _tagName + MultiTags.TagSeparator, MultiTags.TagSeparator.ToString());
                }
                else if (_object.tag.Substring(0, _tagName.Length + 1) == _tagName + MultiTags.TagSeparator)
                {
                    _newTag = _object.tag.Remove(0, _tagName.Length + 1);
                }
                else
                {
                    _newTag = _object.tag.Remove(_object.tag.Length - _tagName.Length - 1, _tagName.Length + 1);
                }
            }
            else
            {
                _newTag = MultiTags.BuiltInTagsNames[0];
            }

            // If new tag does not exist in the list of Unity tags, create it first
            if (!GetUnityTags().Contains(_newTag)) AddTag(_newTag);

            _object.tag = _newTag;
        }
    }


    /// <summary>
    /// Generic Menu method. Show the color picker window to modify a color.
    /// </summary>
    /// <param name="_tag">Tag to modify color.</param>
    public static void ColorPickerMenu(object _tag)
    {
        // Get the tag object
        Tag _tagObject = _tag as Tag;

        // Create delegate to set tag color
        Action<Color> _action = new Action<Color>((Color _color) => _tagObject.Color = _color);

        // Get the ColorPicker class
        EditorWindow _colorPicker = ScriptableObject.CreateInstance("ColorPicker") as EditorWindow;

        // Invoke color picker window
        _colorPicker.GetType().InvokeMember("Show", System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, _colorPicker, new object[] { null, _action, _tagObject.Color, true, false });
    }
    #endregion
}
