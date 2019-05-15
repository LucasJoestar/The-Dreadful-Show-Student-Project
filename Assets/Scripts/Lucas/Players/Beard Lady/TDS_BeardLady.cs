using UnityEngine;

public class TDS_BeardLady : TDS_Player 
{
    /* TDS_BeardLady :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Gameplay class manipulating the Beard Lady player.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[19 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_BeardLady class.
     *	
     *	    Implemented methods to regulate the state of the beard.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="CurrentBeardState"/>.</summary>
    [SerializeField] private BeardState currentBeardState = BeardState.Normal;

    /// <summary>
    /// Current state of the Beard Lady's beard.
    /// The more long it is, the more long the character range is.
    /// </summary>
    public BeardState CurrentBeardState
    {
        get { return currentBeardState; }
        private set
        {
            currentBeardState = value;

            if (value != BeardState.VeryVeryLongDude)
            {
                InvokeGrowBeard();
            }

            CancelInvokeHealBeard();
            BeardCurrentLife = beardMaxLife;

            SetAnimBeard(value);
        }
    }

    /// <summary>Backing field for <see cref="BeardGrowInterval"/>.</summary>
    [SerializeField] private float beardGrowInterval = 5;

    /// <summary>
    /// Interval at which the Beard Lady's beard grow if everything is okay.
    /// </summary>
    public float BeardGrowInterval
    {
        get { return beardGrowInterval; }
        set
        {
            if (value < 0) value = 0;
            beardGrowInterval = value;

            ResetBeardGrow();
        }
    }

    /// <summary>Backing field for <see cref="BeardCurrentLife"/>.</summary>
    [SerializeField] private int beardCurrentLife = 0;

    /// <summary>
    /// Current life of the beard.
    /// </summary>
    public int BeardCurrentLife
    {
        get { return beardCurrentLife; }
        private set
        {
            value = Mathf.Clamp(value, 0, beardMaxLife);
            if (value == 0)
            {
                DegradeBeard();
                return;
            }
            else if (value != beardMaxLife)
            {
                InvokeHealBeard();
            }

            beardCurrentLife = value;
        }
    }

    /// <summary>Backing field for <see cref="BeardHealInterval"/>.</summary>
    [SerializeField] private int beardHealInterval = 1;

    /// <summary>
    /// Interval at which the beard get healed.
    /// </summary>
    public int BeardHealInterval
    {
        get { return beardHealInterval; }
        set
        {
            if (value < 0) value = 0;
            beardGrowInterval = value;
        }
    }

    /// <summary>Backing field for <see cref="beardMaxLife"/>.</summary>
    [SerializeField] private int beardMaxLife = 5;

    /// <summary>
    /// Maximum life of the beard.
    /// </summary>
    public int BeardMaxLife
    {
        get { return beardMaxLife; }
        set
        {
            if (value < 1) value = 1;
            beardMaxLife = value;
        }
    }
    #endregion

    #region Methods

    #region Original Methods

    #region Beard
    /// <summary>
    /// Let's degrade that kind woman's beard !
    /// </summary>
    private void DegradeBeard()
    {
        CancelInvokeGrowBeard();
        CurrentBeardState--;
    }

    /// <summary>
    /// Let's grow the beard of this sweet lady !
    /// </summary>
    private void GrowBeard()
    {
        CurrentBeardState++;
    }

    /// <summary>
    /// Heals the beard life.
    /// </summary>
    private void HealBeard()
    {
        BeardCurrentLife++;
    }

    /// <summary>
    /// Reset variables used to grow the beard.
    /// </summary>
    private void ResetBeardGrow()
    {
        CancelInvokeGrowBeard();
        InvokeGrowBeard();
    }

    /// <summary>
    /// Invoke the method to grow the beard after a certain time.
    /// </summary>
    private void InvokeGrowBeard() => Invoke("GrowBeard", beardGrowInterval);

    /// <summary>
    /// Cancel invoke of the method to grow the beard.
    /// </summary>
    private void CancelInvokeGrowBeard() => CancelInvoke("GrowBeard");

    /// <summary>
    /// Invoke the method to heal the beard after a certain time.
    /// </summary>
    private void InvokeHealBeard() => Invoke("HealBeard", beardHealInterval);

    /// <summary>
    /// Cancel invoke of the method to heal the beard.
    /// </summary>
    private void CancelInvokeHealBeard() => CancelInvoke("HealBeard");
    #endregion

    #region Health
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected override void Die()
    {
        base.Die();

        // Stop beard from growing
        CancelInvokeGrowBeard();
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        // Executes base method
        if (!base.TakeDamage(_damage)) return false;

        // Reset beard grow on hit
        ResetBeardGrow();

        return true;
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <param name="_position">Position in world space from where the hit come from.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage, Vector3 _position)
    {
        // Executes base method
        if (!base.TakeDamage(_damage, _position)) return false;

        // Reset beard grow on hit
        ResetBeardGrow();

        return true;
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set the Beard Lady's beard animator state.
    /// </summary>
    /// <param name="_state">State of the beard.</param>
    public void SetAnimBeard(BeardState _state)
    {
        animator.SetFloat("Beard", (int)_state);
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

        // Set player type, just in case
        PlayerType = PlayerType.BeardLady;

        // Let's make this beard grow repeatedly until it reach its maximum value
        if (currentBeardState != BeardState.VeryVeryLongDude) InvokeGrowBeard();
        SetAnimBeard(currentBeardState);

        beardCurrentLife = beardMaxLife;

        // Degragde beard when hitting something.
        hitBox.OnTouch += () => BeardCurrentLife--;
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
	}
	#endregion

	#endregion
}
