using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DestroySelectedBox : MonoBehaviour
{
    public void DestroyBox()
    {
        // UnitManage 싱글톤 인스턴스에서 selectedBox 가져오기
        GameObject selectedBox = SceneDataManager.Instance.GetSelectedBox();

        SceneDataManager.Instance.RemoveBox(selectedBox);

        SceneManager.LoadScene(0);      // 씬 전환
    }


}
