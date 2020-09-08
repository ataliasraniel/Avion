using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flight_Hud : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Mousecontroller mouseFlight = null;

    [Header("HUD Elements")]
    [SerializeField] private RectTransform boresight = null;
    [SerializeField] private RectTransform mousePos = null;

    private Camera playerCam = null;

    private void Awake()
    {
        if (mouseFlight == null)
            Debug.LogError(name + ": Hud - Mouse Flight Controller not assigned!");

        playerCam = mouseFlight.GetComponentInChildren<Camera>();

        if (playerCam == null)
            Debug.LogError(name + ": Hud - No camera found on assigned Mouse Flight Controller!");
    }

    private void Update()
    {
        if (mouseFlight == null || playerCam == null)
            return;

        UpdateGraphics(mouseFlight);
    }

    private void UpdateGraphics(Mousecontroller controller)
    {
        if (boresight != null)
        {
            boresight.position = playerCam.WorldToScreenPoint(controller.BoresightPos);
            boresight.gameObject.SetActive(boresight.position.z > 1f);
        }

        if (mousePos != null)
        {
            mousePos.position = playerCam.WorldToScreenPoint(controller.MouseAimPos);
            mousePos.gameObject.SetActive(mousePos.position.z > 1f);
        }
    }

    public void SetReferenceMouseFlight(Mousecontroller controller)
    {
        mouseFlight = controller;
    }
}
