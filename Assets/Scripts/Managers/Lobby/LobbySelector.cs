using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Unity.Cinemachine;

public class LobbySelector : MonoBehaviour
{
  [Header("Airplanes")]
  [Tooltip("Os prefabs visuais (views) que ficarão no Lobby")]
  public GameObject[] airplanePrefabs;
  public float spacing = 15f;
  public Vector3 displayRotation = new Vector3(0, -45, 0);

  [Header("Camera Control")]
  public CinemachineCamera cinemachineCamera;
  [Tooltip("A Câmera nativa do Lobby para ser desativada manualmente")]
  public Camera lobbyCameraNative;
  public Vector3 cameraOffset = new Vector3(0, 5, -10);

  [Header("UI")]
  [Tooltip("O GameObject inteiro do Canvas desse Menu para ser desligado após a escolha")]
  public GameObject selectorCanvas;

  [Header("Input")]
  public PlayerInput playerInput;
  public float inputCooldown = 0.3f;
  private float nextInputTime = 0f;

  private int currentIndex = 0;
  private Vector3 targetCameraPos;
  private List<GameObject> displayPlanes = new List<GameObject>();
  private bool hasSelected = false;
  public MouseController mouseController;
  private void Start()
  {
    // Câmeras agora são buscadas na cena ativa inteira, pois esse script nasce de um Prefab
    if (cinemachineCamera == null)
      cinemachineCamera = Object.FindFirstObjectByType<CinemachineCamera>();

    if (lobbyCameraNative == null)
      lobbyCameraNative = Camera.main;

    if (playerInput == null)
      playerInput = GetComponent<PlayerInput>();

    // Força a Unity a ESCUTAR todos os dispositivos e trocar de Scheme automaticamente 
    if (playerInput != null)
    {
      playerInput.neverAutoSwitchControlSchemes = false;

      // Inscreve os eventos de Input pelas Actions do PlayerInput
      playerInput.actions["Move"].performed += OnMoveInput;
      playerInput.actions["ShootPrimary"].performed += OnSelectInput;
    }

    // DEBUG: Informações dos controles conectados
    Debug.Log("<color=yellow>[DEBUG CONTROLLERS]</color> --- INICIA VERIFICAÇÃO ---");
    var allGamepads = Gamepad.all;
    Debug.Log($"<color=yellow>[DEBUG]</color> Total de Gamepads detectados: {allGamepads.Count}");

    for (int i = 0; i < allGamepads.Count; i++)
    {
      Debug.Log($"<color=cyan>[DEBUG]</color> Gamepad {i}: {allGamepads[i].displayName} | ID: {allGamepads[i].deviceId} | Ativo: {allGamepads[i].wasUpdatedThisFrame}");
    }

    if (allGamepads.Count == 0)
    {
      Debug.LogWarning("<color=red>[DEBUG]</color> Nenhum Gamepad detectado pelo Input System no Start do Lobby.");
    }

    SpawnDisplayPlanes();
    UpdateCameraTarget(true);
    mouseController = FindFirstObjectByType<MouseController>();

  }

  // Tudo isso foi apagado para usar Input direto e hardcoded.





  private void OnMoveInput(InputAction.CallbackContext context)
  {
    if (Time.time < nextInputTime) return;

    Vector2 inputDir = context.ReadValue<Vector2>();

    if (inputDir.x > 0.8f)
    {
      MoveRight();
      nextInputTime = Time.time + inputCooldown;
    }
    else if (inputDir.x < -0.8f)
    {
      MoveLeft();
      nextInputTime = Time.time + inputCooldown;
    }
  }

