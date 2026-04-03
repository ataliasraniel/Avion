using UnityEngine;

/// <summary>
/// Controla o avião de forma autônoma, perseguindo um objeto 'targetPos' que se move aleatoriamente.
/// Baseia-se no sistema de Piloto Automático do Airplane.cs.
/// </summary>
public class AirplaneAI : MonoBehaviour
{
    [Header("Boundaries (XYZ Limits)")]
    [Tooltip("Centro da área de voo permitida.")]
    public Vector3 center;
    [Tooltip("Tamanho total da caixa onde o targetPos pode se mover.")]
    public Vector3 size = new Vector3(1000, 500, 1000);
    
    [Header("Randomization")]
    [Tooltip("Intervalo mínimo para o targetPos mudar de lugar.")]
    public float minChangeTime = 8f;
    [Tooltip("Intervalo máximo para o targetPos mudar de lugar.")]
    public float maxChangeTime = 15f;
    
    [Header("Flight Settings")]
    [Tooltip("Distância do alvo para considerar como alcançado antes do tempo.")]
    public float targetReachThreshold = 50f;
    [Tooltip("Velocidade (thrust) sugerida.")]
    public float cruiseThrust = 100f;

    [Header("Gizmos")]
    public bool showGizmos = true;

    private Airplane _airplane;
    private Propeller _propeller;
    private Transform _targetTransform;
    private float _nextChangeTime;
    private float _timer;

    private void Awake()
    {
        _airplane = GetComponent<Airplane>();
        _propeller = GetComponent<Propeller>();
        
        if (_airplane != null)
        {
            _airplane.isAiManaged = true;
            // Alvo inicial à frente para não mergulhar para 0,0,0
            _airplane.aiTargetPos = transform.position + transform.forward * 100f;
        }

        // 1. Cria o GameObject do alvo exclusivo para esta IA
        GameObject targetObj = new GameObject("targetPos_" + name);
        _targetTransform = targetObj.transform;
    }

    private void Start()
    {
        // Se o centro não foi definido, usa a posição inicial
        if (center == Vector3.zero)
        {
            center = transform.position;
        }

        // Define o primeiro ponto aleatório
        MoveTargetPos();
    }

    private void Update()
    {
        if (_airplane == null || _targetTransform == null) return;

        _timer += Time.deltaTime;

        // 2. Verifica se chegou perto do target ou se o cronômetro expirou
        float distToTarget = Vector3.Distance(transform.position, _targetTransform.position);
        
        if (distToTarget < targetReachThreshold || _timer >= _nextChangeTime)
        {
            MoveTargetPos();
        }

        // 3. O alvo do avião é SEMPRE a posição do Transform spawnado
        _airplane.aiTargetPos = _targetTransform.position;

        HandleSpeedControl();
        EnforceBoundaries();
    }

    /// <summary>
    /// Move o GameObject targetPos para uma nova coordenada aleatória dentro do volume.
    /// Chamamos isso para garantir menos determinismo.
    /// </summary>
    private void MoveTargetPos()
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        
        _targetTransform.position = new Vector3(x, y, z);
        
        // Timer aleatório para cada IA ser diferente
        _timer = 0;
        _nextChangeTime = Random.Range(minChangeTime, maxChangeTime);
    }

    private void HandleSpeedControl()
    {
        if (_propeller == null)
        {
            _airplane.thrust = Mathf.MoveTowards(_airplane.thrust, cruiseThrust, Time.deltaTime * 50f);
            return;
        }

        // Acelera se estiver lento ou subindo
        if (_airplane.thrust < cruiseThrust || transform.forward.y > 0.1f)
        {
            _propeller.accelerate = true;
            _propeller.deaccelerate = false;
        }
        else
        {
            _propeller.accelerate = false;
            _propeller.deaccelerate = _airplane.thrust > cruiseThrust * 2f;
        }
    }

    private void EnforceBoundaries()
    {
        // Força a IA de volta ao centro se fugir demais da caixa
        Vector3 pos = transform.position;
        bool outX = Mathf.Abs(pos.x - center.x) > size.x * 0.6f; // Margem um pouco maior para evitar loops
        bool outY = Mathf.Abs(pos.y - center.y) > size.y * 0.6f;
        bool outZ = Mathf.Abs(pos.z - center.z) > size.z * 0.6f;

        if (outX || outY || outZ)
        {
            _targetTransform.position = center;
        }
    }

    private void OnDestroy()
    {
        // Limpeza: remove o gameObject criado dinamicamente ao morrer
        if (_targetTransform != null)
        {
            Destroy(_targetTransform.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);

        if (Application.isPlaying && _targetTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_targetTransform.position, 15f);
            Gizmos.DrawLine(transform.position, _targetTransform.position);
        }
    }
}
