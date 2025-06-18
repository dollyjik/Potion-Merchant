using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject button1, button2, button3, image1, creditsPanel, escapePanel;
    public Image image;
    public bool isEscapePanelOpen = false;
    
    public void PlayGame()
    {
        Time.timeScale = 1;
        image.color = Color.black;
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        image1.SetActive(false);
        StartCoroutine(ChangeScene());
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(.1f);
        
        SceneManager.LoadScene(1);
    }

    public void OpenCredits()
    {
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        image1.SetActive(false);
        creditsPanel.SetActive(true);
    }
    
    public void CloseCredits()
    {
        button1.SetActive(true);
        button2.SetActive(true);
        button3.SetActive(true);
        image1.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void CloseEscape()
    {
        escapePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            Debug.Log("Editördeki Play modu durduruldu.");
        #else 
            Application.Quit();
            Debug.Log("Oyundan çıkılıyor...");
        #endif
    }
}
