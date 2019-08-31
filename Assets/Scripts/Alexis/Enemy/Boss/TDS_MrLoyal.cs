using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
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
    [SerializeField] private List<TDS_SpawnerArea> linkedArenas = new List<TDS_SpawnerArea>();
    [SerializeField] private float chargeCatsRate = 5f;
    [SerializeField] private TDS_Cat[] cats = null;
    [SerializeField] private Vector3 teleportationPosition = Vector3.zero; 
    #endregion

    #region Methods

    #region Original Methods            
    private void CallOut()
    {
        SetAnimationState((int)EnemyAnimationState.Taunt); 
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
        yield return new WaitForSeconds(1.5f);
        sprite.enabled = false;
        transform.position = teleportationPosition;
        TDS_VFXManager.Instance.SpawnEffect(FXType.MrLoyalEndTeleportation, teleportationPosition);
        yield return null;
        sprite.enabled = true; 
        /// Instantiate again particles on the teleportation spot
        TDS_SpawnerArea _currentArea = linkedArenas.Where(a => !a.IsDesactivated).FirstOrDefault();
        if(!_currentArea)
        {
            yield return GetBackIntoBattle(); 
            yield break; 
        }
        _currentArea.OnNextWave.AddListener(CallOut);
        _currentArea.StartSpawnArea();
        while (!_currentArea.IsDesactivated)
        {
            yield return new WaitForSeconds(chargeCatsRate);

            //Activate cats
        }
        yield return GetBackIntoBattle(); 
    }

    /// <summary>
    /// Set MrLoyal's State to get him out of the battle
    /// </summary>
    private void RemoveFromBattle()
    {
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
        yield return new WaitForSeconds(1.5f);
        sprite.enabled = false;
        transform.position = _pos;
        TDS_VFXManager.Instance.SpawnEffect(FXType.MrLoyalEndTeleportation, _pos);
        yield return null;
        sprite.enabled = true;
        yield return null; 
        SetEnemyState(EnemyState.MakingDecision);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        onHalfHealth.AddListener(RemoveFromBattle); 
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
