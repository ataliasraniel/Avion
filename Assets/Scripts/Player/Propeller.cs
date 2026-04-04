using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gerencia a hélice do avião: aceleração (RPM), rotação visual e som.
/// Agora suporta PlayerInput e integra-se com o novo FlightCameraController.
/// </summary>
public class Propeller : MonoBehaviour
{
  public enum PropellerRotationAxis { X, Z };

  [Header("Visual Propeller")]
  public PropellerRotationAxis propellerRotationAxis;
  public Transform propellerTransform;
  public float rotationDegrees = 32f;

  [Header("RPM Settings")]
  [Range(0, 900)] public float rpm = 12f;
  public float maxRpm = 360f;
  public float rpmChangeMultiplier = 50f; // Multiplicador para ganho/perda de RPM

  [Header("Physics & Thrust")]
  public float thrustMultiplier = 10f;
  public float reverseThrustMultiplier = 15f;

  [Header("Audio (Engine)")]
  public float pitchMultiplier = 0.5f;
  public float reversePitchMultiplier = 0.8f;
  public float pitchMin = 0.5f;
  public float pitchMax = 2.0f;
  private AudioSource _soundEmitter;

  [Header("Input & References")]
  public PlayerInput input;
  private InputAction _moveAction;
  private Airplane _airplane;
  private Rigidbody _rb;
  private Transform _seaTransform;
  private FlightCameraController _flightCamera;

  [Header("State Info")]
  public float speedKM;
  public float altitude;
  public float currentRpmPercentage; // 0 a 100%
  public bool isFast;
  public bool accelerate;
  public bool deaccelerate;

  private Gameui_Manager _gameui_Manager;


  private void Awake()
  {
    _airplane = GetComponent<Airplane>();
    _rb = GetComponent<Rigidbody>();
    _soundEmitter = GetComponentInChildren<AudioSource>();
    _flightCamera = GetComponent<FlightCameraController>();

    GameObject ocean = GameObject.FindGameObjectWithTag("Ocean");
    if (ocean != null) _seaTransform = ocean.transform;
  }

  private void Start()
  {
    // Se o PlayerInput já estiver no objeto (ou setado via Airplane), faz o setup
    if (input == null)
    {
      input = GetComponent<PlayerInput>();
    }

    if (input != null)
    {
      Setup(input);
    }

    // Inicializa UI
    _gameui_Manager = GetComponentInChildren<Gameui_Manager>();
    if (_gameui_Manager != null)
    {
      _gameui_Manager.SpeedCounterText(0);
    }
  }

  /// <summary>
  /// Configura o PlayerInput e as ações necessárias.
  /// </summary>
  public void Setup(PlayerInput playerInput)
  {
    input = playerInput;
    if (input != null)
    {
      _moveAction = input.actions["Move"];
    }
  }

  private void Update()
  {
    HandleInput();
    UpdateEngineState();
    UpdateVisuals();
    UpdatePhysics();
    UpdateUI();
  }

  private void HandleInput()
  {
    // Se o avião estiver sob controle da IA, ignoramos o input humano.
    if (_airplane != null && _airplane.isAiManaged) return;

    float moveY = 0;
    if (_moveAction != null)
    {
      // O eixo Y do Move (W/S) controla a aceleração
      moveY = _moveAction.ReadValue<Vector2>().y;
    }

    accelerate = moveY > 0.1f;
    deaccelerate = moveY < -0.1f;
  }

  private void UpdateEngineState()
  {
    // Controle de RPM baseado no input
    if (accelerate)
    {
      rpm += rpmChangeMultiplier * Time.deltaTime;
    }
    else if (deaccelerate)
    {
      rpm -= rpmChangeMultiplier * 1.5f * Time.deltaTime;
    }
    else
    {
      // Decaimento natural de RPM se não estiver acelerando nem freando
      rpm = Mathf.MoveTowards(rpm, 10f, 5f * Time.deltaTime);
    }

    rpm = Mathf.Clamp(rpm, 0, maxRpm);
    currentRpmPercentage = (rpm / maxRpm) * 100f;
    isFast = rpm >= (maxRpm * 0.5f);

    // Atualização do Som (Pitch)
    if (_soundEmitter != null)
    {
      float targetPitch = Mathf.Lerp(pitchMin, pitchMax, rpm / maxRpm);
      _soundEmitter.pitch = Mathf.MoveTowards(_soundEmitter.pitch, targetPitch, Time.deltaTime);
    }
  }

  private void UpdateVisuals()
  {
    if (propellerTransform == null) return;

    // Rotação da hélice multiplicada pelo RPM atual
    float rotationAmount = rotationDegrees * rpm * Time.deltaTime;

    if (propellerRotationAxis == PropellerRotationAxis.X)
    {
      propellerTransform.Rotate(new Vector3(rotationAmount, 0, 0), Space.Self);
    }
    else
    {
      propellerTransform.Rotate(new Vector3(0, 0, rotationAmount), Space.Self);
    }
  }

  private void UpdatePhysics()
  {
    if (_airplane == null) return;

    // Modifica o thrust do avião baseado no estado da hélice
    if (accelerate)
    {
      _airplane.thrust += thrustMultiplier * Time.deltaTime;
    }
    else if (deaccelerate)
    {
      _airplane.thrust -= reverseThrustMultiplier * Time.deltaTime;
    }

    // Cálculos de Telemetria
    if (_rb != null)
    {
      speedKM = Mathf.Round(_rb.linearVelocity.magnitude * 3.6f);
    }

    if (_seaTransform != null)
    {
      altitude = Mathf.Round(transform.position.y - _seaTransform.position.y);
    }
  }

  private void UpdateUI()
  {
    if (_gameui_Manager == null) return;

    _gameui_Manager.RpmCounterText(currentRpmPercentage);
    _gameui_Manager.SpeedCounterText(speedKM);
    _gameui_Manager.AltCounterText(altitude);
  }
}
