using MiniHelpDesk.Views;
using MiniHelpDesk.ViewModels;
using MiniHelpDesk.Cqrs;
using MiniHelpDesk.Data;

namespace MiniHelpDesk;

public sealed class App : Application
{
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var repository = new TicketRepository();
        var viewModel = new TicketViewModel(MauiProgram.CreateBus(repository), new GetTicketsQuery(repository));
        return new Window(new MainPage(viewModel))
        {
            Title = "Mini HelpDesk",
            Width = 1090,
            Height = 775,
            MinimumWidth = 850,
            MinimumHeight = 560
        };
    }
}