using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// New editor for the GameObject class drawing a little space
/// for multi-tags system at the top of the inspector.
/// </summary>
[CustomEditor(typeof(GameObject)), CanEditMultipleObjects]
public class GOTagsEditor : Editor
{
    /* GOTagsEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Customizes the inspector of the GameObject class to add a section for the
     *	custom multi-tags system at the top of the components, just below the header.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     * 
     *      • Create an auto-complete system for the tag field.
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[07 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Now, you can remove tags to editing objects ! Yep.
     * 
     *      • Added possibility to undo action of adding & removing tags to editing objects.
     *      
     *      • When removing or adding tag to objects, previous tags of them not anymore used by other objects are removed.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[06 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Now, editor display all tags in common from editing objects in inspector when doing multi-editing.
     *      
     *      • User can now add new tags to editing objects ! That's cool.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[05 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      Editor now display all tags from the first editing object in the inspector.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[20 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      Creation of the GameObjectTagsEditor.
     * 
	 *	    Researches. A lot, of researches.
     *	    
     *	    Finally, get to know how to draw it, thanks to these guys :
     *	            - https://forum.unity.com/threads/custom-inspector-default-header-for-scriptableobjects.544276/
     *	            - https://forum.unity.com/threads/extending-instead-of-replacing-built-in-inspectors.407612/
     *	            
     *	    So, to do it we create an editor of the GameObjectInspector type.
     *	But warning ! Got a lot of errors when calling the OnDisable method of the editor, so removed it ;
     *	Got also sometime an error when destroying the editor, so maybe this will be removed.
     *	
     *	    To get all tags of the project, use : "UnityEditorInternal.InternalEditorUtility.tags".
     *	    
     *	    Created a simple editor with... almost nothing it it.
     *	Added a header for Tags at the top left and a button at the top right of the editor
     *	to open the Tags Editor Window.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Editor Variables
    /// <summary>
    /// Unity GameObject class built-in editor.
    /// </summary>
    private Editor defaultEditor;

    /// <summary>
    /// All editing game objects.
    /// </summary>
    private GameObject[] targetGO = null;

    /// <summary>
    /// Is the tag editor visible or not ?
    /// </summary>
    private bool isUnfolded = true;

    /// <summary>
    /// Last Unity tag of the editing objects ; used to refresh tags on inspector.
    /// </summary>
    private string[] lastTags = new string[] { };

    /// <summary>
    /// Index of the tag to display in the tag field, to add a new tag to the editing object(s).
    /// </summary>
    private int newTagIndex = 0;

    /// <summary>
    /// Indicates if editing targets do have different tags
    /// </summary>
    private bool haveTargetsDifferentTags = false;
    #endregion

    #region Target Script(s) Variables
    /// <summary>
    /// Alls tags of this object.
    /// </summary>
    private List<Tag> objectTags = new List<Tag>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws an editor for the custom tag system for the GameObject class.
    /// </summary>
    private void DrawTagSystem()
    {
        // If editing game object tag has changed, update its tags
        if (targetGO.Select(g => g.tag) != lastTags)
        {
            GetObjectsTags();
            Repaint();
        }

        EditorGUILayout.BeginHorizontal();

        // Title of the tag section
        EditorGUILayout.LabelField(new GUIContent("Tags"), EditorStyles.boldLabel);

        // Creates a button at the top right of the inspector to open the tags editor window
        // First, get its rect, then draw it
        Rect _editButtonRect = GUILayoutUtility.GetRect(new GUIContent("Edit Tags"), EditorStyles.miniButtonRight, GUILayout.Width(100));

        if (GUI.Button(_editButtonRect, "Edit Tags", EditorStyles.miniButtonRight))
        {
            TagsEditorWindow.CallWindow();
        }

        EditorGUILayout.EndHorizontal();

        // Draws the tag section for editing objects
        if (isUnfolded)
        {
            // Draws a tag field, to add new tags to editing object(s)
            newTagIndex = MultiTagsUtility.TagField(newTagIndex, objectTags.ToArray(), AddTag);

            GUILayout.Space(15);

            if ((objectTags.Count > 1) || ((objectTags.Count == 1) && (objectTags[0].Name != MultiTags.BuiltInTagsNames[0])))
            {
                MultiTagsUtility.DrawTags(objectTags.ToArray(), RemoveTag);
            }

            if (haveTargetsDifferentTags)
            {
                EditorGUILayout.HelpBox("You are editing Game Objects with different tags.\n" +
                                        "Only tags in common will be displayed in the inspector.", MessageType.Info);
            }
        }

        // Males a space in the editor to finish
        GUILayout.Space(10);
    }

    /// <summary>
    /// Get tags of editing object(s).
    /// If editing multiple objects and they have different tags,
    /// get only tags in common.
    /// </summary>
    private void GetObjectsTags()
    {
        // If editing multiple objects and they do not have the same tag,
        // get tags in common between them
        if (serializedObject.isEditingMultipleObjects && targetGO.Select(t => t.tag).Any(t => t != targetGO[0].tag))
        {
            lastTags = targetGO.Select(g => g.tag).ToArray();
            objectTags = MultiTagsUtility.GetTags(targetGO.Select(t => t.GetTags()).Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToArray())).ToList();

            haveTargetsDifferentTags = true;
        }
        // Else, just get tags of the first editing object
        else
        {
            // Get editing object tags
            lastTags = new string[1] { targetGO[0].tag };
            objectTags = MultiTagsUtility.GetTags(targetGO[0]).ToList();

            haveTargetsDifferentTags = false;
        }
    }

    /// <summary>
    /// Adds a new tag to all editing objects.
    /// </summary>
    /// <param name="_tagName">Name of the tag to add.</param>
    private void AddTag(string _tagName)
    {
        Undo.RecordObjects(targets, "Game Object(s) Add Tag \"" + _tagName + "\"");

        string[] _previousTags = targetGO.Select(g => g.tag).Distinct().ToArray();

        foreach (GameObject _gameObject in targetGO)
        {
            if (!_gameObject.GetTags().Contains(_tagName))
            {
                // If object is tag "Untagged", just tag it with the new one
                if (_gameObject.tag == MultiTags.BuiltInTagsNames[0])
                {
                    _gameObject.tag = _tagName;
                }
                // Otherwise, create its new tag if necessary and set it
                else
                {
                    string _newTag = _gameObject.tag + MultiTags.TagSeparator + _tagName;

                    // If new tag does not exist in the list of Unity tags, create it first
                    if (!MultiTagsUtility.GetUnityTags().Contains(_newTag)) MultiTagsUtility.AddTag(_newTag);

                    _gameObject.tag = _newTag;
                }
            }
        }

        // Update project tags with old ones
        UpdateProjectTags(_previousTags);
    }

    /// <summary>
    /// Removes a tag from all editing objects.
    /// </summary>
    /// <param name="_tag">Tag to remove.</param>
    private void RemoveTag(Tag _tag)
    {
        Undo.RecordObjects(targets, "Game Object(s) Remove Tag \"" + _tag.Name + "\"");

        string[] _previousTags = targetGO.Where(g => g.tag.Contains(MultiTags.TagSeparator)).Select(g => g.tag).Distinct().ToArray();

        // Remove tag from editing objects
        MultiTagsUtility.RemoveTagFromGameObjects(_tag.Name, targetGO);

        // Update project tags with old ones
        UpdateProjectTags(_previousTags);
    }

    /// <summary>
    /// Update project when changing object(s) tag.
    /// If no Game Object in the project do use the previous object(s) tag(s),
    /// then remove it / them from the project.
    /// </summary>
    /// <param name="_previousTags">Name of all tags to check to remove.</param>
    private void UpdateProjectTags(string[] _previousTags)
    {
        foreach (string _tag in _previousTags)
        {
            if ((_tag != string.Empty) && !MultiTags.BuiltInTagsNames.Contains(_tag) && (!Resources.FindObjectsOfTypeAll<GameObject>().Where(g => g.tag == _tag).FirstOrDefault()))
            {
                MultiTagsUtility.RemoveTag(_tag);
            }
        }
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    void OnEnable()
    {
        // Get editing objects as game objects
        targetGO = new GameObject[targets.Length];
        
        for (int _i = 0; _i < targets.Length; _i++)
        {
            targetGO[_i] = (GameObject)targets[_i];
        }

        // When this inspector is created, also create the built-in inspector
        defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.GameObjectInspector, UnityEditor"));

        // Get editing object(s) tags
        GetObjectsTags();

        //Debug.Log("Game Object Editor => Enable");
    }

    // Implement this function to make a custom header
    protected override void OnHeaderGUI()
    {
        // Draw the default header
        defaultEditor.DrawHeader();
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom multi-tags system, and then below it the default GameObject inspector
        DrawTagSystem();
        defaultEditor.OnInspectorGUI();
    }
	#endregion

	#endregion
}
