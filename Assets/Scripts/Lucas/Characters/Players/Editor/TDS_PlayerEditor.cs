using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_Player), false), CanEditMultipleObjects]
public class TDS_PlayerEditor : TDS_CharacterEditor 
{
    /* TDS_PlayerEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Custom editor for the TDS_Player class.
     *	    
     *	    Allows to use properties & methods in the inspector.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Moved the aimAngle & throwAimingPoint fields to the TDS_CharacterEditor class.
     *	    - Added the lineRenderer & paradeButton fields.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[04 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the cancelThrowButton, isAiming, aimAngle & throwAimingPoint fields.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[29 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_PlayerEditor class.
     *	
     *	    - Added the summoner, interactionsDetector, groundDetectionBox, attacks, isGrounded, isJumping, comboCurrent, comboMax, comboResetTime, jumpForce, jumpMaximumTime, whatIsObstacle, playerTyp, catchButton, dodgeButton, heavyAttackButton, horizontalAxis, interactButton, jumpButton, lightAttackButton, superAttackButton, throwButton, useObjectButton, verticalAxis, players & isPlayerMultiEditing fields ; and the arePlayerComponentsUnfolded, arePlayerInputsUnfolded, arePlayerVariableUnfolded & isPlayerUnfolded fields & properties.
     *	    - Added the DrawComponentsAndReferences, DrawInputs, DrawPlayerEditor & DrawSettings methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties

    #region Components & References
    /// <summary>SerializedProperties for <see cref="TDS_Player.lineRenderer"/> of type <see cref="LineRenderer"/>.</summary>
    private SerializedProperty lineRenderer = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.Summoner"/> of type <see cref="TDS_Summoner"/>.</summary>
    private SerializedProperty summoner = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.interactionsDetector"/> of type <see cref="TDS_Trigger"/>.</summary>
    private SerializedProperty interactionsDetector = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.groundDetectionBox"/> of type <see cref="TDS_VirtualBox"/>.</summary>
    private SerializedProperty groundDetectionBox = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperties for <see cref="TDS_Player.attacks"/> of type <see cref="List{TDS_Attack}"/>.</summary>
    private SerializedProperty attacks = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isAiming"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isAiming = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isGrounded"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isGrounded = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isJumping"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isJumping = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.comboCurrent"/> of type <see cref="List{bool}"/>.</summary>
    private SerializedProperty comboCurrent = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.comboMax"/> of type <see cref="int"/>.</summary>
    private SerializedProperty comboMax = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.comboResetTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty comboResetTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.JumpForce"/> of type <see cref="float"/>.</summary>
    private SerializedProperty jumpForce = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.jumpMaximumTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty jumpMaximumTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.WhatIsObstacle"/> of type <see cref="LayerMask"/>.</summary>
    private SerializedProperty whatIsObstacle = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.playerType"/> of type <see cref="PlayerType"/>.</summary>
    private SerializedProperty playerType = null;
    #endregion

    #region Inputs
    /// <summary>SerializedProperties for <see cref="TDS_Player.CatchButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty catchButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.CancelThrowButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty cancelThrowButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.DodgeButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty dodgeButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.HeavyAttackButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty heavyAttackButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.HorizontalAxis"/> of type <see cref="string"/>.</summary>
    private SerializedProperty horizontalAxis = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.InteractButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty interactButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.JumpButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty jumpButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.LightAttackButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty lightAttackButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.Parade"/> of type <see cref="string"/>.</summary>
    private SerializedProperty paradeButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.SuperAttackButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty superAttackButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.ThrowButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty throwButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.UseObjectButton"/> of type <see cref="string"/>.</summary>
    private SerializedProperty useObjectButton = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.VerticalAxis"/> of type <see cref="string"/>.</summary>
    private SerializedProperty verticalAxis = null;
    #endregion

    #endregion

    #region Foldouts
    /// <summary>Backing fields for <see cref="ArePlayerComponentsUnfolded"/></summary>
    private bool arePlayerComponentsUnfolded = false;

    /// <summary>
    /// Indicates if the components & references of the editing scripts are unfolded.
    /// </summary>
    public bool ArePlayerComponentsUnfolded
    {
        get { return arePlayerComponentsUnfolded; }
        set
        {
            arePlayerComponentsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("arePlayerComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="ArePlayerInputsUnfolded"/></summary>
    private bool arePlayerInputsUnfolded = false;

    /// <summary>
    /// Indicates if the inputs of the editing scripts are unfolded.
    /// </summary>
    public bool ArePlayerInputsUnfolded
    {
        get { return arePlayerInputsUnfolded; }
        set
        {
            arePlayerInputsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("arePlayerInputsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="ArePlayerComponentsUnfolded"/></summary>
    private bool arePlayerVariablesUnfolded = false;

    /// <summary>
    /// Indicates if the variables of the editing scripts are unfolded.
    /// </summary>
    public bool ArePlayerVariablesUnfolded
    {
        get { return arePlayerVariablesUnfolded; }
        set
        {
            arePlayerVariablesUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("arePlayerVariablesUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="IsPlayerUnfolded"/></summary>
    private bool isPlayerUnfolded = false;

    /// <summary>
    /// Indicates if the editor of this class is unfolded.
    /// </summary>
    public bool IsPlayerUnfolded
    {
        get { return isPlayerUnfolded; }
        set
        {
            isPlayerUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("isPlayerUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_Player class.
    /// </summary>
    private List<TDS_Player> players = new List<TDS_Player>();

    /// <summary>
    /// Indicates if currently editing multiple instances of the class.
    /// </summary>
    private bool isPlayerMultiEditing = false;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the components & references editor of the TDS_Player editing objects.
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        TDS_EditorUtility.PropertyField("Ground detection Box", "Virtual box used to detect if the player is on ground or not", groundDetectionBox);

        GUILayout.Space(5);

        TDS_EditorUtility.ObjectField("Interaction detection Trigger", "Trigger used to detect the available interactions of the player", interactionsDetector, typeof(TDS_Trigger));
        TDS_EditorUtility.ObjectField("Line Renderer", "Line Renderer used to draw the preparing throw preview", lineRenderer, typeof(LineRenderer));
        TDS_EditorUtility.ObjectField("Summoner object", "The Summoner the player is actually wearing", summoner, typeof(TDS_Summoner));
    }

    /// <summary>
    /// Draws the inputs editor of the TDS_Player editing objects.
    /// </summary>
    private void DrawInputs()
    {
        TDS_EditorUtility.TextField("Horizontal Axis", "Name of the axis input used to move in horizontal", horizontalAxis);
        TDS_EditorUtility.TextField("Vertical Axis", "Name of the axis input used to move in vertical", verticalAxis);

        GUILayout.Space(10);

        TDS_EditorUtility.TextField("Jump", "Name of the button input used to perform a jump", jumpButton);
        TDS_EditorUtility.TextField("Interact", "Name of the button input used to interact with the environment", interactButton);
        TDS_EditorUtility.TextField("Use Object", "Name of the button input used to use an object", useObjectButton);
        TDS_EditorUtility.TextField("Throw", "Name of the button input used to throw an object", throwButton);
        TDS_EditorUtility.TextField("Cancel Throw", "Name of the button input used to cancel a throw", cancelThrowButton);

        GUILayout.Space(10);

        TDS_EditorUtility.TextField("Light Attack", "Name of the button input used to perform a Light Attack", lightAttackButton);
        TDS_EditorUtility.TextField("Heavy Attack", "Name of the button input used to perform a Heavy Attack", heavyAttackButton);
        TDS_EditorUtility.TextField("Super Attack", "Name of the button input used to perform a Super Attack", superAttackButton);
        TDS_EditorUtility.TextField("Catch", "Name of the button input used to perform the \"Catch\" Action", catchButton);
        TDS_EditorUtility.TextField("Dodge", "Name of the button input used to perform the \"Dodge\" Action", dodgeButton);
        TDS_EditorUtility.TextField("Parade", "Name of the button input used to parry attacks", paradeButton);
    }

    /// <summary>
    /// Draws the custom editor of the TDS_Player class.
    /// </summary>
    protected void DrawPlayerEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Player class settings
        if (TDS_EditorUtility.Button("Player", "Wrap / unwrap Player class settings", TDS_EditorUtility.HeaderStyle)) IsPlayerUnfolded = !isPlayerUnfolded;

        // If unfolded, draws the custom editor for the Player class
        if (isPlayerUnfolded)
        {
            // Records any changements on the editing objects to allow undo
            Undo.RecordObjects(targets, "Player script(s) settings");

            // Updates the SerializedProperties to get the latest values
            serializedObject.Update();

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Player class components
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) ArePlayerComponentsUnfolded = !arePlayerComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (arePlayerComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Player class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) ArePlayerVariablesUnfolded = !arePlayerVariablesUnfolded;

            // If unfolded, draws the custom editor for the settings
            if (arePlayerVariablesUnfolded)
            {
                DrawSettings();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Player class inputs
            if (TDS_EditorUtility.Button("Inputs", "Wrap / unwrap inputs", TDS_EditorUtility.HeaderStyle)) ArePlayerInputsUnfolded = !arePlayerInputsUnfolded;

            // If unfolded, draws the custom editor for the inputs
            if (arePlayerInputsUnfolded)
            {
                DrawInputs();
            }

            EditorGUILayout.EndVertical();

            // Applies all modified properties on the SerializedObjects
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
    }

    /// <summary>
    /// Draws the settings editor of the TDS_Player editing objects.
    /// </summary>
    private void DrawSettings()
    {
        TDS_EditorUtility.PropertyField("Type of Player", "Type of character this player is", playerType);
        TDS_EditorUtility.PropertyField("Attacks", "All Attacks this player can perform", attacks);

        GUILayout.Space(5);

        TDS_EditorUtility.PropertyField("What is Obstacle", "What layers this player should collide on", whatIsObstacle);

        // Draws a header for the player combos settings
        EditorGUILayout.LabelField("Combo", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        // Draws informations about the current combo if in play mode
        if (EditorApplication.isPlaying && !isPlayerMultiEditing)
        {
            TDS_EditorUtility.ProgressBar(25, (float)comboCurrent.arraySize / comboMax.intValue, "Combo : " + comboCurrent.arraySize);

            GUILayout.Space(5);
        }

        if (TDS_EditorUtility.IntField("Max Combo", "Maximum amount of attacks in one combo", comboMax))
        {
            players.ForEach(p => p.ComboMax = comboMax.intValue);
            serializedObject.Update();
        }

        if (TDS_EditorUtility.FloatField("Combo Reset time", "Time it takes for a combo to automatically be reset when no attack is perform", comboResetTime))
        {
            players.ForEach(p => p.ComboResetTime = comboResetTime.floatValue);
            serializedObject.Update();
        }

        // Draws a header for the player jump settings
        EditorGUILayout.LabelField("Jump", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        TDS_EditorUtility.FloatField("Jump Force", "Maximum amount of attacks in one combo", jumpForce);

         if (TDS_EditorUtility.FloatField("Jump Maximum Time Length", "Maximum time for a jump the player can continue to add force", jumpMaximumTime))
        {
            players.ForEach(p => p.JumpMaximumTime = jumpMaximumTime.floatValue);
            serializedObject.Update();
        }

        // Draws debug informations if in play mode
        if (EditorApplication.isPlaying)
        {
            // Draws a header for the debugs
            EditorGUILayout.LabelField("Debug", TDS_EditorUtility.HeaderStyle);

            GUILayout.Space(3);

            TDS_EditorUtility.Toggle("Grounded", "Is this player on ground or not", isGrounded, false);
            TDS_EditorUtility.Toggle("Jumping", "Is this player currently jumping or not", isJumping, false);
            TDS_EditorUtility.Toggle("Aiming", "Is this player currently aiming for a throw or not", isAiming, false);
        }
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the target editing scripts
        targets.ToList().ForEach(t => players.Add((TDS_Player)t));
        if (targets.Length == 1) isPlayerMultiEditing = false;
        else isPlayerMultiEditing = true;

        // Get the serializedProperties from the serializedObject
        lineRenderer = serializedObject.FindProperty("lineRenderer");
        summoner = serializedObject.FindProperty("Summoner");
        interactionsDetector = serializedObject.FindProperty("interactionsDetector");
        groundDetectionBox = serializedObject.FindProperty("groundDetectionBox");

        attacks = serializedObject.FindProperty("attacks");
        isAiming = serializedObject.FindProperty("isAiming");
        isGrounded = serializedObject.FindProperty("isGrounded");
        isJumping = serializedObject.FindProperty("isJumping");
        comboCurrent = serializedObject.FindProperty("comboCurrent");
        comboMax = serializedObject.FindProperty("comboMax");
        comboResetTime = serializedObject.FindProperty("comboResetTime");
        jumpForce = serializedObject.FindProperty("JumpForce");
        jumpMaximumTime = serializedObject.FindProperty("jumpMaximumTime");
        whatIsObstacle = serializedObject.FindProperty("WhatIsObstacle");
        playerType = serializedObject.FindProperty("playerType");

        catchButton = serializedObject.FindProperty("CatchButton");
        cancelThrowButton = serializedObject.FindProperty("CancelThrowButton");
        dodgeButton = serializedObject.FindProperty("DodgeButton");
        heavyAttackButton = serializedObject.FindProperty("HeavyAttackButton");
        horizontalAxis = serializedObject.FindProperty("HorizontalAxis");
        interactButton = serializedObject.FindProperty("InteractButton");
        jumpButton = serializedObject.FindProperty("JumpButton");
        lightAttackButton = serializedObject.FindProperty("LightAttackButton");
        paradeButton = serializedObject.FindProperty("ParadeButton");
        superAttackButton = serializedObject.FindProperty("SuperAttackButton");
        throwButton = serializedObject.FindProperty("ThrowButton");
        useObjectButton = serializedObject.FindProperty("UseObjectButton");
        verticalAxis = serializedObject.FindProperty("VerticalAxis");

        // Loads the editor folded & unfolded values of this script
        isPlayerUnfolded = EditorPrefs.GetBool("isPlayerUnfolded", isPlayerUnfolded);
        arePlayerComponentsUnfolded = EditorPrefs.GetBool("arePlayerComponentsUnfolded", arePlayerComponentsUnfolded);
        arePlayerInputsUnfolded = EditorPrefs.GetBool("arePlayerInputsUnfolded", arePlayerInputsUnfolded);
        arePlayerVariablesUnfolded = EditorPrefs.GetBool("arePlayerVariablesUnfolded", arePlayerVariablesUnfolded);
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Draws the custom editor of the editing scripts
        DrawPlayerEditor();

        base.OnInspectorGUI();
    }
    #endregion

    #endregion
}
