using AK.Wwise;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Attack", order = 1), Serializable]
public class TDS_Attack : ScriptableObject
{
    /* TDS_Attack :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class used to reference an attack and everything needed about it.
     *	    
     *	    The classes inheriting from TDS_Player use this, but the ones inheriting from TDS_Enemy use TDS_EnemyAttack, which inherit from this class.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[08 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the GetDamages property.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Attack class.
     *	
     *	    - Added the AnimationID, Description & Name fields ; and the damagesMax & damagesMin fields and properties.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

#if UNITY_EDITOR
    /// <summary>Backing field for <see cref="AttackName"/>.</summary>
    [SerializeField] protected string attackName = "New Attack"; 

    /// <summary>
    /// Name of this attack, used to reference it.
    /// </summary>
    public string AttackName { get { return attackName; } }
#endif


    /// <summary>
    /// Effect associated with this attack.
    /// </summary>
    public TDS_AttackEffect Effect = new TDS_AttackEffect();


    /// <summary>Backing field for <see cref="DamagesMax"/>.</summary>
    [SerializeField] protected int damagesMax = 1;

    /// <summary>
    /// The maximum amount of damages this attack can inflict.
    /// It must always be a positive value.
    /// </summary>
    public int DamagesMax
    {
        get { return damagesMax; }
        set
        {
            if (value < 0) value = 0;
            damagesMax = value;

            if (damagesMin > value) DamagesMin = value;
        }
    }

    /// <summary>Backing field for <see cref="DamagesMin"/>.</summary>
    [SerializeField] protected int damagesMin = 1;

    /// <summary>
    /// The minimum amount of damages this attack can inflict.
    /// Always superior to zero, and never superior to <see cref="DamagesMax"/>
    /// </summary>
    public int DamagesMin
    {
        get { return damagesMin; }
        set
        {
            value = Mathf.Clamp(value, 0, damagesMax);
            damagesMin = value;
        }
    }

    /// <summary>
    /// Get random damages from this attack.
    /// </summary>
    public int GetDamages
    {
        get { return Random.Range(damagesMin, damagesMax + 1); }
    }

#if UNITY_EDITOR
    [SerializeField, TextArea] protected string description = string.Empty;
#endif

    [Header("Sound")]

    [SerializeField] private string soundEvent = "ENEMY";
    [SerializeField] private float soundRTPCValue = .1f;
    #endregion

    #region Methods
    /// <summary>
    /// Attacks the target, by inflicting damages and applying effects.
    /// </summary>
    /// <param name="_attacker">The HitBox attacking the target.</param>
    /// <param name="_target">Target to attack.</param>
    /// <returns>Returns -2 if target didn't take any damages, -1 if the target is dead, 0 if no effect could be applied, and 1 if everything went good.</returns>
    public virtual int Attack(TDS_HitBox _attacker, TDS_Damageable _target)
    {
        // Roll the dice to know if the attack effect should be applied
        int _percent = Random.Range(1, 100);
        bool _noEffect = (_percent > Effect.PercentageLowest) && ((_percent > Effect.PercentageHighest) || (_percent > Random.Range(Effect.PercentageLowest, Effect.PercentageHighest + 1)));

        // Get attack damages
        int _damages = GetDamages + _attacker.BonusDamages;
        if (!_noEffect) _damages += Random.Range(Effect.DamagesMin, Effect.DamagesMax + 1);

        bool _doPutOnGround = false;

        // If a player performs an attack from behind, increase damages and try to put on ground
        if (_attacker.Owner && (_target is TDS_Character _targetC) && _attacker.transform.parent && (_attacker.Owner is TDS_Player) && (Mathf.Sign(_attacker.transform.position.x - (_target.transform.position.x + (.5f * (!_targetC.IsFacingRight).ToSign()))) != _targetC.IsFacingRight.ToSign()))
        {
            _damages = _damages * 2;

            // Get 1 / 4 chance to put target on ground
            _doPutOnGround = _noEffect && (Random.Range(1, 100) < 25);
        }

        // Inflict damages, and return if target don't get hurt, if no effect or if target is dead
        if ((_damages < 1)  || !_target.TakeDamage(_damages, _attacker.Collider.bounds.center)) return -2;

        // Increase score
        if ((_attacker.Owner is TDS_Player _player) && (_target is TDS_Enemy _enemyTarget))
        {
            for (int _i = 0; _i < TDS_GameManager.PlayersInfo.Count; _i++)
            {
                if (TDS_GameManager.PlayersInfo[_i].PlayerType == _player.PlayerType)
                {
                    TDS_GameManager.PlayersInfo[_i].PlayerScore.IncreaseInflictedScore(_enemyTarget, _damages);
                }
            }
        }
        else if ((_target is TDS_Player _playerTarget) && (_attacker.Owner is TDS_Enemy _enemy))
        {
            for (int _i = 0; _i < TDS_GameManager.PlayersInfo.Count; _i++)
            {
                if (TDS_GameManager.PlayersInfo[_i].PlayerType == _playerTarget.PlayerType)
                {
                    TDS_GameManager.PlayersInfo[_i].PlayerScore.IncreaseSuffuredScore(_enemy, _damages, _playerTarget.IsDead); ;
                }
            }
        }

        if (_target.IsDead) return -1;
        if (_doPutOnGround)
        {
            ((TDS_Character)_target).PutOnTheGround();
            return 1;
        }
        if (_noEffect) return 0;

        // Apply attack effect
        switch (Effect.EffectType)
        {
            case AttackEffectType.None:
                // Nothing to see here
                break;

            // Put target on ground
            case AttackEffectType.PutOnTheGround:
                if (_target is TDS_Character) ((TDS_Character)_target).PutOnTheGround();
                break;

            // Bring target closer
            case AttackEffectType.BringCloser:
                _target.BringCloser(_target.transform.position.x - (_attacker.Owner ? _attacker.Owner.transform.position.x : _attacker.transform.position.x));
                if (_attacker.Owner is TDS_Enemy _enemy)
                {
                    _enemy.SetAnimationState((int)EnemyAnimationState.BringTargetCloser);
                    _enemy.BringingTarget = _target;
                    _target.OnStopBringingCloser += _enemy.TargetBrought;
                }
                break;

            // Apply recoil to target
            case AttackEffectType.Knockback:
                //_target.Knockback(_target.transform.position.x >= _attacker.transform.position.x);
                break;

            // Project the target in the air
            case AttackEffectType.Project:
                //_target.Project(_target.transform.position.x >= _attacker.transform.position.x);
                break;

            default:
                break;
        }

        return 1;
    }

    public void PlaySound(GameObject _object)
    {
        //AkSoundEngine.SetRTPCValue("ennemies_attack", soundRTPCValue, _object);
        AkSoundEngine.SetSwitch("ENNEMIS_ATTACK_SWITCH", "attaque_1", _object);
        AkSoundEngine.PostEvent(soundEvent, _object);
    }
    #endregion
}
