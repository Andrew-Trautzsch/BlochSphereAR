using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{

    public GameObject objectToRotate;

    public void ChangeScene()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            SceneManager.LoadScene(1);
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else {
            SceneManager.LoadScene(0);
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

}
