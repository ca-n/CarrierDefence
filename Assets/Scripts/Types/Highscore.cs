using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using Managers;
using UnityEngine;

namespace Types
{
    [Serializable]
    public struct Highscore
    {
        public string name;
        public int wave;
        public int score;
    }
}