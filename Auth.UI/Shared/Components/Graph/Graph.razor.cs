using Auth.UI.Shared.Components.Graph;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.UI.Graph;

public partial class Graph : ComponentBase
{
    [Parameter] public IEnumerable<object> Data { get; set; } = new List<object>();
    [Parameter] public string XAxis { get; set; } = "Label";
    [Parameter] public string YAxis { get; set; } = "Value";
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public GraphType Type { get; set; } = GraphType.Line;
    [Parameter] public GraphSize Size { get; set; } = GraphSize.Medium;

    private static readonly string[] Palette =
    {
        "#4e79a7", "#f28e2b", "#e15759", "#76b7b2", "#59a14f",
        "#edc948", "#b07aa1", "#ff9da7", "#9c755f", "#bab0ac"
    };

    private const double MarginLeft = 48;
    private const double MarginRight = 16;
    private const double MarginTop = 16;
    private const double MarginBottom = 40;

    protected double W { get; set; } = 480;
    protected double H { get; set; } = 300;

    protected List<GraphPoint> Points { get; set; } = new();
    protected List<string> YTicks { get; set; } = new();
    protected Dictionary<string, double> YTickPositions { get; set; } = new();
    protected string LinePoints { get; set; } = string.Empty;
    protected List<GraphBar> Bars { get; set; } = new();
    protected List<GraphSlice> PieSegments { get; set; } = new();
    protected List<GraphHeatCell> HeatCells { get; set; } = new();

    protected override void OnParametersSet()
    {
        (W, H) = Size switch
        {
            GraphSize.Small => (320, 200),
            GraphSize.Medium => (480, 300),
            GraphSize.Large => (720, 420),
            GraphSize.ExtraLarge => (960, 560),
            _ => (600, 360)
        };

        ComputeCartesian();
        ComputePie();
        ComputeHeatmap();
    }

    private List<(string Label, double Value)> RawValues()
        => Data.Select(d => (GetString(d, XAxis), GetNumber(d, YAxis))).ToList();

    private void ComputeCartesian()
    {
        Points.Clear();
        Bars.Clear();
        YTicks.Clear();
        YTickPositions.Clear();
        LinePoints = string.Empty;

        var raw = RawValues();
        if (raw.Count == 0) return;

        var min = raw.Min(r => r.Value);
        var max = raw.Max(r => r.Value);
        if (min == max) { max = min + 1; }

        var plotW = W - MarginLeft - MarginRight;
        var plotH = H - MarginTop - MarginBottom;
        var n = raw.Count;

        for (var i = 0; i < n; i++)
        {
            var x = n == 1 ? MarginLeft + plotW / 2 : MarginLeft + (i / (double)(n - 1)) * plotW;
            var y = MarginTop + (1 - (raw[i].Value - min) / (max - min)) * plotH;
            Points.Add(new GraphPoint { Label = raw[i].Label, Value = raw[i].Value, X = x, Y = y });
        }

        for (var t = 0; t <= 4; t++)
        {
            var val = min + (t / 4.0) * (max - min);
            var y = MarginTop + (1 - t / 4.0) * plotH;
            var label = val % 1 == 0 ? val.ToString("0") : val.ToString("0.##");
            YTicks.Add(label);
            YTickPositions[label] = y;
        }

        LinePoints = string.Join(" ", Points.Select(p => $"{p.X:0.##},{p.Y:0.##}"));

        var slot = plotW / n;
        var barW = slot * 0.7;
        for (var i = 0; i < n; i++)
        {
            var x = MarginLeft + i * slot + (slot - barW) / 2;
            var y = Points[i].Y;
            var height = (MarginTop + plotH) - y;
            Bars.Add(new GraphBar { X = x, Y = y, Width = barW, Height = Math.Max(0, height), Label = Points[i].Label });
        }
    }

