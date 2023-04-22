using Character;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    private static List<GameObject> _players;
    
    private void Start()
    {
        _players = new List<GameObject>();
    }

    public static GameObject[] PlayerList => _players.ToArray();

    public static void RegisterPlayer(GameObject obj)
    {
        _players.Add(obj);
    }
}
