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
    /// <summary>SerializedProperties for <see cref="TDS_FireEater.isDrunk"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isDrunk = null;

    /// <summary>SerializedProperties for <see cref="TDS_FireEater.drunkSpeedCoef"/> of type <see cref="float"/>.</summary>
    private SerializedProperty drunkSpeedCoef = null;

    /// <summary>SerializedProperties for <see cref="TDS_FireEater.soberUpTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty soberUpTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_FireEater.soberUpTimer"/> of type <see cref="float"/>.</summary>
    private SerializedProperty soberUpTimer = null;

    /// <summary>SerializedProperties for <see cref="TDS_FireEater.drunkJumpForce"/> of type <see cref="int"/>.</summary>
    private SerializedProperty drunkJumpForce = null;
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

    /// <summary>Backing field for <see cref="AreFireEaterDebugsUnfolded"/>.</summary>
    private bool areFireEaterDebugsUnfolded = false;

    /// <summary>
    /// Indicates if the debugs editor of this class is unfolded.
    /// </summary>
    public bool AreFireEaterDebugsUnfolded
    {
        get { return areFireEaterDebugsUnfolded; }
        set
        {
            areFireEaterDebugsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areFireEaterDebugsUnfolded", value);
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
    /// Draws the custom editor for the debugs of this class.
    /// </summary>
    private void DrawDebugs()
    {
        GUILayout.Space(5);
        TDS_EditorUtility.RadioToggle("Drunk", "Indicates if the Fire Eater is drunk", isDrunk);
        GUILayout.Space(5);

        if (isDrunk.boolValue)
        {
            TDS_EditorUtility.ProgressBar(25, soberUpTimer.floatValue / soberUpTime.floatValue, "Drunk");
            GUILayout.Space(5);
        }
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
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Fire Eater class debugs
            if (TDS_EditorUtility.Button("Debugs", "Wrap / unwrap debugs", TDS_EditorUtility.HeaderStyle)) AreFireEaterDebugsUnfolded = !areFireEaterDebugsUnfolded;

            // If unfolded, draws the custom editor for the debugs
            if (areFireEaterDebugsUnfolded)
            {
                DrawDebugs();
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
        if (Application.isPlaying && !isDrunk.boolValue)
        {
            Color _originalColor = GUI.color;
            GUI.color = new Color(.8f, .25f, .25f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);

            if (GUILayout.Button(new GUIContent("Get Drunk", "Get the Fire Eater drunk"), GUILayout.Width(100), GUILayout.Height(20)))
            {
                fireEaters.ForEach(f => f.GetDrunk());
            }

            EditorGUILayout.EndHorizontal();

            GUI.color = _originalColor;
            GUILayout.Space(10);
        }

        

        if (TDS_EditorUtility.FloatField("Sober Up Time", "Time it takes to the Fire Eater to sober up (in seconds)", soberUpTime))
        {
            fireEaters.ForEach(f => f.SoberUpTime = soberUpTime.floatValue);
        }

        if (TDS_EditorUtility.IntField("Drunk Jump Force", "Force applied when performing a jump when drunk", drunkJumpForce))
        {
            fireEaters.ForEach(f => f.DrunkJumpForce = drunkJumpForce.intValue);
        }

        if (TDS_EditorUtility.FloatField("Drunk Speed Coef", "Coefficient applied to speed when drunk", drunkSpeedCoef))
        {
            fireEaters.ForEach(f => f.DrunkSpeedCoef = drunkSpeedCoef.floatValue);
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
        targets.ToList().ForEach(t => fireEaters.Add((TDS_FireEater)t));
        if (targets.Length == 1) isFireEaterMultiEditing = false;
        else isFireEaterMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        isDrunk = serializedObject.FindProperty("isDrunk");
        drunkSpeedCoef = serializedObject.FindProperty("drunkSpeedCoef");
        soberUpTime = serializedObject.FindProperty("soberUpTime");
        soberUpTimer = serializedObject.FindProperty("soberUpTimer");
        drunkJumpForce = serializedObject.FindProperty("drunkJumpForce");

        // Loads the editor folded & unfolded values of this script
        isFireEaterUnfolded = EditorPrefs.GetBool("isFireEaterUnfolded", isFireEaterUnfolded);
        areFireEaterComponentsUnfolded = EditorPrefs.GetBool("areFireEaterComponentsUnfolded", areFireEaterComponentsUnfolded);
        areFireEaterDebugsUnfolded = EditorPrefs.GetBool("areFireEaterDebugsUnfolded", areFireEaterComponentsUnfolded);
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
