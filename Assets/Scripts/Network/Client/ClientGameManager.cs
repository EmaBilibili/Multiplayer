using UnityEditor.VersionControl;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;
using Task = System.Threading.Tasks.Task;

public class ClientGameManager
{
    private const string MainMenu = "MainMenu";
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        AuthState authState = await AuthenticationManager.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }
        return false;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenu);
    }
}
