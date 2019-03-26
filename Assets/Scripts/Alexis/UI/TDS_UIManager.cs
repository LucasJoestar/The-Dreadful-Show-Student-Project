using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDS_UIManager : MonoBehaviour 
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

    #region UIState
    //State of the UI
    [SerializeField] private UIState uiState;
    #endregion

    #region MenuParents
    //Parent of the main menu UI
    [SerializeField] private GameObject mainMenuParent;
    //Parent of the InGame UI
    [SerializeField] private GameObject inGameMenuParent;
    //Parent of the pause menu UI
    [SerializeField] private GameObject pauseMenuParent;
    #endregion

    #region Coroutines
    /// <summary>
    /// Dictionary to stock every filling coroutine started
    /// </summary>
    private Dictionary<Image, Coroutine> filledImages = new Dictionary<Image, Coroutine>(); 
    #endregion 
    #endregion

    #region Methods

    #region Original Methods
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
    /// Fill the image until its fillAmount until it reaches the fillingValue
    /// At the end of the filling, remove the entry of the dictionary at the key _filledImage
    /// </summary>
    /// <param name="_filledImage">Image to fill</param>
    /// <param name="_fillingValue">Fill amount to reach</param>
    /// <returns></returns>
    private IEnumerator UpdateFilledImage(Image _filledImage, float _fillingValue)
    {
        while(_filledImage.fillAmount != _fillingValue)
        {
            _filledImage.fillAmount = Mathf.Lerp(_filledImage.fillAmount, _fillingValue, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        filledImages.Remove(_filledImage);
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
        if(filledImages.ContainsKey(_filledImage))
        {
            StopCoroutine(filledImages[_filledImage]);
            filledImages.Remove(_filledImage); 
        }
        filledImages.Add(_filledImage, StartCoroutine(UpdateFilledImage(_filledImage, _fillingValue))); 
    }
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
		
    }
	
	// Update is called once per frame
	private void Update()
    {
        
	}
	#endregion

	#endregion
}
