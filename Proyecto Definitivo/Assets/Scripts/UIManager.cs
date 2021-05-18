using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject SettingsMenu;
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && SettingsMenu.activeSelf)
        {
            OnBackClicked();
        }
    }

    public void OnClickExit()
    {
        Application.Quit(0);
    }
    public void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnClickSettings()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }
    public void OnClickContinue()
    {

    }
    public void OnBackClicked()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }
}
