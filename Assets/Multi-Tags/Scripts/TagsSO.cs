using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object used to save and access to all tags of a project
/// for the multi-tags system.
/// </summary>
public class TagsSO : ScriptableObject 
{
    /* TagsSO :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Scriptable object used to store all tags of a project, allowing the save & load them dynamically.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     *  
     *      ... Nothing to see here.
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[06 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	    
     *	    Object now get project tags missing from it and adds its own that
     *	the project doesn't have regularly thanks to custom update in editor.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[03 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	    
     *	    Now, the tags are fully loaded & set when this scriptable object is being loaded. That's really sweet.
     *	    You can copy this scriptable object from project to project, and all tags will be fully loaded. That's cool.
     *	    Oh, and the tags of the project not yet on this object are automatically added on load. Awesome, isn't it ?
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[21 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TagsScriptableObject class.
     *	    
     *	    Base content, with only a list of Tag objects.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// All custom tags of this project.
    /// </summary>
    public List<Tag> Tags = new List<Tag>();

    /// <summary>
    /// All Unity built-in tags of this project.
    /// </summary>
    public List<Tag> UnityBuiltInTags = new List<Tag>();

    #if UNITY_EDITOR
    /// <summary>
    /// Counter used to initialize project with object when reaching a certain value.
    /// </summary>
    private int initializeCounter = 0;
    #endif

    #endregion

    #region Methods

    #region Original Methods

    #if UNITY_EDITOR
    /// <summary>
    /// Initializes the project tags with this object.
    /// </summary>
    private void Initialize()
    {
        // Initializes the project with this object
        UnityEditor.Editor _thisEditor = UnityEditor.Editor.CreateEditor(this);

        if (!_thisEditor)
        {
            return;
        }

        _thisEditor.GetType().InvokeMember("Initialize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public, null, _thisEditor, null);

        // Destroy the created editor
        DestroyImmediate(_thisEditor);
    }

    /// <summary>
    /// Custom update of this object when in editor.
    /// </summary>
    private void EditorUpdate()
    {
        // Increase counter
        initializeCounter++;

        // When reaching a certain limit, initialize the project with the object
        // and reset counter
        if (initializeCounter > 10000)
        {
            Initialize();
            initializeCounter = 0;
        }
    }
    #endif

    #endregion

    #region Unity Methods
    // This function is called when the scriptable object will be destroyed
    private void OnDestroy()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= EditorUpdate;
        #endif
    }

    // This function is called when the object is loaded
    private void OnEnable()
    {
        #if UNITY_EDITOR
        // In editor, intialize project with object on enable
        // and do it every X times with custom update
        UnityEditor.EditorApplication.update -= EditorUpdate;

        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Initialize();
            UnityEditor.EditorApplication.update += EditorUpdate;
        }

        // Initialize on editor quit
        UnityEditor.EditorApplication.quitting -= Initialize;
        UnityEditor.EditorApplication.quitting += Initialize;
        #endif
    }
    #endregion

    #endregion
}

[Serializable]
public class Tag
{
    /* Tag :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	    Class used to store a tag, with all its informations.
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *
     *	Date :			[21 / 01 / 2019]
     *	Author :		[Guibert Lucas]
     *
     *	Changes :
     *
     *	    Creation of the Tag class.
     *	    
     *	    This class Tag contain a name & an associated color.
     *
     *	-----------------------------------
    */

    #region Fields / Properties
    /// <summary>
    /// Name of this tag.
    /// </summary>
    public string Name = "New Tag";

    /// <summary>
    /// Color of this tag, used to render it in the editor.
    /// </summary>
    public Color Color = Color.white;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="_name">Name of the newly created tag.</param>
    public Tag(string _name)
    {
        Name = _name;
    }

    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="_name">Name of the newly created tag.</param>
    /// <param name="_color">Color of the tag, used to display it in the inspector.</param>
    public Tag(string _name, Color _color)
    {
        Name = _name;
        Color = _color;
    }
    #endregion
}
