using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeSceneScript : MonoBehaviour
{
    //出口につけるスクリプト
    //出口に触れたらシーン移動する

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene("TestMap");//すぐにシーン遷移するぞ
        }
    }
}
