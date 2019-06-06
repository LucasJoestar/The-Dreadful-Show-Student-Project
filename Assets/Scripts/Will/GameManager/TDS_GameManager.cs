using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; 

public static class TDS_GameManager
{
    /* TDS_GameManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manage the whole game. Yep.
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
	 *	     • Moved the spawn system to the newly created LevelManager class.
     *	     
     *	     • Set the class as static instead of inheriting from MonoBehaviour.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[26 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Added the spawn system.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[25 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_GameManager class.
     *	
     *	    Added base content of the class.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>
    /// Indicates if the game is currently in pause.
    /// </summary>
    private static bool isPaused = false;

    /// <summary>
    /// Indicates if players are currently in game, or in menu.
    /// </summary>
    private static bool isInGame = true;

    /// <summary>
    /// Character used to split the text asset.
    /// </summary>
    private static char splitCharacter = '#';

    /// <summary>
    /// Text asset referencing all game dialogs and others.
    /// </summary>
    public static TextAsset DialogsAsset { get; private set; }

    public static PlayerType LocalPlayer { get; set; }

    public static int PlayerCount
    {
        get { return PhotonNetwork.room.PlayerCount;  }
    }

    #endregion

    #region Methods
    /// <summary>
    /// Get a dialog with a specific ID.
    /// </summary>
    /// <param name="_id">ID of the dialog to load.</param>
    /// <returns>Returns all text linked to the specified id.</returns>
    public static string[] GetDialog(string _id)
    {
        return DialogsAsset.text.Split(splitCharacter).Where(d => d.StartsWith(_id)).FirstOrDefault().Replace(_id + '\n', string.Empty).Split('\n').Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
    }

    /// <summary>
    /// Leaves the game, and go back to the menu.
    /// </summary>
    public static void LeaveGame()
    {

    }

    /// <summary>
    /// Loads the text asset of the game when a game is loaded.
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void LoadTextAsset()
    {
        if (DialogsAsset) return;
        DialogsAsset = Resources.Load<TextAsset>("Dialogs");
        splitCharacter = DialogsAsset.text[0];
    }

    /// <summary>
    /// Pause or resume the game.
    /// </summary>
    /// <param name="_doPause">Should the game be paused or resumed</param>
    public static void SetPause(bool _doPause)
    {
        isPaused = _doPause;
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public static void Quit()
    {
        Application.Quit();
    }
	#endregion
}
