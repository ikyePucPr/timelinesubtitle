using hrspecian.timelinesubtitle.runtime;
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace hrspecian.timelinesubtitle.editor
{
    public class Editor_TextMoodsTool : EditorWindow
    {
        private const string FOLDER_PATH = "Assets/TextMoods";
        private const string EXAMPLE_PATH = "Packages/com.hrspecian.timelinesubtitle/Assets/_ExampleTool.prefab";

        private TMTenums actionType;
        private int _actionType;

        private TextMood currentTextMood;
        private TextMood tempTextMood;

        private string _name;
        private float _amplitude;
        private float _magnitude;

        /// TESTING
        enum SupportedAspects
        {
            Aspect4by3 = 1,
            Aspect5by4 = 2,
            Aspect16by10 = 3,
            Aspect16by9 = 4
        }

        Camera _cam = null;
        RenderTexture _rt;
        Texture2D _tex2d;
        Scene _scene;

        private TextMeshPro _textMeshRef;

        //SupportedAspects _aspectChoiceIdx = SupportedAspects.Aspect16by10;
        float _curAspect;

        float _worldScreenHeight = 5;
        int _renderTextureHeight = 1080;


        [MenuItem("Tools/Timeline Subtitle/Text Mood Editor")]
        public static Editor_TextMoodsTool ShowWindow()
        {
            Editor_TextMoodsTool tool = EditorWindow.GetWindow<Editor_TextMoodsTool>(
                    title: "Text Mood Editor",
                    focus: true
                    );

            tool.minSize = new Vector2(400f, 430f);
            tool.Show();

            return tool;
        }

        float ToFloat(SupportedAspects aspects)
        {
            switch (aspects)
            {
                case SupportedAspects.Aspect16by10:
                    return 16 / 10f;
                case SupportedAspects.Aspect16by9:
                    return 16 / 9f;
                case SupportedAspects.Aspect4by3:
                    return 4 / 3f;
                case SupportedAspects.Aspect5by4:
                    return 5 / 4f;
                default:
                    throw new ArgumentException();
            }
        }

        void DrawRefScene()
        {
            _rt = new RenderTexture(Mathf.RoundToInt(_curAspect * _renderTextureHeight), _renderTextureHeight, 16);
            _cam.targetTexture = _rt;
            _cam.Render();
            _tex2d = new Texture2D(_rt.width, _rt.height, TextureFormat.RGBA32, false);
            _tex2d.Apply(true);
            Graphics.CopyTexture(_rt, _tex2d);
        }

        Vector2 GetGUIPreviewSize()
        {
            Vector2 camSizeWorld = new Vector2(_worldScreenHeight * _curAspect, _worldScreenHeight);
            float scaleFactor = EditorGUIUtility.currentViewWidth / camSizeWorld.x;
            return new Vector2(EditorGUIUtility.currentViewWidth, scaleFactor * camSizeWorld.y);
        }

        private void OnEnable()
        {
            void OpenSceneDelay()
            {
                EditorApplication.delayCall -= OpenSceneDelay;
                DrawRefScene();
            }

            //_aspectChoiceIdx = SupportedAspects.Aspect16by10;

            _scene = EditorSceneManager.NewPreviewScene();

            PrefabUtility.LoadPrefabContentsIntoPreviewScene(EXAMPLE_PATH, _scene);
            _cam = _scene.GetRootGameObjects()[0].GetComponentInChildren<Camera>();

            _curAspect = ToFloat(SupportedAspects.Aspect16by10);
            _cam.aspect = _curAspect;
            _cam.orthographicSize = _worldScreenHeight;

            _cam.cameraType = CameraType.Preview;
            _cam.scene = _scene;

            _textMeshRef = _scene.GetRootGameObjects()[0].GetComponentInChildren<TextMeshPro>();
            
            EditorApplication.delayCall += OpenSceneDelay;
        }

        private void OnDisable()
        {
            EditorSceneManager.ClosePreviewScene(_scene);
        }

        //void OnCamSettingChange()
        //{
        //    _curAspect = ToFloat(_aspectChoiceIdx);
        //    _cam.aspect = _curAspect;
        //    _cam.orthographicSize = _worldScreenHeight;
        //    DrawRefScene();
        //}

        //class GUIControlStates
        //{
        //    public bool foldout = false;
        //};

        //GUIControlStates _guiStates = new GUIControlStates();

        private void ShowExample()
        {
            DrawRefScene();
            if (_tex2d != null)
            {
                Vector2 sz = GetGUIPreviewSize();
                Rect r = EditorGUILayout.GetControlRect(false,
                    GUILayout.Height(sz.y),
                    GUILayout.ExpandHeight(false));
                EditorGUI.DrawPreviewTexture(r, _tex2d);
            }
        }

        private void Update()
        {
            Repaint();

            if (_textMeshRef == null) { return; }

            _textMeshRef.ForceMeshUpdate();
            var textInfo = _textMeshRef.textInfo;

            int fontSize = Mathf.FloorToInt(_textMeshRef.fontSize);

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                var charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible) continue;

                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; ++j)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] =
                        orig + new Vector3(0, Mathf.Sin(
                            Time.time * 2f + orig.x * _amplitude) * _magnitude, 0);
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                _textMeshRef.UpdateGeometry(meshInfo.mesh, i);
            }
        }

        #region Tools GUI
        private void OnGUI()
        {
            ActionTypeTools();

            GUILayout.Space(30f);

            ShowExample();
        }

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

        #endregion

        #region Submodules GUI
        private void ShowValuesFields()
        {
            _amplitude = EditorGUILayout.FloatField("Amplitude:", _amplitude);
            _magnitude = EditorGUILayout.FloatField("Magnitude: ", _magnitude);
        }

        #endregion

        #region Intern Actions
        private void ResetValuesFields()
        {
            _name = string.Empty;

            _amplitude = 0;
            _magnitude = 0;

            currentTextMood = null;
            tempTextMood = null;
        }

        private void LoadTextMoodAsset()
        {
            tempTextMood = currentTextMood;

            _amplitude = currentTextMood.amplitude;
            _magnitude = currentTextMood.magnitude;
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