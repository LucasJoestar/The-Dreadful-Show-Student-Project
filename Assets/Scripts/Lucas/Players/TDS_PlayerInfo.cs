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
    public int              PlayerID        { get; private set; }   = 0;
    public PlayerType       PlayerType      { get;  set; }          = PlayerType.Unknown;
    public PhotonPlayer     PhotonPlayer    { get; private set; }   = null;
    public TDS_Controller   Controller      { get; private set; }   = null;
    public bool             IsReady         { get; set; }           = false; 
    #endregion

    #region Constructor
    public TDS_PlayerInfo(int _id, TDS_Controller _controller, PlayerType _type = PlayerType.Unknown )
    {
        PlayerID = _id;
        PlayerType = _type;
        Controller = _controller; 
    }

    public TDS_PlayerInfo(int _id, TDS_Controller _controller, PhotonPlayer _photonPlayer, PlayerType _type = PlayerType.Unknown)
    {
        PlayerID = _id;
        PlayerType = _type;
        Controller = _controller;
        PhotonPlayer = _photonPlayer; 
    }
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #endregion
}
