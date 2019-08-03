using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.UI;
using Photon;
using TMPro;

#pragma warning disable 0649

public class TDS_UIManager : PunBehaviour 
{
    /* TDS_UIManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Manager of the UI]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     *	
     *	Date :			[17/06/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Update the UI]
     *	    - Clear the UI when a player disconnect
     *	    - Set a toggle to wait every player in the room before launching the game
	 *
	 *	-----------------------------------
     *	
     *	Date :			[28/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Set the lifebars managed in local]
     *	    - Set a new SerializeField for the prefab of the lifebars
     *	    - Modify methods to manage the lifebat in local
     *	    - Adding a Combo counter
	 *
	 *	-----------------------------------
     *	
	 * 	Date :			[10/04/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Adding Buttons to select players and dialogbox]
     *	    - Buttons to select players 
     *	    - DialogBox to show Narrator's phrases
	 *
	 *	-----------------------------------
     *	
     * 	Date :			[21/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Adding method to stop the filling coroutine]
     *	    - Implementing method to stop the filling coroutine linked to an image
     *	    - Implementing Method SetBossLifeBar, SetEnemyLifeBar and SetPlayerLifeBar to set the life bar of an enemy
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[21/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the UIManager class]
	 *
	 *	-----------------------------------
	*/

    #region Events
    public event Action OnNarratorDialogEnded; 
    #endregion

    #region Fields / Properties
    /// <summary> Singleton of the class TDS_UIManager </summary>
    public static TDS_UIManager Instance;

    #region UIState
    [Header("UI State")]
    //State of the UI
    [SerializeField] private UIState uiState;
    public UIState UIState { get { return uiState; } }
    #endregion

    #region General Settings
    private GameObject uiGameObject;
    [Header("Loading Settings")]
    [SerializeField] private bool isloadingNextScene = false;
    #endregion

    #region Canvas 
    [Header("Canvas")]
    // Canvas based on the screen
    [SerializeField] private Canvas canvasScreen;
    /// <summary> Backing field of <see cref="canvasScreen"/>  </summary>
    public Canvas CanvasScreen { get { return canvasScreen; } }
    // Canvas based on the world
    [SerializeField] private Canvas canvasWorld;
    /// <summary> Backing field of <see cref="canvasWorld"/>  </summary>
    public Canvas CanvasWorld { get { return canvasWorld; } }
    #endregion

    #region MenuParents
    [Header("Menu parent")]
    //Parent of the main menu UI
    [SerializeField] private GameObject mainMenuParent;
    //Parent of the room selection menu
    [SerializeField] private GameObject roomSelectionMenuParent;
    //Parent of the character selection menu
    [SerializeField] private GameObject characterSelectionMenuParent;
    //Parent of the InGame UI
    [SerializeField] private GameObject inGameMenuParent;
    //Parent of the pause menu UI
    [SerializeField] private GameObject pauseMenuParent;
    // Parent of the DialogBox
    [SerializeField] private GameObject dialogBoxParent;
    //Parent of the Error Box
    [SerializeField] private GameObject errorBoxParent; 
    // Parent of the NarratorBox
    [SerializeField] private GameObject narratorBoxParent;
    // Parent of the loading screen
    [SerializeField] private GameObject loadingScreenParent;
    // Parent of the Game Over Screen
    [SerializeField] private GameObject gameOverScreenParent; 
    #endregion

    #region Lifebars
    [Header("Lifebars")]
    [SerializeField] private TDS_LifeBar beardLadyLifeBar;
    [SerializeField] private TDS_LifeBar fatLadyLifeBar;
    [SerializeField] private TDS_LifeBar jugglerLifeBar;
    [SerializeField] private TDS_LifeBar fireEaterLifeBar;

    #region Hidden Players Images
    [Header("Hidden Player's Images")]
    [SerializeField] private Image hiddenBeardLadyImage;
    [SerializeField] private Image hiddenFatLadyImage;
    [SerializeField] private Image hiddenJugglerImage;
    [SerializeField] private Image hiddenFireEaterImage;
    #endregion
    #endregion

    #region Feedback
    // Animator of the Juggler's aim target, giving player feedback
    [Header("Feedback")]
    [SerializeField] private Animator jugglerAimTargetAnimator = null;

    // GameObject for the aim target of the Juggler
    [SerializeField] private RectTransform jugglerAimTarget = null;

    /// <summary>
    /// RectTransform of the Juggler aim target.
    /// </summary>
    public RectTransform JugglerAimTargetTransform
    {
        get
        {
            if (jugglerAimTarget) return jugglerAimTarget;

            Debug.Log("Missing Juggler Aim Target reference !");
            return null;
        }
    }

    // Image for the aim target of the Juggler
    [SerializeField] private Image jugglerAimArrow = null;

    /// <summary>
    /// RectTransform of the Juggler aim arrow.
    /// </summary>
    public RectTransform JugglerAimArrowTransform
    {
        get
        {
            if (jugglerAimArrow) return jugglerAimArrow.rectTransform;

            Debug.Log("Missing Juggler Aim Arrow reference !");
            return null;
        }
    }
    #endregion

