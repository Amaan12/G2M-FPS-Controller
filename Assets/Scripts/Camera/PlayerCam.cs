using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public Transform orientation; // Player
    public Transform cameraHolder;
    public float sensX, sensY;
    float xRotation, yRotation;
    public float startFOV;

    float recoilX = 0f;
    float recoilY = 0f;
    float recoilRecoverySpeed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startFOV = GetComponent<Camera>().fieldOfView;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;
        yRotation += mouseX;
        xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90, 90);

        // Apply recoil offsets
        float finalXRot = xRotation + recoilX;
        float finalYRot = yRotation + recoilY;
        finalXRot = Mathf.Clamp(finalXRot, -90, 90);


        cameraHolder.rotation = Quaternion.Euler(finalXRot, finalYRot, 0f);
        orientation.rotation = Quaternion.Euler(0, finalYRot, 0f);

        // cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        // Smoothly recover recoil
        recoilX = Mathf.Lerp(recoilX, 0f, Time.deltaTime * recoilRecoverySpeed);
        recoilY = Mathf.Lerp(recoilY, 0f, Time.deltaTime * recoilRecoverySpeed);
    }

    public void DoFOV(float endValue) { GetComponent<Camera>().DOFieldOfView(endValue, 0.25f); }
    public void DoTilt(float zTilt) { transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f); }

    public void ApplyRecoil(float recoilUp, float recoilSide, float recoverySpeed)
    {
        recoilRecoverySpeed = recoverySpeed;
        // Cancel any existing recoil tweens
        DOTween.Kill("RecoilX");
        DOTween.Kill("RecoilY");

        // Smoothly apply recoil as a short punch
        DOTween.To(() => recoilX, x => recoilX = x, recoilX - recoilUp, 0.05f).SetId("RecoilX");
        DOTween.To(() => recoilY, y => recoilY = y, recoilY + Random.Range(-recoilSide, recoilSide), 0.05f).SetId("RecoilY");
    }
}
