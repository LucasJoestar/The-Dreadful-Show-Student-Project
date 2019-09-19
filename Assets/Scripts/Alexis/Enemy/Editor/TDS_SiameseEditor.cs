using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary;

[CustomEditor(typeof(TDS_Siamese))]
public class TDS_SiameseEditor : TDS_BossEditor 
{
    /* TDS_SiameseEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Display the Editor of the Siamese class]
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
	 *	Date :			[16/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the TDS_SiameseEditorClass]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>SerializedProperty for <see cref="TDS_Siamese.spinningSpeed"/> of type <see cref="float"/>.</summary>
    SerializedProperty spinningSpeed = null;
    /// <summary> SerializedProperty for <see cref="TDS_Siamese.splitingEnemiesName"/> of type <see cref="TDS_Enemy[]"/>.</summary>
    SerializedProperty splitingEnemiesNames = null;
    /// <summary>SerializedProperty for <see cref="TDS_Siamese.splitingPosition"/> of type <see cref="Vector3[]"/>. </summary>
    SerializedProperty splitingPosition = null;
    /// <summary>SerializedProperty for <see cref="TDS_Siamese.splittingPortrait"/> of type <see cref="GameObject"/>. </summary>
    SerializedProperty splittingPortrait = null;
    /// <summary>SerializedProperty for <see cref="TDS_Siamese.tornadoClip"/> of type <see cref="AudioClip"/>. </summary>
    SerializedProperty tornadoClip = null;

    /// <summary>Backing field for <see cref="AreSplittingSettingsUnfolded"/></summary>
    private bool areSplittingSettingsUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Character class is unfolded or not.
    /// </summary>
    public bool AreSplittingSettingsUnfolded
    {
        get { return areSplittingSettingsUnfolded; }
        set
        {
            areSplittingSettingsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areSplittingSettingsUnfolded", value);
        }
    }
    #endregion

    #region Methods
    /*
    protected void DisplaySettings()
    {
        EditorGUILayout.Space();

        GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
        EditorGUILayout.BeginVertical("HelpBox");

        //Draw a header for the enemy spinning settings
        EditorGUILayout.LabelField("Spinning Settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Spinning Speed", "Speed of the agent during the spinning attack", spinningSpeed);


        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();


        serializedObject.ApplyModifiedProperties();

    }
    */

    protected override void DrawEnemyEditor()
    {
        base.DrawEnemyEditor();

        DrawSplittingSettings(); 
    }

    protected override void DrawSettings()
    {
        base.DrawSettings();
        TDS_EditorUtility.PropertyField("Tornado clip", "Clip played during tornado animation", tornadoClip);
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draw the split settings
    /// </summary>
    private void DrawSplittingSettings()
    {
        if(IsEnemyUnfolded)
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("Box");

            //Draw a header for the enemy spinning settings
            if (TDS_EditorUtility.Button("Splitting Enemies", "Wrap / unwrap splitting enemies", TDS_EditorUtility.HeaderStyle)) AreSplittingSettingsUnfolded = !areSplittingSettingsUnfolded;
            if (areSplittingSettingsUnfolded)
            {
                EditorGUILayout.Space();
                for (int i = 0; i < splitingEnemiesNames.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(splitingEnemiesNames.GetArrayElementAtIndex(i).stringValue, TDS_EditorUtility.HeaderStyle);
                    GUITools.ActionButton("X", RemoveSettingsAtIndex, i, Color.white, Color.black, "Remove this spliting enemy");
                    Repaint();
                    serializedObject.ApplyModifiedProperties(); 
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();

                TDS_Enemy _e = null;
                _e = EditorGUILayout.ObjectField("Add Splitting Enemy", _e, typeof(TDS_Enemy), false) as TDS_Enemy;
                if (_e != null)
                {
                    splitingEnemiesNames.InsertArrayElementAtIndex(0);
                    splitingEnemiesNames.GetArrayElementAtIndex(0).stringValue = _e.EnemyName;
                    splitingPosition.InsertArrayElementAtIndex(0);
                    splitingPosition.GetArrayElementAtIndex(0).vector3Value = Vector3.forward;
                    Repaint();
                }
                EditorGUILayout.Space(); 

                TDS_EditorUtility.ObjectField("Splitting Portrait", "Portrait of the splitting enemies", splittingPortrait, typeof(GameObject)); 
                serializedObject.ApplyModifiedProperties();

            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
       
    }

    private void RemoveSettingsAtIndex(int _index)
    {
        splitingEnemiesNames.DeleteArrayElementAtIndex(_index);
        splitingPosition.DeleteArrayElementAtIndex(_index); 
    }

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        spinningSpeed = serializedObject.FindProperty("spinningSpeed");
        splitingEnemiesNames = serializedObject.FindProperty("splitingEnemiesNames");
        splitingPosition = serializedObject.FindProperty("splitingPosition");
        splittingPortrait = serializedObject.FindProperty("splittingPortrait");
        tornadoClip = serializedObject.FindProperty("tornadoClip"); 
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();

        Vector3 _objectPosition = (serializedObject.targetObject as TDS_Siamese).gameObject.transform.position;

        for (int i = 0; i < splitingEnemiesNames.arraySize; i++)
        {
            SerializedProperty _p = splitingPosition.GetArrayElementAtIndex(i);
            Handles.Label(_objectPosition + _p.vector3Value + Vector3.up, splitingEnemiesNames.GetArrayElementAtIndex(i).stringValue);
            Vector3 _pos = _objectPosition + _p.vector3Value; 
            _pos = Handles.PositionHandle(_pos, Quaternion.identity);
            _p.vector3Value = (_pos - _objectPosition); 
        }
        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #endregion
}
