using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerSelector : MonoBehaviour
{
  [Header("Identity")]
  [HideInInspector] public int playerIndex = 0; // Preenchido na hora que nasce

  [Header("Visuals")]
  public Image selectorImage;
  public Color player1Color = new Color(0, 1, 1, 1); // Cyan
  public Color player2Color = new Color(0, 1, 0, 1); // Verde

  [Header("References")]
  [HideInInspector] public PlayerSelectorManager manager;

  [HideInInspector]
  public int currentIndex = 0;

  private PlayerInput playerInput;
  private float inputCooldown = 0.25f;
  private float nextInputTime = 0f;
  private bool hasConfirmed = false;

  private void Awake()
  {
    playerInput = GetComponent<PlayerInput>();
    playerIndex = playerInput.playerIndex;

    if (manager == null)
      manager = Object.FindFirstObjectByType<PlayerSelectorManager>();

    // Joga o cursor recém-nascido pra dentro da UI (Canvas)!
    // Isso evita que a Unity instancie ele na "Raiz do Mundo", ficando invisível/gigante fora do Canvas.
    if (manager != null)
    {
      Transform dest = manager.cursorsContainer != null ? manager.cursorsContainer : manager.transform;
      transform.SetParent(dest, false);
      transform.localScale = Vector3.one; // Previne bizarrices de UI scaler
    }
  }

  private System.Collections.IEnumerator Start()
  {
    if (selectorImage == null) selectorImage = GetComponent<Image>();
    UpdateColor();

    // Aguarda um pequeno momento para que as caixas da UI terminem de ser desenhadas no tamanho/posicao real
    yield return new WaitForEndOfFrame();

    ResetPosition();
  }

  public void UpdateColor()
  {
    if (selectorImage != null)
    {
      selectorImage.color = (playerIndex == 0) ? player1Color : player2Color;
    }
  }

  // Chamado pelo Input/LobbySelector para mover o cursor pra esquerda
  public void MoveLeft()
  {
    if (manager == null || manager.spawnedItems.Count == 0) return;

    if (currentIndex > 0)
    {
      currentIndex--;
      UpdatePosition();
    }
  }

  // Chamado pelo Input/LobbySelector para mover o cursor pra direita
  public void MoveRight()
  {
    if (manager == null || manager.spawnedItems.Count == 0) return;

    if (currentIndex < manager.spawnedItems.Count - 1)
    {
      currentIndex++;
      UpdatePosition();
    }
  }

  public void ResetPosition()
  {
    currentIndex = 0;
    UpdatePosition();
  }

  public void UpdatePosition()
  {
    if (manager != null && manager.spawnedItems.Count > currentIndex)
    {
      // Pega o Transform do item alvo (A caixinha do avião na UI)
      Transform targetItem = manager.spawnedItems[currentIndex].transform;

      // Move a âncora/posição do Seletor (a borda gráfica) visualmente para cravar exatmente em cima do Item alvo
      transform.position = targetItem.position;
    }
  }

  public AirplaneData GetSelectedAirplaneData()
  {
    if (manager != null && manager.spawnedItems.Count > currentIndex)
    {
      return manager.spawnedItems[currentIndex].data;
    }
    return null;
  }

  private void OnEnable()
  {
    // Se a Unity rodar OnEnable muito rápido (antes do Awake), pegamos aqui
    if (playerInput == null) playerInput = GetComponent<PlayerInput>();

    if (playerInput != null && playerInput.actions != null)
    {
      playerInput.actions["Move"].performed += OnMoveInput;
      playerInput.actions["ShootPrimary"].performed += OnSelectInput;
    }
  }

  private void OnDisable()
  {
    if (playerInput != null && playerInput.actions != null)
    {
      playerInput.actions["Move"].performed -= OnMoveInput;
      playerInput.actions["ShootPrimary"].performed -= OnSelectInput;
    }
  }

  private void OnMoveInput(InputAction.CallbackContext context)
  {
    print("Movinggg");
    if (Time.time < nextInputTime) return;

    Vector2 inputDir = context.ReadValue<Vector2>();
    print("Input: " + inputDir);

    if (inputDir.x > 0.5f)
    {
      MoveRight();
      nextInputTime = Time.time + inputCooldown;
    }
    else if (inputDir.x < -0.5f)
    {
      MoveLeft();
      nextInputTime = Time.time + inputCooldown;
    }
  }

  private void OnSelectInput(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      ConfirmSelection();
    }
  }

  private void ConfirmSelection()
  {
    if (manager == null || !manager.isLayoutReady) return;
    if (hasConfirmed) return; // Impede dupla confirmação

    AirplaneData selectedData = GetSelectedAirplaneData();
    if (selectedData == null) return;

    hasConfirmed = true;

    // Feedback visual: escurece o cursor para indicar que está travado
    if (selectorImage != null)
    {
      Color c = selectorImage.color;
      c.a = 0.4f;
      selectorImage.color = c;
    }

    // Desinscreve os inputs de movimento para o cursor não se mover mais após confirmar
    if (playerInput != null && playerInput.actions != null)
    {
      playerInput.actions["Move"].performed -= OnMoveInput;
    }

    Debug.Log($"<color=magenta>[PlayerSelector]</color> Player {playerIndex} confirmou: {selectedData.airplaneName}!");

    // Registra o player e seus dispositivos na sessão persistente
    // PlayersSession vai checar se todos confirmaram e trocar de cena automaticamente
    if (PlayersSession.Instance != null && playerInput != null)
    {
      InputDevice[] devices = playerInput.user.pairedDevices.ToArray();
      PlayersSession.Instance.AddPlayer(
          playerIndex,
          devices,
          playerInput,
          selectedData.airplanePrefab,
          selectedData.airplaneName
      );
    }
    else
    {
      Debug.LogError("[PlayerSelector] PlayersSession não encontrado na cena!");
    }
  }
}
