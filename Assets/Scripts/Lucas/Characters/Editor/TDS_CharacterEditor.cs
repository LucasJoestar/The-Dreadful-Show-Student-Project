﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Character), false), CanEditMultipleObjects]
public class TDS_CharacterEditor : TDS_DamageableEditor 
{
    /* TDS_CharacterEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
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

    #endregion

    #region Methods

    #region Original Methods
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {

    }

	// Use this for initialization
    private void Start()
    {
		
    }
	
	// Update is called once per frame
	private void Update()
    {
        
	}
	#endregion

	#endregion
}
