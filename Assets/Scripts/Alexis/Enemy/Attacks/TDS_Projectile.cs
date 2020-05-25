using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_Projectile : TDS_Object 
{
    /* TDS_Projectile :
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
    /// Range of the projectile
    /// </summary>
    [SerializeField] private float range = 7;

    /// <summary>
    /// Speed of the projectile
    /// </summary>
    [SerializeField] private float speed = 5;

    /// <summary>
    /// Hit box of the projectile.
    /// </summary>
    [SerializeField] private TDS_HitBox hitBox = null;
    public TDS_HitBox HitBox { get { return hitBox;  } }

    [SerializeField] private AnimationCurve trajectory = new AnimationCurve();

    [SerializeField] private new BoxCollider collider = null;
    [SerializeField] private SpriteRenderer sprite = null;
    [SerializeField] private GameObject shadow = null;
    [SerializeField] private ParticleSystem feedbackFX = null;
    #endregion

    bool isDestroying = false;

    #region Methods

    #region Original Methods
    private IEnumerator ProjectileMovement(Vector3 _direction)
    {
        Vector3 _originalPosition = transform.position;
        Vector3 _destination = transform.position + (_direction * range);
        float _delta = 0;
        float _time = range / speed; 

        while(_delta <= _time)
        {
            transform.position = new Vector3(Mathf.Lerp(_originalPosition.x, _destination.x, _delta / _time), _originalPosition.y + trajectory.Evaluate(_delta / _time), _originalPosition.z);
            yield return null;
            _delta += Time.deltaTime;
        }
        PrepareDestruction(); 
    }

    /// <summary>
    /// Plays feedback for when the throwable gets destroyed.
    /// </summary>
    protected virtual void DestroyFeedback()
    {
        collider.enabled = false;
        sprite.enabled = false;
        if (shadow) shadow.SetActive(false);

        // Play magic poof

        if (!feedbackFX) return;

        feedbackFX.gameObject.SetActive(true);
    }

    /// <summary>
    /// Destroy the gameObject
    /// </summary>
    private void CallDestruction()
    {
        if (PhotonNetwork.isMasterClient)
        {
            hitBox.Desactivate();

            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "PrepareDestruction"), new object[] { });

            Invoke("Destroy", 2);
        }

        DestroyFeedback();
    }

    public void StartProjectileMovement(Vector3 _direction, float _range)
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "StartProjectileMovement"), new object[] { _direction.x, _direction.y, _direction.z, _range });
        }
        range = _range;
        StartCoroutine(ProjectileMovement(_direction));
    }

    public void StartProjectileMovement(float _xDirection, float _yDirection, float _zDirection, float _range)
    {
        StartProjectileMovement(new Vector3(_xDirection, _yDirection, _zDirection), _range);
    }

    private void PrepareDestruction()
    {
        if (isDestroying)
            return;

        isDestroying = true;
        StopAllCoroutines();
        Invoke("CallDestruction", .001f);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        if (!hitBox)
        {
            hitBox = GetComponent<TDS_HitBox>();
            if (!hitBox) Debug.LogWarning("HitBox is missing on Projectile !");
        }
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        PrepareDestruction(); 
    }
    #endregion

    #endregion
}
