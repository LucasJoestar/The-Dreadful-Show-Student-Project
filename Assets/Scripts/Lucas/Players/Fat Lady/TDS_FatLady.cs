﻿using System.Collections;
using UnityEngine;

public class TDS_FatLady : TDS_Player 
{
    /* TDS_FatLady :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages the behaviour and characteristics of the Fat Lady player.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[12 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_FatLady class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Variables
    /// <summary>Backing field for <see cref="IsBerserk"/>.</summary>
    [SerializeField] private bool isBerserk = false;

    /// <summary>
    /// Indicates if the Fat Lady is currently in berserk mode or not.
    /// </summary>
    public bool IsBerserk
    {
        get { return isBerserk; }
        set
        {
            isBerserk = value;

            if (value) SetFatLadyAnim(FatLadyAnimState.Berserk);
            else SetFatLadyAnim(FatLadyAnimState.Pacific);
        }
    }

    /// <summary>Backing field for <see cref="IsSnackAvailable"/></summary>
    [SerializeField] private bool isSnackAvailable = true;

    /// <summary>
    /// Indicates if the Fat Lady snack is ready for use.
    /// </summary>
    public bool IsSnackAvailable
    {
        get { return isSnackAvailable; }
        set
        {
            if (restaureSnackCoroutine != null) StopCoroutine(restaureSnackCoroutine);
            if (!value) restaureSnackCoroutine = StartCoroutine(RestauringSnack());

            isSnackAvailable = value;
        }
    }

    /// <summary>Backing field for <see cref="SnackRestaureTime"/>.</summary>
    [SerializeField] private float snackRestaureTime = 30;

    /// <summary>
    /// Current amount of food the Fat Lady has.
    /// </summary>
    public float SnackRestaureTime
    {
        get { return snackRestaureTime; }
        set
        {
            if (value < 0) value = 0;
            snackRestaureTime = value;
        }
    }

    /// <summary>Backing field for <see cref="BerserkHealthStep"/>.</summary>
    [SerializeField] private int berserkHealthStep = 33;

    /// <summary>
    /// Health value separating the berserk mode (when healrh is lower) from the "pacific" mode (when higher).
    /// </summary>
    public int BerserkHealthStep
    {
        get { return berserkHealthStep; }
        set
        {
            if (value < 0) value = 0;
            berserkHealthStep = value;
        }
    }

    /// <summary>Backing field for <see cref="SnackHealValue"/>.</summary>
    [SerializeField] private int snackHealValue = 3;

    /// <summary>
    /// Heal value when snacking.
    /// </summary>
    public int SnackHealValue
    {
        get { return snackHealValue; }
        set
        {
            if (value < 0) value = 0;
            snackHealValue = value;
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine used to restaure the Fat Lady's snack.
    /// </summary>
    private Coroutine restaureSnackCoroutine = null;
    #endregion

    #region Memory & Debugs
    /// <summary>
    /// Timer used to restaure the snack.
    /// </summary>
    [SerializeField] private float snackRestaureTimer = 0;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Berserk & Food
    /// <summary>
    /// Checks the health status of the Fat Lady and set her berserk mode consequently.
    /// </summary>
    /// <param name="_health">New health value of the Fat Lady.</param>
    public void CheckHealthStatus(int _health)
    {
        if (_health > berserkHealthStep)
        {
            if (isBerserk) IsBerserk = false;
        }
        else if (!isBerserk) IsBerserk = true;
    }

    /// <summary>
    /// Coroutine restauring the Fat Lady's snack.
    /// </summary>
    public void RestaureSnack() => IsSnackAvailable = true;

    /// <summary>
    /// Coroutine restauring the Fat Lady's snack after a certain time amount.
    /// </summary>
    private IEnumerator RestauringSnack()
    {
        snackRestaureTimer = 0;

        while (snackRestaureTimer < snackRestaureTime)
        {
            yield return null;
            snackRestaureTimer += Time.deltaTime;
        }

        RestaureSnack();
    }

    /// <summary>
    /// Heal the Fat Lady be eating food if having some.
    /// </summary>
    /// <returns>Returns true if having some food to eat, false otherwise.</returns>
    public bool SnackHeal()
    {
        if (!isSnackAvailable) return false;

        Heal(snackHealValue);
        IsSnackAvailable = false;

        return true;
    }

    /// <summary>
    /// Starts the animation to eat some food if having in stock.
    /// </summary>
    /// <returns>Returns false if having nothing to eat or being at maximum health value, true otherwise.</returns>
    public bool StartEatingSnack()
    {
        if ((!isSnackAvailable) || (healthCurrent == healthMax)) return false;

        SetFatLadyAnim(FatLadyAnimState.Snack);
        FreezePlayer();

        return true;
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player prepare an attack.
    /// By default, the player is supposed to just directly attack ; but for certain situations, the attack might not played directly : that's the goal of this method, to be override to rewrite a pre-attack behaviour.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    protected override IEnumerator PrepareAttack(bool _isLight)
    {
        if (!_isLight)
        {
            SetAnim(PlayerAnimState.HeavyAttack);
            float _timer = 0;

            while (TDS_InputManager.GetButton(TDS_InputManager.HEAVY_ATTACK_BUTTON))
            {
                // Charge attack while holding attack button down
                if (_timer > 2.5f)
                {
                    TDS_Camera.Instance.StartScreenShake(.0075f);
                }
                else if (_timer > 1f)
                {
                    TDS_Camera.Instance.StartScreenShake(_timer / 500);
                }

                yield return null;
                _timer += Time.deltaTime;
            }
        }

        // Executes the attack
        preparingAttackCoroutine = StartCoroutine(base.PrepareAttack(_isLight));
        yield break;
    }

    /// <summary>
    /// Stops the player from preparing an attack.
    /// </summary>
    /// <returns>Returns true if successfully stopped preparing an attack, false if none was in preparation.</returns>
    public override bool StopPreparingAttack()
    {
        if (!base.StopPreparingAttack()) return false;

        // If combo at zero, reset animation manually (automatically set if stopping a combo)
        if (comboCurrent.Count == 0) SetAnim(PlayerAnimState.ComboBreaker);
        return true;
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    /// <returns>Returns an int indicating at which step the method returned :
    /// 0 if everything went good ;
    /// A negative number if an action has been performed ;
    /// 1 if dodging, parrying or preparing an attack ;
    /// 2 if having a throwable ;
    /// and 3 if attacking.</returns>
    public override int CheckActionsInputs()
    {
        int _result = base.CheckActionsInputs();
        if (_result != 0) return _result;

        // Check if it's snack time
        if (TDS_InputManager.GetButtonDown(TDS_InputManager.SNACK_BUTTON) && isGrounded)
        {
            StartEatingSnack();
            return -1;
        }

        // If everything went good, return 0
        return 0;
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set the animation state of the Fat Lady's animator.
    /// </summary>
    /// <param name="_state"></param>
    public void SetFatLadyAnim(FatLadyAnimState _state)
    {
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetFatLadyAnim"), new object[] { (int)_state });
        }

        switch (_state)
        {
            case FatLadyAnimState.Berserk:
                animator.SetBool("IsBerserk", true);
                break;

            case FatLadyAnimState.Pacific:
                animator.SetBool("IsBerserk", false);
                break;

            case FatLadyAnimState.Snack:
                animator.SetTrigger("Snack");
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set the animation state of the Fat Lady's animator.
    /// </summary>
    /// <param name="_state"></param>
    public void SetFatLadyAnim(int _state)
    {
        SetFatLadyAnim((FatLadyAnimState)_state);
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Set player type, just in case
        PlayerType = PlayerType.FatLady;

        // Check Fat Lady health status when it changes
        OnHealthChanged += CheckHealthStatus;

        // Set food to maximum when hitting a checkpoint
        TDS_Checkpoint.OnCheckpointActivated += RestaureSnack;
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    protected override void OnDestroy()
    {
        base.OnDestroy();

        TDS_Checkpoint.OnCheckpointActivated -= RestaureSnack;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
	#endregion

	#endregion
}
