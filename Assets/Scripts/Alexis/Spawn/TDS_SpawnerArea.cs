using System;
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
    public const int MINIMUM_THROWABLES = 2;
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

    /// <summary>
    /// Does the area has to be called by event or by its trigger
    /// </summary>
    [SerializeField] protected bool isActivatedByEvent = false; 

    /// <summary>
    /// Is the area call the first wave when the last wave is over  
    /// </summary>
    [SerializeField] protected bool isLooping = false;

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

    /// <summary>
    /// Make spawn all enemies at every point of the wave index
    /// Increase Wave Index
    /// </summary>
    private void ActivateWave()
    {
        if (!PhotonNetwork.isMasterClient) return; 
        if (waves.Count == 0)
        {
            ActivatedAreas.Add(this);
            return;
        }
        if (waveIndex >= waves.Count && !isLooping)
        {
            ActivatedAreas.Remove(this);
            IsDesactivated = true; 
            OnAreaDesactivated?.Invoke();
            OnOneAreaDesactivated?.Invoke();
            TDS_UIManager.Instance.SwitchCurtains(false);
            return;
        }
        else if(waveIndex >= waves.Count)
        {
            waveIndex = 0;
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

            TDS_Player _juggler = TDS_LevelManager.Instance.AllPlayers.Where(p => p.PlayerType == PlayerType.Juggler).FirstOrDefault();

            if (_juggler) ((TDS_Juggler)_juggler).Throwables.ForEach(t => LinkThrowable(t));
        }
    }

    /// <summary>
    /// For each enemy call the method WaitAndActivate
    /// If the wave is activated by event, activate them with the taunt
    /// </summary>
    public void ActivateEnemies()
    {
        if(waveIndex == 0)
        {
            OnStartFight?.Invoke();
            TDS_UIManager.Instance.SwitchCurtains(true);
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
            PhotonNetwork.Destroy(e.gameObject);
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
        if (((areaThrowables.Count == MINIMUM_THROWABLES) || (areaThrowables.Count == 0)) && TDS_LevelManager.Instance.AllPlayers.Any(p => p && (p.PlayerType == PlayerType.Juggler) && !p.IsDead))
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
        if (isActivatedByEvent || isActivated || !isReady) return; 
        // If a player enter in the collider
        // Start the waves and disable the collider
        if(_coll.GetComponent<TDS_Player>())
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
