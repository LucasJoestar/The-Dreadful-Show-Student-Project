using System.Linq;
using UnityEngine;
using System.Collections.Generic; 

#pragma warning disable 0414
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

    #region Fields / Properties

    #region Status
    /// <summary>
    /// Indicates if the game is currently in pause.
    /// </summary>
    private static bool isPaused = false;

    /// <summary>
    /// Indicates if players are currently in game, or in menu.
    /// </summary>
    private static bool isInGame = true;
    #endregion

    #region Players
    public static PlayerType LocalPlayer { get; set; }

    public static int PlayerCount
    {
        get { return PhotonNetwork.room.PlayerCount;  }
    }

    public static int CurrentSceneIndex = 0;

    public static Dictionary<PhotonPlayer, bool> PlayerListReady { get; private set; } = new Dictionary<PhotonPlayer, bool>();

    public static bool LocalIsReady = false;
    #endregion

    #region Resolution
    /// <summary>
    /// Current resolution of the game.
    /// </summary>
    public static Resolution CurrentResolution { get; private set; }

    /// <summary>
    /// Rect of the Camera.
    /// </summary>
    public static Rect CameraRect { get; private set; }
    #endregion

    #region Resources
    /// <summary>
    /// Character used to split the text asset.
    /// </summary>
    private static char splitCharacter = '#';

    /// <summary>
    /// Text asset referencing all game dialogs and others.
    /// </summary>
    public static TextAsset DialogsAsset { get; private set; }
    #endregion

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
    /// Loads the text asset of the game when a game is loaded.
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void InitializeGameManager()
    {
        // Get dialogs asset
        if (!DialogsAsset)
        {
            DialogsAsset = Resources.Load<TextAsset>("Dialogs");
            splitCharacter = DialogsAsset.text[0];
        }

        // Set screen resolution
        SetResolution(Screen.currentResolution);
    }

    /// <summary>
    /// Leaves the game, and go back to the menu.
    /// </summary>
    public static void LeaveGame()
    {

    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public static void Quit()
    {
        Application.Quit();
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
    /// Set the new resolution of the game.
    /// </summary>
    /// <param name="_newResolution">New game resolution.</param>
    public static void SetResolution(Resolution _newResolution)
    {
        CurrentResolution = _newResolution;

        // Calculates camera rect for aspect ratio of 16/9
        float _targetAspect = TDS_Camera.CAMERA_ASPECT_WIDTH / TDS_Camera.CAMERA_ASPECT_HEIGHT;
        float _cameraAspect = (float)Screen.width / Screen.height;

        float _heightRatio = _cameraAspect / _targetAspect;

        // If ratio is correct, the keep it
        if (_heightRatio == 1)
        {
            CameraRect = new Rect(0, 0, 1, 1);
        }
        // If ratio is inferior to one (not large enough), set black bars on top & bottom of the screen
        else if (_heightRatio < 1)
        {
            CameraRect = new Rect(0, (1 - _heightRatio) / 2, 1, _heightRatio);
        }
        // If superior to one (too large), then set black bars on left & right of the screen
        else
        {
            float _widthRatio = 1 / _heightRatio;
            CameraRect = new Rect((1 - _widthRatio) / 2, 0, _widthRatio, 1);
        }

        // Set camera aspect if needed
        TDS_Camera.Instance?.SetCameraAspect();

        // Set aim bounds for local player if Juggler
        if (TDS_LevelManager.Instance?.LocalPlayer?.PlayerType == PlayerType.Juggler)
        {
            ((TDS_Juggler)TDS_LevelManager.Instance.LocalPlayer).SetAimBounds();
        }
    }
	#endregion
}
