using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TipsPopController : MonoBehaviour
{
    public static TipsPopController Instance;

    public Transform popTransform;

    public GameObject popPrefab;

    public int maxPop;
    public float popupDuration;
    public float popupTime;

    readonly Queue<GameObject> popQueue = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        popTransform = transform;
    }


    /// <summary>
    /// 传入需要弹窗的文本，实现弹窗效果；
    /// </summary>
    /// <param name="text"></param>
    public void SetTips(string text)
    {
        GameObject newTips = Instantiate(popPrefab, popTransform);

        newTips.GetComponentInChildren<TMP_Text>().text = text;

        popQueue.Enqueue(newTips);

        if(popQueue.Count > maxPop)
        {
            Destroy(popQueue.Dequeue());
        }

        StartCoroutine(SetPopFade(newTips));

    }

    IEnumerator SetPopFade(GameObject tips)
    {
        yield return new WaitForSeconds(popupDuration);

        if (tips == null)
            yield break;

        CanvasGroup canvas = tips.GetComponent<CanvasGroup>();

        float alpha = canvas.alpha;

        Vector3 position = tips.transform.position;

        Vector3 finPosition = new (position.x, position.y + 80, position.z);

        for(float t = 0; t < popupTime; t += Time.deltaTime)
        {
            if (tips == null)
                yield break;

            position.y += Time.deltaTime * 80f;

            tips.transform.position = position;

            canvas.alpha = Mathf.Lerp(alpha, 0, t / popupTime);

            yield return null;
        }

        Destroy(tips);

    }

}
