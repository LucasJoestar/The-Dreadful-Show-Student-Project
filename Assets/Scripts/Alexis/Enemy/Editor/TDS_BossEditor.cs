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
    protected SerializedProperty damagesThreshold = null;
    protected SerializedProperty portrait = null;

    protected SerializedProperty onTwoThirdsHealth = null;
    protected SerializedProperty onHalfHealth = null;
    protected SerializedProperty onOneThirdHealth = null; 

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
    protected override void DrawSettings()
    {
        base.DrawSettings();

        TDS_EditorUtility.IntSlider("Damages Threshold", "How much damages had to be taken to play the hit animation", damagesThreshold, 1, 50);

        TDS_EditorUtility.ObjectField("Boss Portrait", "Portrait to display next to the boss' lifebar", portrait, typeof(GameObject));

        EditorGUILayout.LabelField("Health events", TDS_EditorUtility.HeaderStyle); 
        TDS_EditorUtility.PropertyField("On Two Thirds Health", "Event called when the boss has reached two thirds of his life", onTwoThirdsHealth);
        TDS_EditorUtility.PropertyField("On Half Health", "Event called when the boss has reached half of his life", onHalfHealth);
        TDS_EditorUtility.PropertyField("On One Third Health", "Event called when the boss has reached one third of his life", onOneThirdHealth);
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        damagesThreshold = serializedObject.FindProperty("damagesThreshold");
        portrait = serializedObject.FindProperty("portrait");
        onTwoThirdsHealth = serializedObject.FindProperty("onTwoThirdsHealth");
        onHalfHealth = serializedObject.FindProperty("onHalfHealth");
        onOneThirdHealth = serializedObject.FindProperty("onOneThirdHealth");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
    }
    #endregion

    #endregion
}
