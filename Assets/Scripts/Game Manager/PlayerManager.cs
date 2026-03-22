using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;

    private LoadoutData data;
    private Transform L, R, U, D;
    private SlotItemData leftData, rightData, upData, downData;

    void Awake()
    {
        data = LoadoutData.Selected;

        L = player.transform.Find("L");
        R = player.transform.Find("R");
        U = player.transform.Find("U");
        D = player.transform.Find("D");

        leftData = data.selections[3].itemData;
        rightData = data.selections[1].itemData;
        upData = data.selections[0].itemData;
        downData = data.selections[2].itemData;

        GameObject leftSide = Instantiate(leftData.prefab, L);
        leftSide.GetComponent<SideData>().IsBlue = leftData.isBlue;

        GameObject rightSide = Instantiate(rightData.prefab, R);
        rightSide.GetComponent<SideData>().IsBlue = rightData.isBlue;

        GameObject upSide = Instantiate(upData.prefab, U);
        upSide.GetComponent<SideData>().IsBlue = upData.isBlue;

        GameObject downSide = Instantiate(downData.prefab, D);
        downSide.GetComponent<SideData>().IsBlue = downData.isBlue;

    }
}
