using UnityEngine;
using System.Collections; 

[ExecuteAlways]
public class TDS_LightBehavior : MonoBehaviour
{
    #region Fields/Properties
    #region Light
    [Space]
    Light lightCustom;
    float lightInt;
    [SerializeField]
    float minInt = 3f;
    [SerializeField]
    float maxInt = 5f;

    [SerializeField]
    float changeLightRate = 1;

    private Coroutine lightCoroutine = null; 
    #endregion
    #endregion

    #region Methods   
    #region Light
    void ChangeLightIntensity()
    {
        if(lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null; 
        }
        lightCoroutine = StartCoroutine(UpdateLightIntensity()); 
    }

    IEnumerator UpdateLightIntensity()
    {
        lightInt = Random.Range(minInt, maxInt);
        float _originalIntensity = lightCustom.intensity;
        float _delta = 0;
        while (_delta < changeLightRate)
        {
            lightCustom.intensity = Mathf.Lerp(_originalIntensity, lightInt, _delta / changeLightRate); 
            yield return null;
            _delta += Time.deltaTime;
        }
        lightCoroutine = null;
        ChangeLightIntensity(); 
    }
    #endregion
    #endregion

    #region UnityMethods
    void Start()
    {
        lightCustom = GetComponent<Light>();
        ChangeLightIntensity(); 
    }

    #endregion
}