    private void ComputePie()
    {
        PieSegments.Clear();
        var raw = RawValues().Where(r => r.Value > 0).ToList();
        var total = raw.Sum(r => r.Value);
        if (total <= 0) return;

        var cx = W / 2;
        var cy = H / 2;
        var r = Math.Min(W, H) / 2 - 12;

        var angle = 0d;
        for (var i = 0; i < raw.Count; i++)
        {
            var sweep = raw[i].Value / total;
            var start = angle;
            var end = angle + sweep;
            var mid = (start + end) / 2;
            var color = Palette[i % Palette.Length];
            PieSegments.Add(new GraphSlice
            {
                Path = PieSlice(cx, cy, r, start, end),
                Color = color,
                Label = raw[i].Label,
                Value = raw[i].Value,
                Percent = (int)Math.Round(sweep * 100),
                LabelX = cx + r * 0.62 * Math.Cos(2 * Math.PI * mid - Math.PI / 2),
                LabelY = cy + r * 0.62 * Math.Sin(2 * Math.PI * mid - Math.PI / 2) + 4
            });
            angle = end;
        }
    }

    private static string PieSlice(double cx, double cy, double r, double start, double end)
    {
        var s = 2 * Math.PI * start - Math.PI / 2;
        var e = 2 * Math.PI * end - Math.PI / 2;
        var x1 = cx + r * Math.Cos(s);
        var y1 = cy + r * Math.Sin(s);
        var x2 = cx + r * Math.Cos(e);
        var y2 = cy + r * Math.Sin(e);
        var large = (end - start) > 0.5 ? 1 : 0;
        return $"M{cx:0.##},{cy:0.##} L{x1:0.##},{y1:0.##} A{r:0.##},{r:0.##} 0 {large} 1 {x2:0.##},{y2:0.##} Z";
    }

    private void ComputeHeatmap()
    {
        HeatCells.Clear();
        var raw = RawValues();
        if (raw.Count == 0) return;

        var min = raw.Min(r => r.Value);
        var max = raw.Max(r => r.Value);
        if (min == max) max = min + 1;

        var cols = (int)Math.Ceiling(Math.Sqrt(raw.Count));
        var rows = (int)Math.Ceiling(raw.Count / (double)cols);
        var plotW = W - MarginLeft - MarginRight;
        var plotH = H - MarginTop - MarginBottom;
        var size = Math.Min(plotW / cols, plotH / rows);

        for (var i = 0; i < raw.Count; i++)
        {
            var c = i % cols;
            var r = i / cols;
            var norm = (raw[i].Value - min) / (max - min);
            HeatCells.Add(new GraphHeatCell
            {
                X = MarginLeft + c * size,
                Y = MarginTop + r * size,
                Size = size,
                Color = HeatColor(norm),
                Label = raw[i].Label,
                ShowLabel = size > 44
            });
        }
    }

    private static string HeatColor(double t)
    {
        t = Math.Max(0, Math.Min(1, t));
        var low = (R: 247, G: 251, B: 255);
        var high = (R: 8, G: 48, B: 107);
        var r = (int)(low.R + (high.R - low.R) * t);
        var g = (int)(low.G + (high.G - low.G) * t);
        var b = (int)(low.B + (high.B - low.B) * t);
        return $"rgb({r},{g},{b})";
    }

    private static string GetString(object item, string property)
    {
        var value = item?.GetType().GetProperty(property)?.GetValue(item);
        return value?.ToString() ?? string.Empty;
    }

    private static double GetNumber(object item, string property)
    {
        var value = item?.GetType().GetProperty(property)?.GetValue(item);
        return value switch
        {
            double d => d,
            int i => i,
            long l => l,
            float f => f,
            decimal m => (double)m,
            string s when double.TryParse(s, out var parsed) => parsed,
            _ => 0
        };
    }

    public class GraphPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class GraphBar
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    public class GraphSlice
    {
        public string Path { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public int Percent { get; set; }
        public double LabelX { get; set; }
        public double LabelY { get; set; }
    }

    public class GraphHeatCell
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool ShowLabel { get; set; }
    }

    private MarkupString SvgText(double x, double y, string anchor, string cssClass, string content)
        => (MarkupString)$"<text x=\"{x:0.##}\" y=\"{y:0.##}\" text-anchor=\"{anchor}\" class=\"{cssClass}\">{content}</text>";
}
