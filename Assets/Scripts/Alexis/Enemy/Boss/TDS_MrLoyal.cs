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
    [SerializeField] private List<TDS_SpawnerArea> linkedAreas = new List<TDS_SpawnerArea>();
    [SerializeField] private float chargeCatsRate = 5f;
    [SerializeField] private TDS_Cat[] cats = null;
    [SerializeField] private Vector3 teleportationPosition = Vector3.zero;

    [SerializeField] private AudioClip fakirAudioClip       = null;
    [SerializeField] private AudioClip mimeAudioClip        = null;
    [SerializeField] private AudioClip acrobatAudioClip     = null;
    [SerializeField] private AudioClip mightyManAudioClip   = null;
    [SerializeField] private AudioClip catAudioClip         = null;
    [SerializeField] private AudioClip[] tauntAudioClips = null;
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
        if (!audioSource) return; 
        AudioClip _clip = null;
        switch (_enemyName)
        {
            case "Fakir":
                _clip = fakirAudioClip;
                break;
            case "Mime":
                _clip = mimeAudioClip; 
                break;
            case "Acrobat":
                _clip = acrobatAudioClip; 
                break;
            case "MightyMan":
                _clip = mightyManAudioClip; 
                break;
            case  "Cat":
                _clip = catAudioClip;
                break;
            default:
                return;
        }
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = _clip;
        audioSource.volume = .25f; 
        audioSource.Play(); 
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "PlayCallOutSound"), new object[] { _enemyName });
    }

    private void PlayRandomTaunt()
    {
        if (!audioSource || tauntAudioClips == null || tauntAudioClips.Length == 0) return;
        int _index = (int)Random.Range((int)0, (int)tauntAudioClips.Length); 
        AudioClip _clip = tauntAudioClips[_index];
        if (!_clip) return; 
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = _clip;
        audioSource.volume = .25f;
        audioSource.Play();
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "PlayTaunt"), new object[] { _index });
            Invoke("PlayRandomTaunt", Random.Range(tauntRateMin, tauntRateMax));
        }

    }

    private void PlayTaunt(int _index)
    {
        if (!audioSource || tauntAudioClips == null || tauntAudioClips.Length == 0) return;
        AudioClip _clip = tauntAudioClips[_index];
        if (!_clip) return;
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = _clip;
        audioSource.volume = .25f;
        audioSource.Play();
    }

    /// <summary>
    /// Make MrLoyal gets out of the battle, then make him call the cats at a rate
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetOutOfBattle()
    {
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
        TDS_Bounds _bounds = TDS_Camera.Instance?.CurrentBounds;
        Vector3 _pos = Vector3.zero; 
        if (_bounds == null)
        {
            _pos = TDS_LevelManager.Instance.AllPlayers.FirstOrDefault().transform.position; 
        }
        else
        {
            _pos = new Vector3((_bounds.XMax + _bounds.XMin)/2, 0, (_bounds.ZMin + _bounds.ZMax)/2); 
        }
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
        animator.SetBool("isOutOfBattle", false); 
        SetEnemyState(EnemyState.MakingDecision);
        Invoke("PlayRandomTaunt", Random.Range(tauntRateMin, tauntRateMax));
        if (isEnraged) cats.ToList().ForEach(c => c.SetCatIndependant()); 
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
