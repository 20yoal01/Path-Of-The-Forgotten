using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*

UNPAUSE YOUTUBE VIDEO AT 12:45 ("How to make a Camera System (Like Hollow Knights))

*/
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshhold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }


    private Coroutine _lerpYPanCoroutine;

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private float _normYPanAmount;

    [SerializeField] private float BowlerpSpeed = 2f; // Speed of the lerp movement
    [SerializeField] private float BowmaxOffset = 1.5f; // Maximum offset allowed
    private Coroutine adjustCameraCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                _currentCamera = _allVirtualCameras[i];
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        _normYPanAmount = _framingTransposer.m_YDamping;
    }

    #region Bow Camera Offset

    /// <summary>
    /// Adjusts the camera to focus in the direction of the bow charge.
    /// </summary>
    /// <param name="direction">The direction the player is aiming, typically a Vector2.</param>
    public void AdjustCameraForBowCharge(bool diagonal)
    {
        // Stop any previous camera adjustment
        if (adjustCameraCoroutine != null)
        {
            StopCoroutine(adjustCameraCoroutine);
        }

        Vector2 direction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().IsFacingRight ? Vector2.right : Vector2.right * -1;

        Vector2 targetOffset;

        if (diagonal)
        {
            float isFacingRight = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().IsFacingRight ? 1 : -1;
            targetOffset = new Vector2(1, 1).normalized * BowmaxOffset;
            targetOffset.x = targetOffset.x * isFacingRight;
        }
        else
        {
            targetOffset = new Vector2(direction.x * BowmaxOffset, 0);
        }
        adjustCameraCoroutine = StartCoroutine(LerpCameraOffset(targetOffset));
    }

    /// <summary>
    /// Resets the camera to its default position after the bow charge ends.
    /// </summary>
    public void ResetCameraOffset()
    {
        // Stop any previous camera adjustment
        if (adjustCameraCoroutine != null)
        {
            StopCoroutine(adjustCameraCoroutine);
        }

        // Start the coroutine to reset the camera
        adjustCameraCoroutine = StartCoroutine(LerpCameraOffset(Vector2.zero));
    }

    /// <summary>
    /// Lerps the camera's offset smoothly.
    /// </summary>
    /// <param name="targetOffset">The target offset to move towards.</param>
    private IEnumerator LerpCameraOffset(Vector2 targetOffset)
    {
        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        Vector2 currentOffset = new Vector2(_framingTransposer.m_TrackedObjectOffset.x,
                                            _framingTransposer.m_TrackedObjectOffset.y);

        while (Vector2.Distance(currentOffset, targetOffset) > 0.01f)
        {
            currentOffset = Vector2.Lerp(currentOffset, targetOffset, BowlerpSpeed * Time.deltaTime);
            _framingTransposer.m_TrackedObjectOffset = new Vector3(currentOffset.x, currentOffset.y, 0);
            yield return null; // Wait for the next frame
        }

        // Ensure the final offset matches the target
        _framingTransposer.m_TrackedObjectOffset = new Vector3(targetOffset.x, targetOffset.y, 0);
    }

    #endregion

    #region Lerp the Y Damping

    public void LerpYDaming(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region Swap Cameras

    public void SwapCamera(CinemachineVirtualCamera oldCamera, CinemachineVirtualCamera newCamera)
    {
        oldCamera.enabled = false;
        newCamera.enabled = true;

        _currentCamera = newCamera;

        _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    #endregion
}
