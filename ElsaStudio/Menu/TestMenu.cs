using Elsa.Studio;
using Elsa.Studio.Contracts;
using Elsa.Studio.Models;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace ElsaStudio.Menu
{
    public class TestMenu : IMenuProvider
    {
        public ValueTask<IEnumerable<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default)
        {
            var menuItems = new List<MenuItem>
        {
            new()
            {
                Icon = Icons.Material.Outlined.Lightbulb,
                Href = "test",
                Text = "Test",
                GroupName = MenuItemGroups.General.Name,
                Match = NavLinkMatch.All
            }
        };

            return new ValueTask<IEnumerable<MenuItem>>(menuItems);
        }
    }
}
