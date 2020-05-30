using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    /// Indicates if the game is currently in cutscene state.
    /// </summary>
    public static bool IsInCutscene = false;

    /// <summary>
    /// Get if the game is paused or not
    /// </summary>
    public static bool IsPaused { get; set; }

    /// <summary>
    /// Indicates if players are currently in game, or in menu.
    /// </summary>
    private static bool isInGame = true;

    public static bool IsOnline { get; set; } = false;

    private static bool localisationIsEnglish = true; 
    public static bool LocalisationIsEnglish
    {
        get
        {
            return localisationIsEnglish; 
        }
        set
        {
            PlayerPrefs.SetInt("LocalisationIsEnglish", value ? 1 : 0);
            localisationIsEnglish = value; 
        } 
    }
    #endregion

    #region Player Info
    public static List<TDS_PlayerInfo> PlayersInfo = new List<TDS_PlayerInfo>(); 
    #endregion

    #region Players Online
    public static PlayerType LocalPlayer { get; set; }

    public static int PlayerCount
    {
        get { return TDS_LevelManager.Instance.AllPlayers.Length;  }
    }

    public static int CurrentSceneIndex = 0;


    public static bool LocalIsReady
    {
        get
        {
            if (PhotonNetwork.offlineMode)
                return !PlayersInfo.Any(i => !i.IsReady);
            if (PhotonNetwork.connected)
            {
                TDS_PlayerInfo _info = PlayersInfo.Where(i => i.PhotonPlayer == PhotonNetwork.player).FirstOrDefault();
                if (_info == null) return false;
                return _info.IsReady; 
            }
            return false; 
                 
        }
    }
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
    /// Inputs asset referencing all game controllers.
    /// </summary>
    public static TDS_InputSO InputsAsset { get; private set; }

    /// <summary>
    /// Audio asset referencing multi-used audio tracks.
    /// </summary>
    public static TDS_AudioSO AudioAsset { get; private set; }


    /// <summary>
    /// Character used to split the text asset.
    /// </summary>
    private static char splitCharacter = '#';

    /// <summary>
    /// Text asset referencing all game dialogs and others.
    /// </summary>
    public static TextAsset DialogsAsset { get; private set; }
    #endregion

    public static GameObject MainAudio { get; private set; }

    #endregion

    #region Methods
    /// <summary>
    /// Get a dialog with a specific ID.
    /// </summary>
    /// <param name="_id">ID of the dialog to load.</param>
    /// <returns>Returns all text linked to the specified id.</returns>
    public static string[] GetDialog(string _id)
    {
        _id = _id.ToLower();
        return DialogsAsset.text.Split(splitCharacter).Where(d => d.StartsWith(_id)).FirstOrDefault()?.Replace(_id + '\n', string.Empty).Split('\n').Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
    }

    /// <summary>
    /// Get random piece of dialog with a specific ID.
    /// </summary>
    /// <param name="_id">ID of the dialogs.</param>
    /// <returns>Returns all text linked to the chosen dialog.</returns>
    public static string[] GetRandomDialog(string _id)
    {
        string[] _match = DialogsAsset.text.Split(splitCharacter).Where(d => d.StartsWith(_id)).ToArray();

        if (_match.Length > 0)
        {
            return _match[Random.Range(0, _match.Length)].Replace(_id + '\n', string.Empty).Split('\n').Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();
        }

        return new string[] { };
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

        if (!InputsAsset)
        {
            InputsAsset = Resources.Load<TDS_InputSO>(TDS_InputSO.INPUT_ASSET_PATH);
        }
        if (!AudioAsset)
        {
            AudioAsset = Resources.Load<TDS_AudioSO>(TDS_AudioSO.FILE_PATH);
        }

        // Set screen resolution
        Resolution _resolution = new Resolution();
        _resolution.width = Screen.width;
        _resolution.height = Screen.height;

        SetResolution(_resolution);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Method called when any scene is loaded.
    /// </summary>
    /// <param name="_scene">Loaded scene.</param>
    /// <param name="_loadMode">Load scene mode used.</param>
    private static void OnSceneLoaded(Scene _scene, LoadSceneMode _loadMode)
    {
        if (IsInCutscene) IsInCutscene = false;
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
        IsPaused = _doPause;
        TDS_UIManager.Instance?.SetPause(_doPause);
    }

    public static void SetMainCamera(Camera _camera)
    {
        TDS_UIManager.Instance.SetMainCamera(_camera);
    }

    public static void SetMainAudio(GameObject _go)
    {
        MainAudio = _go;
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
        float _cameraAspect = (float)_newResolution.width / _newResolution.height;

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
        TDS_Player _juggler = TDS_LevelManager.Instance?.AllPlayers.Where(p => p.PlayerType == PlayerType.Juggler).FirstOrDefault();

        if (_juggler && _juggler.photonView.isMine)
        {
            ((TDS_Juggler)_juggler).SetAimBounds();
        }
    }
	#endregion
}
