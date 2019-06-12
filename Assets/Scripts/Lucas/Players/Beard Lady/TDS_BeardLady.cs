using System.Collections;
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

    #region Components & References
    /// <summary>
    /// FX instantiated when the beard grows up.
    /// </summary>
    [SerializeField] private GameObject beardMagicFX = null;

    /// <summary>
    /// Transform used to instantiate the beard FX.
    /// </summary>
    [SerializeField] private Transform beardFXTransform = null;
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="CurrentBeardState"/>.</summary>
    [SerializeField] private BeardState currentBeardState = BeardState.Normal;

    /// <summary>
    /// Current state of the Beard Lady's beard.
    /// The more long it is, the more long the character range is.
    /// </summary>
    public BeardState CurrentBeardState
    {
        get { return currentBeardState; }
        set
        {
            if (value > currentBeardState)
            {
                Instantiate(beardMagicFX, beardFXTransform, false);
            }
            else
            {
                CancelInvokeGrowBeard();
            }

            currentBeardState = value;

            if (value < BeardState.VeryVeryLongDude)
            {
                Debug.Log("Beard State => " + value);
                InvokeGrowBeard();
            }

            CancelInvokeHealBeard();
            BeardCurrentLife = beardMaxLife;

            SetBeardAnim(value);
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

    /// <summary>Backing field for <see cref="BeardHealInterval"/>.</summary>
    [SerializeField] private float beardHealInterval = 1;

    /// <summary>
    /// Interval at which the beard get healed.
    /// </summary>
    public float BeardHealInterval
    {
        get { return beardHealInterval; }
        set
        {
            if (value < 0) value = 0;
            beardGrowInterval = value;
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
        set
        {
            value = Mathf.Clamp(value, 0, beardMaxLife);
            if (value == 0)
            {
                CurrentBeardState--;
                return;
            }
            else if (value != beardMaxLife)
            {
                InvokeHealBeard();
            }

            beardCurrentLife = value;
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

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the grow beard method.
    /// </summary>
    private Coroutine growBeardCoroutine = null;

    /// <summary>
    /// Reference of the current coroutine of the heal beard method.
    /// </summary>
    private Coroutine healBeardCoroutine = null;
    #endregion

    #region Memory
    /// <summary>
    /// Timer for the grow beard coroutine, also used in the Beard Lady custom editor.
    /// </summary>
    [SerializeField] private float growBeardTimer = 0;

    /// <summary>
    /// Timer for the heal beard coroutine, also used in the Beard Lady custom editor.
    /// </summary>
    [SerializeField] private float healBeardTimer = 0;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Beard
    /// <summary>
    /// Let's grow the beard of this sweet lady !
    /// </summary>
    private IEnumerator GrowBeard()
    {
        growBeardTimer = beardGrowInterval;

        while (growBeardTimer > 0)
        {
            yield return null;
            growBeardTimer -= Time.deltaTime;
        }

        growBeardCoroutine = null;
        CurrentBeardState++;
    }

    /// <summary>
    /// Heals the beard life.
    /// </summary>
    private IEnumerator HealBeard()
    {
        healBeardTimer = BeardHealInterval;

        while (healBeardTimer > 0)
        {
            yield return null;
            healBeardTimer -= Time.deltaTime;
        }

        healBeardCoroutine = null;
        BeardCurrentLife++;
    }

    /// <summary>
    /// Reset variables used to grow the beard.
    /// </summary>
    public void ResetBeardGrow()
    {
        if ((growBeardCoroutine == null) || (currentBeardState >= BeardState.VeryVeryLongDude)) return;

        CancelInvokeGrowBeard();
        InvokeGrowBeard();
    }

    /// <summary>
    /// Starts the coroutine to grow the beard after a certain time.
    /// </summary>
    public void InvokeGrowBeard() => growBeardCoroutine = StartCoroutine(GrowBeard());

    /// <summary>
    /// Stops the coroutine of the method to grow the beard.
    /// </summary>
    public void CancelInvokeGrowBeard()
    {
        if (growBeardCoroutine != null) StopCoroutine(growBeardCoroutine);
    }

    /// <summary>
    /// Starts the coroutine to heal the beard after a certain time.
    /// </summary>
    public void InvokeHealBeard() => healBeardCoroutine = StartCoroutine(HealBeard());

    /// <summary>
    /// Stops the coroutine of the method to heal the beard.
    /// </summary>
    public void CancelInvokeHealBeard()
    {
        if (healBeardCoroutine != null) StopCoroutine(healBeardCoroutine);
    }
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
    public void SetBeardAnim(BeardState _state)
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

        // Set player type, just in case
        PlayerType = PlayerType.BeardLady;
    }

	// Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Let's make this beard grow repeatedly until it reach its maximum value
        if (currentBeardState != BeardState.VeryVeryLongDude) InvokeGrowBeard();
        SetBeardAnim(currentBeardState);

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
