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

    #region Unity Methods
    // OnBecameInvisible is called when the renderer is no longer visible by any camera
    private void OnBecameInvisible()
    {
        if (!Owner || Owner.IsDead || Owner.IsInvulnerable || !PlayerSprite.enabled || !Application.isPlaying ) return; 
        TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, true);
    }

    // OnBecameVisible is called when the renderer became visible by any camera
    private void OnBecameVisible()
    {
        if (!Owner || Owner.IsDead || Owner.IsInvulnerable || !PlayerSprite.enabled || !Application.isPlaying) return;
        TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, false);
    }

    // Use this for initialization
    private void Start()
    {
        // If missing references, just disable it
        if (!Owner || !PlayerSprite) enabled = false;
    }
    #endregion
}
