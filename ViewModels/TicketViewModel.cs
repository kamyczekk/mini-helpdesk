using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MiniHelpDesk.Cqrs;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.ViewModels;

/*
klasa: TicketViewModel

opis:
Łączy widok z zapytaniem i magistralą komend. Zarządza formularzem, wyborem zgłoszenia i licznikami.

pola / właściwości:
Tickets, ReadOnlyObservableCollection<Ticket>, lista powiązana z widokiem
SelectedTicket, Ticket?, zgłoszenie wybrane do edycji
Name, string?, nazwa w formularzu
Description, string?, opis w formularzu
Priority, string?, wybrany priorytet
SelectedStatus, string?, status wybranego zgłoszenia
NameError, string?, błąd walidacji nazwy
DescriptionError, string?, błąd walidacji opisu
PriorityError, string?, błąd walidacji priorytetu
TotalCount, int, liczba wszystkich zgłoszeń
OpenCount, int, liczba niezakończonych zgłoszeń
SaveCommand, Command, zapis formularza
CancelEditCommand, Command, zakończenie edycji formularza
DeleteCommand, Command, usunięcie wybranego zgłoszenia
*/
internal sealed class TicketViewModel : INotifyPropertyChanged
{
    private readonly CommandBus _bus;
    private Ticket? _selectedTicket;
    private string? _name;
    private string? _description;
    private string? _priority;
    private bool _showAllErrors;
    private TicketErrors _errors = TicketValidator.Validate(null, null, null);

    public TicketViewModel(CommandBus bus, GetTicketsQuery query)
    {
        _bus = bus;
        Tickets = query.Execute();
        foreach (var ticket in Tickets)
            ticket.PropertyChanged += TicketChanged;

        ((INotifyCollectionChanged)Tickets).CollectionChanged += TicketsChanged;
        SaveCommand = new Command(Save);
        CancelEditCommand = new Command(() => SelectedTicket = null);
        DeleteCommand = new Command(Delete);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReadOnlyObservableCollection<Ticket> Tickets { get; }
    public IList<string> Priorities => TicketOptions.Priorities;
    public IList<string> Statuses => TicketOptions.Statuses;
    public Command SaveCommand { get; }
    public Command CancelEditCommand { get; }
    public Command DeleteCommand { get; }

    public int TotalCount => Tickets.Count;
    public int OpenCount => Tickets.Count(ticket => ticket.Status != TicketOptions.Closed);
    public bool IsEditing => SelectedTicket is not null;
    public string FormTitle => SelectedTicket is { } ticket ? $"Edycja zgłoszenia #{ticket.Id}" : "Nowe zgłoszenie";
    public string SaveLabel => IsEditing ? "Zapisz zmiany" : "Dodaj zgłoszenie";

    public Ticket? SelectedTicket
    {
        get => _selectedTicket;
        set
        {
            if (_selectedTicket == value)
                return;

            _selectedTicket = value;
            LoadForm(value);
            Notify();
            Notify(nameof(SelectedStatus));
            Notify(nameof(FormTitle));
            Notify(nameof(IsEditing));
            Notify(nameof(SaveLabel));
        }
    }

    public string? Name
    {
        get => _name;
        set => SetFormField(ref _name, value);
    }

    public string? Description
    {
        get => _description;
        set => SetFormField(ref _description, value);
    }

    public string? Priority
    {
        get => _priority;
        set => SetFormField(ref _priority, value);
    }

    public string? SelectedStatus
    {
        get => SelectedTicket?.Status;
        set
        {
            if (SelectedTicket is { } ticket && value is not null && value != ticket.Status)
                _bus.Send(new ChangeTicketStatusCommand(ticket.Id, value));
        }
    }

    public string? NameError => Name is not null || _showAllErrors ? _errors.Name : null;
    public string? DescriptionError => Description is not null || _showAllErrors ? _errors.Description : null;
    public string? PriorityError => _showAllErrors ? _errors.Priority : null;
    public bool HasNameError => NameError is not null;
    public bool HasDescriptionError => DescriptionError is not null;
    public bool HasPriorityError => PriorityError is not null;

    /*
    funkcja: Save

    opis:
    Zapisuje dane formularza komendą dodania lub edycji po pomyślnej walidacji.

    parametry:
    brak

    zwraca:
    void, nic nie zwraca
    */
    private void Save()
    {
        _showAllErrors = true;
        Validate();
        if (!_errors.IsValid)
            return;

        if (SelectedTicket is { } ticket)
        {
            _bus.Send(new EditTicketCommand(ticket.Id, Name!, Description!, Priority!));
            SelectedTicket = null;
            return;
        }

        _bus.Send(new AddTicketCommand(Name!, Description!, Priority!));
        LoadForm(null);
    }

    private void LoadForm(Ticket? ticket)
    {
        _name = ticket?.Name;
        _description = ticket?.Description;
        _priority = ticket?.Priority;
        _showAllErrors = false;
        Notify(nameof(Name));
        Notify(nameof(Description));
        Notify(nameof(Priority));
        Validate();
    }

    private void Delete()
    {
        if (SelectedTicket is not { } ticket)
            return;

        SelectedTicket = null;
        _bus.Send(new DeleteTicketCommand(ticket.Id));
    }

    private void TicketsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
            foreach (Ticket ticket in e.NewItems)
                ticket.PropertyChanged += TicketChanged;

        if (e.OldItems is not null)
            foreach (Ticket ticket in e.OldItems)
                ticket.PropertyChanged -= TicketChanged;

        Notify(nameof(TotalCount));
        Notify(nameof(OpenCount));
    }

    private void TicketChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Ticket.Status))
            return;

        Notify(nameof(OpenCount));
        if (ReferenceEquals(sender, SelectedTicket))
            Notify(nameof(SelectedStatus));
    }

    private void SetFormField(ref string? field, string? value, [CallerMemberName] string property = "")
    {
        if (field == value)
            return;

        field = value;
        Notify(property);
        Validate();
    }

    private void Validate()
    {
        _errors = TicketValidator.Validate(Name, Description, Priority);
        Notify(nameof(NameError));
        Notify(nameof(DescriptionError));
        Notify(nameof(PriorityError));
        Notify(nameof(HasNameError));
        Notify(nameof(HasDescriptionError));
        Notify(nameof(HasPriorityError));
    }

    private void Notify([CallerMemberName] string property = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
}