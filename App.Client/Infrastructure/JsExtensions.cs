using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;

namespace App.Client.Infrastructure;

public static class JsExtensions
{
    public static async ValueTask<IJSObjectReference> GetJsModule(this IJSRuntime jsRuntime, Type componentType)
    {
        ArgumentNullException.ThrowIfNull(jsRuntime);
        ArgumentNullException.ThrowIfNull(componentType);

        var fullName = componentType.FullName;
        var rootNamespace = typeof(_Imports).Namespace;
        string modulePath;

        if (fullName.StartsWith(rootNamespace))
        {
            var subPath = fullName.Substring(rootNamespace.Length + 1);
            subPath = subPath.Replace('.', '/');
            modulePath = $"./_content/{rootNamespace}/{subPath}.razor.js";
        }
        else
        {
            throw new InvalidOperationException("The component is not within the expected namespace.");
        }

        var jsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", modulePath);
        return jsModule;
    }
}