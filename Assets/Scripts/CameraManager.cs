
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private float dampVelocity;
    public Camera myCamera;
    protected override void Awake()
    {
        
    }

    private void Start()
    {
        myCamera = GetComponent<Camera>();
    }

    public void AdjustCamera(int width, int height)
    {
        transform.position = new Vector3(width * 0.5f, height+height/3, 0.5f);
        myCamera.orthographicSize = width;
        //Mathf.SmoothDamp(myCamera.orthographicSize,width,ref dampVelocity,0.03f);
    }
    // damp ekle camera size kısmına dotweenede bak
    // iki tane leveli yapsın sonra objeler sağa kaysın kayma bitince kamera adjust olsun
    // bu next level animasyonu olur
}