    #region Room Selection Menu
    [Header("RoomSelectionMenu")]
    [SerializeField] private TDS_RoomSelectionElement[] roomSelectionElements = new TDS_RoomSelectionElement[] { }; 
    #endregion

    #region CharacterSelectionMenus
    [Header("Character Selection Menu")]
    [SerializeField] private TDS_CharacterMenuSelection characterSelectionMenu;
    public TDS_CharacterMenuSelection CharacterSelectionMenu { get { return characterSelectionMenu; }}
    #region TextField
    [SerializeField] private TMP_Text playerNameField;
    public TMP_Text PlayerNameField
    {
        get { return playerNameField; }
    }
    [SerializeField] private TMP_Text playerCountText;


    #endregion
    #endregion

    #region Buttons
    [Header("Buttons")]
    [SerializeField] private UnityEngine.UI.Button launchGameButton;
    public Button LaunchGameButton
    {
        get { return launchGameButton;  }
    }
    [SerializeField] private UnityEngine.UI.Button buttonQuitPause;
    [SerializeField] private UnityEngine.UI.Button buttonQuitGame;
    [SerializeField] private UnityEngine.UI.Button buttonRestartGame; 
    #endregion

    #region Animator
    [Header("Animator")]
    [SerializeField] private Animator curtainsAnimator;
    [SerializeField] private Animator arrowAnimator;
    [SerializeField] private Animator waitingPanelAnimator;
    #endregion

    #region Text
    [Header("Dialog/Narrator/Error Box")]
    //Text of the dialog Box
    [SerializeField] private TMP_Text dialogBoxText;
    //Text of the narrator Box
    [SerializeField] private TMP_Text narratorBoxText;
    //Text of the Error Box
    [SerializeField] private TMP_Text errorBoxText;
    #endregion

    #region Resources
    [Header("Enemies")]
    [SerializeField] GameObject lifeBarPrefab = null;
    [SerializeField] private TDS_BossLifeBar bossHealthBar;
    #endregion

    #region Coroutines
    /// <summary>
    /// Dictionary to stock every filling coroutine started
    /// </summary>
    private Dictionary<TDS_LifeBar, Coroutine> filledImages = new Dictionary<TDS_LifeBar, Coroutine>();

    private Coroutine narratorCoroutine;

    private Dictionary<PlayerType, Coroutine> followHiddenPlayerCouroutines = new Dictionary<PlayerType, Coroutine>();

    private Coroutine checkInputCoroutine = null;
    #endregion

    #region ComboManager
    //[Header("Combo Manager")]
    //[SerializeField] private TDS_ComboManager comboManager;
    public TDS_ComboManager ComboManager { get; set; }
    #endregion

    #region WorkInProgress
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region IEnumerator
    /// <summary>
    /// Check the inputs and call the methods used during the various states of the menu 
    /// </summary>
    /// <param name="_state">State of the menu</param>
    /// <returns></returns>
    private IEnumerator CheckInputMenu(UIState _state)
    {
        if (_state == UIState.InMainMenu || _state == UIState.InGame || _state == UIState.InGameOver) yield break;
        Action CancelAction = null;
        Action SubmitAction = null;
        Action<int> HorizontalAxisAction = null;

        switch (_state)
        {
            case UIState.InMainMenu:
                break;
            case UIState.InRoomSelection:
                CancelAction = () => ActivateMenu(UIState.InMainMenu); ;
                break;
            case UIState.InOnlineCharacterSelection:
                CancelAction = CancelInCharacterSelection;
                SubmitAction = SubmitInCharacterSelection;
                HorizontalAxisAction = characterSelectionMenu.LocalElement.ChangeImage;
                break;
            case UIState.InGame:
                break;
            case UIState.InPause:
                CancelAction = () => ActivateMenu(UIState.InGame);
                break;
            case UIState.InGameOver:
                break;
            default:
                break;
        }
        int _value = 0;
        while (UIState == _state)
        {
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.CANCEL_BUTTON))
            {
                yield return new WaitForEndOfFrame();
                CancelAction?.Invoke();
            }
            else if (TDS_InputManager.GetButtonDown(TDS_InputManager.SUBMIT_BUTTON))
            {
                yield return new WaitForEndOfFrame();
                SubmitAction?.Invoke();
            }
            else if (TDS_InputManager.GetAxisDown(TDS_InputManager.HORIZONTAL_AXIS, out _value))
            {
                yield return new WaitForEndOfFrame();
                HorizontalAxisAction?.Invoke(_value);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Display an image that follow the hidden player
    /// Convert the player position into a screen position for the image
    /// </summary>
    /// <param name="_followedPlayer">Hidden and followed player </param>
    /// <param name="_followingImage">The image that follows the player</param>
    /// <returns></returns>
    private IEnumerator FollowHiddenPlayer(TDS_Player _followedPlayer, Image _followingImage)
    {
        if (!_followedPlayer || !Application.isPlaying || Camera.main == null) yield break;
        _followingImage.gameObject.SetActive(true);
        Vector2 _screenPos = Camera.main.WorldToScreenPoint(_followedPlayer.transform.position);
        _screenPos.x = Mathf.Clamp(_screenPos.x, _followingImage.rectTransform.rect.width / 2, Screen.width - (_followingImage.rectTransform.rect.width / 2));
        _screenPos.y = Mathf.Clamp(_screenPos.y, _followingImage.rectTransform.rect.height / 2, Screen.width - (_followingImage.rectTransform.rect.height / 2));
        _followingImage.transform.position = _screenPos;
        _followingImage.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, _screenPos.x < Screen.width/2 ? 90 : -90 ); 
        while (_followedPlayer && _followingImage && Application.isPlaying)
        {
            //Debug.LogError(Camera.main.WorldToScreenPoint(_followedPlayer.transform.position));
            _screenPos = Camera.main.WorldToScreenPoint(_followedPlayer.transform.position);
            _screenPos.x = Mathf.Clamp(_screenPos.x, _followingImage.rectTransform.rect.width / 2, Screen.width - (_followingImage.rectTransform.rect.width / 2));
            _screenPos.y = Mathf.Clamp(_screenPos.y, _followingImage.rectTransform.rect.height / 2, Screen.height - (_followingImage.rectTransform.rect.height / 2));
            _followingImage.transform.position = Vector3.MoveTowards(_followingImage.transform.position, _screenPos, Time.deltaTime * 3600);

            yield return null;
        }
        if (!_followedPlayer)
        {
            _followingImage.gameObject.SetActive(false);
        }
        yield return null;
    }

