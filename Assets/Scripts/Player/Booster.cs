﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Booster : MonoBehaviour
{
  ///<summary>
  /// este script dará conta do booster
  ///<summary>

  public float cooldown = 1;
  public float boostTime = 1;
  public float boostPower = 600;
  public bool canBoost = true;
  public bool isBoosting = false;
  private Airplane _airplane;



  [Header("FX")]
  public string boostSFXname;
  public GameObject boostParticleFX;
  public Transform[] thursters;
  private bool boostCamera;
  private FlightCameraController _flightcamera;

  private ControllerManager _controllerManager;

  public float teste;
  private void Start()
  {

    _airplane = GetComponent<Airplane>();
    _flightcamera = FindObjectOfType<FlightCameraController>();
    _controllerManager = ControllerManager.instance;
    Gameui_Manager.instance.turboSliderCounter.maxValue = teste;
  }

  private void Update()
  {
    if (_controllerManager.inputActions.Game.Boost.triggered && canBoost)
    {
      StartCoroutine(Boost());
    }
    TurboUI();
  }

  IEnumerator Boost()
  {
    //ao usar, faz com que o avião aumente a velocidade, determinada
    //por uma variável
    canBoost = false;
    AudioManager.instance.Play(boostSFXname);
    TurboParticle();
    FlightCameraShakeManager.instance.ShakeLow();
    //UpdateFOV();
    _flightcamera.BoostFOV();
    //faz com que a câmera se afaste um pouco para dar a impressão de velocidade
    var thurstNormal = _airplane.thrust; //pega o número normal do thrust do avião
    _airplane.thrust += boostPower;
    yield return new WaitForSeconds(boostTime);
    //ResetFOV();
    _flightcamera.ResetFOV();
    _airplane.thrust = thurstNormal;
    yield return new WaitForSeconds(cooldown);
    canBoost = true;
  }
  void TurboUI()
  {
    //TODO: implementar no slider o medidor real do boost
    if (!canBoost)
    {
      teste -= Time.deltaTime * 0.5f;
      Gameui_Manager.instance.TurboOMetter(teste);
    }


  }
  void TurboParticle()
  {
    for (int i = 0; i < thursters.Length; i++)
    {
      print(i);
      var clone = Instantiate(boostParticleFX, thursters[i].position,
  thursters[i].rotation, thursters[i].transform);
    }

  }


}
