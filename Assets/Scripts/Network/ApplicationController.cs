using System;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientSingletonPrefab;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicateServer)
    {
        if (isDedicateServer)
        {
            
        }
        else
        {
            ClientSingleton clientSingleton = Instantiate(_clientSingletonPrefab);

            await clientSingleton.CreateClient();
        }
    }
}
