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
     *      Nothing to see here...
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
	 *	    Creation of the TagsEditor class.
     *	
     *	    • Created the editor for the scriptable object TagsSO from the TagsEditorWindow.
     *	    
     *	    • Finally, tags are displayed in a very cool way, and go to the line when reaching screen end.
     *	That wasn't a piece of cake, but it's done.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// The editing object of this editor.
    /// </summary>
    private TagsSO      tagsSO      = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Displays the tags of the project.
    /// </summary>
    public void DrawTags()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);

        // Draws a header
        EditorGUILayout.LabelField("Unity built-in Tags", EditorStyles.boldLabel);

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();

        // Draw a button on top right to connect project tags
        if (GUILayout.Button(new GUIContent("Connect Project Tags", "Use this if Unity tags or tags on this asset have been created without using this editor ;\nThis will connect this project Unity tags with the tags asset to create missing tags on both of them."), GUILayout.Width(150), GUILayout.Height(25))) ConnectProjectTags();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // If no tags, draws a information box and return
        if (tagsSO.UnityBuiltInTags == null || tagsSO.UnityBuiltInTags.ObjectTags.Length == 0)
        {
            EditorGUILayout.HelpBox("No built-in tag found on this project", MessageType.Info);
        }
        else
        {
            // Draw Unity built-in tags
            MultiTagsUtility.GUILayoutDisplayTags(tagsSO.UnityBuiltInTags.ObjectTags);
        }

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        // Draws a header with a little plus button next to it, allowing to create new tags
        GUIContent _customTags = new GUIContent("Custom Tags", "Tags created by users on this project");
        EditorGUILayout.LabelField(_customTags, EditorStyles.boldLabel, GUILayout.MaxWidth(EditorStyles.boldLabel.CalcSize(_customTags).x));

        GUIStyle _olPlus = MultiTagsUtility.OLPlusStyle;
        Rect _buttonRect = GUILayoutUtility.GetRect(GUIContent.none, _olPlus);
        _buttonRect.y += 2;

        if (GUI.Button(_buttonRect, new GUIContent(string.Empty, "Create a new tag on this project"), _olPlus))
        {
            CreateTagWindow.CallWindow();
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        // If no tags, draws a information box and return
        if (tagsSO.CustomTags == null || tagsSO.CustomTags.ObjectTags.Length == 0)
        {
            EditorGUILayout.HelpBox("No tag found on this project. How about create a first one ?", MessageType.Info);
        }
        else
        {
            // Draw all custom tags ; if clicking on a tag left button, remove it from the project and repaint
            MultiTagsUtility.GUILayoutTagsField(tagsSO.CustomTags.ObjectTags, RemoveTag);
        }
    }

    /// <summary>
    /// Connect this project Unity tags with the tags asset to create missing ones on both of them.
    /// </summary>
    private void ConnectProjectTags()
    {
        List<string> _projectTags = MultiTagsUtility.GetUnityTags().ToList();
        List<string> _projectMultiTags = _projectTags.SelectMany(t => t.Split(MultiTags.TAG_SEPARATOR)).Distinct().ToList();
        string[] _referenceTags = tagsSO.CustomTags.TagNames;
        string[] _referenceUnityTags = tagsSO.UnityBuiltInTags.TagNames;

        // Adds each tag of this scriptable object to the project in not having them yet
        foreach (string _tag in _referenceTags)
        {
            if (!_projectTags.Contains(_tag))
            {
                MultiTagsUtility.CreateUnityTag(_tag);
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
                if (!_referenceUnityTags.Contains(_tag)) tagsSO.UnityBuiltInTags.AddTag(new Tag(_tag));
            }
            else tagsSO.CustomTags.AddTag(new Tag(_tag));
        }

        // Saves the asset
        EditorUtility.SetDirty(tagsSO);
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

        MultiTagsUtility.DestroyTag(_tag.Name);
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // If no target, return
        if (!target) return;

        // Get editing object
        tagsSO = (TagsSO)target;
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draw all project tags
        DrawTags();
    }
    #endregion

    #endregion
}
