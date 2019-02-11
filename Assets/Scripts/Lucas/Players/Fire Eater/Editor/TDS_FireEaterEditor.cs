using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_FireEater), true), CanEditMultipleObjects]
public class TDS_FireEaterEditor : TDS_PlayerEditor
{
    /* TDS_FireEaterEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor of the TDS_FireEater class allowing to use properties, methods and a cool presentation for this script.
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
	 *	Creation of the TDS_FireEaterEditor class.
     *	
     *	    - Added the fireEaters & isFireEaterMultiEditing fields ; and the areFireEaterComponentsUnfolded, areFireEaterSettingsUnfolded & isFireEaterUnfolded fields & properties.
     *	    - Added the DrawComponentsAndReferences, DrawFireEaterEditor & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreFireEaterComponentsUnfolded"/>.</summary>
    private bool areFireEaterComponentsUnfolded = false;

    /// <summary>
    /// Indicates if the components & references editor of this class is unfolded.
    /// </summary>
    public bool AreFireEaterComponentsUnfolded
    {
        get { return areFireEaterComponentsUnfolded; }
        set
        {
            areFireEaterComponentsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areFireEaterComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreFireEaterSettingsUnfolded"/>.</summary>
    private bool areFireEaterSettingsUnfolded = false;

    /// <summary>
    /// Indicates if the settings editor of this class is unfolded.
    /// </summary>
    public bool AreFireEaterSettingsUnfolded
    {
        get { return areFireEaterSettingsUnfolded; }
        set
        {
            areFireEaterSettingsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areFireEaterSettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsFireEaterUnfolded"/>.</summary>
    private bool isFireEaterUnfolded = false;

    /// <summary>
    /// Indicates if the editor of this class is unfolded.
    /// </summary>
    public bool IsFireEaterUnfolded
    {
        get { return isFireEaterUnfolded; }
        set
        {
            isFireEaterUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("isFireEaterUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_FireEater classes.
    /// </summary>
    private List<TDS_FireEater> fireEaters = new List<TDS_FireEater>();

    /// <summary>
    /// Indicates if currently editing multiple instances of this class.
    /// </summary>
    private bool isFireEaterMultiEditing = false;
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
    /// Draws the custom editor of the TDS_FireEater class.
    /// </summary>
    protected void DrawFireEaterEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Fire Eater class settings
        if (TDS_EditorUtility.Button("Fire Eater", "Wrap / unwrap Fire Eater class settings", TDS_EditorUtility.HeaderStyle)) IsFireEaterUnfolded = !isFireEaterUnfolded;

        // If unfolded, draws the custom editor for the Fire Eater class
        if (isFireEaterUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Fire Eater script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Fire Eater class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreFireEaterComponentsUnfolded = !areFireEaterComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areFireEaterComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Fire Eater class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreFireEaterSettingsUnfolded = !areFireEaterSettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areFireEaterSettingsUnfolded)
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
        targets.ToList().ForEach(t => fireEaters.Add((TDS_FireEater)t));
        if (targets.Length == 1) isFireEaterMultiEditing = false;
        else isFireEaterMultiEditing = true;

        // Get the serializedProperties from the serializedObject


        // Loads the editor folded & unfolded values of this script
        isFireEaterUnfolded = EditorPrefs.GetBool("isFireEaterUnfolded", isFireEaterUnfolded);
        areFireEaterComponentsUnfolded = EditorPrefs.GetBool("areFireEaterComponentsUnfolded", areFireEaterComponentsUnfolded);
        areFireEaterSettingsUnfolded = EditorPrefs.GetBool("areFireEaterSettingsUnfolded", areFireEaterSettingsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        DrawFireEaterEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
