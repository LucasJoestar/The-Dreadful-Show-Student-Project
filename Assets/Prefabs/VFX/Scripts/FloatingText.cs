using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    #region Fields / Properties
    [SerializeField]
    float destroyTime = 1.5f;
    [SerializeField]
    float offset = 2;
    [SerializeField]
    Color textColor;
    TMP_Text text;
    [SerializeField]
    float randomizedOffset = .5f;
    Vector3 randomizePosition  {get {return new Vector3(randomizedOffset, 0, 0);}}
    #endregion

    #region Methodes
    #region Original Methodes
    void Init()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.color = textColor;

        Destroy(gameObject, destroyTime);

        transform.localPosition += Vector3.up * offset;
        transform.localPosition += new Vector3(Random.Range(-randomizePosition.x, randomizePosition.x),
                                               Random.Range(-randomizePosition.y, randomizePosition.y),
                                               Random.Range(-randomizePosition.z, randomizePosition.z));
    }
    #endregion

    #region Unity Methodes
    void Start()
    {
        Init();
    }
    #endregion
    #endregion
}
