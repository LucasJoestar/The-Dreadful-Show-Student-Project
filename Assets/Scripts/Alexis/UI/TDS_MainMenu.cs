using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro; 

public class TDS_MainMenu : MonoBehaviour 
{
    /* TDS_MainMenu :
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

    #region Fields and properties 

    #region Singleton
    public static TDS_MainMenu Instance; 
    #endregion

    #region MenuParents
    [Header("Menu Parents")]
    [SerializeField] private GameObject mainMenuParent;
    [SerializeField] private GameObject roomMenuParent;
    [SerializeField] private GameObject characterSelectionMenuParent;
    #endregion

    #region CharacterSelectionMenus
    [Header("Character Selection Menu")]
    [SerializeField] private GameObject[] characterSelectionMenus;
    [SerializeField] private Button[] beardLadyButtons;
    [SerializeField] private Button[] fatLadyButtons;
    [SerializeField] private Button[] jugglerButtons;
    [SerializeField] private Button[] fireEaterButtons;
    [SerializeField] private Button[] ResetButtons;
    #endregion

    #region TextField
    [Header("Player Name")]
    [SerializeField] private TMP_Text playerName;
    #endregion

    #region Buttons
    [Header("Launch Game Button")]
    [SerializeField] private Button LaunchGameButton; 
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    public void LoadLevel()
    {
        SceneManager.LoadScene(1); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void UpdateMenuState(UIState _state)
    {
        switch (_state)
        {
            case UIState.InMainMenu:
                if(mainMenuParent) mainMenuParent.SetActive(true);
                if(roomMenuParent) roomMenuParent.SetActive(false);
                if(characterSelectionMenuParent) characterSelectionMenuParent.SetActive(false);
                break;
            case UIState.InRoomSelection:
                if (mainMenuParent) mainMenuParent.SetActive(false);
                if(roomMenuParent)roomMenuParent.SetActive(true);
                if (characterSelectionMenuParent) characterSelectionMenuParent.SetActive(false);
                break;
            case UIState.InCharacterSelection:
                if (mainMenuParent) mainMenuParent.SetActive(false);
                if(roomMenuParent)roomMenuParent.SetActive(false);
                if (characterSelectionMenuParent) characterSelectionMenuParent.SetActive(true);
                break;
            case UIState.InGame:
                break;
            case UIState.InPause:
                break;
            default:
                break;
        }
    }
    #endregion

    #region UnityMethods
    public void Awake()
    {
        if(Instance)
        {
            Destroy(this); 
            return; 
        }
        Instance = this; 
    }
    #endregion 

    #endregion
}
