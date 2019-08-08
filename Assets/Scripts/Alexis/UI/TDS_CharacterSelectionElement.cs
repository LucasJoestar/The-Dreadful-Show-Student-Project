using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using TMPro; 

public class TDS_CharacterSelectionElement : MonoBehaviour 
{
    /* TDS_CharacterSelectionElement :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    private PhotonPlayer photonPlayer = null;
    public PhotonPlayer PhotonPlayer { get { return photonPlayer; } }

    [SerializeField] private TMP_Text playerName = null;
    [SerializeField] private TMP_InputField playerNameInputField = null; 

    [SerializeField] private TDS_CharacterSelectionImage[] characterSelectionImages = new TDS_CharacterSelectionImage[] { };
    public TDS_CharacterSelectionImage[] CharacterSelectionImages
    {
        get { return characterSelectionImages;  }
    }

    [SerializeField] private Toggle readyToggle = null;
    public Toggle ReadyToggle { get { return readyToggle; } }

    [SerializeField] private Button leftArrowButton = null;
    [SerializeField] private Button rightArrowButton = null;

    [SerializeField] private Image localPelletImage = null;
    [SerializeField] private Image ownerCrownImage = null; 

    private int currentIndex = 0;
    private bool isLocked = false; 
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
        set
        {
            isLocked = value;
            readyToggle.animator.SetBool("IsReady", value);
            readyToggle.animator.SetTrigger("ReadyChanged"); 
        }
    }

    public TDS_CharacterSelectionImage CurrentSelection
    {
        get { return characterSelectionImages[currentIndex]; }
    }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Set the photon player and display the element
    /// </summary>
    /// <param name="_player">Photon player linked to the element</param>
    public void SetPhotonPlayer(PhotonPlayer _player)
    {
        photonPlayer = _player;
        if (playerName) playerName.text = _player.NickName;
        if (playerNameInputField) playerNameInputField.text = _player.NickName; 
        if (_player.IsMasterClient && ownerCrownImage) ownerCrownImage.gameObject.SetActive(true);
        gameObject.SetActive(true); 
    }

    /// <summary>
    /// Unlink the photon player and hide the element
    /// </summary>
    public void DisconnectPlayer()
    {
        if(!CurrentSelection.CanBeSelected)
        {
            TDS_UIManager.Instance.UnlockPlayerType(CurrentSelection.CharacterType); 
        }
        //LockElement(false); 
        photonPlayer = null;
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Display the next selectable character of the element
    /// </summary>
    public void DisplayNextImage()
    {
        if (TDS_GameManager.LocalIsReady) return; 
        if (photonPlayer == null) return; 
        if (characterSelectionImages.Where(i => i.CanBeSelected).Count() <= 1) return;
        CurrentSelection.CharacterImage.gameObject.SetActive(false);
        currentIndex = currentIndex++ == characterSelectionImages.Length -1 ? 0 : currentIndex;
        if(!characterSelectionImages[currentIndex].CanBeSelected)
        {
            DisplayNextImage();
            return; 
        }
        CurrentSelection.CharacterImage.gameObject.SetActive(true);
        TDS_UIManager.Instance?.UpdateLocalCharacterIndex(photonPlayer, currentIndex);
    }

    /// <summary>
    /// Display the previous selectable character of the element
    /// </summary>
    public void DisplayPreviousImage()
    {
        if (TDS_GameManager.LocalIsReady) return;
        if (photonPlayer == null) return;
        if (characterSelectionImages.Where(i => i.CanBeSelected).Count() <= 1) return;
        CurrentSelection.CharacterImage.gameObject.SetActive(false);
        currentIndex = currentIndex-- == 0 ? characterSelectionImages.Length -1  : currentIndex;
        if (!characterSelectionImages[currentIndex].CanBeSelected)
        {
            DisplayPreviousImage();
            return;
        }
        CurrentSelection.CharacterImage.gameObject.SetActive(true);
        TDS_UIManager.Instance?.UpdateLocalCharacterIndex(photonPlayer, currentIndex);
    }

    /// <summary>
    /// Display the selectable element at the selected index 
    /// </summary>
    /// <param name="_index">Selected Index</param>
    public void DisplayImageAtIndex(int _index)
    {
        if (photonPlayer == null) return;
        if (_index >= characterSelectionImages.Length || !characterSelectionImages[_index].CanBeSelected) return;
        CurrentSelection.CharacterImage.gameObject.SetActive(false);
        currentIndex = _index;
        CurrentSelection.CharacterImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Display the character's image of type of the character 
    /// </summary>
    /// <param name="_playerType">Type of the character</param>
    public void DisplayImageOfType(PlayerType _playerType)
    {
        if (photonPlayer == null) return;
        CurrentSelection.CharacterImage.gameObject.SetActive(false);
        TDS_CharacterSelectionImage _image = characterSelectionImages.Where(i => i.CharacterType == _playerType).FirstOrDefault();
        if (_image == null) return;
        currentIndex = characterSelectionImages.ToList().IndexOf(_image);
        CurrentSelection.CharacterImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Display visual elements when the player is ready or not
    /// </summary>
    /// <param name="_playerIsReady">Does the elements have to be displayed or not?</param>
    public void LockElement(bool _isPlayerReady)
    {
        // SET THE TOGGLE
        if (photonPlayer != null && PhotonNetwork.player.ID != photonPlayer.ID)
        {
            TriggerToggle();
            IsLocked = _isPlayerReady; 
        }
    }

    /// <summary>
    /// When the player is local link all the selectables and event necessary to navigate and select a character
    /// </summary>
    public void SetPlayerLocal()
    {
        if (localPelletImage) localPelletImage.gameObject.SetActive(true);
        if (playerNameInputField) playerNameInputField.interactable = true; 
        if (leftArrowButton)
        {
            leftArrowButton.interactable = true;
            leftArrowButton.onClick.AddListener(DisplayPreviousImage);
            leftArrowButton.gameObject.SetActive(true); 
        }
        if(rightArrowButton)
        {
            rightArrowButton.interactable = true;
            rightArrowButton.onClick.AddListener(DisplayNextImage);
            rightArrowButton.gameObject.SetActive(true); 
        }      

        Selectable _launchButton = TDS_UIManager.Instance.LaunchGameButton;
        if(_launchButton != null)
        {
            Selectable _returnButton = _launchButton.navigation.selectOnDown;

            Navigation _nav = readyToggle.navigation;
            _nav.mode = Navigation.Mode.Explicit;

            _nav.selectOnDown = _launchButton;
            readyToggle.navigation = _nav;

            _nav = _launchButton.navigation; 
            _nav.selectOnDown = _returnButton;
            _nav.selectOnUp = readyToggle;
            _launchButton.navigation = _nav;

            _nav = _returnButton.navigation;
            _nav.selectOnUp = _launchButton;
            _nav.selectOnDown = null;
            _returnButton.navigation = _nav; 

        }
        readyToggle.interactable = true;
        readyToggle.onValueChanged.AddListener(delegate { TDS_UIManager.Instance.SelectCharacter(); } );
    }

    /// <summary>
    /// Remove the local player and all links and events 
    /// </summary>
    public void ClearToggle()
    {
        readyToggle.onValueChanged.RemoveAllListeners();

        if (localPelletImage) localPelletImage.gameObject.SetActive(false);
        if (playerNameInputField) playerNameInputField.interactable = false;

        if (leftArrowButton)
        {
            leftArrowButton.interactable = false;
            leftArrowButton.onClick.RemoveAllListeners();
            leftArrowButton.gameObject.SetActive(false);
        }
        if (rightArrowButton)
        {
            rightArrowButton.interactable = false;
            rightArrowButton.onClick.RemoveAllListeners();
            rightArrowButton.gameObject.SetActive(false);
        }

        if (isLocked)
        {
            IsLocked = false; 
            TriggerToggle();
            readyToggle.isOn = false;
        }

        readyToggle.interactable = false; 

        Selectable _launchButton = TDS_UIManager.Instance.LaunchGameButton;
        if (_launchButton != null)
        {
            Selectable _returnButton = _launchButton.navigation.selectOnDown;

            Navigation _nav = readyToggle.navigation; 

            _nav.selectOnDown = _returnButton;
            readyToggle.navigation = _nav;

            _nav.selectOnDown = _returnButton;
            _nav.selectOnUp = null;
            _launchButton.navigation = _nav;

            _nav.selectOnUp = _launchButton;
            _nav.selectOnDown = null;
            _returnButton.navigation = _nav;
        }
        TDS_GameManager.LocalIsReady = false; 
    }

    public void ChangeImage(int _axisValue)
    {
        if (_axisValue > 0)
            DisplayNextImage();
        else
            DisplayPreviousImage(); 
    }

    public void TriggerToggle()
    {
        readyToggle.GetComponent<Animator>().SetTrigger(readyToggle.animationTriggers.pressedTrigger);
    }
    #endregion
    #endregion
}
