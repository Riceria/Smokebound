using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    PlayerControls controls;
    public PlayerData playerData;
    public LayerMask solidObjectLayer;
    public Tilemap currentTilemap;
    private Vector3[] bounds;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isMoving;
    private Vector2 moveVector;
    public float moveSpeed = 5f;

    void Awake()
    {
        // instantiate the actions wrapper class
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bounds = new Vector3[2];
        playerData.isAttacked = false;

        // add callback methods for when gameplay actions are performed
        controls.gameplay.move.performed += OnMove;
    }

    void Start()
    {
        Debug.Log("started");
        currentTilemap.CompressBounds();
    }

    void FixedUpdate()
    {
        if (!isMoving && !playerData.isAttacked)
        {
            moveVector = controls.gameplay.move.ReadValue<Vector2>();

            // remove diagonal movement
            if (moveVector.x != 0) {
                moveVector.y = 0;
            }

            // overworld walking animations
            if (moveVector != Vector2.zero)
            {
                animator.SetFloat("Horizontal", moveVector.x);
                animator.SetFloat("Vertical", moveVector.y);

                if (moveVector.x == 1 || moveVector.x == -1 ||
                    moveVector.y == 1 || moveVector.y == -1)
                {
                    animator.SetFloat("Last_Horizontal", moveVector.x);
                    animator.SetFloat("Last_Vertical", moveVector.y);
                }

                var targetPos = rb.position;
                if (moveVector.x == 1 || moveVector.x == -1) {
                    targetPos.x += moveVector.x;
                }
                if (moveVector.y == 1 || moveVector.y == -1) {
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
        while ((targetpos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            rb.position = Vector3.MoveTowards(rb.position, targetpos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        rb.MovePosition(targetpos);
        isMoving = false;
        
    }

    private bool IsWalkable(Vector3 targetpos)
    {
        float x_offset = 0.5f, y_offset = 0.5f;
        bounds[0] = currentTilemap.LocalToWorld(currentTilemap.localBounds.min);
        bounds[1] = currentTilemap.LocalToWorld(currentTilemap.localBounds.max);

        // stay within tilemap boundary
        if (targetpos.x < (bounds[0].x + x_offset) || targetpos.x > (bounds[1].x - x_offset)||
            targetpos.y < bounds[0].y || targetpos.y > (bounds[1].y - y_offset)) {
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
