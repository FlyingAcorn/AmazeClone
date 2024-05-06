using UnityEngine;
public class Ball : MonoBehaviour
{
    [SerializeField]private bool blockInput;
    private float movementX;
    private float movementZ;
    [SerializeField] private int ballSpeed;
    private Vector3 lastFloorPos;
    private Vector3 startTouchPos;
    private Vector3 movementDirection;
    private Rigidbody myBody;
    [SerializeField] private ParticleSystem hitParticle;





    private void Start()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
        myBody = GetComponent<Rigidbody>();
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }
    private void GameManagerOnOnGameStateChanged(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Victory)
        {
            myBody.velocity = Vector3.zero;
            transform.position = lastFloorPos;
            movementDirection = Vector3.zero;
            transform.localScale = Vector3.one;
            blockInput = true;
        }
        if (state == GameManager.GameState.Play)
        {
            movementDirection = Vector3.zero;
            blockInput = false;
            lastFloorPos = transform.position;
        }
    }
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        //Movement kısmına raycast atabilirsin
        //dibindeki bloğa hit oluyorsa o yöne gidemez.
        if (GameManager.Instance.state != GameManager.GameState.Play) return;
        if (!blockInput)
        {
            TouchInput();
#if UNITY_EDITOR
            KeyInput();
#endif
        }
        else
        {
            myBody.velocity = new Vector3(movementDirection.x * ballSpeed, 0,
                movementDirection.z * ballSpeed);
           // transform.position += movementDirection * (Time.deltaTime * ballSpeed);
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

        if (touch.phase != TouchPhase.Ended) return;
        var endTouchPos = new Vector3(touch.position.x, 0, touch.position.y);
        var swipeDirection = endTouchPos - startTouchPos;
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
        blockInput = true;
    }
#if UNITY_EDITOR
    private void KeyInput()
    {
        movementX = Input.GetAxisRaw("Horizontal");
        movementZ = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Horizontal"))
        {
            movementDirection = new Vector3(movementX, 0, 0);
            transform.localScale = new Vector3(1, 1, 0.7f);
            blockInput = true;
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            movementDirection = new Vector3(0, 0, movementZ);
            transform.localScale = new Vector3(0.7f, 1, 1);
            blockInput = true;
        }
    }
#endif

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out Block _ )) return;
        if (GameManager.Instance.state == GameManager.GameState.Play)
        {
            hitParticle.Play();
            transform.localScale = Vector3.one;
            transform.position = lastFloorPos;
        }
        blockInput = false;
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (!trigger.gameObject.TryGetComponent(out Floor floor)) return;
        
        floor.floorParticle.Play();
        var floorPos = floor.transform.position;
        lastFloorPos = new Vector3(floorPos.x, floorPos.y+0.5f,floorPos.z);
        floor.gameObject.TryGetComponent(out Renderer component);
        component.material.color = Color.blue;
        GridManager.Instance.whiteFloors.Remove(floor.gameObject);
        if (!GridManager.Instance.blueFloors.Contains(floor.gameObject))
        {
            GridManager.Instance.blueFloors.Add(floor.gameObject);
        }
        if (GridManager.Instance.whiteFloors.Count != 0) return;
        if (GameManager.Instance.state !=GameManager.GameState.Play) return;
        GameManager.Instance.UpdateGameState(GameManager.GameState.Victory);
    }

   
}
