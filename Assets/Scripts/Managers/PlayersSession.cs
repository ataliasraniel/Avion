using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayersSession : MonoBehaviour
{
  public static PlayersSession Instance { get; private set; }

  [Header("Scene Transition")]
  [Tooltip("Nome exato da cena de jogo que será carregada quando todos confirmarem")]
  public string gameSceneName = "Game";

  [Tooltip("Quantos players são necessários para iniciar? (1 = Solo, 2 = Coop obrigatório)")]
  public int requiredPlayers = 2;

  [System.Serializable]
  public class PlayerData
  {
    public int playerIndex;
    public InputDevice[] devices;
    public PlayerInput playerInput;       // Referência direta ao componente
    public GameObject selectorObject;    // O GameObject do PlayerSelector (carrega o PlayerInput)
    public string airplaneName;
    public GameObject airplanePrefab;
    public bool hasConfirmed = false;
  }


  public List<PlayerData> players = new List<PlayerData>();

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void AddPlayer(int index, InputDevice[] devices, PlayerInput input, GameObject prefab, string name)
  {
    // Remove se já existir (para evitar duplicatas em re-seleção)
    players.RemoveAll(p => p.playerIndex == index);

    // Guarda a referência do GameObject do selector. O DontDestroyOnLoad será aplicado
    // APENAS no momento da troca de cena, para não remover do Canvas antes da hora.
    GameObject selectorGO = input != null ? input.gameObject : null;

    players.Add(new PlayerData
    {
      playerIndex = index,
      devices = devices,
      playerInput = input,
      selectorObject = selectorGO,
      airplanePrefab = prefab,
      airplaneName = name,
      hasConfirmed = true
    });

    Debug.Log($"<color=cyan>[PlayersSession]</color> Player {index} ({name}) registrado. Dispositivos: {devices.Length}. Total: {players.Count}/{requiredPlayers}");

    TryStartGame();
  }

  /// <summary>
  /// Checa se todos os players que entraram (dispositivos ativos) confirmaram.
  /// Se houver apenas 1 player e ele confirmar -> Inicia Solo.
  /// Se houver 2 players e ambos confirmarem -> Inicia Coop.
  /// Se houver 2 players e apenas 1 confirmou -> Aguarda o segundo.
  /// </summary>
  private void TryStartGame()
  {
    // 1. Descobrimos quantos seletores ativos existem na cena (players que deram "Join")
    PlayerSelector[] activeSelectors = Object.FindObjectsByType<PlayerSelector>(FindObjectsSortMode.None);
    int totalJoinedPlayers = activeSelectors.Length;

    if (totalJoinedPlayers == 0) return;

    // 2. Contamos quantos desses seletores já confirmaram a escolha
    int confirmedCount = 0;
    foreach (var p in players)
    {
      if (p.hasConfirmed) confirmedCount++;
    }

    Debug.Log($"<color=cyan>[PlayersSession]</color> Status: {confirmedCount} de {totalJoinedPlayers} confirmaram.");

    // 3. Condição: Todos que entraram devem ter confirmado (mínimo 1)
    if (confirmedCount >= totalJoinedPlayers && totalJoinedPlayers > 0)
    {
      // Apenas AGORA desacoplamos os seletores do Canvas e os protegemos.
      foreach (var p in players)
      {
        if (p.selectorObject != null)
        {
          p.selectorObject.transform.SetParent(null);
          DontDestroyOnLoad(p.selectorObject);
          Debug.Log($"<color=green>[PlayersSession]</color> Player {p.playerIndex}: Selector protegido.");
        }
      }

      Debug.Log($"<color=green>[PlayersSession]</color> Todos os jogadores ativos confirmaram! Carregando: '{gameSceneName}'...");
      SceneManager.LoadScene(gameSceneName);
    }
    else
    {
      Debug.Log($"<color=yellow>[PlayersSession]</color> Aguardando confirmação de todos os players conectados...");
    }
  }

  public PlayerData GetPlayer(int index)
  {
    return players.Find(p => p.playerIndex == index);
  }

  public void ClearSession()
  {
    players.Clear();
    Debug.Log("[PlayersSession] Sessão limpa.");
  }
}
