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
     *  Date :			[16 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 * 
     *  Got to change the layer test with tag test in OnTriggerEnter method.
     *  
     *      - Added the OnStartAttack & OnStopAttack events.
     *      - Added the touchedObjects field & property.
     *      - Added the Activate & Desactivate methods ; and the Unity OnTriggerEnter & OnTriggerExit methods.
     * 
	 *	-----------------------------------
     * 
	 *	Date :			[15 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_HitBox class.
     *	
     *	    - Added collider & owner fields ; CurrentAttack property ; and WhatHit field.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Start attack event.
    /// Called when the hit box is being activated with the specified attack as parameter.
    /// </summary>
    public event Action<TDS_Attack> OnStartAttack = null;

    /// <summary>
    /// Stop attack event.
    /// Called when the hit box is being desactivated at the end of an attack.
    /// </summary>
    public event Action OnStopAttack = null;

    /// <summary>
    /// Touched something event.
    /// Called when touched something with an attack.
    /// </summary>
    public event Action OnTouch = null;
    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Trigger collider used to detect what should be touched by the hit box.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>Readonly property for <see cref="collider"/>.</summary>
    public BoxCollider Collider { get { return collider; } }

    /// <summary>
    /// Character owner of this hit box.
    /// </summary>
    public TDS_Character Owner = null;
    #endregion

    #region Variables
    /// <summary>
    /// The current attack performed by this hit box.
    /// </summary>
    public TDS_Attack CurrentAttack { get; private set; } = null;

    /// <summary>
    /// Indicates if the hit box is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// All touched colliders during the current attack.
    /// </summary>
    public List<Collider> TouchedObjects { get; private set; } = new List<Collider>();

    /// <summary>
    /// Bonus damages for the current attack.
    /// </summary>
    public int BonusDamages = 0;

    /// <summary>
    /// All tags to hit.
    /// </summary>
    public Tags HittableTags = new Tags();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activates the hit box.
    /// When activated, produces the effect of the given attack to any <see cref="TDS_Damageable"/> in <see cref="collider"/> zone whose layer is in <see cref="WhatHit"/>.
    /// </summary>
    /// <param name="_attack">Attack used to hit what is in the hit box.</param>
    public void Activate(TDS_Attack _attack)
    {
        if (!PhotonNetwork.isMasterClient) return;

        // If the given attack is null, send the information in the console
        // and return before activating the hit box.
        if (_attack == null)
        {
            Debug.LogWarning("The given attack to activate " + Owner.name + "'s hit box is null ! Activation is canceled.");
            return;
        }
        CurrentAttack = _attack;
        collider.enabled = true;
        IsActive = true;
        OnStartAttack?.Invoke(_attack);
    }

    /// <summary>
    /// Activates the hit box.
    /// When activated, produces the effect of the given attack to any <see cref="TDS_Damageable"/> in <see cref="collider"/> zone whose layer is in <see cref="WhatHit"/>.
    /// </summary>
    /// <param name="_attack">Attack used to hit what is in the hit box.</param>
    /// <param name="_owner">The person who attack.</param>
    public void Activate(TDS_Attack _attack, TDS_Character _owner)
    {
        Owner = _owner;
        Activate(_attack);
    }

    /// <summary>
    /// Activates the hit box.
    /// When activated, produces the effect of the given attack to any <see cref="TDS_Damageable"/> in <see cref="collider"/> zone whose layer is in <see cref="WhatHit"/>.
    /// </summary>
    /// <param name="_attack">Attack used to hit what is in the hit box.</param>
    /// <param name="_owner">The person who attack.</param>
    /// <param name="_hittableTags">Tags to hit.</param>
    public void Activate(TDS_Attack _attack, TDS_Character _owner, Tag[] _hittableTags)
    {
        Owner = _owner;
        HittableTags.ObjectTags = _hittableTags;

        Activate(_attack);
    }

    /// <summary>
    /// Desactivates the hit box.
    /// Stop the current attack if there is one.
    /// </summary>
    public void Desactivate()
    {
        CurrentAttack = null;
        BonusDamages = 0;
        collider.enabled = false;
        TouchedObjects.Clear();

        if (IsActive)
        {
            IsActive = false;
            OnStopAttack?.Invoke();
        }
    }

    /// <summary>
    /// Inflict damages to a specified target.
    /// </summary>
    /// <param name="_target">Target to hit.</param>
    private void InflictDamages(TDS_Damageable _target)
    {
        // Attack the target
        if (CurrentAttack.Attack(this, _target) < -1) return;

        // Call local method on the character who hit
        if (Owner)
        {
            if (PhotonNetwork.offlineMode && (Owner is TDS_Player _player))
            {
                _player.HitCallback(_target.Collider.bounds.center.x, _target.Collider.bounds.max.y, _target.transform.position.z);
            }
            else
            {
                TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", Owner.photonView.owner, TDS_RPCManager.GetInfo(Owner.photonView, Owner.GetType(), "HitCallback"), new object[] { _target.Collider.bounds.center.x, _target.Collider.bounds.max.y, _target.transform.position.z });
            }
        }

        // Triggers event
        OnTouch?.Invoke();
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Try to get components references if they are missing
        if (!collider)
        {
            collider = GetComponent<BoxCollider>();
            if (!collider) Debug.LogWarning("The HitBox " + name + " Collider is missing !");
        }
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        if (collider && collider.enabled)
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // If already touched this collider, return
        if (TouchedObjects.Contains(other)) return;

        // If the collider object should be hit, hit it
        // Check if object has tags
        if ((Owner && (other.gameObject == Owner.gameObject)) || !other.gameObject.HasTag(HittableTags.ObjectTags)) return;

        TDS_Damageable _target = other.GetComponent<TDS_Damageable>();
        if (!_target ) return;

        // Deal damages and apply effect
        TouchedObjects.Add(other);
        InflictDamages(_target);
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        // Removes object from the list of touched objects if it was in
        if (TouchedObjects.Contains(other)) TouchedObjects.Remove(other);
    }

    // Use this for initialization
    private void Start()
    {
        // Desactivate the hitbox at start time
        if (CurrentAttack == null) Desactivate();

        // If hit box is not set as trigger, set it
        if (!collider.isTrigger) collider.isTrigger = true;
    }
    #endregion

    #endregion
}
