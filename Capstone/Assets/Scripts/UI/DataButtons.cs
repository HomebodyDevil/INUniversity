using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataButtons : MonoBehaviour
{
    enum DataButtonType
    {
        LocalSave,
        LocalLoad,
        ServerSave,
        ServerLoad,
        Clear,
    }

    [SerializeField] private DataButtonType type;

    public void OnButtonClick()
    {
        switch (type)
        {
            case DataButtonType.LocalSave:
                DataManager.Instance().SaveData();
                break;
            case DataButtonType.LocalLoad:
                DataManager.Instance().LoadData();
                break;
            case DataButtonType.Clear:
                DataManager.Instance().ResetData();
                break;
        }

        SoundManager.OnButtonUp.Invoke();
    }
}
