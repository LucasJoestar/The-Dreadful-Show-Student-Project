using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#pragma warning disable 0114, 0414
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

    #region Components & References
    /// <summary>SerializedProperties for <see cref="TDS_Player.beardMagicFX"/> of type <see cref="GameObject"/>.</summary>
    private SerializedProperty beardMagicFX = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.beardFXTransform"/> of type <see cref="Transform"/>.</summary>
    private SerializedProperty beardFXTransform = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperties for <see cref="TDS_Player.currentBeardState"/> of type <see cref="BeardState"/>.</summary>
    private SerializedProperty currentBeardState = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.beardGrowInterval"/> of type <see cref="float"/>.</summary>
    private SerializedProperty beardGrowInterval = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.beardHealInterval"/> of type <see cref="float"/>.</summary>
    private SerializedProperty beardHealInterval = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.growBeardTimer"/> of type <see cref="float"/>.</summary>
    private SerializedProperty growBeardTimer = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.healBeardTimer"/> of type <see cref="float"/>.</summary>
    private SerializedProperty healBeardTimer = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.beardCurrentLife"/> of type <see cref="int"/>.</summary>
    private SerializedProperty beardCurrentLife = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.beardMaxLife"/> of type <see cref="int"/>.</summary>
    private SerializedProperty beardMaxLife = null;
    #endregion

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

    /// <summary>Backing field for <see cref="AreBeardLadyDebugsUnfolded"/>.</summary>
    private bool areBeardLadyDebugsUnfolded = false;

    /// <summary>
    /// Indicates if the debugs editor of this class is unfolded.
    /// </summary>
    public bool AreBeardLadyDebugsUnfolded
    {
        get { return areBeardLadyDebugsUnfolded; }
        set
        {
            areBeardLadyDebugsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areBeardLadyDebugsUnfolded", value);
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
        EditorGUILayout.ObjectField(beardMagicFX, new GUIContent("Beard State", "Current state of the Beard Lady's beard"));

        EditorGUILayout.ObjectField(beardFXTransform, new GUIContent("Beard FXs Transform", "Transform used to instantiate beard-related FXs"));
    }

    /// <summary>
    /// Draws the debugs editor of the TDS_Juggler editing objects.
    /// </summary>
    private void DrawDebugs()
    {
        GUILayout.Space(5);
        TDS_EditorUtility.ProgressBar(25, growBeardTimer.floatValue / beardGrowInterval.floatValue, "Beard Grow Progress");

        GUILayout.Space(10);

        TDS_EditorUtility.ProgressBar(25, healBeardTimer.floatValue / beardHealInterval.floatValue, "Beard Heal Progress");
        GUILayout.Space(5);
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

            if (Application.isPlaying)
            {
                GUILayout.Space(15);
                EditorGUILayout.BeginVertical("Box");

                // Button to show or not the Beard Lady class debugs
                if (TDS_EditorUtility.Button("Debugs", "Wrap / unwrap debugs", TDS_EditorUtility.HeaderStyle)) AreBeardLadyDebugsUnfolded = !areBeardLadyDebugsUnfolded;

                // If unfolded, draws the custom editor for the debugs
                if (areBeardLadyDebugsUnfolded)
                {
                    DrawDebugs();
                }

                EditorGUILayout.EndVertical();
            }

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
        if (TDS_EditorUtility.PropertyField("Beard State", "Current state of the Beard Lady's beard", currentBeardState) && Application.isPlaying)
        {
            beardLadies.ForEach(b => b.CancelInvokeGrowBeard());
            beardLadies.ForEach(b => b.CurrentBeardState = (BeardState)currentBeardState.enumValueIndex);
            serializedObject.Update();
        }

        GUILayout.Space(3);
        TDS_EditorUtility.ProgressBar(25, (float)currentBeardState.enumValueIndex / 3, "Beard State");
        GUILayout.Space(5);

        if (TDS_EditorUtility.FloatField("Beard Grow Interval", "Interval between two beard grow up", beardGrowInterval))
        {
            beardLadies.ForEach(b => b.BeardGrowInterval = beardGrowInterval.floatValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.IntSlider("Beard Life", "Current beard life", beardCurrentLife, 0, beardMaxLife.intValue) && Application.isPlaying)
        {
            beardLadies.ForEach(b => b.BeardCurrentLife = beardCurrentLife.intValue);
            serializedObject.Update();
        }

        GUILayout.Space(3);
        TDS_EditorUtility.ProgressBar(25, (float)beardCurrentLife.intValue / beardMaxLife.intValue, "Beard Life");
        GUILayout.Space(5);

        if (TDS_EditorUtility.IntField("Beard Max Life", "Maximum beard life value", beardMaxLife))
        {
            beardLadies.ForEach(b => b.BeardMaxLife = beardMaxLife.intValue);
            serializedObject.Update();
        }
        if (TDS_EditorUtility.FloatField("Beard Heal Interval", "Interval between two beard heal", beardHealInterval))
        {
            beardLadies.ForEach(b => b.BeardHealInterval = beardHealInterval.floatValue);
            serializedObject.Update();
        }
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
        beardMagicFX = serializedObject.FindProperty("beardMagicFX");
        beardFXTransform = serializedObject.FindProperty("beardFXTransform");

        currentBeardState = serializedObject.FindProperty("currentBeardState");
        beardGrowInterval = serializedObject.FindProperty("beardGrowInterval");
        beardHealInterval = serializedObject.FindProperty("beardHealInterval");
        growBeardTimer = serializedObject.FindProperty("growBeardTimer");
        healBeardTimer = serializedObject.FindProperty("healBeardTimer");
        beardCurrentLife = serializedObject.FindProperty("beardCurrentLife");
        beardMaxLife = serializedObject.FindProperty("beardMaxLife");

        // Loads the editor folded & unfolded values of this script
        isBeardLadyUnfolded = EditorPrefs.GetBool("isBeardLadyUnfolded", isBeardLadyUnfolded);
        areBeardLadyComponentsUnfolded = EditorPrefs.GetBool("areBeardLadyComponentsUnfolded", areBeardLadyComponentsUnfolded);
        areBeardLadyDebugsUnfolded = EditorPrefs.GetBool("areBeardLadyDebugsUnfolded", areBeardLadyDebugsUnfolded);
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
