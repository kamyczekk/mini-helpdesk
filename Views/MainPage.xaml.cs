using MiniHelpDesk.Tests;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Views;

public partial class MainPage : ContentPage
{
    internal MainPage(TicketViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void RunTests(object? sender, EventArgs e)
    {
        var (title, report) = TicketTestRunner.Run();
        await DisplayAlertAsync(title, report, "Zamknij");
    }
}