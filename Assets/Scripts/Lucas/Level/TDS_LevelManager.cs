using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

using Random = UnityEngine.Random;

public class TDS_LevelManager : PunBehaviour 
{
    /* TDS_LevelManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages the behaviour of a level.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	... Mhmm...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[01 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_LevelManager class.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Event called when a new checkpoint is set.
    /// </summary>
    public static Action<TDS_Checkpoint> OnCheckpointActivated = null;

    /// <summary>
    /// Event called when the current cutscene ends.
    /// </summary>
    public static event Action OnCutsceneEnds = null;
    #endregion

    #region Fields / Properties

    #region Variables
    /// <summary>
    /// Current playing cutscene.
    /// </summary>
    private PlayableDirector currentCutscene = null;

    /// <summary>
    /// Local player, the one who play on this machine.
    /// </summary>
    [SerializeField] protected TDS_Player localPlayer = null;

    /// <summary>Public accessor for <see cref="localPlayer"/>.</summary>
    public TDS_Player LocalPlayer { get { return localPlayer; } }

    /// <summary>
    /// Other players, that is the ones playing with you, guy. (Online and local)
    /// </summary>
    [SerializeField] protected List<TDS_Player> otherPlayers = new List<TDS_Player>();

    /// <summary>Public accessor for <see cref="otherPlayers"/>.</summary>
    public List<TDS_Player> OtherPlayers { get { return otherPlayers; } }

    /// <summary>
    /// Get all players of the game, local and online ones.
    /// </summary>
    public TDS_Player[] AllPlayers
    {
        get { return otherPlayers.Append(localPlayer).Where(p => p != null).ToArray(); }
    }


    /// <summary>
    /// Point where to spawn at game start.
    /// </summary>
    public Vector3 StartSpawnPoint = new Vector3();


    /// <summary>
    /// Juggler supplies available to be dropped in game.
    /// </summary>
    [SerializeField] private TDS_Destructible[] jugglerSupplies = new TDS_Destructible[] { };

    /// <summary>
    /// Dictionary referencing all scene objects tags with their ID as key and tags as value.
    /// </summary>
    public Dictionary<int, Tags> ObjectsTags = new Dictionary<int, Tags>();
    #endregion

    #region Coroutines
    /// <summary>
    /// Current coroutine to play narrator quotes.
    /// </summary>
    private Coroutine narratorCoroutine = null;
    #endregion

    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static TDS_LevelManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods

    #region Narrator
    /// <summary>
    /// Plays a quote from the narrator.
    /// </summary>
    /// <param name="_quote">Quote to play.</param>
    public void PlayNarratorQuote(TDS_NarratorQuote _quote)
    {
        if (narratorCoroutine != null)
        {
            StopCoroutine(narratorCoroutine);
        }

        narratorCoroutine = StartCoroutine(PlayNarratorQuoteCoroutine(_quote));
    }

    /// <summary>
    /// Plays a quote from the narrator.
    /// </summary>
    /// <param name="_quote">Quote to play.</param>
    /// <returns></returns>
    private IEnumerator PlayNarratorQuoteCoroutine(TDS_NarratorQuote _quote)
    {
        TDS_UIManager.Instance.ActivateNarratorBox(_quote.Quote);

        // If an audio track is linked, play it
        if (_quote.AudioTrack)
        {
            TDS_SoundManager.Instance.PlayNarratorQuote(_quote.AudioTrack);
            yield return new WaitForSeconds(_quote.AudioTrack.length);
        }
        else yield return new WaitForSeconds(_quote.Quote.Length / 15);

        TDS_UIManager.Instance.DesactivateNarratorBox();

        narratorCoroutine = null;
    }
    #endregion

    /// <summary>
    /// Make the player with the type contained in the GameManager spawn
    /// </summary>
    public void Spawn()
    {
        Vector2 _randomPos = new Vector2();

        if (PhotonNetwork.offlineMode)
        {
            for (int i = 0; i < TDS_GameManager.PlayersInfo.Count; i++)
            {
                _randomPos = Random.insideUnitCircle;

                TDS_PlayerInfo _info = TDS_GameManager.PlayersInfo[i];
                if ((_info != null) && (_info.PlayerType != PlayerType.Unknown))
                {
                    otherPlayers.Add((Instantiate(Resources.Load(_info.PlayerType.ToString()), new Vector3(StartSpawnPoint.x + _randomPos.x, StartSpawnPoint.y, StartSpawnPoint.z + _randomPos.y), Quaternion.identity) as GameObject).GetComponent<TDS_Player>());
                }
            }

            TDS_Camera.Instance.SetLocalMultiplayerCamera();
        }
        else if (TDS_GameManager.LocalPlayer != PlayerType.Unknown)
        {
            _randomPos = Random.insideUnitCircle;

            localPlayer = PhotonNetwork.Instantiate(TDS_GameManager.LocalPlayer.ToString(), new Vector3(StartSpawnPoint.x + _randomPos.x, StartSpawnPoint.y, StartSpawnPoint.z + _randomPos.y), Quaternion.identity, 0).GetComponent<TDS_Player>();
            TDS_Camera.Instance.Target = localPlayer.transform;
        }
    }

    /// <summary>
    /// Add the player in the list of online players
    /// </summary>
    /// <param name="_onlinePlayer">Player to add to the onlinePlayer List</param>
    public void InitOnlinePlayer(TDS_Player _onlinePlayer)
    {
        otherPlayers.Add(_onlinePlayer);
    }

    /// <summary>
    /// Plays a cutscene and call the <see cref="OnCutsceneEnds"/> event when it ends.
    /// </summary>
    /// <param name="_cutscene">PlayableDirector to play.</param>
    public void PlayCutscene(PlayableDirector _cutscene)
    {
        currentCutscene = _cutscene;
        if (_cutscene.time == 0) _cutscene.Play();
        _cutscene.stopped += OnCutsceneStopeed;
    }

    /// <summary>
    /// Called when the current cutscene is stopped.
    /// </summary>
    /// <param name="_cutscene">Cutscene that stopped.</param>
    private void OnCutsceneStopeed(PlayableDirector _cutscene)
    {
        if (_cutscene != currentCutscene) return;
        currentCutscene = null;
        _cutscene.stopped -= OnCutsceneStopeed;
        OnCutsceneEnds?.Invoke();
    }

    /// <summary>
    /// Remove the player with the selected id from the onlinePlayers list
    /// </summary>
    /// <param name="_playerId">Id of the removed player</param>
    public void RemoveOnlinePlayer(TDS_Player _player)
    {
        if (!_player)
        {
            Debug.LogError("Player Not Found"); 
            return;
        }
        TDS_UIManager.Instance.ClearUIRelatives(_player.PlayerType);
        if (otherPlayers.Contains(_player)) otherPlayers.Remove(_player);
    }

    /// <summary>
    /// Check the count of living players, if there is no player alive, reload the scene
    /// </summary>
    public IEnumerator CheckLivingPlayers()
    {
        if (!PhotonNetwork.offlineMode && localPlayer.IsDead)
        {
            yield return new WaitForSeconds(2f);

            //TDS_UIManager.Instance.ResetUIManager();
            if (OtherPlayers.All(p => p.IsDead)) TDS_UIManager.Instance.StartCoroutine(TDS_UIManager.Instance.ResetInGameUI());
            else if (OtherPlayers.Count > 0)
            {
                TDS_Camera.Instance.Target = OtherPlayers.Where(p => !p.IsDead).First().transform;
            }
            yield break; 
        }
        else if (AllPlayers.All(p => p.IsDead)) TDS_UIManager.Instance.StartCoroutine(TDS_UIManager.Instance.ResetInGameUI());
    }

    /// <summary>
    /// Skips the current cutscene.
    /// </summary>
    public void SkipCutscene()
    {
        if (!currentCutscene) return;
        currentCutscene.Stop();
    }

    /// <summary>
    /// Spawns a supply box for the Juggler on the playable zone.
    /// </summary>
    public void SpawnJugglerSupply()
    {
        if (jugglerSupplies.Length == 0) return;

        // Spawn supply !
        PhotonNetwork.Instantiate(jugglerSupplies[Random.Range(0, jugglerSupplies.Length)].name, new Vector3(Random.Range(TDS_Camera.Instance.CurrentBounds.XMin + 3, TDS_Camera.Instance.CurrentBounds.XMax - 3), 17.5f, Random.Range(TDS_Camera.Instance.CurrentBounds.ZMin + 2, TDS_Camera.Instance.CurrentBounds.ZMax - 2)), Quaternion.identity, 0);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance if null
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }
    #endregion

    #endregion
}
