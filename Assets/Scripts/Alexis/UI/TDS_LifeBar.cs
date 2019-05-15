using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class TDS_LifeBar : PunBehaviour
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

    [SerializeField] private Image filledImage;
    public Image FilledImage { get { return filledImage; } }

    private TDS_Character owner = null;

    private Vector3 offset;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Stop the coroutine that fill the life bar and destroy the game object
    /// </summary>
    public void DestroyLifeBar()
    {
        if (TDS_UIManager.Instance)
        {
            TDS_UIManager.Instance.StopFilling(filledImage);
        }
        PhotonNetwork.Destroy(this.gameObject);
    }

    /// <summary>
    /// Make the gameObject follow its owner with a certain offset
    /// </summary>
    public void FollowOwner()
    {
        if (!hasToFollowOwner || !PhotonNetwork.isMasterClient) return;
        transform.position = Vector3.MoveTowards(transform.position, owner.transform.position + offset, Time.deltaTime * 10);
    }

    /// <summary>
    /// Set the owner of the lifebar 
    /// Link Destroy method on the event OnDie of the owner
    /// </summary>
    /// <param name="_owner">Owner of the life bar</param>
    public void SetOwner(TDS_Character _owner)
    {
        owner = _owner;
        if(_owner.gameObject.HasTag("Player"))
        {
            GetComponent<PhotonView>().enabled = false;
        }
        if (_owner.gameObject.HasTag("Enemy"))
        {
            owner.OnDie += DestroyLifeBar;
        }
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
        owner = _owner;
        offset = _offset; 
        hasToFollowOwner = _hasToFollow;
        owner.OnDie += DestroyLifeBar;
    }
    #endregion

    #region Unity Methods
    private void Update()
    {
        FollowOwner();
    }

    #endregion

    #endregion


}
