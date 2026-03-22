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

    // void Awake()
    // {
    //     int colour = UnityEngine.Random.Range(0, 1);
    //     isBlue = colour == 0 ? true : false;
    // }
}
