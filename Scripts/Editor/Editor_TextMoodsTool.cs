using System;
using UnityEditor;
using UnityEngine;

namespace hrspecian.timelinesubtitle.editor
{
    public class Editor_TextMoodsTool : EditorWindow
    {
        private const string FOLDER_PATH = "TextMoods/";
        private const string _example = "Sample Text";

        [SerializeField]
        public TMTenums actionType;

        private int _actionType;

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

        private void Awake()
        {
            this.Initialize();
        }

        private void Initialize()
        {

        }

        private void OnGUI()
        {
            string[] temp = Enum.GetNames(typeof(TMTenums));

            _actionType = EditorGUILayout.Popup("Action", _actionType, temp);

            actionType = (TMTenums)_actionType;

            switch (actionType)
            {
                case TMTenums.TestMood:

                    break;
                case TMTenums.CreatMood:

                    break;
                case TMTenums.modifyMood:

                    break;
            }
        }
    }

    [Serializable]
    public enum TMTenums
    {
        TestMood,
        CreatMood,
        modifyMood
    }
}
