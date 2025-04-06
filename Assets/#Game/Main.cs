using _Game;
using _Game.Marker;
using _Game.Utils;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private MarkerComponent _marker;

    void Awake()
    {
        G.MarkerPool = new ObjectPool<MarkerComponent>(_marker, 200, transform);
    }

    void Update()
    {
    }
}