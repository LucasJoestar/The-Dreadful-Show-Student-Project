using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#pragma warning disable 0414
[CustomEditor(typeof(TDS_Juggler), true), CanEditMultipleObjects]
public class TDS_JugglerEditor : TDS_PlayerEditor 
{
    /* TDS_JugglerEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor of the TDS_Juggler class allowing to use properties, methods and a cool presentation for this script.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[20 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the juggleSpeed field.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[19 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Added the throwables, throwableDistanceFromCenter, maxThrowableAmount & selectedThrowableIndex fields.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_JugglerEditor class.
     *	
     *	    - Added the jugglers & isJugglerMultiEditing fields ; and the areJugglerComponentsUnfolded, areJugglerSettingsUnfolded & isJugglerUnfolded fields & properties.
     *	    - Added the DrawComponentsAndReferences, DrawJugglerEditor & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #region Components & References
    /// <summary>SerializedProperties for <see cref="TDS_Player.throwPreviewZone"/> of type <see cref="GameObject"/>.</summary>
    private SerializedProperty throwPreviewZone = null;

    /// <summary>SerializedProperty for <see cref="TDS_Juggler.Throwables"/> of type <see cref="List{T}"/><see cref="TDS_Throwable"/>.</summary>
    private SerializedProperty throwables = null;

    /// <summary>SerializedProperties for <see cref="TDS_Juggler.juggleTransform"/> of type <see cref="Transform"/>.</summary>
    private SerializedProperty juggleTransform = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperties for <see cref="TDS_Player.isAiming"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isAiming = null;

    /// <summary>SerializedProperty for <see cref="TDS_Juggler.JuggleSpeed"/> of type <see cref="float"/>.</summary>
    private SerializedProperty juggleSpeed = null;

    /// <summary>SerializedProperty for <see cref="TDS_Juggler.ThrowableDistanceFromCenter"/> of type <see cref="float"/>.</summary>
    private SerializedProperty throwableDistanceFromCenter = null;

    /// <summary>SerializedProperty for <see cref="TDS_Juggler.MaxThrowableAmount"/> of type <see cref="int"/>.</summary>
    private SerializedProperty maxThrowableAmount = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.throwPreviewPrecision"/> of type <see cref="int"/>.</summary>
    private SerializedProperty throwPreviewPrecision = null;

    /// <summary>SerializedProperties for <see cref="TDS_Juggler.whatCanAim"/> of type <see cref="LayerMask"/>.</summary>
    private SerializedProperty whatCanAim = null;

    /// <summary>SerializedProperties for <see cref="TDS_Juggler.juggleTransformIdealLocalPosition"/> of type <see cref="Vector3"/>.</summary>
    private SerializedProperty juggleTransformIdealLocalPosition = null;
    #endregion

    #endregion

    #region Foldouts
    /// <summary>Backing field for <see cref="AreJugglerComponentsUnfolded"/>.</summary>
    private bool areJugglerComponentsUnfolded = false;

    /// <summary>
    /// Indicates if the components & references editor of this class is unfolded.
    /// </summary>
    public bool AreJugglerComponentsUnfolded
    {
        get { return areJugglerComponentsUnfolded; }
        set
        {
            areJugglerComponentsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areJugglerComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreJugglerDebugsUnfolded"/>.</summary>
    private bool areJugglerDebugsUnfolded = false;

    /// <summary>
    /// Indicates if the debugs editor of this class is unfolded.
    /// </summary>
    public bool AreJugglerDebugsUnfolded
    {
        get { return areJugglerDebugsUnfolded; }
        set
        {
            areJugglerDebugsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areJugglerDebugsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreJugglerSettingsUnfolded"/>.</summary>
    private bool areJugglerSettingsUnfolded = false;

    /// <summary>
    /// Indicates if the settings editor of this class is unfolded.
    /// </summary>
    public bool AreJugglerSettingsUnfolded
    {
        get { return areJugglerSettingsUnfolded; }
        set
        {
            areJugglerSettingsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("areJugglerSettingsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsJugglerUnfolded"/>.</summary>
    private bool isJugglerUnfolded = false;

    /// <summary>
    /// Indicates if the editor of this class is unfolded.
    /// </summary>
    public bool IsJugglerUnfolded
    {
        get { return isJugglerUnfolded; }
        set
        {
            isJugglerUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("isJugglerUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_Juggler classes.
    /// </summary>
    private List<TDS_Juggler> jugglers = new List<TDS_Juggler>();

    /// <summary>
    /// Indicates if currently editing multiple instances of this class.
    /// </summary>
    private bool isJugglerMultiEditing = false;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the custom editor for the components & references of this class.
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        if (EditorApplication.isPlaying)
        {
            TDS_EditorUtility.PropertyField("Throwables in Hands", "All throwables currently juggling with", throwables);

            GUILayout.Space(5);

            if (TDS_EditorUtility.ObjectField("Selected Throwable", "Currently selected throwable to use", throwable, typeof(TDS_Throwable)))
            {
                jugglers.ForEach(j => j.Throwable = (TDS_Throwable)throwable.objectReferenceValue);
                serializedObject.Update();
            }

            GUILayout.Space(5);
        }

        GUILayout.Space(5);

        TDS_EditorUtility.ObjectField("Juggle Transform", "Juggle transform, where to set as children objects juggling with", juggleTransform, typeof(Transform));
        TDS_EditorUtility.ObjectField("Throw Preview Zone", "Zone at the end of the projectile preview used for feedback value", throwPreviewZone, typeof(GameObject));
    }

    /// <summary>
    /// Draws the debugs editor of the TDS_Juggler editing objects.
    /// </summary>
    private void DrawDebugs()
    {
        TDS_EditorUtility.RadioToggle("Aiming", "Is the Juggler currently aiming for a throw or not", isAiming);
    }

    /// <summary>
    /// Draws the custom editor of the TDS_Juggler class.
    /// </summary>
    protected void DrawJugglerEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Juggler class settings
        if (TDS_EditorUtility.Button("Juggler", "Wrap / unwrap Juggler class settings", TDS_EditorUtility.HeaderStyle)) IsJugglerUnfolded = !isJugglerUnfolded;

        // If unfolded, draws the custom editor for the Juggler class
        if (isJugglerUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Juggler script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Juggler class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreJugglerComponentsUnfolded = !areJugglerComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areJugglerComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Juggler class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreJugglerSettingsUnfolded = !areJugglerSettingsUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (areJugglerSettingsUnfolded)
            {
                DrawSettings();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Juggler class debugs
            if (TDS_EditorUtility.Button("Debugs", "Wrap / unwrap debugs", TDS_EditorUtility.HeaderStyle)) AreJugglerDebugsUnfolded = !areJugglerDebugsUnfolded;

            // If unfolded, draws the custom editor for the debugs
            if (areJugglerDebugsUnfolded)
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
        if (TDS_EditorUtility.IntField("Maximum Throwable(s)", "Maximum amount of throwables hte Juggler can juggle with", maxThrowableAmount))
        {
            jugglers.ForEach(j => j.MaxThrowableAmount = maxThrowableAmount.intValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.FloatField("Juggle Speed", "Speed at which the Juggler juggle with his objects", juggleSpeed))
        {
            jugglers.ForEach(j => j.JuggleSpeed = juggleSpeed.floatValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.FloatField("Throw. Dist. from Center", "Distance of each throwables from the hands transform of the character", throwableDistanceFromCenter))
        {
            jugglers.ForEach(j => j.ThrowableDistanceFromCenter = throwableDistanceFromCenter.floatValue);
            serializedObject.Update();
        }

        GUILayout.Space(5);

        TDS_EditorUtility.Vector3Field("Juggle Transf. Ideal Pos.", "Position the juggle transform is always looking to have (in local space)", juggleTransformIdealLocalPosition);

        // Draws a header for the juggler aim settings
        EditorGUILayout.LabelField("Aim", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        TDS_EditorUtility.PropertyField("What Can Aim", "Indicates what the player can aim at", whatCanAim);

        GUILayout.Space(2);

        if (EditorApplication.isPlaying)
        {
            if (TDS_EditorUtility.FloatSlider("Aiming Angle", "Angle used by the Juggler to aim for a throw", aimAngle, 15f, 60f))
            {
                jugglers.ForEach(j => j.AimAngle = aimAngle.floatValue);
                jugglers.ForEach(j => j.ThrowAimingPoint = j.ThrowAimingPoint);
                serializedObject.Update();
            }

            if (TDS_EditorUtility.Vector3Field("Throw Aiming Point", "Position to aim when preparing a throw (in local space)", throwAimingPoint))
            {
                jugglers.ForEach(j => j.ThrowAimingPoint = throwAimingPoint.vector3Value);
            }
        }

        if (TDS_EditorUtility.IntField("Projectile Preview Precision", "Amount of points used to draw previews of the projectile trajectory", throwPreviewPrecision))
        {
            jugglers.ForEach(j => j.ThrowPreviewPrecision = throwPreviewPrecision.intValue);
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
        targets.ToList().ForEach(t => jugglers.Add((TDS_Juggler)t));
        if (targets.Length == 1) isJugglerMultiEditing = false;
        else isJugglerMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        throwPreviewZone = serializedObject.FindProperty("throwPreviewZone");
        juggleTransform = serializedObject.FindProperty("juggleTransform");

        isAiming = serializedObject.FindProperty("isAiming");
        throwables = serializedObject.FindProperty("Throwables");
        juggleSpeed = serializedObject.FindProperty("juggleSpeed");
        throwableDistanceFromCenter = serializedObject.FindProperty("throwableDistanceFromCenter");
        maxThrowableAmount = serializedObject.FindProperty("maxThrowableAmount");
        throwPreviewPrecision = serializedObject.FindProperty("throwPreviewPrecision");
        whatCanAim = serializedObject.FindProperty("whatCanAim");
        juggleTransformIdealLocalPosition = serializedObject.FindProperty("juggleTransformIdealLocalPosition");

        // Loads the editor folded & unfolded values of this script
        isJugglerUnfolded = EditorPrefs.GetBool("isJugglerUnfolded", isJugglerUnfolded);
        areJugglerDebugsUnfolded = EditorPrefs.GetBool("areJugglerDebugsUnfolded", areJugglerDebugsUnfolded);
        areJugglerComponentsUnfolded = EditorPrefs.GetBool("areJugglerComponentsUnfolded", areJugglerComponentsUnfolded);
        areJugglerSettingsUnfolded = EditorPrefs.GetBool("areJugglerSettingsUnfolded", areJugglerSettingsUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        DrawJugglerEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
