using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Client.Pages.Layout;

public partial class SiteSettingsPanel : IDialogContentComponent<GlobalState>, IAsyncDisposable
{
    [Parameter] public GlobalState Content { get; set; }
    [Inject] private GlobalState GlobalState { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private BaseLayerLuminance BaseLayerLuminance { get; set; }
    [Inject] private AccentBaseColor AccentBaseColor { get; set; }
    [Inject] private Direction Direction { get; set; }
    private const string _themeSettingSystem = "System";
    private const string _themeSettingDark = "Dark";
    private const string _themeSettingLight = "Light";
    private string _currentTheme = _themeSettingSystem;
    private LocalizationDirection _dir;
    private OfficeColor _selectedColorOption;
    private bool _rtl;
    private IJSObjectReference _jsModule;
    private ElementReference _container;

    protected override void OnInitialized()
    {
        _rtl = Content.Dir == LocalizationDirection.rtl;
        _container = Content.Container;

        OfficeColor[] colors = Enum.GetValues<OfficeColor>();
        _selectedColorOption = colors.Where(x => x.ToAttributeValue() == Content.Color).FirstOrDefault();

        if (_selectedColorOption == OfficeColor.Default)
        {
            _selectedColorOption = colors[new Random().Next(colors.Length)];

            Content.Color = _selectedColorOption.ToAttributeValue();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/App.Client/js/theme.js");

        _currentTheme = await _jsModule.InvokeAsync<string>("getThemeCookieValue");
        StateHasChanged();
    }

    public async Task UpdateDirectionAsync()
    {
        _dir = _rtl ? LocalizationDirection.rtl : LocalizationDirection.ltr;

        Content.Dir = _dir;

        await _jsModule.InvokeVoidAsync("switchDirection", _dir.ToString());
        await Direction.WithDefault(_dir.ToAttributeValue());

        StateHasChanged();
    }

    private async Task UpdateThemeAsync()
    {
        if (_jsModule is not null)
        {
            StandardLuminance newLuminance = await GetBaseLayerLuminanceForSetting(_currentTheme);

            await BaseLayerLuminance.WithDefault(newLuminance.GetLuminanceValue());
            GlobalState.SetLuminance(newLuminance);
            await _jsModule.InvokeVoidAsync("switchHighlightStyle", newLuminance == StandardLuminance.DarkMode);
            await _jsModule.InvokeVoidAsync("setThemeCookie", _currentTheme);
        }

        //_currentTheme = newValue;
    }

    private async Task UpdateColorAsync(ChangeEventArgs args)
    {
        string value = args.Value?.ToString();
        if (!string.IsNullOrEmpty(value))
        {
            if (value != "default")
            {
                await AccentBaseColor.WithDefault(value.ToSwatch());
            }
            else
            {
                await AccentBaseColor.WithDefault("#0078D4".ToSwatch());
            }

            Content.Color = value;
        }
    }

    private Task<StandardLuminance> GetBaseLayerLuminanceForSetting(string setting)
    {
        if (setting == _themeSettingLight)
        {
            return Task.FromResult(StandardLuminance.LightMode);
        }
        else if (setting == _themeSettingDark)
        {
            return Task.FromResult(StandardLuminance.DarkMode);
        }
        else // "System"
        {
            return GetSystemThemeLuminance();
        }
    }

    private async Task<StandardLuminance> GetSystemThemeLuminance()
    {
        if (_jsModule is not null)
        {
            var systemTheme = await _jsModule.InvokeAsync<string>("getSystemTheme");
            if (systemTheme == _themeSettingDark)
            {
                return StandardLuminance.DarkMode;
            }
        }

        return StandardLuminance.LightMode;
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