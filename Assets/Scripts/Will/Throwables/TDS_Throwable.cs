using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TDS_HitBox),typeof(Rigidbody))]
public class TDS_Throwable : MonoBehaviour 
{
    /* TDS_Throwable :
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
    #region ObjectSettings
    [SerializeField,Header("Object settings")]
    bool canBeTakeByEnemies = false;
    [SerializeField]
    bool isHeld = false;
    [SerializeField]
    int damageMaxObject = 7;
    [SerializeField]
    int damageMinObject = 2;
    [SerializeField,Range(0,10)]
    int objectDurability = 10;
    [SerializeField]
    new Rigidbody rigidbody = null;
    #endregion
    #region PlayerSettings
    [SerializeField, Header("Character settings")]
    bool isHoldByPlayer = false;    
    [SerializeField]
    TDS_Character owner;
    [SerializeField]
    Transform rootCharacterObject;
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    void Drop()
    {

    }
    void PickUp()
    {
        //check here who PickUp the object
        //get the object root 
    }
    void Throw(/*positionFinal (where throw the object)*/)
    {

    }    
	#endregion

	#region Unity Methods
	// Use this for initialization
    void Start ()
    {
        if(!rigidbody) rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}
	#endregion
	#endregion
}