    /// <summary>
    /// Stop all coroutines and switch the UI State to InGameOver
    /// </summary>
    public IEnumerator ResetInGameUI()
    {
        yield return new WaitForSeconds(1.5f);
        beardLadyLifeBar.ResetLifeBar(); 
        fatLadyLifeBar.ResetLifeBar();
        jugglerLifeBar.ResetLifeBar();
        fireEaterLifeBar.ResetLifeBar();
        bossHealthBar.ResetLifeBar(); 

        narratorCoroutine = null;
        followHiddenPlayerCouroutines.Clear();
        filledImages.Clear();
        curtainsAnimator.SetTrigger("Reset");
        ComboManager.ResetComboManager();
        for (int i = 0; i < canvasWorld.transform.childCount; i++)
        {
            Destroy(canvasWorld.transform.GetChild(i).gameObject);
        }
        ActivateMenu(UIState.InGameOver);
    }

    /// <summary>
    /// Fill the image until its fillAmount until it reaches the fillingValue
    /// At the end of the filling, remove the entry of the dictionary at the key _filledImage
    /// </summary>
    /// <param name="_lifebar">Image to fill</param>
    /// <param name="_fillingValue">Fill amount to reach</param>
    /// <returns></returns>
    private IEnumerator UpdateFilledImage(TDS_LifeBar _lifebar, float _fillingValue)
    {
        if (!_lifebar) yield break;  
        if(_lifebar.ForegroundFilledImage) _lifebar.ForegroundFilledImage.fillAmount = _fillingValue; 
        while (_lifebar.FilledImage.fillAmount != _fillingValue &&  _lifebar.FilledImage != null)
        {
            _lifebar.FilledImage.fillAmount = Mathf.MoveTowards(_lifebar.FilledImage.fillAmount, _fillingValue, Time.deltaTime/5 );
            yield return new WaitForEndOfFrame();
        }
        filledImages.Remove(_lifebar);
    }

    /// <summary>
    /// Update the player count when the player is in the room selection Menu
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdatePlayerCount()
    {
        while (!PhotonNetwork.connected)
        {
            Debug.Log("Not Connected");
            yield return new WaitForSeconds(1);
        }
        RoomInfo[] _infos = new RoomInfo[] { };
        while (uiState == UIState.InRoomSelection)
        {
            if (!PhotonNetwork.connected) yield break;
            _infos = PhotonNetwork.GetRoomList();
            roomSelectionElements.ToList().ForEach(e => e.PlayerCount = 0);
            for (int i = 0; i < _infos.Length; i++)
            {
                for (int j = 0; j < roomSelectionElements.Length; j++)
                {
                    if (roomSelectionElements[j].RoomName == _infos[i].Name)
                    {
                        roomSelectionElements[j].PlayerCount = _infos[i].PlayerCount;
                    }
                }
            }
            yield return new WaitForSeconds(2);
        }
    }

    /// <summary>
    /// Display all quotes in the narrator box
    /// </summary>
    /// <param name="_quotes">Quotes to display</param>
    /// <returns></returns>
    private IEnumerator PlayNarratorQuotes(string[] _quotes)
    {
        if (narratorBoxParent == null || narratorBoxText == null) yield break;
        narratorBoxParent.SetActive(true);
        foreach (string _quote in _quotes)
        {
            narratorBoxText.text = _quote; 
            yield return new WaitForSeconds(_quote.Length/20);
        }
        narratorBoxParent.SetActive(false);
        OnNarratorDialogEnded?.Invoke();
        narratorCoroutine = null; 
    }

