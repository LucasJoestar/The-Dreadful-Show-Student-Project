using UnityEditor;
using UnityEngine;

public class UnityCustomCaller : MonoBehaviour
{
    [MenuItem("Tools/CustomUnity/Change camera/ClassicCam")]
    public static void CallManager()
    {
        if (Camera.main)
        {
            DestroyImmediate(Camera.main.gameObject);
        }
        Instantiate(Resources.Load("CustomMainCamera"));
        Debug.Log("Camera custom is now on the scene.");
    }

    [MenuItem("Tools/CustomUnity/Change camera/PostProcessCam")]
    public static void CallPostProcessCam()
    {
        if (Camera.main)
        {
            DestroyImmediate(Camera.main.gameObject);
        }
        Instantiate(Resources.Load("MainCameraPostPro"));
        Debug.Log("Camera custom PostPro is now on the scene.");
    }
}