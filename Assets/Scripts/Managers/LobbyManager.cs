using UnityEngine;

public class LobbyManager : MonoBehaviour
{
  public static LobbyManager instance;

  private void Awake()
  {
    instance = this;
  }

  // A lógica de gerenciar players e split screens não ocorrerá mais centralizadamente
  // pelo PlayerInput nesse script, visto que cada jogador instanciará
  // seu próprio prefab gerindo seu próprio ciclo de vida a partir de agora.
}