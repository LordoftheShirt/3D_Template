using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroUnitBase : UnitBase
{
    private bool _canMove = true;
    private bool _isAiming = false;

    // these subscribe the 'OnStateChanged' method to the OnBeforeStateChanged event. Whenever it triggers, so will the OnStateChanged method (I presume). 
    private void Awake()
    {
        ExampleGameManager.OnBeforeStateChanged += OnStateChanged;
    } 



    private void Start()
    {
        SetStats(ResourceSystem.Instance.GetExampleHero(0).BaseStats);
        _canMove = true;
        _isAiming = false;
    }

    private void FixedUpdate()
    {
        Movement();
    }


    private void Movement()
    {
        if (_canMove)
        {

        }
    }

    private void OnDestroy() => ExampleGameManager.OnBeforeStateChanged -= OnStateChanged;

    private void OnStateChanged(GameState newState)
    {
        // turn based example.
        if (newState == GameState.PlayerDeath)
        {
            _canMove = false;
        }
        else
        {
            _canMove = true;
        } 
    }
    //public void RightJoyStick(InputAction.CallbackContext ctx) => rightJoystick = ctx.ReadValue<Vector2>();

    //public void LeftJoyStick(InputAction.CallbackContext ctx1) => leftJoystick = ctx1.ReadValue<Vector2>();

    public void Aim(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            if (_isAiming)
            {
                _isAiming = false;
                Debug.Log("IsAiming OFF");
            }
            else
            {
                _isAiming = true;
                Debug.Log("IsAiming ON");
            }
        } 
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("Hold?");
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

    }




}
