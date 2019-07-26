﻿public class TDS_Destructible : TDS_Damageable
{
    /* TDS_Damageable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class for all destructibles elements of the game, like crates, barrels and more.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[01 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      Creation of the TDS_Destructible class.
	*/

    #region Fields / Properties
    
    #endregion

    #region Methods

    #region Original Methods

    #region Health
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected override void Die()
    {
        base.Die();
        
        if (PhotonNetwork.isMasterClient) SetAnimationState(DestructibleAnimState.Destruction);
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        if (!base.TakeDamage(_damage)) return false;
        
        if (!isDead && PhotonNetwork.isMasterClient)
        {
            SetAnimationState(DestructibleAnimState.Hit);
        }

        return true;
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set this destructible animation state.
    /// </summary>
    /// <param name="_state">New animation state of the destructible.</param>
    public void SetAnimationState(DestructibleAnimState _state)
    {
        // Online
        if (PhotonNetwork.isMasterClient)
        {
            // if (!animator) return;
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnimationState"), new object[] { (int)_state });
        }

        switch (_state)
        {
            case DestructibleAnimState.Hit:
                animator.SetTrigger("Hit");
                break;

            case DestructibleAnimState.Destruction:
                animator.SetTrigger("Destruction");
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set this destructible animation state.
    /// </summary>
    /// <param name="_state">New animation state of the destructible.</param>
    public void SetAnimationState(int _state)
    {
        SetAnimationState((DestructibleAnimState)_state);
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }
    #endregion

    #endregion
}
