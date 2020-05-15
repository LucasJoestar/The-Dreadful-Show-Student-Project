using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SHACONTROLLER_OldMovieEffect))]
public class SHADEDITOR_OldMovieEffect : Editor
{
    #region F/P
    SerializedProperty oldFilmEffectAmount;
    SerializedProperty sepiaColor;
    SerializedProperty vignetteTexture;
    SerializedProperty vignetteAmount;
    SerializedProperty scratchesTexture;
    SerializedProperty scratchesYSpeed;
    SerializedProperty scratchesXSpeed;
    SerializedProperty dustTexture;
    SerializedProperty dustYSpeed;
    SerializedProperty dustXSpeed;
    #endregion

    #region Meths
    void OnEnable()
    {
        oldFilmEffectAmount = serializedObject.FindProperty("oldFilmEffectAmount");
        sepiaColor = serializedObject.FindProperty("sepiaColor");
        vignetteTexture = serializedObject.FindProperty("vignetteTexture");
        vignetteAmount = serializedObject.FindProperty("vignetteAmount");
        scratchesTexture = serializedObject.FindProperty("scratchesTexture");
        scratchesYSpeed = serializedObject.FindProperty("scratchesYSpeed");
        scratchesXSpeed = serializedObject.FindProperty("scratchesXSpeed");
        dustTexture = serializedObject.FindProperty("dustTexture");
        dustYSpeed = serializedObject.FindProperty("dustYSpeed");
        dustXSpeed = serializedObject.FindProperty("dustXSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(oldFilmEffectAmount);
        EditorGUILayout.PropertyField(sepiaColor);
        EditorGUILayout.PropertyField(vignetteTexture);
        EditorGUILayout.PropertyField(vignetteAmount);
        EditorGUILayout.PropertyField(scratchesTexture);
        EditorGUILayout.PropertyField(scratchesYSpeed);
        EditorGUILayout.PropertyField(scratchesXSpeed);
        EditorGUILayout.PropertyField(dustTexture);
        EditorGUILayout.PropertyField(dustYSpeed);
        EditorGUILayout.PropertyField(dustXSpeed);

        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}