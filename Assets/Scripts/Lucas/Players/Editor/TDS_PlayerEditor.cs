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
     *	Date :			[21 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the dPadXAxis, dPadYAxis, rightStickXAxis & rightStickYAxis fields.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[12 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the projectilePreviewArrow & ProjectilePreviewEndZone fields.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the isDodging, isMoving & isParrying fields ; and the arePlayerDebugsUnfolded field & property.
     *	    - Added the DrawDebugs method.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Moved the aimAngle & throwAimingPoint fields to the TDS_CharacterEditor class.
     *	    - Added the lineRenderer, parryButton & throwPreviewPrecision fields.
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
    /// <summary>SerializedProperties for <see cref="TDS_Player.Summoner"/> of type <see cref="TDS_Summoner"/>.</summary>
    private SerializedProperty summoner = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.interactionBox"/> of type <see cref="TDS_PlayerInteractionBox"/>.</summary>
    private SerializedProperty interactionBox = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.groundDetectionBox"/> of type <see cref="TDS_VirtualBox"/>.</summary>
    private SerializedProperty groundDetectionBox = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.fxTransformPV"/> of type <see cref="PhotonView"/>.</summary>
    private SerializedProperty fxTransformPV = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperties for <see cref="TDS_Player.attacks"/> of type <see cref="List{TDS_Attack}"/>.</summary>
    private SerializedProperty attacks = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isDodging"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isDodging = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isGrounded"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isGrounded = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isJumping"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isJumping = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isMoving"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isMoving = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isParrying"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isParrying = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.IsPlayable"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isPlayable = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.isPreparingAttack"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isPreparingAttack = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.comboCurrent"/> of type <see cref="List{bool}"/>.</summary>
    private SerializedProperty comboCurrent = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.comboResetTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty comboResetTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.JumpForce"/> of type <see cref="float"/>.</summary>
    private SerializedProperty jumpForce = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.jumpMaximumTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty jumpMaximumTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.comboMax"/> of type <see cref="int"/>.</summary>
    private SerializedProperty comboMax = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.WhatIsObstacle"/> of type <see cref="LayerMask"/>.</summary>
    private SerializedProperty whatIsObstacle = null;

    /// <summary>SerializedProperties for <see cref="TDS_Player.playerType"/> of type <see cref="PlayerType"/>.</summary>
    private SerializedProperty playerType = null;
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

    /// <summary>Backing fields for <see cref="ArePlayerDebugsUnfolded"/></summary>
    private bool arePlayerDebugsUnfolded = false;

    /// <summary>
    /// Indicates if the debugs of the editing scripts are unfolded.
    /// </summary>
    public bool ArePlayerDebugsUnfolded
    {
        get { return arePlayerDebugsUnfolded; }
        set
        {
            arePlayerDebugsUnfolded = value;

            // Save this value
            EditorPrefs.SetBool("arePlayerDebugsUnfolded", value);
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
    /// All editing TDS_Player classes.
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

        TDS_EditorUtility.ObjectField("Interaction Box", "Trigger used to detect the available interactions of the player", interactionBox, typeof(TDS_PlayerInteractionBox));
        TDS_EditorUtility.ObjectField("FX Transform", "Transform used to spawn all kind of FXs", fxTransformPV, typeof(PhotonView));
        TDS_EditorUtility.ObjectField("Summoner object", "The Summoner the player is actually wearing", summoner, typeof(TDS_Summoner));
    }

    /// <summary>
    /// Draws the debugs editor of the TDS_Player editing objects.
    /// </summary>
    private void DrawDebugs()
    {
        TDS_EditorUtility.RadioToggle("Grounded", "Is this player on ground or not", isGrounded);
        TDS_EditorUtility.RadioToggle("Moving", "Is this player currently moving or not", isMoving);
        TDS_EditorUtility.RadioToggle("Jumping", "Is this player currently jumping or not", isJumping);
        TDS_EditorUtility.RadioToggle("Dodging", "Is this player currently dodging or not", isDodging);
        TDS_EditorUtility.RadioToggle("Preparing Attack", "Is this player currently preparing an attack", isPreparingAttack);
        TDS_EditorUtility.RadioToggle("Parrying", "Is this player currently parrying or not", isParrying);
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

            // If in play mode, draws the debugs of this class
            if (EditorApplication.isPlaying)
            {
                GUILayout.Space(15);
                EditorGUILayout.BeginVertical("Box");

                // Button to show or not the Player class debugs
                if (TDS_EditorUtility.Button("Debugs", "Wrap / unwrap debugs", TDS_EditorUtility.HeaderStyle)) ArePlayerDebugsUnfolded = !arePlayerDebugsUnfolded;

                // If unfolded, draws the custom editor for the debugs
                if (arePlayerDebugsUnfolded)
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
            string _combo = string.Empty;
            for (int _i = 0; _i < comboCurrent.arraySize; _i++)
            {
                _combo += comboCurrent.GetArrayElementAtIndex(_i).boolValue ? "L" : "H";
            } 

            TDS_EditorUtility.ProgressBar(25, (float)comboCurrent.arraySize / comboMax.intValue, $"Combo : {comboCurrent.arraySize} | {_combo}");

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

        // Draws a header for the player special settings
        EditorGUILayout.LabelField("Special", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        TDS_EditorUtility.Toggle("Playable", "Is the player playable or not", isPlayable);

        // Draws a header for the player jump settings
        EditorGUILayout.LabelField("Jump", TDS_EditorUtility.HeaderStyle);

        GUILayout.Space(3);

        TDS_EditorUtility.FloatField("Jump Force", "Maximum amount of attacks in one combo", jumpForce);

         if (TDS_EditorUtility.FloatField("Jump Maximum Time Length", "Maximum time for a jump the player can continue to add force", jumpMaximumTime))
         {
            players.ForEach(p => p.JumpMaximumTime = jumpMaximumTime.floatValue);
            serializedObject.Update();
         }

        if (EditorApplication.isPlaying && (playerType.intValue != (int)PlayerType.Juggler))
        {
            // Draws a header for the player aim settings
            EditorGUILayout.LabelField("Aim", TDS_EditorUtility.HeaderStyle);

            GUILayout.Space(3);

            if (TDS_EditorUtility.FloatSlider("Aiming Angle", "Angle used by this player to aim for a throw", aimAngle, 15f, 60f))
            {
                players.ForEach(p => p.AimAngle = aimAngle.floatValue);
                serializedObject.Update();
            }
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
        summoner = serializedObject.FindProperty("Summoner");
        interactionBox = serializedObject.FindProperty("interactionBox");
        groundDetectionBox = serializedObject.FindProperty("groundDetectionBox");
        fxTransformPV = serializedObject.FindProperty("fxTransformPV");

        attacks = serializedObject.FindProperty("attacks");
        isDodging = serializedObject.FindProperty("isDodging");
        isGrounded = serializedObject.FindProperty("isGrounded");
        isJumping = serializedObject.FindProperty("isJumping");
        isMoving = serializedObject.FindProperty("isMoving");
        isParrying = serializedObject.FindProperty("isParrying");
        isPlayable = serializedObject.FindProperty("IsPlayable");
        isPreparingAttack = serializedObject.FindProperty("isPreparingAttack");
        comboCurrent = serializedObject.FindProperty("comboCurrent");
        comboResetTime = serializedObject.FindProperty("comboResetTime");
        jumpForce = serializedObject.FindProperty("JumpForce");
        jumpMaximumTime = serializedObject.FindProperty("jumpMaximumTime");
        comboMax = serializedObject.FindProperty("comboMax");
        whatIsObstacle = serializedObject.FindProperty("WhatIsObstacle");
        playerType = serializedObject.FindProperty("playerType");

        // Loads the editor folded & unfolded values of this script
        isPlayerUnfolded = EditorPrefs.GetBool("isPlayerUnfolded", isPlayerUnfolded);
        arePlayerComponentsUnfolded = EditorPrefs.GetBool("arePlayerComponentsUnfolded", arePlayerComponentsUnfolded);
        arePlayerDebugsUnfolded = EditorPrefs.GetBool("arePlayerDebugsUnfolded", arePlayerDebugsUnfolded);
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
