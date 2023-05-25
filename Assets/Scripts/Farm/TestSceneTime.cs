using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestSceneTime : MonoBehaviour
{
    public Text text;
   

    // Update is called once per frame
    void Update()
    {
        text.text = Time.timeSinceLevelLoad.ToString();
        if (Input.GetKeyDown(KeyCode.A)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
