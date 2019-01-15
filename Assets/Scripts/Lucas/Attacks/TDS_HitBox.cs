using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_HitBox : MonoBehaviour 
{
    /* TDS_HitBox :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Controls a character attacks outcomes.
     *	    
     *	    Contains the reference of the character linked to this hit box, what should be hit, the current attack and what it has touched yet.
     *	Call the Activate method with a specified attack to enable the hit box when starting an attack, and the Desactivate method do disable it when it's over.
     *	
     *	    The position and size of the hit box should be directly managed in the character's attacks animations.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[15 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_HitBox class.
     *	    - Added collider & owner fields ; CurrentAttack property ; and whatHit field.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Trigger collider used to detect what should be touched by the hit box.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// Character owner of this hit box.
    /// </summary>
    [SerializeField] private TDS_Character owner = null;
    #endregion

    /// <summary>
    /// The current attack performed by this hit box.
    /// </summary>
    public TDS_Attack CurrentAttack { get; private set; }

    /// <summary>
    /// What layers this hit box can touch, and therefore hit.
    /// </summary>
    [SerializeField] private LayerMask whatHit = new LayerMask();

    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    // Use this for initialization
    private void Start ()
    {
		
    }
	
	// Update is called once per frame
	private void Update ()
    {
        
	}
	#endregion

	#endregion
}
