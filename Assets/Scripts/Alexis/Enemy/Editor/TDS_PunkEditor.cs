using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Punk))]
public class TDS_PunkEditor : TDS_EnemyEditor 
{
    /* TDS_PunkEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Editor class of the TDS_Punk
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[13/02/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the editor class]
     *	    - Create a serialized field attacks for the enemyAttacks of the punk
     *	    - Create bool to display or not the attacks 
	 *
	 *	-----------------------------------
	*/

    #region Fields and properties

    #region Target Scripts Info
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isPunkMultiEditing = false;

    /// <summary>
    /// All editing instances of the Enemy class.
    /// </summary>
    private List<TDS_Punk> punks = new List<TDS_Punk>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    #endregion

    #region Unity Methods
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //Get the target editiong scripts
        targets.ToList().ForEach(t => punks.Add((TDS_Punk)t));
        if (targets.Length == 1) isPunkMultiEditing = false;
        else isPunkMultiEditing = true; 
    }
    #endregion

    #endregion
}
