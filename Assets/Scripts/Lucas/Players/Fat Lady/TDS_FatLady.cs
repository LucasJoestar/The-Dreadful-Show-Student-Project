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

    #region Fields / Properties
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


    /// <summary>Backing field for <see cref="FoodCurrent"/>.</summary>
    [SerializeField] private int foodCurrent = 0;

    /// <summary>
    /// Current amount of food the Fat Lady has.
    /// </summary>
    public int FoodCurrent
    {
        get { return foodCurrent; }
        set
        {
            value = Mathf.Clamp(value, 0, foodMax);
            foodCurrent = value;
        }
    }

    /// <summary>Backing field for <see cref="FoodMax"/>.</summary>
    [SerializeField] private int foodMax = 3;

    /// <summary>
    /// Maximum amount of food at the Fat Lady's disposal.
    /// </summary>
    public int FoodMax
    {
        get { return foodMax; }
        set
        {
            if (value < 0) value = 0;
            foodMax = value;
        }
    }

    /// <summary>Backing field for <see cref="FoodHealValue"/>.</summary>
    [SerializeField] private int foodHealValue = 3;

    /// <summary>
    /// Heal value when eating some food.
    /// </summary>
    public int FoodHealValue
    {
        get { return foodHealValue; }
        set
        {
            if (value < 0) value = 0;
            foodHealValue = value;
        }
    }
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
    /// Starts the animation to eat some food if having in stock.
    /// </summary>
    /// <returns>Returns false if having nothing to eat or being at maximum health value, true otherwise.</returns>
    public bool Eat()
    {
        if ((foodCurrent == 0) || (healthCurrent == healthMax)) return false;

        SetFatLadyAnim(FatLadyAnimState.Eat);

        return true;
    }

    /// <summary>
    /// Heal the Fat Lady be eating food if having some.
    /// </summary>
    /// <returns>Returns true if having some food to eat, false otherwise.</returns>
    public bool FoodHeal()
    {
        if (foodCurrent == 0) return false;

        Heal(foodHealValue);
        FoodCurrent--;

        return true;
    }

    /// <summary>
    /// Set the amount of food of the Fat Lady to its maximum.
    /// </summary>
    public void SetFoodToMax() => FoodCurrent = foodMax;
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

            while (TDS_InputManager.GetButton(TDS_InputManager.HEAVY_ATTACK_BUTTON))
            {
                // Charge attack while holding attack button down
                yield return null;
            }
        }

        // Executes the attack
        preparingAttackCoroutine = StartCoroutine(base.PrepareAttack(_isLight));
        yield break;
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
        TDS_Checkpoint.OnCheckpointActivated += SetFoodToMax;
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        TDS_Checkpoint.OnCheckpointActivated -= SetFoodToMax;
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
