using System;
using System.Collections;
using Unity.VisualScripting;

// using System.Numerics;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    PlayerControls controls;
    public CharacterStatus playerStatus;
    public TilemapManager tilemapManager;
    [SerializeField] private TilemapTransitionTrigger transitionTrigger;
    public LayerMask solidObjectLayer;
    public LayerMask transitionLayer; 
    // [SerializeField] private Tilemap currentTilemap;
    private Vector3[] bounds;
    private Animator animator;
    // private Rigidbody2D rb;
    private bool isMoving, wasMovingVertical;
    private Vector2 moveVector;
    public float moveSpeed = 5f;

    // from tutorial
    [SerializeField] private float moveDuration;
    [SerializeField] private float gridSize;

    void Awake()
    {
        // instantiate the actions wrapper class & component references
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        bounds = new Vector3[2];
        moveDuration = 0.15f;
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

                // var targetPos = rb.position;
                var targetPos = transform.position;
                if (Mathf.Abs(moveVector.x) == 1) {
                    targetPos.x += moveVector.x;
                }
                if (Mathf.Abs(moveVector.y) == 1) {
                    targetPos.y += moveVector.y;
                }

                if (AreaTransition(targetPos) && !playerStatus.isTeleporting) {
                    StartCoroutine(Teleport(targetPos));
                } else if (IsWalkable(targetPos)) {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);
        // Debug.Log("isTeleporting: " + playerStatus.isTeleporting);
    }

    IEnumerator Move(Vector3 targetpos)
    {
        isMoving = true;
        playerStatus.isMoving = true;
        Vector3 startPos = transform.position;
        //Vector3 endPos = startPos + (direction * gridSize);

        float elapsedTime = 0;
        while (elapsedTime < moveDuration) {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetpos, percent);

            Vector2 direction = targetpos - startPos;
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction.normalized, direction.magnitude, solidObjectLayer);
            if (hit.collider != null) {
                // Obstacle detected, stop movement
                Debug.Log("Move(): Ray hit, stopping movement");
                transform.position = startPos;
                isMoving = false;
                yield break;
            }

            // transform.position = targetpos;
            yield return null;
        }

        transform.position = targetpos;
        tilemapManager.UpdatePlayerPosition(transform.position);
        isMoving = false;
        playerStatus.isMoving = false;
    }

    IEnumerator Teleport(Vector3 targetpos)
    {
        if (playerStatus.isTeleporting) {
            yield break;
        }
    
        Debug.Log("Enter Teleport()");
        isMoving = true;
        playerStatus.isMoving = true;
        Vector3 startPos = transform.position;

        float elapsedTime = 0;
        while (elapsedTime < moveDuration) {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPos, targetpos, percent);
            yield return null;
        }

        transform.position = targetpos;
        isMoving = false;
        playerStatus.isMoving = false;
        tilemapManager.UpdatePlayerPosition(transform.position);
        transitionTrigger.TransitionToTilemap(transitionTrigger.GetDestinationTilemap());
        yield return new WaitForSeconds(1);
        playerStatus.isTeleporting = false;
    }

    private bool IsWalkable(Vector3 targetpos)
    {
        Tilemap currentTilemap = tilemapManager.GetCurrentTilemap();

        if (currentTilemap == null)
        {
            Debug.Log("IsWalkable(): No currentTilemap found");
            return false;
        }

        BoundsInt bounds = currentTilemap.cellBounds;
        Vector3 tilemapCenter = currentTilemap.transform.position + new Vector3(bounds.center.x, bounds.center.y, 0f);

        // Check if the target position is within the tilemap boundaries
        if (!bounds.Contains(currentTilemap.WorldToCell(targetpos)) && !playerStatus.isTeleporting)
        {
            Debug.Log("IsWalkable(): Player out of currentTilemap bounds");
            return false;
        }

        // Check for obstacles using raycasting
        Vector2 direction = targetpos - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, solidObjectLayer);
        if (hit.collider != null)
        {
            Debug.Log("IsWalkable(): Obstacle detected by raycast, cannot move");
            return false;
        }

        return true;
    }

    private bool AreaTransition(Vector3 targetpos)
    {
        // Check for obstacles using raycasting
        Vector2 direction = targetpos - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, direction.magnitude, transitionLayer);
        if (hit.collider != null && !playerStatus.isTeleporting)
        {
            Debug.Log("AreaTransition(): Transition layer detected, beginning transition");
            transitionTrigger = hit.collider.GetComponentInParent<TilemapTransitionTrigger>();
            return true;
        }

        return false;
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
