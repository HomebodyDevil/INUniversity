using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionPanel : MonoBehaviour
{
    public void OnClicked()
    {
        //Debug.Log("Clicked Deck");

        bool isActive = gameObject.activeSelf;

        if (isActive)
        {
            //gameObject.SetActive(false);
        }
        else
        {
            CanclePanel.OnCanclePanelClicked.Invoke();

            gameObject.SetActive(true);
            UIManager.Instance().CurrentUIManager().ActivateCanclePanel();
            UIManager.Instance().CurrentUIManager().activeObject.Push(gameObject);

            // Panel�� Ȱ��ȭ���� �� ī�޶��� �������� �����ϱ� ����.
            // ���� ������ �߻��� ������ �����... ����...
            // ���ֿ� �� ���� ����� ����� �װ� ������ ����...
            // Time.timeScale = 0;

            //Player player = Player.Instance();
            //PlayerCameraController cameraController = player.gameObject.GetComponent<PlayerCameraController>();
            //cameraController.InActivateAllCamera();

            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = false;
        }
    }
}
