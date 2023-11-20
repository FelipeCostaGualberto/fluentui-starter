﻿namespace App.Client.Infrastructure;

public delegate void OnLogHandler(string text);

public static class DemoLogger
{
    public static event OnLogHandler OnLogHandler;

    public static void WriteLine(string text)
    {
        OnLogHandler?.Invoke(text);
    }
}