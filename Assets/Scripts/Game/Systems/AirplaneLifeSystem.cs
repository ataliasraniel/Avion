using UnityEngine;

/// <summary>
/// Sistema de vida e danos do avião.
/// Gerencia danos por partes (asas, motor, cauda) e a queda do avião ao morrer.
/// </summary>
public class AirplaneLifeSystem : MonoBehaviour
{
  public enum AirplanePart { LeftWing, RightWing, Engine, Tail, Body }

  [Header("General Health")]
  public float maxHealth = 100f;
  public float currentHealth;
  public bool isDead;

  [Header("Parts Health")]
  public float wingHealth = 30f;
  public float engineHealth = 40f;
  public float tailHealth = 30f;

  private float _currentLWingHealth;
  private float _currentRWingHealth;
  private float _currentEngineHealth;
  private float _currentTailHealth;

  [Header("Death Settings")]
  public float destroyY = -10f; // Posição Y (mar) para destruição total
  public GameObject explosionFX;
  public Transform visualModel; // Referência ao modelo filho que vai girar
  public float deathSpinSpeed = 150f;
  private Vector3 _randomSpinAxis;

  [Header("References")]
  private Airplane _airplane;
  private Propeller _propeller;
  private Rigidbody _rb;
  private FlightCameraController _flightCamera;

  private void Awake()
  {
    _airplane = GetComponent<Airplane>();
    _propeller = GetComponent<Propeller>();
    _rb = GetComponent<Rigidbody>();
    _flightCamera = GetComponent<FlightCameraController>();

    _currentEngineHealth = engineHealth;
    _currentTailHealth = tailHealth;
    currentHealth = maxHealth;

    UpdateUI();
  }

  private void Start()
  {
    UpdateUI();
  }

  private void Update()
  {
    if (isDead)
    {
      // Efeito visual de queda: gira apenas o modelo separadamente da física
      if (visualModel != null)
      {
        visualModel.Rotate(_randomSpinAxis * deathSpinSpeed * Time.deltaTime);
      }

      // Se já morreu, verifica se chegou na água/chão
      if (transform.position.y <= destroyY)
      {
        Explode();
      }
      return;
    }

    // Checagem de morte por HP total
    if (currentHealth <= 0)
    {
      Die();
    }
  }

  /// <summary>
  /// Aplica dano a uma parte específica do avião.
  /// </summary>
  public void TakeDamage(float damage, AirplanePart part)
  {
    if (isDead) return;

    currentHealth -= damage;

    // Tremer a câmera se este for o avião do jogador
    if (_flightCamera != null)
    {
      _flightCamera.Shake(0.15f, 0.4f);
    }

    switch (part)
    {
      case AirplanePart.LeftWing:
        _currentLWingHealth -= damage;
        UpdateFlightPhysics();
        break;
      case AirplanePart.RightWing:
        _currentRWingHealth -= damage;
        UpdateFlightPhysics();
        break;
      case AirplanePart.Engine:
        _currentEngineHealth -= damage;
        UpdateEnginePerformance();
        break;
      case AirplanePart.Tail:
        _currentTailHealth -= damage;
        UpdateTailControl();
        break;
    }

    UpdateUI();
  }

  private void UpdateFlightPhysics()
  {
    if (_airplane != null)
    {
      if (_currentLWingHealth < wingHealth * 0.5f || _currentRWingHealth < wingHealth * 0.5f)
      {
        _airplane.forceMult *= 0.999f;
      }
    }
  }

  private void UpdateEnginePerformance()
  {
    if (_propeller != null && _currentEngineHealth < engineHealth * 0.5f)
    {
      _propeller.maxRpm *= 0.7f;
      _propeller.thrustMultiplier *= 0.7f;
    }
  }

  private void UpdateTailControl()
  {
    if (_airplane != null && _currentTailHealth < tailHealth * 0.5f)
    {
      _airplane.turnTorque = new Vector3(_airplane.turnTorque.x * 0.8f, _airplane.turnTorque.y * 0.8f, _airplane.turnTorque.z);
    }
  }

  private void Die()
  {
    if (isDead) return;
    isDead = true;

    print("Avião abatido! Caindo...");

    // Desabilita scripts de controle
    if (_airplane != null) _airplane.enabled = false;
    if (_propeller != null)
    {
      _propeller.rpm = 0;
      _propeller.enabled = false;
    }

    // Ativa comportamento de queda física mantendo a inercia absoluta
    if (_rb != null)
    {
      _rb.useGravity = true;

      // Remove todo o arrasto para o avião não parar "de nariz" (Pura Inércia)
      _rb.linearDamping = 0f;
      _rb.angularDamping = 0.05f;

      // Dá um último "empurrão" baseado na velocidade atual para garantir que ele voe pra frente
      Vector3 lastPush = transform.forward * _rb.linearVelocity.magnitude;
      _rb.AddForce(lastPush, ForceMode.Impulse);
    }

    // Define um eixo aleatório para o giro visual do modelo
    _randomSpinAxis = new Vector3(Random.Range(-1f, 1f), 0.2f, Random.Range(-1f, 1f)).normalized;

    // Se for IA, desabilita a inteligência
    AirplaneAI ai = GetComponent<AirplaneAI>();
    if (ai != null) ai.enabled = false;
  }

  private void Explode()
  {
    if (explosionFX != null)
    {
      Instantiate(explosionFX, transform.position, Quaternion.identity);
    }

    Destroy(gameObject);
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!isDead && collision.relativeVelocity.magnitude > 10f)
    {
      float collisionDamage = collision.relativeVelocity.magnitude * 2f;
      TakeDamage(collisionDamage, AirplanePart.Body);
    }
  }

  private void UpdateUI()
  {
    print("Atualizando UI de vida: " + currentHealth + " / " + maxHealth);
    Gameui_Manager gameui_Manager = GetComponentInChildren<Gameui_Manager>();
    if (gameui_Manager != null)
    {
      gameui_Manager.UpdateLife(currentHealth, maxHealth);
    }
  }
}