    private IEnumerator PreapreLeavingRoom()
    {
        characterSelectionMenu.LocalElement.ClearToggle();
        while (characterSelectionMenu.LocalElement.ReadyToggle.animator.IsInTransition(0))
        {
            yield return null; 
        }
        yield return new WaitForSeconds(.5f);

        characterSelectionMenu.ClearMenu();
        TDS_GameManager.LocalPlayer = PlayerType.Unknown;
        yield return null;

        TDS_NetworkManager.Instance.LeaveRoom();
        ActivateMenu((int)UIState.InRoomSelection);
    }
    #endregion

    #region void
    /// <summary>
    /// Fill the text of the dialog box as _text
    /// Set the parent of the dialogbox Active
    /// </summary>
    /// <param name="_text">Text to fill in the text fieldw</param>
    public void ActivateDialogBox(string _text)
    {
        if (dialogBoxParent == null || dialogBoxText == null) return;
        dialogBoxText.text = _text;
        dialogBoxParent.SetActive(true);  
    }

    /// <summary>
    /// Fill the text of the errorBox box and display it
    /// </summary>
    /// <param name="_text"></param>
    public void ActivateErrorBox(string _text)
    {
        if (errorBoxParent == null || errorBoxText == null) return;
        errorBoxText.text = _text;
        errorBoxParent.SetActive(true);
    }

