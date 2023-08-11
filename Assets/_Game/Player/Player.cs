using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PlayerControls controls;
    public PlayerData playerData;
    private Animator animator;
    private bool isMoving;
    private Vector2 moveVector;
    public Rigidbody2D rb;
    public float moveSpeed = 5f;

    private void Awake()
    {
        // instantiate the actions wrapper class
        controls = new PlayerControls();

        animator = GetComponent<Animator>();

        // add callback methods for when gameplay actions are performed
        controls.gameplay.move.performed += OnMove;
    }

    private void Start()
    {
        Debug.Log("started");
    }

    private void Update()
    {
        if (!isMoving && !playerData.isAttacked)
        {
            moveVector = controls.gameplay.move.ReadValue<Vector2>();

            // remove diagonal movement
            if (moveVector.x != 0) {
                moveVector.y = 0;
            }

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

                var targetPos = transform.position;
                targetPos.x += moveVector.x;
                targetPos.y += moveVector.y;

                StartCoroutine(Move(targetPos));
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetpos)
    {
        isMoving = true;
        while ((targetpos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetpos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetpos;
        isMoving = false;
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
