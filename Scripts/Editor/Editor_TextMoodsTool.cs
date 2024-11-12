using hrspecian.timelinesubtitle.runtime;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace hrspecian.timelinesubtitle.editor
{
    public class Editor_TextMoodsTool : EditorWindow
    {
        private const string _example = "Sample Text";
        private const string FOLDER_PATH = "Assets/TextMoods";

        private TMTenums actionType;
        private int _actionType;

        private TextMood currentTextMood;
        private TextMood tempTextMood;

        private string _name;

        private float _amplitude;
        private float _magnitude;
        private float _lerpTime;
        private AnimationCurve _lerpAlphaCurve;


        [MenuItem("Tools/Timeline Subtitle/Text Mood Editor")]
        public static Editor_TextMoodsTool ShowWindow()
        {
            Editor_TextMoodsTool tool = EditorWindow.GetWindow<Editor_TextMoodsTool>(
                    title: "Text Mood Editor",
                    focus: true
                    );

            tool.minSize = new Vector2(400f, 400f);
            tool.Show();

            return tool;
        }

        private void Awake() => this.Initialize();
        private void Initialize() { }

        private void OnGUI()
        {
            ActionTypeTools();

            GUILayout.Space(30f);

            ShowExample();
        }

        #region Tools GUI
        private void ActionTypeTools()
        {
            string[] temp = Enum.GetNames(typeof(TMTenums));
            for (int i = 0; i < temp.Length; i++) { temp[i] = SplitCamelCase(temp[i]); }

            _actionType = EditorGUILayout.Popup("Action", _actionType, temp);
            actionType = (TMTenums)_actionType;

            GUILayout.Space(10);

            switch (actionType)
            {
                case TMTenums.CreateTextMood:
                    CreateMoodTool();
                    break;
                case TMTenums.ModifyTextMood:
                    ModifyMoodTool();
                    break;
            }
        }

        private void CreateMoodTool()
        {
            if (currentTextMood != null) { ResetValuesFields(); }

            _name = EditorGUILayout.TextField("TextMood Name: ", _name);

            ShowValuesFields();

            GUILayout.Space(10f);

            if (GUILayout.Button("Save Asset"))
            {
                SaveTextMoodAsset();
            }
        }

        private void ModifyMoodTool()
        {
            currentTextMood = (TextMood)EditorGUILayout.ObjectField(currentTextMood, typeof(TextMood), true);

            if (currentTextMood == null) { return; }

            ShowValuesFields();

            if (currentTextMood != tempTextMood)
                LoadTextMoodAsset();

            GUILayout.Space(10f);

            if (GUILayout.Button("Reload Asset"))
            {
                LoadTextMoodAsset();
            }

            if (GUILayout.Button("Save Asset"))
            {
                SaveTextMoodAsset();
            }
        }

        private void ShowExample()
        {
            GUILayout.Label(_example);
        }

        #endregion

        #region Submodules GUI
        private void ShowValuesFields()
        {
            _amplitude = EditorGUILayout.FloatField("Amplitude:", _amplitude);
            _magnitude = EditorGUILayout.FloatField("Magnitude: ", _magnitude);
            _lerpTime = EditorGUILayout.FloatField("Lerp Time: ", _lerpTime);
            _lerpAlphaCurve = EditorGUILayout.CurveField("Lerp Alpha Curve: ", _lerpAlphaCurve);
        }

        #endregion

        #region Intern Actions
        private void ResetValuesFields()
        {
            _name = string.Empty;

            _amplitude = 0;
            _magnitude = 0;
            _lerpTime = 0;
            _lerpAlphaCurve = new AnimationCurve();

            currentTextMood = null;
            tempTextMood = null;
        }

        private void LoadTextMoodAsset()
        {
            tempTextMood = currentTextMood;

            _amplitude = currentTextMood.amplitude;
            _magnitude = currentTextMood.magnitude;
            _lerpTime = currentTextMood.lerpTime;
            _lerpAlphaCurve = currentTextMood.lerpAlphaCurve;
        }

        private void SaveTextMoodAsset()
        {

            if (currentTextMood != null)
            {
                SaveFile(AssetDatabase.GetAssetPath(currentTextMood));
            }
            else if (!string.IsNullOrEmpty(_name))
            {
                if (!FileAlreadyExists())
                    SaveFile($"{FOLDER_PATH}/{_name}.asset");
            }
        }

        private bool FileAlreadyExists()
        {
            if (!Directory.Exists(FOLDER_PATH))
                Directory.CreateDirectory(FOLDER_PATH);

            return File.Exists($"{FOLDER_PATH}/{_name}.asset");
        }

        private void SaveFile(string path)
        {
            TextMood mood = ScriptableObject.CreateInstance<TextMood>();

            mood.amplitude = _amplitude;
            mood.magnitude = _magnitude;
            mood.lerpTime = _lerpTime;
            mood.lerpAlphaCurve = _lerpAlphaCurve;

            AssetDatabase.CreateAsset(mood, path);

            ResetValuesFields();
        }

        private string SplitCamelCase(string input) =>
            System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();

        #endregion
    }

    [Serializable]
    public enum TMTenums
    {
        CreateTextMood,
        ModifyTextMood
    }
}