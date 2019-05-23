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
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.recoilDistance"/> of type <see cref="float"/>.</summary>
    private SerializedProperty recoilDistance = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.recoilDistanceDeath"/> of type <see cref="float"/>.</summary>
    private SerializedProperty recoilDistanceDeath = null;
    /// <summary>SerializedProperty for <see cref="TDS_Enemy.recoilTimeDeath"/> of type <see cref="float"/>.</summary>
    private SerializedProperty recoilTimeDeath = null;
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
    void AddAttack(string _typeString)
    {
        if(_typeString == typeof(TDS_EnemyAttack).ToString())
        {
            attacks.InsertArrayElementAtIndex(attacks.arraySize - 1);
            serializedObject.ApplyModifiedProperties(); 
            (serializedObject.targetObject as TDS_Enemy).Attacks[attacks.arraySize - 1] = new TDS_EnemyAttack(); 
        }
        else if(_typeString == typeof(TDS_ThrowingAttackBehaviour).ToString())
        {
            attacks.InsertArrayElementAtIndex(attacks.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
            (serializedObject.targetObject as TDS_Enemy).Attacks[attacks.arraySize - 1] = new TDS_ThrowingAttackBehaviour();

        }
        else if(_typeString == typeof(TDS_SpinningAttackBehaviour).ToString())
        {
            attacks.InsertArrayElementAtIndex(attacks.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
            (serializedObject.targetObject as TDS_Enemy).Attacks[attacks.arraySize - 1] = new TDS_SpinningAttackBehaviour();
        }
        else
        {
            Debug.Log("Cancel"); 
        }
    }


    private void DrawAttacks()
    {

        EditorGUILayout.BeginVertical("Box");

        //Draw a header for the enemy evolution settings
        EditorGUILayout.LabelField("Attack Settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Attacks", "", attacks);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("Box");

        if (TDS_EditorUtility.Button("Add new Attack", "Add a new attack to this enemy's attack list", TDS_EditorUtility.HeaderStyle))
        {
            GenericMenu _menu = new GenericMenu();
            GenericMenu.MenuFunction _func = new GenericMenu.MenuFunction(() => AddAttack(typeof(TDS_EnemyAttack).ToString()) ); 
            _menu.AddItem(new GUIContent("Normal Attack"), false, _func);

            _func = new GenericMenu.MenuFunction(() => AddAttack(typeof(TDS_ThrowingAttackBehaviour).ToString()));
            _menu.AddItem(new GUIContent("Throwing Attack"), false, _func);

            _func = new GenericMenu.MenuFunction(() => AddAttack(typeof(TDS_SpinningAttackBehaviour).ToString()));
            _menu.AddItem(new GUIContent("Spinning Attack"), false, _func);

            _func = new GenericMenu.MenuFunction(() => Debug.Log("Cancel") );
            _menu.AddItem(new GUIContent("Cancel"), true, _func);
             
            _menu.ShowAsContext(); 
        }

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

    void DrawEnemyEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the enemy class settings
        if (TDS_EditorUtility.Button( serializedObject.targetObject.name , "Wrap / unwrap Character class settings", TDS_EditorUtility.HeaderStyle)) IsEnemyUnfolded = !isEnemyUnfolded;
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
        EditorGUILayout.LabelField("Recoil", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.FloatSlider("Recoil Distance", "The distance the enemy has to be pushed when they're hit", recoilDistance, .1f, 1);
        TDS_EditorUtility.FloatSlider("Recoil Distance on death", "The distance the enemy has to be pushed when they die", recoilDistanceDeath, .1f, 5);
        TDS_EditorUtility.FloatSlider("Recoil Duration on death", "The time during the enemy is pushed when dying", recoilTimeDeath, .01f, 1);

        GUILayout.Space(3);

        // Draw a header for the enemy detection settings 
        EditorGUILayout.LabelField("Detection", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.FloatSlider("Detection range", "The maximum distance of the field of view of the enemy", detectionRange, 1, 25);
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
        recoilDistance = serializedObject.FindProperty("recoilDistance");
        recoilDistanceDeath = serializedObject.FindProperty("recoilDistanceDeath");
        recoilTimeDeath = serializedObject.FindProperty("recoilTimeDeath");

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
        //Draw The inspector for the enemy class
        DrawEnemyEditor(); 

        base.OnInspectorGUI();
    }

    // Implement this method to draw Handles 
    protected virtual void OnSceneGUI()
    {
        if (Selection.activeGameObject == null) return;
        Vector3 _pos = Selection.activeGameObject.transform.position;
        for (int i = 0; i < attacks.arraySize; i++)
        {
            SerializedProperty _attack = attacks.GetArrayElementAtIndex(i);
            switch (_attack.FindPropertyRelative("AnimationID").intValue)
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
                default:
                    Handles.color = Color.white;
                    break;
            }
            Handles.DrawWireDisc(_pos, Vector3.up, _attack.FindPropertyRelative("maxRange").floatValue);
        }

        Handles.color = Color.cyan;
        if (canThrow.boolValue)
        {
            Handles.DrawWireDisc((serializedObject.targetObject as TDS_Enemy).transform.position, Vector3.up, throwRange.floatValue);
        }
    }

    #endregion

    #endregion
}
