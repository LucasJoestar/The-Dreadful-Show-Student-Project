using System;

[Serializable]
public class TDS_PlayerInfo  
{
    /* TDS_PlayerInfo :
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

    #region Events

    #endregion

    #region Fields / Properties
    public int              PlayerID            { get; private set; }   = 0;
    public PlayerType       PlayerType          { get;  set; }          = PlayerType.Unknown;
    public TDS_PlayerScore  PlayerScore         { get; set; }           = new TDS_PlayerScore();
    public TDS_PlayerScore  PreviousLevelScore  { get; set; } = new TDS_PlayerScore();
    public PhotonPlayer     PhotonPlayer        { get; private set; }   = null;
    public TDS_Controller   Controller          { get; private set; }   = null;
    public bool             IsReady             { get; set; }           = false;
    public int            Health                { get; set; }           = 0;
    #endregion

    #region Constructor
    public TDS_PlayerInfo(int _id, TDS_Controller _controller, PlayerType _type = PlayerType.Unknown )
    {
        PlayerID = _id;
        PlayerType = _type;
        Controller = _controller;
        TDS_SceneManager.OnLoadScene += UpdateScoreOnLevel;
    }

    public TDS_PlayerInfo(int _id, TDS_Controller _controller, PhotonPlayer _photonPlayer, PlayerType _type = PlayerType.Unknown)
    {
        PlayerID = _id;
        PlayerType = _type;
        Controller = _controller;
        PhotonPlayer = _photonPlayer;
        TDS_SceneManager.OnLoadScene += UpdateScoreOnLevel;
    }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Updates players score based on new level loaded.
    /// </summary>
    /// <param name="_sceneIndex">Build index of level loaded.</param>
    private void UpdateScoreOnLevel(int _sceneIndex)
    {
        if (_sceneIndex == TDS_GameManager.CurrentSceneIndex)
        {
            PlayerScore = PreviousLevelScore;
            return;
        }
        PreviousLevelScore = PlayerScore;
    }
    #endregion

    #endregion
}
