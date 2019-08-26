using UnityEngine;

public class TDS_PlayerSpriteHolder : MonoBehaviour 
{
    /* TDS_PlayerSpriteHolder :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
     *	
     *	Script used on a player's sprite to send events when the sprite stop rendering. 
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
	 *	Date :			[05/06/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the PlayerSpriteHolder class]
	 *  - Implementing the variable Owner
     *  - Implementing the Methods OnBecameInvisible and OnBecameVisible
     *  
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Player associated to this script.
    /// </summary>
    public TDS_Player Owner = null;

    /// <summary>
    /// Sprite to check visibility.
    /// </summary>
    public SpriteRenderer PlayerSprite = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Shows or hides out of screen player information.
    /// </summary>
    /// <param name="_isOutOfScreen">Is the player out of screen or not.</param>
    /// <returns>Returns true if changed out of screen info, false if a problem has been encountered.</returns>
    public bool ShowOutOfScreenInfo(bool _isOutOfScreen)
    {
        if (!Owner || Owner.IsDead || Owner.IsInvulnerable || !PlayerSprite.enabled || !Application.isPlaying) return false;

        TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, _isOutOfScreen);
        return true;
    }

    /// <summary>
    /// Shows out of screen player information when he revives.
    /// </summary>
    public void ShowOutOfScreenInfoOnRevive()
    {
        if (!PlayerSprite.enabled || PlayerSprite.isVisible) return;
        TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, true);
    }
    #endregion

    #region Unity Methods
    // OnBecameInvisible is called when the renderer is no longer visible by any camera
    private void OnBecameInvisible()
    {
        if (ShowOutOfScreenInfo(true)) Owner.StartMovingPlayerInView();
    }

    // OnBecameVisible is called when the renderer became visible by any camera
    private void OnBecameVisible()
    {
        if (ShowOutOfScreenInfo(false)) Owner.StopMovingPlayerInView();
    }

    // Use this for initialization
    private void Start()
    {
        // If missing references, just disable it
        if (!Owner || !PlayerSprite)
        {
            enabled = false;
            return;
        }

        Owner.OnDie += () => TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, false);
        Owner.OnRevive += ShowOutOfScreenInfoOnRevive;
    }
    #endregion

    #endregion
}
