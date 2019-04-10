using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon; 

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
     *	    - Implementing Method to set the life bar of an enemy
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

    #endregion

    #region Fields / Properties
    /// <summary> Singleton of the class TDS_UIManager </summary>
    public static TDS_UIManager Instance;

    #region Buttons

    #region Character Selection
    //Button to select the BeardLady
    [SerializeField] private Button ButtonSelectionBeardLady;
    //Button to select the Juggler
    [SerializeField] private Button ButtonSelectionJuggler;
    //Button to select the FatLady
    [SerializeField] private Button ButtonSelectionFatLady;
    //Button to select the FireEater 
    [SerializeField] private Button ButtonSelectionFireEater;
    #endregion

    #region PauseButton
    [SerializeField] private Button ButtonQuitPause;
    #endregion

    #endregion

    #region Canvas 
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
    private Dictionary<Image, Coroutine> filledImages = new Dictionary<Image, Coroutine>();
    #endregion

    #region LifeBar
    [SerializeField] private TDS_LifeBar playerHealthBar;
    #endregion

    #region MenuParents
    //Parent of the main menu UI
    [SerializeField] private GameObject mainMenuParent;
    //Parent of the InGame UI
    [SerializeField] private GameObject inGameMenuParent;
    //Parent of the pause menu UI
    [SerializeField] private GameObject pauseMenuParent;
    // Parent of the DialogBox
    [SerializeField] private GameObject dialogBoxParent;
    // Parent of the NarratorBox
    [SerializeField] private GameObject narratorBoxParent;
    #endregion

    #region UIState
    //State of the UI
    [SerializeField] private UIState uiState;
    #endregion

    #region Text
    //Text of the dialog Box
    [SerializeField] private TMPro.TMP_Text dialogBoxText ;
    //Text of the narrator Box
    [SerializeField] private TMPro.TMP_Text narratorBoxText;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region IEnumerator
    /// <summary>
    /// Fill the image until its fillAmount until it reaches the fillingValue
    /// At the end of the filling, remove the entry of the dictionary at the key _filledImage
    /// </summary>
    /// <param name="_filledImage">Image to fill</param>
    /// <param name="_fillingValue">Fill amount to reach</param>
    /// <returns></returns>
    private IEnumerator UpdateFilledImage(Image _filledImage, float _fillingValue)
    {
        while (_filledImage.fillAmount != _fillingValue && _filledImage != null)
        {
            _filledImage.fillAmount = Mathf.Lerp(_filledImage.fillAmount, _fillingValue, Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }
        filledImages.Remove(_filledImage);
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
        if (!dialogBoxParent || !dialogBoxText) return;
        dialogBoxText.text = _text;
        dialogBoxParent.SetActive(true);  
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
            case UIState.InMenu:
                mainMenuParent.SetActive(true);
                inGameMenuParent.SetActive(false);
                pauseMenuParent.SetActive(false); 
                break;
            case UIState.InGame:
                mainMenuParent.SetActive(false);
                inGameMenuParent.SetActive(true);
                pauseMenuParent.SetActive(false);
                break;
            case UIState.InPause:
                mainMenuParent.SetActive(false);
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
    public void ActivateNarratorBox(string _text)
    {
        if (!narratorBoxParent || !narratorBoxText) return;
        narratorBoxText.text = _text;
        narratorBoxParent.SetActive(true);
    }

    /// <summary>
    /// Set the dialogbox parent as inactive
    /// </summary>
    public void DesactivateDialogBox()
    {
        if (!dialogBoxParent) return;
        dialogBoxParent.SetActive(false); 
    }

    /// <summary>
    /// Set the dialogbox parent as inactive
    /// </summary>
    public void DesactivateNarratorBox()
    {
        if (!narratorBoxParent) return;
        narratorBoxParent.SetActive(false);
    }

    /// <summary>
    /// Check if the image is already being filled
    /// If so, stop the coroutine and remove it from the dictionary 
    /// Then start the coroutine and stock it with the filledImage as a key in the dictionary
    /// </summary>
    /// <param name="_filledImage">Image to fill</param>
    /// <param name="_fillingValue">Filling value to reach</param>
    public void FillImage(Image _filledImage, float _fillingValue)
    {
        StopFilling(_filledImage); 
        filledImages.Add(_filledImage, StartCoroutine(UpdateFilledImage(_filledImage, _fillingValue))); 
    }

    /// <summary>
    /// Set the events linked to the various Buttons of the UI
    /// Character Selection Button -> Spawn a character and hide the menu
    /// Button Quit Pause -> Hide the pause menu
    /// </summary>
    private void SetButtons()
    {
        //if (!photonView.isMine) return; 
        ButtonSelectionBeardLady.onClick.AddListener(() => TDS_LevelManager.Instance?.Spawn(PlayerType.BeardLady));
        ButtonSelectionJuggler.onClick.AddListener(() => TDS_LevelManager.Instance?.Spawn(PlayerType.Juggler));
        ButtonSelectionFatLady.onClick.AddListener(() => TDS_LevelManager.Instance?.Spawn(PlayerType.FatLady));
        ButtonSelectionFireEater.onClick.AddListener(() => TDS_LevelManager.Instance?.Spawn(PlayerType.FireEater));

        ButtonSelectionBeardLady.onClick.AddListener(() => ActivateMenu(UIState.InGame));
        ButtonSelectionJuggler.onClick.AddListener(() => ActivateMenu(UIState.InGame));
        ButtonSelectionFatLady.onClick.AddListener(() => ActivateMenu(UIState.InGame));
        ButtonSelectionFireEater.onClick.AddListener(() => ActivateMenu(UIState.InGame));

        ButtonQuitPause.onClick.AddListener(() => ActivateMenu(UIState.InGame));
    }

    /// <summary>
    /// Instantiate enemy Life bar
    /// Link it to the enemy
    /// </summary>
    /// <param name="_enemy"></param>
    public void SetEnemyLifebar(TDS_Enemy _enemy)
    {
        Vector3 _offset = Vector3.up * 2; 
        TDS_LifeBar _healthBar = PhotonNetwork.Instantiate("LifeBar", _enemy.transform.position + _offset, Quaternion.identity, 0).GetComponent<TDS_LifeBar>();
        _healthBar.SetOwner(_enemy, _offset, true);
        _enemy.HealthBar = _healthBar.FilledImage; 
        _healthBar.transform.SetParent(canvasWorld.transform);
    }

    /// <summary>
    /// Instantiate player LifeBar and set its owner as the local player
    /// if local 
    /// </summary>
    /// <param name="_player"></param>
    public void SetPlayerLifeBar(TDS_Player _player)
    {
        if(photonView.isMine)
        {
            playerHealthBar.SetOwner(_player);
            _player.HealthBar = playerHealthBar.FilledImage;
            _player.OnTakeDamage += _player.UpdateLifeBar; 
        }

    }

    /// <summary>
    /// Stop the coroutine that fill the image
    /// </summary>
    /// <param name="_filledImage"></param>
    public void StopFilling(Image _filledImage)
    {
        if (filledImages.ContainsKey(_filledImage))
        {
            StopCoroutine(filledImages[_filledImage]);
            filledImages.Remove(_filledImage);
        }
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
        }
        else
        {
            Destroy(this);
            return; 
        }
    }

	// Use this for initialization
    private void Start()
    {
        SetButtons();
    }
	
	// Update is called once per frame
	private void Update()
    {
        
	}
	#endregion

	#endregion
}
