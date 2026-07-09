using Auth.UI.Components.UI.AutocompleteSelect;
using Auth.UI.Components.UI.Breadcrumb;
using Auth.UI.Components.UI.Button;
using Auth.UI.Components.UI.Calendar;
using Auth.UI.Components.UI.Card;
using Auth.UI.Components.UI.Carousel;
using Auth.UI.Components.UI.Checkbox;
using Auth.UI.Components.UI.Collapse;
using Auth.UI.Components.UI.Datepicker;
using Auth.UI.Components.UI.Drawer;
using Auth.UI.Components.UI.Form;
using Auth.UI.Components.UI.Graph;
using Auth.UI.Components.UI.Menu;
using Auth.UI.Components.UI.Modal;
using Auth.UI.Components.UI.Pagination;
using Auth.UI.Components.UI.Progress;
using Auth.UI.Components.UI.Radio;
using Auth.UI.Components.UI.Rate;
using Auth.UI.Components.UI.Slider;
using Auth.UI.Components.UI.Steps;
using Auth.UI.Components.UI.Switch;
using Auth.UI.Components.UI.Table;
using Auth.UI.Components.UI.Tabs;
using Auth.UI.Components.UI.Tag;
using Auth.UI.Components.UI.TimePicker;
using Auth.UI.Components.UI.Timeline;
using Auth.UI.Components.UI.Toaster;
using Auth.UI.Components.UI.Tour;
using Auth.UI.Components.UI.Upload;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Auth.UI.Components.Pages.Shared;

public partial class Showcase : ComponentBase
{
    [Inject] private ToasterService ToasterService { get; set; } = default!;

    private Modal _modal = default!;
    private ConfirmationModal _confirm = default!;
    private Tour _tour = default!;
    private Drawer _drawer = default!;

    private string ItemName { get; set; } = string.Empty;
    private string SelectedDept { get; set; } = string.Empty;

    private List<BreadcrumbItem> BreadcrumbItems { get; set; } = new()
    {
        new() { Text = "Home", Url = "/", Icon = "info" },
        new() { Text = "Library", Url = "/showcase" },
        new() { Text = "Components" }
    };

    private List<SelectItem> Departments { get; set; } = new()
    {
        new() { Id = "1", Value = "Engineering" },
        new() { Id = "2", Value = "Sales" },
        new() { Id = "3", Value = "Marketing" },
        new() { Id = "4", Value = "Support" }
    };

    private List<TableColumn<Person>> Columns { get; set; } = new()
    {
        new() { Title = "Name", Property = nameof(Person.Name) },
        new() { Title = "Age", Property = nameof(Person.Age), Format = "0" },
        new() { Title = "Department", Property = nameof(Person.Department) },
        new() { Title = "Active", Property = nameof(Person.Active) },
        new() { Title = "Actions", Sortable = false, Template = person => (RenderTreeBuilder builder) =>
        {
            builder.OpenComponent<Button>(0);
            builder.AddAttribute(1, "Variant", Button.ButtonVariant.Light);
            builder.AddAttribute(2, "Size", Button.ButtonSize.Small);
            builder.AddAttribute(3, "IconName", "edit");
            builder.CloseComponent();
        } }
    };

    private List<Person> People { get; set; } = new();
    private List<ChartPoint> ChartData { get; set; } = new();

    protected override void OnInitialized()
    {
        for (var i = 1; i <= 23; i++)
        {
            People.Add(new Person
            {
                Name = $"Person {i}",
                Age = 20 + (i % 40),
                Department = Departments[i % Departments.Count].Value,
                Active = i % 3 != 0
            });
        }

        ChartData = new()
        {
            new() { Label = "Mon", Value = 12 },
            new() { Label = "Tue", Value = 19 },
            new() { Label = "Wed", Value = 8 },
            new() { Label = "Thu", Value = 24 },
            new() { Label = "Fri", Value = 15 },
            new() { Label = "Sat", Value = 30 },
            new() { Label = "Sun", Value = 21 }
        };
    }

    private void Notify(string message) => ToasterService.ShowInfo(message);

    private Task OpenModal() => _modal.ShowAsync();
    private Task OpenConfirm() => _confirm.ShowAsync();

