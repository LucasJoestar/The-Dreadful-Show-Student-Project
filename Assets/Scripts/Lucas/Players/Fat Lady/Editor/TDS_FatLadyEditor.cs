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
    /// <summary>SerializedProperties for <see cref="TDS_FatLady.isBerserk"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isBerserk = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.berserkHealthStep"/> of type <see cref="int"/>.</summary>
    private SerializedProperty berserkHealthStep = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.foodCurrent"/> of type <see cref="int"/>.</summary>
    private SerializedProperty foodCurrent = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.foodHealValue"/> of type <see cref="int"/>.</summary>
    private SerializedProperty foodHealValue = null;

    /// <summary>SerializedProperties for <see cref="TDS_FatLady.foodMax"/> of type <see cref="int"/>.</summary>
    private SerializedProperty foodMax = null;
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
            if (TDS_EditorUtility.Toggle("Berserk", "Indicates if the Fat Lady is in Berserk or in \"Pacific\" mode", isBerserk))
            {
                fatLadies.ForEach(f => f.IsBerserk = isBerserk.boolValue);
                serializedObject.Update();
            }

            GUILayout.Space(5);
        }

        if (TDS_EditorUtility.IntField("Berserk Health Step", "Health value separating the Berserk mode and the Pacific one", berserkHealthStep))
        {
            fatLadies.ForEach(f => f.BerserkHealthStep = berserkHealthStep.intValue);
            serializedObject.Update();
        }
        if (TDS_EditorUtility.IntSlider("Current Food", "Amount of food the Fat Lady is currently carrying on", foodCurrent, 0, foodMax.intValue))
        {
            fatLadies.ForEach(f => f.FoodCurrent = foodCurrent.intValue);
            serializedObject.Update();
        }

        GUILayout.Space(3);
        TDS_EditorUtility.ProgressBar(25, (float)foodCurrent.intValue / foodMax.intValue, "Food");
        GUILayout.Space(3);

        if (TDS_EditorUtility.IntField("Max Food", "Maximum amount of food the Fat Lady can carry", foodMax))
        {
            fatLadies.ForEach(f => f.FoodMax = foodMax.intValue);
            serializedObject.Update();
        }
        if (TDS_EditorUtility.IntField("Food Heal Value", "Heal value when eating some food", foodHealValue))
        {
            fatLadies.ForEach(f => f.FoodHealValue = foodHealValue.intValue);
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
        targets.ToList().ForEach(t => fatLadies.Add((TDS_FatLady)t));
        if (targets.Length == 1) isFatLadyMultiEditing = false;
        else isFatLadyMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        isBerserk = serializedObject.FindProperty("isBerserk");
        berserkHealthStep = serializedObject.FindProperty("berserkHealthStep");
        foodCurrent = serializedObject.FindProperty("foodCurrent");
        foodHealValue = serializedObject.FindProperty("foodHealValue");
        foodMax = serializedObject.FindProperty("foodMax");

        // Loads the editor folded & unfolded values of this script
        isFatLadyUnfolded = EditorPrefs.GetBool("isFatLadyUnfolded", isFatLadyUnfolded);
        areFatLadyComponentsUnfolded = EditorPrefs.GetBool("areFatLadyComponentsUnfolded", areFatLadyComponentsUnfolded);
        areFatLadySettingsUnfolded = EditorPrefs.GetBool("areFatLadySettingsUnfolded", areFatLadySettingsUnfolded);
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
