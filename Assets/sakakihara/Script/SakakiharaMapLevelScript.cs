using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakakiharaMapLevelScript : MonoBehaviour
{
    //仮で作ったレベル

   [SerializeField] private int mapLevel;
    public static int MAPLevel;

    private void Awake()
    {
        mapLevel = 3;
        MAPLevel = mapLevel;
    }

}
