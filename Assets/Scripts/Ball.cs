using System;
using UnityEngine;
public class Ball : MonoBehaviour
{
    private bool isMoving;
    private float movementX;
    private float movementZ;
    [SerializeField] private int ballSpeed;
    private Vector3 lastFloorPos;
    private Vector3 startTouchPos;
    private Vector3 movementDirection;

    private void Start()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }
    private void GameManagerOnOnGameStateChanged(GameManager.GameState state)
    {

        if (state == GameManager.GameState.Victory)
        {
            movementDirection = Vector3.zero;
        }
    }

    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        if (!isMoving)
        {
           TouchInput();
#if UNITY_EDITOR
            KeyInput();
#endif
        }
        else
        {
            transform.position += movementDirection * (Time.deltaTime * ballSpeed);
        }
    }
    private void TouchInput()
    {
        if (Input.touchCount !=1) return;
        var touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            startTouchPos = new Vector3(touch.position.x, 0,touch.position.y);
        }

        if (touch.phase == TouchPhase.Ended)
        {
            var endTouchPos = new Vector3(touch.position.x, 0, touch.position.y);
            var swipeDirection = endTouchPos - startTouchPos;
            Debug.Log(swipeDirection);
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.z))
            {
                movementDirection = swipeDirection.x >0 ? Vector3.right : Vector3.left;
                transform.localScale =new Vector3( 1, 1,0.7f);
            }
            else
            {
                movementDirection = swipeDirection.z >0 ? new Vector3(0,0,1) : new Vector3(0,0,-1);
                transform.localScale = new Vector3(0.7f, 1, 1);
            }
            isMoving = true;
        }
    }
#if UNITY_EDITOR
    private void KeyInput()
    {
        if (!Input.GetButtonDown("Horizontal") && !Input.GetButtonDown("Vertical")) return;
        movementX = Input.GetAxisRaw("Horizontal");
        movementZ = Input.GetAxisRaw("Vertical");
        transform.localScale = Input.GetButtonDown("Vertical") ?
            new Vector3( 0.7f, 1,1) : new Vector3( 1, 1,0.7f);
        isMoving = true;
        movementDirection = new Vector3(movementX, 0, movementZ).normalized;
    }
#endif

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Block _))
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = lastFloorPos;
            isMoving = false;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.TryGetComponent(out Floor floor))
        {
            var floorPos = floor.transform.position;
            floor.OnTouched();
            lastFloorPos = new Vector3(floorPos.x, floorPos.y+0.5f,floorPos.z);
            
        }
    }
}
