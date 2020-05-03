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

    #region Animator
    private readonly int hideLeftCurtain_Hash = Animator.StringToHash("HideLeftCurtain");
    private readonly int hideRightCurtain_Hash = Animator.StringToHash("HideRightCurtain");
    #endregion

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
            curtainsAnimator.SetBool(hideLeftCurtain_Hash, true);

            return;
        }
        curtainsAnimator.SetBool(hideRightCurtain_Hash, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.HasTag(detectedTags.ObjectTags) || !curtainsAnimator) return;
        if (isLeftLimit)
        {
            curtainsAnimator.SetBool(hideLeftCurtain_Hash, false);

            return;
        }
        curtainsAnimator.SetBool(hideRightCurtain_Hash, false);       
    }
    #endregion

    #endregion
}
