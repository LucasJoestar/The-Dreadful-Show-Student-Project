﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.Events; 
using Photon;

[RequireComponent(typeof(PhotonView))]
public class TDS_SpawnerArea : PunBehaviour
{
    /* TDS_SpawnerArea :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	 - Contains all spawn points of the zone.
     *	 - Call every waves when the previous one is cleared by the players
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     *	
	 *  Date :			[03/04/2019]
	 *	Author :		[Thiebaut Alexiss]
	 *
	 *	Changes :
	 *
	 *	    [Implement UnityEvents]
     *	        - OnNextWave: called when a wave is completed
     *	        - OnAreaActivated: called when a player activate the trigger 
     *	        - OnAreaDesactivated: called when all waves are completed
	 *
	 *	-----------------------------------
     *	
	 *  Date :			[18/02/2019]
	 *	Author :		[Thiebaut Alexiss]
	 *
	 *	Changes :
	 *
	 *	    [Implement ActivateSpawn Method]
     *	        - Called when a wave has to be activated
     *	        - Instanciate all enemies and store them into the spawnedEnemies list
	 *
	 *	-----------------------------------
     * 	Date :			[14/02/2019]
	 *	Author :		[Thiebaut Alexiss]
	 *
	 *	Changes :
	 *
	 *	    Remplacement de la list de spawn points par une liste de vagues qui sera parcourrue pour avancer dans les vagues
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[11/02/2019]
	 *	Author :		[Thiebaut Alexiss]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the SpawnerArea Class]
     *	    - Création de l'event OnNextWave
     *	    - Création des variables photonView, isLooping, waveIndex, wavesLength, deadEnemies, spawnedEnemies et spawnPoints
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Static event called each time a spawn area is being desactivated.
    /// </summary>
    public static event Action OnOneAreaDesactivated = null;

    /// <summary>
    /// This UnityEvent is called when the area is activated
    /// </summary>
    [SerializeField] private UnityEvent OnAreaActivated = null;
    /// <summary>
    /// This UnityEvent is called when the area is desactivated
    /// </summary>
    [SerializeField] private UnityEvent OnAreaDesactivated = null;
    /// <summary>
    /// This UnityEvent is called when a wave has to be started
    /// </summary>
    [SerializeField] public UnityEvent OnNextWave = null; 
    /// <summary>
    /// Called when the fight is starting 
    /// </summary>
    [SerializeField] private UnityEvent OnStartFight = null;
    #endregion

    #region Fields / Properties

    #region Constants
    /// <summary>
    /// Amount of minimum throwables that need to be linked to this area.
    /// </summary>
    public const int MINIMUM_THROWABLES = 1;
    #endregion

    #region Components and references
    #endregion

    #region Variables
    /// <summary>
    /// Is the area ready to spawn the enemies
    /// </summary>
    [SerializeField] private bool isReady = false;

    /// <summary>
    /// Is the area activated or not
    /// </summary>
    [SerializeField] private bool isActivated = false;

    public bool IsActivated => isActivated; 

    /// <summary>
    /// Does the area has to be called by event or by its trigger
    /// </summary>
        [SerializeField] protected bool isActivatedByEvent = false; 

    /// <summary>
    /// Is the area call the first wave when the last wave is over  
    /// </summary>
    [SerializeField] protected bool isLooping = false;

    public bool IsLooping { get { return isLooping; } }

    /// <summary>
    /// Interval at which the area reactive itself if looping. 
    /// </summary>
    [SerializeField] private int loopInterval = 0;

    /// <summary>
    /// Current index of the area
    /// Increase this value when  
    /// </summary>
    private int waveIndex = 0;

    /// <summary>
    /// List of dead enemies enemies belonging to this area
    /// </summary>
    private List<TDS_Enemy> deadEnemies = new List<TDS_Enemy>();

    /// <summary>
    /// List of enemies belonging to this area
    /// </summary>
    [SerializeField] private List<TDS_Enemy> spawnedEnemies = new List<TDS_Enemy>();

    public List<TDS_Enemy> SpawnedEnemies { get { return spawnedEnemies; } }

    /// <summary>
    /// Get an array of all active enemies for all activated spawn areas.
    /// </summary>
    public static TDS_Enemy[] ActiveEnemies { get { return ActivatedAreas.SelectMany(a => a.spawnedEnemies).ToArray(); } }

    /// <summary>
    /// List of the currently activated spawn areas.
    /// </summary>
    public static List<TDS_SpawnerArea> ActivatedAreas { get; private set; } = new List<TDS_SpawnerArea>();

    /// <summary>
    /// All throwables linked to this area.
    /// </summary>
    [SerializeField] private List<TDS_Throwable> areaThrowables = new List<TDS_Throwable>();

    /// <summary>Public accessor for <see cref="areaThrowables"/>.</summary>
    public List<TDS_Throwable> AreaThrowables { get { return areaThrowables; } }

    [SerializeField] List<TDS_Wave> waves = new List<TDS_Wave>();

    public bool IsDesactivated { get; private set; } = false; 
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Wait some time and activate the enemy
    /// </summary>
    /// <param name="_activatedEnemy">Enemy to activate</param>
    /// <param name="_hasToTaunt">Does the enemy has to taunt</param>
    /// <returns></returns>
    private IEnumerator WaitAndActivate(TDS_Enemy _activatedEnemy, bool _hasToTaunt)
    {
        yield return new WaitForSeconds(UnityEngine.Random.value);
        _activatedEnemy.ActivateEnemy(_hasToTaunt); 
    }

    /// <summary>
    /// Get the count of targeted players of a certain player type within the spanwed enemies
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public int GetPlayerTargetCount(PlayerType _type)
    {
        return spawnedEnemies.Where(e => e.PlayerTarget != null && e.PlayerTarget.PlayerType == _type).Count();
    }

    /// <summary>
    /// Get the enemies within a range around a player
    /// </summary>
    /// <param name="_player">Player targeted</param>
    /// <param name="_distance">Range around the player</param>
    /// <returns></returns>
    public int GetEnemyContactCount(TDS_Player _player, float _distance, TDS_Enemy _askingEnemy)
    {
        return spawnedEnemies.Where(e => Vector3.Distance(_player.transform.position, e.transform.position) < _distance && e != _askingEnemy).Count(); 
    }

    /// <summary>
    /// Return the number of enemies holding an object
    /// </summary>
    /// <returns></returns>
    public int GetEnemyThrowingCount()
    {
        return spawnedEnemies.Where(e => e.Throwable != null).Count(); 
    }

    public string GetMaxEnemyType()
    {

        int _bestCount = 0;
        string _name = string.Empty;
        if (spawnedEnemies.Where(e => e.EnemyName == "MightyMan(Clone)").ToList().Count > _bestCount)
        {
            return "MightyMan"; 
        }

        if (spawnedEnemies.Where(e => e.EnemyName == "Mime(Clone)").ToList().Count > _bestCount)
        {
            _name = "Mime";
            _bestCount = spawnedEnemies.OfType<TDS_Mime>().ToList().Count;
        }
        if (spawnedEnemies.Where(e => e.EnemyName == "Fakir(Clone)").ToList().Count > _bestCount)
        {
            _name = "Fakir";
            _bestCount = spawnedEnemies.OfType<TDS_Fakir>().ToList().Count; 
        }
        if (spawnedEnemies.Where(e => e.EnemyName == "Acrobat(Clone)").ToList().Count > _bestCount)
        {
            _name = "Acrobat";
            _bestCount = spawnedEnemies.OfType<TDS_Acrobat>().ToList().Count;
        }
        return _name;
    }

    /// <summary>
    /// Make spawn all enemies at every point of the wave index
    /// Increase Wave Index
    /// </summary>
    private void ActivateWave()
    {
        if (!PhotonNetwork.isMasterClient || IsDesactivated) return; 
        if (waves.Count == 0)
        {
            ActivatedAreas.Remove(this);
            return;
        }
        if (waveIndex >= waves.Count && !isLooping)
        {
            ActivatedAreas.Remove(this);
            isActivated = false;
            IsDesactivated = true;
            OnAreaDesactivated?.Invoke();
            OnOneAreaDesactivated?.Invoke();
            if (ActivatedAreas.Count == 0)
            {
                TDS_UIManager.Instance.SwitchCurtains(false);

                TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "SwitchCurtains", new object[] { false });
            }
            return;
        }
        else if(waveIndex >= waves.Count)
        {
            waveIndex = 0;
            Invoke("ActivateWave", loopInterval);
            return;
        }
        List<TDS_Enemy> _spawnedEnemies = waves[waveIndex].GetWaveEnemies(this);
        spawnedEnemies.AddRange(_spawnedEnemies);
        if (waves[waveIndex].IsActivatedByEvent)
        {
            foreach (TDS_Enemy e in _spawnedEnemies)
            {
                e.IsPacific = true;
                e.IsParalyzed = true;
            }
        }
        else
        {
            ActivateEnemies();
        }
        //If the wave is empty, start the next wave
        if (spawnedEnemies.Count == 0)
        {
            waveIndex++;
            OnNextWave?.Invoke();
        }
    }

    /// <summary>
    /// Add a new enemy to the list of spawned enemies
    /// </summary>
    /// <param name="_enemy">Enemy to add</param>
    public void AddEnemy(TDS_Enemy _enemy)
    {
        if(_enemy && !spawnedEnemies.Contains(_enemy))
        {
            spawnedEnemies.Add(_enemy);
            _enemy.Area = this; 
        }
    }

    /// <summary>
    /// Called when the Area has to be started
    /// When a player enter in the trigger or when the activation event is called
    /// Call the OnAreaActivated envent when the spawner area is ready and not activated yet
    /// </summary>
    public void StartSpawnArea()
    {
        if (PhotonNetwork.isMasterClient && isReady && !isActivated)
        {
            Action _removeEnemies = null;
            foreach (TDS_Enemy _enemy in spawnedEnemies)
            {
                if (_enemy.IsDead)
                {
                    _removeEnemies += () => spawnedEnemies.Remove(_enemy);
                    _removeEnemies += () => deadEnemies.Add(_enemy);
                }
                else _enemy.Area = this;
            }

            _removeEnemies?.Invoke();
            ActivatedAreas.Add(this);
            OnAreaActivated?.Invoke();

            foreach (TDS_Player _player in TDS_LevelManager.Instance.AllPlayers)
            {
                if (_player.Throwable) LinkThrowable(_player.Throwable);
                if (_player.PlayerType == PlayerType.Juggler)
                {
                    ((TDS_Juggler)_player).Throwables.ForEach(t => LinkThrowable(t));
                }
            }
        }
    }

    /// <summary>
    /// For each enemy call the method WaitAndActivate
    /// If the wave is activated by event, activate them with the taunt
    /// </summary>
    public void ActivateEnemies()
    {
        if (spawnedEnemies.Count == 0) return;

        if (waveIndex == 0)
        {
            OnStartFight?.Invoke();
            TDS_UIManager.Instance.SwitchCurtains(true);

            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "SwitchCurtains", new object[] { true });
        }
        spawnedEnemies.ForEach(e => StartCoroutine(WaitAndActivate(e, waves[waveIndex].IsActivatedByEvent)));
    }

    /// <summary>
    /// Destroy all dead enemies
    /// Clear the dead enemies list
    /// </summary>
    public void ClearDeadEnemies()
    {
        foreach (TDS_Enemy e in deadEnemies)
        {
            if (e)
            {
                TDS_VFXManager.Instance.SpawnEffect(FXType.MagicDisappear, e.transform.position);
                PhotonNetwork.Destroy(e.photonView);
            }
        }
        deadEnemies.Clear();
    }

    /// <summary>
    /// Remove the enemy from the spawnedEnemies list and add it to the dead enemies list
    /// If there is no more enemies, call the event OnNextWave
    /// </summary>
    /// <param name="_removedEnemy">Enemy to remove from the spawnedEnemies list</param>
    public void RemoveEnemy(TDS_Enemy _removedEnemy)
    {
        if (!PhotonNetwork.isMasterClient) return;
        spawnedEnemies.Remove(_removedEnemy);
        deadEnemies.Add(_removedEnemy);
        if (spawnedEnemies.Count == 0)
        {
            waveIndex++;
            OnNextWave?.Invoke();
        }
    }


    /// <summary>
    /// Initializes this spawn area.
    /// </summary>
    private void Initialize()
    {
        OnNextWave.AddListener(ActivateWave);
        OnAreaActivated.AddListener(ActivateWave);
        isReady = true;

        // Subscribe linked objects destruction to method
        foreach (TDS_Throwable _throwable in areaThrowables)
        {
            _throwable.OnDestroyed += () => RemoveThrowable(_throwable);
        }
        CheckRemainingObjects();
    }


    /// <summary>
    /// Checks the remaining objects linekd to this area, and executes actions depending on observed result.
    /// </summary>
    public void CheckRemainingObjects()
    {
        // If remaining not enough throwables and a Juggler is in game, just spawn an object supply box
        if ((areaThrowables.Count == MINIMUM_THROWABLES) && TDS_LevelManager.Instance.AllPlayers.Any(p => p && (p.PlayerType == PlayerType.Juggler) && !p.IsDead))
        {
            TDS_LevelManager.Instance.SpawnJugglerSupply();
        }
    }

    /// <summary>
    /// Link a throwable to this area.
    /// </summary>
    /// <param name="_throwable">Throwable to link.</param>
    public void LinkThrowable(TDS_Throwable _throwable)
    {
        if (!areaThrowables.Contains(_throwable))
        {
            areaThrowables.Add(_throwable);
            _throwable.OnDestroyed += () => RemoveThrowable(_throwable);
        }
    }

    /// <summary>
    /// Remove a throwable from this area.
    /// </summary>
    /// <param name="_throwable">Throwable to remove.</param>
    public void RemoveThrowable(TDS_Throwable _throwable)
    {
        if (areaThrowables.Contains(_throwable))
        {
            areaThrowables.Remove(_throwable);

            if (isActivated) CheckRemainingObjects();
        }
    }

    /// <summary>
    /// Stops the area.
    /// </summary>
    public void StopArea()
    {
        if (isLooping) isLooping = false;
        waveIndex = waves.Count;
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        OnAreaActivated.AddListener(() => isActivated = true);

        // Call it when the player is connected
        if (PhotonNetwork.isMasterClient) Initialize();
    }

    private void OnTriggerEnter(Collider _coll)
    {
        if (isActivatedByEvent || isActivated || !isReady || IsDesactivated) return; 
        // If a player enter in the collider
        // Start the waves and disable the collider
        if (_coll.GetComponent<TDS_Player>())
        {
            StartSpawnArea(); 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .2f); 
        for (int i = 0; i < waves.Count; i++)
        {
            Gizmos.color = waves[i].DebugColor;
            for (int j = 0; j < waves[i].SpawnPoints.Count; j++)
            {
                Gizmos.DrawLine(transform.position, waves[i].SpawnPoints[j].SpawnPosition);
                Gizmos.DrawSphere(waves[i].SpawnPoints[j].SpawnPosition, .1f);
            }

        }
    }

    /// <summary>
    /// Called when the area is not ready yet
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnJoinedRoom()
    {
        if (!isReady && PhotonNetwork.isMasterClient) Initialize();
    }
    #endregion

    #endregion
}
