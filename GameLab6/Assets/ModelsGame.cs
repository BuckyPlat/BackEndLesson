using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ModelGameLevel
{
    [System.Serializable]
    public class ResponseAPI
    {
        public bool isSuccess;
        public string notification;
        public List<GameLevel> data;
    }
    [System.Serializable]
    public class GameLevel
    {
        public int LevelId;
        public string title;
        public string description;
    }
}
