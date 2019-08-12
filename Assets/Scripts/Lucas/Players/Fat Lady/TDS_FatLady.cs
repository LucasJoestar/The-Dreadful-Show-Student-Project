using System;
using System.Collections;
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

    #region Events
    /// <summary>
    /// Event called when the Fat Lady ate her snack.
    /// </summary>
    public event Action OnAteSnack = null;

    /// <summary>
    /// Event called when the Fat Lady's snack ahs been restaured.
    /// </summary>
    public event Action OnRestauredSnack = null;

    /// <summary>
    /// Event called every frame while restauring the Fat Lady's snack, with percentage from 0 to 1 as parameter (1 is fully restaured).
    /// </summary>
    public event Action<float> OnRestauringSnack = null;
    #endregion

    #region Fields / Properties

    #region Variables
    /// <summary>Backing field for <see cref="IsAngry"/>.</summary>
    [SerializeField] private bool isAngry = false;

    /// <summary>
    /// Indicates if the Fat Lady is currently very very angry or not.
    /// </summary>
    public bool IsAngry
    {
        get { return isAngry; }
        set
        {
            isAngry = value;

            if (value)
            {
                SpeedCoef *= angrySpeedCoef;
                SetFatLadyAnim(FatLadyAnimState.Angry);
            }
            else
            {
                SpeedCoef /= angrySpeedCoef;
                SetFatLadyAnim(FatLadyAnimState.Cool);
            }
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
            if (value)
            {
                OnRestauredSnack?.Invoke();
            }
            else
            {
                OnAteSnack?.Invoke();
                restaureSnackCoroutine = StartCoroutine(RestauringSnack());
            }

            isSnackAvailable = value;
        }
    }

    /// <summary>Backing field for <see cref="AngrySpeedCoef"/>.</summary>
    [SerializeField] private float angrySpeedCoef = 1.25f;

    /// <summary>
    /// Coefficient of the Fat Lady's speed when she's angry.
    /// </summary>
    public float AngrySpeedCoef
    {
        get { return angrySpeedCoef; }
        set
        {
            if (value < 0) value = 0;
            angrySpeedCoef = value;
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

    /// <summary>Backing field for <see cref="AngryHealthStep"/>.</summary>
    [SerializeField] private int angryHealthStep = 33;

    /// <summary>
    /// Health value separating the angry mode (when health is lower) from the "cool" mode (when higher).
    /// </summary>
    public int AngryHealthStep
    {
        get { return angryHealthStep; }
        set
        {
            if (value < 0) value = 0;
            angryHealthStep = value;
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
        if (isDead)
        {
            if (isAngry) IsAngry = false;
            return;
        }
        if (_health > angryHealthStep)
        {
            if (isAngry) IsAngry = false;
        }
        else if (!isAngry) IsAngry = true;
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
            OnRestauringSnack?.Invoke(snackRestaureTimer / snackRestaureTime);
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
            SetFatLadyAnim(FatLadyAnimState.PrepareAttack);
            float _timer = 0;
            float _littleLimit = isAngry ? .5f : 1f;
            float _bigLimit = isAngry ? 1.5f : 2.5f;
            float _littleShake = isAngry ? 350 : 500;
            float _bigShake = isAngry ? .008f : .0075f;

            while (TDS_InputManager.GetButton(TDS_InputManager.HEAVY_ATTACK_BUTTON))
            {
                // Charge attack while holding attack button down
                if (_timer > _bigLimit)
                {
                    TDS_Camera.Instance.StartScreenShake(_bigShake);
                }
                else if (_timer > _littleLimit)
                {
                    TDS_Camera.Instance.StartScreenShake(_timer / _littleShake);
                }

                yield return null;
                _timer += Time.deltaTime;
            }
        }

        // Executes the attack
        PreparingAttackCoroutine = StartCoroutine(base.PrepareAttack(_isLight));
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
        if (comboCurrent.Count == 0) SetAnimOnline(PlayerAnimState.ComboBreaker);
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
            case FatLadyAnimState.Angry:
                animator.SetBool("IsAngry", true);
                break;

            case FatLadyAnimState.Cool:
                animator.SetBool("IsAngry", false);
                break;

            case FatLadyAnimState.Snack:
                animator.SetTrigger("Snack");
                break;

            case FatLadyAnimState.PrepareAttack:
                animator.SetTrigger("PrepareAttack");
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
        TDS_Checkpoint.OnPassCheckpoint += RestaureSnack;
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    protected override void OnDestroy()
    {
        base.OnDestroy();

        TDS_Checkpoint.OnPassCheckpoint -= RestaureSnack;
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
