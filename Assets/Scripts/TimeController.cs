using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float slowdownFactor = 0.0005f;
    public float slowdownLength = 200f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("TimeController, Update : TimeScale = " + Time.timeScale + ", DeltaTime = " + Time.deltaTime);

        Time.timeScale += Time.unscaledDeltaTime * (1f / slowdownLength);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        

    }
    public void DoSlowMo()
    {
        /*Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;*/
    }
}