  private void OnSelectInput(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      SelectAirplane();
    }
  }

  private void Update()
  {
    // O Update agora fica vazio de lógica de input pois usamos eventos setados no Start
  }

  private void ExecuteMoveLeft()
  {
    MoveLeft();
    nextInputTime = Time.time + inputCooldown;
  }

  private void ExecuteMoveRight()
  {
    MoveRight();
    nextInputTime = Time.time + inputCooldown;
  }


  private void SpawnDisplayPlanes()
  {
    for (int i = 0; i < airplanePrefabs.Length; i++)
    {
      if (airplanePrefabs[i] == null) continue;

      Vector3 spawnPos = transform.position + Vector3.right * (i * spacing);
      GameObject plane = Instantiate(airplanePrefabs[i], spawnPos, Quaternion.identity);
      plane.transform.SetParent(this.transform);
      plane.transform.localEulerAngles = displayRotation;

      // Disable physics so the display planes don't fall or move


      // Disable unnecessary components if needed, or put them in a display layer
      MonoBehaviour[] scripts = plane.GetComponentsInChildren<MonoBehaviour>();
      foreach (var script in scripts)
      {
        // Disable everything except transforms/renderers generally, or keep it simple
        // We'll leave it simple; mostly just stopping physics is enough. 
      }

      displayPlanes.Add(plane);
    }
  }

  public void MoveLeft()
  {
    if (currentIndex > 0)
    {
      currentIndex--;
      UpdateCameraTarget();
      print("index: " + currentIndex);
    }
  }

  public void MoveRight()
  {
    if (currentIndex < airplanePrefabs.Length - 1)
    {
      currentIndex++;
      UpdateCameraTarget();
    }
  }

  private void UpdateCameraTarget(bool instant = false)
  {
    if (displayPlanes.Count > 0 && cinemachineCamera != null)
    {
      Transform targetPlane = displayPlanes[currentIndex].transform;
      cinemachineCamera.Follow = targetPlane;
      cinemachineCamera.LookAt = targetPlane;

      // Tells Cinemachine to instantly warp instead of smoothing/damping to the first airplane
      if (instant)
      {
        cinemachineCamera.PreviousStateIsValid = false;
      }
    }
  }


  public void SelectAirplane()
  {
    if (hasSelected) return; // Previne múltiplos disparos simulâneos
    if (displayPlanes.Count == 0) return;

    AirplaneView viewData = displayPlanes[currentIndex].GetComponent<AirplaneView>();

    if (viewData != null && viewData.airplaneData != null && viewData.airplaneData.airplanePrefab != null)
    {
      hasSelected = true;

      // Registra o player e seus dispositivos na sessão persistente
      if (PlayersSession.Instance != null && playerInput != null)
      {
        InputDevice[] devices = playerInput.user.pairedDevices.ToArray();
        PlayersSession.Instance.AddPlayer(
            playerInput.playerIndex,
            devices,
            playerInput,
            viewData.airplaneData.airplanePrefab,
            viewData.airplaneData.airplaneName
        );
      }


      // A seleção foi feita. Limpa a vitrine.
      CleanUpLobby();
    }
    else
    {
      Debug.LogError("AirplaneView, AirplaneData ou o Prefab jogável estão faltando no visual selecionado!");
    }
  }

  void LateUpdate()
  {
    lobbyCameraNative.transform.position = new Vector3(0, 5, -20);
  }

  private void CleanUpLobby()
  {
    if (selectorCanvas != null)
    {
      selectorCanvas.SetActive(false);
    }

    foreach (GameObject plane in displayPlanes)
    {
      if (plane != null)
        plane.SetActive(false);
    }


    if (cinemachineCamera != null) { Destroy(cinemachineCamera.gameObject); }

    if (lobbyCameraNative != null)
    {
      // 1. Desabilitar explicitamente o CinemachineBrain para liberar o transform da Unity Camera.
      // O Brain trava o transform e só o libera se for desativado.
      var cinemachineBrain = lobbyCameraNative.GetComponent("CinemachineBrain") as MonoBehaviour;

      if (cinemachineBrain != null)
      {
        cinemachineBrain.enabled = false;
        Debug.Log("<color=yellow>[Lobby]</color> CinemachineBrain DESATIVADO.");
      }

      // 2. Forçar a posição e rotação manuais agora que o Brain está OFF.
      // cinemachineCamera.transform.position = new Vector3(0, 5, -20);
      // cinemachineCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
      var transposer = cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineTransposer;
      if (transposer != null)
        transposer.m_FollowOffset = new Vector3(0, 5, -20);

      Debug.Log("<color=green>[Lobby]</color> NEW CAMERA POS FORCED: " + lobbyCameraNative.transform.position);
    }

    // Limpar inscrições de eventos para evitar que chamadas de input tentem acessar este script destruído
    if (playerInput != null)
    {
      playerInput.actions["Move"].performed -= OnMoveInput;
      playerInput.actions["ShootPrimary"].performed -= OnSelectInput;
    }

    // Aniquila os visuais "falsos" do Lobby que eram filhos do Menu
    foreach (Transform child in transform)
    {
      Destroy(child.gameObject);
    }

    // O jogador selecionou e o avião já nasceu! 
    // Destruímos EXCLUSIVAMENTE o Script "LobbySelector", mas preservamos o GameObject vivo!
    // Assim, o componente PlayerInput original sobrevive perfeitamente para conduzir a nave.
    Destroy(this);
  }
}
