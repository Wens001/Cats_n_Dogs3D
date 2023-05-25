using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTimeTool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            DebugTime();
        }
    }

    void DebugTime() {
        string date_str = DateTime.Now.ToString();
        Debug.Log("date_str:" + date_str);
        DateTime date = Convert.ToDateTime(date_str);
        Debug.Log("date:" + date.ToString());

        DateTime nowDateTime = DateTime.Now;
        DateTime nowUtcDateTime = DateTime.UtcNow;
        long timeStamp = TimeTool.GetTimeStamp(true);
        Debug.Log("DateTime.Now:" + nowDateTime.ToString());
        Debug.Log("DateTime.UtcNow:" + nowUtcDateTime.ToString());
        Debug.Log("TimeTool.GetTimeStamp(true):" + timeStamp);
        long gettimeStamp = TimeTool.UtcDateToTimeStamp(nowUtcDateTime);
        Debug.Log("UtcDateToTimeStamp:" + gettimeStamp);
        Debug.Log("DateToTimeStamp:" + TimeTool.DateToTimeStamp(nowDateTime));
        Debug.Log("GetDateTime:" + TimeTool.UtcStampToDateTime(gettimeStamp));
    }
}
