using System.Collections;
using UnityEngine;
using Photon; 

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

    #region Methods

    #region Original Methods
    private IEnumerator ProjectileMovement(Vector3 _direction)
    {
        if (!PhotonNetwork.isMasterClient) yield break;
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

        TDS_SoundManager.Instance.PlayEffectSound(TDS_GameManager.AudioAsset.S_MagicPoof, audioSource);

        if (!feedbackFX) return;

        feedbackFX.gameObject.SetActive(true);
    }

    /// <summary>
    /// Destroy the gameObject
    /// </summary>
    private void CallDestruction()
    {
        if (!PhotonNetwork.isMasterClient) return;
        StopAllCoroutines(); 
        hitBox.Desactivate();

        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "DestroyFeedback"), new object[] { });

        DestroyFeedback();
        Invoke("Destroy", 2);
    }

    public void StartProjectileMovement(Vector3 _direction, float _range)
    {
        if (!PhotonNetwork.isMasterClient) return;
        range = _range;
        StartCoroutine(ProjectileMovement(_direction));
    }

    private void PrepareDestruction()
    {
        if (!PhotonNetwork.isMasterClient) return;
        CancelInvoke("CallDestruction");
        StopAllCoroutines();
        Invoke("CallDestruction", .001f);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

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
        if (!PhotonNetwork.isMasterClient) return;
        PrepareDestruction(); 
    }
    #endregion

    #endregion
}
