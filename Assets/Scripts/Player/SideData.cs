using Unity.VisualScripting;
using UnityEngine;

public class SideData : MonoBehaviour
{
    [SerializeField]private bool isBlue;
    public bool IsBlue
    {
        get {return isBlue;}
        set {isBlue = value;}
    }
}
