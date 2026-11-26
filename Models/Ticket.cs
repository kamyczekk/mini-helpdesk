using System.ComponentModel;

namespace MiniHelpDesk.Models;

/*
klasa: Ticket

opis:
Przechowuje dane zgłoszenia i powiadamia widok o ich zmianie.

pola / właściwości:
Id, int, unikalny identyfikator
Name, string, nazwa zgłoszenia
Description, string, opis problemu
Priority, string, priorytet
Status, string, bieżący status
PropertyChanged, PropertyChangedEventHandler?, zdarzenie zmiany właściwości
*/
internal sealed class Ticket(int id, string name, string description, string priority) : INotifyPropertyChanged
{
    public int Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public string Priority { get; private set; } = priority;
    public string Status { get; private set; } = TicketOptions.New;

    public event PropertyChangedEventHandler? PropertyChanged;

    /*
    funkcja: Edit

    opis:
    Zapisuje zwalidowane dane zgłoszenia i zgłasza zmienione właściwości.

    parametry:
    name, string, nazwa zgłoszenia
    description, string, opis problemu
    priority, string, priorytet zgłoszenia

    zwraca:
    void, nic nie zwraca
    */
    internal void Edit(string name, string description, string priority)
    {
        Update(Name, name, value => Name = value, nameof(Name));
        Update(Description, description, value => Description = value, nameof(Description));
        Update(Priority, priority, value => Priority = value, nameof(Priority));
    }

    /*
    funkcja: ChangeStatus

    opis:
    Aktualizuje status i wysyła powiadomienie, jeśli wartość się zmieniła.

    parametry:
    status, string, nowy status

    zwraca:
    void, nic nie zwraca
    */
    internal void ChangeStatus(string status) =>
        Update(Status, status, value => Status = value, nameof(Status));

    private void Update(string current, string next, Action<string> assign, string property)
    {
        if (current == next)
            return;

        assign(next);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}