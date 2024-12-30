using System;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;

    [SerializeField] private HostGameManager _gameManager;

    private static HostSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                instance = FindAnyObjectByType<HostSingleton>();
            }

            if (instance == null)
            {
                Debug.LogError("No HostSingleton found in this scene!");
                return null;
            }

            return instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
       _gameManager = new HostGameManager();
    }
}
