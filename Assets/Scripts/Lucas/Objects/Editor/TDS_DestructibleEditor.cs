using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0414, 0642
[CustomEditor(typeof(TDS_Destructible), true), CanEditMultipleObjects]
public class TDS_DestructibleEditor : TDS_DamageableEditor
{
    /* TDS_DestructibleEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Custom editor for the TDS_Destructible class.
     *  
     *      Allows to use properties & methods in the inspector.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[29 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TDS_DestructibleEditor class.
	*/

    #region Fields / Properties

    #region SerializedProperties
    /// <summary>SerializedProperty for <see cref="TDS_Destructible.loot"/> of type <see cref="GameObject"/>[].</summary>
    private SerializedProperty loot = null;

    /// <summary>SerializedProperty for <see cref="TDS_Destructible.lootChance"/> of type <see cref="int"/>.</summary>
    private SerializedProperty lootChance = null;

    /// <summary>SerializedProperty for <see cref="TDS_Destructible.lootMax"/> of type <see cref="int"/>.</summary>
    private SerializedProperty lootMax = null;

    /// <summary>SerializedProperty for <see cref="TDS_Destructible.lootMin"/> of type <see cref="int"/>.</summary>
    private SerializedProperty lootMin = null;
    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreDestrComponentsUnfolded"/></summary>
    private bool areDestrComponentsUnfolded = false;

    /// <summary>
    /// Are the components of the Destructible class unfolded for the editor ?
    /// </summary>
    public bool AreDestrComponentsUnfolded
    {
        get { return areDestrComponentsUnfolded; }
        set
        {
            areDestrComponentsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areDestrComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreDestrSettingsUnfolded"/></summary>
    private bool areDestrSettingsUnfolded = false;

    /// <summary>
    /// Are the settings of the Destructible class unfolded for the editor ?
    /// </summary>
    public bool AreDestrSettingsUnfolded
    {
        get { return areDestrSettingsUnfolded; }
        set
        {
            areDestrSettingsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areDestrSettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsDestrUnfolded"/>.</summary>
    private bool isDestrUnfolded = true;

    /// <summary>
    /// Indicates if the editor for the Destructible class is unfolded or not.
    /// </summary>
    public bool IsDestrUnfolded
    {
        get { return isDestrUnfolded; }
        set
        {
            isDestrUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isDestrUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isDestrMultiEditing = false;

    /// <summary>
    /// All editing instances of the Destructible class.
    /// </summary>
    private List<TDS_Destructible> destructibles = new List<TDS_Destructible>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor for the Destructible class components & references.
    /// </summary>
    protected void DrawComponentsAndReferences()
    {
        TDS_EditorUtility.PropertyField("Loot", "All available loot for this destructible ; note that once a loot has been dropped, it cannot be dropped again, so add it again if you want multiple instances of it to drop", loot);
    }

    /// <summary>
    /// Draws the editor for the editing Destructible classes
    /// </summary>
    protected void DrawDestructibleEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Damageable class settings
        if (TDS_EditorUtility.Button("Destructible", "Wrap / unwrap Destructible class settings", TDS_EditorUtility.HeaderStyle)) IsDestrUnfolded = !isDestrUnfolded;

        // If unfolded, draws the custom editor for the Destructible class
        if (isDestrUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Destructible script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Destructible class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreDestrComponentsUnfolded = !areDestrComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areDestrComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Destructible class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreDestrSettingsUnfolded = !areDestrSettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areDestrSettingsUnfolded)
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
    /// Draws the editor for the Destructible class settings.
    /// </summary>
    protected void DrawSettings()
    {
        TDS_EditorUtility.IntSlider("Loot Chance", "Chance in percentage to have drop on this destructible destruction", lootChance, 0, 100);

        GUILayout.Space(3);

        TDS_EditorUtility.IntSlider("Min Loot", "Minimum amount of loot for this destructible", lootMin, 0, lootMax.intValue);
        if (TDS_EditorUtility.IntField("Max Loot", "Maximum amount of loot for this destructible", lootMax))
        {
            destructibles.ForEach(d => d.LootMax = lootMax.intValue);
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
        targets.ToList().ForEach(t => destructibles.Add((TDS_Destructible)t));
        if (targets.Length == 1) isDestrMultiEditing = false;
        else isDestrMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        loot = serializedObject.FindProperty("loot");
        lootChance = serializedObject.FindProperty("lootChance");
        lootMax = serializedObject.FindProperty("lootMax");
        lootMin = serializedObject.FindProperty("lootMin");
        
        // Loads the editor folded and unfolded values of this class
        isDestrUnfolded = EditorPrefs.GetBool("isDestrUnfolded", isDestrUnfolded);
        areDestrComponentsUnfolded = EditorPrefs.GetBool("areDestrComponentsUnfolded", areDestrComponentsUnfolded);
        areDestrSettingsUnfolded = EditorPrefs.GetBool("areDestrSettingsUnfolded", areDestrSettingsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the inspector for the Damageable class
        DrawDestructibleEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
