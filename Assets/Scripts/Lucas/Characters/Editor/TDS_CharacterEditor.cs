﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Character), true), CanEditMultipleObjects]
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
	 *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Character editor class.
     *	
     *	    - Added 
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

    /// <summary>SerializedProperty for <see cref="TDS_Character.Throwable"/> of type <see cref="TDS_Throwable"/>.</summary>
    private SerializedProperty throwable = null;
    #endregion

    #region Variables
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
    #endregion

    #endregion

    #region Foldouts
    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    private static bool areCharaComponentsUnfolded = false;

    /// <summary>
    /// Are the settings of the Character class unfolded for the editor ?
    /// </summary>
    private static bool areCharaSettingsUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Character class is unfolded or not.
    /// </summary>
    private static bool isCharaUnfolded = false;
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
    /// Draws the editor for the editing Character classes
    /// </summary>
    protected void DrawCharacterEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Character class settings
        if (TDS_EditorUtility.Button("Character", "Wrap / unwrap Character class settings", TDS_EditorUtility.HeaderStyle)) isCharaUnfolded = !isCharaUnfolded;

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
            if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) areCharaComponentsUnfolded = !areCharaComponentsUnfolded;

            // If unfolded, draws the custom editor for the Components & References
            if (areCharaComponentsUnfolded)
            {
                DrawComponentsAndReferences();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical("Box");

            // Button to show or not the Character class settings
            if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) areCharaSettingsUnfolded = !areCharaSettingsUnfolded;

            // If unfolded, draws the custom editor for the sttings
            if (areCharaSettingsUnfolded)
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
    /// Draws the editor for the Character class components & references
    /// </summary>
    private void DrawComponentsAndReferences()
    {
        // If playing, display editor for the health bar & the throwable
        if (EditorApplication.isPlaying)
        {
            TDS_EditorUtility.ObjectField("Health Bar", "HitBox of this character, used to detect what they touch when attacking", healthBar, typeof(UnityEngine.UI.Image));

            GUILayout.Space(3);

            TDS_EditorUtility.ObjectField("Throwable", "Throwable this character is actually wearing", throwable, typeof(TDS_Throwable));

            GUILayout.Space(5);
        }

        TDS_EditorUtility.ObjectField("Hit Box", "HitBox of this character, used to detect what they touch when attacking", hitBox, typeof(TDS_HitBox));
        TDS_EditorUtility.ObjectField("Rigidbody", "Rigidbody of this character, used for physic simulation", rigidbody, typeof(Rigidbody));

        GUILayout.Space(3);
    }

    /// <summary>
    /// Draws the editor for the Character class settings
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
        throwable = serializedObject.FindProperty("Throwable");

        isFacingRight = serializedObject.FindProperty("isFacingRight");
        isPacific = serializedObject.FindProperty("IsPacific");
        isParalyzed = serializedObject.FindProperty("IsParalyzed");
        speedAccelerationTime = serializedObject.FindProperty("speedAccelerationTime");
        speedCoef = serializedObject.FindProperty("speedCoef");
        speedCurrent = serializedObject.FindProperty("speedCurrent");
        speedInitial = serializedObject.FindProperty("speedInitial");
        speedMax = serializedObject.FindProperty("speedMax");
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
