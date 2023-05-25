using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonTest : MonoBehaviour
{
    PlantInfoList plantInfoList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) {
            TestLiteJson();
        }
    }

    PlantInfoList GetPlantInfoList()
    {
        plantInfoList = new PlantInfoList();
        plantInfoList.plantInfoList = new List<PlantInfo>();
        for (int i = 0; i <= 3; i++)
        {
            PlantInfo person = new PlantInfo();
            person.id = i+1;
            person.name = ((PlantType)i).ToString();
            person.buyCost = 0;
            person.growTime = 0;
            person.sellCost = 0;
            plantInfoList.plantInfoList.Add(person);
        }
        return plantInfoList;
    }

    private void TestLiteJson()
    {
        //序列化
        string jsonStr = "";
        jsonStr = JsonMapper.ToJson(GetPlantInfoList());//写
        StreamWriter writer;
        FileInfo file = new FileInfo(Application.dataPath + "/Farm/Resources/PlantInfoJson.json");
        //if (!file.Exists)
        //{
        //    writer = file.CreateText();
        //}
        //else
        //{
        //    writer = file.AppendText();
        //}
        writer = file.CreateText();
        writer.WriteLine(jsonStr);
        writer.Flush();
        writer.Dispose();
        writer.Close(); 
        print(string.Format("LiteJson序列化：{0}",jsonStr));
    }
}
