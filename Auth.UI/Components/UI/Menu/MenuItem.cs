using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Menu;

public class MenuItem { public string Text { get; set; } = string.Empty; public string? Icon { get; set; } = null; public bool Disabled { get; set; } }
public class MenuActionItem : MenuItem { public string? Key { get; set; } }
public class MenuLinkItem : MenuItem { public string Url { get; set; } = "#"; public bool PreventDefault { get; set; } }
public class MenuDivider { }
public class MenuHeaderItem : MenuItem { }
