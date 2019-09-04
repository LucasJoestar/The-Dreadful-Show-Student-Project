using System.Collections.Generic;
using UnityEngine;

public class TDS_Destructible : TDS_Damageable
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
    /// <summary>
    /// All available loot for this destructible.
    /// </summary>
    [SerializeField] protected GameObject[] loot = new GameObject[] { };


    /// <summary>Backing field for <see cref="LootChance"/>.</summary>
    [SerializeField] protected int lootChance = 100;

    /// <summary>
    /// Loot chance to appear (in percent).
    /// </summary>
    public int LootChance
    {
        get { return lootChance; }
        set
        {
            lootChance = Mathf.Clamp(value, 0, 100);
        }
    }

    /// <summary>Backing field for <see cref="LootMin"/>.</summary>
    [SerializeField] protected int lootMin = 1;

    /// <summary>
    /// Minimum amount of loot to drop.
    /// </summary>
    public int LootMin
    {
        get { return lootMin; }
        set
        {
            lootMin = Mathf.Clamp(value, 0, lootMax);
        }
    }

    /// <summary>Backing field for <see cref="LootMax"/>.</summary>
    [SerializeField] protected int lootMax = 1;

    /// <summary>
    /// Maximum amount of loot to drop.
    /// </summary>
    public int LootMax
    {
        get { return lootMax; }
        set
        {
            if (value < 0) value = 0;
            if (lootMin > value) lootMin = value;

            lootMax = value;
        }
    }
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

        if (rigidbody) rigidbody.isKinematic = true;
        collider.enabled = false;

        if (!PhotonNetwork.isMasterClient) return;

        // Drop loot
        if ((loot.Length > 0) && (LootChance > 0) && (Random.Range(1, 101) <= lootChance))
        {
            List<GameObject> _availableLoot = new List<GameObject>(loot);

            int _lootAmount = Random.Range(lootMin, lootMax + 1);
            for (int _i = 0; _i < _lootAmount; _i++)
            {
                Loot(ref _availableLoot);
                if (_availableLoot.Count == 0) break;
            }
        }

        SetAnimationState(DestructibleAnimState.Destruction);
    }

    /// <summary>
    /// Loots a random object from a given list.
    /// </summary>
    /// <param name="_availableLoot">List of available objects to loot.</param>
    /// <returns>Returns the looted object.</returns>
    protected virtual GameObject Loot(ref List<GameObject> _availableLoot)
    {
        GameObject _loot = _availableLoot[Random.Range(0, _availableLoot.Count)];

        Rigidbody _rigidbody = _loot.GetComponent<Rigidbody>();

        Vector3 _position = _rigidbody ?
                            new Vector3(sprite.bounds.center.x + (sprite.bounds.extents.x * Random.Range(-.9f, .9f)), sprite.bounds.center.y + (sprite.bounds.extents.y * Random.Range(-.5f, .9f)), sprite.bounds.center.z + (sprite.bounds.extents.z * Random.Range(-.9f, .9f))) :
                            new Vector3(sprite.bounds.center.x, 0, sprite.bounds.center.z);

        GameObject _instance = PhotonNetwork.Instantiate(_loot.name, _position, Quaternion.identity, 0);


        if (_rigidbody)
        {
            _rigidbody.AddForce(new Vector3(Random.Range(-250, 250), Random.Range(100, 400), Random.Range(-150, 150)));
        }

        _availableLoot.Remove(_loot);

        return _instance;
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

    #region Effects
    /// <summary>
    /// Project this damageable in the air.
    /// </summary>
    /// <param name="_toRight">Should the damageable be pushed to the right of left.</param>
    /// <returns>Returns true if successfully projected this damageable in the air, false otherwise.</returns>
    public override bool Project(bool _toRight)
    {
        if (!base.Project(_toRight)) return false;

        if (rigidbody)
        {
            rigidbody.AddForce(new Vector3(_toRight.ToSign() * 200, 500, 0));
        }
        return true;
    }
    #endregion

    #endregion

    #region Unity Methods
    // Use this for initialization
    protected override void Start()
    {
        if (PhotonNetwork.connected && (photonView.owner == null)) photonView.TransferOwnership(PhotonNetwork.player);

        base.Start();
    }
    #endregion

    #endregion
}
