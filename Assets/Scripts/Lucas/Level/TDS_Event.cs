using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

using Object = UnityEngine.Object;

#pragma warning disable 0649
[Serializable]
public class TDS_Event 
{
    /* TDS_Event :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Event object used in an event system, triggering something.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	    • Implement RPC methods if not local.
     *	    
     *	    • Wait for a specific action
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Event class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Name of this event.
    /// </summary>
    public string Name = "New Event";

    /// <summary>
    /// Does this event require a specific player type in game.
    /// </summary>
    [SerializeField] private bool doRequireSpecificPlayerType = false;

    /// <summary>
    /// Indicates if the event is waiting for something or not.
    /// </summary>
    public bool IsWaiting { get; private set; } = true;

    /// <summary>
    /// Type of this event.
    /// </summary>
    [SerializeField] private CustomEventType eventType = CustomEventType.UnityEventLocal;

    /// <summary>Public accessor for <see cref="eventType"/>.</summary>
    public CustomEventType EventType { get { return eventType; } }

    /// <summary>
    /// Delay to wait before starting processing this event.
    /// </summary>
    [SerializeField] private float delay = 0;

    /// <summary>
    /// Float associated with this event.
    /// </summary>
    [SerializeField] private float eventFloat = 1;

    /// <summary>
    /// Time during which the camera will wait in front of its target before reset.
    /// </summary>
    [SerializeField] private float cameraWaitTime = 1;

    /// <summary>
    /// Integer associated with this event.
    /// </summary>
    [SerializeField] private int eventInt = 1;

    /// <summary>
    /// Prefab to instantiate.
    /// </summary>
    [SerializeField] private GameObject prefab = null;

    /// <summary>
    /// Narrator quote linked to the event.
    /// </summary>
    [SerializeField] private TDS_NarratorQuote quote = null;

    /// <summary>
    /// Cutscene to play during this event.
    /// </summary>
    [SerializeField] private PlayableDirector cutscene = null;

    /// <summary>
    /// Player type required to execute this action.
    /// If unknown, then no specific player type is required.
    /// </summary>
    [SerializeField] private PlayerType playerType = PlayerType.Unknown;

    /// <summary>
    /// Event string, used for text ID to load a narrator quote / information box text, or as name of a prefab to instantiate with Photon.
    /// </summary>
    [SerializeField] private string eventString = "ID";

    /// <summary>
    /// Transform used for the event.
    /// </summary>
    [SerializeField] private Transform eventTransform = null;

    /// <summary>
    /// UI Quotes to set.
    /// </summary>
    [SerializeField] private TDS_UIQuote[] uiQuotes = null;

    /// <summary>
    /// Unity event to invoke.
    /// </summary>
    [SerializeField] private UnityEvent unityEvent;

    /// <summary>
    /// Action to wait player to perform.
    /// </summary>
    [SerializeField] private WaitForAction actionType = WaitForAction.UseRabbit;
    #endregion

    #region Methods
    /// <summary>
    /// Check left bound position, and stop waiting if set as expected.
    /// </summary>
    private void CheckBound()
    {
        if (TDS_Camera.Instance.CurrentBounds.XMin >= eventTransform.position.x) StopWaiting();
    }

    /// <summary>
    /// Stop waiting. Just stop it.
    /// </summary>
    public void StopWaiting() => IsWaiting = false;

    /// <summary>
    /// Checks if the object that died has the required tag to complete the event.
    /// </summary>
    /// <param name="_objectTags">Tags of the object that just died.</param>
    private void CheckObjectDeath(GameObject _object)
    {
        if (_object.HasTag(eventString))
        {
            eventInt--;

            if (eventInt <= 0) StopWaiting();
        }
    }

    /// <summary>
    /// Decreases the counter of spawn area to wait for desactivation.
    /// </summary>
    public void DeductSpawnArea()
    {
        eventInt--;

        if (eventInt <= 0) StopWaiting();
    }

    /// <summary>
    /// Triggers this event.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Trigger()
    {
        // If this event requires a specific player type and he's not in the game, just skip this event
        if (doRequireSpecificPlayerType && !TDS_LevelManager.Instance.AllPlayers.Any(p => p.PlayerType == playerType)) yield break;

        // Wait for delay
        yield return new WaitForSeconds(delay);

        switch (eventType)
        {
            // Activate UI curtains
            case CustomEventType.ActivateCurtains:
                TDS_UIManager.Instance.SwitchCurtains(true);
                break;

            // Make a camera movement
            case CustomEventType.CameraMovement:
                yield return TDS_Camera.Instance.LookTarget(eventTransform.position.x, eventTransform.position.y, eventTransform.position.z, cameraWaitTime, eventFloat);
                break;

            // Desactivate the current information box
            case CustomEventType.DesactiveInfoBox:
                if (doRequireSpecificPlayerType)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateDialogBox")
                      , new object[] { });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateDialogBox")
                      , new object[] { });
                }
                break;

            // Display a message in an information box
            case CustomEventType.DisplayInfoBox:
                // Activate the info box for the requested player or for everyone
                if (doRequireSpecificPlayerType)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateDialogBox")
                      , new object[] { TDS_GameManager.GetDialog(eventString)[0] });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateDialogBox")
                      , new object[] { TDS_GameManager.GetDialog(eventString)[0] });
                }
                break;

            // Freeze local player
            case CustomEventType.FreezePlayerForCutscene:
                TDS_GameManager.IsInCutscene = true;
                TDS_UIManager.Instance.ActivateCutsceneBlackBars();
                if (!PhotonNetwork.offlineMode) TDS_LevelManager.Instance.LocalPlayer.FreezePlayer();
                else
                {
                    foreach (TDS_Player _player in TDS_LevelManager.Instance.AllPlayers)
                    {
                        _player.FreezePlayer();
                    }
                }
                break;

            // Instantiate a prefab
            case CustomEventType.Instantiate:
                GameObject _object = Object.Instantiate(prefab, eventTransform.position, eventTransform.rotation);
                _object.transform.SetParent(eventTransform, true);
                break;

            // Instantiate with Photon
            case CustomEventType.InstantiatePhoton:
                PhotonNetwork.Instantiate(prefab.name, eventTransform.position, eventTransform.rotation, 0);
                break;

            // Makes a cool fade before displaying loading screen, and loading next level
            case CustomEventType.LoadNextLevel:
                if (!PhotonNetwork.isMasterClient) yield break;

                if (!PhotonNetwork.offlineMode)
                {
                    TDS_SceneManager.Instance.PrepareOnlineSceneLoading(TDS_GameManager.CurrentSceneIndex + 1, (int)UIState.InGame);
                }
                else TDS_SceneManager.Instance.PrepareSceneLoading(TDS_GameManager.CurrentSceneIndex + 1, (int)UIState.InGame);

                if (TDS_GameManager.CurrentSceneIndex == 0)
                    break;

                yield return new WaitForSeconds(1f);
                
                if (!PhotonNetwork.offlineMode) TDS_LevelManager.Instance?.LocalPlayer?.FreezePlayer();
                else
                {
                    foreach (TDS_Player _player in TDS_LevelManager.Instance?.AllPlayers)
                    {
                        _player.FreezePlayer();
                    }
                }
                break;

            // Moves the local player around a point
            case CustomEventType.MovePlayerAroundPoint:
                if (!PhotonNetwork.offlineMode) TDS_LevelManager.Instance.LocalPlayer.GoAround(eventTransform.position, false);
                else
                {
                    foreach (TDS_Player _player in TDS_LevelManager.Instance.AllPlayers)
                    {
                        _player.GoAround(eventTransform.position, false);
                    }
                }
                break;

            // Triggers a particular quote of the Narrator
            case CustomEventType.Narrator:
                TDS_LevelManager.Instance?.PlayNarratorQuote(quote);
                break;

            // Plays a cutscene
            case CustomEventType.PlayCutscene:
                TDS_LevelManager.Instance?.PlayCutscene(cutscene);
                break;

            // Plays a new music !
            case CustomEventType.PlayMusic:
                TDS_SoundManager.Instance?.PlayMusic((Music)eventInt, eventFloat);
                break;

            // Remove UI curtains
            case CustomEventType.RemoveCurtains:
                TDS_UIManager.Instance?.SwitchCurtains(false);
                break;

            // Set all listed ui texts
            case CustomEventType.SetUIQuotes:
                foreach (TDS_UIQuote _quote in uiQuotes)
                {
                    _quote.Text.text = _quote.Quote.Quote;
                }
                break;

            // Switch UI arrow
            case CustomEventType.SwitchArrow:
                TDS_UIManager.Instance?.SwitchArrow();
                yield return new WaitForSeconds(2.5f);
                TDS_UIManager.Instance?.SwitchArrow();
                break;

            // Unfreeze local player
            case CustomEventType.UnfreezePlayerFromCutscene:
                TDS_GameManager.IsInCutscene = false;
                TDS_UIManager.Instance?.DesactivateCutsceneBlackBars();
                if (!PhotonNetwork.offlineMode) TDS_LevelManager.Instance?.LocalPlayer.UnfreezePlayer();
                else
                {
                    foreach (TDS_Player _player in TDS_LevelManager.Instance?.AllPlayers)
                    {
                        _player.UnfreezePlayer();
                    }
                }
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEventLocal:
                unityEvent.Invoke();
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEventOnline:
                unityEvent.Invoke();
                break;

            // Wait until end of cutscene
            case CustomEventType.WaitEndOfCutscene:
                TDS_LevelManager.OnCutsceneEnds += StopWaiting;

                while (IsWaiting) yield return null;

                TDS_LevelManager.OnCutsceneEnds -= StopWaiting;
                IsWaiting = true;
                break;

            // Wait for an action of the local player
            case CustomEventType.WaitForAction:
                switch (actionType)
                {
                    case WaitForAction.UseRabbit:
                        TDS_WhiteRabbit.OnUseRabbit += StopWaiting;
                        TDS_WhiteRabbit.OnLoseRabbit += StopWaiting;
                        break;

                    default:
                        break;
                }

                while (IsWaiting)
                {
                    yield return null;
                }

                switch (actionType)
                {
                    case WaitForAction.UseRabbit:
                        TDS_WhiteRabbit.OnUseRabbit -= StopWaiting;
                        TDS_WhiteRabbit.OnLoseRabbit -= StopWaiting;
                        break;

                    default:
                        break;
                }

                IsWaiting = true;
                break;

            // Wait that everyone is in the zone
            case CustomEventType.WaitForEveryone:
                if (TDS_Camera.Instance?.CurrentBounds.XMin < eventTransform.position.x)
                {
                    TDS_Camera.Instance.OnXMinBoundChanged += CheckBound;
                    bool _doSwitch = TDS_LevelManager.Instance.AllPlayers.Length > 1;

                    // Switch waiting players panel if playing with your folks
                    if (_doSwitch)
                    {
                       TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "SwitchWaitingPanel"), new object[] { });

                        yield return new WaitForSeconds(.25f);
                    }

                    // Wait patiently
                    while (IsWaiting) yield return null;

                    if (_doSwitch)
                    {
                       TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance?.photonView, typeof(TDS_UIManager), "SwitchWaitingPanel"), new object[] { });
                    }
                }
                break;

            // Wait that an object with a particular tag dies
            case CustomEventType.WaitForObjectDeath:
                TDS_Damageable.OnDieWithObject += CheckObjectDeath;

                while (IsWaiting) yield return null;

                TDS_Damageable.OnDieWithObject -= CheckObjectDeath;
                break;

            // Wait that a certain amount of spawn area are desactivated
            case CustomEventType.WaitForSpawnAreaDesactivation:
                TDS_SpawnerArea.OnOneAreaDesactivated += DeductSpawnArea;

                while (IsWaiting) yield return null;

                TDS_SpawnerArea.OnOneAreaDesactivated -= DeductSpawnArea;
                break;

            // Nobody here but us chicken
            default:
                break;
        }
        
        yield break;
    }
	#endregion
}

[Serializable]
public class TDS_UIQuote
{
    /// <summary>Backing field for <see cref="Text"/>.</summary>
    [SerializeField] private TMPro.TextMeshProUGUI text = null;

    /// <summary>
    /// UI Text to set value.
    /// </summary>
    public TMPro.TextMeshProUGUI Text { get { return text; } }

    /// <summary>Backing field for <see cref="Quote"/>.</summary>
    [SerializeField] private TDS_NarratorQuote quote = null;

    /// <summary>
    /// Quote to set text from.
    /// </summary>
    public TDS_NarratorQuote Quote { get { return quote; } }
}