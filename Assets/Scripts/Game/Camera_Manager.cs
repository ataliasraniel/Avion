using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Camera_Manager : MonoBehaviour
{
    public Airplane airplane;
    public GameObject flightCamera;
    private string flightCameraTxt = "Flight Camera";
    public GameObject freeCamera;
    private string freeCameraTxt = "Free Look Camera";
    public Canvas flightCanvas;

    private Gameui_Manager uiManager;
    /// <summary>
    ///este script controlará a mudança de câmera, entre a free look e a câmera de voo
    ///</summary>
    private void Start()
    {
        uiManager = FindObjectOfType<Gameui_Manager>();
    }

    public bool isFreeCamera = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isFreeCamera)
            {
                isFreeCamera = false;
                flightCanvas.enabled = true;
                uiManager.ShowPopup(flightCameraTxt); //manda um popup com um texto informando a câmera que está sendo usada
            }
            else
            {
                isFreeCamera = true;
                flightCanvas.enabled = false;
                uiManager.ShowPopup(freeCameraTxt); //manda um popup com um texto informando a câmera que está sendo usada
            }

        }
        ChangeCamera();
    }

    private void ChangeCamera()
    {
        if (isFreeCamera)
        {
            freeCamera.SetActive(true);
            flightCamera.SetActive(false);
            airplane.lookAround = true;

        }
        else
        {
            freeCamera.SetActive(false);
            flightCamera.SetActive(true);
            airplane.lookAround = false;


        }
    }
}
