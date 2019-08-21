using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_CurtainsLimit : MonoBehaviour 
{
    /* TDS_CurtainsLimit :
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
    /// <summary>
    /// Detected tags of the limit.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags();

    [SerializeField] private Animator curtainsAnimator = null;

    [SerializeField] private bool isLeftLimit = true;
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    private void Start()
    {
        curtainsAnimator = TDS_UIManager.Instance.CurtainsAnimator; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.HasTag(detectedTags.ObjectTags) || !curtainsAnimator) return;
        if (isLeftLimit)
        {
            curtainsAnimator.SetBool("HideLeftCurtain", true);

            return;
        }
        curtainsAnimator.SetBool("HideRightCurtain", true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.HasTag(detectedTags.ObjectTags) || !curtainsAnimator) return;
        if (isLeftLimit)
        {
            curtainsAnimator.SetBool("HideLeftCurtain", false);

            return;
        }
        curtainsAnimator.SetBool("HideRightCurtain", false);       
    }
    #endregion

    #endregion
}
