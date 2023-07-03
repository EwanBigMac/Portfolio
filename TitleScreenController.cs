using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadValues()
    {
        MainManager.Instance.Load();
    }

    public void SaveValues()
    {
        MainManager.Instance.Save();
    }

    public void sliceStart()
    {
        SceneManager.LoadScene("raaSlice");
    }

    public void rayTraceStart()
    {
        SceneManager.LoadScene("RayTraceNew");
    }

    public void billboardStart()
    {
        SceneManager.LoadScene("raaBillboard");
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
