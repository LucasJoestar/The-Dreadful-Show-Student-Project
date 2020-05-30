﻿using System;
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

    #region Events
    /// <summary>
    /// Event called when the beard lady's state changed.
    /// </summary>
    public event Action<BeardState> OnBeardStateChanged = null;
    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// PhotonView of the transform used to spawn beard related FXs.
    /// </summary>
    [SerializeField] private PhotonView beardFXTransformPV = null;
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
            if (value < 0) value = 0;

            if (value > currentBeardState)
            {
                TDS_VFXManager.Instance?.SpawnEffect(FXType.BeardGrowsUp, beardFXTransformPV);
            }
            else
            {
                if (value < currentBeardState)
                {
                    TDS_VFXManager.Instance?.SpawnEffect(FXType.BeardDamaged, beardFXTransformPV);
                }
            }

            CancelInvokeGrowBeard();
            currentBeardState = value;
            OnBeardStateChanged?.Invoke(value);

            if (value < BeardState.VeryVeryLongDude)
            {
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
            beardHealInterval = value;
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
                ResetBeardGrow();
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

    #region Animator
    private readonly int beard_Hash = Animator.StringToHash("Beard");
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
        if (photonView.isMine) CancelInvokeGrowBeard();
    }

    /// <summary>
    /// Method called when this character hit something.
    /// Override it to implement feedback and other things.
    /// </summary>
    /// <param name="_opponentXCenter">X value of the opponent collider center position.</param>
    /// <param name="_opponentYMax">Y value of the opponent collider max position.</param>
    /// <param name="_opponentZ">Z value of the opponent position.</param>
    public override void HitCallback(float _opponentXCenter, float _opponentYMax, float _opponentZ)
    {
        base.HitCallback(_opponentXCenter, _opponentYMax, _opponentZ);

        // Reduces beard life on hit
        BeardCurrentLife--;
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
        if (photonView.isMine) ResetBeardGrow();

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
        animator.SetFloat(beard_Hash, (int)_state);

        TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "SetBeardAnim", new object[] { (int)_state });
    }

    /// <summary>
    /// Set the Beard Lady's beard animator state.
    /// </summary>
    /// <param name="_state">State of the beard.</param>
    public void SetBeardAnim(int _state)
    {
        animator.SetFloat(beard_Hash, _state);
    }
    #endregion

    #region Movements
    /// <summary>
    /// Freezes the player's movements and actions.
    /// </summary>
    public override void FreezePlayer()
    {
        base.FreezePlayer();

        CancelInvokeGrowBeard();
    }

    /// <summary>
    /// Unfreezes the player's movements and actions.
    /// </summary>
    public override void UnfreezePlayer()
    {
        base.UnfreezePlayer();

        ResetBeardGrow();
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

        if (photonView.isMine)
        {
            // Let's make this beard grow repeatedly until it reach its maximum value
            if (currentBeardState != BeardState.VeryVeryLongDude) InvokeGrowBeard();
            SetBeardAnim(currentBeardState);

            beardCurrentLife = beardMaxLife;
        }
    }
	#endregion

	#endregion
}
