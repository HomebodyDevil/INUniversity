using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class A_UIManager : MonoBehaviour
{
    public Stack<GameObject> activeObject;

    abstract public void ActivateCanclePanel();
    abstract public void OnCanclePanel();
}
