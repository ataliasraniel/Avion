using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Mainmenu_Controller : MonoBehaviour
{
    [Header("Screens")]
    public GameObject gameButtons;
    public GameObject gameTitle;
    public GameObject settingsScreen;

    [Header("Painéis")]
    public GameObject graphics;
    public GameObject sound;
    public GameObject credits;
    public GameObject navPanel;
    UI_Animations_Controller uiAnimations;

    private void Start()
    {
        uiAnimations = FindObjectOfType<UI_Animations_Controller>();
        //reseta os estados do mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    #region controle das telas do menu
    //controle dos menus   
    public void OpenSettings()
    {
        //abre o menu de configurações
        gameButtons.SetActive(false);
        gameTitle.SetActive(false);
        navPanel.SetActive(false);
        settingsScreen.SetActive(true);
        graphics.SetActive(true);
        uiAnimations.Fade(1);

    }
    public void Back()
    {
        //volta para o menu
        gameButtons.SetActive(true);
        gameTitle.SetActive(true);
        settingsScreen.SetActive(false);
        navPanel.SetActive(true);
        uiAnimations.Fade(0);
    }
    public void Confirm()
    {
        //confirma as alterações
        Back();
        //TODO: fazer a função de salvar as alterações do jogador
    }
    #endregion

    //chamar as cenas e sair do jogo
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void Quit()
    {
        Application.Quit();
    }
    #region navegação do menu de configurações
    public void GraphicsPanel()
    {
        graphics.SetActive(true);
        sound.SetActive(false);
        credits.SetActive(false);
        uiAnimations.PanelsFade(1);
    }
    public void SoundPanel()
    {
        graphics.SetActive(false);
        sound.SetActive(true);
        credits.SetActive(false);
        uiAnimations.PanelsFade(1);
    }
    public void CreditsPanel()
    {
        graphics.SetActive(false);
        sound.SetActive(false);
        credits.SetActive(true);
        uiAnimations.PanelsFade(1);
    }

    #endregion
}
