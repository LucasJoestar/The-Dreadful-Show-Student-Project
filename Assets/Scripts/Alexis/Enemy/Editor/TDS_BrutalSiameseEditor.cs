using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

[CustomEditor(typeof(TDS_BrutalSiamese))]
public class TDS_BrutalSiameseEditor : TDS_EnemyEditor 
{
    /* TDS_BrutalSiameseEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Editor if the Brutal Siamese class]
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
	 *	Date :			[18/09/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the class]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    // Implement this method to make a custom inspector
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    // Implement this method to draw Handles 
    protected override void OnSceneGUI()
    {
        base.OnSceneGUI(); 
    }
    #endregion

    #endregion
}
