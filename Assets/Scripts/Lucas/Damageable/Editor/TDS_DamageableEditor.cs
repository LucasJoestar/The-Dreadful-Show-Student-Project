using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Damageable), false), CanEditMultipleObjects]
public class TDS_DamageableEditor : Editor 
{
    /* TDS_DamageableEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Custom editor for the TDS_Damageable class.
     *  
     *      Allows to use properties & methods in the inspector.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     * 
     *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Second, pretty complete, version of the editor, and it's cool.
     *	    
     *	    - Added the damageValue & healValue fields.
     *	    - Added the DrawLifeButtons method.
     *	    - Renamed the DrawVariables method in DrawLifeSettings.
	 *
	 *	-----------------------------------
     *	
     *	Date :			[23 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    First incomplete version of the editor, but it's cool.
     *	    
     *	    - Added the animator, collider, sprite, isDead, isIndestructible, isInvulnerable, healthCurrent& healthMax fields ; the areDamagComponentsUnfolded, areDamagVariableUnfolded, isDamageableUnfolded fields ; and the damageable, damageables & isMultiEditing fields.
     *	    - Added the DrawComponentsAndReferences, DrawDamageableEditor & DrawVariables methods.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[22 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_DamageableEditor script.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #region Components & References
    /// <summary>SerializedProperty for <see cref="TDS_Damageable.animator"/> of type <see cref="Animator"/>.</summary>
    private SerializedProperty animator = null;

    /// <summary>SerializedProperty for <see cref="TDS_Damageable.collider"/> of type <see cref="BoxCollider"/>.</summary>
    private SerializedProperty collider = null;

    /// <summary>SerializedProperty for <see cref="TDS_Damageable.sprite"/> of type <see cref="SpriteRenderer"/>.</summary>
    private SerializedProperty sprite = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperty for <see cref="TDS_Damageable.isDead"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isDead = null;

    /// <summary>SerializedProperty for <see cref="TDS_Damageable.isIndestructible"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isIndestructible = null;

    /// <summary>SerializedProperty for <see cref="TDS_Damageable.IsInvulnerable"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isInvulnerable = null;

    /// <summary>SerializedProperty for <see cref="TDS_Damageable.healthCurrent"/> of type <see cref="int"/>.</summary>
    private SerializedProperty healthCurrent = null;

    /// <summary>SerializedProperty for <see cref="TDS_Damageable.healthMax"/> of type <see cref="int"/>.</summary>
    private SerializedProperty healthMax = null;
    #endregion

    #endregion

    #region Foldouts
    /// <summary>
    /// Are the components of the Damageable class unfolded for editor ?
    /// </summary>
    protected bool areDamagComponentsUnfolded = true;

    /// <summary>
    /// Are the life settings of the Damageable class unfolded for editor ?
    /// </summary>
    protected bool areDamagLifeSettingsUnfolded = true;

    /// <summary>
    /// Indicates if the editor for the Damageable class is unfolded or not.
    /// </summary>
    protected bool isDamageableUnfolded = true;
    #endregion

    #region Editor Variables
    /// <summary>
    /// Value used to damage the object
    /// </summary>
    private int damageValue = 0;

    /// <summary>
    /// Value used to heal the object
    /// </summary>
    private int healValue = 0;
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// The editing instance of the class, if only editing one.
    /// </summary>
    protected TDS_Damageable damageable = null;

    /// <summary>
    /// All editing instances of the class, if multi editing.
    /// </summary>
    protected List<TDS_Damageable> damageables = new List<TDS_Damageable>();

    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    protected bool isMultiEditing = false;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor for Damageable class components & references
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        TDS_EditorUtility.ObjectField("Animator", "Animator of this object", animator, typeof(Animator));
        TDS_EditorUtility.ObjectField("Collider", "Non-trigger BoxCollider of this object, used to detect collisions", collider, typeof(BoxCollider));
        TDS_EditorUtility.ObjectField("Sprite", "Main SpriteRenderer used to render this object", sprite, typeof(SpriteRenderer));

        GUILayout.Space(3);
    }

    /// <summary>
    /// Draws the editor for the editing damageable classes
    /// </summary>
    protected void DrawDamageableEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);

        GUI.backgroundColor = new Color(.55f, .55f, .55f);
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Damageable class settings
        if (TDS_EditorUtility.Button("Damageable", "Wrap / unwrap Damageable class settings", TDS_EditorUtility.HeaderStyle)) isDamageableUnfolded = !isDamageableUnfolded;

        // If unfolded, draws the custom editor for the Damageable class
        if (isDamageableUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Damageable script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = new Color(.9f, .9f, .9f);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Damageable class settings
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) areDamagComponentsUnfolded = !areDamagComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areDamagComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Damageable class settings
            if (TDS_EditorUtility.Button("Life", "Wrap / unwrap life settings", TDS_EditorUtility.HeaderStyle)) areDamagLifeSettingsUnfolded = !areDamagLifeSettingsUnfolded;

            // If unfolded, draws the custom editor for the life sttings
            if (areDamagLifeSettingsUnfolded)
            {
                DrawLifeSettings();
            }

            EditorGUILayout.EndVertical();

            // Applies all modified properties on the SerializedObjects
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draws buttons allowing to heal or damage the object
    /// </summary>
    private void DrawLifeButtons()
    {
        // Get the colors & option used for the buttons width
        Color _originalColor = GUI.color;
        Color _originalBackgroundcolor = GUI.backgroundColor;
        Color _newBackgroundColor = new Color(.75f, .75f, .75f);
        GUILayoutOption _buttonWidth = GUILayout.Width(Screen.width / 2.5f);
        GUILayoutOption _fieldWidth = GUILayout.Width(Screen.width / 10f);

        // Draws the heal & damage buttons, with two int fields for the value to use
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(TDS_EditorUtility.LabelStyle.padding.left);

        // Heal button
        GUI.backgroundColor = _newBackgroundColor;
        EditorGUILayout.BeginVertical("Box", _buttonWidth);
        GUI.backgroundColor = _originalColor;
        GUI.color = new Color(.25f, 1, .25f);
        if (GUILayout.Button(new GUIContent("Heal", "Heal this object using the below value"), EditorStyles.miniButton))
        {
            damageables.ForEach(d => d.Heal(healValue));
        }
        GUI.color = _originalColor;

        // Int field to edit the value used to heal the object
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.backgroundColor = _originalColor;
        healValue = EditorGUILayout.IntField(healValue, _fieldWidth);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        // Damage buttpn
        GUI.backgroundColor = _newBackgroundColor;
        EditorGUILayout.BeginVertical("Box", _buttonWidth);
        GUI.backgroundColor = _originalColor;
        GUI.color = new Color(1, .25f, .25f);
        if (GUILayout.Button(new GUIContent("Damage", "Damage this object using the below value"), EditorStyles.miniButton))
        {
            damageables.ForEach(d => d.TakeDamage(damageValue));
        }
        GUI.color = _originalColor;

        // Int field to edit the value used to damage the object
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.backgroundColor = _originalColor;
        damageValue = EditorGUILayout.IntField(damageValue, _fieldWidth);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    /// <summary>
    /// Draws the editor for Damageable class life settings
    /// </summary>
    private void DrawLifeSettings()
    {
        // If the serializedProperty is changed, triggers the property of the field
        // After the property has been used, update the object so that serializedProperties can be refresh
        if (TDS_EditorUtility.IntSlider("Health", "Current health of this object", healthCurrent, 0, healthMax.intValue))
        {
            damageables.ForEach(d => d.HealthCurrent = healthCurrent.intValue);
            serializedObject.Update();
        }

        if (!healthCurrent.hasMultipleDifferentValues)
        {
            GUILayout.Space(5);

            TDS_EditorUtility.ProgressBar(25, (float)healthCurrent.intValue / healthMax.intValue, "Health");

            GUILayout.Space(5);
        }

        // If the application is playing, draws two button next to each other allowing the heal & damage the object
        if (EditorApplication.isPlaying) DrawLifeButtons();

        if (TDS_EditorUtility.IntField("Max Health", "Maximum health of the object ; its health cannot exceed this value", healthMax))
        {
            damageables.ForEach(d => d.HealthMax = healthMax.intValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.Toggle("Dead", "Indicates if the object is dead, or not", isDead))
        {
            damageables.ForEach(d => d.IsDead = isDead.boolValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.Toggle("Indestructible", "When indestructible, the object will not be dead when its health reach zero", isIndestructible))
        {
            damageables.ForEach(d => d.IsIndestructible = isIndestructible.boolValue);
            serializedObject.Update();
        }

        TDS_EditorUtility.Toggle("Invulnerable", "When invulnerable, the object cannot take any damage", isInvulnerable);
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get the target editing scripts
        damageable = (TDS_Damageable)target;
        targets.ToList().ForEach(t => damageables.Add((TDS_Damageable)t));
        if (targets.Length == 1) isMultiEditing = false;
        else isMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        animator = serializedObject.FindProperty("animator");
        collider = serializedObject.FindProperty("collider");
        sprite = serializedObject.FindProperty("sprite");

        isDead = serializedObject.FindProperty("isDead");
        isIndestructible = serializedObject.FindProperty("isIndestructible");
        isInvulnerable = serializedObject.FindProperty("IsInvulnerable");
        healthCurrent = serializedObject.FindProperty("healthCurrent");
        healthMax = serializedObject.FindProperty("healthMax");
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawDamageableEditor();
    }
    #endregion

    #endregion
}
