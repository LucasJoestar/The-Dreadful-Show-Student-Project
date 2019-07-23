using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 

[RequireComponent(typeof(PhotonView), typeof(BoxCollider), typeof(Rigidbody))]
public class TDS_Projectile : PunBehaviour 
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

    #endregion

    #region Methods

    #region Original Methods
    private IEnumerator ProjectileMovement(Vector3 _direction)
    {
        if (!PhotonNetwork.isMasterClient) yield break; 
        Vector3 _destination = transform.position + (_direction * range); 

        while(Vector3.Distance(transform.position, _destination) > .1f)
        {
            transform.position += _direction * speed * Time.deltaTime;
            yield return null; 
        }
        CallDestruction();
    }

    /// <summary>
    /// Destroy the gameObject
    /// </summary>
    private void CallDestruction()
    {
        if (!PhotonNetwork.isMasterClient) return;
        hitBox.Desactivate();
        TDS_VFXManager.Instance.InstanciateParticleSystemByName("Projectile_PoufMagique", transform.position); 
        PhotonNetwork.Destroy(gameObject);
    }

    public void StartProjectileMovement(Vector3 _direction, float _range)
    {
        if (!PhotonNetwork.isMasterClient) return;
        range = _range;
        StartCoroutine(ProjectileMovement(_direction));
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!hitBox)
        {
            hitBox = GetComponent<TDS_HitBox>();
            if (!hitBox) Debug.LogWarning("HitBox is missing on Projectile !");
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider collider)
    {
        if (!PhotonNetwork.isMasterClient) return;

        StopAllCoroutines();
        Invoke("CallDestruction", .001f);
    }
    #endregion

    #endregion
}
