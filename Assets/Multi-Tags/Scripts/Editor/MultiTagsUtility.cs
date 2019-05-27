using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

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
     *      Nothing to see here...
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[04 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Fixed the Tags fields foldout GUI issue, so now it's good !
     *	    
     *	    • Moved the methods to add or remove tags from this project from the Tags Scriptable Object editor to this class.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[03 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Tried to add a foldout function next to Tags fields, but had a GUI problem, so it doesn't work perfectly...
	 *
	 *	-----------------------------------
     * 
     *	Date :			[30 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Optimization, and added fields & properties for GUIStyles and their size.
     *	    
     *	    • Corrected serializedProperties related fields methods to automatically draw their name, unless the user manually disable it with an overload.
     *	    
     *	    • Fixed multiple issues with GUI & Layout related methods. Damn, that's a work !
	 *
	 *	-----------------------------------
     * 
     *	Date :			[27 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Fixed serialization problem for both Tag & Tags fields.
     *	    
     *	    • Fixed issue about Tags field size on layout, with default inspector margins.
     *	    
     *	    • Fixed width & height for Tag & Tags fields GUI methods.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[26 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Arranged the Tag field methods, and fixed reference issue.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[24 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Moved the methods to add or remove a tag from game objects and to update project tags from the GOTagsEditor class to this one.
     *	    
     *	    • Added an overload to ignore "Untagged" tag for tags field.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[23 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Added more methods to draw tags with and without layout.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[22 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Added multiple methods with overloads to draw tag & tags fields with and without layout.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[30 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Removed some useless methods that now have an equivalent in the MultiTags class.
	 *
	 *	-----------------------------------
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
	 *	    • Created methods to draw tags in the editor, with or without a button
     *	left to each of them.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[22 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • Moved the Char variable used to separate tags to the newly created MultiTags class.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[03 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    • We can now create and remove tags from the project with the
     *	AddTag & RemoveTag methods.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[30 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the MultiTagsUtility class.
     *	
     *	    • Added a Char variable used to separate tags witht he multi-tags system
     *	from the Unity one.
     *	
     *	    • Creation of methods to get all this project tags using the Unity system
     *	or the multi one, witht he GetTags & GetUnityTags methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Key used to get boolean value with the Editor Prefs class, indicating if the tags in the editor are unfolded or not.
    /// </summary>
    private const string                TAGS_FOLDOUT_PREFS_KEY                      = "tagsFoldout";


    /// <summary>
    /// Indicates if this class fields & properties has been initialized.
    /// </summary>
    private static bool                 isInitialized                               = false;


    /// <summary>Backing field for <see cref="AreTagsUnfolded"/>.</summary>
    private static bool                 areTagsUnfolded                             = true;


    /// <summary>Backing field for <see cref="TagStyle"/>.</summary>
    private static GUIStyle             tagStyle                                    = null;

    /// <summary>Backing field for <see cref="OLMinusStyle"/>.</summary>
    private static GUIStyle             olMinusStyle                                = null;

    /// <summary>Backing field for <see cref="OLPlusStyle"/>.</summary>
    private static GUIStyle             olPlusStyle                                 = null;


    /// <summary>Backing field for <see cref="TagStyle"/>.</summary>
    private static float                tagHeight                                   = 0;

    /// <summary>Backing field for <see cref="FoldoutWidth"/>.</summary>
    private static float                foldoutWidth                                   = 0;

    /// <summary>Backing field for <see cref="OLMinusSize"/>.</summary>
    private static Vector2              olMinusSize                                 = Vector2.zero;

    /// <summary>Backing field for <see cref="OLPlusSize"/>.</summary>
    private static Vector2              olPlusSize                                  = Vector2.zero;


    /// <summary>
    /// GUI Style used to draw tags.
    /// </summary>
    public static GUIStyle              TagStyle
    {
        get
        {
            if (!isInitialized) Initialize();

            return tagStyle;
        }
    }

    /// <summary>
    /// GUI Style used to draw little minus buttons.
    /// </summary>
    public static GUIStyle              OLMinusStyle
    {
        get
        {
            if (!isInitialized) Initialize();

            return olMinusStyle;
        }
    }

    /// <summary>
    /// GUI Style used to draw little plus buttons.
    /// </summary>
    public static GUIStyle              OLPlusStyle
    {
        get
        {
            if (!isInitialized) Initialize();

            return olPlusStyle;
        }
    }


    /// <summary>
    /// Height of tags drawn with <see cref="TagStyle"/>.
    /// </summary>
    public static float                 TagHeight
    {
        get
        {
            if (!isInitialized) Initialize();

            return tagHeight;
        }
    }

    /// <summary>
    /// Width used to draw a foldout without label.
    /// </summary>
    public static float                 FoldoutWidth
    {
        get
        {
            if (!isInitialized) Initialize();

            return foldoutWidth;
        }
    }

    /// <summary>
    /// Total size of the <see cref="OLMinusStyle"/> style.
    /// </summary>
    public static Vector2               OLMinusSize
    {
        get
        {
            if (!isInitialized) Initialize();

            return olMinusSize;
        }
    }

    /// <summary>
    /// Total size of the <see cref="OLPlusSize"/> style.
    /// </summary>
    public static Vector2               OLPlusSize
    {
        get
        {
            if (!isInitialized) Initialize();

            return olPlusSize;
        }
    }


    /// <summary>
    /// Indicates if the tags in the editor are unfolded or not.
    /// </summary>
    public static bool                  AreTagsUnfolded
    {
        get
        {
            if (!isInitialized) Initialize();
            return areTagsUnfolded;
        }
        set
        {
            areTagsUnfolded = value;
            EditorPrefs.SetBool(TAGS_FOLDOUT_PREFS_KEY, value);
        }
    }

    /// <summary>
    /// Width used to draw tags fields with GUI Layout.
    /// </summary>
    public static float                 LayoutTagsFieldWidth { get { return EditorGUIUtility.currentViewWidth - 25; } }
    #endregion

    #region Methods

    #region Tags Edit & Utilities
    /// <summary>
    /// Creates and add a tag to this Unity project. This do not add the tag to the Tags Scriptable Object.
    /// </summary>
    /// <param name="_tag">New tag to create.</param>
    public static void CreateUnityTag(string _tag) { InternalEditorUtility.AddTag(_tag); }

    /// <summary>
    /// Destroy and remove a tag from this Unity project. This do not remove the tag from the Tags Scriptable Object.
    /// </summary>
    /// <param name="_tag">Tag to destroy.</param>
    public static void DestroyUnityTag(string _tag) { InternalEditorUtility.RemoveTag(_tag); }


    /// <summary>
    /// Creates a brand new tag and add it to the project.
    /// </summary>
    /// <param name="_tag">New tag to add.</param>
    public static void CreateTag(Tag _tag)
    {
        // Get Tags asset
        TagsSO _tagsAsset = GetTagsAsset();

        Undo.RecordObject(_tagsAsset, "Tag \"" + _tag.Name + "\" Creation");

        CreateUnityTag(_tag.Name);
        _tagsAsset.CustomTags.AddTag(_tag);

        // Saves the Tags asset.
        EditorUtility.SetDirty(_tagsAsset);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Removes a tag from the project.
    /// </summary>
    /// <param name="_tag">Tag to destroy.</param>
    public static void DestroyTag(string _tag)
    {
        // Get Tags asset
        TagsSO _tagsAsset = GetTagsAsset();

        // Get all project objects with tag to remove it from them
        GameObject[] _allObjectsWithTag = Resources.FindObjectsOfTypeAll<GameObject>().Where(g => g.GetTagNames().Contains(_tag)).ToArray();

        Undo.RecordObjects(_allObjectsWithTag, "Game Object(s) Remove Tag \"" + _tag + "\"");

        Undo.RecordObject(_tagsAsset, "Tag \"" + _tag + "\" Destruction");

        // Remove tag from all objects having it
        if (_allObjectsWithTag.Length > 0) RemoveTagFromGameObjects(_tag, _allObjectsWithTag);

        // Remove all Unity tags containing this tag
        GetUnityTags().Where(t => t.Split(MultiTags.TAG_SEPARATOR).Contains(_tag)).ToList().ForEach(t => DestroyUnityTag(t));

        // Remove tag from object
        _tagsAsset.CustomTags.RemoveTag(_tag);

        // Saves the Tags asset.
        EditorUtility.SetDirty(_tagsAsset);
        AssetDatabase.SaveAssets();
    }


    /// <summary>
    /// Check if the scriptable object containing all this project tags exist ; if not, create it.
    /// </summary>
    public static void CheckTagsAsset()
    {
        if (!Resources.Load<TagsSO>(MultiTags.TAGS_ASSET_PATH)) CreateTagsAsset();
    }

    /// <summary>
    /// Get the scriptable object containing all this project tags. If none, create it.
    /// </summary>
    public static TagsSO GetTagsAsset()
    {
        TagsSO _tagsSO = Resources.Load<TagsSO>(MultiTags.TAGS_ASSET_PATH);
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
        Directory.CreateDirectory(Application.dataPath + "/Resources/" + Path.GetDirectoryName(MultiTags.TAGS_ASSET_PATH));
        AssetDatabase.CreateAsset(_reference, "Assets/Resources/" + MultiTags.TAGS_ASSET_PATH + ".asset");
        AssetDatabase.SaveAssets();

        return _reference;
    }


    /// <summary>
    /// Get all this project tags from Unity, using the multi-tags system.
    /// </summary>
    /// <returns>Returns a string array of all tags, using the multi-tags system, of this projrct.</returns>
    public static string[] GetProjectTags()
    {
        return GetUnityTags().SelectMany(t => t.Split(MultiTags.TAG_SEPARATOR)).Distinct().ToArray();
    }

    /// <summary>
    /// Get all this project Unity tags, without using the multi-tags system.
    /// </summary>
    /// <returns>Returns a string array of all Unity tags from this project, without using multi-tags system.</returns>
    public static string[] GetUnityTags() { return InternalEditorUtility.tags; }


    /// <summary>
    /// Adds a tag to all game objects in a given array.
    /// </summary>
    /// <param name="_tagName">Name of the tag to add.</param>
    /// <param name="_gameObjects">Array of game objects to add tag to.</param>
    public static void AddTagToGameObjects(string _tagName, GameObject[] _gameObjects)
    {
        Undo.RecordObjects(_gameObjects, "Game Object(s) Add Tag \"" + _tagName + "\"");

        string[] _previousTags = _gameObjects.Select(g => g.tag).Distinct().ToArray();

        foreach (GameObject _gameObject in _gameObjects)
        {
            if (!_gameObject.GetTagNames().Contains(_tagName))
            {
                // If object is tag "Untagged", just tag it with the new one
                if (_gameObject.tag == MultiTags.BuiltInTagsNames[0])
                {
                    _gameObject.tag = _tagName;
                }
                // Otherwise, create its new tag if necessary and set it
                else
                {
                    string[] _newTags = (_gameObject.tag + MultiTags.TAG_SEPARATOR + _tagName).Split(MultiTags.TAG_SEPARATOR);
                    Array.Sort(_newTags, StringComparer.InvariantCulture);
                    string _newTag = _newTags[0];

                    for (int _i = 1; _i < _newTags.Length; _i++)
                    {
                        _newTag += MultiTags.TAG_SEPARATOR + _newTags[_i];
                    }
                    

                    // If new tag does not exist in the list of Unity tags, create it first
                    if (!GetUnityTags().Contains(_newTag)) CreateUnityTag(_newTag);

                    _gameObject.tag = _newTag;
                }
            }
        }

        // Update project tags with old ones
        UpdateProjectTags(_previousTags);
    }

    /// <summary>
    /// Adds a tag to all game objects in a given array.
    /// </summary>
    /// <param name="_tag">Name of the tag to add.</param>
    /// <param name="_gameObjects">Array of game objects to add tag to.</param>
    public static void AddTagToGameObjects(Tag _tag, GameObject[] _gameObjects)
    {
        AddTagToGameObjects(_tag.Name, _gameObjects);
    }


    /// <summary>
    /// Removes a tag from all game objects in a given array.
    /// </summary>
    /// <param name="_tagName">Name of the tag to remove.</param>
    /// <param name="_gameObjects">Array of game objects to remove tag from.</param>
    public static void RemoveTagFromGameObjects(string _tagName, GameObject[] _gameObjects)
    {
        Undo.RecordObjects(_gameObjects, "Game Object(s) Remove Tag \"" + _tagName + "\"");

        // Get object having specified tag
        _gameObjects = _gameObjects.Where(g => g.GetTagNames().Contains(_tagName)).ToArray();

        string[] _previousTags = _gameObjects.Where(g => g.tag.Contains(MultiTags.TAG_SEPARATOR)).Select(g => g.tag).Distinct().ToArray();

        // Remove tag from all objects
        foreach (GameObject _object in _gameObjects)
        {
            string _newTag = string.Empty;

            if (_object.tag != _tagName)
            {
                if (_object.tag.Contains(MultiTags.TAG_SEPARATOR + _tagName + MultiTags.TAG_SEPARATOR))
                {
                    _newTag = _object.tag.Replace(MultiTags.TAG_SEPARATOR + _tagName + MultiTags.TAG_SEPARATOR, MultiTags.TAG_SEPARATOR.ToString());
                }
                else if (_object.tag.Substring(0, _tagName.Length + 1) == _tagName + MultiTags.TAG_SEPARATOR)
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

            // If new game object tag does not exist in the list of Unity tags, create it first
            if (!GetUnityTags().Contains(_newTag)) CreateUnityTag(_newTag);

            _object.tag = _newTag;
        }

        // Update project tags with old ones
        UpdateProjectTags(_previousTags);
    }

    /// <summary>
    /// Removes a tag from all game objects in a given array.
    /// </summary>
    /// <param name="_tag">Tag to remove.</param>
    /// <param name="_gameObjects">Array of game objects to remove tag from.</param>
    public static void RemoveTagFromGameObjects(Tag _tag, GameObject[] _gameObjects)
    {
        RemoveTagFromGameObjects(_tag.Name, _gameObjects);
    }


    /// <summary>
    /// Update project when changing object(s) tag.
    /// If no Game Object in the project do use the previous object(s) tag(s),
    /// then remove it / them from the project.
    /// </summary>
    /// <param name="_tagsToCheck">Name of all tags to check to remove.</param>
    private static void UpdateProjectTags(string[] _tagsToCheck)
    {
        string[] _customUnityTags = GetUnityTags().Except(MultiTags.BuiltInTagsNames).ToArray();

        foreach (string _tag in _tagsToCheck)
        {
            if (_customUnityTags.Contains(_tag) && (!Resources.FindObjectsOfTypeAll<GameObject>().Where(g => g.tag == _tag).FirstOrDefault()))
            {
                DestroyUnityTag(_tag);
            }
        }
    }
    #endregion

    #region Editor, GUI & Layout

    #region Tag Field

    #region GUI
    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUITagField(Rect _position, GUIContent _label, SerializedProperty _property, GUIStyle _style)
    {
        // Draw label before tag
        GUI.Label(new Rect(_position.x, _position.y + 2, Mathf.Min(_position.width, EditorGUIUtility.labelWidth), Mathf.Min(_position.height, EditorGUIUtility.singleLineHeight)), _label, _style);

        return DoGUITagField(new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, Mathf.Max(_position.width - EditorGUIUtility.labelWidth, 0), _position.height), _property);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUITagField(Rect _position, GUIContent _label, SerializedProperty _property)
    {
        return GUITagField(_position, _label, _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUITagField(Rect _position, string _label, SerializedProperty _property, GUIStyle _style)
    {
        return GUITagField(_position, new GUIContent(_label), _property, _style);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUITagField(Rect _position, string _label, SerializedProperty _property)
    {
        return GUITagField(_position, new GUIContent(_label), _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <param name="_doDisplayName">Should the name of the property be displayed or not.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUITagField(Rect _position, SerializedProperty _property, bool _doDisplayName)
    {
        if (_doDisplayName) return GUITagField(_position, _property);
        else return DoGUITagField(_position, _property);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUITagField(Rect _position, SerializedProperty _property)
    {
        return GUITagField(_position, new GUIContent(ObjectNames.NicifyVariableName(_property.name), _property.tooltip), _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag, without label.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    private static bool DoGUITagField(Rect _position, SerializedProperty _property)
    {
        // Get tag to display informations
        Tag[] _allTags = GetTagsAsset().AllTags;
        string[] _allTagsName = _allTags.Select(t => t.Name).ToArray();

        string _name = _property.FindPropertyRelative("Name").stringValue;
        Color _color = _property.FindPropertyRelative("Color").colorValue;
        int _index = Array.IndexOf(_allTagsName, _name);

        // If editing multiple objects with different values, show it ; if value is not what it should be, correct it
        if (_property.hasMultipleDifferentValues)
        {
            _color = Color.white;
            _name = "----------";
            _index = -1;
        }
        else if (!_allTagsName.Contains(_name))
        {
            SetPropertyValue(_property, _allTags[0]);
        }
        else if (_color == Color.clear)
        {
            _property.FindPropertyRelative("Color").colorValue = Color.white;
            _color = Color.white;
        }
        else if (_color != _allTags[_index].Color)
        {
            SetPropertyValueIfDifferent(_property, _allTags[_index]);
        }

        // Get current event
        Event _event = Event.current;

        // Get bold label GUI style
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        // Get original GUI colors
        Color _guiOriginalColor = GUI.color;
        Color _boldLabelOriginalColor = _boldLabel.normal.textColor;

        // Set colors
        GUI.color = _color;
        if (GUI.color.grayscale < .5f) _boldLabel.normal.textColor = Color.white;
        else _boldLabel.normal.textColor = Color.black;

        Rect _tagRect = new Rect(_position.x, _position.y, Mathf.Min(CalculBoldLabelWidth(_name) + 9, _position.width), Mathf.Min(TagHeight, _position.height));

        // Creates the box for the whole tag area
        GUI.Box(_tagRect, string.Empty, TagStyle);

        // Set GUI color as white to display tag name properly
        GUI.color = Color.white;

        // Label to display the tag name
        if (_index < 0) GUI.Label(new Rect(_tagRect.x + 9, _tagRect.y + 2, _tagRect.width, _tagRect.height), _name);

        int _newIndex = EditorGUI.Popup(new Rect(_tagRect.x + 4, _tagRect.y + 2, _tagRect.width, _tagRect.height), _index, _allTagsName, _boldLabel);

        // Change value if changed
        bool _hasChanged = _newIndex != _index;

        if (_hasChanged)
        {
            SetPropertyValue(_property, _allTags[_newIndex]);
        }

        // Set back original GUI colors
        GUI.color = _guiOriginalColor;
        _boldLabel.normal.textColor = _boldLabelOriginalColor;

        // Create menu context on right click
        if ((_event.type == EventType.ContextClick) && _tagRect.Contains(_event.mousePosition) && !_property.hasMultipleDifferentValues)
        {
            GenericMenu _menu = new GenericMenu();
            GenericMenu.MenuFunction2 _colorCallback = new GenericMenu.MenuFunction2((object _object) => ColorPickerMenu((Tag)_object));

            _menu.AddItem(new GUIContent("Change Color", "Change the color of this Tag"), false, _colorCallback, MultiTags.GetTag(_name));

            _menu.ShowAsContext();
        }

        return _hasChanged;
    }


    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUITagField(Rect _position, GUIContent _label, Tag _tag, GUIStyle _style)
    {
        // Draw label before tag
        GUI.Label(new Rect(_position.x, _position.y + 2, Mathf.Min(_position.width, EditorGUIUtility.labelWidth), Mathf.Min(_position.height, EditorGUIUtility.singleLineHeight)), _label, _style);

        return GUITagField(new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, Mathf.Max(_position.width - EditorGUIUtility.labelWidth, 0), _position.height), _tag);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUITagField(Rect _position, GUIContent _label, Tag _tag)
    {
        return GUITagField(_position, _label, _tag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUITagField(Rect _position, string _label, Tag _tag, GUIStyle _style)
    {
        return GUITagField(_position, new GUIContent(_label), _tag, _style);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUITagField(Rect _position, string _label, Tag _tag)
    {
        return GUITagField(_position, new GUIContent(_label), _tag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUITagField(Rect _position, Tag _tag)
    {
        // Get tag to display informations
        Tag _newTag = _tag;

        Tag[] _allTags = GetTagsAsset().AllTags;
        string[] _allTagsName = _allTags.Select(t => t.Name).ToArray();
        int _index = Array.IndexOf(_allTagsName, _newTag.Name);

        // If value do not correspond to what it should be, correct it
        if (_index < 0)
        {
            _newTag = _allTags[0];
            _index = 0;
        }
        else if (_newTag != _allTags[_index])
        {
            _newTag = _allTags[_index];
        }
        else if (_newTag.Color == Color.clear)
        {
            _newTag.Color = Color.white;
        }

        // Get current event
        Event _event = Event.current;

        // Get bold label GUI style
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        // Get original GUI colors
        Color _guiOriginalColor = GUI.color;
        Color _boldLabelOriginalColor = _boldLabel.normal.textColor;

        // Set colors
        GUI.color = _newTag.Color;
        if (GUI.color.grayscale < .5f) _boldLabel.normal.textColor = Color.white;
        else _boldLabel.normal.textColor = Color.black;

        Rect _tagRect = new Rect(_position.x, _position.y, Mathf.Min(CalculBoldLabelWidth(_newTag.Name) + 9, _position.width), Mathf.Min(TagHeight, _position.height));

        // Creates the box for the whole tag area
        GUI.Box(_tagRect, string.Empty, TagStyle);

        // Set GUI color as white to display tag name properly
        GUI.color = Color.white;

        // Label to display the tag name
        int _newIndex = EditorGUI.Popup(new Rect(_tagRect.x + 4, _tagRect.y + 2, _tagRect.width, _tagRect.height), _index, _allTagsName, _boldLabel);

        // Change value if changed
        if (_newIndex != _index)
        {
            _newTag = _allTags[_newIndex];
        }

        // Set back original GUI colors
        GUI.color = _guiOriginalColor;
        _boldLabel.normal.textColor = _boldLabelOriginalColor;

        // Create menu context on right click
        if ((_event.type == EventType.ContextClick) && _tagRect.Contains(_event.mousePosition))
        {
            GenericMenu _menu = new GenericMenu();
            GenericMenu.MenuFunction2 _colorCallback = new GenericMenu.MenuFunction2((object _object) => ColorPickerMenu((Tag)_object));

            _menu.AddItem(new GUIContent("Change Color", "Change the color of this Tag"), false, _colorCallback, MultiTags.GetTag(_newTag.Name));

            _menu.ShowAsContext();
        }

        return _newTag;
    }
    #endregion

    #region GUI Layout
    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUILayoutTagField(GUIContent _label, SerializedProperty _property, GUIStyle _style)
    {
        return GUITagField(EditorGUILayout.GetControlRect(true, TagHeight, _style), _label, _property, _style);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUILayoutTagField(GUIContent _label, SerializedProperty _property)
    {
        return GUILayoutTagField(_label, _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUILayoutTagField(string _label, SerializedProperty _property, GUIStyle _style)
    {
        return GUILayoutTagField(new GUIContent(_label), _property, _style);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUILayoutTagField(string _label, SerializedProperty _property)
    {
        return GUILayoutTagField(new GUIContent(_label), _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <param name="_doDisplayName">Should the name of the property be displayed or not.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUILayoutTagField(SerializedProperty _property, bool _doDisplayName)
    {
        if (_doDisplayName) return GUILayoutTagField(_property);
        else return DoGUITagField(EditorGUILayout.GetControlRect(false, TagHeight), _property);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_property">The tag serialized property to edit.</param>
    /// <returns>Returns true if the property has changed, false otherwise.</returns>
    public static bool GUILayoutTagField(SerializedProperty _property)
    {
        return GUILayoutTagField(new GUIContent(ObjectNames.NicifyVariableName(_property.name), _property.tooltip), _property, EditorStyles.label);
    }


    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUILayoutTagField(GUIContent _label, Tag _tag, GUIStyle _style)
    {
        return GUITagField(EditorGUILayout.GetControlRect(true, TagHeight, _style), _label, _tag, _style);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUILayoutTagField(GUIContent _label, Tag _tag)
    {
        return GUILayoutTagField(_label, _tag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUILayoutTagField(string _label, Tag _tag, GUIStyle _style)
    {
        return GUILayoutTagField(new GUIContent(_label), _tag, _style);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tag">The tag to edit.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUILayoutTagField(string _label, Tag _tag)
    {
        return GUILayoutTagField(new GUIContent(_label), _tag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for selecting a tag.
    /// </summary>
    /// <param name="_tag">The tag to edit.</param>
    /// <returns>Returns the tag selected by the user.</returns>
    public static Tag GUILayoutTagField(Tag _tag)
    {
        return GUITagField(EditorGUILayout.GetControlRect(false, TagHeight), _tag);
    }
    #endregion

    #endregion

    #region Tags Field

    #region GUI
    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, GUIContent _label, SerializedProperty _property, GUIStyle _style)
    {
        // Draw label before tag
        GUI.Label(new Rect(_position.x, _position.y + 2, Mathf.Min(_position.width, EditorGUIUtility.labelWidth), Mathf.Min(_position.height, EditorGUIUtility.singleLineHeight)), _label, _style);

        Rect _return = DoGUITagsField(new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, Mathf.Max(_position.width - EditorGUIUtility.labelWidth, 0), _position.height), _property);
        _return.x = _position.x;

        return _return;
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, GUIContent _label, SerializedProperty _property)
    {
        return GUITagsField(_position, _label, _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, string _label, SerializedProperty _property, GUIStyle _style)
    {
        return GUITagsField(_position, new GUIContent(_label), _property, _style);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, string _label, SerializedProperty _property)
    {
        return GUITagsField(_position, new GUIContent(_label), _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <param name="_doDisplayName">Should the name of the property be displayed or not.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, SerializedProperty _property, bool _doDisplayName)
    {
        if (_doDisplayName) return GUITagsField(_position, _property);
        else return DoGUITagsField(_position, _property);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, SerializedProperty _property)
    {
        return GUITagsField(_position, new GUIContent(ObjectNames.NicifyVariableName(_property.name), _property.tooltip), _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object, without label.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect DoGUITagsField(Rect _position, SerializedProperty _property)
    {
        // Get tags to display informations
        Tag[] _allTags = GetTagsAsset().AllTags;
        Tags[] _allPropertyTags = new Tags[_property.serializedObject.targetObjects.Length];

        FieldInfo _fieldInfo = _property.serializedObject.targetObject.GetType().GetField(_property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (_fieldInfo != null)
        {
            for (int _i = 0; _i < _allPropertyTags.Length; _i++)
            {
                _allPropertyTags[_i] = _fieldInfo.GetValue(_property.serializedObject.targetObjects[_i]) as Tags;
            }
        }

        // If editing multiple objects with different values, only draw tags in common
        Tag[] _tagsInCommon = null;

        if (_property.serializedObject.isEditingMultipleObjects)
        {
            string[] _tagsName = _allPropertyTags.Select(t => t.ObjectTags.Select(o => o.Name)).Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToArray()).ToArray();
            _tagsInCommon = new Tag[_tagsName.Length];

            for (int _i = 0; _i < _tagsName.Length; _i++)
            {
                _tagsInCommon[_i] = _allPropertyTags[0].ObjectTags.Where(t => t.Name == _tagsName[_i]).FirstOrDefault();
            }
        }
        else
        {
            _tagsInCommon = _allPropertyTags[0].ObjectTags;
        }

        // Get current event
        Event _event = Event.current;

        // Get bold label GUI style
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        // Get original GUI colors
        Color _guiOriginalColor = GUI.color;
        Color _boldLabelOriginalColor = _boldLabel.normal.textColor;

        // Get rect informations
        Rect _lastRect = new Rect(_position.x, _position.y, 0, 0);
        float _xStartPos = _lastRect.x;

        // Draw a little plus button allowing to add new tags
        Tag[] _tagsToAdd = _allTags.Except(_tagsInCommon).ToArray();

        if (_tagsToAdd.Length > 0)
        {
            Rect _buttonRect = new Rect(new Vector2(_lastRect.position.x, _lastRect.position.y + 2), OLPlusSize);

            if (GUI.Button(_buttonRect, new GUIContent(string.Empty, "Add a new Tag to the list"), OLPlusStyle))
            {
                GenericMenu _menu = new GenericMenu();
                GenericMenu.MenuFunction2 _callback = new GenericMenu.MenuFunction2((object _tag) => _allPropertyTags.Where(t => !t.ObjectTags.Contains((Tag)_tag)).ToList().ForEach(t => t.AddTag((Tag)_tag)));

                foreach (Tag _tag in _tagsToAdd)
                {
                    _menu.AddItem(new GUIContent(_tag.Name, "Add this tag"), false, _callback, _tag);
                }

                _menu.ShowAsContext();
            }

            // Updates rect
            _lastRect.x += _buttonRect.width - 3;
        }

        // Create boolean indicating if mouse if under a tag rect
        bool _isMouseUnderTag = false;

        // Draw each tag
        foreach (Tag _tag in _tagsInCommon)
        {
            // Check if the tag exist and if so if its reference is linked
            Tag _matchingTag = _allTags.Where(t => t.Name == _tag.Name).FirstOrDefault();
            if (_matchingTag == default(Tag))
            {
                foreach (Tags _tags in _allPropertyTags)
                {
                    _tags.RemoveTag(_tag);
                }

                // Equivalent to repaint
                _property.serializedObject.SetIsDifferentCacheDirty();
            }
            else if (_tag != _matchingTag)
            {
                foreach (Tags _tags in _allPropertyTags)
                {
                    int _index = Array.FindIndex(_tags.ObjectTags, (Tag t) => t.Name == _tag.Name);
                    if (_index >= 0) _tags.ObjectTags[_index] = _matchingTag;
                }
            }

            // Calcul rects
            Rect _labelRect = new Rect(_lastRect.xMax, _lastRect.y, CalculBoldLabelWidth(_tag.Name) + 5, TagHeight);

            if (_labelRect.xMin > _xStartPos)
            {
                if ((_labelRect.xMax + OLMinusSize.x) > (_position.xMax - FoldoutWidth - 7))
                {
                    // When at the end of the first line, draw a foldout button allowing to fold / unfold tags
                    if (_labelRect.y == _position.yMin)
                    {
                        Rect _foldoutRect = new Rect(_position.xMax - FoldoutWidth, _labelRect.y + 2, FoldoutWidth, _labelRect.height);

                        if (GUI.Button(_foldoutRect, GUIContent.none, GUIStyle.none))
                        {
                            AreTagsUnfolded = !AreTagsUnfolded;
                        }

                        if (_event.type == EventType.Repaint) EditorStyles.foldout.Draw(_foldoutRect, _foldoutRect.Contains(_event.mousePosition), GUIUtility.hotControl > 0, AreTagsUnfolded, false);

                        // If folded, tags are only drawn on the first line
                        if (!AreTagsUnfolded)
                        {
                            _position.height = _labelRect.height;
                            break;
                        }
                    }

                    _labelRect.y += _labelRect.height + 2;
                    _labelRect.x = _xStartPos;

                    // If exceeding rect bounds, stop drawing
                    if (_labelRect.y > _position.yMax) break;
                }
                else
                {
                    _labelRect.x += 5;
                }
            }

            _labelRect.yMax = Mathf.Min(_labelRect.yMax, _position.yMax);
            Rect _badgeRect = new Rect(_labelRect.position,  new Vector2(_labelRect.size.x + OLMinusSize.x, _labelRect.size.y));

            // Update last rect
            _lastRect = _badgeRect;

            // Set colors
            GUI.color = _tag.Color;
            if (GUI.color.grayscale < .5f) _boldLabel.normal.textColor = Color.white;
            else _boldLabel.normal.textColor = Color.black;

            // Creates the box for the whole tag area
            GUI.Box(_badgeRect, string.Empty, TagStyle);

            // Set GUI color as white to display tag name properly
            GUI.color = Color.white;

            // Draw button left to the tag to remove it
            if (GUI.Button(new Rect(new Vector2(_badgeRect.position.x + 1, _badgeRect.position.y + 2), OLMinusSize), GUIContent.none, OLMinusStyle) && (_event.button == 0))
            {
                foreach (Tags _tags in _allPropertyTags)
                {
                    _tags.RemoveTag(_tag);
                }
            }

            // Label to display the tag name
            GUI.Label(new Rect(_labelRect.x + OLMinusSize.x, _labelRect.y + 2, _labelRect.width, _labelRect.height), _tag.Name, _boldLabel);

            // Create menu context on right click
            if (_badgeRect.Contains(_event.mousePosition))
            {
                // Set indicative boolean
                _isMouseUnderTag = true;

                if (_event.type == EventType.ContextClick)
                {
                    GenericMenu _menu = new GenericMenu();
                    GenericMenu.MenuFunction2 _colorCallback = new GenericMenu.MenuFunction2((object _object) => ColorPickerMenu((Tag)_object));

                    _menu.AddItem(new GUIContent("Change Color", "Change the color of this Tag"), false, _colorCallback, _tag);

                    _menu.ShowAsContext();
                }
            }
        }

        // Set back original GUI colors
        GUI.color = _guiOriginalColor;
        _boldLabel.normal.textColor = _boldLabelOriginalColor;

        // Create menu context on right click
        if (!_isMouseUnderTag && (_event.type == EventType.ContextClick) && new Rect(_position.x, _position.y, _position.width, _lastRect.yMax - _position.y).Contains(_event.mousePosition))
        {
            GenericMenu _menu = new GenericMenu();
            GenericMenu.MenuFunction _orderByColorAscending = new GenericMenu.MenuFunction(() => _allPropertyTags.ToList().ForEach(o => o.ObjectTags = o.ObjectTags.OrderBy(t => t.Color.grayscale).ToArray()));
            GenericMenu.MenuFunction _orderByColorDescending = new GenericMenu.MenuFunction(() => _allPropertyTags.ToList().ForEach(o => o.ObjectTags = o.ObjectTags.OrderByDescending(t => t.Color.grayscale).ToArray()));

            GenericMenu.MenuFunction _orderByNameAscending = new GenericMenu.MenuFunction(() => _allPropertyTags.ToList().ForEach(o => o.ObjectTags = o.ObjectTags.OrderBy(t => t.Name).ToArray()));
            GenericMenu.MenuFunction _orderByNameDescending = new GenericMenu.MenuFunction(() => _allPropertyTags.ToList().ForEach(o => o.ObjectTags = o.ObjectTags.OrderByDescending(t => t.Name).ToArray()));

            GenericMenu.MenuFunction _orderByLengthAscending = new GenericMenu.MenuFunction(() => _allPropertyTags.ToList().ForEach(o => o.ObjectTags = o.ObjectTags.OrderBy(t => t.Name.Length).ToArray()));
            GenericMenu.MenuFunction _orderByLengthDescending = new GenericMenu.MenuFunction(() => _allPropertyTags.ToList().ForEach(o => o.ObjectTags = o.ObjectTags.OrderByDescending(t => t.Name.Length).ToArray()));

            _menu.AddItem(new GUIContent("Order by.../Color/Ascending", "Order this Tags by Color, in an ascending way"), false, _orderByColorAscending);
            _menu.AddItem(new GUIContent("Order by.../Color/Descending", "Order this Tags by Color, in a descending way"), false, _orderByColorDescending);

            _menu.AddItem(new GUIContent("Order by.../Name/Ascending", "Order this Tags by Name, in an ascending way"), false, _orderByNameAscending);
            _menu.AddItem(new GUIContent("Order by.../Name/Descending", "Order this Tags by Name, in a descending way"), false, _orderByNameDescending);

            _menu.AddItem(new GUIContent("Order by.../Name Length/Ascending", "Order this Tags by Name length, in an ascending way"), false, _orderByLengthAscending);
            _menu.AddItem(new GUIContent("Order by.../Name Length/Descending", "Order this Tags by Name length, in a descending way"), false, _orderByLengthDescending);

            _menu.ShowAsContext();
        }

        // Return new Rect position
        return new Rect(_position.x, _lastRect.y + TagHeight, 0, 0);
    }


    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, GUIContent _label, Tags _tags, GUIStyle _style)
    {
        return GUITagsField(_position, _label, _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, _style);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, GUIContent _label, Tags _tags)
    {
        return GUITagsField(_position, _label, _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, string _label, Tags _tags, GUIStyle _style)
    {
        return GUITagsField(_position, new GUIContent(_label), _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, _style);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, string _label, Tags _tags)
    {
        return GUITagsField(_position, new GUIContent(_label), _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, Tags _tags)
    {
        return GUITagsField(_position, _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, false);
    }


    /// <summary>
    /// Make a field for editing a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, GUIContent _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback, GUIStyle _style)
    {
        // Draw label before tag
        GUI.Label(new Rect(_position.x, _position.y + 2, Mathf.Min(_position.width, EditorGUIUtility.labelWidth), Mathf.Min(_position.height, EditorGUIUtility.singleLineHeight)), _label, _style);

        Rect _return = GUITagsField(new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, Mathf.Max(_position.width - EditorGUIUtility.labelWidth, 0), _position.height), _tags, _addTagCallback, _removeTagCallback, false);
        _return.x = _position.x;

        return _return;
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, GUIContent _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback)
    {
        return GUITagsField(_position, _label, _tags, _addTagCallback, _removeTagCallback, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, string _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback, GUIStyle _style)
    {
        return GUITagsField(_position, new GUIContent(_label), _tags, _addTagCallback, _removeTagCallback, _style);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, string _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback)
    {
        return GUITagsField(_position, new GUIContent(_label), _tags, _addTagCallback, _removeTagCallback, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback)
    {
        return GUITagsField(_position, _tags, _addTagCallback, _removeTagCallback, false);
    }


    /// <summary>
    /// Make a field for editing a tag array, but without button to add new ones.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUITagsField(Rect _position, Tag[] _tags, Action<Tag> _removeTagCallback)
    {
        return GUITagsField(_position, _tags, null, _removeTagCallback, false);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <param name="_doIgnoreUntaggedTag">If true, Untagged tag is not displayed, and cannot be added.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    private static Rect GUITagsField(Rect _position, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback, bool _doIgnoreUntaggedTag)
    {
        // Get if user can add or remove tags
        bool _canAddTag = _addTagCallback != null;
        bool _canRemoveTag = _removeTagCallback != null;

        // Get current event
        Event _event = Event.current;

        // Get all tags
        Tag[] _allTags = GetTagsAsset().AllTags;

        // Get bold label GUI style
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        // Get original GUI colors
        Color _guiOriginalColor = GUI.color;
        Color _boldLabelOriginalColor = _boldLabel.normal.textColor;

        // Get rect informations
        Rect _lastRect = new Rect(_position.x, _position.y, 0, 0);
        float _xStartPos = _lastRect.x;

        // If can add tag, draw a little plus button allowing to add new tags
        if (_canAddTag)
        {
            Tag[] _tagsToAdd = _allTags.Except(_tags).ToArray();
            if (_doIgnoreUntaggedTag) _tagsToAdd = _tagsToAdd.Where(t => t.Name != MultiTags.BuiltInTagsNames[0]).ToArray();

            if (_tagsToAdd.Length > 0)
            {
                // Display button to add new tag
                Rect _buttonRect = new Rect(new Vector2(_lastRect.position.x, _lastRect.position.y + 2), OLPlusSize);

                if (GUI.Button(_buttonRect, new GUIContent(string.Empty, "Add a new Tag to the list"), OLPlusStyle))
                {
                    GenericMenu _menu = new GenericMenu();
                    GenericMenu.MenuFunction2 _callback = new GenericMenu.MenuFunction2((object _tag) => _addTagCallback.Invoke((Tag)_tag));

                    foreach (Tag _tag in _tagsToAdd)
                    {
                        _menu.AddItem(new GUIContent(_tag.Name, "Add this tag"), false, _callback, _tag);
                    }

                    _menu.ShowAsContext();
                }

                // Updates rect
                _lastRect.x += _buttonRect.width - 3;
            }
        }

        // Draw each tag
        for (int _i = 0; _i < _tags.Length; _i++)
        {
            Tag _tag = _tags[_i];

            // If the tag is "Untagged" one and it should be ignored, ignore it
            if (_doIgnoreUntaggedTag && (_tag.Name == MultiTags.BuiltInTagsNames[0])) continue;

            // Check if the tag exist and if so if its reference is linked
            Tag _matchingTag = _allTags.Where(t => t.Name == _tag.Name).FirstOrDefault();
            if (_matchingTag == default(Tag))
            {
                if (_canRemoveTag) _removeTagCallback.Invoke(_tag);
                else _tags[_i] = _allTags[0];
            }
            else if (_tag != _matchingTag)
            {
                _tags[_i] = _matchingTag;
            }

            // Calcul rects
            Rect _labelRect = new Rect(_lastRect.xMax, _lastRect.y, CalculBoldLabelWidth(_tag.Name) + 5, TagHeight);

            if (_labelRect.xMin > _xStartPos)
            {
                if ((_labelRect.xMax + (_canRemoveTag ? OLMinusSize.x : 4)) > (_position.xMax - FoldoutWidth - 7))
                {
                    // When at the end of the first line, draw a foldout button allowing to fold / unfold tags
                    if (_labelRect.y == _position.yMin)
                    {
                        Rect _foldoutRect = new Rect(_position.xMax - FoldoutWidth, _labelRect.y + 2, FoldoutWidth, _labelRect.height);

                        if (GUI.Button(_foldoutRect, GUIContent.none, GUIStyle.none))
                        {
                            AreTagsUnfolded = !AreTagsUnfolded;
                        }

                        if (_event.type == EventType.Repaint) EditorStyles.foldout.Draw(_foldoutRect, _foldoutRect.Contains(_event.mousePosition), GUIUtility.hotControl > 0, AreTagsUnfolded, false);

                        // If folded, tags are only drawn on the first line
                        if (!AreTagsUnfolded)
                        {
                            _position.height = _labelRect.height;
                            break;
                        }
                    }

                    _labelRect.y += _labelRect.height + 2;
                    _labelRect.x = _xStartPos;

                    // If exceeding rect bounds, stop drawing
                    if (_labelRect.y > _position.yMax) break;
                }
                else
                {
                    _labelRect.x += 5;
                }
            }

            _labelRect.yMax = Mathf.Min(_labelRect.yMax, _position.yMax);
            Rect _badgeRect = _labelRect;
            _badgeRect.width += (_canRemoveTag ? OLMinusSize.x : 4);

            // Update last rect
            _lastRect = _badgeRect;

            // Set colors
            GUI.color = _tag.Color;
            if (GUI.color.grayscale < .5f) _boldLabel.normal.textColor = Color.white;
            else _boldLabel.normal.textColor = Color.black;

            // Creates the box for the whole tag area
            GUI.Box(_badgeRect, string.Empty, TagStyle);

            // Set GUI color as white to display tag name properly
            GUI.color = Color.white;

            // Draw button left to the tag to remove it
            if (_canRemoveTag && GUI.Button(new Rect(new Vector2(_badgeRect.position.x + 1, _badgeRect.position.y + 2), OLMinusSize), GUIContent.none, OLMinusStyle) && (_event.button == 0))
            {
                _removeTagCallback.Invoke(_tag);
            }

            // Label to display the tag name
            GUI.Label(new Rect(_labelRect.x + (_canRemoveTag ? OLMinusSize.x : 4), _labelRect.y + 2, _labelRect.width, _labelRect.height), _tag.Name, _boldLabel);

            // Create menu context on right click
            if (_badgeRect.Contains(_event.mousePosition))
            {
                if ((_event.type == EventType.ContextClick))
                {
                    GenericMenu _menu = new GenericMenu();
                    GenericMenu.MenuFunction2 _colorCallback = new GenericMenu.MenuFunction2((object _object) => ColorPickerMenu((Tag)_object));

                    _menu.AddItem(new GUIContent("Change Color", "Change the color of this Tag"), false, _colorCallback, _tag);

                    _menu.ShowAsContext();
                }
            }
        }
        
        // Set back original GUI colors
        GUI.color = _guiOriginalColor;
        _boldLabel.normal.textColor = _boldLabelOriginalColor;

        // Return new Rect position
        return new Rect(_position.x, _lastRect.y + TagHeight, 0, 0);
    }
    #endregion

    #region GUI Layout
    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutTagsField(GUIContent _label, SerializedProperty _property, GUIStyle _style)
    {
        Rect _position = EditorGUILayout.GetControlRect(true, 0, _style);
        
        Rect _newPosition = GUITagsField(new Rect(_position.position, new Vector2(LayoutTagsFieldWidth, float.MaxValue)), _label, _property, _style);
        GUILayoutUtility.GetRect(_position.width, _newPosition.y - _position.y);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    public static void GUILayoutTagsField(GUIContent _label, SerializedProperty _property)
    {
        GUILayoutTagsField(_label, _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutTagsField(string _label, SerializedProperty _property, GUIStyle _style)
    {
        GUILayoutTagsField(new GUIContent(_label), _property, _style);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    public static void GUILayoutTagsField(string _label, SerializedProperty _property)
    {
        GUILayoutTagsField(new GUIContent(_label), _property, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    /// <param name="_doDisplayName">Should the name of the property be displayed or not.</param>
    public static void GUILayoutTagsField(SerializedProperty _property, bool _doDisplayName)
    {
        if (_doDisplayName) GUILayoutTagsField(_property);
        else
        {
            Rect _position = EditorGUILayout.GetControlRect(false, 0);

            Rect _newPosition = DoGUITagsField(new Rect(_position.position, new Vector2(LayoutTagsFieldWidth, float.MaxValue)), _property);
            GUILayoutUtility.GetRect(_position.width, _newPosition.y - _position.y);
        }
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_property">The Tags serialized property to edit.</param>
    public static void GUILayoutTagsField(SerializedProperty _property)
    {
        GUILayoutTagsField(new GUIContent(ObjectNames.NicifyVariableName(_property.name), _property.tooltip), _property, EditorStyles.label);
    }


    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutTagsField(GUIContent _label, Tags _tags, GUIStyle _style)
    {
        GUILayoutTagsField(_label, _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, _style);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    public static void GUILayoutTagsField(GUIContent _label, Tags _tags)
    {
        GUILayoutTagsField(_label, _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutTagsField(string _label, Tags _tags, GUIStyle _style)
    {
        GUILayoutTagsField(new GUIContent(_label), _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, _style);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The Tags object to edit.</param>
    public static void GUILayoutTagsField(string _label, Tags _tags)
    {
        GUILayoutTagsField(new GUIContent(_label), _tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a Tags object.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_tags">The Tags object to edit.</param>
    public static void GUILayoutTagsField(Tags _tags)
    {
        GUILayoutTagsField(_tags.ObjectTags, _tags.AddTag, _tags.RemoveTag, false);
    }


    /// <summary>
    /// Make a field for editing a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutTagsField(GUIContent _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback, GUIStyle _style)
    {
        Rect _position = EditorGUILayout.GetControlRect(true, 0, _style);

        Rect _newPosition = GUITagsField(new Rect(_position.position, new Vector2(LayoutTagsFieldWidth, float.MaxValue)), _label, _tags, _addTagCallback, _removeTagCallback, _style);
        GUILayoutUtility.GetRect(_position.width, _newPosition.y - _position.y);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    public static void GUILayoutTagsField(GUIContent _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback)
    {
        GUILayoutTagsField(_label, _tags, _addTagCallback, _removeTagCallback, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutTagsField(string _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback, GUIStyle _style)
    {
        GUILayoutTagsField(new GUIContent(_label), _tags, _addTagCallback, _removeTagCallback, _style);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    public static void GUILayoutTagsField(string _label, Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback)
    {
        GUILayoutTagsField(new GUIContent(_label), _tags, _addTagCallback, _removeTagCallback, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for editing a tag array, but without button to add new ones.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    public static void GUILayoutTagsField(Tag[] _tags, Action<Tag> _removeTagCallback)
    {
        GUILayoutTagsField(_tags, null, _removeTagCallback, false);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    public static void GUILayoutTagsField(Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback)
    {
        GUILayoutTagsField(_tags, _addTagCallback, _removeTagCallback, false);
    }

    /// <summary>
    /// Make a field for editing a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups. 
    /// </summary>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    /// <param name="_doIgnoreUntaggedTag">If true, Untagged tag is not displayed, and cannot be added.</param>
    public static void GUILayoutTagsField(Tag[] _tags, Action<Tag> _addTagCallback, Action<Tag> _removeTagCallback, bool _doIgnoreUntaggedTag)
    {
        Rect _position = EditorGUILayout.GetControlRect(false, 0);

        Rect _newPosition = GUITagsField(new Rect(_position.position, new Vector2(LayoutTagsFieldWidth, float.MaxValue)), _tags, _addTagCallback, _removeTagCallback, _doIgnoreUntaggedTag);
        GUILayoutUtility.GetRect(_position.width, _newPosition.y - _position.y);
    }
    #endregion

    #endregion

    #region Display Tags

    #region GUI
    /// <summary>
    /// Make a field for displaying a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUIDisplayTags(Rect _position, GUIContent _label, Tag[] _tags, GUIStyle _style)
    {
        return GUITagsField(_position, _label, _tags, null, null, _style);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUIDisplayTags(Rect _position, GUIContent _label, Tag[] _tags)
    {
        return GUITagsField(_position, _label, _tags, null, null, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUIDisplayTags(Rect _position, string _label, Tag[] _tags, GUIStyle _style)
    {
        return GUITagsField(_position, new GUIContent(_label), _tags, null, null, _style);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUIDisplayTags(Rect _position, string _label, Tag[] _tags)
    {
        return GUITagsField(_position, new GUIContent(_label), _tags, null, null, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// </summary>
    /// <param name="_position">Rectangle on the screen to use for the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <returns>Returns new Rect position after this field, back to the line, where to continue drawing.</returns>
    public static Rect GUIDisplayTags(Rect _position, Tag[] _tags)
    {
        return GUITagsField(_position, _tags, null, null, false);
    }
    #endregion

    #region GUI Layout
    /// <summary>
    /// Make a field for displaying a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutDisplayTags(GUIContent _label, Tag[] _tags, GUIStyle _style)
    {
        GUILayoutTagsField(_label, _tags, null, null, _style);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    public static void GUILayoutDisplayTags(GUIContent _label, Tag[] _tags)
    {
        GUILayoutTagsField(_label, _tags, null, null, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_style">GUIStyle to use for the label.</param>
    public static void GUILayoutDisplayTags(string _label, Tag[] _tags, GUIStyle _style)
    {
        GUILayoutTagsField(new GUIContent(_label), _tags, null, null, _style);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups.
    /// </summary>
    /// <param name="_label">Optional label to display in front of the field.</param>
    /// <param name="_tags">The tag array to display.</param>
    /// <param name="_addTagCallback">Callback when trying to add a tag.</param>
    /// <param name="_removeTagCallback">Callback when trying to remove a tag.</param>
    public static void GUILayoutDisplayTags(string _label, Tag[] _tags)
    {
        GUILayoutTagsField(new GUIContent(_label), _tags, null, null, EditorStyles.label);
    }

    /// <summary>
    /// Make a field for displaying a tag array.
    /// Since this field height has to be calculated, it cannot be affected by layout groups.
    /// </summary>
    /// <param name="_tags">The tag array to display.</param>
    public static void GUILayoutDisplayTags(Tag[] _tags)
    {
        GUILayoutTagsField(_tags, null, null, false);
    }
    #endregion

    #endregion

    #region Utilities
    /// <summary>
    /// Show the color picker window to modify a color.
    /// </summary>
    /// <param name="_color">Color to modify.</param>
    public static void ColorPickerMenu(Color _color)
    {
        // Create delegate to set tag color
        Action<Color> _action = new Action<Color>((Color _c) => _color = _c);

        // Get the ColorPicker class
        EditorWindow _colorPicker = ScriptableObject.CreateInstance("ColorPicker") as EditorWindow;

        // Invoke color picker window
        _colorPicker.GetType().InvokeMember("Show", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, _colorPicker, new object[] { null, _action, _color, true, false });
    }

    /// <summary>
    /// Show the color picker window to modify a tag color.
    /// </summary>
    /// <param name="_tag">Tag to modify color.</param>
    public static void ColorPickerMenu(Tag _tag)
    {
        // Get the tag object
        Tag _tagObject = _tag as Tag;

        // Create delegate to set tag color
        Action<Color> _action = new Action<Color>((Color _color) => _tagObject.Color = _color);

        // Get the ColorPicker class
        EditorWindow _colorPicker = ScriptableObject.CreateInstance("ColorPicker") as EditorWindow;

        // Invoke color picker window
        _colorPicker.GetType().InvokeMember("Show", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, _colorPicker, new object[] { null, _action, _tagObject.Color, true, false });
    }


    /// <summary>
    /// Calculates the total width of a label drawn with the Bold Label GUIStyle.
    /// </summary>
    /// <param name="_label">Label to calculate width.</param>
    /// <returns>Returns total width of the label drawn with Bold Label GUIStyle.</returns>
    public static float CalculBoldLabelWidth(string _label)
    {
        return EditorStyles.boldLabel.CalcSize(new GUIContent(_label)).x;
    }


    /// <summary>
    /// Set the value of a property.
    /// </summary>
    /// <param name="_property">Property to change value.</param>
    /// <param name="_value">New value of the property.</param>
    public static void SetPropertyValue(SerializedProperty _property, object _value)
    {
        // Get field info and set value if not null
        FieldInfo _fieldInfo = _property.serializedObject.targetObject.GetType().GetField(_property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (_fieldInfo == null) return;

        foreach (Object _target in _property.serializedObject.targetObjects)
        {
            _fieldInfo.SetValue(_target, _value);
        }

        // Equivalent to repaint
        _property.serializedObject.SetIsDifferentCacheDirty();
    }

    /// <summary>
    /// Set the value of a property if its value does not match.
    /// </summary>
    /// <param name="_property">Property to check value.</param>
    /// <param name="_value">Value to check if property value does match.</param>
    public static void SetPropertyValueIfDifferent(SerializedProperty _property, object _value)
    {
        // Get field info and check value if not null
        FieldInfo _fieldInfo = _property.serializedObject.targetObject.GetType().GetField(_property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (_fieldInfo == null) return;

        bool _isDifferent = false;

        foreach (Object _target in _property.serializedObject.targetObjects)
        {
            if (_fieldInfo.GetValue(_target) != _value)
            {
                _fieldInfo.SetValue(_target, _value);
                _isDifferent = true;
            }
        }

        // Equivalent to repaint
        if (_isDifferent) _property.serializedObject.SetIsDifferentCacheDirty();
    }
    #endregion

    #endregion

    #region Initialization
    /// <summary>
    /// Initializes all this class fields & properties.
    /// </summary>
    private static void Initialize()
    {
        // If one field has already been initialized, then return
        if (tagStyle != null) return;

        tagStyle = new GUIStyle("CN CountBadge");
        olMinusStyle = new GUIStyle("OL Minus");
        olPlusStyle = new GUIStyle("OL Plus");

        tagHeight = tagStyle.CalcHeight(GUIContent.none, 0) + 1;
        foldoutWidth = EditorStyles.foldout.CalcSize(GUIContent.none).x;
        olMinusSize = olMinusStyle.CalcSize(GUIContent.none);
        olPlusSize = olPlusStyle.CalcSize(GUIContent.none);

        areTagsUnfolded = EditorPrefs.GetBool(TAGS_FOLDOUT_PREFS_KEY, areTagsUnfolded);

        isInitialized = true;
    }
    #endregion

    #endregion
}

/// <summary>
/// Custom property drawer for the Tag class.
/// </summary>
[CustomPropertyDrawer(typeof(Tag))]
public class TagDrawer : PropertyDrawer
{
    /* TagDrawer :
     * 
     *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom property drawer for the Tag object.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     * 
     *      Nothing to see here...
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     * 
     *	Date :			[07 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Full property drawer of the class, meanining it now support multi-editing, set value as object reference, display the field name
     *  in front of it (and event tooltip, if this property was working)... That now work very fine !
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[06 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      Creation of the TagDrawer class.
     *      
     *      • Create a property drawer not displaying field name nor supporting multi-editing, and not setting value as reference.
	 *
	 *	-----------------------------------
    */

    #region Methods
    // Override this method to specify how tall the GUI for this field is in pixels
    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        return MultiTagsUtility.TagHeight;
    }

    // Override this method to make your own GUI for the property.
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        MultiTagsUtility.GUITagField(_position, _label, _property);
    }
    #endregion
}

/// <summary>
/// Custom property drawer for the Tags class.
/// </summary>
[CustomPropertyDrawer(typeof(Tags))]
public class TagsDrawer : PropertyDrawer
{
    /* TagsDrawer :
     * 
     *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom property drawer for the Tags class.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     * 
     *	Date :			[21 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Fixed height issue.
     *      
     *      • Added a context menu on right click allowing to order tags by multiple parameters.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[20 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Implemented a system to add tags, and set specified the height for the property drawer. But got to fix it, the height is not perfectly defined.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[07 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Added system to draw tags, with multi-editing, but got to test it since no system to add tag was added. Also, still got to specify the height of this property drawer.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[06 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      Creation of the TagsDrawer class.
	 *
	 *	-----------------------------------
    */

    #region Methods
    // Override this method to specify how tall the GUI for this field is in pixels
    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        if (!MultiTagsUtility.AreTagsUnfolded) return MultiTagsUtility.TagHeight;


        // Get tags to display, and calcul height depending on their size and field width
        Tags[] _allTags = new Tags[_property.serializedObject.targetObjects.Length];

        FieldInfo _fieldInfo = _property.serializedObject.targetObject.GetType().GetField(_property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // If this field info is null, then an unexpected error as occured
        if (_fieldInfo == null) return 0;

        for (int _i = 0; _i < _allTags.Length; _i++)
        {
            _allTags[_i] = _fieldInfo.GetValue(_property.serializedObject.targetObjects[_i]) as Tags;
        }

        Tag[] _tagsInCommon = _property.serializedObject.isEditingMultipleObjects ?
                              _allTags.Select(t => t.ObjectTags).Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToArray()) :
                              _allTags[0].ObjectTags;

        // Get rect informations
        GUIStyle _boldLabel = EditorStyles.boldLabel;

        Rect _lastRect = new Rect(EditorGUIUtility.labelWidth + EditorStyles.inspectorDefaultMargins.padding.left, EditorGUIUtility.singleLineHeight, 0, 0);

        float _xStartPos = EditorGUIUtility.labelWidth + EditorStyles.inspectorDefaultMargins.padding.left;

        if (MultiTagsUtility.GetTagsAsset().AllTags.Except(_tagsInCommon).ToArray().Length > 0)
        {
            _lastRect.x += MultiTagsUtility.OLPlusSize.x - 3;
        }

        foreach (Tag _tag in _tagsInCommon)
        {
            // Calcul rects
            _lastRect.xMin = _lastRect.xMax;
            _lastRect.xMax += MultiTagsUtility.CalculBoldLabelWidth(_tag.Name) + 5 + MultiTagsUtility.OLMinusSize.x;

            if ((_lastRect.xMax > (EditorGUIUtility.currentViewWidth - 6)) && (_lastRect.xMin > _xStartPos))
            {
                _lastRect.y += MultiTagsUtility.TagHeight + 2;
                _lastRect.x = _xStartPos;
            }
            else
            {
                _lastRect.x += 5;
            }
        }

        // Return height
        return _lastRect.y;
    }

    // Override this method to make your own GUI for the property.
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        MultiTagsUtility.GUITagsField(_position, _label, _property);
    }
    #endregion
}