    private async Task SaveAsync()
    {
        await Task.Delay(400);
        ToasterService.ShowSuccess($"Saved '{ItemName}'");
        await _modal.HideAsync();
    }

    private Task ConfirmedAsync()
    {
        ToasterService.ShowDanger("Item deleted");
        return Task.CompletedTask;
    }

    private Task DeclinedAsync()
    {
        ToasterService.ShowWarning("Delete cancelled");
        return Task.CompletedTask;
    }

    private void OnDeptChanged(string id) => SelectedDept = id;

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Department { get; set; } = string.Empty;
        public bool Active { get; set; }
    }

    public class ChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    private List<object> MenuItems { get; set; } = new()
    {
        new MenuActionItem { Text = "Profile", Icon = "info", Key = "profile" },
        new MenuActionItem { Text = "Settings", Icon = "edit", Key = "settings" },
        new MenuDivider(),
        new MenuHeaderItem { Text = "More" },
        new MenuLinkItem { Text = "Help", Url = "/help" },
        new MenuActionItem { Text = "Logout", Icon = "alert-circle", Key = "logout", Disabled = true }
    };

    private Task OnMenuAction(MenuActionItem item)
    {
        Notify($"Menu: {item.Key}");
        return Task.CompletedTask;
    }

    private int PaginationPage { get; set; } = 1;
    private Task OnPaginationPageChanged(int page)
    {
        PaginationPage = page;
        return Task.CompletedTask;
    }

    private List<StepItem> StepItems { get; set; } = new()
    {
        new() { Title = "Account", Description = "Create account" },
        new() { Title = "Profile", Description = "Setup profile" },
        new() { Title = "Confirm", Description = "Review details" }
    };
    private int StepActiveIndex { get; set; } = 0;
    private Task NextStep()
    {
        if (StepActiveIndex < StepItems.Count - 1) StepActiveIndex++;
        return Task.CompletedTask;
    }
    private Task PrevStep()
    {
        if (StepActiveIndex > 0) StepActiveIndex--;
        return Task.CompletedTask;
    }

    private List<TabItem> TabItems { get; set; } = new()
    {
        new() { Title = "Home", Icon = "info", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Home tab content")) },
        new() { Title = "Profile", Icon = "edit", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Profile tab content")) },
        new() { Title = "Settings", Icon = "sliders", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Settings tab content")) }
    };
    private TabItem ActiveTab { get; set; } = new();

    private Task OnTabChanged(TabItem tab)
    {
        ActiveTab = tab;
        return Task.CompletedTask;
    }

    private bool CheckboxChecked { get; set; } = true;
    private Task OnCheckboxChanged(bool value)
    {
        CheckboxChecked = value;
        return Task.CompletedTask;
    }

    private DateTime? DatepickerValue { get; set; } = DateTime.Today;
    private Task OnDatepickerChanged(DateTime? date)
    {
        DatepickerValue = date;
        return Task.CompletedTask;
    }
    private DateTime? DatepickerPopupValue { get; set; }
    private Task OnDatepickerPopupChanged(DateTime? date)
    {
        DatepickerPopupValue = date;
        return Task.CompletedTask;
    }

    private ShowcaseFormModel FormModel { get; set; } = new();
    private Task OnFormValidSubmit(EditContext ctx)
    {
        Notify($"Form submitted: {FormModel.Name}");
        return Task.CompletedTask;
    }
    private Task OnFormInvalidSubmit(EditContext ctx)
    {
        Notify("Form has errors");
        return Task.CompletedTask;
    }
    public class ShowcaseFormModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private string RadioGroup { get; set; } = "demo-radio";
    private string? RadioSelected { get; set; } = "A";
    private List<string> RadioOptions { get; set; } = new() { "A", "B", "C" };
    private Task OnRadioChanged(string? value)
    {
        RadioSelected = value;
        return Task.CompletedTask;
    }

    private int RateValue { get; set; } = 3;
    private Task OnRateChanged(int value)
    {
        RateValue = value;
        return Task.CompletedTask;
    }

    private double SliderValue { get; set; } = 50;
    private Task OnSliderChanged(double value)
    {
        SliderValue = value;
        return Task.CompletedTask;
    }
    private double SliderValue2 { get; set; } = 250;
    private Task OnSlider2Changed(double value)
    {
        SliderValue2 = value;
        return Task.CompletedTask;
    }

    private bool SwitchChecked { get; set; } = true;
    private Task OnSwitchChanged(bool value)
    {
        SwitchChecked = value;
        return Task.CompletedTask;
    }

    private TimeOnly? TimepickerValue { get; set; } = new TimeOnly(9, 30);
    private Task OnTimepickerChanged(TimeOnly? time)
    {
        TimepickerValue = time;
        return Task.CompletedTask;
    }
    private TimeOnly? TimepickerPopupValue { get; set; }
    private Task OnTimepickerPopupChanged(TimeOnly? time)
    {
        TimepickerPopupValue = time;
        return Task.CompletedTask;
    }

    private DateTime? CalendarValue { get; set; } = DateTime.Today;
    private DateTime CalendarViewDate { get; set; } = DateTime.Today;
    private Task OnCalendarChanged(DateTime? date)
    {
        CalendarValue = date;
        if (date.HasValue) CalendarViewDate = date.Value;
        return Task.CompletedTask;
    }

    private List<CarouselSlide> CarouselSlides { get; set; } = new()
    {
        new() { Caption = "Slide 1", ImageUrl = "https://picsum.photos/seed/slide1/800/360" },
        new() { Caption = "Slide 2", ImageUrl = "https://picsum.photos/seed/slide2/800/360" },
        new() { Caption = "Slide 3", ImageUrl = "https://picsum.photos/seed/slide3/800/360" }
    };
    private int CarouselIndex { get; set; } = 0;
    private Task OnCarouselIndexChanged(int idx)
    {
        CarouselIndex = idx;
        return Task.CompletedTask;
    }

    private List<CollapseItem> CollapseItems { get; set; } = new()
    {
        new() { Title = "What is this?", Content = (RenderFragment)(__builder => __builder.AddContent(0, "This is a Blazor component library.")) },
        new() { Title = "How to use?", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Use components directly in your pages.")) },
        new() { Title = "Is it free?", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Yes, it is free to use.")) }
    };

    private List<TimelineItem> TimelineItems { get; set; } = new()
    {
        new() { Time = "10:00", Title = "Project start", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Kickoff meeting")) },
        new() { Time = "12:30", Title = "Design", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Mockups approved")) },
        new() { Time = "15:00", Title = "Deploy", Content = (RenderFragment)(__builder => __builder.AddContent(0, "Released to production")) }
    };

    private bool TourVisible { get; set; } = false;
    private int TourStep { get; set; } = 0;
    private List<string> TourSteps { get; set; } = new()
    {
        "Welcome to the UI library showcase.",
        "Browse all components in one page.",
        "Click Next or Finish to close the tour."
    };
    private Task StartTour()
    {
        TourStep = 0;
        TourVisible = true;
        return Task.CompletedTask;
    }
    private Task NextTourStep()
    {
        if (TourStep < TourSteps.Count - 1) TourStep++;
        else EndTour();
        return Task.CompletedTask;
    }
    private Task PrevTourStep()
    {
        if (TourStep > 0) TourStep--;
        return Task.CompletedTask;
    }
    private Task EndTour()
    {
        TourVisible = false;
        return Task.CompletedTask;
    }

    private bool DrawerVisible { get; set; } = false;
    private Task OpenDrawer()
    {
        DrawerVisible = true;
        return Task.CompletedTask;
    }

    private double ProgressValue { get; set; } = 65;
    private double ProgressValue2 { get; set; } = 140;

    private Task OnUploadSelected(IBrowserFile file)
    {
        Notify($"Uploaded: {file.Name}");
        return Task.CompletedTask;
    }

    private Task OnUploadRemoved(IBrowserFile file)
    {
        Notify($"Removed: {file.Name}");
        return Task.CompletedTask;
    }

    private Task OnTagClose()
    {
        Notify("Tag closed");
        return Task.CompletedTask;
    }
}
