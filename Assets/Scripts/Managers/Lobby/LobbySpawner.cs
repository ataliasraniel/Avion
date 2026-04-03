using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Coloque este script numa cena de jogo.
/// Ele lê os dados do PlayersSession (que sobreviveu do Lobby) e spawna cada avião
/// com o PlayerInput correto já injetado nos scripts de voo.
/// </summary>
public class LobbySpawner : MonoBehaviour
{
  public static LobbySpawner instance;

  [Header("Spawn Points")]
  [Tooltip("Pontos de spawn para cada player. Index 0 = P1, Index 1 = P2, etc.")]
  public Transform[] spawnPoints;

  private void Awake()
  {
    if (instance == null) instance = this;
    else Destroy(gameObject);
  }

  private void Start()
  {
    SpawnAllPlayers();
  }

  private void SpawnAllPlayers()
  {
    if (PlayersSession.Instance == null)
    {
      Debug.LogError("<color=red>[LobbySpawner]</color> PlayersSession não encontrado! Certifique-se de que ele existe na cena do Lobby.");
      return;
    }

    var players = PlayersSession.Instance.players;

    if (players == null || players.Count == 0)
    {
      Debug.LogError("<color=red>[LobbySpawner]</color> Nenhum player registrado no PlayersSession!");
      return;
    }

    for (int i = 0; i < players.Count; i++)
    {
      var playerData = players[i];

      if (playerData.airplanePrefab == null)
      {
        Debug.LogError($"<color=red>[LobbySpawner]</color> Player {playerData.playerIndex} não tem prefab de avião registrado!");
        continue;
      }

      if (playerData.playerInput == null)
      {
        Debug.LogError($"<color=red>[LobbySpawner]</color> Player {playerData.playerIndex} não tem PlayerInput registrado!");
        continue;
      }

      Transform spawnPoint = (spawnPoints != null && i < spawnPoints.Length)
        ? spawnPoints[i]
        : transform; // fallback: usa a própria posição do spawner

      SpawnAirplane(playerData, spawnPoint);
    }
  }

  public void SpawnAirplane(PlayersSession.PlayerData playerData, Transform spawnPoint)
  {
    // 1. Instancia o avião no ponto de spawn
    GameObject airplane = Instantiate(playerData.airplanePrefab, spawnPoint.position, spawnPoint.rotation);

    PlayerInput lobbyInput = playerData.playerInput;

    // 2. Remove qualquer PlayerInput embutido no prefab do avião
    //    para evitar que a Unity reconheça um segundo jogador e force split-screen.
    PlayerInput embeddedInput = airplane.GetComponentInChildren<PlayerInput>();
    if (embeddedInput != null)
    {
      Destroy(embeddedInput);
      Debug.Log($"<color=yellow>[LobbySpawner]</color> PlayerInput embutido destruído no avião do Player {playerData.playerIndex}.");
    }

    // 3. Pega os scripts de voo do avião instanciado
    Airplane mainScript = airplane.GetComponentInChildren<Airplane>();
    MouseController mouseController = airplane.GetComponentInChildren<MouseController>();

    // 4. Usa o Setup() do Airplane que injeta input e referência ao MouseController de uma vez
    if (mainScript != null && mouseController != null)
    {
      mainScript.Setup(lobbyInput, mouseController);
      mouseController.SetReferenceAirplane(airplane.transform, mouseController);
      Debug.Log($"<color=green>[LobbySpawner]</color> Input injetado no Airplane + MouseController do Player {playerData.playerIndex}.");
    }
    else
    {
      Debug.LogWarning($"[LobbySpawner] Player {playerData.playerIndex}: Airplane={mainScript != null}, MouseController={mouseController != null}");
    }

    // 5. Injeta no Booster também, se existir
    Booster booster = airplane.GetComponentInChildren<Booster>();
    if (booster != null)
    {
      booster.input = lobbyInput;
    }

    Debug.Log($"<color=green>[LobbySpawner]</color> Avião '{playerData.airplaneName}' do Player {playerData.playerIndex} spawnado em {spawnPoint.name}!");
  }
}
