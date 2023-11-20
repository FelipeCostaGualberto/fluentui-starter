using App.Client.Pages.Layout;

namespace App.BlazorServer;

public class AppBlazorServer : AppClient
{
    public AppBlazorServer()
    {
        ClientAssembly = typeof(Program).Assembly;
    }
}