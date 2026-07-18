using Microsoft.AspNetCore.Components;

namespace Shared.UI.Components.Icons;

public partial class Icon : ComponentBase
{
    public enum IconSize { Small, Medium, Large }

    private static readonly Dictionary<string, string> Icons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["x"] = "<line x1=\"18\" y1=\"6\" x2=\"6\" y2=\"18\"/><line x1=\"6\" y1=\"6\" x2=\"18\" y2=\"18\"/>",
        ["check"] = "<polyline points=\"20 6 9 17 4 12\"/>",
        ["check-circle"] = "<path d=\"M22 11.08V12a10 10 0 1 1-5.93-9.14\"/><polyline points=\"22 4 12 14.01 9 11.01\"/>",
        ["info"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"12\" y1=\"16\" x2=\"12\" y2=\"12\"/><line x1=\"12\" y1=\"8\" x2=\"12.01\" y2=\"8\"/>",
        ["info-circle"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"12\" y1=\"16\" x2=\"12\" y2=\"12\"/><line x1=\"12\" y1=\"8\" x2=\"12.01\" y2=\"8\"/>",
        ["alert-triangle"] = "<path d=\"M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z\"/><line x1=\"12\" y1=\"9\" x2=\"12\" y2=\"13\"/><line x1=\"12\" y1=\"17\" x2=\"12.01\" y2=\"17\"/>",
        ["alert-circle"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><line x1=\"12\" y1=\"8\" x2=\"12\" y2=\"12\"/><line x1=\"12\" y1=\"16\" x2=\"12.01\" y2=\"16\"/>",
        ["search"] = "<circle cx=\"11\" cy=\"11\" r=\"8\"/><line x1=\"21\" y1=\"21\" x2=\"16.65\" y2=\"16.65\"/>",
        ["chevron-down"] = "<polyline points=\"6 9 12 15 18 9\"/>",
        ["chevron-right"] = "<polyline points=\"9 18 15 12 9 6\"/>",
        ["chevron-left"] = "<polyline points=\"15 18 9 12 15 6\"/>",
        ["plus"] = "<line x1=\"12\" y1=\"5\" x2=\"12\" y2=\"19\"/><line x1=\"5\" y1=\"12\" x2=\"19\" y2=\"12\"/>",
        ["filter"] = "<polygon points=\"22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3\"/>",
        ["columns"] = "<rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\" ry=\"2\"/><line x1=\"12\" y1=\"3\" x2=\"12\" y2=\"21\"/>",
        ["trash"] = "<polyline points=\"3 6 5 6 21 6\"/><path d=\"M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2\"/>",
        ["arrow-up"] = "<line x1=\"12\" y1=\"19\" x2=\"12\" y2=\"5\"/><polyline points=\"5 12 12 5 19 12\"/>",
        ["arrow-down"] = "<line x1=\"12\" y1=\"5\" x2=\"12\" y2=\"19\"/><polyline points=\"19 12 12 19 5 12\"/>",
        ["edit"] = "<path d=\"M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7\"/><path d=\"M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z\"/>",
        ["sliders"] = "<line x1=\"4\" y1=\"21\" x2=\"4\" y2=\"14\"/><line x1=\"4\" y1=\"10\" x2=\"4\" y2=\"3\"/><line x1=\"12\" y1=\"21\" x2=\"12\" y2=\"12\"/><line x1=\"12\" y1=\"8\" x2=\"12\" y2=\"3\"/><line x1=\"20\" y1=\"21\" x2=\"20\" y2=\"16\"/><line x1=\"20\" y1=\"12\" x2=\"20\" y2=\"3\"/><line x1=\"1\" y1=\"14\" x2=\"7\" y2=\"14\"/><line x1=\"9\" y1=\"8\" x2=\"15\" y2=\"8\"/><line x1=\"17\" y1=\"16\" x2=\"23\" y2=\"16\"/>",
        ["fingerprint"] = "<path d=\"M12 10a2 2 0 0 0-2 2c0 1.02-.1 2.51-.26 4\"/><path d=\"M14 13.12c0 2.38 0 6.38-1 8.88\"/><path d=\"M17.29 21.02c.12-.6.43-2.3.5-3.02\"/><path d=\"M2 12a10 10 0 0 1 18-6\"/><path d=\"M2 19h.01\"/><path d=\"M5 19.5h.01\"/><path d=\"M7 18v-1.22c.46-.2.91-.51 1.34-.89\"/><path d=\"M9 16.5h.01\"/><path d=\"M11 17.5c.5-.2 1-.5 1.5-.87\"/><path d=\"M13.5 15.5c.74-.62 1.5-1.5 1.5-2.5\"/><path d=\"M17 14c.5.5 1 1.5 1 2.5\"/>",
        ["key"] = "<circle cx=\"7.5\" cy=\"15.5\" r=\"4.5\"/><path d=\"M10.7 12.3 21 2\"/><path d=\"M16 7l3 3\"/><path d=\"M19 4l2 2\"/>",
        ["shield"] = "<path d=\"M12 2 4 5v6c0 5 3.5 8.5 8 11 4.5-2.5 8-6 8-11V5z\"/>",
        ["smartphone"] = "<rect x=\"7\" y=\"2\" width=\"10\" height=\"20\" rx=\"2\"/><line x1=\"11\" y1=\"18\" x2=\"13\" y2=\"18\"/>",
        ["usb"] = "<circle cx=\"12\" cy=\"5\" r=\"2\"/><path d=\"M12 7v6\"/><path d=\"M8 13h8l-1.5 7h-5z\"/>",
        ["lock"] = "<rect x=\"5\" y=\"11\" width=\"14\" height=\"9\" rx=\"2\"/><path d=\"M8 11V8a4 4 0 0 1 8 0v3\"/>",
        ["user"] = "<circle cx=\"12\" cy=\"8\" r=\"4\"/><path d=\"M4 21c0-4 4-6 8-6s8 2 8 6\"/>",
        ["eye"] = "<path d=\"M1 12s4-7 11-7 11 7 11 7-4 7-11 7-11-7-11-7z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/>",
        ["eye-off"] = "<path d=\"M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24\"/><line x1=\"1\" y1=\"1\" x2=\"23\" y2=\"23\"/>"
    };

    [Parameter] public string Name { get; set; } = "info";
    [Parameter] public IconSize Size { get; set; } = IconSize.Medium;
    [Parameter] public int StrokeWidth { get; set; } = 2;
    [Parameter] public int? Width { get; set; }

    private string InnerSvg => Icons.TryGetValue(Name, out var svg)
        ? svg
        : $"<text x=\"12\" y=\"16\" font-size=\"10\" text-anchor=\"middle\" stroke=\"none\" fill=\"currentColor\">{Name}</text>";
}
