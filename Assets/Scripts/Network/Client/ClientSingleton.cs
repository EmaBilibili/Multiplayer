using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    [SerializeField] private ClientGameManager _gameManager;

    private static ClientSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                instance = FindAnyObjectByType<ClientSingleton>();
            }

            if (instance == null)
            {
                Debug.LogError("No ClientSingleton found in this scene!");
                return null;
            }

            return instance;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateClient()
    {
       _gameManager = new ClientGameManager();

       await _gameManager.InitAsync();
    }
}
