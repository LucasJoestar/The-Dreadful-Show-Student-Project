using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(TDS_Cat))]
public class TDS_CatEditor : TDS_CharacterEditor 
{
    /* TDS_CatEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    private SerializedProperty catState = null;
    private SerializedProperty leftPerchInfos = null;
    private SerializedProperty rightPerchInfos = null;
    private SerializedProperty catAttack = null;
    private SerializedProperty chargeRate = null; 
    #endregion

    #region Methods

    #region Original Methods
    private void DrawCatSettings()
    {
        EditorGUILayout.LabelField(serializedObject.targetObject.name, TDS_EditorUtility.HeaderStyle); 
        TDS_EditorUtility.PropertyField("Cat State", "On which perch the cat is", catState);

        TDS_EditorUtility.PropertyField("Left Perch Infos", "Left perch infos", leftPerchInfos);
        TDS_EditorUtility.PropertyField("Right Perch Infos", "Left perch infos", rightPerchInfos);

        TDS_EditorUtility.PropertyField("Cat Attack", "Attack casted by the cat", catAttack);
        TDS_EditorUtility.FloatSlider("Charge Rate", "Rate using when the cat is independant", chargeRate, 5, 25); 
        serializedObject.ApplyModifiedProperties(); 
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        catState = serializedObject.FindProperty("catState");
        leftPerchInfos = serializedObject.FindProperty("leftPerchInfos");
        rightPerchInfos = serializedObject.FindProperty("rightPerchInfos");
        catAttack = serializedObject.FindProperty("catAttack");
        chargeRate = serializedObject.FindProperty("chargeRate"); 
    }

    public override void OnInspectorGUI()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        //Draw The inspector for the enemy class
        DrawCatSettings();

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;

        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        leftPerchInfos.FindPropertyRelative("perchPosition").vector3Value = Handles.PositionHandle(leftPerchInfos.FindPropertyRelative("perchPosition").vector3Value, Quaternion.identity);
        Handles.Label(leftPerchInfos.FindPropertyRelative("perchPosition").vector3Value, "Left Perch Position");

        leftPerchInfos.FindPropertyRelative("landingPosition").vector3Value = Handles.PositionHandle(leftPerchInfos.FindPropertyRelative("landingPosition").vector3Value, Quaternion.identity);
        Handles.Label(leftPerchInfos.FindPropertyRelative("landingPosition").vector3Value, "Left landing Position");

        rightPerchInfos.FindPropertyRelative("perchPosition").vector3Value = Handles.PositionHandle(rightPerchInfos.FindPropertyRelative("perchPosition").vector3Value, Quaternion.identity);
        Handles.Label(rightPerchInfos.FindPropertyRelative("perchPosition").vector3Value, "Right Perch Position");

        rightPerchInfos.FindPropertyRelative("landingPosition").vector3Value = Handles.PositionHandle(rightPerchInfos.FindPropertyRelative("landingPosition").vector3Value, Quaternion.identity);
        Handles.Label(rightPerchInfos.FindPropertyRelative("landingPosition").vector3Value, "Right landing Position");

        serializedObject.ApplyModifiedProperties();

    }
    #endregion

    #endregion
}
