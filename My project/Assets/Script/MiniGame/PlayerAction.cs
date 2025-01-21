using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{

    //PC key
    bool keyRDown, keyLDown, keyUDown, keyDDown;
    bool keyRUp, keyLUp, keyUUp, keyDUp;


    //mobile key
    bool up_down, down_down, left_down, right_down;
    bool up_up, down_up,left_up, right_up;

    //Check
    bool isDown;

    bool Rdown, Ldown, Udown, Ddown;
    bool Rup, Lup, Uup, Dup;


    public bool GetRdown() { return Rdown; }
    public bool GetLdown() { return Ldown; }
    public bool GetUdown() { return Udown; }
    public bool GetDdown() { return Ddown; }

    public bool GetDown() { return isDown; }

    void Update()
    {
        // 키보드 방향키 입력 체크
        keyRDown = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        keyLDown = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        keyUDown = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        keyDDown = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

        keyRUp = Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D);
        keyLUp = Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A);
        keyUUp = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
        keyDUp = Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);


        // 방향키 눌림 상태 확인
        Rdown = right_down || keyRDown;
        Ldown = left_down || keyLDown;
        Udown = up_down || keyUDown;
        Ddown = down_down || keyDDown;

        // 방향키 입력 상태 업데이트
        isDown = Rdown || Ldown || Udown || Ddown;

        // 방향키 떼짐 상태 확인
        Rup = right_up || keyRUp;
        Lup = left_up || keyLUp;
        Uup = up_up || keyUUp;
        Dup = down_up || keyDUp;
    }
    void LateUpdate()
    {
        up_down = false;
        down_down = false;
        left_down = false;
        right_down = false;
        up_up = false;
        down_up = false;
        left_up = false;
        right_up = false;

        //Debug.Log($"Rdown: {Rdown}, Ldown: {Ldown}, Udown: {Udown}, Ddown: {Ddown}");
        //Debug.Log($"isDown: {isDown}");
    }
    public void ButtonDown(string type)
    {
        switch (type)
        {
            case "U":
                up_down = true;
                break;
            case "D":
                down_down = true;
                break;
            case "L":
                left_down = true;
                break;
            case "R":
                right_down = true;
                break;
        }
    }

    public void ButtonUp(string type)
    {
        switch (type)
        {
            case "U":
                up_up = true;
                break;
            case "D":
                down_up = true;
                break;
            case "L":
                left_up = true;
                break;
            case "R":
                right_up = true;
                break;
        }
    }


}
