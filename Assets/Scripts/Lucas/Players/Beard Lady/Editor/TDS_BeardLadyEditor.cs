using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_BeardLady), true), CanEditMultipleObjects]
public class TDS_BeardLadyEditor : TDS_PlayerEditor
{
    /* TDS_BeardLadyEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor of the TDS_BeardLady class allowing to use properties, methods and a cool presentation for this script.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_BeardLadyEditor class.
     *	
     *	    - Added the beardLadies & isBeardLadyMultiEditing fields ; and the areBeardLadyComponentsUnfolded, areBeardLadySettingsUnfolded & isBeardLadyUnfolded fields & properties.
     *	    - Added the DrawComponentsAndReferences, DrawBeardLadyEditor & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreBeardLadyComponentsUnfolded"/>.</summary>
    private bool areBeardLadyComponentsUnfolded = false;

    /// <summary>
    /// Indicates if the components & references editor of this class is unfolded.
    /// </summary>
    public bool AreBeardLadyComponentsUnfolded
    {
        get { return areBeardLadyComponentsUnfolded; }
        set
        {
            areBeardLadyComponentsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areBeardLadyComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreBeardLadySettingsUnfolded"/>.</summary>
    private bool areBeardLadySettingsUnfolded = false;

    /// <summary>
    /// Indicates if the settings editor of this class is unfolded.
    /// </summary>
    public bool AreBeardLadySettingsUnfolded
    {
        get { return areBeardLadySettingsUnfolded; }
        set
        {
            areBeardLadySettingsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areBeardLadySettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsBeardLadyUnfolded"/>.</summary>
    private bool isBeardLadyUnfolded = false;

    /// <summary>
    /// Indicates if the editor of this class is unfolded.
    /// </summary>
    public bool IsBeardLadyUnfolded
    {
        get { return isBeardLadyUnfolded; }
        set
        {
            isBeardLadyUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("isBeardLadyUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_BeardLady classes.
    /// </summary>
    private List<TDS_BeardLady> beardLadies = new List<TDS_BeardLady>();

    /// <summary>
    /// Indicates if currently editing multiple instances of this class.
    /// </summary>
    private bool isBeardLadyMultiEditing = false;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the custom editor for the components & references of this class.
    /// </summary>
    private void DrawComponentsAndReferences()
    {

    }

    /// <summary>
    /// Draws the custom editor of the TDS_BeardLady class.
    /// </summary>
    protected void DrawBeardLadyEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Beard Lady class settings
        if (TDS_EditorUtility.Button("Beard Lady", "Wrap / unwrap Beard Lady class settings", TDS_EditorUtility.HeaderStyle)) IsBeardLadyUnfolded = !isBeardLadyUnfolded;

        // If unfolded, draws the custom editor for the Beard Lady class
        if (isBeardLadyUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Beard Lady script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Beard Lady class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreBeardLadyComponentsUnfolded = !areBeardLadyComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areBeardLadyComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Beard Lady class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreBeardLadySettingsUnfolded = !areBeardLadySettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areBeardLadySettingsUnfolded)
            {
                DrawSettings();
            }

            EditorGUILayout.EndVertical();

            // Applies all modified properties on the SerializedObjects
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
    }

    /// <summary>
    /// Draws the custom editor for this class settings.
    /// </summary>
    private void DrawSettings()
    {

    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => beardLadies.Add((TDS_BeardLady)t));
        if (targets.Length == 1) isBeardLadyMultiEditing = false;
        else isBeardLadyMultiEditing = true;

        // Get the serializedProperties from the serializedObject


        // Loads the editor folded & unfolded values of this script
        isBeardLadyUnfolded = EditorPrefs.GetBool("isBeardLadyUnfolded", isBeardLadyUnfolded);
        areBeardLadyComponentsUnfolded = EditorPrefs.GetBool("areBeardLadyComponentsUnfolded", areBeardLadyComponentsUnfolded);
        areBeardLadySettingsUnfolded = EditorPrefs.GetBool("areBeardLadySettingsUnfolded", areBeardLadySettingsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        DrawBeardLadyEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
