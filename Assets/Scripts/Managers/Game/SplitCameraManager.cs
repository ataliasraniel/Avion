using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o split de câmera para múltiplos jogadores.
/// Suporta 1, 2 e agora 3 jogadores simultâneos.
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

    // 3 Players Grid (2 em cima, 1 embaixo largo)
    private static readonly Rect grid3P1 = new Rect(0f, 0.5f, 0.5f, 0.5f);
    private static readonly Rect grid3P2 = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
    private static readonly Rect grid3P3 = new Rect(0f, 0f, 1f, 0.5f);

    // Tela cheia (solo)
    private static readonly Rect fullscreen = new Rect(0f, 0f, 1f, 1f);

    private void Start()
    {
        // Espera 1 frame para garantir que o spawn de jogadores terminou
        StartCoroutine(SetupCamerasDelayed());
    }

    private IEnumerator SetupCamerasDelayed()
    {
        yield return null; 
        SetupCameras();
    }

    public void SetupCameras()
    {
        // Busca todas as câmeras ativas na cena
        Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        List<Camera> flightCams = new List<Camera>();

        foreach (var cam in allCameras)
        {
            // Ignora câmeras de UI e desativadas
            if (!cam.gameObject.activeInHierarchy) continue;
            if (cam.GetComponent<UnityEngine.UI.GraphicRaycaster>() != null) continue;
            if (cam.name.Contains("UI")) continue; // Filtro extra de nome

            flightCams.Add(cam);
        }

        int count = flightCams.Count;

        if (count == 0)
        {
            Debug.LogError("<color=red>[SplitCameraManager]</color> Nenhuma câmera de voo encontrada!");
            return;
        }

        // Ordena as câmeras pelo PlayerIndex (via MouseController no pai)
        flightCams.Sort((a, b) =>
        {
            int idxA = GetPlayerIndex(a);
            int idxB = GetPlayerIndex(b);
            return idxA.CompareTo(idxB);
        });

        Debug.Log($"<color=cyan>[SplitCameraManager]</color> Configurando {count} câmeras.");

        // Lógica de distribuição baseada na quantidade de jogadores
        if (count == 1 || forceSinglePlayer)
        {
            flightCams[0].rect = fullscreen;
        }
        else if (count == 2)
        {
            if (splitMode == SplitMode.Vertical)
            {
                flightCams[0].rect = vertP1;
                flightCams[1].rect = vertP2;
            }
            else
            {
                flightCams[0].rect = horizP1;
                flightCams[1].rect = horizP2;
            }
        }
        else if (count >= 3)
        {
            // Modo Grid 3-Players (P1 TopLeft, P2 TopRight, P3 BottomFull)
            flightCams[0].rect = grid3P1;
            flightCams[1].rect = grid3P2;
            flightCams[2].rect = grid3P3;

            // Se houver uma 4ª câmera (futuro), deixamos ela desativada ou no 4º quadrante
            if (count >= 4)
            {
                // Ajustamos o P3 para TopLeft e P4 para BottomRight se quiser 4p
                // Por enquanto focamos no pedido de 3p
                for (int i = 3; i < flightCams.Count; i++)
                {
                    flightCams[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private int GetPlayerIndex(Camera cam)
    {
        MouseController mc = cam.GetComponentInParent<MouseController>();
        if (mc != null && mc.input != null)
            return mc.input.playerIndex;

        return 0;
    }
}
