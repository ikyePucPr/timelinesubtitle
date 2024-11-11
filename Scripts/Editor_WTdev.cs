using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WTdev))]
public class Editor_WTdev : Editor
{
    #region SerializedProperties
    SerializedProperty wobblyText;

    SerializedProperty actionTypes;

    SerializedProperty text;
    SerializedProperty exposedTime;
    SerializedProperty textMood;

    SerializedProperty moodName;
    SerializedProperty amplitude;
    SerializedProperty magnitude;
    SerializedProperty lerpTime;
    SerializedProperty curve;
    #endregion

    private bool validation = false;

    private void OnEnable()
    {
        wobblyText = serializedObject.FindProperty("wobblyText");

        actionTypes = serializedObject.FindProperty("actionTypes");

        text = serializedObject.FindProperty("text");
        exposedTime = serializedObject.FindProperty("exposedTime");
        textMood = serializedObject.FindProperty("textMood");

        moodName = serializedObject.FindProperty("moodName");
        amplitude = serializedObject.FindProperty("amplitude");
        magnitude = serializedObject.FindProperty("magnitude");
        lerpTime = serializedObject.FindProperty("lerpTime");
        curve = serializedObject.FindProperty("curve");
    }


    public override void OnInspectorGUI()
    {
        WTdev script = (WTdev)target;
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(wobblyText);

        EditorGUILayout.PropertyField(text);
        EditorGUILayout.PropertyField(exposedTime);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(actionTypes);

        EditorGUILayout.Space();
        switch (script.actionTypes)
        {
            case WTMenums.testing:
                EditorGUILayout.PropertyField(textMood);
                if (script.textMood != null)
                {
                    if (GUILayout.Button("Testar Mood existente"))
                        script.TestMood();
                }
                break;

            case WTMenums.modifying:
                EditorGUILayout.PropertyField(textMood);
                if (script.textMood != null)
                {
                    if (validation)
                    {
                        validation = false;
                        script.ImportMood();
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(amplitude);
                        EditorGUILayout.PropertyField(magnitude);
                        EditorGUILayout.PropertyField(lerpTime);
                        EditorGUILayout.PropertyField(curve);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("Exibir Texto"))
                            script.StartText();
                        if (GUILayout.Button("Esconder Texto"))
                            script.EndText();

                        EditorGUILayout.Space();
                        if (GUILayout.Button("Salvar"))
                            script.SaveCurrent();
                    }
                    validation = (script.tempMood != script.textMood);
                }
                break;

            case WTMenums.creating:
                if (validation)
                {
                    validation = false;
                    script.ClearTemp();
                }

                EditorGUILayout.PropertyField(moodName);
                EditorGUILayout.PropertyField(amplitude);
                EditorGUILayout.PropertyField(magnitude);
                EditorGUILayout.PropertyField(lerpTime);
                EditorGUILayout.PropertyField(curve);

                EditorGUILayout.Space();
                if (GUILayout.Button("Exibir Texto"))
                    script.StartText();
                if (GUILayout.Button("Esconder Texto"))
                    script.EndText();
                if (GUILayout.Button("Testar temporario"))
                    script.TestMood();

                EditorGUILayout.Space();
                if (!string.IsNullOrEmpty(script.moodName))
                {
                    if (GUILayout.Button("Salvar Novo Mood"))
                        script.SaveNew();
                }
                validation = (script.textMood != null);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
