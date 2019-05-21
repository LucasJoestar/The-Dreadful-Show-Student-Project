using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(TDS_Boss))]
public class TDS_BossEditor : TDS_EnemyEditor 
{
    /* TDS_BossEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
     *	    Display Editor for the TDS_Boss Class and its children
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
	 *	[Initialisation of the TDS_BossEditor Class]
	 *
	 *	-----------------------------------
	*/


    #region Fields / Properties
    /// <summary>SerializedProperty for <see cref="TDS_Boss.Attacks"/> of type <see cref="bool"/>.</summary>
    SerializedProperty attacks = null;

    #region FoldOut
    /// <summary>Backing field for <see cref="IsBossUnfolded"/></summary>
    private bool isBossUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Character class is unfolded or not.
    /// </summary>
    public bool IsBossUnfolded
    {
        get { return isBossUnfolded; }
        set
        {
            isBossUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isBossUnfolded", value);
        }
    }
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    protected void DrawBossSettings()
    {
        GUILayout.Space(5);

        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        if (TDS_EditorUtility.Button(serializedObject.targetObject.name, "Wrap / unwrap Punk class settings", TDS_EditorUtility.HeaderStyle)) IsBossUnfolded = !isBossUnfolded;
        if (isBossUnfolded)
        {
            DisplaySettings(); 
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
    }

    protected virtual void DisplaySettings()
    {
        GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
        EditorGUILayout.BeginVertical("HelpBox");

        //Draw a header for the enemy evolution settings
        EditorGUILayout.LabelField("Attack Settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Boss Attacks", "", attacks);

        EditorGUILayout.EndVertical();
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();

        // Get the attacks property
        attacks = serializedObject.FindProperty("attacks");

        //Set the fold out
        isBossUnfolded = EditorPrefs.GetBool("isBossUnfolded", isBossUnfolded);
    }

    public override void OnInspectorGUI()
    {
        DrawBossSettings(); 
        
        base.OnInspectorGUI();
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();

        if (Selection.activeGameObject == null) return;
        Vector3 _pos = Selection.activeGameObject.transform.position;
        for (int i = 0; i < attacks.arraySize; i++)
        {
            SerializedProperty _attack = attacks.GetArrayElementAtIndex(i);
            switch (_attack.FindPropertyRelative("AnimationID").intValue)
            {
                case 6:
                    Handles.color = Color.red;
                    break;
                case 7:
                    Handles.color = Color.green;
                    break;
                case 8:
                    Handles.color = Color.blue;
                    break; 
                case 9:
                    Handles.color = Color.magenta; 
                    break;
                default:
                    Handles.color = Color.white;
                    break;
            }
            Handles.DrawWireDisc(_pos, Vector3.up, _attack.FindPropertyRelative("predictedRange").floatValue);
        }
    }
    #endregion

    #endregion
}
