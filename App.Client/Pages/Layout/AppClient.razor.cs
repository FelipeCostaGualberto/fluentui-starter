using System.Reflection;

namespace App.Client.Pages.Layout;

public partial class AppClient
{
    public Assembly ClientAssembly { get; set; }
    public const string MESSAGES_NOTIFICATION_CENTER = "NOTIFICATION_CENTER";
    public const string MESSAGES_TOP = "TOP";
    public const string MESSAGES_DIALOG = "DIALOG";
    public const string MESSAGES_CARD = "CARD";
}