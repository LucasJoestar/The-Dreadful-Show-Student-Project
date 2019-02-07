﻿using System;
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
    bool isHoldByPlayer = false;
    [SerializeField]
    float objectSpeed = 15f;
    [SerializeField,Range(0,10)]
    int objectDurability = 10;
    [SerializeField]
    new Rigidbody rigidbody = null;    
    #endregion
    #region PlayerSettings
    [SerializeField, Header("Character settings")]       
    TDS_Character owner;
    [SerializeField]
    Transform rootCharacterObject;
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    void Drop()
    {
        if (!isHeld) return;
        rigidbody.isKinematic = false;
        isHeld = false;
    }
    void DestroyThrowableObject()
    {
        Destroy(gameObject);
    }
    void PickUp(TDS_Character _carrier)
    {
        //check here who PickUp the object
        //get the object root 
        //if (!canBeTakeByEnemies) return;

        gameObject.layer = LayerMask.NameToLayer("Player");
        rigidbody.isKinematic = true;
        transform.position = rootCharacterObject.transform.position;
        isHeld = true;
        
        if(_carrier is TDS_Player)
        {
            isHoldByPlayer = true;
        }

    }
    void Throw(/*positionFinal (where throw the object)*/)
    {
        rigidbody.velocity = transform.right * objectSpeed;
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