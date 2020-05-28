using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#pragma warning disable 0414
[CustomEditor(typeof(TDS_FatLady), true), CanEditMultipleObjects]
public class TDS_FatLadyEditor : TDS_PlayerEditor
{
    /* TDS_FatLadyEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor of the TDS_FatLady class allowing to use properties, methods and a cool presentation for this script.
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
	 *	Creation of the TDS_FatLadyEditor class.
     *	
     *	    - Added the fatLadies & isFatLadyMultiEditing fields ; and the areFatLadyComponentsUnfolded, areFatLadySettingsUnfolded & isFatLadyUnfolded fields & properties.
     *	    - Added the DrawComponentsAndReferences, DrawFatLadyEditor & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #region Variables
    /// <summary>SerializedProperties for <see cref="TDS_FatLady.isAngry"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isAngry = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.isSnackAvailable"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isSnackAvailable = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.angrySpeedCoef"/> of type <see cref="int"/>.</summary>
    private SerializedProperty angrySpeedCoef = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.snackRestaureTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty snackRestaureTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.snackRestaureTimer"/> of type <see cref="float"/>.</summary>
    private SerializedProperty snackRestaureTimer = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.angryHealthStep"/> of type <see cref="int"/>.</summary>
    private SerializedProperty angryHealthStep = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.snackHealValue"/> of type <see cref="int"/>.</summary>
    private SerializedProperty snackHealValue = null;
    #endregion

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreFatLadyComponentsUnfolded"/>.</summary>
    private bool areFatLadyComponentsUnfolded = false;

    /// <summary>
    /// Indicates if the components & references editor of this class is unfolded.
    /// </summary>
    public bool AreFatLadyComponentsUnfolded
    {
        get { return areFatLadyComponentsUnfolded; }
        set
        {
            areFatLadyComponentsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areFatLadyComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreFatLadySettingsUnfolded"/>.</summary>
    private bool areFatLadySettingsUnfolded = false;

    /// <summary>
    /// Indicates if the settings editor of this class is unfolded.
    /// </summary>
    public bool AreFatLadySettingsUnfolded
    {
        get { return areFatLadySettingsUnfolded; }
        set
        {
            areFatLadySettingsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areFatLadySettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreFatLadySoundsUnfolded"/>.</summary>
    private bool areFatLadySoundsUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Fat Lady sounds is unfolded.
    /// </summary>
    public bool AreFatLadySoundsUnfolded
    {
        get { return areFatLadySoundsUnfolded; }
        set
        {
            areFatLadySoundsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areFatLadySoundsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsFatLadyUnfolded"/>.</summary>
    private bool isFatLadyUnfolded = false;

    /// <summary>
    /// Indicates if the editor of this class is unfolded.
    /// </summary>
    public bool IsFatLadyUnfolded
    {
        get { return isFatLadyUnfolded; }
        set
        {
            isFatLadyUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("isFatLadyUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_FatLady classes.
    /// </summary>
    private List<TDS_FatLady> fatLadies = new List<TDS_FatLady>();

    /// <summary>
    /// Indicates if currently editing multiple instances of this class.
    /// </summary>
    private bool isFatLadyMultiEditing = false;
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
    /// Draws the custom editor of the TDS_FatLady class.
    /// </summary>
    protected void DrawFatLadyEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Fat Lady class settings
        if (TDS_EditorUtility.Button("Fat Lady", "Wrap / unwrap Fat Lady class settings", TDS_EditorUtility.HeaderStyle)) IsFatLadyUnfolded = !isFatLadyUnfolded;

        // If unfolded, draws the custom editor for the Fat Lady class
        if (isFatLadyUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Fat Lady script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Fat Lady class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreFatLadyComponentsUnfolded = !areFatLadyComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areFatLadyComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Fat Lady class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreFatLadySettingsUnfolded = !areFatLadySettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areFatLadySettingsUnfolded)
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
        if (Application.isPlaying)
        {
            if (TDS_EditorUtility.Toggle("Angry", "Indicates if the Fat Lady is in Angry or in \"Cool\" mode", isAngry))
            {
                fatLadies.ForEach(f => f.IsAngry = isAngry.boolValue);
                serializedObject.Update();
            }

            GUILayout.Space(5);
        }

        if (TDS_EditorUtility.FloatField("Angry Speed Coef", "Coefficient applied to the Fat Lady's speed when angry", angrySpeedCoef))
        {
            fatLadies.ForEach(f => f.AngrySpeedCoef = angrySpeedCoef.floatValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.IntField("Angry Health Step", "Health value separating the Angry mode and the Cool one", angryHealthStep))
        {
            fatLadies.ForEach(f => f.AngryHealthStep = angryHealthStep.intValue);
            serializedObject.Update();
        }

        GUILayout.Space(3);

        if (Application.isPlaying)
        {
            if (TDS_EditorUtility.Toggle("Snack Available", "Indicates if the Fat Lady's snack is available for use", isSnackAvailable))
            {
                fatLadies.ForEach(f => f.IsSnackAvailable = isSnackAvailable.boolValue);
                serializedObject.Update();
            }

            GUILayout.Space(3);
            TDS_EditorUtility.ProgressBar(25, isSnackAvailable.boolValue ? 0 : snackRestaureTimer.floatValue / snackRestaureTime.floatValue, "Snack Restauration");
            GUILayout.Space(3);
        }

        if (TDS_EditorUtility.FloatField("Snack Restaure Time", "Time i takes to restaure the snack after eating (in seconds)", snackRestaureTime))
        {
            fatLadies.ForEach(f => f.SnackRestaureTime = snackRestaureTime.floatValue);
            serializedObject.Update();
        }
        if (TDS_EditorUtility.IntField("Snack Heal Value", "Heal value when snacking", snackHealValue))
        {
            fatLadies.ForEach(f => f.SnackHealValue = snackHealValue.intValue);
            serializedObject.Update();
        }
    }


    /// <summary>
    /// Draws edtiro for the Fat Lady sounds.
    /// </summary>
    private void DrawFatLadySounds()
    {
        
    }

    /// <summary>
    /// Here are all elements drawn in the sound editor.
    /// </summary>
    protected override void SoundEditor()
    {
        base.SoundEditor();

        GUILayout.Space(15);
        EditorGUILayout.BeginVertical("Box");

        // Button to show or not the Fat Lady sounds
        if (TDS_EditorUtility.Button("Fat Lady", "Wrap / unwrap Fat Lady Sounds", TDS_EditorUtility.HeaderStyle)) AreFatLadySoundsUnfolded = !areFatLadySoundsUnfolded;

        // If unfolded, draws the custom editor for the Fat Lady sounds
        if (areFatLadySoundsUnfolded)
        {
            DrawFatLadySounds();
        }

        EditorGUILayout.EndVertical();
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => fatLadies.Add((TDS_FatLady)t));
        if (targets.Length == 1) isFatLadyMultiEditing = false;
        else isFatLadyMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        isAngry = serializedObject.FindProperty("isAngry");
        isSnackAvailable = serializedObject.FindProperty("isSnackAvailable");
        angrySpeedCoef = serializedObject.FindProperty("angrySpeedCoef");
        snackRestaureTime = serializedObject.FindProperty("snackRestaureTime");
        snackRestaureTimer = serializedObject.FindProperty("snackRestaureTimer");
        angryHealthStep = serializedObject.FindProperty("angryHealthStep");
        snackHealValue = serializedObject.FindProperty("snackHealValue");

        // Loads the editor folded & unfolded values of this script
        isFatLadyUnfolded = EditorPrefs.GetBool("isFatLadyUnfolded", isFatLadyUnfolded);
        areFatLadyComponentsUnfolded = EditorPrefs.GetBool("areFatLadyComponentsUnfolded", areFatLadyComponentsUnfolded);
        areFatLadySettingsUnfolded = EditorPrefs.GetBool("areFatLadySettingsUnfolded", areFatLadySettingsUnfolded);

        areFatLadySoundsUnfolded = EditorPrefs.GetBool("areFatLadySoundsUnfolded", areFatLadySoundsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        DrawFatLadyEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
