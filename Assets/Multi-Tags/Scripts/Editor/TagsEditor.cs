using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor of the TagsSO scriptable object class.
/// </summary>
[CustomEditor(typeof(TagsSO))]
public class TagsEditor : Editor 
{
    /* TagsEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor for the TagsSO class.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
     *  
     *      • Change the color of existing tags. That should be easy.
     *      
     *      • Change the name of created tags. That should be tricky.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[18 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    • Tags scriptable object is asset is now successfully entierly saved
     *	when modified.
     *	    
	 *	-----------------------------------
     * 
     *	Date :			[08 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    • Project tags are now updated when removing a tag !
     *	    
	 *	-----------------------------------
     * 
	 *	Date :			[04 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TagsEditor class.
     *	
     *	    • Created the editor for the scriptable object TagsSO from the TagsEditorWindow.
     *	    
     *	    • Finally, tags are displayed in a very cool way, and go to the line when reaching screen end.
     *	That wasn't a piece of cake, but it's done.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Editor
    /// <summary>
    /// The editing object of this editor.
    /// </summary>
    private TagsSO tagsSO = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Editor
    /// <summary>
    /// Displays the tags of the project.
    /// </summary>
    private void DrawTags()
    {
        // Draws a header
        EditorGUILayout.LabelField("Unity built-in Tags", EditorStyles.boldLabel);

        // If no tags, draws a information box and return
        if (tagsSO.UnityBuiltInTags == null || tagsSO.UnityBuiltInTags.Count == 0)
        {
            EditorGUILayout.HelpBox("No built-in tag found on this project", MessageType.Info);
        }
        else
        {
            // Draw Unity built-in tags
            MultiTagsUtility.DrawTags(tagsSO.UnityBuiltInTags.ToArray());
        }

        // Draws a header
        EditorGUILayout.LabelField("Custom Tags", EditorStyles.boldLabel);

        // If no tags, draws a information box and return
        if (tagsSO.Tags == null || tagsSO.Tags.Count == 0)
        {
            EditorGUILayout.HelpBox("No tag found on this project. How about create a first one ?", MessageType.Info);
        }
        else
        {
            // Draw all custom tags ; if clicking on a tag left button, remove it from the project and repaint
            if (MultiTagsUtility.DrawTags(tagsSO.Tags.ToArray(), RemoveTag))
            {
                Repaint();
                return;
            }
        }
    }

    /// <summary>
    /// Draws the multi-tags system editor.
    /// </summary>
    public void DrawTagsEditor()
    {
        // Draw all project tags
        DrawTags();
    }
    #endregion

    #region Tags
    /// <summary>
    /// Initializes this scriptable object with the project tags, and adds all tags this object contains to the project if not having them yet.
    /// </summary>
    public void Initialize()
    {
        List<string> _projectTags = MultiTagsUtility.GetUnityTags().ToList();
        List<string> _projectMultiTags = _projectTags.SelectMany(t => t.Split(MultiTags.TagSeparator)).Distinct().ToList();
        string[] _referenceTags = tagsSO.Tags.Select(t => t.Name).ToArray();
        string[] _referenceUnityTags = tagsSO.UnityBuiltInTags.Select(t => t.Name).ToArray();

        // Adds each tag of this scriptable object to the project in not having them yet
        foreach (string _tag in _referenceTags)
        {
            if (!_projectTags.Contains(_tag))
            {
                MultiTagsUtility.AddTag(_tag);
            }
            else
            {
                _projectTags.Remove(_tag);
            }

            if (_projectMultiTags.Contains(_tag)) _projectMultiTags.Remove(_tag);
        }

        // Adds all tags of this project this object doesn't have to it
        foreach (string _tag in _projectMultiTags)
        {
            if (MultiTags.BuiltInTagsNames.Contains(_tag))
            {
                if (!_referenceUnityTags.Contains(_tag)) tagsSO.UnityBuiltInTags.Add(new Tag(_tag));
            }
            else tagsSO.Tags.Add(new Tag(_tag));
        }

        SaveAsset();
    }

    /// <summary>
    /// Creates a brand new tag and add it to the project.
    /// </summary>
    /// <param name="_tag">New tag to add.</param>
    public void CreateTag(Tag _tag)
    {
        Undo.RecordObject(target, "Tag \"" + _tag.Name + "\" Creation");

        MultiTagsUtility.AddTag(_tag.Name);
        tagsSO.Tags.Add(_tag);
        tagsSO.Tags = tagsSO.Tags.OrderByDescending(t => t.Color.grayscale).ToList();

        SaveAsset();
    }

    /// <summary>
    /// Removes a tag from the project.
    /// </summary>
    /// <param name="_tag">Tag to remove</param>
    public void RemoveTag(Tag _tag)
    {
        // Display informative dialog about tag suppresion
        if (!EditorUtility.DisplayDialog("Confirm \"" + _tag.Name + "\" Tag Suppresion",
                                         "Are you sure you want to remove the Tag \"" + _tag.Name + "\" from the project ?" + "\n\n" + 
                                         "This tag will be removed from all Game Objects with this tag in loaded scene(s), but if a Game Object " +
                                         "in an unloaded scene has it, it will keep it and tag will be recreated when loading this scene." + "\n\n" +
                                         "To be sure the tag will be definitely removed, please check in other scenes and remove it from objects " +
                                         "having it first.",
                                         "Confirm",
                                         "Cancel")) return;

        // Get all project objects with tag to remove it from them
        GameObject[] _allObjectsWithTag = Resources.FindObjectsOfTypeAll<GameObject>().Where(g => g.GetTags().Contains(_tag.Name)).ToArray();

        Undo.RecordObjects(_allObjectsWithTag, "Game Object(s) Remove Tag \"" + _tag.Name + "\"");

        Undo.RecordObject(target, "Tag \"" + _tag.Name + "\" Destruction");

        // Remove tag from all objects having it
        MultiTagsUtility.RemoveTagFromGameObjects(_tag.Name, _allObjectsWithTag);

        // Remove all Unity tags containing this tag
        MultiTagsUtility.GetUnityTags().Where(t => t.Split(MultiTags.TagSeparator).Contains(_tag.Name)).ToList().ForEach(t => MultiTagsUtility.RemoveTag(t));

        // Remove tag from object
        tagsSO.Tags.Remove(_tag);

        SaveAsset();
    }

    /// <summary>
    /// Saves this asset.
    /// </summary>
    private void SaveAsset()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
    #endregion

    #endregion

    #region Unity Methods
    private void OnDisable()
    {
        //Debug.Log("Disable editor");
    }

    // This function is called when the object is loaded
    private void OnEnable()
    {
        // If no target, return
        if (!target) return;

        // Get editing object
        tagsSO = (TagsSO)target;

        //Debug.Log("Enable Editor");
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawTagsEditor();
    }
    #endregion

    #endregion
}
