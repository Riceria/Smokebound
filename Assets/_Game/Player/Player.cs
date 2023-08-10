using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // this field will contain the actions wrapper instance
    PlayerControls controls;
    public PlayerData playerData;

    void Awake()
    {
        // instantiate the actions wrapper class
        controls = new PlayerControls();

        // add a callback method for when gameplay actions are performed
        controls.gameplay.jump.performed += OnJump;
        controls.gameplay.move.performed += OnMove;
    }

    void Start()
    {
        Debug.Log("started");
    }

    void Update()
    {
        Vector2 moveVector = controls.gameplay.move.ReadValue<Vector2>();
        this.transform.Translate(moveVector * (Time.deltaTime * 3));
        playerData.currentPosition = transform.position;

    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // this is the "jump" action callback method
        //Debug.Log("Jump!");
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
