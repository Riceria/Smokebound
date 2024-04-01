using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    PlayerControls controls;
    public CharacterStatus playerStatus;
    public TilemapManager tilemapManager;
    public LayerMask solidObjectLayer;
    // [SerializeField] private Tilemap currentTilemap;
    private Vector3[] bounds;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isMoving, wasMovingVertical;
    private Vector2 moveVector;
    public float moveSpeed = 5f;

    void Awake()
    {
        // instantiate the actions wrapper class & component references
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bounds = new Vector3[2];
        playerStatus.isAttacked = false;
        playerStatus.isTeleporting = false;
        playerStatus.currentPosition.x = 0.5f;
        playerStatus.currentPosition.y = 0.3f;

        // add callback methods for when gameplay actions are performed
        controls.gameplay.move.performed += OnMove;
    }

    void Start()
    {
        Debug.Log("started");
        playerStatus.characterGameObject = gameObject;
        // currentTilemap.CompressBounds();
        // playerStatus.currentPosition = currentTilemap.cellBounds.center;
        // rb.position = currentTilemap.cellBounds.center;
    }

    void Update()
    {
        moveVector = controls.gameplay.move.ReadValue<Vector2>();
        bool isMovingHorizontal = Mathf.Abs(moveVector.x) > 0.5f;
        bool isMovingVertical = Mathf.Abs(moveVector.y) > 0.5f;
        
        // Disable diagonal movement & prioritize direction of last key pressed
        if (isMovingVertical && isMovingHorizontal)
        {
            // Round normalized vector to 1 or -1
            if (wasMovingVertical) {
                moveVector.y = 0;
                moveVector.x = Mathf.Round(moveVector.x);
            } else {
                moveVector.x = 0;
                moveVector.y = Mathf.Round(moveVector.y);
            }
        }
        else if (isMovingHorizontal)
        {
            moveVector.y = 0;
            wasMovingVertical = false;
        }
        else if (isMovingVertical)
        {
            moveVector.x = 0;
            wasMovingVertical = true;
        }
    }

    void FixedUpdate()
    {
        if (!isMoving && !playerStatus.isAttacked)
        {
            // Overworld walking animations
            if (moveVector != Vector2.zero)
            {
                animator.SetFloat("Horizontal", moveVector.x);
                animator.SetFloat("Vertical", moveVector.y);

                if (Mathf.Abs(moveVector.x) == 1 || 
                    Mathf.Abs(moveVector.y) == 1)
                {
                    animator.SetFloat("Last_Horizontal", moveVector.x);
                    animator.SetFloat("Last_Vertical", moveVector.y);
                }

                var targetPos = rb.position;
                if (Mathf.Abs(moveVector.x) == 1) {
                    targetPos.x += moveVector.x;
                }
                if (Mathf.Abs(moveVector.y) == 1) {
                    targetPos.y += moveVector.y;
                }

                if (IsWalkable(targetPos)) {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetpos)
    {
        isMoving = true;
        playerStatus.isMoving = true;
        while ((targetpos - transform.position).sqrMagnitude > Mathf.Epsilon && !playerStatus.isTeleporting)
        {
            rb.position = Vector3.MoveTowards(rb.position, targetpos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        if (!playerStatus.isTeleporting) {
            rb.MovePosition(targetpos);
            tilemapManager.UpdatePlayerPosition(rb.position);
        }
        Debug.Log(rb.position);

        isMoving = false;
        playerStatus.isMoving = false;
        
    }

    private bool IsWalkable(Vector3 targetpos)
    {
        float x_offset = 0.5f, y_offset = 0.5f;
        Tilemap currentTilemap = tilemapManager.GetCurrentTilemap();

        if (currentTilemap == null) {
            return false;
        }
        // bounds[0] = currentTilemap.LocalToWorld(currentTilemap.localBounds.min);
        // bounds[1] = currentTilemap.LocalToWorld(currentTilemap.localBounds.max);

        // Debug.Log("bounds[0]: " + bounds[0] + " | bounds[1]: " + bounds[1]);

        // stay within tilemap boundary
        // if (targetpos.x < (bounds[0].x + x_offset) || targetpos.x > (bounds[1].x - x_offset)||
        //     targetpos.y < bounds[0].y || targetpos.y > (bounds[1].y - y_offset)) {
        //     return false;
        // }

        BoundsInt bounds = currentTilemap.cellBounds;

        // stay within tilemap boundary
        if (targetpos.x < (bounds.min.x + x_offset) || targetpos.x > (bounds.max.x - x_offset) ||
            targetpos.y < (bounds.min.y + y_offset) || targetpos.y > (bounds.max.y - y_offset))
        {
            return false;
        }
        if (Physics2D.OverlapCircle(targetpos, 0.2f, solidObjectLayer) != null) {
            return false;
        }

        return true;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // this is the "move" action callback method
        //Debug.Log(context);
    }
    void OnEnable()
    {
        controls.gameplay.Enable();
    }

    void OnDisable()
    {
        controls.gameplay.Disable();
    }
}
