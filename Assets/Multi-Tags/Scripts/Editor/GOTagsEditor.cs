using System;
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
     *      Nothing to see here...
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[24 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      • Moved the methods to add tag, remove tag and update project tags into the MultiTagsUtility class.
     *      
     *      • Organize a bit the class ; now, it's fresher.
	 *
	 *	-----------------------------------
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
     *      • Editor now display all tags from the first editing object in the inspector.
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
	 *	    • Researches. A lot, of researches.
     *	    
     *	    • Finally, get to know how to draw it, thanks to these guys :
     *	            - https://forum.unity.com/threads/custom-inspector-default-header-for-scriptableobjects.544276/
     *	            - https://forum.unity.com/threads/extending-instead-of-replacing-built-in-inspectors.407612/
     *	            
     *	    So, to do it we create an editor of the GameObjectInspector type.
     *	But warning ! Got a lot of errors when calling the OnDisable method of the editor, so removed it ;
     *	Got also sometime an error when destroying the editor, so maybe this will be removed.
     *	
     *	    • To get all tags of the project, use : "UnityEditorInternal.InternalEditorUtility.tags".
     *	    
     *	    • Created a simple editor with... almost nothing it it.
     *	    
     *	    • Added a header for Tags at the top left and a button at the top right of the editor
     *	to open the Tags Editor Window.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Editor Variables
    /// <summary>
    /// Unity GameObject class built-in editor.
    /// </summary>
    private Editor              defaultEditor                   = null;

    /// <summary>
    /// All editing game objects.
    /// </summary>
    private GameObject[]        targetGO                        = null;

    /// <summary>
    /// Is the tag editor visible or not ?
    /// </summary>
    private bool                isUnfolded                      = true;

    /// <summary>
    /// Last Unity tag of the editing objects ; used to refresh tags on inspector.
    /// </summary>
    private string[]            lastTags                        = new string[] { };

    /// <summary>
    /// Indicates if editing targets do have different tags
    /// </summary>
    private bool                haveTargetsDifferentTags        = false;
    #endregion

    #region Target Script(s) Variables
    /// <summary>
    /// Alls tags of the editing object, or tags in common of editing objects if performing multi-editing.
    /// </summary>
    private Tag[]                editingTags                      = new Tag[] { };
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws an editor for the custom tag system for the GameObject class.
    /// </summary>
    private void DrawTagSystem()
    {
        // If editing game object(s) tag has changed, update its tags
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
            // Draws a tags field, to edit tag from editing object(s)
            Action<Tag> _addTagCallback = new Action<Tag>((Tag _tag) => MultiTagsUtility.AddTagToGameObjects(_tag, targetGO));
            Action<Tag> _removeTagCallback = new Action<Tag>((Tag _tag) => MultiTagsUtility.RemoveTagFromGameObjects(_tag, targetGO));

            MultiTagsUtility.GUILayoutTagsField(editingTags, _addTagCallback, _removeTagCallback, true);

            if (haveTargetsDifferentTags)
            {
                EditorGUILayout.HelpBox("You are editing Game Objects with different tags.\n" +
                                        "Only tags in common will be displayed in the inspector.", MessageType.Info);
            }
        }

        // Males a space in the editor to finish
        GUILayout.Space(7);
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
        if (serializedObject.isEditingMultipleObjects)
        {
            string[][] _objectTags = targetGO.Select(t => t.GetTagNames()).ToArray();
            string[] _tagsInCommon = _objectTags.Aggregate((previousList, nextList) => previousList.Intersect(nextList).ToArray());

            editingTags = MultiTags.GetTags(_tagsInCommon);
            lastTags = targetGO.Select(g => g.tag).ToArray();
    
            haveTargetsDifferentTags = _objectTags.Any(t => !Enumerable.SequenceEqual(t, _tagsInCommon));
        }
        // Else, just get tags of the first editing object
        else
        {
            // Get editing object tags
            lastTags = new string[1] { targetGO[0].tag };
            editingTags = targetGO[0].GetTags();

            haveTargetsDifferentTags = false;
        }
    }
    #endregion

    #region Unity Methods
    // This function is called when the scriptable object goes out of scope
    private void OnDisable()
    {
        //Debug.Log("Game Object Editor => Disable");
        if (defaultEditor)
        {
            if (defaultEditor.GetType().GetField("m_PreviewCache", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(defaultEditor) == null)
            {
                defaultEditor.GetType().GetMethod("OnEnable").Invoke(defaultEditor, null);
            }

            DestroyImmediate(defaultEditor);
        }
    }

    // This function is called when the object is loaded
    private void OnEnable()
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
