using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// This class only listens to inputs. It should never assign their use.
public class InputManager : Singleton<InputManager>
{
    private RTSInput input;

    public delegate void LeftClickEvent(Vector3 position);
    public event LeftClickEvent OnLeftClick;

    public delegate void LeftClickRelease();
    public event LeftClickRelease OnLeftClickRelease;
    
    public delegate void RightClickEvent(Vector3 position);
    public event RightClickEvent OnRightClick;

    public delegate void RightClickRelease();
    public event RightClickRelease OnRightClickRelease;

    protected override void Awake() 
    {
        base.Awake();
        input = new RTSInput();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        // continuary actions.
        input.RTS.CameraScroll.performed += ctx => Scrolling(ctx);

        input.RTS.Up.performed += ctx => MoveUpHold(ctx);
        input.RTS.Down.performed += ctx => MoveDownHold(ctx);
        input.RTS.Left.performed += ctx => MoveLeftHold(ctx);
        input.RTS.Right.performed += ctx => MoveRightHold(ctx);

        // Trigger once actions.
        input.RTS.LeftClick.started += ctx => InteractStart(ctx);
        input.RTS.LeftClick.canceled += ctx => InteractEnd(ctx);

        input.RTS.RightClick.started += ctx => RightClickStart(ctx);
        input.RTS.RightClick.canceled += ctx => RightClickEnd(ctx);
    }

    public float Scrolling(InputAction.CallbackContext context)
    {
        // -1 is scroll down, 1 is scroll up. 
        return input.RTS.CameraScroll.ReadValue<float>();
    }

    public void MoveUpHold(InputAction.CallbackContext context) 
    {

    }

    public void MoveDownHold(InputAction.CallbackContext context)
    {

    }

    public void MoveLeftHold(InputAction.CallbackContext context)
    {

    }

    public void MoveRightHold(InputAction.CallbackContext context)
    {

    }

    public void InteractStart(InputAction.CallbackContext context)
    {
        //print(PointerWorldPosition());
        if (OnLeftClick != null) OnLeftClick(PointerWorldPosition());
    }

    public void InteractEnd(InputAction.CallbackContext context)
    {
        // left click
        if (OnLeftClickRelease != null) OnLeftClickRelease();

    }

    public void RightClickStart(InputAction.CallbackContext context)
    {
        if (OnRightClick != null) OnRightClick(PointerWorldPosition());
    }

    public void RightClickEnd(InputAction.CallbackContext context)
    {
        if (OnRightClickRelease != null) OnRightClickRelease();
    }

    public Vector3 PointerWorldPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(input.RTS.Pointer.ReadValue<Vector2>().x, input.RTS.Pointer.ReadValue<Vector2>().y, 0));
        worldPosition.z = 0;
        return worldPosition;
    }

    public float DetectLeftClickChange()
    {
        return input.RTS.LeftClick.ReadValue<float>();
    }

    public float DetectRightClickChange()
    {
        return input.RTS.RightClick.ReadValue<float>();
    }
}
