using UnityEngine;
using UnityEngine.InputSystem;

public class LobbySpawner : MonoBehaviour
{
  public static LobbySpawner instance;

  [Header("Spawn Configuration")]
  [Tooltip("Coloque aqui os Transforms vazios que servirão de berço para os aviões nascerem.")]
  public Transform[] spawnPoints;

  private int currentSpawnIndex = 0;

  private void Awake()
  {
    if (instance == null) instance = this;
    else Destroy(gameObject);
  }

  /// <summary>
  /// Spawna o avião selecionado e o atrela à raiz do Jogador (LobbySelector root) que possui o PlayerInput.
  /// </summary>
  public GameObject SpawnAirplane(GameObject airplanePrefab, GameObject playerRoot)
  {
    // 1. Desativar Split Screen no PlayerInputManager global para garantir SINGLE PLAYER no spawn
    var pim = FindFirstObjectByType<PlayerInputManager>();
    if (pim != null)
    {
      // pim.splitScreen = false;
    }

    if (spawnPoints == null || spawnPoints.Length == 0)
    {
      Debug.LogError("<color=red>[LobbySpawner]</color> Nenhum SpawnPoint configurado! Crie GameObjects vazios na cena e arraste para o array do Spawner.");
      return null;
    }

    // Pega o ponto de spawn atual e avança o índice para não nascer um em cima do outro
    Transform spawnPoint = spawnPoints[currentSpawnIndex % spawnPoints.Length];
    currentSpawnIndex++;

    // Instancia a verdadeira malha mecânica do avião com suas físicas
    GameObject airplaneInstance = Instantiate(airplanePrefab, spawnPoint.position, spawnPoint.rotation);
    print(airplaneInstance.transform.position);
    MouseController mouseController = GameObject.FindFirstObjectByType<MouseController>();
    var mainScript = airplaneInstance.GetComponentInChildren<Airplane>();
    PlayerInput lobbyInput = playerRoot.GetComponent<PlayerInput>();
    // if (mainScript != null) mainScript.Setup(lobbyInput);
    mouseController.SetReferenceAirplane(airplaneInstance.transform, mouseController);
    // mainScript.
    print("SETTED");

    // 2. Extrai o PlayerInput original validado da raiz do Lobby

    // 3. Destrói sumariamente qualquer PlayerInput indesejado que venha embutido no prefab do Avião
    // (Isso impede que a Unity reconheça ele como se um Jogador Extra estivesse entrando, evitando o Split)
    PlayerInput badInput = airplaneInstance.GetComponent<PlayerInput>();
    if (badInput != null) Destroy(badInput);

    // // 4. Passamos adiante a nossa Instância original e soberana para os módulos de Voo do Avião



    // var boosterScript = airplaneInstance.GetComponentInChildren<Booster>();
    // if (boosterScript != null) boosterScript.input = lobbyInput;

    // var extMouseScript = airplaneInstance.GetComponentInChildren<MouseController>();
    // if (extMouseScript != null) extMouseScript.input = lobbyInput;


    Debug.Log($"<color=green>[LobbySpawner]</color> Avião {airplanePrefab.name} spawnado recebendo Input diretamente do Lobby!");
    return airplaneInstance;
  }
}
