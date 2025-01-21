using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DestroySelectedBox : MonoBehaviour
{
    public void DestroyBox()
    {
        // UnitManage �̱��� �ν��Ͻ����� selectedBox ��������
        GameObject selectedBox = SceneDataManager.Instance.GetSelectedBox();

        SceneDataManager.Instance.RemoveBox(selectedBox);

        SceneManager.LoadScene(0);      // �� ��ȯ
    }


}
