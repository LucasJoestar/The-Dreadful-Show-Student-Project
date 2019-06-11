using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 

public class TDS_BottomLimit : PunBehaviour 
{
    /* TDS_BottomLimit :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
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
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	 */

    #region Methods

    #region Original Methods
    /// <summary>
    /// Reset the character position if it falls out of the bounds
    /// </summary>
    /// <param name="_character">Reset character</param>
    private void ResetCharacterPosition(TDS_Character _character)
    {
        Rigidbody _r = _character.GetComponent<Rigidbody>();
        _r.isKinematic = true;
        _r.velocity = Vector3.zero;
        _character.transform.position = new Vector3(_character.transform.position.x, .5f, _character.transform.position.z);
        _r.isKinematic = false;
    }
    #endregion

    #region Unity Methods
    private void OnCollisionEnter(Collision _collider)
    {
        if (!PhotonNetwork.isMasterClient || !_collider.gameObject.GetComponent<TDS_Character>()) return;
        TDS_Character _character = _collider.gameObject.GetComponent<TDS_Character>();
        ResetCharacterPosition(_character); 
    }
    #endregion

    #endregion
}
