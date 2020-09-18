using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Motor : MonoBehaviour
{
    private CharacterController controller;
    public float rotSpeedX;
    public float rotSpeedY;
    public float baseSpeed;
    [Header("Controle de Velocidade")]
    public float speed;
    public float maxSpeed;
    public float accelerateBoost;
    public float desacelerateBoost;
    public enum Acceleration
    {
        acelerating,
        idle,
        desacelerating
    };
    public Acceleration acceleration;

    //[Header("Hélice")]
    private Helice helice;

    [Header("Audio")]
    //Controla o áudio da engine
    public AudioSource audioSource;
    public float maxPitch;
    public float pitchIncreaseSpeed;
    public float pitchDecreaseSpeed;

    void Start()
    {
        helice = GetComponent<Helice>();
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        MotorController();
        HeliceController();
        //clamp a velocidade máxima e mínima
        baseSpeed = Mathf.Clamp(baseSpeed, 80, maxSpeed);

    }
    void HeliceController()
    {
        //impede que o pitch passe de certa velocidade
        //pega a referência do input de aceleração
        float accelerationInput = Input.GetAxis("Acceleration");
        if (accelerationInput > 0.1)
        {
            acceleration = Acceleration.acelerating;
        }
        else if (accelerationInput < -0.1)
        {
            acceleration = Acceleration.desacelerating;
        }
        else
        {
            acceleration = Acceleration.idle;
        }
        switch (acceleration)
        {
            case Acceleration.idle:

                return;

            case Acceleration.acelerating:

                //faz com que a velocidade tenha um boost                
                baseSpeed += accelerateBoost * Time.deltaTime;
                audioSource.pitch += pitchIncreaseSpeed * Time.deltaTime;   //faz com que o pitch aumente, dando a impressão de força
                audioSource.pitch = Mathf.Clamp(audioSource.pitch, 0, maxPitch);
                //aumenta a rotação da hélice
                helice.rpm += Time.deltaTime;
                break;

            case Acceleration.desacelerating:
                baseSpeed -= desacelerateBoost * Time.deltaTime;
                //faz com que o pitch diminua, diminuindo a aceleração
                audioSource.pitch -= pitchDecreaseSpeed * Time.deltaTime;
                audioSource.pitch = Mathf.Clamp(audioSource.pitch, 0.3f, maxPitch);
                helice.rpm -= 1.5f * Time.deltaTime;
                break;
        }

        //impede que a velocidade passe da velocidade máxima
        speed = Mathf.Clamp(speed, 0, maxSpeed);
    }


    void MotorController()
    {
        //give the player some foward velocity
        Vector3 moveVector = transform.forward * baseSpeed * Time.deltaTime;
        //gather player's input
        float inputY = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        //get the delta direction
        Vector3 yaw = inputX * transform.right * rotSpeedX * Time.deltaTime;
        Vector3 pitch = inputY * transform.up * -rotSpeedY * Time.deltaTime;
        Vector3 dir = yaw + pitch;

        //make sure we limit the player from doing a loop
        float maxX = Quaternion.LookRotation(moveVector + dir).eulerAngles.x;
        //if hes not going too far up/dow, add the direction to the moveVector
        if (maxX < 90 && maxX > 70 || maxX > 270 && maxX < 290)
        {
            //too far, dont do anything
        }
        else
        {
            //add the direction to the current move
            moveVector += dir;
            //have the player face where hes going
            transform.rotation = Quaternion.LookRotation(moveVector);
        }

        //move him 
        controller.Move(moveVector * Time.deltaTime);
    }
}
