using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticated,
    Authenticating,
    Error,
    TimeOut
}

public static class AuthenticationManager
{
    public static AuthState CurrentAuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (CurrentAuthState == AuthState.Authenticated)
        {
            return CurrentAuthState;
        }
        
        CurrentAuthState = AuthState.Authenticating;

        int tries = 0;

        while (CurrentAuthState == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                CurrentAuthState = AuthState.Authenticated;
                break;
            }

            tries++;
            await Task.Delay(1000);
        }
        return CurrentAuthState;
    }
}
