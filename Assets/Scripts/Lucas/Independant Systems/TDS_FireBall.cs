using System.Collections;
using UnityEngine;

public class TDS_FireBall : MonoBehaviour 
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

    /// <summary>
    /// Photon view of the fire ball.
    /// </summary>
    [SerializeField] private PhotonView photonView = null;
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Call the method to make the fire ball explode for all players.
    /// </summary>
    private void CallExplode()
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, GetType(), "Explode"), new object[] { });
    }

    /// <summary>
    /// Destroys the fire ball.
    /// </summary>
    public void Destroy() => Destroy(gameObject);

    /// <summary>
    /// Make the fire ball explode.
    /// </summary>
    private void Explode()
    {
        hitBox.Desactivate();
        animator.SetTrigger("Explosion");
    }

    /// <summary>
    /// Make the trajectory of the fire ball.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MakeTrajectory()
    {
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
    private void Awake()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
            if (!animator) Debug.LogWarning("Animator is missing on Fire Ball !");
        }
        if (!hitBox)
        {
            hitBox = GetComponent<TDS_HitBox>();
            if (!hitBox) Debug.LogWarning("HitBox is missing on Fire Ball !");
        }
        if (!photonView)
        {
            photonView = GetComponent<PhotonView>();
            if (!photonView) Debug.LogWarning("PhotonView is missing on Fire Ball !");
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider collider)
    {
        if (!photonView.isMine || isDestroying) return;

        isDestroying = true;

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
