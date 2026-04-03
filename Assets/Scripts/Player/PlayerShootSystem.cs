using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerShootSystem : MonoBehaviour
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

  public FlightCameraController _flightCamera;

  public PlayerInput input;

  private void Start()
  {
    if (input != null)
    {
      Setup(input, _flightCamera);
    }
    else
    {
      Debug.LogWarning("PlayerShootSystem: No PlayerInput assigned. Please call Setup() with a valid PlayerInput.");
    }

    Airplane airplane = GetComponent<Airplane>();
    if (airplane != null)
    {
      // _flightCamera = airplane.cameraController;
    }

    if (_flightCamera == null)
      _flightCamera = GetComponent<FlightCameraController>();

    magazine = MaxMagazine;
  }

  public void Setup(PlayerInput playerInput, FlightCameraController flightCamera)
  {
    input = playerInput;
    _flightCamera = flightCamera;

    // Inscreve os eventos de input
    if (input != null && input.actions != null)
    {
      input.actions["ShootPrimary"].performed += OnShootStarted;
      input.actions["ShootPrimary"].canceled += OnShootCanceled;

      input.actions["Reload"].performed += OnReloadPerformed;

      input.actions["Sight"].performed += OnSightStarted;
      input.actions["Sight"].canceled += OnSightCanceled;
    }
  }



  private void OnDisable()
  {
    if (input != null && input.actions != null)
    {
      input.actions["ShootPrimary"].performed -= OnShootStarted;
      input.actions["ShootPrimary"].canceled -= OnShootCanceled;

      input.actions["Reload"].performed -= OnReloadPerformed;

      input.actions["Sight"].performed -= OnSightStarted;
      input.actions["Sight"].canceled -= OnSightCanceled;
    }
  }

  private bool isShooting = false;

  private void OnShootStarted(InputAction.CallbackContext context) => isShooting = true;
  private void OnShootCanceled(InputAction.CallbackContext context) => isShooting = false;

  private void OnSightStarted(InputAction.CallbackContext context) => OnSight(true);
  private void OnSightCanceled(InputAction.CallbackContext context) => OnSight(false);

  private void Update()
  {
    if (isShooting)
    {
      HandleShotLogic();
    }
  }

  private void OnReloadPerformed(InputAction.CallbackContext context)
  {
    if (munition > 0 && magazine < MaxMagazine && canShot)
    {
      StartCoroutine(ReloadAction());
    }
  }

  private void OnSight(bool isPressed)
  {
    if (_flightCamera == null) return;

    if (isPressed)
      _flightCamera.StartAim();
    else
      _flightCamera.StopAim();
  }

  private void HandleShotLogic()
  {
    print("Trying to shoot. Can shoot: " + canShot + ", Magazine: " + magazine);
    if (Time.time > nextFire && magazine > 0 && canShot)
    {
      magazine--;
      currentMagazine++;
      UpdateUI();
      nextFire = Time.time + fireRate;
      StartCoroutine(shotFX());
    }
    else if (magazine <= 0)
    {
      AudioManager.instance.Play("EmpytMagazine");
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
    ShootSystemAudioManager.instance.Play(shotSFXname);
  }
  private void ShakeCamera()
  {
    FlightCameraShakeManager.instance.CustomShake(duration, strength, vibrato,
    randomness, fadeOut);
  }
  private void Reload()
  {
    // Método agora vazio ou removido pois o Reload usa evento e Coroutine direta
  }

  private IEnumerator ReloadAction()
  {
    AudioManager.instance.Play("ReloadSFX");
    canShot = false;

    yield return new WaitForSeconds(1.3f);

    munition -= currentMagazine;
    magazine = MaxMagazine;
    currentMagazine = 0;
    canShot = true;
    UpdateUI();
  }

  IEnumerator ReloadFX()
  {
    // Mantido apenas para compatibilidade se outros scripts chamarem, 
    // mas o principal agora é ReloadAction
    yield return ReloadAction();
  }
  private void UpdateUI()
  {
    if (Gameui_Manager.instance != null)
      Gameui_Manager.instance.BulletCounter(magazine);

  }
}

