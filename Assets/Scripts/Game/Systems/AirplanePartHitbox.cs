using UnityEngine;

/// <summary>
/// Deve ser anexado a objetos filhos do avião que possuem Colisores (Asas, Motor, etc).
/// Encaminha o dano do projétil para o AirplaneLifeSystem pai.
/// </summary>
public class AirplanePartHitbox : MonoBehaviour
{
    [Header("Configuração da Parte")]
    [Tooltip("Qual parte do avião este colisor representa.")]
    public AirplaneLifeSystem.AirplanePart partType;

    [Header("Referências")]
    [SerializeField] private AirplaneLifeSystem _mainSystem;

    private void Awake()
    {
        // Tenta encontrar o sistema de vida no pai se não for atribuído
        if (_mainSystem == null)
        {
            _mainSystem = GetComponentInParent<AirplaneLifeSystem>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_mainSystem == null || _mainSystem.isDead) return;

        // Detecta o projétil
        BulletType1 bullet = other.GetComponent<BulletType1>();
        if (bullet != null)
        {
            // Aplica o dano na parte específica deste hitbox
            _mainSystem.TakeDamage(bullet.damage, partType);
            
            // Destrói o projétil ao impacto
            Destroy(other.gameObject);
        }
    }
}
