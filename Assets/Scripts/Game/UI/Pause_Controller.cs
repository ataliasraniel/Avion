using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause_Controller : MonoBehaviour
{
    //controla os menus de pausa e estados do jogo
    private bool paused = false;
    [Header("Itens do Pause")]
    public GameObject pausePanel;
    public GameObject pauseTitle;
    public Canvas pauseCanvas;

    private void Start()
    {
        pauseCanvas.enabled = false;
        //faz com que o cursor suma enqanto estiver no jogo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                paused = false;
            }
            else
            {
                paused = true;
            }


        }
        ManagePause();
    }
    private void ManagePause()
    {
        if (paused)
        {
            //se pausar, ativa o canvas e os botões do pause e pausa o tempo

            pauseCanvas.enabled = true;
            pausePanel.SetActive(true);
            pauseTitle.SetActive(true);
            Time.timeScale = 0.1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
        else
        {
            //se não estiver pausado, fecha os botões e o canvas e volta ao tempo normal
            pauseCanvas.enabled = false;
            pausePanel.SetActive(false);
            pauseTitle.SetActive(false);
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #region interações dos botões
    public void Return()
    {
        paused = false;
    }
    public void Settings()
    {
        //TODO: implementar a tela de ajustes no jogo
    }
    public void Quit()
    {
        paused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Main Menu");
    }
    #endregion
}
