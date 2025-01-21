using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    private InputActions playerInput;
    private Player player;
    private CinemachineFreeLook freeLookCamera;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        { 
            //if (gameObject != null)
            //    Destroy(gameObject);
        }
    }

    public static InputManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }
   
    private void Awake()
    {
        Initialize();

        if (playerInput == null) playerInput = new InputActions();

        playerInput.PlayerTouch.TouchPress.performed -= ctx => TouchPressed(ctx);
        playerInput.PlayerTouch.TouchPress.performed += ctx => TouchPressed(ctx);
        playerInput.PlayerTouch.TouchPress.canceled -= ctx => TouchReleased(ctx);
        playerInput.PlayerTouch.TouchPress.canceled += ctx => TouchReleased(ctx);
    }

    private void Start()
    {

        player = Player.Instance();
        freeLookCamera = player.gameObject.GetComponent<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.PlayerTouch.TouchPress.performed -= ctx => TouchPressed(ctx);
        playerInput.PlayerTouch.TouchPress.canceled -= ctx => TouchReleased(ctx);

        playerInput.Disable();
    }

    private void TouchPressed(InputAction.CallbackContext ctx)
    {
        Vector2 value = playerInput.PlayerTouch.TouchPosition.ReadValue<Vector2>();
        Vector3 pos = Camera.main.ScreenToWorldPoint(value);
        Vector3 mainCamPos = Camera.main.transform.position;

        // Debug.Log("터치!" + value);

        // 터치 위치 디버깅
        Ray ray = Camera.main.ScreenPointToRay(value);

        // 적 / 땅을 체크
        if (Physics.Raycast(ray, out var _hitInfo, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            Debug.DrawRay(mainCamPos, _hitInfo.point - mainCamPos, Color.blue, .1f);
            //Debug.Log(_hitInfo.transform.name);
        }
        else if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(mainCamPos, hitInfo.point - mainCamPos, Color.red, .1f);
        }
    }

    private void TouchReleased(InputAction.CallbackContext ctx)
    {
        Vector2 value = playerInput.PlayerTouch.TouchPosition.ReadValue<Vector2>();
        Vector3 pos = Camera.main.ScreenToWorldPoint(value);
        Vector3 mainCamPos = Camera.main.transform.position;

        // Debug.Log("뗐다!" + value);

        // 터치 위치 디버깅
        Ray ray = Camera.main.ScreenPointToRay(value);

        // 적 / 땅을 체크
        if (Physics.Raycast(ray, out var _hitInfo, Mathf.Infinity, LayerMask.GetMask("Enemy")))
        {
            Debug.DrawRay(mainCamPos, _hitInfo.point - mainCamPos, Color.blue, .1f);
            //Debug.Log(_hitInfo.transform.name);
        }
        else if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(mainCamPos, hitInfo.point - mainCamPos, Color.red, .1f);
        }
    }
}
