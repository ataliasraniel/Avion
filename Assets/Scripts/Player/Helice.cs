using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helice : MonoBehaviour
{
    ///<summary> 
    ///este script dará conta da aceleração do avião, comunicando-se com
    ///o script Airplane para acelerar a velocidade do avião
    ///e controlar a rotação da hélice
    ///</summary>

    public Transform heliceLow;
    public Transform heliceFast;
    private bool isFast;


    [Header("Aceleração")]
    [Range(8, 36)]
    public float rpm = 12f;
    public float rpmMultiplier = 0.1f;
    public float degrees = 32f;
    public bool accelerate;
    public bool deaccelerate;
    public float speedMultiplier = 0.1f;
    public float reverseSpeedMultiplier = 0.3f;
    private Airplane _airplane;

    [Header("Som")]
    public float pitchMultiplier = 0.02f;
    public float reversePitchMultiplier = 0.06f;
    public float pitchMin = -0.1f;
    public float pitchMax = 1.2f;
    private AudioSource soundEmmiter;
    public AudioClip[] rpmSFX;

    [Header("Informações")]
    public float speedKM;
    public float altitude;
    public float actualRpm = 1;
    private Rigidbody _rb;
    private Transform seaPos;


    void Start()
    {
        heliceFast.gameObject.SetActive(false);
        soundEmmiter = GetComponentInChildren<AudioSource>();
        Gameui_Manager.instance.RpmCounterText(actualRpm);
        Gameui_Manager.instance.SpeedCounterText(speedKM);
        Gameui_Manager.instance.AltCounterText(altitude);
        seaPos = GameObject.FindGameObjectWithTag("Ocean").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();
        _airplane = GetComponent<Airplane>();
    }
    private void Update()
    {

        RPMController();
        Rotate();
        CalculateSpeed();
        CalculateAltitude();
        SpeedController();

    }
    void RPMController()
    {
        //a depender da rotação, aumentar a velocidade do avião, trocar
        //o som da hélice e aumentar a rotação da hélice
        soundEmmiter.pitch = Mathf.Clamp(soundEmmiter.pitch, pitchMin, pitchMax);
        rpm = Mathf.Clamp(rpm, 1, 36);
        actualRpm = Mathf.Clamp(rpm, 0, 100);
        if (Input.GetKey(KeyCode.W))
        {
            accelerate = true;
            rpm += Time.deltaTime + rpmMultiplier;
            actualRpm += 1 * Time.deltaTime;
            soundEmmiter.pitch += Time.deltaTime * pitchMultiplier;
            //faz com que o rpm seja passado para um texto
            Gameui_Manager.instance.RpmCounterText(actualRpm);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            deaccelerate = true;
            actualRpm -= 1 * Time.deltaTime;
            rpm -= Time.deltaTime + rpmMultiplier;
            soundEmmiter.pitch -= Time.deltaTime * reversePitchMultiplier;
            //faz com que o rpm seja passado para um texto
            Gameui_Manager.instance.RpmCounterText(actualRpm);

        }
        else
        {
            accelerate = false;
            deaccelerate = false;
        }
        //se o rpm for maior que o limite, troca para a hélice rápida
        if (rpm >= 30f)
        {
            isFast = true;
        }
        else
        {
            isFast = false;
        }
    }
    void SpeedController()
    {
        //controla a velocidade do avião
        if (accelerate)
        {
            _airplane.thrust += speedMultiplier * Time.deltaTime;
        }
        else if (deaccelerate)
        {
            _airplane.thrust -= reverseSpeedMultiplier * Time.deltaTime;

        }
    }
    void Rotate()
    {
        //faz com que a hélice do avião gire    
        if (isFast)
        {
            heliceFast.gameObject.SetActive(true);
            heliceLow.gameObject.SetActive(false);
            heliceFast.Rotate(0, 0, degrees * rpm * Time.deltaTime, Space.Self);
        }
        else
        {
            heliceFast.gameObject.SetActive(false);
            heliceLow.gameObject.SetActive(true);
            heliceLow.Rotate(0, 0, degrees * rpm * Time.deltaTime, Space.Self);
        }

    }
    void CalculateSpeed()
    {
        //calcula a velocidade do corpo em KM/h
        speedKM = _rb.velocity.magnitude * 3.6f;
        speedKM = Mathf.Round(speedKM);
        Gameui_Manager.instance.SpeedCounterText(speedKM);
    }
    void CalculateAltitude()
    {
        //calcula a velocidade em relação ao ponto 0 do mapa, o mar
        float thisPos = transform.position.y;
        altitude = thisPos - seaPos.position.y;
        altitude = Mathf.Round(altitude);
        Gameui_Manager.instance.AltCounterText(altitude);
    }
    void AltitudeController()
    {
        //TODO: implementar a queda do avião quando ele estiver sem força
        //1 - desligar o controle do avião colocando suas forças multiplicadoras pra baixo
        //2 - ativar a gravidade do rigidbody
        //3 - aumentar a massa do objeto para cair mais rápido
    }


}

