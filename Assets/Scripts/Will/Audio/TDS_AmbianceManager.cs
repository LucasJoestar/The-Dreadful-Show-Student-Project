using UnityEngine;


public class TDS_AmbianceManager : MonoBehaviour
{
    #region Fields / Properties
    [SerializeField]
        BoxCollider colliderSound = null;
    [SerializeField]
        AudioSource[] soundAmbience = null;
    [SerializeField]
        Tags detectTag = null;
    #endregion

    #region Methods
    #region Original Methods   
    void Init()
    {
        if (colliderSound == null)
            colliderSound = GetComponent<BoxCollider>();
            colliderSound.isTrigger = true;
        if (soundAmbience == null || soundAmbience.Length == 0)
            soundAmbience = GetComponentsInChildren<AudioSource>();
    }
    #endregion

    #region Unity Methods
    void OnTriggerEnter(Collider _collider)
    {
        if (_collider.gameObject.HasTag(detectTag.ObjectTags))
        {
            foreach (AudioSource _sources in soundAmbience)
            {
                _sources.Play();
                _sources.loop = true;
            }
        }  
    }

    void OnTriggerExit(Collider _collider)
    {
        if (_collider.gameObject.HasTag(detectTag.ObjectTags))
        {
            foreach (AudioSource _sources in soundAmbience)
            {
                _sources.Stop();
            }
        }
    }

    void Start()
    {
        Init();
    }
    #endregion
    #endregion
}