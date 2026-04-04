using UnityEngine;

public class BulletType1 : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float damage = 5f;

    private void Start()
    {
        // Limite de vida do projétil para não pesar na memória
        Destroy(gameObject, 2f);
    }

    private void FixedUpdate()
    {
        // Avança o projétil baseado no eixo forward
        transform.position += transform.forward * Time.fixedDeltaTime * moveSpeed;
    }
}
