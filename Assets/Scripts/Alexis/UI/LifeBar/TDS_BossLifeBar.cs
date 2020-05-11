using System.Linq;
using UnityEngine; 

public class TDS_BossLifeBar : TDS_LifeBar
{
    /* TDS_LifeBar :
    *
    *	#####################
    *	###### PURPOSE ######
    *	#####################
    *
    *	[Class that holds the boss lifebar behaviour]
    *	    - Filling LifeBar 
    *
    *	#####################
    *	### MODIFICATIONS ###
    *	#####################
    *
    *
    *	-----------------------------------
   */

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] private Transform portraitAnchor = null; 
    private TDS_Enemy[] subOwners = null;
    #endregion

    #region Methods

    #region Original Methods
    public override void DestroyLifeBar()
    {
        if ((owner != null && !owner.IsDead) || (subOwners != null && subOwners.Any(e => !e.IsDead))) return;
        owner = null;
        subOwners = null;
        gameObject.SetActive(false);
    }

    public override void SetOwner(TDS_Character _owner)
    {
        base.SetOwner(_owner);
    }

    public void SetSubOwners(TDS_Enemy[] _subOwners)
    {
        subOwners = _subOwners; 
    }

    public void SetBossPortrait(GameObject _portrait)
    {
        if(portraitAnchor.childCount > 0)
        {
            Destroy(portraitAnchor.GetChild(0).gameObject); 
        }
        Instantiate(_portrait, portraitAnchor, false); 
    }

    public override void UpdateLifeBar()
    {
        if (!TDS_UIManager.Instance || !owner && subOwners == null) return;
        float _fillingValue = 0; 
        if (subOwners == null)
        {
            _fillingValue = Mathf.Clamp((float)owner.HealthCurrent / (float)owner.HealthMax, 0, 1);

        }
        else
        {
            float _current = owner != null ? owner.HealthCurrent : 0;
            float _max = owner != null ? owner.HealthMax : 0;
            for (int i = 0; i < subOwners.Length; i++)
            {
                _current += subOwners[i].HealthCurrent;
                _max += subOwners[i].HealthMax; 
            }
            _fillingValue = Mathf.Clamp((float)_current / (float)_max, 0, 1);
        }
        TDS_UIManager.Instance.FillImage(this, _fillingValue);
    }
    #endregion

    #endregion
}
