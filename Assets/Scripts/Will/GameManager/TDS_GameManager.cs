using UnityEngine;

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
    #endregion

    #region Methods
    /// <summary>
    /// Leaves the game, and go back to the menu.
    /// </summary>
    public static void LeaveGame()
    {

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
