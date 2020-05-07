using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_EnemySpriteHolder : MonoBehaviour
{
    /* TDS_EnemySpriteHolder :
    *
    *	#####################
    *	###### PURPOSE ######
    *	#####################
    *
    *	[PURPOSE]
    *	
    *	Script used on a enemy's sprite to send events when the sprite stop rendering. 
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
    *	Date :			
    *	Author :		
    *
    *	Changes :
    *
    *  
    *	-----------------------------------
   */



    #region Fields and properties
    [SerializeField] private TDS_Enemy spriteOwner = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    // OnBecameInvisible is called when the renderer is no longer visible by any camera
    private void OnBecameInvisible()
    {
        if (!spriteRenderer.enabled) return;
        spriteOwner.OnBecameInvisibleCallBack(); 
    }

    // OnBecameVisible is called when the renderer became visible by any camera
    private void OnBecameVisible()
    {
        if (!spriteRenderer.enabled) return;
        spriteOwner.OnBecameVisibleCallBack();
    }
    private void Start()
    {
        // If missing references, just disable it
        if (!spriteOwner || !spriteRenderer)
        {
            enabled = false;
            return;
        }
        if (!spriteRenderer.isVisible)
            spriteOwner.OnBecameInvisibleCallBack();
    }
    #endregion

    #endregion
}
