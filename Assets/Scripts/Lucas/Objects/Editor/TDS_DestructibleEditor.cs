using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_Destructible)), CanEditMultipleObjects]
public class TDS_DestructibleEditor : TDS_DamageableEditor
{
    /* TDS_DestructibleEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Custom editor for the TDS_Destructible class.
     *  
     *      Allows to use properties & methods in the inspector.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[29 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TDS_DestructibleEditor class.
	*/

    #region Fields / Properties

    #region SerializedProperties

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="IsDestrUnfolded"/></summary>
    private bool isDestrUnfolded = true;

    /// <summary>
    /// Indicates if the editor for the Destructible class is unfolded or not.
    /// </summary>
    public bool IsDestrUnfolded
    {
        get { return isDestrUnfolded; }
        set
        {
            isDestrUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isDestrUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isDestrMultiEditing = false;

    /// <summary>
    /// All editing instances of the Destructible class.
    /// </summary>
    private List<TDS_Destructible> destructibles = new List<TDS_Destructible>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor for the editing Destructible classes
    /// </summary>
    protected void DrawDestructibleEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Damageable class settings
        if (TDS_EditorUtility.Button("Destructible", "Wrap / unwrap Destructible class settings", TDS_EditorUtility.HeaderStyle)) IsDestrUnfolded = !isDestrUnfolded;

        // If unfolded, draws the custom editor for the Destructible class
        if (isDestrUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Destructible script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Destructible class settings
            if (TDS_EditorUtility.Button("???", "Wrap / unwrap ???", TDS_EditorUtility.HeaderStyle));

            EditorGUILayout.EndVertical();

            // Applies all modified properties on the SerializedObjects
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => destructibles.Add((TDS_Destructible)t));
        if (targets.Length == 1) isDestrMultiEditing = false;
        else isDestrMultiEditing = true;

        // Get the serializedProperties from the serializedObject


        // Loads the editor folded and unfolded values of this class
        isDestrUnfolded = EditorPrefs.GetBool("isDestrUnfolded", isDestrUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the inspector for the Damageable class
        DrawDestructibleEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
