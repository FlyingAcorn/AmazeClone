using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool isMoving;
    private float movementX;
    private float movementZ;
    [SerializeField] private int ballSpeed;
    private Vector3 lastFloorPos;
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
#if UNITY_EDITOR
        if (!isMoving)
        {
            if (!Input.GetButtonDown("Horizontal") && !Input.GetButtonDown("Vertical")) return;
            movementX = Input.GetAxisRaw("Horizontal");
            movementZ = Input.GetAxisRaw("Vertical");
            transform.localScale = Input.GetButtonDown("Vertical") ?
                new Vector3( 0.7f, 1,1) : new Vector3( 1, 1,0.7f);
            isMoving = true;
        }
        else
        {
            var movementDirection = new Vector3(movementX, 0, movementZ);
            transform.position += movementDirection * (Time.deltaTime * ballSpeed);
        }
#endif
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Block _))
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = lastFloorPos;
            isMoving = false;
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.TryGetComponent(out Floor floor))
        {
            var floorPos = floor.transform.position;
            floor.gameObject.TryGetComponent(out Renderer component);
            component.material.color = Color.blue;
            lastFloorPos = new Vector3(floorPos.x, floorPos.y+0.5f,floorPos.z);
        }
    }
}
