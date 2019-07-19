using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor;

#pragma warning disable 0414


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
    /// <summary>Backing field for <see cref="AreEnemyAttacksUnfolded"/></summary>
    private bool areEnemyAttacksUnfolded = false;

    /// <summary>
    /// Are the attacks of the enemy class unfolded for editor ?
    /// </summary>
    public bool AreEnemyAttacksUnfolded
    {
        get { return areEnemyAttacksUnfolded; }
        set
        {
            areEnemyAttacksUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areEnemyAttacksUnfolded", value);
        }
    }

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
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.throwRange"/> of type <see cref="bool"/>.</summary>
    protected SerializedProperty throwRange = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.detectionRange"/> of type <see cref="float"/>.</summary>
    protected SerializedProperty detectionRange = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.wanderingRangeMin"/> of type <see cref="float"/>.</summary>
    protected SerializedProperty wanderingRangeMin = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.wanderingRangeMax"/> of type <see cref="float"/>.</summary>
    protected SerializedProperty wanderingRangeMax = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.tauntProbability"/> of type <see cref="float"/>.</summary>
    private SerializedProperty tauntProbability = null; 
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.enemyState"/> of type <see cref="EnemyState"/>.</summary>
    private SerializedProperty enemyState = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.attacks"/> of type <see cref="TDS_EnemyAttack[]"/>.</summary>
    private SerializedProperty attacks = null;

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
    private void DrawAttacks()
    {
        EditorGUILayout.BeginVertical("Box");

        //Draw a header for the enemy evolution settings
        EditorGUILayout.LabelField("Attack Settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Attacks", "", attacks);

        EditorGUILayout.EndVertical();

    }


    /// <summary>
    /// Draw the Editor of the enemy's components and references
    /// Display an object field for the anemy's agent 
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        TDS_EditorUtility.ObjectField("Agent", "Custom NavMesh Agent of the enemy. \n Used to navigate", agent, typeof(CustomNavMeshAgent));
        GUILayout.Space(3);
    }

    protected virtual void DrawEnemyEditor()
    {
        
        // Button to show or not the enemy class settings
        if (TDS_EditorUtility.Button( serializedObject.targetObject.name , "Wrap / unwrap Character class settings", TDS_EditorUtility.HeaderStyle)) IsEnemyUnfolded = !isEnemyUnfolded;
        if(isEnemyUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Damageable script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            if (EditorApplication.isPlaying)
            {
                GUILayout.Box($"ENEMY STATE: {(EnemyState)enemyState.enumValueIndex}", TDS_EditorUtility.HeaderStyle);
            }

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
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical("Box");

            if (TDS_EditorUtility.Button("Attacks", "Wrap / unwrap attacks class settings", TDS_EditorUtility.HeaderStyle)) AreEnemyAttacksUnfolded = !areEnemyAttacksUnfolded;
            if (areEnemyAttacksUnfolded)
            {
                DrawAttacks();
            }

            EditorGUILayout.EndVertical();

            // Applies all modified properties on the SerializedObjects
            serializedObject.ApplyModifiedProperties();
        }


    }

    /// <summary>
    /// Draw the editor for the variables settings of the enemy
    /// </summary>
    protected virtual void DrawSettings()
    {
        // Draw a header for the enemy detection settings 
        EditorGUILayout.LabelField("Detection", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.FloatSlider("Detection range", "The maximum distance of the field of view of the enemy", detectionRange, 1, 25);
        TDS_EditorUtility.FloatSlider("Wandering range Min", "The wandering distance around the targeted player when other enemies attacking an enemy", wanderingRangeMin, 1, wanderingRangeMax.floatValue);
        TDS_EditorUtility.FloatSlider("Wandering range Max", "The wandering distance around the targeted player when other enemies attacking an enemy", wanderingRangeMax, wanderingRangeMin.floatValue, 10);
        TDS_EditorUtility.FloatSlider("Taunt Probability", "The chance to taunt after wandering", tauntProbability, 0, 100);
        GUILayout.Space(3);

        //Draw a header for the enemy down settings
        EditorGUILayout.LabelField("Damages Settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.Toggle("Can be grounded", "Is the enemy can be grounded", canBeDown);
        GUILayout.Space(3);

        // Draws a header for the enemy attacks settings
        TDS_EditorUtility.Toggle("Can Throw", "Is the enemy can throw objects", canThrow); 
        if(canThrow.boolValue)
        {
            TDS_EditorUtility.FloatSlider("Throw Range", "Distance reached by the throwed object", throwRange, .5f, 20f); 
        }

        GUILayout.Space(3);
    }

    private void OnAttackTypeSelected(TDS_EnemyAttack _attack)
    {
        Debug.Log(_attack.GetType().ToString()); 
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
        throwRange = serializedObject.FindProperty("throwRange"); 
        detectionRange = serializedObject.FindProperty("detectionRange");
        wanderingRangeMin = serializedObject.FindProperty("wanderingRangeMin");
        wanderingRangeMax = serializedObject.FindProperty("wanderingRangeMax");
        tauntProbability = serializedObject.FindProperty("tauntProbability"); 

        enemyState = serializedObject.FindProperty("enemyState");

        attacks = serializedObject.FindProperty("attacks"); 

        //Load the editor folded and unfolded values of this class
        isEnemyUnfolded = EditorPrefs.GetBool("isEnemyUnfolded", isEnemyUnfolded);
        areEnemyComponentsUnfolded = EditorPrefs.GetBool("areEnemyComponentsUnfolded", areEnemyComponentsUnfolded);
        areEnemySettingsUnfolded = EditorPrefs.GetBool("areEnemySettingsUnfolded", areEnemySettingsUnfolded);
    }

    // Implement this method to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        //Draw The inspector for the enemy class
        DrawEnemyEditor();

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;

        base.OnInspectorGUI();
    }

    // Implement this method to draw Handles 
    protected virtual void OnSceneGUI()
    {
        if (Selection.activeGameObject == null) return;
        Vector3 _pos = Selection.activeGameObject.transform.position;
        for (int i = 0; i < attacks.arraySize; i++)
        {
            TDS_EnemyAttack _attack = (serializedObject.targetObject as TDS_Enemy).Attacks[i];
            if(_attack == null)
            {
                continue;
            }
            switch (_attack.AnimationID)
            {
                case 6:
                    Handles.color = Color.red;
                    break;
                case 7:
                    Handles.color = Color.green;
                    break;
                case 8:
                    Handles.color = Color.blue;
                    break;
                case 9:
                    Handles.color = Color.magenta; 
                    break;
                default:
                    Handles.color = Color.white;
                    break;
            }
            Handles.DrawWireDisc(_pos, Vector3.up, _attack.MaxRange);
            Handles.DrawWireDisc(_pos, Vector3.up, _attack.MinRange);
            Handles.Label(_pos + new Vector3(_attack.MaxRange, 0), _attack.AttackName); 
        }

        Handles.color = Color.cyan;
        if (canThrow.boolValue)
        {
            Handles.DrawWireDisc((serializedObject.targetObject as TDS_Enemy).transform.position, Vector3.up, throwRange.floatValue);
            Handles.Label(_pos + new Vector3(throwRange.floatValue, 0),"Throw Range");
        }

        Handles.color = Color.yellow;
        Handles.DrawWireDisc((serializedObject.targetObject as TDS_Enemy).transform.position, Vector3.up, detectionRange.floatValue);
        Handles.Label(_pos + new Vector3(detectionRange.floatValue, 0), "Detection Range");

        if(Application.isPlaying)
        {
            Handles.color = Color.grey;
            _pos = (serializedObject.targetObject as TDS_Enemy).PlayerTarget ? (serializedObject.targetObject as TDS_Enemy).PlayerTarget.transform.position : Vector3.zero;
            Handles.DrawWireDisc(_pos, Vector3.up, wanderingRangeMin.floatValue);
            Handles.DrawWireDisc(_pos, Vector3.up, wanderingRangeMax.floatValue);
        }

    }

    #endregion

    #endregion
}
