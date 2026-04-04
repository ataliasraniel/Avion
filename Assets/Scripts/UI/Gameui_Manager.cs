using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Gameui_Manager : MonoBehaviour
{

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
  public TextMeshProUGUI lifeCounter;
  public GameObject player;



  private void Start()
  {
    player = this.transform.parent.gameObject;
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
  public void UpdateLife(float value, float max)
  {
    if (lifeCounter == null) return;
    lifeCounter.text = "<b>LIFE:</b> " + value.ToString("F0") + " / " + max.ToString("F0");
  }
  #endregion
}