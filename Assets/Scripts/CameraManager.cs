using DG.Tweening;
using UnityEngine;
public class CameraManager : Singleton<CameraManager>
{
    private float dampVelocity;
    public Camera myCamera;
    protected override void Awake()
    {
        myCamera ??= GetComponent<Camera>();
    }
    public void AdjustCamera(int width, int height)
    {
        transform.DOMove(new Vector3(width * 0.5f, height + height / 3, 0.5f), 0.5f);
        myCamera.DOOrthoSize(width, 2);
    }
}
