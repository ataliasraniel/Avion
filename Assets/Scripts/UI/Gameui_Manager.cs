using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Gameui_Manager : MonoBehaviour
{
    public static Gameui_Manager instance;

    ///<<summary>>
    //este script cuidará de toda a UI do jogo: velocidade do avião
    ///rotação do motor, altura etc
    ///</summary>>
    public TextMeshProUGUI cameraText;
    public Transform popupPos;
    public GameObject popupPrefab;

    [Header("UI do avião")]
    public TextMeshProUGUI rpmCounter;
    public TextMeshProUGUI speedCounter;
    public TextMeshProUGUI altCounter;
    public TextMeshProUGUI bulletCounter;
    public Slider turboSliderCounter;
    private GameObject player;
    private Helice _helice;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _helice = player.GetComponentInChildren<Helice>();
        turboSliderCounter.value = turboSliderCounter.maxValue;
    }
    public void ShowPopup(string messege)
    {
        var clone = Instantiate(popupPrefab, popupPos);
        clone.GetComponent<Popup_Behavior>().messege = messege;

    }
    #region textos e contadores
    public void RpmCounterText(float text)
    {
        rpmCounter.text = "<b>RPM:</b> " + text.ToString("F0") + "%";
    }
    public void SpeedCounterText(float text)
    {
        speedCounter.text = "<b>SPEED:</b> " + text.ToString() + " Km/h";
    }
    public void AltCounterText(float text)
    {
        altCounter.text = "<b>ALT:</b> " + text.ToString() + " m";
    }
    public void TurboOMetter(float value)
    {
        turboSliderCounter.value = value;
    }
    public void BulletCounter(float value)
    {
        bulletCounter.text = "<b>BULLETS:</b> " + value.ToString();
    }
    #endregion
}