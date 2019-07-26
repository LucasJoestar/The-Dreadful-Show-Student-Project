using UnityEngine;
using UnityEngine.UI;

public class TDS_FatLadyLifeBar : TDS_PlayerLifeBar 
{
    /* TDS_FatLadyLifeBar :
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
    [SerializeField] Image feedingStatusBar = null;
    [SerializeField] Image feedingStatusFilledBar = null;
    [SerializeField] Image foregroundImage = null; 
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Set the owner and add the methods on the linked events
    /// </summary>
    /// <param name="_owner"></param>
    public override void SetOwner(TDS_Character _owner)
    {
        if (_owner is TDS_FatLady)
        {
            base.SetOwner(_owner);
            if (PhotonNetwork.offlineMode || TDS_GameManager.LocalPlayer == PlayerType.FatLady)
            {
                feedingStatusBar.gameObject.SetActive(true);
                ((TDS_FatLady)_owner).OnAteSnack += StartFeedingBar;
                ((TDS_FatLady)_owner).OnRestauringSnack += SetFeedingBarFilledValue;
                ((TDS_FatLady)_owner).OnRestauredSnack += ResetFeedingBar;
                return;
            }
            feedingStatusBar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Set the filled amount
    /// </summary>
    /// <param name="_newValue"></param>
    private void SetFeedingBarFilledValue(float _newValue)
    {
        if (feedingStatusFilledBar)
            feedingStatusFilledBar.fillAmount = Mathf.MoveTowards(feedingStatusFilledBar.fillAmount,_newValue, Time.deltaTime);
    }

    /// <summary>
    /// Set the color of the foreground to grey 
    /// Set the fill amount to 0
    /// </summary>
    private void StartFeedingBar()
    {
        if (foregroundImage) foregroundImage.color = Color.grey; 
        if (feedingStatusFilledBar) feedingStatusFilledBar.fillAmount = 0; 
    }

    /// <summary>
    /// Set the color of the foreground to white 
    /// Set the fill amount to 1
    /// </summary>
    private void ResetFeedingBar()
    {
        if (foregroundImage) foregroundImage.color = Color.white;
        if (feedingStatusFilledBar) feedingStatusFilledBar.fillAmount = 1;
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (playerType != PlayerType.FatLady)
            playerType = PlayerType.FatLady;
    }
    #endregion

    #endregion
}
