using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

#pragma warning disable 0649

public class TDS_LifeBar : UnityEngine.MonoBehaviour
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
     *  Date :			[28/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Removing the online settings]
     *	    - Remove the inheritance from PunBehaviour and the Interface IPunObservable
     *	    - Set filling values are now set in local
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[01/04/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the TDS_LifeBar class]
     *	    - Create variables: filled Image, Owner, Has To Follow and Offset
     *	    - Create Methods DestroyLifeBar, FollowOwner and SetOwner
	 *
	 *	-----------------------------------
	*/

    #region Events 
    #endregion

    #region Fields and properties
    private bool hasToFollowOwner = false;

    [SerializeField] protected Image foregroundFilledImage = null; 
    public Image ForegroundFilledImage { get { return foregroundFilledImage; }}
    [SerializeField] protected Image filledImage = null;
    public Image FilledImage { get { return filledImage; } }

    protected TDS_Character owner = null;

    private Vector3 offset;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Stop the coroutine that fill the life bar and destroy the game object
    /// </summary>
    public virtual void DestroyLifeBar()
    {
        if(owner is TDS_Enemy)
        {
            if (TDS_UIManager.Instance)
            {
                TDS_UIManager.Instance.StopFilling(this);
            }
            UnityEngine.Object.Destroy(this.gameObject);
            owner.OnDie -= DestroyLifeBar;
        }
    }

    /// <summary>
    /// Make the gameObject follow its owner with a certain offset
    /// </summary>
    public void FollowOwner()
    {
        if (!hasToFollowOwner || !owner) return;
        transform.position = Vector3.MoveTowards(transform.position, owner.transform.position + offset, Time.deltaTime * 10);
    }

    /// <summary>
    /// Set all fill amount to 1
    /// </summary>
    public virtual void ResetLifeBar()
    {
        if(foregroundFilledImage) foregroundFilledImage.fillAmount = 1;
        if(filledImage) filledImage.fillAmount = 1;
        owner = null; 
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the owner of the lifebar 
    /// Link Destroy method on the event OnDie of the owner
    /// </summary>
    /// <param name="_owner">Owner of the life bar</param>
    public virtual void SetOwner(TDS_Character _owner)
    {
        owner = _owner;
    }

    /// <summary>
    /// Set the owner of the lifebar 
    /// Link Destroy method on the event OnDie of the owner
    /// Set the follow boolean on true and get an offset
    /// </summary>
    /// <param name="_owner">Owner of the life bar</param>
    /// <param name="_offset">Offset from the owner</param>
    /// <param name="_hasToFollow">Does the lifebar has to follow its owner</param>
    public void SetOwner(TDS_Character _owner, Vector3 _offset, bool _hasToFollow = true)
    {
        SetOwner(_owner); 
        if(owner is TDS_Enemy)
        {
            offset = _offset;
            hasToFollowOwner = _hasToFollow;
            owner.OnDie += DestroyLifeBar;
        }
    }
    #endregion

    #region Unity Methods
    protected virtual void Update()
    {
        FollowOwner();
    }
    #endregion

    #endregion

}
