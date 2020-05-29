using System.Collections;
using UnityEngine;

public class TDS_FireBall : TDS_Object 
{
    /* TDS_FireBall :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages the behaviour of the fire balls invoked by the Fire Eater.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[28 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_FireBall class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Animator
    private static readonly int explosion_Hash = Animator.StringToHash("Explosion");
    #endregion

    /// <summary>
    /// Animator of the Fire Ball.
    /// </summary>
    [SerializeField] private Animator animator = null;

    /// <summary>
    /// Indicates if the fire ball is actually destroying itself.
    /// </summary>
    [SerializeField] private bool isDestroying = false;

    /// <summary>
    /// Maximum lifetime of the fire ball.
    /// </summary>
    [SerializeField] private float lifetime = 7;

    /// <summary>
    /// Speed of the fire ball.
    /// </summary>
    [SerializeField] private float speed = 5;

    /// <summary>
    /// Hit box of the fire ball.
    /// </summary>
    [SerializeField] private TDS_HitBox hitBox = null;
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Call the method to make the fire ball explode for all players.
    /// </summary>
    private void CallExplode()
    {
        TDS_RPCManager.Instance.CallRPC(PhotonTargets.All, photonView, GetType(), "Explode", new object[] { });
    }

    /// <summary>
    /// Make the fire ball explode.
    /// </summary>
    private void Explode()
    {
        if (hitBox.IsActive) hitBox.Desactivate();
        animator.SetTrigger(explosion_Hash);
    }

    /// <summary>
    /// Make the trajectory of the fire ball.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MakeTrajectory()
    {
        transform.position += transform.right * .5f;

        float _initialLifetime = lifetime;

        while (true)
        {
            if (lifetime < (_initialLifetime / 3)) speed *= .99f;

            transform.position += transform.right * speed * Time.deltaTime;

            yield return null;

            lifetime -= Time.deltaTime;

            if (lifetime < 0) break;
        }

        CallExplode();
    }
	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        if (!hitBox)
        {
            hitBox = GetComponent<TDS_HitBox>();
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider collider)
    {
        if (!photonView.isMine || isDestroying) return;

        isDestroying = true;
        AkSoundEngine.PostEvent("Play_FIREBALL_HIT", gameObject);

        StopAllCoroutines();
        Invoke("CallExplode", .001f);
    }

    // Use this for initialization
    private void Start()
    {
        if (!photonView.isMine) return;

        StartCoroutine(MakeTrajectory());
    }
	#endregion

	#endregion
}