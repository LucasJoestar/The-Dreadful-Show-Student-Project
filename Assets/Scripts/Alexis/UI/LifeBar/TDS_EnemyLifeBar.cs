using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_EnemyLifeBar : TDS_LifeBar
{
    /* TDS_LifeBar :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Class that holds the lifebar behaviour]
     *	    - Filling LifeBar 
     *	    - Make it follow its owner 
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     * 
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	
	*/


    #region Events

    #endregion

    #region Fields / Properties
    private Vector3 offset;

    #endregion

    #region Methods

    #region Original Methods
    public override void DestroyLifeBar()
    {
        if (TDS_UIManager.Instance)
        {
            TDS_UIManager.Instance.StopFilling(this);
        }
        Destroy(gameObject);
        owner.OnDie -= DestroyLifeBar;
    }

    /// <summary>
    /// Set the owner of the lifebar 
    /// Link Destroy method on the event OnDie of the owner
    /// Set the follow boolean on true and get an offset
    /// </summary>
    /// <param name="_owner">Owner of the life bar</param>
    /// <param name="_offset">Offset from the owner</param>
    /// <param name="_hasToFollow">Does the lifebar has to follow its owner</param>
    public void SetOwner(TDS_Character _owner, Vector3 _offset)
    {
        SetOwner(_owner);
        if (owner is TDS_Enemy)
        {
            offset = _offset;
            owner.OnDie += DestroyLifeBar;
        }
    }

    /// <summary>
    /// Make the gameObject follow its owner with a certain offset
    /// </summary>
    public void FollowOwner()
    {
        if (!owner) return;
        transform.position = Vector3.MoveTowards(transform.position, owner.transform.position + offset, Time.deltaTime * 25);
    }

    public override void UpdateLifeBar(int _currentHealth)
    {
        if (!background.gameObject.activeInHierarchy)
            background.gameObject.SetActive(true); 
        base.UpdateLifeBar(_currentHealth);
    }
    #endregion

    #region Unity Methods
    protected void Update()
    {
        FollowOwner();
    }
    #endregion

    #endregion


}
