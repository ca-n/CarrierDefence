using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Managers;
using Types;
using UnityEngine;

public static class HighscoreStorage
{
    // save highscore to disk
    public static void Save(Highscore highscore)
    {
        List<Highscore> highscores = Load();
        for (int i = 0; i < highscores.Count; ++i)
        {
            Highscore saved = highscores[i];
            if (highscore.wave > saved.wave ||
                (highscore.wave == saved.wave && highscore.score > saved.score))
            {
                highscores.Insert(i, highscore);
            }
        }
        Serializer.Serialize(GameManager.Instance.HighscorePath, highscores);
    }

    // loads the highscores from disk
    public static List<Highscore> Load()
    {
        List<Highscore> highscores = Serializer.Deserialize<List<Highscore>>(GameManager.Instance.HighscorePath);
        if (highscores == null) highscores = new List<Highscore>();
        return highscores;
    }
}