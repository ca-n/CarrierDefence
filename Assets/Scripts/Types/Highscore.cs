using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using UnityEngine;

namespace Types
{
    [Serializable]
    public class Highscore
    {
        private static string _path = Application.persistentDataPath + "/highscores.dat";
        public string Name { get; private set; }
        public int Wave { get; private set; }
        public int Score { get; private set; }

        public Highscore(string name, int wave, int score)
        {
            Name = name;
            Wave = wave;
            Score = score;
        }

        // save highscore to disk
        public static void Save(Highscore highscore)
        {
            List<Highscore> highscores = LoadAll();
            for (int i = 0; i < highscores.Count; ++i)
            {
                Highscore saved = highscores[i];
                if (highscore.Wave > saved.Wave || (highscore.Wave == saved.Wave && highscore.Score > saved.Score))
                {
                    highscores.Insert(i, highscore);
                }
            }

            FileStream fileStream = File.Exists(_path) ? File.OpenWrite(_path) : File.Create(_path);
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(fileStream, highscore);
            fileStream.Close();
        }

        // loads the highscores from disk
        public static List<Highscore> LoadAll()
        {
            if (!File.Exists(_path)) return new List<Highscore>();
            FileStream fileStream = File.OpenRead(_path);
            BinaryFormatter binFormatter = new BinaryFormatter();
            List<Highscore> highscores = (List<Highscore>) binFormatter.Deserialize(fileStream);
            fileStream.Close();
            return highscores;
        }
    }
}