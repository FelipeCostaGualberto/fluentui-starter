using App.Client.Pages.Layout;

namespace App.WasmClient;

public class AppWasm : AppClient
{
    public AppWasm()
    {
        ClientAssembly = typeof(Program).Assembly;
    }
}