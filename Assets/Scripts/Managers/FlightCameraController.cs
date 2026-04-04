using UnityEngine;
using DG.Tweening;

/// <summary>
/// Gerencia a câmera do flight rig via FOV.
/// Uma única câmera por jogador — essencial para o split screen funcionar corretamente.
///
///  • Mira  : reduz o FOV para dar sensação de zoom.
///  • Boost : aumenta o FOV para sensação de velocidade.
/// </summary>
public class FlightCameraController : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    #region Serialized Fields

    [Header("Camera")]
    [Tooltip("Câmera única do flight rig.")]
    [SerializeField] private Camera flightCamera;

    [Header("Aim FOV")]
    [Tooltip("FOV ao mirar (zoom in).")]
    [SerializeField] private float aimFov = 35f;

    [Tooltip("Duração da transição de mira.")]
    [SerializeField] private float aimTransitionTime = 0.3f;

    [Tooltip("Ease da transição de mira.")]
    [SerializeField] private Ease aimTransitionEase = Ease.OutQuad;

    [Header("Boost FOV")]
    [Tooltip("FOV durante o boost (zoom out).")]
    [SerializeField] private float boostFov = 75f;

    [Tooltip("Duração da transição de boost.")]
    [SerializeField] private float boostLerpTime = 0.4f;

    #endregion

    // ─────────────────────────────────────────────────────────────
    #region Public API — Camera Effects

    /// <summary>
    /// Aplica um tremor de câmera (Shake) via Rotação.
    /// Útil para tiros, explosões ou tontura ao levar dano.
    /// </summary>
    /// <param name="duration">Duração em segundos.</param>
    /// <param name="strength">Força/Intensidade da rotação.</param>
    /// <param name="vibrato">Frequência do tremor.</param>
    public void Shake(float duration, float strength, int vibrato = 10)
    {
        if (flightCamera == null) return;

        // Interrompe qualquer shake anterior para não acumular estranhamente
        flightCamera.transform.DOComplete();
        flightCamera.transform.DOShakeRotation(duration, strength, vibrato);
    }

    #endregion

    // ─────────────────────────────────────────────────────────────
    #region Private State

    private float _defaultFov;
    private bool  _isAiming;

    #endregion

    // ─────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        if (flightCamera != null)
            _defaultFov = flightCamera.fieldOfView;
    }

    #endregion

    // ─────────────────────────────────────────────────────────────
    #region Public API — Aim

    /// <summary>
    /// Reduz o FOV para simular zoom de mira.
    /// </summary>
    public void StartAim()
    {
        if (_isAiming || flightCamera == null) return;
        _isAiming = true;

        flightCamera.DOKill();
        flightCamera.DOFieldOfView(aimFov, aimTransitionTime).SetEase(aimTransitionEase);
    }

    /// <summary>
    /// Restaura o FOV padrão ao sair da mira.
    /// </summary>
    public void StopAim()
    {
        if (!_isAiming || flightCamera == null) return;
        _isAiming = false;

        flightCamera.DOKill();
        flightCamera.DOFieldOfView(_defaultFov, aimTransitionTime).SetEase(aimTransitionEase);
    }

    #endregion

    // ─────────────────────────────────────────────────────────────
    #region Public API — Boost FOV

    /// <summary>
    /// Aumenta o FOV para sensação de velocidade (ignorado durante a mira).
    /// </summary>
    public void ApplyBoostFov()
    {
        if (_isAiming || flightCamera == null) return;

        flightCamera.DOKill();
        flightCamera.DOFieldOfView(boostFov, boostLerpTime).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// Restaura o FOV padrão após o boost (ignorado durante a mira).
    /// </summary>
    public void ResetFlightFov()
    {
        if (_isAiming || flightCamera == null) return;

        flightCamera.DOKill();
        flightCamera.DOFieldOfView(_defaultFov, boostLerpTime).SetEase(Ease.InCubic);
    }

    #endregion
}
