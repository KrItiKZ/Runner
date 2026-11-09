using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Three horizontal lanes: bottom / middle / top")]
    [SerializeField] private float bottomLaneY = -2.9f;
    [SerializeField] private float middleLaneY = -1.15f;
    [SerializeField] private float topLaneY = 0.6f;
    [SerializeField] private int startingLane = 1;
    [SerializeField] private float laneChangeSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.25f;
    [SerializeField] private float jumpDuration = 0.58f;

    [Header("Input")]
    [SerializeField] private bool useKeyboard = true;

    public int CurrentLane { get; private set; }
    public float CurrentJumpHeight { get; private set; }
    public bool IsJumping => CurrentJumpHeight > 0.05f;

    private Rigidbody2D body;
    private GameManager gameManager;
    private bool canControl;
    private float currentLaneY;
    private float jumpTimer;
    private float fixedX;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        gameManager = FindAnyObjectByType<GameManager>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        CurrentLane = Mathf.Clamp(startingLane, 0, 2);
        currentLaneY = GetLaneY(CurrentLane);
        fixedX = body.position.x;
        body.position = new Vector2(fixedX, currentLaneY);
    }

    private void Update()
    {
        if (!canControl || gameManager == null || gameManager.State != GameManager.GameState.Running) return;

        if (useKeyboard)
            ReadKeyboard();

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(jumpTimer / jumpDuration);
            CurrentJumpHeight = 4f * jumpHeight * t * (1f - t);
        }
        else
        {
            jumpTimer = 0f;
            CurrentJumpHeight = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (body == null) return;

        float targetY = GetLaneY(CurrentLane);
        currentLaneY = Mathf.MoveTowards(currentLaneY, targetY, laneChangeSpeed * Time.fixedDeltaTime);
        body.MovePosition(new Vector2(fixedX, currentLaneY + CurrentJumpHeight));
    }

    private void ReadKeyboard()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null) return;
        if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            MoveLaneUp();
        if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
            MoveLaneDown();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Jump();
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) MoveLaneUp();
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) MoveLaneDown();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
#endif
    }

    public void MoveLaneUp()
    {
        if (!canControl) return;
        CurrentLane = Mathf.Min(2, CurrentLane + 1);
    }

    public void MoveLaneDown()
    {
        if (!canControl) return;
        CurrentLane = Mathf.Max(0, CurrentLane - 1);
    }

    public void Jump()
    {
        if (!canControl || jumpTimer > 0f) return;
        jumpTimer = jumpDuration;
    }

    public void SetCanControl(bool value)
    {
        canControl = value;
        if (!value)
        {
            jumpTimer = 0f;
            CurrentJumpHeight = 0f;
        }
    }

    private float GetLaneY(int lane)
    {
        switch (lane)
        {
            case 0: return bottomLaneY;
            case 2: return topLaneY;
            default: return middleLaneY;
        }
    }
}
