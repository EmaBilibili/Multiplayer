using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager GameManager { get; private set; }

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

    public async Task<bool> CreateClient()
    {
       GameManager = new ClientGameManager();

       return await GameManager.InitAsync();
    }
}
