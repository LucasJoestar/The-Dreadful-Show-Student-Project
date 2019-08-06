using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public abstract class TDS_LifeBar : MonoBehaviour
{
    /* TDS_LifeBar :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Class that holds the lifebar behaviour]
     *	    - Filling LifeBar 
     *	    - Make it follow its owner 
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     * 
     *  Date :			[28/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Removing the online settings]
     *	    - Remove the inheritance from PunBehaviour and the Interface IPunObservable
     *	    - Set filling values are now set in local
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[01/04/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the TDS_LifeBar class]
     *	    - Create variables: filled Image, Owner, Has To Follow and Offset
     *	    - Create Methods DestroyLifeBar, FollowOwner and SetOwner
	 *
	 *	-----------------------------------
	*/

    #region Events 
    #endregion

    #region Fields and properties
    [SerializeField] protected Image background = null; 
    public Image Background { get { return background; } }
    [SerializeField] protected Image foregroundFilledImage = null; 
    public Image ForegroundFilledImage { get { return foregroundFilledImage; }}
    [SerializeField] protected Image filledImage = null;
    public Image FilledImage { get { return filledImage; } }

    protected TDS_Character owner = null;

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Stop the coroutine that fill the life bar and destroy the game object
    /// </summary>
    public virtual void DestroyLifeBar()
    {
        owner = null; 
        gameObject.SetActive(false); 
    }

    /// <summary>
    /// Set all fill amount to 1
    /// </summary>
    public virtual void ResetLifeBar()
    {
        if(foregroundFilledImage) foregroundFilledImage.fillAmount = 1;
        if(filledImage) filledImage.fillAmount = 1;
        owner = null; 
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the owner of the lifebar 
    /// Link Destroy method on the event OnDie of the owner
    /// </summary>
    /// <param name="_owner">Owner of the life bar</param>
    public virtual void SetOwner(TDS_Character _owner)
    {
        owner = _owner;
    }

    /// <summary>
    /// Fill the lifebar!
    /// </summary>
    public virtual void UpdateLifeBar(int _currentHealth)
    {
        if (!TDS_UIManager.Instance || !owner) return;
        float _fillingValue = Mathf.Clamp((float)owner.HealthCurrent / (float)owner.HealthMax, 0, 1);
        TDS_UIManager.Instance.FillImage(this, _fillingValue); 
    }
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        if (!background) background = transform.GetChild(0).GetComponent<Image>(); 
    }
    #endregion

    #endregion

}
