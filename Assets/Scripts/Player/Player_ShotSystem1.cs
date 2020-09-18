using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Player_ShotSystem1 : MonoBehaviour
{
    ///<summary>
    //este script dará conta do sistema de tiro do avião
    //sistema de tiros por prefabs e rigidbodies
    ///<sumary>

    [Header("Prefabs")]
    public Transform pfBullet;

    [Header("Lógica")]
    public int gunMinDamage = 2;
    public int gunMaxDamage = 3;
    private int actualDamage;
    public LayerMask hitMask;
    public float fireRate = 0.3f;
    public int magazine = 200;
    private int currentMagazine;
    public int MaxMagazine = 9999;
    public int munition = 100;
    private Transform crosshair;
    private Transform gun;
    public Transform[] gunShotPos;
    private SpriteRenderer _sprite;
    public bool canShot = true;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.7f);
    [Header("UI")]
    private TextMeshProUGUI magazineCountUI;
    private string magazineCounts;
    public bool mirar = false;

    [Header("Animação e FX")]
    public GameObject muzzle;
    public GameObject smoke;
    public GameObject hitFX;


    [Header("Audio")]
    public string shotSFXname;
    public AudioClip shotSFX;
    public AudioClip eptMagazine;
    public AudioClip reloadSFX;
    private float nextFire;
    private Vector3 target;
    private LineRenderer rastroTiro;

    [Header("Camera Shake")]
    public float duration = 0.1f;
    public float strength = 0.4f;
    public int vibrato = 30;
    public float randomness = 60;
    public bool fadeOut;

    private Flightcamera_Controller _flightCamera;

    private void Start()
    {

        // magazineCountUI = GameObject.Find("Magazine_txt").GetComponent<TextMeshProUGUI>();
        // UpdateUI();
        // gun = GetComponent<Transform>();
        // rastroTiro = GetComponent<LineRenderer>();
        // gunAudio = GetComponent<AudioSource>();
        magazine = MaxMagazine;
        _flightCamera = FindObjectOfType<Flightcamera_Controller>();
    }
    private void Update()
    {
        //target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Mirar();
        Shot();
        Reload();
    }

    private IEnumerator MagazineAFX()
    {
        //efeito sonoro quando recarrega
        yield return new WaitForSeconds(0.1f);

    }
    private void ShotProjectile()
    {

        // Vector3 pos = transform.position;
        // float distance = speed * deltaTime;
        // if (Physics.Raycast(pos, transform.forward, out hit, distance, ~0, QueryTriggerInteraction.Ignore)
        //     {
        //     transform.position = hit.point;
        //     DoCollision();
        //     return;
        // }
        // trasnform.position += transform.forward * speed * deltaTime;

    }
    private void Shot()
    {
        //TODO: fazer o sistema de colisão com raycast e movimento 
        if (Input.GetMouseButton(0) && Time.time > nextFire && magazine > 0 && canShot == true)
        {

            magazine--;
            currentMagazine++;
            UpdateUI();
            nextFire = Time.time + fireRate;

            StartCoroutine(shotFX());

            // if (hit)
            // {
            //     instancia um efeito de impacto onde o tiro atingir
            //     Instantiate(hitFX, hit.point, Quaternion.identity);
            //     actualDamage = Random.Range(gunMinDamage, gunMaxDamage);

            //     rastroTiro.SetPosition(1, hit.point);
            // }
            // if (hit.rigidbody != null)
            // {
            //     hit.rigidbody.AddForce(-hit.normal * hitForce);
            // }

        }


        if (Input.GetMouseButtonDown(0) && magazine <= 0)
        {
            AudioManager.instance.Play("EmpytMagazine");
        }
        // if (Input.GetMouseButtonUp(0))
        // {
        //     if (smoke != null)
        //     {
        //         float chance = Random.value;
        //         if (chance < 0.3)
        //         {
        //             Instantiate(smoke, gunShotPos.position, Quaternion.identity, gunShotPos);
        //         }
        //     }
        // }
    }
    private void Mirar()
    {
        //TODO: puxar a mira do camera controller
        if (Input.GetMouseButton(1))
        { _flightCamera.Mirar(); }
        else if (Input.GetMouseButtonUp(1))
        {
            _flightCamera.ResetShotCamera();
        }
    }
    private IEnumerator shotFX()
    {
        var clone = Instantiate(pfBullet, gunShotPos[Random.Range(0, gunShotPos.Length)].position, gunShotPos[0].rotation);
        yield return null;
        //efeito sonoro e visual quando se atira        
        GunAudio();
        var muzzleClone = Instantiate(muzzle,
        gunShotPos[Random.Range(0, gunShotPos.Length)].position, gunShotPos[0].rotation);
        // Destroy(muzzleClone, 1);
        ShakeCamera();

        // yield return shotDuration;
    }
    void GunAudio()
    {
        AudioManager.instance.Play(shotSFXname);
    }
    private void ShakeCamera()
    {
        FlighCameraShake_Manager.instance.CustomShake(duration, strength, vibrato,
        randomness, fadeOut);
    }
    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && munition > 0 && magazine < MaxMagazine) //recarrega a arma e atualiza a UI
        {
            munition -= currentMagazine;
            magazine = MaxMagazine;
            currentMagazine = 0;
            StartCoroutine(ReloadFX());
            UpdateUI();
        }
    }
    IEnumerator ReloadFX()
    {
        AudioManager.instance.Play("ReloadSFX");
        canShot = false;
        yield return new WaitForSeconds(1.3f);
        canShot = true;
    }
    private void UpdateUI()
    {
        Gameui_Manager.instance.BulletCounter(magazine);

    }
}