    /// <summary>
    /// Activates the aim target on UI of the Juggler.
    /// </summary>
    public void ActivateJugglerAimTarget()
    {
        SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Neutral);
        jugglerAimTarget.gameObject.SetActive(true);
    }

    /// <summary>
    /// Call the method Activate Menu from Unity Event
    /// </summary>
    /// <param name="_uiState">UI State</param>
    public void ActivateMenu(int _uiState)
    {
        ActivateMenu((UIState)_uiState); 
    }

    /// <summary>
    /// Activate or desactivate Menu depending of the uistate
    /// </summary>
    /// <param name="_state">State</param>
    public void ActivateMenu(UIState _state)
    {
        uiState = _state;
        switch (uiState)
        {
            case UIState.InMainMenu:
                mainMenuParent.SetActive(true);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(false);
                Selectable.allSelectables.FirstOrDefault()?.Select();
                break;
            case UIState.InRoomSelection:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(true);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(false);
                Selectable.allSelectables.FirstOrDefault()?.Select();

                StartCoroutine(UpdatePlayerCount());
                if (checkInputCoroutine != null)
                    StopCoroutine(checkInputCoroutine);
                checkInputCoroutine = StartCoroutine(CheckInputMenu(UIState.InRoomSelection)); 
                break;
            case UIState.InOnlineCharacterSelection:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(true);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(false);

                if (checkInputCoroutine != null)
                    StopCoroutine(checkInputCoroutine);
                checkInputCoroutine = StartCoroutine(CheckInputMenu(UIState.InOnlineCharacterSelection));
                break;
            case UIState.InGame:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(true);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(false);
                break;
            case UIState.InPause:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(true);
                gameOverScreenParent.SetActive(false);
                Selectable.allSelectables.FirstOrDefault()?.Select();
                if (checkInputCoroutine != null)
                    StopCoroutine(checkInputCoroutine);
                checkInputCoroutine = StartCoroutine(CheckInputMenu(UIState.InPause));
                break;
            case UIState.InGameOver:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(true);
                Selectable.allSelectables.FirstOrDefault()?.Select();
                StopAllCoroutines();
                if (!PhotonNetwork.isMasterClient)
                {
                    TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdateReadySettings"), new object[] { PhotonNetwork.player.ID, false });
                }
                break; 
            default:
                break;
        }
    }

    /// <summary>
    /// Fill the text of the dialog box as _text
    /// Set the parent of the dialogbox Active
    /// </summary>
    /// <param name="_text">Text to fill in the text fieldw</param>
    public void ActivateNarratorBox(string[] _text)
    {
        if (narratorCoroutine != null)
        {
            OnNarratorDialogEnded?.Invoke(); 
            StopCoroutine(narratorCoroutine);
        }
        narratorCoroutine = StartCoroutine(PlayNarratorQuotes(_text)); 
    }

    /// <summary>
    /// Action called when the player use the cancel button in the character selection menu 
    /// if the player is ready, unlock its character, else leave the room
    /// </summary>
    private void CancelInCharacterSelection()
    {
        if (uiState != UIState.InOnlineCharacterSelection) return;
        if(TDS_GameManager.LocalIsReady)
        {
            SelectCharacter();
            characterSelectionMenu.LocalElement.TriggerToggle();
            return; 
        }
        StartLeavingRoom(); 
    }

    /// <summary>
    /// Clear the legacy UI from the online player when this one is disconnected
    /// </summary>
    /// <param name="_playerTypeID">ID of the player type of the disconnected player</param>
    public void ClearUIRelatives(PlayerType _playerType)
    {
        switch (_playerType)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                if (beardLadyLifeBar) beardLadyLifeBar.gameObject.SetActive(false);
                if (hiddenBeardLadyImage) hiddenBeardLadyImage.gameObject.SetActive(false);
                break;
            case PlayerType.FatLady:
                if (fatLadyLifeBar) fatLadyLifeBar.gameObject.SetActive(false);
                if (hiddenFatLadyImage) hiddenFatLadyImage.gameObject.SetActive(false);
                break;
            case PlayerType.FireEater:
                if (fireEaterLifeBar) fireEaterLifeBar.gameObject.SetActive(false);
                if (hiddenFireEaterImage) hiddenFireEaterImage.gameObject.SetActive(false);
                break;
            case PlayerType.Juggler:
                if (jugglerLifeBar) jugglerLifeBar.gameObject.SetActive(false);
                if (hiddenJugglerImage) hiddenJugglerImage.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set the dialogbox parent as inactive
    /// </summary>
    public void DesactivateDialogBox()
    {
        if (dialogBoxParent == null) return;
        dialogBoxParent.SetActive(false); 
    }

    /// <summary>
    /// Set the dialogbox parent as inactive
    /// </summary>
    public void DesactivateErrorBox()
    {
        if (errorBoxParent == null) return;
        errorBoxParent.SetActive(false);
    }

    /// <summary>
    /// Desactivates the aim target on UI of the Juggler.
    /// </summary>
    public void DesctivateJugglerAimTarget()
    {
        jugglerAimTarget.gameObject.SetActive(false);
        SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Disabled);
    }

    /// <summary>
    /// Set the dialogbox parent as inactive
    /// </summary>
    public void DesactivateNarratorBox()
    {
        if (narratorBoxParent == null) return;
        narratorBoxParent.SetActive(false);
    }

    /// <summary>
    /// Activate or desactivate an image on the left or the right of the screen to show where is a player when he's not rendered
    /// </summary>
    /// <param name="_player">Visible or invisible player</param>
    /// <param name="_isInvisible">is the player invisible?</param>
    public void DisplayHiddenPlayerPosition(TDS_Player _player, bool _isInvisible)
    {
        if (!_player) return;
        Image _image = null;
        switch (_player.PlayerType)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                _image = hiddenBeardLadyImage;
                break;
            case PlayerType.FatLady:
                _image = hiddenFatLadyImage;
                break;
            case PlayerType.FireEater:
                _image = hiddenFireEaterImage;
                break;
            case PlayerType.Juggler:
                _image = hiddenJugglerImage;
                break;
            default:
                break;
        }
        if (!_image) return;
        if (_isInvisible)
        {
            if(!followHiddenPlayerCouroutines.ContainsKey(_player.PlayerType))
            {
                Coroutine _c = StartCoroutine(FollowHiddenPlayer(_player, _image));
                followHiddenPlayerCouroutines.Add(_player.PlayerType, _c);
            }
        }
        else
        {
            if(followHiddenPlayerCouroutines.ContainsKey(_player.PlayerType))
            {
                Coroutine _c = followHiddenPlayerCouroutines[_player.PlayerType]; 
                if(_c != null) StopCoroutine(_c);
                followHiddenPlayerCouroutines.Remove(_player.PlayerType); 
            }
            _image.gameObject.SetActive(false); 
        }
    }

    /// <summary>
    /// Display the loading screen during the load of a scene
    /// </summary>
    /// <param name="_isLoading">Does the scene is loading or not</param>
    public void DisplayLoadingScreen(bool _isLoading)
    {
        if (loadingScreenParent) loadingScreenParent.SetActive(_isLoading);
    }

    /// <summary>
    /// Check if the image is already being filled
    /// If so, stop the coroutine and remove it from the dictionary 
    /// Then start the coroutine and stock it with the filledImage as a key in the dictionary
    /// </summary>
    /// <param name="_filledImage">Image to fill</param>
    /// <param name="_fillingValue">Filling value to reach</param>
    public void FillImage(TDS_LifeBar _lifebar, float _fillingValue)
    {
        StopFilling(_lifebar); 
        filledImages.Add(_lifebar, StartCoroutine(UpdateFilledImage(_lifebar, _fillingValue))); 
    }

    /// <summary>
    /// Load the next level 
    /// If Master client, send the information to the other players
    /// </summary>
    public void LoadLevel()
    {
        characterSelectionMenu.LocalElement.ClearToggle(); 
        if (isloadingNextScene)
        {
            //if (PhotonNetwork.isMasterClient)
              //  TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LoadLevel"), new object[] { });
            TDS_SceneManager.Instance?.PrepareOnlineSceneLoading(1, (int)UIState.InGame);
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
                TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LoadLevel"), new object[] { });
            TDS_LevelManager.Instance.Spawn();
            ActivateMenu(UIState.InGame);
        }
    }

    /// <summary>
    /// Called when the toggle is pressed
    /// Update the ready settings
    /// If the player has an Unknown PlayerType, the game cannot start
    /// </summary>
    public void OnPlayerReady(bool _isReady)
    {
        TDS_GameManager.LocalIsReady = _isReady;

        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.OthersBuffered, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetPlayerReady"), new object[] { PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady });
        SetPlayerReady(PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady); 
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdateReadySettings"), new object[] { PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady });
            return; 
        }
        UpdateReadySettings(PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady); 
    }

    /// <summary>
    /// Call when the Restart Button is pressed 
    /// If not master client, send a message to say the player is ready 
    /// If master, send a message to everybody to reset the level
    /// </summary>
    public void OnRestartButtonPressed()
    {
        beardLadyLifeBar.ResetLifeBar();
        fatLadyLifeBar.ResetLifeBar();
        fireEaterLifeBar.ResetLifeBar();
        jugglerLifeBar.ResetLifeBar();
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "ResetLevel"), new object[] { });
            ResetLevel();
        }
        else
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdateReadySettings"), new object[] { PhotonNetwork.player.ID, true });
        }
    }

    public void QuitGame() => Application.Quit();

    /// <summary>
    /// Reload the level
    /// </summary>
    public void ResetLevel()
    {
        TDS_SceneManager.Instance.PrepareOnlineSceneLoading(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, (int)UIState.InGame);
        ActivateMenu(UIState.InGame); 
    }

    /// <summary>
    /// Hide the online life bar of a selected player 
    /// </summary>
    /// <param name="_removedPlayer">Player Type ID to remove</param>
    public void RemovePlayerLifeBar(int _removedPlayer)
    {
        PlayerType _type = (PlayerType)_removedPlayer;
        switch (_type)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                beardLadyLifeBar.gameObject.SetActive(false); 
                break;
            case PlayerType.FatLady:
                fatLadyLifeBar.gameObject.SetActive(false);
                break;
            case PlayerType.FireEater:
                fireEaterLifeBar.gameObject.SetActive(false);
                break;
            case PlayerType.Juggler:
                jugglerLifeBar.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Remove the player from the player ready list
    /// </summary>
    /// <param name="_playerID">Id of the removed player</param>
    public void RemovePlayer(int _playerID)
    {
        PhotonPlayer _player = PhotonPlayer.Find(_playerID);
        if (!TDS_GameManager.PlayerListReady.ContainsKey(_player)) return;
        TDS_GameManager.PlayerListReady.Remove(_player); 
    }

    /// <summary>
    /// Select a new character (used in UnityEvent)
    /// </summary>
    /// <param name="_newPlayerType">Index of the enum PlayerType</param>
    public void SelectCharacter()
    {
        int _newPlayerType = (int)characterSelectionMenu.LocalElement.CurrentSelection.CharacterType; 
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.OthersBuffered, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdatePlayerSelectionInfo"), new object[] { (int)TDS_GameManager.LocalPlayer, _newPlayerType, PhotonNetwork.player.ID });

        TDS_GameManager.LocalPlayer = (PlayerType)_newPlayerType == TDS_GameManager.LocalPlayer ? PlayerType.Unknown : (PlayerType)_newPlayerType;
        OnPlayerReady(TDS_GameManager.LocalPlayer != PlayerType.Unknown); 
    }

    /// <summary>
    /// Set the BossLifeBar's owner and set the game object active
    /// Add the updating method when the boss take damage
    /// When the boss dies, set the life to inactive
    /// </summary>
    /// <param name="_boss"></param>
    public void SetBossLifeBar(TDS_Boss _boss)
    {
        if (!bossHealthBar) return;
        bossHealthBar.SetOwner(_boss);
        bossHealthBar.gameObject.SetActive(true);

        _boss.HealthBar = bossHealthBar;
        if(_boss.Portrait)
        {
            bossHealthBar.SetBossPortrait(_boss.Portrait); 
        }

        _boss.OnTakeDamage += bossHealthBar.UpdateLifeBar;
        _boss.OnHeal += bossHealthBar.UpdateLifeBar;
        _boss.OnDie += bossHealthBar.DestroyLifeBar;
    }

    /// <summary>
    /// Set the boss lifebar's subowners
    /// and set the game object to active 
    /// and add the event on take damages and heal
    /// </summary>
    /// <param name="_subonwers"></param>
    public void SetBossLifeBar(TDS_Enemy[] _subonwers, GameObject _portrait = null)
    {
        if (!bossHealthBar) return;
        bossHealthBar.SetSubOwners(_subonwers);
        bossHealthBar.gameObject.SetActive(true);
        if(_portrait != null) bossHealthBar.SetBossPortrait(_portrait); 

        for (int i = 0; i < _subonwers.Length; i++)
        {
            TDS_Enemy _e = _subonwers[i];

            _e.OnTakeDamage += bossHealthBar.UpdateLifeBar;
            _e.OnHeal += bossHealthBar.UpdateLifeBar;
            _e.OnDie += bossHealthBar.DestroyLifeBar;
        }

    }

    /// <summary>
    /// Instantiate enemy Life bar
    /// Link it to the enemy
    /// </summary>
    /// <param name="_enemy"></param>
    public void SetEnemyLifebar(TDS_Enemy _enemy)
    {
        if (lifeBarPrefab == null || !canvasWorld) return;
        Vector3 _offset = (Vector3.up * .2f) - Vector3.forward; 
        TDS_EnemyLifeBar _healthBar = Instantiate(lifeBarPrefab, _enemy.transform.position + _offset, Quaternion.identity, canvasWorld.transform).GetComponent<TDS_EnemyLifeBar>();

        _healthBar.SetOwner(_enemy, _offset);
        _healthBar.Background.gameObject.SetActive(false); 
        _enemy.HealthBar = _healthBar;

        _enemy.OnTakeDamage += _healthBar.UpdateLifeBar;
        _enemy.OnHeal += _healthBar.UpdateLifeBar; 
    }

    /// <summary>
    /// Set animation state for the Juggler's aim target UI system.
    /// </summary>
    /// <param name="_state">New state of the aim target.</param>
    public void SetJugglerAimTargetAnim(JugglerAimTargetAnimState _state)
    {
        jugglerAimTargetAnimator.SetInteger("State", (int)_state);
    }

    /// <summary>
    /// Set the game in pause menu
    /// If the player is alone, freeze the time
    /// </summary>
    /// <param name="_isPaused"></param>
    public void SetPause(bool _isPaused)
    {
        if (TDS_LevelManager.Instance && TDS_LevelManager.Instance.OtherPlayers.Count == 0)
        {
            Time.timeScale = _isPaused ? 0 : 1;
        }
        ActivateMenu(_isPaused ? UIState.InPause : UIState.InGame);
    }

    /// <summary>
    /// Instantiate player LifeBar and set its owner as the local player
    /// if local 
    /// </summary>
    /// <param name="_player"></param>
    public void SetPlayerLifeBar(TDS_Player _player)
    {
        TDS_LifeBar _playerLifeBar = null;
        switch (_player.PlayerType)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                _playerLifeBar = beardLadyLifeBar;
                break;
            case PlayerType.FatLady:
                _playerLifeBar = fatLadyLifeBar;
                break;
            case PlayerType.FireEater:
                _playerLifeBar = fireEaterLifeBar;
                break;
            case PlayerType.Juggler:
                _playerLifeBar = jugglerLifeBar;
                break;
            default:
                break;
        }
        if (!_playerLifeBar) return; 
        _playerLifeBar.gameObject.SetActive(true);
        _playerLifeBar.SetOwner(_player);
        _player.HealthBar = _playerLifeBar;
        _player.OnTakeDamage += _playerLifeBar.UpdateLifeBar;
        _player.OnHeal += _playerLifeBar.UpdateLifeBar; 
        if (_player == TDS_LevelManager.Instance.LocalPlayer && _player.photonView.isMine)
            _playerLifeBar.transform.SetSiblingIndex(0);
    }

    public void SetPlayerReady(int _playerID, bool _isReady)
    {
        characterSelectionMenu.LockPlayer(_playerID, _isReady);
    }

    /// <summary>
    /// Set the new name of the player (Used in Unity Event)
    /// </summary>
    public void SetNewName()
    {
        if (playerNameField)
            TDS_NetworkManager.Instance.PlayerNamePrefKey = playerNameField.text;
    }

    /// <summary>
    /// Set the new name of the player as a string
    /// </summary>
    /// <param name="_newName">the new name</param>
    public void SetNewName(string _newName)
    {
        if(!TDS_NetworkManager.Instance)
        {
            Debug.LogError("NetworkManager Not found");
            return; 
        }
        TDS_NetworkManager.Instance.PlayerNamePrefKey = _newName;
        if (playerNameField) playerNameField.text = _newName; 
    }

    public void SetRoomInterractable(bool _areInterractable)
    {
        roomSelectionElements.ToList().ForEach(e => e.RoomSelectionButton.interactable = _areInterractable);
    }

    public void StartLeavingRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "StartLeavingRoom"), new object[] { });
        }
        StartCoroutine(PreapreLeavingRoom());
    }

    /// <summary>
    /// Stop the coroutine that fill the image
    /// </summary>
    /// <param name="_lifeBar"></param>
    public void StopFilling(TDS_LifeBar _lifeBar)
    {
        if (filledImages.ContainsKey(_lifeBar))
        {
            if(filledImages[_lifeBar] != null) StopCoroutine(filledImages[_lifeBar]);
            filledImages.Remove(_lifeBar);
        }
    }

    /// <summary>
    /// Action called when the player use the submit button in the character selection menu 
    /// Lock the character when the player is not ready 
    /// When the player is master client and everyone is ready, launch the game
    /// </summary>
    private void SubmitInCharacterSelection()
    {
        if (uiState != UIState.InOnlineCharacterSelection) return;
        if (!TDS_GameManager.LocalIsReady)
        {
            SelectCharacter();
            characterSelectionMenu.LocalElement.TriggerToggle(); 
            return;
        }
        if (PhotonNetwork.isMasterClient && launchGameButton && !TDS_GameManager.PlayerListReady.Any(p => p.Value == false) && TDS_GameManager.LocalIsReady)
        {
            TDS_NetworkManager.Instance.LockRoom();
            LoadLevel();
        }
    }

    /// <summary>
    /// Display or remove the curtains in the canvas
    /// </summary>
    public void SwitchArrow()
    {
        if (!arrowAnimator) return;
        arrowAnimator.SetTrigger("Switch");
    }

    /// <summary>
    /// Display or remove the curtains in the canvas
    /// </summary>
    public void SwitchCurtains()
    {
        if (!curtainsAnimator) return;
        curtainsAnimator.SetTrigger("Switch"); 
    }

    /// <summary>
    /// Display or hide the waiting Panel
    /// </summary>
    public void SwitchWaitingPanel()
    {
        if (!waitingPanelAnimator) return;
        waitingPanelAnimator.SetTrigger("SwitchPanel");
    }

    /// <summary>
    /// Unlock a player on the local client
    /// Called when a player leave the room
    /// </summary>
    /// <param name="_playerType">Type of the player to disconnect</param>
    public void UnlockPlayerType(PlayerType _playerType)
    {
        characterSelectionMenu.UnlockCharacterOnline((PlayerType)_playerType); 
    }

    /// <summary>
    /// When a player select a new character, display the image of the character on the others players
    /// </summary>
    /// <param name="_player">Updated player</param>
    /// <param name="_newCharacterSelectionIndex">New Index</param>
    public void UpdateLocalCharacterIndex(PhotonPlayer _player, int _newCharacterSelectionIndex)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdateOnlineCharacterIndex"), new object[] { _player.ID, _newCharacterSelectionIndex });
    }

    /// <summary>
    /// Used Online, update the selection index on others players
    /// </summary>
    /// <param name="_player">id of the updated player</param>
    /// <param name="_newCharacterSelectionIndex">new Index</param>
    public void UpdateOnlineCharacterIndex(int _player, int _newCharacterSelectionIndex) => characterSelectionMenu.UpdateMenuOnline(_player, _newCharacterSelectionIndex);

    public void UpdateOnlineCharacterType(int _player, int _newCharacterSelectionType)
    {
        characterSelectionMenu.UpdateMenuOnline(_player, (PlayerType)_newCharacterSelectionType);
    }

    /// <summary>
    /// Display the number of players in the room and their names
    /// If the player is the master client, also display the launch button
    /// </summary>
    /// <param name="_playerCount">Number of players</param>
    /// <param name="_displayLaunchButton">LaunchButton</param>
    public void UpdatePlayerCount(int _playerCount, bool _displayLaunchButton, PhotonPlayer[] _players)
    {
        if (uiState != UIState.InOnlineCharacterSelection) return;
        if (playerCountText) playerCountText.text = $"Players : {_playerCount}/4";
        if (launchGameButton) launchGameButton.gameObject.SetActive(_displayLaunchButton);
    }

    /// <summary>
    /// Update the player selection informations 
    /// Updated each time a player select a character
    /// </summary>
    /// <param name="_previousPlayerType"></param>
    /// <param name="_nextPlayerType"></param>
    public void UpdatePlayerSelectionInfo(int _previousPlayerType, int _nextPlayerType, int _playerID)
    {
        characterSelectionMenu.UpdateOnlineSelection((PlayerType)_previousPlayerType, (PlayerType)_nextPlayerType, _playerID);
    }

    /// <summary>
    /// Update the ready settings
    /// If everyone is ready, set the launch button as interactable
    /// </summary>
    /// <param name="_playerId"></param>
    /// <param name="_isReady"></param>
    public void UpdateReadySettings(int _playerId, bool _isReady)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        PhotonPlayer _player = PhotonPlayer.Find(_playerId);
        if (TDS_GameManager.PlayerListReady.ContainsKey(_player))
        {
            TDS_GameManager.PlayerListReady[_player] = _isReady;
        }
        if (uiState == UIState.InOnlineCharacterSelection && launchGameButton) launchGameButton.interactable = !TDS_GameManager.PlayerListReady.Any(p => p.Value == false) && TDS_GameManager.LocalIsReady;
        if (uiState == UIState.InGameOver && buttonRestartGame) buttonRestartGame.interactable = !TDS_GameManager.PlayerListReady.Any(p => p.Value == false);
    }
    #endregion

    private void SendInfoToNewPlayer(PhotonPlayer _newPlayer)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", _newPlayer, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdateOnlineCharacterType"), new object[] { PhotonNetwork.player.ID, (int)characterSelectionMenu.LocalElement.CurrentSelection.CharacterType });
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", _newPlayer, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetPlayerReady"), new object[] { PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady });
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            photonView.viewID = 999; 
        }
        else
        {
            Destroy(gameObject);
            return; 
        }
        uiGameObject = transform.GetChild(0).gameObject;
    }

    // Use this for initialization
    private void Start()
    {
        if (playerNameField)
        {
            string _name = $"Guest {(int)UnityEngine.Random.Range(0, 999)}";
            playerNameField.text = _name;
            SetNewName(_name);
        }
        if (uiGameObject)
            uiGameObject.SetActive(true);
        ActivateMenu(uiState);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        SetRoomInterractable(true);
    }

    public override void OnDisconnectedFromPhoton()
    {
        roomSelectionElements.ToList().ForEach(e => e.RoomSelectionButton.interactable = false);
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate(); 
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        characterSelectionMenu.AddNewPlayer(newPlayer);
        SendInfoToNewPlayer(newPlayer);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        base.OnPhotonPlayerDisconnected(otherPlayer);
        characterSelectionMenu.RemovePlayer(otherPlayer); 
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.playerList.ToList().ForEach(p => characterSelectionMenu.AddNewPlayer(p)); 
    }

    #endregion

    #endregion
}
