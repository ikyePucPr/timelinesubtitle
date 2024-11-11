using System.Collections;
using UnityEngine;
using TMPro;

public class WobblyText : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private WobblyTextObject currentText;
    [SerializeField] private bool isShowing = false;

    public void ShowTextByTimer(int textID)
    {
        currentText = DirectorsManager.Instance().GetTextByID(textID);
        textComponent.text = currentText.text;

        StopAllCoroutines();
        StartCoroutine(ShowTextByTimer());
    }

    private IEnumerator ShowTextByTimer()
    {
        isShowing = true;

        yield return LerpCanvasGroupCoroutine();

        yield return new WaitForSecondsRealtime(currentText.exposedTime);

        yield return LerpCanvasGroupCoroutine(false);

        yield return isShowing = false;
    }

    private IEnumerator LerpCanvasGroupCoroutine(bool show = true)
    {
        float t = 0;
        float lerpGoal = show ? 1 : 0;

        while (canvasGroup.alpha != lerpGoal)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, lerpGoal, currentText.textMood.lerpAlphaCurve.Evaluate((t += Time.deltaTime) / (currentText.textMood.lerpTime <= 0 ? 1 : currentText.textMood.lerpTime)));
            yield return null;
        }
        yield return null;
    }

    private void Update()
    {
        if (!isShowing)
            return;

        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; ++j)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * currentText.textMood.amplitude) * currentText.textMood.magnitude, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }

#if UNITY_EDITOR
    public void DEV_SetCurrent(WobblyTextObject WTobj)
    {
        currentText = WTobj;
        textComponent.text = currentText.text;
    }

    public void DEV_ShowText(bool show = true)
    {
        isShowing = show;
        StopAllCoroutines();
        canvasGroup.alpha = show ? 0 : 1;
        StartCoroutine(LerpCanvasGroupCoroutine(show));
    }

    public void DEV_ShowTextTimer()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0;
        StartCoroutine(ShowTextByTimer());
    }
#endif
}