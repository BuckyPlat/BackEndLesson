using System;
using System.Collections.Generic;
using UnityEngine;

namespace Region
{
    [Serializable]
    public class Response
    {
        public bool isSuccess;
        public string notification;
        public List<RegionData> data;
    }

    [Serializable]
    public class RegionData
    {
        public int regionId;
        public string name;
    }
}
