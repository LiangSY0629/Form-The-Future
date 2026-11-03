using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    public static SceneFade Instance;

    public CanvasGroup canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        canvas = GetComponentInChildren<CanvasGroup>();
    }

    /// <summary>
    /// 传入透明度的值，将当前画布的透明度过渡到传入的透明度；
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns></returns>
    async Task Fade(float alpha, float fadeTime)
    {
        float a = canvas.alpha;

        for(float t=0;t<fadeTime; t+= Time.deltaTime)
        {
            canvas.alpha = Mathf.Lerp(a, alpha, t * 1.1f / fadeTime);
            await Task.Yield();
        }

        canvas.alpha = alpha;

    }


    /// <summary>
    /// 淡入；传入颜色呈现对应颜色的淡入效果；
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public async Task FadeIn(Color color, float t)
    {
        canvas.GetComponent<Image>().color = color;
        await Fade(1, t);
    }


    /// <summary>
    /// 淡出；传入对应颜色呈现对应颜色的淡出效果；
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public async Task FadeOut(Color color, float t)
    {
        canvas.GetComponent<Image>().color = color;
        await Fade(0, t);
    }


}
