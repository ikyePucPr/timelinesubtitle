using System;
using UnityEditor;
using UnityEngine;

public class WTdev : MonoBehaviour
{
#if UNITY_EDITOR
    private const string FOLDER_PATH = "Assets/_Project/Scripts/Data/TextMoods/";

    public WobblyText wobblyText;

    public WTMenums actionTypes;
    public string text;
    public float exposedTime;
    public TextWobblyMoods textMood;

    public TextWobblyMoods tempMood;

    public string moodName;
    public float amplitude;
    public float magnitude;
    public float lerpTime;
    public AnimationCurve curve;


    public void ImportMood()
    {
        tempMood = textMood;

        amplitude = tempMood.amplitude;
        magnitude = tempMood.magnitude;
        lerpTime = tempMood.lerpTime;
        curve = tempMood.lerpAlphaCurve;
    }

    public void TestMood()
    {
        ImportMood();
        wobblyText.DEV_ShowTextTimer();
    }

    public void StartText() => wobblyText.DEV_ShowText();

    public void EndText() => wobblyText.DEV_ShowText(false);

    public void SaveNew()
    {
        string tempPath = $"{FOLDER_PATH}{moodName}.asset";

        SaveFile(tempPath);
    }

    public void SaveCurrent()
    {
        SaveFile(AssetDatabase.GetAssetPath(textMood));
    }

    private void SaveFile(string path)
    {
        TextWobblyMoods temp = ScriptableObject.CreateInstance<TextWobblyMoods>();

        temp.amplitude = amplitude;
        temp.magnitude = magnitude;
        temp.lerpTime = lerpTime;
        temp.lerpAlphaCurve = curve;

        AssetDatabase.CreateAsset(temp, path);
    }

    public void ClearTemp()
    {
        textMood = null;
        tempMood = null;

        moodName = null;
        amplitude = 0;
        magnitude = 0;
        lerpTime = 0;
        curve = null;
    }

    private void Update()
    {
        wobblyText.DEV_SetCurrent(
            new WobblyTextObject
            {
                text = text,
                exposedTime = exposedTime,
                textMood = new TextWobblyMoods
                {
                    amplitude = amplitude,
                    magnitude = magnitude,
                    lerpTime = lerpTime,
                    lerpAlphaCurve = curve
                }
            });
    }
#endif
}

[Serializable]
public enum WTMenums
{
    testing,
    creating,
    modifying
}