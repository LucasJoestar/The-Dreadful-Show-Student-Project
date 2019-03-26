using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_HitBox)), CanEditMultipleObjects]
public class TDS_HitBoxEditor : Editor
{
    /* TDS_HitBoxEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor of the HitBox class, to draw a tag field for the hittable tags.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	Mhmm...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[25 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the HitBoxEditor class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// All editing HitBox classes.
    /// </summary>
    private List<TDS_HitBox> _hitBoxes = new List<TDS_HitBox>();

    /// <summary>
    /// Indicates if currently editing multiple objects.
    /// </summary>
    private bool ismultiEditing = false;

    /// <summary>
    /// Tags to hit in common from all editing hit boxes.
    /// </summary>
    private List<Tag> targetTags = new List<Tag>();

    /// <summary>
    /// Index of the selected tag from the list of all project ones.
    /// </summary>
    private int selectedTagIndex = 0;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Add a tag to all editing objects.
    /// </summary>
    /// <param name="_tag">Tag to add.</param>
    private void AddTag(string _tag)
    {
        _hitBoxes.ForEach(h => h.HittableTags.Add(_tag));

        targetTags.Add(MultiTagsUtility.GetTag(_tag));
    }

    /// <summary>
    /// Remove a tag to all editing objects.
    /// </summary>
    /// <param name="_tag">Tag to remove.</param>
    private void RemoveTag(Tag _tag)
    {
        _hitBoxes.ForEach(h => h.HittableTags.Remove(_tag.Name));

        targetTags.Remove(_tag);

        selectedTagIndex = 0;
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get the target editing scripts
        targets.ToList().ForEach(t => _hitBoxes.Add((TDS_HitBox)t));
        if (targets.Length == 1) ismultiEditing = false;
        else ismultiEditing = true;

        // Get the tags from the editing hit boxes
        targetTags = MultiTagsUtility.GetTags(_hitBoxes.SelectMany(h => h.HittableTags).Distinct().ToArray()).ToList();
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        base.OnInspectorGUI();

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();

        // Draw a tag field for hittable tags
        EditorGUILayout.LabelField(new GUIContent("Hittable Tags", "Tags defining which object to hit"), GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 5));

        selectedTagIndex = MultiTagsUtility.TagField(selectedTagIndex, targetTags.ToArray(), AddTag);

        EditorGUILayout.EndHorizontal();

        // Draw all tags in hittable tags
        if (targetTags.Count == 0)
        {
            GUILayout.Space(5);
            return;
        }

        GUILayout.Space(5);

        MultiTagsUtility.DrawTags(targetTags.ToArray(), RemoveTag);
    }
    #endregion

    #endregion
}
