using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private TDS_CharacterSelectionImage[] characterSelectionImages = new TDS_CharacterSelectionImage[] { };
    public TDS_CharacterSelectionImage[] CharacterSelectionImages
    {
        get { return characterSelectionImages;  }
    }

    [SerializeField] private Image lockingFeedback = null;
    [SerializeField] private Toggle readyToggle = null; 

    private int currentIndex = 0;

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
        gameObject.SetActive(true); 
    }

    /// <summary>
    /// Unlink the photon player and hide the element
    /// </summary>
    public void DisconnectPlayer()
    {
        photonPlayer = null;
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Display the next selectable character of the element
    /// </summary>
    public void DisplayNextImage()
    {
        if (TDS_UIManager.Instance.LocalIsReady) return;
        if (characterSelectionImages.Where(i => i.CanBeSelected).Count() <= 1) return;
        characterSelectionImages[currentIndex].CharacterImage.gameObject.SetActive(false);
        currentIndex = currentIndex++ == characterSelectionImages.Length -1 ? 0 : currentIndex;
        if(!characterSelectionImages[currentIndex].CanBeSelected)
        {
            DisplayNextImage();
            return; 
        }
        characterSelectionImages[currentIndex].CharacterImage.gameObject.SetActive(true);
        TDS_UIManager.Instance?.UpdateLocalCharacterIndex(photonPlayer, currentIndex); 
    }

    /// <summary>
    /// Display the previous selectable character of the element
    /// </summary>
    public void DisplayPreviousImage()
    {
        if (TDS_UIManager.Instance.LocalIsReady) return;
        if (characterSelectionImages.Where(i => i.CanBeSelected).Count() <= 1) return;
        characterSelectionImages[currentIndex].CharacterImage.gameObject.SetActive(false);
        currentIndex = currentIndex-- == 0 ? characterSelectionImages.Length -1  : currentIndex;
        if (!characterSelectionImages[currentIndex].CanBeSelected)
        {
            DisplayPreviousImage();
            return;
        }
        characterSelectionImages[currentIndex].CharacterImage.gameObject.SetActive(true);
        TDS_UIManager.Instance?.UpdateLocalCharacterIndex(photonPlayer, currentIndex);
    }

    /// <summary>
    /// Display the selectable element at the selected index 
    /// </summary>
    /// <param name="_index">Selected Index</param>
    public void DisplayImageAtIndex(int _index)
    {
        if (!characterSelectionImages[_index].CanBeSelected) return; 
        characterSelectionImages[currentIndex].CharacterImage.gameObject.SetActive(false);
        currentIndex = _index;
        characterSelectionImages[currentIndex].CharacterImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Select the character at the current index to play
    /// </summary>
    public void SelectCharacter()
    {
        if (!characterSelectionImages[currentIndex].CanBeSelected) return;
        TDS_UIManager.Instance?.SelectCharacter((int)characterSelectionImages[currentIndex].CharacterType); 
    }

    /// <summary>
    /// Display visual elements when the player is ready or not
    /// </summary>
    /// <param name="_playerIsReady">Does the elements have to be displayed or not?</param>
    public void LockElement(bool _playerIsReady)
    {
        // SET THE TOGGLE
        if(lockingFeedback) lockingFeedback.gameObject.SetActive(_playerIsReady); 
    }

    public void SetPlayerReady()
    {
        LockElement(readyToggle.isOn);
        TDS_UIManager.Instance.OnPlayerReady(readyToggle.isOn); 
    }

    public void ActivateToggle()
    {
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
        readyToggle.onValueChanged.AddListener(delegate { TDS_UIManager.Instance.SelectCharacter((int)characterSelectionImages[currentIndex].CharacterType);  });
        readyToggle.Select(); 
    }

    public void ClearToggle()
    {
        readyToggle.onValueChanged.RemoveAllListeners();
        readyToggle.isOn = false;
        readyToggle.interactable = false; 
        Selectable _launchButton = TDS_UIManager.Instance.LaunchGameButton;
        if (_launchButton != null)
        {
            Selectable _returnButton = _launchButton.navigation.selectOnDown;

            Navigation _nav = new Navigation();
            _nav.mode = Navigation.Mode.Explicit;

            _nav.selectOnDown = _returnButton;
            readyToggle.navigation = _nav;

            _nav.selectOnDown = _returnButton;
            _nav.selectOnUp = null;
            _launchButton.navigation = _nav;

            _nav.selectOnUp = _launchButton;
            _nav.selectOnDown = null;
            _returnButton.navigation = _nav;
        }
    }
    #endregion

	#endregion
}
