using System.Collections.Generic;
using System;

[Serializable]
public class PlantInfoList {
    public List<PlantInfo> plantInfoList;
}

[Serializable]
public class PlantInfo 
{
    public int id;
    public string name;
    public int buyCost;
    public int growTime;
    public int sellCost;
}

public enum PlantType { 
    NULL,
    Pumpkin,//南瓜
    Corn,//玉米
    Apple,//苹果
}
