using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using Photon;
using TMPro; 

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
     *	
     *	Date :			[28/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Set the lifebars managed in local]
     *	    - Set a new SerializeField for the prefab of the lifebars
     *	    - Modify methods to manage the lifebat in local
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

    #region GameObject
    [SerializeField] private GameObject uiGameObject;
    #endregion

    #region Buttons

    /*
    [Header("Character Selection")]
    //Button to select the BeardLady
    [SerializeField] private Button buttonSelectionBeardLady;
    //Button to select the Juggler
    [SerializeField] private Button buttonSelectionJuggler;
    //Button to select the FatLady
    [SerializeField] private Button buttonSelectionFatLady;
    //Button to select the FireEater 
    [SerializeField] private Button buttonSelectionFireEater;
    */
    #region CharacterSelectionMenus
    [Header("Character Selection Menu")]
    [SerializeField] private TDS_CharacterMenuSelection characterSelectionMenu;
    [SerializeField] private TMP_Text[] playersName;
    #endregion

    #region TextField
    [Header("Player Name")]
    [SerializeField] private TMP_Text playerNameField;
    public TMP_Text PlayerNameField
    {
        get { return playerNameField; }
    }
    [Header("Player counter")]
    [SerializeField] private TMP_Text playerCountText;
    #endregion

    #region Buttons
    [Header("Buttons")]
    [SerializeField] private Button launchGameButton;
    [SerializeField] private Button buttonQuitPause;
    [SerializeField] private Button buttonQuitGame; 
    #endregion

    #endregion

    #region Animator
    [Header("Animator")]
    [SerializeField] private Animator curtainsAnimator;
    [SerializeField] private Animator arrowAnimator; 
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

    #region Coroutines
    /// <summary>
    /// Dictionary to stock every filling coroutine started
    /// </summary>
    private Dictionary<TDS_LifeBar, Coroutine> filledImages = new Dictionary<TDS_LifeBar, Coroutine>();

    private Coroutine narratorCoroutine; 
    
    #endregion

    #region LifeBar
    [Header("LifeBars")]
    [SerializeField] private TDS_LifeBar playerHealthBar;
    [SerializeField] private TDS_LifeBar bossHealthBar;

    #endregion

    #region OnlineLifeBarParent
    [Header("Online lifebars")]
    [SerializeField] private TDS_LifeBar onlineBeardLadyLifeBar;
    [SerializeField] private TDS_LifeBar onlineFatLadyLifeBar;
    [SerializeField] private TDS_LifeBar onlineJugglerLifeBar;
    [SerializeField] private TDS_LifeBar onlineFireEaterLifeBar;
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
    // Parent of the NarratorBox
    [SerializeField] private GameObject narratorBoxParent;
    #endregion

    #region Hidden Players Images
    [Header("Parents of the Hidden Player's Images")]
    [SerializeField] private Transform hiddenPlayerParentLeft;
    [SerializeField] private Transform hiddenPlayerParentRight;

    [Header("Hidden Player's Images")]
    [SerializeField] private Image hiddenBeardLadyImage;
    [SerializeField] private Image hiddenFatLadyImage;
    [SerializeField] private Image hiddenJugglerImage;
    [SerializeField] private Image hiddenFireEaterImage; 
    #endregion 

    #region UIState
    [Header("UI State")]
    //State of the UI
    [SerializeField] private UIState uiState;
    public UIState UIState { get { return uiState;  } }
    #endregion

    #region Text
    [Header("Dialog/Narrator Box")]
    //Text of the dialog Box
    [SerializeField] private TMPro.TMP_Text dialogBoxText ;
    //Text of the narrator Box
    [SerializeField] private TMPro.TMP_Text narratorBoxText;
    #endregion

    #region Resources
    [Header("LifeBar")]
    [SerializeField] GameObject lifeBarPrefab = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region IEnumerator
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
            _lifebar.FilledImage.fillAmount = Mathf.MoveTowards(_lifebar.FilledImage.fillAmount, _fillingValue, Time.deltaTime/2 );
            yield return null;
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
                break;
            case UIState.InRoomSelection:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(true);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                break;
            case UIState.InCharacterSelection:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(true);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false);
                break;
            case UIState.InGame:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(true);
                pauseMenuParent.SetActive(false);
                break;
            case UIState.InPause:
                mainMenuParent.SetActive(false);
                roomSelectionMenuParent.SetActive(false);
                characterSelectionMenuParent.SetActive(false);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(true);
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
        if (_isInvisible && Camera.main != null && _player != null)
        {
            if (_player.transform.position.x > Camera.main.transform.position.x)
            {
                _image.transform.SetParent(hiddenPlayerParentRight);
            }
            else
            {
                _image.transform.SetParent(hiddenPlayerParentLeft);
            }
        }
        _image.gameObject.SetActive(_isInvisible);
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
    /// Set a button as interractable
    /// Call the method online to set the button interractability on each player
    /// </summary>
    /// <param name="_b">Button to set</param>
    /// <param name="_type">PlayerType to instanciate</param>
    /// <param name="_isInteractable">Interractability of the button</param>
    private void SetButtonInteractable(Button _b, PlayerType _type, bool _isInteractable)
    {
        _b.interactable = _isInteractable;
        if (PhotonNetwork.connected) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetButtonInteractable"), new object[] { (int)_type, _isInteractable });
    }

    /// <summary>
    /// Set a selection button interractable 
    /// Called online
    /// </summary>
    /// <param name="_buttonType">Type of player</param>
    /// <param name="_isInteractable">Interractability</param>
    private void SetButtonInteractable(int _buttonType, bool _isInteractable)
    {
        switch ((PlayerType)_buttonType)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                characterSelectionMenu.BeardLadyButton.interactable = _isInteractable;
                break;
            case PlayerType.FatLady:
                characterSelectionMenu.FatLadyButton.interactable = _isInteractable;
                break;
            case PlayerType.FireEater:
                characterSelectionMenu.FireEaterButton.interactable = _isInteractable;
                break;
            case PlayerType.Juggler:
                characterSelectionMenu.JugglerButton.interactable = _isInteractable;
                break;
            default:
                break;
        }
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

        _boss.OnTakeDamage += _boss.UpdateLifeBar;
        _boss.OnDie += () => bossHealthBar.gameObject.SetActive(false);
    }

    /// <summary>
    /// Instantiate enemy Life bar
    /// Link it to the enemy
    /// </summary>
    /// <param name="_enemy"></param>
    public void SetEnemyLifebar(TDS_Enemy _enemy)
    {
        if (lifeBarPrefab == null || !canvasWorld) return; 
        Vector3 _offset =  Vector3.up * _enemy.transform.localScale.y * 3; 
        TDS_LifeBar _healthBar = UnityEngine.Object.Instantiate(lifeBarPrefab, _enemy.transform.position + _offset, Quaternion.identity, canvasWorld.transform).GetComponent<TDS_LifeBar>();

        _healthBar.SetOwner(_enemy, _offset, true);
        _enemy.HealthBar = _healthBar;

        _enemy.OnTakeDamage += _enemy.UpdateLifeBar; 
    }

    /// <summary>
    /// Instantiate player LifeBar and set its owner as the local player
    /// if local 
    /// </summary>
    /// <param name="_player"></param>
    public void SetPlayerLifeBar(TDS_Player _player)
    {
        TDS_LifeBar _playerLifeBar = null;
        if (_player == TDS_LevelManager.Instance.LocalPlayer && _player.photonView.isMine)
        {
            _playerLifeBar = playerHealthBar; 
        }
        else
        {
            switch (_player.PlayerType)
            {
                case PlayerType.Unknown:
                    break;
                case PlayerType.BeardLady:
                    _playerLifeBar = onlineBeardLadyLifeBar; 
                    break;
                case PlayerType.FatLady:
                    _playerLifeBar = onlineFatLadyLifeBar;
                    break;
                case PlayerType.FireEater:
                    _playerLifeBar = onlineFireEaterLifeBar;
                    break;
                case PlayerType.Juggler:
                    _playerLifeBar = onlineJugglerLifeBar;
                    break;
                default:
                    break;
            }
            if (!_playerLifeBar) return; 
            
        }
        _playerLifeBar.gameObject.SetActive(true);
        _playerLifeBar.SetOwner(_player);
        _player.HealthBar = _playerLifeBar;
        _player.OnTakeDamage += _player.UpdateLifeBar;
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
    public void SwitchCurtains()
    {
        if (!curtainsAnimator) return;
        curtainsAnimator.SetTrigger("Switch"); 
    }

    //-------------------MAIN MENU MIGRATION-------------------//
    /// <summary>
    /// Load the next level 
    /// If Master client, send the information to the other players
    /// </summary>
    public void LoadLevel()
    {
        if (PhotonNetwork.isMasterClient)
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LoadLevel"), new object[] { });
        SceneManager.LoadScene(1);
        ActivateMenu(UIState.InGame); 
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
        for (int i = 0; i < _playerCount; i++)
        {
            playersName[i].text = _players[i].NickName;
            playersName[i].gameObject.SetActive(true);
        }
        for (int i = _playerCount; i < playersName.Length; i++)
        {
            playersName[i].gameObject.SetActive(false);
        }

        if (launchGameButton) launchGameButton.gameObject.SetActive(_displayLaunchButton);
    }

    /// <summary>
    /// Update the player selection informations 
    /// Updated each time a player select a character
    /// </summary>
    /// <param name="_previousPlayerType"></param>
    /// <param name="_nextPlayerType"></param>
    public void UpdatePlayerSelectionInfo(int _previousPlayerType, int _nextPlayerType)
    {
        characterSelectionMenu.UpdateMenuOnline((PlayerType)_previousPlayerType, (PlayerType)_nextPlayerType);
    }

    /// <summary>
    /// Select a new character (used in UnityEvent)
    /// </summary>
    /// <param name="_newPlayerType">Index of the enum PlayerType</param>
    public void SelectCharacter(int _newPlayerType)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdatePlayerSelectionInfo"), new object[] { (int)TDS_GameManager.LocalPlayer, _newPlayerType });
        TDS_GameManager.LocalPlayer = (PlayerType)_newPlayerType == TDS_GameManager.LocalPlayer ? PlayerType.Unknown : (PlayerType)_newPlayerType;
        characterSelectionMenu.UpdateLocalSelection();
    }

    public void QuitGame() => Application.Quit(); 
    //---------------------------------------------------------//

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return; 
        }
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        if (uiGameObject)
            uiGameObject.SetActive(true);       
        if (playerNameField)
        {
            playerNameField.text = "New Player";
            playerNameField.GetComponentInChildren<TMP_Text>().text = "NewPlayer";
            SetNewName();
        }
    }
	#endregion

	#endregion
}
