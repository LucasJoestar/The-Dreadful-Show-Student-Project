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

    #region Fields / Properties

    #region Animator
    private readonly int hideLeftCurtain_Hash = Animator.StringToHash("HideLeftCurtain");
    private readonly int hideRightCurtain_Hash = Animator.StringToHash("HideRightCurtain");
    #endregion

    [SerializeField] LayerMask whatDetect = new LayerMask();

    [SerializeField] private Animator curtainsAnimator = null;

    [SerializeField] private new BoxCollider collider = null;

    [SerializeField] private bool isLeftLimit = true;

    private bool isActive = false;
    #endregion

    #region Methods
    private void Start()
    {
        curtainsAnimator = TDS_UIManager.Instance.CurtainsAnimator; 
    }

    private static Collider[] colliders = new Collider[1];

    private void Update()
    {
        if ((Physics.OverlapBoxNonAlloc(collider.bounds.center, collider.bounds.extents, colliders, Quaternion.identity, whatDetect) > 0) != isActive)
        {
            isActive = !isActive;
            curtainsAnimator.SetBool(isLeftLimit ? hideLeftCurtain_Hash : hideRightCurtain_Hash, isActive);
        }
    }
    #endregion
}
