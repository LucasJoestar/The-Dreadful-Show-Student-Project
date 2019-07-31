using UnityEngine;

public class TDS_PlayerLifeBar : TDS_LifeBar 
{
    /* TDS_PlayerLifeBar :
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
    [SerializeField] protected TDS_ComboManager comboCounter = null;
    [SerializeField] protected PlayerType playerType = PlayerType.Unknown;
    #endregion

    #region Methods

    #region Original Methods
    public override void SetOwner(TDS_Character _owner)
    {
        base.SetOwner(_owner);
        if(comboCounter)
        {
            if (!PhotonNetwork.offlineMode && _owner is TDS_Player && TDS_GameManager.LocalPlayer == playerType)
            {
                ((TDS_Player)_owner).HitBox.OnTouch += comboCounter.IncreaseCombo;
                comboCounter.ResetComboManager(); 
                comboCounter.gameObject.SetActive(true);
                TDS_UIManager.Instance.ComboManager = comboCounter;
            }
            else
            {
                comboCounter.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #endregion
}
