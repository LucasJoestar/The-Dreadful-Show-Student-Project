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
    [SerializeField] private TDS_PlayerLifeBar beardLadyLifeBar;
    [SerializeField] private TDS_PlayerLifeBar fatLadyLifeBar;
    [SerializeField] private TDS_PlayerLifeBar jugglerLifeBar;
    [SerializeField] private TDS_PlayerLifeBar fireEaterLifeBar;

    #region Hidden Players Images
    [Header("Hidden Player's Images")]
    [SerializeField] private Image hiddenBeardLadyImage;
    [SerializeField] private Image hiddenFatLadyImage;
    [SerializeField] private Image hiddenJugglerImage;
    [SerializeField] private Image hiddenFireEaterImage;
    #endregion
    #endregion

    #region Feedback
    // Animator used to display cutscene black bars.
    [Header("Feedback")]
    [SerializeField] private Animator cutsceneBlackBarsAnimator = null;

    // Animator of the Juggler's aim target, giving player feedback
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
    [SerializeField] private TDS_RoomSelectionManager roomSelectionManager = null;
    public TDS_RoomSelectionManager RoomSelectionManager { get { return roomSelectionManager; } }
    #endregion

    #region Character Selection Menu
    [SerializeField] private TDS_CharacterSelectionManager characterSelectionManager = null;
    public TDS_CharacterSelectionManager CharacterSelectionManager { get { return characterSelectionManager; } }
    #endregion


    #region TextField
        [SerializeField] private TMP_Text playerNameField;
    public TMP_Text PlayerNameField
    {
        get { return playerNameField; }
    }
    [SerializeField] private TMP_Text playerCountText;
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
    [SerializeField] TDS_EnemyLifeBar enemyHealthBar = null;
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
        Action _cancelAction = null;
        Action _submitAction = null;
        Action<int> _horizontalAxisAction = null;

        switch (_state)
        {
            case UIState.InMainMenu:
                break;
            case UIState.InRoomSelection:
                _cancelAction = () => ActivateMenu(UIState.InMainMenu); ;
                break;
            case UIState.InCharacterSelection:
                if (!characterSelectionManager) break;
                _cancelAction = characterSelectionManager.CancelInOnlineCharacterSelection;
                _submitAction = characterSelectionManager.SubmitInOnlineCharacterSelection;
                while (characterSelectionManager.CharacterSelectionMenu.LocalElement == null)
                {
                    yield return null; 
                }
                _horizontalAxisAction = characterSelectionManager.CharacterSelectionMenu.LocalElement.ChangeImage;
                break;
            case UIState.InGame:
                break;
            case UIState.InPause:
                _cancelAction = () => ActivateMenu(UIState.InGame);
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
                _cancelAction?.Invoke();
            }
            else if (TDS_InputManager.GetButtonDown(TDS_InputManager.SUBMIT_BUTTON))
            {
                yield return new WaitForEndOfFrame();
                _submitAction?.Invoke();
            }
            else if (TDS_InputManager.GetAxisDown(TDS_InputManager.HORIZONTAL_AXIS, out _value))
            {
                yield return new WaitForEndOfFrame();
                _horizontalAxisAction?.Invoke(_value);
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
    
    private IEnumerator PrepareConnectionToPhoton()
    {
        DisplayLoadingScreen(true);
        float _timer = 30; 
        while (_timer > 0)
        {
            if(PhotonNetwork.insideLobby)
            {
                yield return new WaitForSeconds(.25f);
                DisplayLoadingScreen(false);
                ActivateMenu(UIState.InRoomSelection); 
                yield break; 
            }
            yield return null; 
            _timer -= Time.deltaTime; 
        }
        DisplayLoadingScreen(false);
        ActivateMenu(UIState.InMainMenu);
        yield break;
    }
    #endregion

    #region void
    /// <summary>
    /// Activates the cutscene black bars, on top & bottom of screen.
    /// </summary>
    public void ActivateCutsceneBlackBars()
    {
        cutsceneBlackBarsAnimator.SetBool("IsActivated", true);
    }

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
                if (!PhotonNetwork.connected)
                {
                    StartCoroutine(PrepareConnectionToPhoton());
                    break; 
                }

                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(true);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(false);
                Selectable.allSelectables.FirstOrDefault()?.Select();

                if(roomSelectionManager) StartCoroutine(roomSelectionManager.UpdatePlayerCount());
                if (checkInputCoroutine != null)
                    StopCoroutine(checkInputCoroutine);
                checkInputCoroutine = StartCoroutine(CheckInputMenu(UIState.InRoomSelection)); 
                break;
            case UIState.InCharacterSelection:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(true);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                gameOverScreenParent.SetActive(false);

                if (checkInputCoroutine != null)
                    StopCoroutine(checkInputCoroutine);
                checkInputCoroutine = StartCoroutine(CheckInputMenu(UIState.InCharacterSelection));
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
    /// Desactivates the cutscene black bars, on top & bottom of screen.
    /// </summary>
    public void DesactivateCutsceneBlackBars()
    {
        cutsceneBlackBarsAnimator.SetBool("IsActivated", false);
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
        characterSelectionManager.CharacterSelectionMenu.LocalElement.ClearToggle(); 
        if (isloadingNextScene)
        {
            //if (PhotonNetwork.isMasterClient)
              //  TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LoadLevel"), new object[] { });
            TDS_SceneManager.Instance?.PrepareOnlineSceneLoading(TDS_GameManager.CurrentSceneIndex + 1, (int)UIState.InGame);
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
        if (enemyHealthBar == null || !canvasWorld) return;
        Vector3 _offset = (Vector3.up * .2f) - Vector3.forward; 
        TDS_EnemyLifeBar _healthBar = Instantiate(enemyHealthBar, _enemy.transform.position + _offset, Quaternion.identity, canvasWorld.transform).GetComponent<TDS_EnemyLifeBar>();

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
        Debug.Log("Set => " + _state);
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
        TDS_PlayerLifeBar _playerLifeBar = null;
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
        _player.OnHealthChanged += _playerLifeBar.UpdateLifeBar;
        if (_player is TDS_Juggler _juggler) _juggler.OnHasObject += _playerLifeBar.DisplayThrowObjectInfo;
        else _player.OnHasObject += _playerLifeBar.DisplayThrowObjectInfo;
        if (_player == TDS_LevelManager.Instance.LocalPlayer && _player.photonView.isMine)
            _playerLifeBar.transform.SetSiblingIndex(0);
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
        //if (playerNameField) playerNameField.text = _newName; 
    }

    public void StartLeavingRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "StartLeavingRoom"), new object[] { });
        }
        StartCoroutine(characterSelectionManager.PreapreLeavingRoom());
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
    public void SwitchCurtains(bool _areVisible)
    {
        if (!curtainsAnimator) return;
        curtainsAnimator.SetBool("Visible", _areVisible); 
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
        if (uiState == UIState.InCharacterSelection && launchGameButton) launchGameButton.interactable = !TDS_GameManager.PlayerListReady.Any(p => p.Value == false) && TDS_GameManager.LocalIsReady;
        if (uiState == UIState.InGameOver && buttonRestartGame) buttonRestartGame.interactable = !TDS_GameManager.PlayerListReady.Any(p => p.Value == false);
    }

    /// <summary>
    /// Display the number of players in the room and their names
    /// If the player is the master client, also display the launch button
    /// </summary>
    /// <param name="_playerCount">Number of players</param>
    /// <param name="_displayLaunchButton">LaunchButton</param>
    public void UpdatePlayerCount(int _playerCount, bool _displayLaunchButton, PhotonPlayer[] _players)
    {
        if (uiState != UIState.InCharacterSelection) return;
        if (playerCountText) playerCountText.text = $"Players : {_playerCount}/4";
        if (launchGameButton) launchGameButton.gameObject.SetActive(_displayLaunchButton);
    }
    #endregion

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
        if (!characterSelectionManager) characterSelectionManager = GetComponent<TDS_CharacterSelectionManager>(); 
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

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate(); 
    }
    #endregion

    #endregion
}
