using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class TDS_FireEater : TDS_Player 
{
    /* TDS_FireEater :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Gameplay class manipulating the Fire Eater player.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[21 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_FireEater class.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Event called when triggering the mini game.
    /// </summary>
    public event Action OnTriggerMiniGame = null;
    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Sprite of the Fire Eater mini game.
    /// </summary>
    [SerializeField] private SpriteRenderer miniGameSprite = null;

    /// <summary>
    /// Anchor used for the mini game sprites.
    /// </summary>
    [SerializeField] private Transform miniGameAnchor = null;
    #endregion

    #region Constants
    /// <summary>
    /// Time used to activate the Fire Eater mini game when pressing the attack button.
    /// </summary>
    public const float TIME_TO_ACTIVATE_MINI_GAME = .35f;
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="IsDrunk"/>.</summary>
    [SerializeField] private bool isDrunk = false;

    /// <summary>
    /// Indicates if the Fire Eater is currently drunk.
    /// </summary>
    public bool IsDrunk
    {
        get { return isDrunk; }
        private set
        {
            isDrunk = value;

            // Play drunk sound
        }
    }

    /// <summary>Backing field for <see cref="IsInMiniGame"/>.</summary>
    [SerializeField] private bool isInMiniGame = false;

    /// <summary>
    /// Indicates if the Fire Eater is currently in the mini game.
    /// </summary>
    public bool IsInMiniGame
    {
        get { return isInMiniGame; }
        set
        {
            isInMiniGame = value;
            if (photonView.isMine)
            {
                if (value)
                {
                    float _xBound = TDS_Camera.Instance.Camera.WorldToViewportPoint(new Vector2(miniGameSprite.bounds.min.x - 2, miniGameSprite.bounds.center.y)).x;
                    if (_xBound < 0)
                    {
                        miniGameAnchor.localPosition = new Vector3((TDS_Camera.Instance.CameraXRatio * 2 * _xBound) * -isFacingRight.ToSign(), miniGameAnchor.localPosition.y, miniGameAnchor.localPosition.z);
                    }
                    else if ((_xBound = TDS_Camera.Instance.Camera.WorldToViewportPoint(new Vector2(miniGameSprite.bounds.max.x + 2, miniGameSprite.bounds.center.y)).x) > 1)
                    {
                        miniGameAnchor.localPosition = new Vector3((TDS_Camera.Instance.CameraXRatio * 2 * (1 - _xBound)) * isFacingRight.ToSign(), miniGameAnchor.localPosition.y, miniGameAnchor.localPosition.z);
                    }
                }
                else if (miniGameAnchor.localPosition.x != 0)
                {
                    miniGameAnchor.localPosition = new Vector3(0, miniGameAnchor.localPosition.y, miniGameAnchor.localPosition.z);
                }
                animator.SetBool(isInMiniGame_Hash, value);
            }
        }
    }

    /// <summary>Backing field for <see cref="DrunkSpeedCoef"/>.</summary>
    [SerializeField] private float drunkSpeedCoef = .8f;

    /// <summary>
    /// Coefficient applied to speed when drunk.
    /// </summary>
    public float DrunkSpeedCoef
    {
        get { return drunkSpeedCoef; }
        set
        {
            if (value < 0) value = 0;
            drunkSpeedCoef = value;

            if (isDrunk) speedCoef = value;
        }
    }

    /// <summary>Backing field for <see cref="SoberUpTime"/>.</summary>
    [SerializeField] private float soberUpTime = 10;

    /// <summary>
    /// Time it takes to the Fire Eater to sober up.
    /// </summary>
    public float SoberUpTime
    {
        get { return soberUpTime; }
        set
        {
            if (value < 0) value = 0;
            soberUpTime = value;
        }
    }

    /// <summary>
    /// Distance to move the Fire Eater on x before getting up on a drunken dodge.
    /// </summary>
    [SerializeField] private float xMovementAfterDrunkenDodge = 1.5f;

    /// <summary>Backing field for <see cref="DrunkJumpForce"/>.</summary>
    [SerializeField] private int drunkJumpForce = 200;

    /// <summary>
    /// Force to apply when performing a jump when drunk.
    /// </summary>
    public int DrunkJumpForce
    {
        get { return drunkJumpForce; }
        set
        {
            if (value < 0) value = 0;
            drunkJumpForce = value;
        }
    }
    #endregion

    #region Memory
    /// <summary>
    /// Timer used to make the Fire Eater sober up, in the GetDrunkCoroutine method.
    /// </summary>
    [SerializeField] private float soberUpTimer = 0;
    #endregion

    #region Sounds
    /// <summary>
    /// Audio track to play when crashing.
    /// </summary>
    [SerializeField] private AudioClip crashSound = null;

    /// <summary>
    /// Audio track to play when drinking.
    /// </summary>
    [SerializeField] private AudioClip drinkSound = null;

    /// <summary>
    /// Audio track to play when spitting fire.
    /// </summary>
    [SerializeField] private AudioClip fireBreathSound = null;

    /// <summary>
    /// Audio track to play when puking.
    /// </summary>
    [SerializeField] private AudioClip pukeSound = null;

    /// <summary>
    /// Audio track to play when spitting a fire ball.
    /// </summary>
    [SerializeField] private AudioClip spitFireBallSound = null;

    /// <summary>
    /// Audio track to play when spitting alcohol.
    /// </summary>
    [SerializeField] private AudioClip spitSound = null;


    /// <summary>
    /// Audio track to play when hitting damageable with extinct torch.
    /// </summary>
    [SerializeField] private AudioClip[] attackExtinctHit = null;

    /// <summary>
    /// Audio track to play when hitting damageable with lighting torch.
    /// </summary>
    [SerializeField] private AudioClip[] attackFiretHit = null;
    #endregion

    #region Animator
    private readonly int fire_Hash = Animator.StringToHash("Fire");
    private readonly int fireID_Hash = Animator.StringToHash("FireID");
    private readonly int isDrunk_Hash = Animator.StringToHash("IsDrunk");
    private readonly int isInMiniGame_Hash = Animator.StringToHash("IsInMiniGame");
    private readonly int miniGameSpeed_Hash = Animator.StringToHash("MiniGameSpeed");
    private readonly int puke_Hash = Animator.StringToHash("Puke");
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Drunk
    /// <summary>
    /// Starts the coroutine to make the Fire Eater get drunk.
    /// </summary>
    public void GetDrunk() => StartCoroutine(GetDrunkCorourine());

    /// <summary>
    /// Makes the Fire Eater get drunk, and sober him up after a certain time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetDrunkCorourine()
    {
        IsDrunk = true;
        SpeedCoef *= drunkSpeedCoef;
        SetFireEaterAnim(FireEaterAnimState.Drunk);

        soberUpTimer = soberUpTime;

        while (soberUpTimer > 0)
        {
            yield return null;
            soberUpTimer -= Time.deltaTime;
        }

        SpeedCoef /= drunkSpeedCoef;
        IsDrunk = false;
        SetFireEaterAnim(FireEaterAnimState.Sober);
    }

    /// <summary>
    /// Call this before starting to get up from ground after a drunken dodge to correctly set the character position.
    /// </summary>
    public void MoveAfterDrunkenDodge()
    {
        MoveTo(transform.position + (Vector3.right * isFacingRight.ToSign() * xMovementAfterDrunkenDodge));
    }
    #endregion

    #region MiniGame
    /// <summary>
    /// Make the mini game fun !
    /// </summary>
    /// <param name="_buttonType">Type of the button to held.</param>
    /// <returns></returns>
    private IEnumerator MiniGame(ButtonType _buttonType)
    {
        SetFireEaterAnim(FireEaterAnimState.MiniGame);

        isInMiniGame = true;
        OnTriggerMiniGame = () => SetFireEaterAnim(FireEaterAnimState.DoNotSpit);

        // Timer before showing the mini game
        float _timer = .1f;
        while (isInMiniGame && (_timer > 0))
        {
            if (!controller.GetButton(_buttonType))
            {
                isInMiniGame = false;
                break;
            }

            yield return null;
            _timer -= Time.deltaTime;
        }

        // While maintaining the attack button and being in mini game (it can be cancelled when hit, or when ending), play it
        if (isInMiniGame)
        {
            IsInMiniGame = true;
            animator.SetFloat(miniGameSpeed_Hash, Random.Range(.5f, 1f));

            while (isInMiniGame)
            {
                yield return null;

                if (!controller.GetButton(_buttonType))
                {
                    IsInMiniGame = false;
                    break;
                }
            }
        }

        // Triggers associated mini game state action
        OnTriggerMiniGame?.Invoke();
        SetFireEaterAnim(FireEaterAnimState.Fire);
    }

    /// <summary>
    /// Set the Fire Eater mini game state.
    /// </summary>
    /// <param name="_state"></param>
    public void SetMiniGameState(int _state)
    {
        if (!photonView.isMine) return;

        switch (_state)
        {
            case 0:
                OnTriggerMiniGame = () => SetFireEaterAnim(FireEaterAnimState.Spit);
                break;

            case 1:
                OnTriggerMiniGame = null;
                break;

            case 2:
                OnTriggerMiniGame = () => SetFireEaterAnim(FireEaterAnimState.DoNotSpit);
                OnTriggerMiniGame += GetDrunk;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Exit the mini game.
    /// </summary>
    public void ExitMiniGame()
    {
        OnTriggerMiniGame = () => SetFireEaterAnim(FireEaterAnimState.DoNotSpit);
        IsInMiniGame = false;
    }
    #endregion

    #region Actions
    /// <summary>
    /// Performs the catch attack of this player.
    /// </summary>
    public override void Catch()
    {
        if (isDrunk) return;

        base.Catch();
    }

    /// <summary>
    /// Make the player dodge.
    /// </summary>
    public override void StartDodge()
    {
        if (isDrunk && (!isGrounded || isJumping || isAttacking)) return;
        base.StartDodge();
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
        if (isDodging)
        {
            while (isDodging)
            {
                yield return null;

                if (dodgeTimer > DODGE_MINIMUM_TIMER)
                {
                    StopDodge();
                    SetAnimOnline(PlayerAnimState.Dodge);
                    break;
                }
            }

            yield return null;
        }

        ButtonType _buttonType = _isLight ? ButtonType.LightAttack : ButtonType.HeavyAttack;
        float _timer = TIME_TO_ACTIVATE_MINI_GAME;

        if (!isDrunk && isGrounded && !isJumping)
        {
            while (controller.GetButton(_buttonType))
            {
                if (_timer > 0)
                {
                    yield return null;
                    _timer -= Time.deltaTime;
                }
                else
                {
                    StartCoroutine(MiniGame(_buttonType));
                    yield return null;
                    break;
                }
            }
        }

        // Executes the attack
        Attack(_isLight);
        PreparingAttackCoroutine = null;
        yield break;
    }

    /// <summary>
    /// Make the Fire Eater puke.
    /// </summary>
    public void Puke()
    {
        IsAttacking = true;
        SetFireEaterAnim(FireEaterAnimState.Puke);
    }

    /// <summary>
    /// Spit a fire ball in front of the Fire Eater.
    /// </summary>
    /// <param name="_isUltra">Set 0 for small fire ball, anything else for ultra.
    /// 'Just cause animation don't take a boolean... What a waste.</param>
    public void SpitFireBall(int _isUltra)
    {
        if (!PhotonNetwork.isMasterClient) return;

        GameObject _fireBall = PhotonNetwork.Instantiate("FireBall", transform.position + (transform.right * .03f) + (Vector3.up * 1.35f), transform.rotation, 0);

        _fireBall.GetComponentInChildren<TDS_HitBox>().Activate(attacks[_isUltra == 0 ? 12 : 13], this);
        if (_isUltra != 0) _fireBall.transform.localScale *= 1.15f;
    }

    /// <summary>
    /// Makes the player start preparing an attack. This is the method called just before calling the <see cref="PrepareAttack(bool)"/> coroutine.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public override void StartPreparingAttack(bool _isLight)
    {
        if (isDrunk && (isAttacking || !isGrounded || isJumping)) return;

        base.StartPreparingAttack(_isLight);
    }

    /// <summary>
    /// Performs the Super attack if the gauge is filled enough.
    /// </summary>
    public override void SuperAttack()
    {
        if (isDrunk) return;

        base.SuperAttack();
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

        // Stop being drunk man, you're dead
        if (isDrunk) soberUpTimer = 0;
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        if (base.TakeDamage(_damage))
        {
            if (photonView.isMine && isInMiniGame) ExitMiniGame();
            return true;
        }
        return false;
    }
    #endregion

    #region Movements
    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();

        miniGameAnchor.Rotate(Vector3.up, 180);
        miniGameAnchor.localScale = new Vector3(miniGameAnchor.localScale.x, miniGameAnchor.localScale.y, miniGameAnchor.localScale.z * -1);
    }

    /// <summary>
    /// Starts a brand new jump !
    /// </summary>
    public override void StartJump()
    {
        if (isDrunk)
        {
            if (isDodging) return;

            // Adds a different force when drunk
            rigidbody.AddForce(Vector3.up * drunkJumpForce);
            return;
        }

        base.StartJump();
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set an animation state of the Fire Eater, used in the animator.
    /// </summary>
    /// <param name="_state">State to set in animation.</param>
    public void SetFireEaterAnim(FireEaterAnimState _state)
    {
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetFireEaterAnim"), new object[] { (int)_state });
        }

        switch (_state)
        {
            case FireEaterAnimState.Sober:
                animator.SetBool(isDrunk_Hash, false);
                break;

            case FireEaterAnimState.Drunk:
                animator.SetBool(isDrunk_Hash, true);
                break;

            case FireEaterAnimState.MiniGame:
                animator.ResetTrigger(fire_Hash);
                animator.SetInteger(fireID_Hash, 9999999);
                break;

            case FireEaterAnimState.Spit:
                animator.SetInteger(fireID_Hash, 9999999);
                break;

            case FireEaterAnimState.DoNotSpit:
                animator.SetInteger(fireID_Hash, -9999999);
                break;

            case FireEaterAnimState.Fire:
                animator.SetTrigger(fire_Hash);
                break;

            case FireEaterAnimState.Puke:
                animator.SetTrigger(puke_Hash);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set an animation state of the Fire Eater, used in the animator.
    /// </summary>
    /// <param name="_state">State to set in animation.</param>
    public void SetFireEaterAnim(int _state)
    {
        SetFireEaterAnim((FireEaterAnimState)_state);
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Interacts with the nearest available object in range.
    /// </summary>
    /// <returns>Returns true if interacted with something. False if nothing was found.</returns>
    public override bool Interact()
    {
        if (isDrunk)
        {
            Puke();
            return false;
        }

        return base.Interact();
    }
    #endregion

    #region Sounds
    /// <summary>
    /// Plays sound for when crashing on the floor.
    /// </summary>
    protected void PlayCrash()
    {
        // Play crash
    }

    /// <summary>
    /// Plays sound for when drinking.
    /// </summary>
    protected void PlayDrink()
    {
        // Play drink
    }

    /// <summary>
    /// Plays sound for when spitting fire.
    /// </summary>
    protected void PlayFireBreath()
    {
        // Play fire breath
    }

    /// <summary>
    /// Plays sound for when puking.
    /// </summary>
    protected void PlayPuke()
    {
        // Play puke
    }

    /// <summary>
    /// Plays sound for when spitting fire ball.
    /// </summary>
    protected void PlaySpitFireBall()
    {
        // Play spit fire ball
    }

    /// <summary>
    /// Plays sound for when spitting alcohol.
    /// </summary>
    protected void PlaySpit()
    {
        // Play spit
    }


    /// <summary>
    /// Plays sound for when hitting something with an extinct torch.
    /// </summary>
    protected void PlayExtinctAttack()
    {
        // Play extinct attack
    }

    /// <summary>
    /// Plays sound for when hitting something with a lighting torch.
    /// </summary>
    protected void PlayFireAttack()
    {
        // Play fire attack
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Set player type, just in case
        PlayerType = PlayerType.FireEater;
    }
    #endregion

    #endregion
}
