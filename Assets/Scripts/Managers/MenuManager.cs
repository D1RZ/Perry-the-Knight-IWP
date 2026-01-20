using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject tintImage;

    [SerializeField] private GameObject SettingsPage;

    [SerializeField] private CanvasGroup buttonsParent;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");

        // If running in the Unity Editor
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // If running as a build
                Application.Quit();
        #endif
    }

    public void OpenSettingsPage()
    {
        buttonsParent.interactable = false;
        buttonsParent.blocksRaycasts = false;
        buttonsParent.alpha = 0;
        tintImage.SetActive(true);
        SettingsPage.SetActive(true);
    }

    public void CloseSettingsPage()
    {
        buttonsParent.interactable = true;
        buttonsParent.blocksRaycasts = true;
        buttonsParent.alpha = 1;
        tintImage.SetActive(false);
        SettingsPage.SetActive(false);
    }    

}