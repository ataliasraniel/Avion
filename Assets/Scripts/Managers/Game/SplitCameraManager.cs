using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o split de câmera para múltiplos jogadores.
/// Busca automaticamente as câmeras na cena após o spawn dos aviões.
/// Coloque este script na cena de jogo. Ele roda após o LobbySpawner.
/// </summary>
public class SplitCameraManager : MonoBehaviour
{
  public enum SplitMode
  {
    Vertical,   // | P1 | P2 |  (lado a lado)
    Horizontal  // P1 em cima, P2 embaixo
  }

  [Header("Split Configuration")]
  public SplitMode splitMode = SplitMode.Vertical;

  [Header("Single Player")]
  [Tooltip("Em modo single player, a câmera ocupa a tela toda")]
  public bool forceSinglePlayer = false;

  // Viewport Rects pré-calculados
  // Vertical split: P1 esquerda, P2 direita
  private static readonly Rect vertP1 = new Rect(0f, 0f, 0.5f, 1f);
  private static readonly Rect vertP2 = new Rect(0.5f, 0f, 0.5f, 1f);

  // Horizontal split: P1 em cima, P2 embaixo
  private static readonly Rect horizP1 = new Rect(0f, 0.5f, 1f, 0.5f);
  private static readonly Rect horizP2 = new Rect(0f, 0f, 1f, 0.5f);

  // Tela cheia (solo)
  private static readonly Rect fullscreen = new Rect(0f, 0f, 1f, 1f);

  private void Start()
  {
    // Espera 1 frame para garantir que LobbySpawner já spawnrou tudo no Start()
    StartCoroutine(SetupCamerasDelayed());
  }

  private IEnumerator SetupCamerasDelayed()
  {
    yield return null; // aguarda 1 frame

    SetupCameras();
  }

  public void SetupCameras()
  {
    // Busca todas as cameras ativas na cena
    Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

    // Filtra apenas câmeras de voo (descarta câmeras UI, Cinemachine Brain, etc.)
    List<Camera> flightCams = new List<Camera>();
    foreach (var cam in allCameras)
    {
      // Ignora câmeras de UI (RenderMode Overlay) e desativadas
      if (!cam.gameObject.activeInHierarchy) continue;
      if (cam.GetComponent<UnityEngine.UI.GraphicRaycaster>() != null) continue;

      flightCams.Add(cam);
    }

    int count = flightCams.Count;

    if (count == 0)
    {
      Debug.LogError("<color=red>[SplitCameraManager]</color> Nenhuma câmera de voo encontrada na cena!");
      return;
    }

    if (count == 1 || forceSinglePlayer)
    {
      // Solo: câmera ocupa tela toda
      flightCams[0].rect = fullscreen;
      Debug.Log("<color=cyan>[SplitCameraManager]</color> Modo Single Player — câmera em tela cheia.");
      return;
    }

    if (count >= 2)
    {
      // Ordena as câmeras por playerIndex se possível (via MouseController no pai)
      flightCams.Sort((a, b) =>
      {
        int idxA = GetPlayerIndex(a);
        int idxB = GetPlayerIndex(b);
        return idxA.CompareTo(idxB);
      });

      if (splitMode == SplitMode.Vertical)
      {
        flightCams[0].rect = vertP1;
        flightCams[1].rect = vertP2;
        Debug.Log("<color=green>[SplitCameraManager]</color> Split Vertical aplicado (P1 esquerda | P2 direita).");
      }
      else
      {
        flightCams[0].rect = horizP1;
        flightCams[1].rect = horizP2;
        Debug.Log("<color=green>[SplitCameraManager]</color> Split Horizontal aplicado (P1 cima | P2 baixo).");
      }

      // Se houver mais de 2, desativa as extras para não bagunçar
      for (int i = 2; i < flightCams.Count; i++)
      {
        flightCams[i].gameObject.SetActive(false);
        Debug.LogWarning($"[SplitCameraManager] Câmera extra '{flightCams[i].name}' desativada (máx 2 suportado).");
      }
    }
  }

  /// <summary>
  /// Tenta descobrir o playerIndex da câmera olhando nos pais por um MouseController.
  /// Se não encontrar, retorna 0 como fallback.
  /// </summary>
  private int GetPlayerIndex(Camera cam)
  {
    // O MouseController tem um PlayerInput com playerIndex
    MouseController mc = cam.GetComponentInParent<MouseController>();
    if (mc != null && mc.input != null)
      return mc.input.playerIndex;

    return 0;
  }
}
