using UnityEditor;
using UnityEngine;

public class UnityCustomCaller : MonoBehaviour
{
    [MenuItem("Tools/CustomUnity/Change camera")]
    public static void CallManager()
    {
        if (Camera.main)
        {
            DestroyImmediate(Camera.main.gameObject);
        }
        Instantiate(Resources.Load("CustomMainCamera"));
        Debug.Log("Camera custom is now on the scene.");
    }
}