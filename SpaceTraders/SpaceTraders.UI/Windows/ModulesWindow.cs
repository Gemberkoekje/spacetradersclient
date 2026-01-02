using SpaceTraders.Core.Enums;
using SpaceTraders.Core.IDs;
using SpaceTraders.Core.Models.ShipModels;
using SpaceTraders.Core.Services;
using SpaceTraders.UI.CustomControls;
using SpaceTraders.UI.Extensions;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceTraders.UI.Windows;

internal sealed class ModulesWindow : DataBoundWindowWithContext<ImmutableArray<Module>, ShipContext>
{
    private readonly ShipService _shipService;
    private CustomListBox<ModuleListValue>? _modulesListBox;

    public ModulesWindow(RootScreen rootScreen, ShipService shipService)
        : base(rootScreen, 52, 20)
    {
        _shipService = shipService;

        SubscribeToEvent<ImmutableArray<Ship>>(
            handler => shipService.Updated += handler,
            handler => shipService.Updated -= handler,
            OnServiceUpdated);

        Initialize();
    }

    protected override ImmutableArray<Module> FetchData() =>
        _shipService.GetShips().FirstOrDefault(s => s.Symbol == Context.Ship)?.Modules ?? [];

    protected override void BindData(ImmutableArray<Module> data)
    {
        Title = $"Modules for ship {Context.Ship}";
        _modulesListBox?.SetCustomData([.. data.Select(module => new ModuleListValue(module))]);
    }

    protected override void DrawContent()
    {
        var y = 2;
        _modulesListBox = Controls.AddListbox<ModuleListValue>($"Modules", 2, y, 80, 10);
        Binds.Add("Modules", _modulesListBox);
        y += 10;
        Controls.AddButton("Show Module", 2, y, (_, _) => OpenModule());
    }

    private void OpenModule()
    {
        if (_modulesListBox?.GetSelectedItem() is ModuleListValue moduleListValue)
        {
            RootScreen.ShowWindow<ModuleWindow, ModuleContext>(new (moduleListValue.Module.Symbol));
        }
    }

    private sealed record ModuleListValue(Module Module)
    {
        public override string ToString() => $"{Module.Name} (Capacity: {Module.Capacity}, Range: {Module.Range})";
    }
}
