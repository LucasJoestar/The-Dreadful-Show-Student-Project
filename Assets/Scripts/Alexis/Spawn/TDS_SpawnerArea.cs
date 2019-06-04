using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 
using Photon;

[RequireComponent(typeof(BoxCollider), typeof(PhotonView))]
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
    /// This UnityEvent is called when the area is activated
    /// </summary>
    [SerializeField] private UnityEvent OnAreaActivated;
    /// <summary>
    /// This UnityEvent is called when the area is desactivated
    /// </summary>
    [SerializeField] private UnityEvent OnAreaDesactivated;
    /// <summary>
    /// This UnityEvent is called when a wave has to be started
    /// </summary>
    [SerializeField] private UnityEvent OnNextWave;
    #endregion

    #region Fields / Properties

    #region Components and references
    #endregion

    #region Variables
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
    private List<TDS_Enemy> spawnedEnemies = new List<TDS_Enemy>();

    [SerializeField] List<TDS_Wave> waves = new List<TDS_Wave>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Make spawn all enemies at every point of the wave index
    /// Increase Wave Index
    /// </summary>
    public void ActivateSpawn()
    {
        if (!PhotonNetwork.isMasterClient) return;  
        if (waveIndex == waves.Count && !isLooping)
        {
            OnAreaDesactivated?.Invoke();
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "CallOnAreaDesactivatedEvent"), new object[] { });
            return;
        }
        else if(waveIndex == waves.Count)
        {
            waveIndex = 0;
        }
        spawnedEnemies.AddRange(waves[waveIndex].GetWaveEnemies(this));
        if(waves[waveIndex].IsActivatedByEvent)
        {
            foreach (TDS_Enemy e in spawnedEnemies)
            {
                e.IsPacific = true;
                e.IsParalyzed = true;
            }
        }
        //If the wave is empty, start the next wave
        if (spawnedEnemies.Count == 0)
        {
            waveIndex++;
            OnNextWave?.Invoke();
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "CallOnNextWaveEvent"), new object[] { });
        }
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
        if(spawnedEnemies.Count == 0)
        {
            waveIndex++;
            OnNextWave?.Invoke();
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "CallOnNextWaveEvent"), new object[] { });
        }
    }

    public void ActivateEnemies() => spawnedEnemies.ForEach(e => e.ActivateEnemy()); 

    private void CallOnAreaActivatedEvent() => OnAreaActivated?.Invoke();
    private void CallOnAreaDesactivatedEvent() => OnAreaDesactivated?.Invoke();
    private void CallOnNextWaveEvent() => OnNextWave?.Invoke(); 
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Call it when the player is connected
        if (!PhotonNetwork.isMasterClient) return;
        OnNextWave.AddListener(ActivateSpawn);
        OnAreaActivated.AddListener(ActivateSpawn);
    }

    private void Start()
    {
        if (TDS_UIManager.Instance)
        {
            OnAreaActivated.AddListener(TDS_UIManager.Instance.SwitchCurtains);
            OnAreaDesactivated.AddListener(TDS_UIManager.Instance.SwitchCurtains);
        }
        if(!PhotonNetwork.isMasterClient || isActivatedByEvent)
        {
            GetComponent<BoxCollider>().enabled = false; 
        }
    }

    private void OnTriggerEnter(Collider _coll)
    {
        // If a player enter in the collider
        // Start the waves and disable the collider
        if(_coll.GetComponent<TDS_Player>())
        {
            GetComponent<BoxCollider>().enabled = false;
            if (PhotonNetwork.isMasterClient)
            {
                OnAreaActivated?.Invoke();
                //Debug.Log("CALL by " + photonView.viewID); 
                TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "CallOnAreaActivatedEvent"), new object[] { });
            }
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
    #endregion

    #endregion
}
