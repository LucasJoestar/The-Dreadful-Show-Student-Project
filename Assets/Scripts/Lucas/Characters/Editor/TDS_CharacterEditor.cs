using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_Character), false), CanEditMultipleObjects]
public class TDS_CharacterEditor : TDS_DamageableEditor 
{
    /* TDS_CharacterEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Custom editor for the TDS_Character class.
     *	    
     *	    Allows to use properties & methods in the inspector.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[12 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the handsTransform, throwBonusDamagesMax & throwBonusDamagesMin fields.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the isAttacking field ; and the areCharaDebugsUnfolded field & property.
     *	    - Added the DrawDebugs method.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the aimAngle & throwAimingPoint fields.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[29 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the AreCharaComponentsUnfolded, AreCharaSettingsUnfolded & IsCharaUnfolded properties.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_CharacterEditor class.
     *	
     *	    - Added the hitBox, healthBar, rigidbody, throwable, isFacingRight, isPacific, isParalyzed, speedAccelerationTime, speedCoef, speedCurrent, speedInitial, speedMax, areCharaComponentsUnfolded, areCharaSettingsUnfolded, isCharaUnfolded, isCharaMultiEditing & characters fields.
     *	    - Added the DrawCharacterEditor, DrawComponentsAndReferences & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #region Components & References
    /// <summary>SerializedProperty for <see cref="TDS_Character.hitBox"/> of type <see cref="TDS_HitBox"/>.</summary>
    private SerializedProperty hitBox = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.healthBar"/> of type <see cref="UnityEngine.UI.Image"/>.</summary>
    private SerializedProperty healthBar = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.rigidbody"/> of type <see cref="Rigidbody"/>.</summary>
    private SerializedProperty rigidbody = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.throwable"/> of type <see cref="TDS_Throwable"/>.</summary>
    protected SerializedProperty throwable = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.handsTransform"/> of type <see cref="Transform"/>.</summary>
    private SerializedProperty handsTransform = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperty for <see cref="TDS_Character.isAttacking"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isAttacking = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.isFacingRight"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isFacingRight = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.IsPacific"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isPacific = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.IsParalyzed"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isParalyzed = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.speedAccelerationTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedAccelerationTime = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.speedCoef"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedCoef = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.speedCurrent"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedCurrent = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.speedInitial"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedInitial = null;

    /// <summary>SerializedProperty for <see cref="TDS_Character.speedMax"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedMax = null;

    /// <summary>SerializedProperties for <see cref="TDS_Character.aimAngle"/> of type <see cref="float"/>.</summary>
    protected SerializedProperty aimAngle = null;

    /// <summary>SerializedProperties for <see cref="TDS_Character.throwBonusDamagesMax"/> of type <see cref="int"/>.</summary>
    protected SerializedProperty throwBonusDamagesMax = null;

    /// <summary>SerializedProperties for <see cref="TDS_Character.throwBonusDamagesMin"/> of type <see cref="int"/>.</summary>
    protected SerializedProperty throwBonusDamagesMin = null;

    /// <summary>SerializedProperties for <see cref="TDS_Character.throwAimingPoint"/> of type <see cref="Vector3"/>.</summary>
    protected SerializedProperty throwAimingPoint = null;
    #endregion

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreCharaComponentsUnfolded"/></summary>
    private bool areCharaComponentsUnfolded = false;

    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreCharaComponentsUnfolded
    {
        get { return areCharaComponentsUnfolded; }
        set
        {
            areCharaComponentsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areCharaComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreCharaDebugsUnfolded"/></summary>
    private bool areCharaDebugsUnfolded = false;

    /// <summary>
    /// Are the debugs of the Character class unfolded for editor ?
    /// </summary>
    public bool AreCharaDebugsUnfolded
    {
        get { return areCharaDebugsUnfolded; }
        set
        {
            areCharaDebugsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areCharaDebugsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreCharaSettingsUnfolded"/></summary>
    private bool areCharaSettingsUnfolded = false;

    /// <summary>
    /// Are the settings of the Character class unfolded for the editor ?
    /// </summary>
    public bool AreCharaSettingsUnfolded
    {
        get { return areCharaSettingsUnfolded; }
        set
        {
            areCharaSettingsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areCharaSettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsCharaUnfolded"/></summary>
    private bool isCharaUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Character class is unfolded or not.
    /// </summary>
    public bool IsCharaUnfolded
    {
        get { return isCharaUnfolded; }
        set
        {
            isCharaUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isCharaUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isCharaMultiEditing = false;

    /// <summary>
    /// All editing instances of the Character class.
    /// </summary>
    private List<TDS_Character> characters = new List<TDS_Character>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor for the editing Character classes.
    /// </summary>
    protected void DrawCharacterEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Character class settings
        if (TDS_EditorUtility.Button("Character", "Wrap / unwrap Character class settings", TDS_EditorUtility.HeaderStyle)) IsCharaUnfolded = !isCharaUnfolded;

        // If unfolded, draws the custom editor for the Character class
        if (isCharaUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Damageable script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Character class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreCharaComponentsUnfolded = !areCharaComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areCharaComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Character class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreCharaSettingsUnfolded = !areCharaSettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areCharaSettingsUnfolded)
            {
                DrawSettings();
            }

            EditorGUILayout.EndVertical();

            // If on play mode, draws debugs of the class
            if (EditorApplication.isPlaying)
            {
                GUILayout.Space(15);
                EditorGUILayout.BeginVertical("Box");

                // Button to show or not the Character class debugs
                if (TDS_EditorUtility.Button("Debugs", "Wrap / unwrap debugs", TDS_EditorUtility.HeaderStyle)) AreCharaDebugsUnfolded = !areCharaDebugsUnfolded;

                // If unfolded, draws the custom editor for the debugs
                if (areCharaDebugsUnfolded)
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
    /// Draws the editor for the Character class components & references.
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        // If playing, display editor for the health bar & the throwable
        if (EditorApplication.isPlaying)
        {
            TDS_EditorUtility.ObjectField("Health Bar", "HitBox of this character, used to detect what they touch when attacking", healthBar, typeof(UnityEngine.UI.Image));

            GUILayout.Space(3);

            if (!targets.Any(t => t is TDS_Juggler) && TDS_EditorUtility.ObjectField("Throwable", "Throwable this character is actually wearing", throwable, typeof(TDS_Throwable)))
            {
                {
                    characters.ForEach(c => c.GrabObject((TDS_Throwable)throwable.objectReferenceValue));
                    serializedObject.Update();
                }
            }

            GUILayout.Space(5);
        }

        TDS_EditorUtility.ObjectField("Hit Box", "HitBox of this character, used to detect what they touch when attacking", hitBox, typeof(TDS_HitBox));
        TDS_EditorUtility.ObjectField("Rigidbody", "Rigidbody of this character, used for physic simulation", rigidbody, typeof(Rigidbody));
        TDS_EditorUtility.ObjectField("Hands Transform", "Transform at the position of the character hands ; mainly used as root for carrying throwable", handsTransform, typeof(Transform));

        GUILayout.Space(3);
    }

    /// <summary>
    /// Draws the editor for the Character class debug elements.
    /// </summary>
    private void DrawDebugs()
    {
        GUILayout.Space(3);

        TDS_EditorUtility.RadioToggle("Attacking", "Is this character currently attacking or not", isAttacking);
    }

    /// <summary>
    /// Draws the editor for the Character class settings.
    /// </summary>
    private void DrawSettings()
    {
        // Draws a header for the character behaviour settings
        EditorGUILayout.LabelField("Behaviour", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        TDS_EditorUtility.Toggle("Pacific", "When pacific, the character will not attack", isPacific);

        TDS_EditorUtility.Toggle("Paralyzed", "When paralyzed, the character will no move", isParalyzed);

        // When on play and setting the toggle, do not change the property but execute the Flip method instead
        if (EditorApplication.isPlaying)
        {
            if (!isCharaMultiEditing && TDS_EditorUtility.Toggle("Facing Right Side", "Indicates if the character is currently facing the right side of the screen, or not", isFacingRight, false))
            {
                characters[0].Flip();
                serializedObject.Update();
            }
        }
        else
        {
            TDS_EditorUtility.Toggle("Facing Right Side", "Indicates if the character is currently facing the right side of the screen, or not", isFacingRight);
        }

        // Draws a header for the character behaviour settings
        EditorGUILayout.LabelField("Speed", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        // When on play and not multi editing, diplay a progress bar representing the current speed of the character
        if (EditorApplication.isPlaying)
        {
            if (!isCharaMultiEditing)
            {
                TDS_EditorUtility.ProgressBar(20, speedCurrent.floatValue / speedMax.floatValue, "Speed");
                GUILayout.Space(5);
            }
        }

        // If the serializedProperty is changed, triggers the property of the field
        // After the property has been used, update the object so that serializedProperties can be refresh
        if (TDS_EditorUtility.FloatSlider("Initial Speed", "Speed of the character when starting moving", speedInitial, 0, speedMax.floatValue))
        {
            characters.ForEach(c => c.SpeedInitial = speedInitial.floatValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.FloatField("Max Speed", "Maximum speed of the character", speedMax))
        {
            characters.ForEach(c => c.SpeedMax = speedMax.floatValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.FloatField("Speed Acceleration Time", "Time that take the character to get its speed to the maximum value, after starting moving (in seconds)", speedAccelerationTime))
        {
            characters.ForEach(c => c.SpeedAccelerationTime = speedAccelerationTime.floatValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.FloatField("Speed Coefficient", "Global coefficient used to multiply all speed values for this character", speedCoef))
        {
            characters.ForEach(c => c.SpeedCoef = speedCoef.floatValue);
            serializedObject.Update();
        }

        if (!EditorApplication.isPlaying)
        {
            // Draws a header for the player aim settings
            EditorGUILayout.LabelField("Throwables & Aiming", TDS_EditorUtility.HeaderStyle);

            GUILayout.Space(3);

            if (TDS_EditorUtility.IntField("Throw max. Bonus Damages", "Maximum amount of bonus damages when throwing an object", throwBonusDamagesMax))
            {
                characters.ForEach(p => p.ThrowBonusDamagesMax = throwBonusDamagesMax.intValue);
                serializedObject.Update();
            }
            if (TDS_EditorUtility.IntSlider("Throw min. Bonus Damages", "Minimum amount of bonus damages when throwing an object", throwBonusDamagesMin, 0, throwBonusDamagesMax.intValue))
            {
                characters.ForEach(p => p.ThrowBonusDamagesMin = throwBonusDamagesMin.intValue);
                serializedObject.Update();
            }

            GUILayout.Space(3);

            if (TDS_EditorUtility.FloatSlider("Aiming Angle", "Angle used by this player to aim for a throw", aimAngle, 0f, 80f))
            {
                characters.ForEach(p => p.AimAngle = aimAngle.floatValue);
                serializedObject.Update();
            }

            TDS_EditorUtility.Vector3Field("Throw Aiming Point", "Position to aim when preparing a throw (Local space)", throwAimingPoint);
        }

        GUILayout.Space(3);
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => characters.Add((TDS_Character)t));
        if (targets.Length == 1) isCharaMultiEditing = false;
        else isCharaMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        hitBox = serializedObject.FindProperty("hitBox");
        healthBar = serializedObject.FindProperty("healthBar");
        rigidbody = serializedObject.FindProperty("rigidbody");
        throwable = serializedObject.FindProperty("throwable");
        handsTransform = serializedObject.FindProperty("handsTransform");

        isAttacking = serializedObject.FindProperty("isAttacking");
        isFacingRight = serializedObject.FindProperty("isFacingRight");
        isPacific = serializedObject.FindProperty("IsPacific");
        isParalyzed = serializedObject.FindProperty("IsParalyzed");
        speedAccelerationTime = serializedObject.FindProperty("speedAccelerationTime");
        speedCoef = serializedObject.FindProperty("speedCoef");
        speedCurrent = serializedObject.FindProperty("speedCurrent");
        speedInitial = serializedObject.FindProperty("speedInitial");
        speedMax = serializedObject.FindProperty("speedMax");
        aimAngle = serializedObject.FindProperty("aimAngle");
        throwBonusDamagesMax = serializedObject.FindProperty("throwBonusDamagesMax");
        throwBonusDamagesMin = serializedObject.FindProperty("throwBonusDamagesMin");
        throwAimingPoint = serializedObject.FindProperty("throwAimingPoint");

        // Loads the editor folded a unfolded values of this class
        isCharaUnfolded = EditorPrefs.GetBool("isCharaUnfolded", isCharaUnfolded);
        areCharaComponentsUnfolded = EditorPrefs.GetBool("areCharaComponentsUnfolded", areCharaComponentsUnfolded);
        areCharaDebugsUnfolded = EditorPrefs.GetBool("areCharaDebugsUnfolded", areCharaDebugsUnfolded);
        areCharaSettingsUnfolded = EditorPrefs.GetBool("areCharaSettingsUnfolded", areCharaSettingsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Draws the inspector for the Character class
        DrawCharacterEditor();
    }
    #endregion

    #endregion
}
