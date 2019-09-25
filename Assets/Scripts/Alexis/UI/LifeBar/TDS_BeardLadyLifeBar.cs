using UnityEngine;
using UnityEngine.UI; 

public class TDS_BeardLadyLifeBar : TDS_PlayerLifeBar 
{
    /* TDS_BeardLadyLifeBar :
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
    [SerializeField] private Image beardStateBar = null;
    [SerializeField] private Image beardStateFilleBar = null;
    //[SerializeField] private float fillingTimer = .25f;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Set the owner and add the methods on the linked events
    /// </summary>
    /// <param name="_owner"></param>
    public override void SetOwner(TDS_Character _owner)
    {
        if (_owner is TDS_BeardLady)
        {
            base.SetOwner(_owner);
            if (!beardStateBar) return; 
            if (PhotonNetwork.offlineMode || (TDS_GameManager.LocalPlayer == PlayerType.BeardLady))
            {
                beardStateBar.gameObject.SetActive(true);
                ((TDS_BeardLady)_owner).OnBeardStateChanged += ChangeBeardState; 
                return; 
            }
            beardStateBar.gameObject.SetActive(false); 
        }
    }

    /// <summary>
    /// Change the beard lady state on the beard bar
    /// </summary>
    /// <param name="_state"></param>
    private void ChangeBeardState(BeardState _state)
    {
        if (!beardStateFilleBar) return;
        beardStateFilleBar.fillAmount = (float)((int)_state +1) / ((int)BeardState.VeryVeryLongDude+1);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        if (playerType != PlayerType.BeardLady)
            playerType = PlayerType.BeardLady;
    }
	#endregion

	#endregion
}
