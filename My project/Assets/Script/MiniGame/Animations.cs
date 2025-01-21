using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Animations : MonoBehaviour
{
    public GameManager GameManager;

    Animator StartButton;
    Animator MainImage;
    Animator Control;
    Animator BackGround;
    Animator GameOver;
    Animator GameClear;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartButton = GameObject.Find("StartButton").GetComponent<Animator>();
        MainImage = GameObject.Find("MainImage").GetComponent<Animator>();
        Control = GameObject.Find("Control").GetComponent<Animator>();
        BackGround = GameObject.Find("ArrowBackground").GetComponent<Animator>();
        GameOver = GameObject.Find("Gameover").GetComponent<Animator>();
        GameClear = GameObject.Find("StageClear").GetComponent<Animator>();
        StartAnim();
    }

    public void StartAnim()
    {
        MainImage.SetBool("isShow", true);
        StartButton.SetBool("isShow", true);
    }

    public void EndAnim()
    {
        MainImage.SetBool("isShow", false);
        StartButton.SetBool("isShow", false);
    }

    public void StartGame()
    {
        Control.SetBool("isShow", true);
        BackGround.SetBool("isShow", true);
    }

    public void EndGame()
    {
        Control.SetBool("isShow", false);
        BackGround.SetBool("isShow", false);
        GameOver.SetBool("isShow", true);
    }

    public void StageClear()
    {
        GameClear.SetBool("isShow", true);
    }

    public void StageClearDown()
    {
        GameClear.SetBool("isShow", false);
    }

}
