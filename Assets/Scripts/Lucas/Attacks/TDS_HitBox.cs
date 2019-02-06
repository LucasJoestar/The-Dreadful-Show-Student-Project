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

    #region Variables
    /// <summary>
    /// The current attack performed by this hit box.
    /// </summary>
    public TDS_Attack CurrentAttack { get; private set; } = null;

    /// <summary>
    /// All touched objects during the current attack.
    /// Key is the <see cref="Collider"/> of the object detected.
    /// Value is the object <see cref="TDS_Damageable"/> component used to deal damages and apply effect.
    /// </summary>
    public Dictionary<Collider, TDS_Damageable> TouchedObjects { get; private set; } = new Dictionary<Collider, TDS_Damageable>();

    /// <summary>
    /// What layers this hit box can touch, and therefore hit.
    /// </summary>
    public LayerMask WhatHit = new LayerMask();
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
        // If the given attack is null, send the information in the console
        // and return before activating the hit box.
        if (_attack == null)
        {
            Debug.LogWarning("The given attack to activate " + owner.name + "'s hit box is null ! Activation is canceled.");
            return;
        }

        CurrentAttack = _attack;
        collider.enabled = true;
        OnStartAttack?.Invoke(_attack);
    }

    /// <summary>
    /// Desactivates the hit box.
    /// Stop the current attack if there is one.
    /// </summary>
    public void Desactivate()
    {
        CurrentAttack = null;
        collider.enabled = false;
        TouchedObjects.Clear();

        OnStopAttack?.Invoke();
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
        if (!owner)
        {
            owner = GetComponentInParent<TDS_Character>();
            if (!owner) Debug.LogWarning("The HitBox " + name + " Character reference is missing !");
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // If the collider object should be hit, hit it
        
        // Check tag instead of layer
        if (WhatHit != (WhatHit | (1 << other.gameObject.layer))) return;

        TDS_Damageable _target = other.GetComponent<TDS_Damageable>();

        if (!_target || TouchedObjects.ContainsValue(_target)) return;

        // Deal damages and apply effect
        Debug.Log(owner.name + " attack " + other.name + " !");

        TouchedObjects.Add(other, _target);
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        // Removes object from the list of touched objects if it was in
        if (TouchedObjects.ContainsKey(other)) TouchedObjects.Remove(other);
    }
	#endregion

	#endregion
}
