using System;
using System.Reflection;
using System.Threading.Tasks;
using App.Client.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;
using Microsoft.JSInterop;

namespace App.Client.Pages.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject] private GlobalState GlobalState { get; set; }
    [Inject] private IJSRuntime Js { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private AccentBaseColor AccentBaseColor { get; set; }
    private bool _menuchecked = true;
    private bool _isMobile;
    private ElementReference _container;
    private IJSObjectReference _jsModule;
    private OfficeColor _selectedColorOption;
    private string _previousUri;
    private string _version;
    private TableOfContents _toc;

    protected override void OnInitialized()
    {
        _version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        var colors = Enum.GetValues<OfficeColor>();
        _selectedColorOption = colors[new Random().Next(colors.Length)];
        GlobalState.SetColor(_selectedColorOption.ToAttributeValue());

        _previousUri = Navigation.Uri;
        Navigation.LocationChanged += OnLocationChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        GlobalState.SetContainer(_container);

        _jsModule = await Js.GetJsModule(this.GetType());

        _isMobile = await _jsModule.InvokeAsync<bool>("isMobile");

        var isDarkMode = await _jsModule.InvokeAsync<bool>("isDarkMode");
        GlobalState.SetLuminance(isDarkMode ? StandardLuminance.DarkMode : StandardLuminance.LightMode);

        if (_selectedColorOption != OfficeColor.Default)
        {
            await AccentBaseColor.WithDefault(_selectedColorOption.ToAttributeValue().ToSwatch());
        }
    }

    public EventCallback OnRefreshTableOfContents => EventCallback.Factory.Create(this, RefreshTableOfContents);

    private async Task RefreshTableOfContents()
    {
        await _toc.Refresh();
    }

    private void HandleChecked()
    {
        _menuchecked = !_menuchecked;
    }

    private void OnLocationChanged(object sender, LocationChangedEventArgs e)
    {
        if (!e.IsNavigationIntercepted && new Uri(_previousUri!).AbsolutePath != new Uri(e.Location).AbsolutePath)
        {
            _previousUri = e.Location;
            if (_isMobile && _menuchecked == true)
            {
                _menuchecked = false;
                StateHasChanged();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // The JSRuntime side may routinely be gone already if the reason we're disposing is that
            // the client disconnected. This is not an error.
        }
    }
}