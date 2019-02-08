using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Enemy)), CanEditMultipleObjects]
public class TDS_EnemyEditor : TDS_CharacterEditor 
{
    /* TDS_EnemyEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom Editor of the Enemy class
     *	    - Display all fields and properties relative to the enemy class
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[07/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the Editor]
     *	
     *	    - Added the agent, canBeDown, canThrow, detectionRange and attacks properties
     *	    - Added the methods DrawEnemyEditor, DrawComponentsAndReferences and DrawSettings Methods
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region ForldOut
    /// <summary>Backing field for <see cref="AreEnemyComponentsUnfolded"/></summary>
    private bool areEnemyComponentsUnfolded = false;

    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreEnemyComponentsUnfolded
    {
        get { return areEnemyComponentsUnfolded; }
        set
        {
            areEnemyComponentsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areEnemyComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreEnemySettingsUnfolded"/></summary>
    private bool areEnemySettingsUnfolded = false;

    /// <summary>
    /// Are the settings of the Character class unfolded for the editor ?
    /// </summary>
    public bool AreEnemySettingsUnfolded
    {
        get { return areEnemySettingsUnfolded; }
        set
        {
            areEnemySettingsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areEnemySettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsEnemyUnfolded"/></summary>
    private bool isEnemyUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Character class is unfolded or not.
    /// </summary>
    public bool IsEnemyUnfolded
    {
        get { return isEnemyUnfolded; }
        set
        {
            isEnemyUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isEnemyUnfolded", value);
        }
    }
    #endregion

    #region SerializedProperties

    #region Components and References
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.agent"/> of type <see cref="CustomNavMeshAgent"/>.</summary>
    private SerializedProperty agent = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.canBeDown"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty canBeDown = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.canThrow"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty canThrow = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.detectionRange"/> of type <see cref="float"/>.</summary>
    private SerializedProperty detectionRange = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.attacks"/> of type <see cref="TDS_EnemyAttack[]"/>.</summary>
    private SerializedProperty attacks = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.enemyState"/> of type <see cref="EnemyState"/>.</summary>
    private SerializedProperty enemyState = null;
    #endregion

    #endregion

    #region Target Scripts Info
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isEnemyMultiEditing = false;

    /// <summary>
    /// All editing instances of the Enemy class.
    /// </summary>
    private List<TDS_Enemy> enemies = new List<TDS_Enemy>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    /// <summary>
    /// Draw the Editor of the enemy's components and references
    /// Display an object field for the anemy's agent 
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        TDS_EditorUtility.ObjectField("Agent", "Custom NavMesh Agent of the enemy. \n Used to navigate", agent, typeof(CustomNavMeshAgent));
        GUILayout.Space(3);
    }

    void DrawEnemyEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the enemy class settings
        if (TDS_EditorUtility.Button("Enemy", "Wrap / unwrap Character class settings", TDS_EditorUtility.HeaderStyle)) IsEnemyUnfolded = !isEnemyUnfolded;
        if(isEnemyUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Damageable script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Character class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreEnemyComponentsUnfolded = !areEnemyComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areEnemyComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Character class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreEnemySettingsUnfolded = !areEnemySettingsUnfolded;

            // If unfolded, draws the custom editor for the sttings
            if (areEnemySettingsUnfolded)
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
    /// Draw the editor for the variables settings of the enemy
    /// </summary>
    private void DrawSettings()
    {
        if(EditorApplication.isPlaying)
        {
            GUILayout.Box($"ENEMY STATE: {(EnemyState)enemyState.enumValueIndex}", TDS_EditorUtility.HeaderStyle);
        }

        // Draw a header for the enemy detection settings 
        EditorGUILayout.LabelField("Detection", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.FloatSlider("Detection range", "The maximum distance of the field of view of the enemy", detectionRange, 1,25);
        GUILayout.Space(3);

        //Draw a header for the enemy down settings
        EditorGUILayout.LabelField("Damages Settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.Toggle("Can be grounded", "Is the enemy can be grounded", canBeDown);
        GUILayout.Space(3);

        // Draws a header for the enemy attacks settings
        EditorGUILayout.LabelField("Attacks", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Attacks", "All Attacks this enemy can cast", attacks);
        GUILayout.Space(2);
        TDS_EditorUtility.Toggle("Can Throw", "Is the enemy can throw objects", canThrow); 

        GUILayout.Space(3);
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => enemies.Add((TDS_Enemy)t));
        if (targets.Length == 1) isEnemyMultiEditing = false;
        else isEnemyMultiEditing = true;

        //Get the serialized properties from the serializedObject
        agent = serializedObject.FindProperty("agent");

        canBeDown = serializedObject.FindProperty("canBeDown");
        canThrow = serializedObject.FindProperty("canThrow");
        detectionRange = serializedObject.FindProperty("detectionRange");
        attacks = serializedObject.FindProperty("attacks");
        enemyState = serializedObject.FindProperty("enemyState"); 

        //Load the editor folded and unfolded values of this class
        isEnemyUnfolded = EditorPrefs.GetBool("isEnemyUnfolded", isEnemyUnfolded);
        areEnemyComponentsUnfolded = EditorPrefs.GetBool("areEnemyComponentsUnfolded", areEnemyComponentsUnfolded);
        areEnemySettingsUnfolded = EditorPrefs.GetBool("areEnemySettingsUnfolded", areEnemySettingsUnfolded);
    }

    // Implement this method to make a custom inspector
    public override void OnInspectorGUI()
    {
        //Draw The inspector for the enemy class
        DrawEnemyEditor(); 

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
