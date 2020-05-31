using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_MrLoyal : TDS_Boss 
{
    /* TDS_MrLoyal :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Animator
    private readonly int isOutOfBattle_Hash = Animator.StringToHash("isOutOfBattle");
    #endregion

    [SerializeField] private List<TDS_SpawnerArea> linkedAreas = new List<TDS_SpawnerArea>();
    [SerializeField] private float chargeCatsRate = 5f;
    [SerializeField] private TDS_Cat[] cats = null;
    [SerializeField] private Vector3 teleportationPosition = Vector3.zero;


    [SerializeField] private TDS_NarratorQuote fakirQuote       = null;
    [SerializeField] private TDS_NarratorQuote mimeQuote        = null;
    [SerializeField] private TDS_NarratorQuote acrobatQuote     = null;
    [SerializeField] private TDS_NarratorQuote mightyManQuote   = null;
    [SerializeField] private TDS_NarratorQuote catQuote         = null;
    [SerializeField] private TDS_NarratorQuote[] tauntQuotes    = null;
    [SerializeField] private float tauntRateMin = 3;
    [SerializeField] private float tauntRateMax = 25;

    private bool isEnraged = false;
    [SerializeField] private int bonusRageDamages = 10;
    [SerializeField] private float bonusSpeedCoefficient = 1.5f; 
    #endregion

    #region Methods

    #region Original Methods            
    private void CallOut()
    {
        SetAnimationState((int)EnemyAnimationState.Taunt);
        if (linkedAreas.Where(a => !a.IsDesactivated).FirstOrDefault() == null) return; 
        string _enemyName = linkedAreas.Where(a => !a.IsDesactivated).FirstOrDefault().GetMaxEnemyType();

        PlayCallOutSound(_enemyName);
    }

    private void PlayCallOutSound(string _enemyName)
    {
        TDS_NarratorQuote _quote = null; 
        switch (_enemyName)
        {
            case "Fakir":
                _quote = fakirQuote; 
                break;
            case "Mime":
                _quote = mimeQuote; 
                break;
            case "Acrobat":
                _quote = acrobatQuote; 
                break;
            case "MightyMan":
                _quote = mightyManQuote; 
                break;
            case  "Cat":
                _quote = catQuote; 
                break;
            default:
                return;
        }
        TDS_LevelManager.Instance.PlayNarratorQuote(_quote); 

        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, this.GetType(), "PlayCallOutSound", new object[] { _enemyName });
    }

    private void PlayRandomTaunt()
    {
        if (tauntQuotes == null || tauntQuotes.Length == 0) return;
        int _index = (int)Random.Range((int)0, (int)tauntQuotes.Length); 
        TDS_NarratorQuote _quote = tauntQuotes[_index];
        if (_quote == null) return;
        TDS_LevelManager.Instance?.PlayNarratorQuote(_quote);
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, this.GetType(), "PlayTaunt", new object[] { _index });
    }

    private void PlayTaunt(int _index)
    {
        if (tauntQuotes == null || tauntQuotes.Length == 0) return;
        TDS_NarratorQuote _quote = tauntQuotes[_index];
        if (_quote == null) return;
        TDS_LevelManager.Instance?.PlayNarratorQuote(_quote);
    }

    /// <summary>
    /// Make MrLoyal gets out of the battle, then make him call the cats at a rate
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetOutOfBattle()
    {
        // Remove from available enemies
        AllEnemies.Remove(this);

        /// Call the particle here
        TDS_VFXManager.Instance.SpawnEffect(FXType.MrLoyalTeleportation, transform.position); 
        /// Then wait some time and teleport Mr Loyal on his plateform
        yield return new WaitForSeconds(1.25f);
        sprite.enabled = false;
        transform.position = teleportationPosition;
        TDS_VFXManager.Instance.SpawnEffect(FXType.MrLoyalEndTeleportation, teleportationPosition);
        yield return null;
        sprite.enabled = true;
        yield return new WaitForSeconds(1f);
        /// Instantiate again particles on the teleportation spot
        TDS_SpawnerArea _currentArea = linkedAreas.Where(a => !a.IsDesactivated).FirstOrDefault();
        if(!_currentArea)
        {
            yield return GetBackIntoBattle(); 
            yield break; 
        }
        _currentArea.OnNextWave.AddListener(CallOut);
        _currentArea.StartSpawnArea();
        yield return null; 
        CallOut();
        float _timer = 0;
        while (!_currentArea.IsDesactivated)
        {
            yield return null;
            _timer += Time.deltaTime; 
            if(_timer > chargeCatsRate)
            {
                cats.ToList().ForEach(c => c.ActivateCat());
                PlayCallOutSound("Cat");
                _timer = 0; 
            }
        }
        yield return GetBackIntoBattle(); 
    }

    /// <summary>
    /// Set MrLoyal's State to get him out of the battle
    /// </summary>
    private void RemoveFromBattle()
    {
        CancelInvoke("PlayRandomTaunt");
        SetEnemyState(EnemyState.OutOfBattle);
    }

    private IEnumerator GetBackIntoBattle()
    {
        SetAnimationState((int)EnemyAnimationState.Idle);
        TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;

        //if (_bounds == null)
        //{
        //    _pos = TDS_LevelManager.Instance.AllPlayers.FirstOrDefault().transform.position; 
        //}
        Vector3 _pos = new Vector3((_bounds.XMax + _bounds.XMin) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2);

        //Reinstantiate the particle here
        TDS_VFXManager.Instance.SpawnEffect(FXType.MrLoyalTeleportation, transform.position);
        yield return new WaitForSeconds(1.25f);
        sprite.enabled = false;
        transform.position = _pos;
        TDS_VFXManager.Instance.SpawnEffect(FXType.MrLoyalEndTeleportation, _pos);
        yield return null;
        sprite.enabled = true;
        yield return null;
        //ActivateEnemy();
        animator.SetBool(isOutOfBattle_Hash, false); 
        SetEnemyState(EnemyState.MakingDecision);
        Invoke("PlayRandomTaunt", Random.Range(tauntRateMin, tauntRateMax));
        if (isEnraged) cats.ToList().ForEach(c => c.SetCatIndependant());

        // Add to battle
        AllEnemies.Add(this);
    }

    public override void ActivateEnemy(bool _hastoTaunt = false)
    {
        if (!PhotonNetwork.isMasterClient || IsDead) return;
        IsPacific = false;
        IsParalyzed = false;
        RemoveFromBattle(); 
    }

    protected override void Die()
    {
        base.Die();
        CancelInvoke("PlayRandomTaunt");
        cats.ToList().ForEach(c => c.DesactivateCat());
        foreach (TDS_Enemy e in AllEnemies)
        {
            e.IsPacific = true;
            e.IsParalyzed = true;
            e.StopAll();
        }
    }

    protected override float StartAttack()
    {
        if (isEnraged) SetBonusDamages(bonusRageDamages); 
        return base.StartAttack();
    }

    private void Enrage()
    {
        if (isEnraged) return; 
        isEnraged = true;
        speedCoef *= bonusSpeedCoefficient;
        SetAnimationState((int)EnemyAnimationState.Rage); 
    }

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        onHalfHealth.AddListener(RemoveFromBattle);
        onHalfHealth.AddListener(Enrage); 
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        ActivateEnemy(); 
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(teleportationPosition, .25f);
        Gizmos.DrawLine(transform.position, teleportationPosition);
    }
    #endregion

    #endregion
}
