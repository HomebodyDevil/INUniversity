using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static PlayerMovement instance;

    public static Action OnMakePlayerCanMove;
    public static Action OnMakePlayerCantMove;

    public Vector3 moveDirection;
    public bool isMoving;

    private GPSManager gps;
    private Player player;

    [SerializeField, Range(.1f, 50f)] private float rotateSpeed = 7f;
    [SerializeField] private float moveSpeed = 5f;

    private void Awake()
    {
        Initialize();

        gps = GetComponent<GPSManager>();
        player = GetComponent<Player>();        
    }

    void Start()
    {
        OnMakePlayerCanMove -= MakePlayerCanMove;
        OnMakePlayerCanMove += MakePlayerCanMove;

        OnMakePlayerCantMove -= MakePlayerCantMove;
        OnMakePlayerCantMove += MakePlayerCantMove;

        moveDirection = Vector3.forward;

        OnMakePlayerCanMove.Invoke();
    }

    public static PlayerMovement Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    private void RotatePlayer()
    {
        if (moveDirection.magnitude > 0)
        {
            // 부드럽게 돌도록 Lerp를 사용.
            // 이동 방향을 바라보도록 한다.
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = 
                Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    private void MovePlayer()
    {
        if (this.gameObject == null) return;

        transform.position = Vector3.Lerp(transform.position, new Vector3((float)player.xCor, transform.position.y,
                                                    (float)player.zCor), moveSpeed * Time.deltaTime);

        Vector2 currPos = new Vector2(transform.position.x, transform.position.z);
        float dist = Vector2.Distance(currPos, new Vector2((float)player.xCor, (float)player.zCor));
        if ((int)dist > 0)
            isMoving = true;
        else
            isMoving = false;

        //transform.position = new Vector3((float)player.xCor, transform.position.y, (float)player.zCor);
    }

    public void MakePlayerCanMove()
    {
        // Player스크립트에 Action으로 추가돼있음. 따라서 생략 가능할듯.
        // Player.Instance().canMove = true;
        StartCoroutine("MakePlayerMovingCoroutine");
    }

    IEnumerator MakePlayerMovingCoroutine()
    {
        while(true)
        {
            //RotatePlayer();
            MovePlayer();

            yield return null;
        }
    }

    public void MakePlayerCantMove()
    {
        // Player스크립트에 Action으로 추가돼있음. 따라서 생략 가능할듯.
        // Player.Instance().canMove = false;
        StopCoroutine("MakePlayerMovingCoroutine");
    }
}
