using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Juggler), true), CanEditMultipleObjects]
public class TDS_JugglerEditor : TDS_PlayerEditor 
{
    /* TDS_JugglerEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor of the TDS_Juggler class allowing to use properties, methods and a cool presentation for this script.
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
	 *	Creation of the TDS_JugglerEditor class.
     *	
     *	    - Added the jugglers & isJugglerMultiEditing fields ; and the areJugglerComponentsUnfolded, areJugglerSettingsUnfolded & isJugglerUnfolded fields & properties.
     *	    - Added the DrawComponentsAndReferences, DrawJugglerEditor & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreJugglerComponentsUnfolded"/>.</summary>
    private bool areJugglerComponentsUnfolded = false;

    /// <summary>
    /// Indicates if the components & references editor of this class is unfolded.
    /// </summary>
    public bool AreJugglerComponentsUnfolded
    {
        get { return areJugglerComponentsUnfolded; }
        set
        {
            areJugglerComponentsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areJugglerComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreJugglerSettingsUnfolded"/>.</summary>
    private bool areJugglerSettingsUnfolded = false;

    /// <summary>
    /// Indicates if the settings editor of this class is unfolded.
    /// </summary>
    public bool AreJugglerSettingsUnfolded
    {
        get { return areJugglerSettingsUnfolded; }
        set
        {
            areJugglerSettingsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areJugglerSettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsJugglerUnfolded"/>.</summary>
    private bool isJugglerUnfolded = false;

    /// <summary>
    /// Indicates if the editor of this class is unfolded.
    /// </summary>
    public bool IsJugglerUnfolded
    {
        get { return isJugglerUnfolded; }
        set
        {
            isJugglerUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("isJugglerUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_Juggler classes.
    /// </summary>
    private List<TDS_Juggler> jugglers = new List<TDS_Juggler>();

    /// <summary>
    /// Indicates if currently editing multiple instances of this class.
    /// </summary>
    private bool isJugglerMultiEditing = false;
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
    /// Draws the custom editor of the TDS_Juggler class.
    /// </summary>
    protected void DrawJugglerEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Juggler class settings
        if (TDS_EditorUtility.Button("Juggler", "Wrap / unwrap Juggler class settings", TDS_EditorUtility.HeaderStyle)) IsJugglerUnfolded = !isJugglerUnfolded;

        // If unfolded, draws the custom editor for the Juggler class
        if (isJugglerUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Juggler script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Juggler class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreJugglerComponentsUnfolded = !areJugglerComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areJugglerComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Juggler class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreJugglerSettingsUnfolded = !areJugglerSettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areJugglerSettingsUnfolded)
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
        targets.ToList().ForEach(t => jugglers.Add((TDS_Juggler)t));
        if (targets.Length == 1) isJugglerMultiEditing = false;
        else isJugglerMultiEditing = true;

        // Get the serializedProperties from the serializedObject


        // Loads the editor folded & unfolded values of this script
        isJugglerUnfolded = EditorPrefs.GetBool("isJugglerUnfolded", isJugglerUnfolded);
        areJugglerComponentsUnfolded = EditorPrefs.GetBool("areJugglerComponentsUnfolded", areJugglerComponentsUnfolded);
        areJugglerSettingsUnfolded = EditorPrefs.GetBool("areJugglerSettingsUnfolded", areJugglerSettingsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        DrawJugglerEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
