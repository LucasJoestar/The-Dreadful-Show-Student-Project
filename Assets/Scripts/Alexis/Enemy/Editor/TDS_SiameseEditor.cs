using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    #endregion

    #region Methods

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
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
