using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageAnimation : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] Image image;

    [SerializeField] float fps = 10;
    [SerializeField] string spriteFolderPath = "Main Menu";

    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>(spriteFolderPath);
    }

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        Stop();
        StartCoroutine(AnimSequence());
    }

    public void Stop()
    {
        StopAllCoroutines();
        ShowFrame(0);
    }

    IEnumerator AnimSequence()
    {
        var delay = new WaitForSeconds(1f / fps);
        int index = 0;
        while (true)
        {
            if (index >= sprites.Length) index = 0;
            ShowFrame(index);
            index++;
            yield return delay;
        }
    }

    void ShowFrame(int index)
    {
        image.sprite = sprites[index];
    }
}

