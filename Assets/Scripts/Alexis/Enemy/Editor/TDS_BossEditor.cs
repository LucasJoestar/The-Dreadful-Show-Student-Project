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
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        damagesThreshold = serializedObject.FindProperty("damagesThreshold"); 
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
