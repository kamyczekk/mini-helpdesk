using System.Collections.ObjectModel;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.Data;

/*
klasa: TicketRepository

opis:
Przechowuje zgłoszenia w pamięci i udostępnia obserwowalny widok listy.

pola / właściwości:
_items, ObservableCollection<Ticket>, zgłoszenia w pamięci
_nextId, int, ostatni przydzielony identyfikator
Tickets, ReadOnlyObservableCollection<Ticket>, widok listy do odczytu
*/
internal sealed class TicketRepository
{
    private readonly ObservableCollection<Ticket> _items = [];
    private int _nextId;

    public TicketRepository() => Tickets = new(_items);

    public ReadOnlyObservableCollection<Ticket> Tickets { get; }

    /*
    funkcja: Add

    opis:
    Przydziela identyfikator i dodaje nowe zgłoszenie.

    parametry:
    name, string, nazwa zgłoszenia
    description, string, opis problemu
    priority, string, priorytet zgłoszenia

    zwraca:
    void, nic nie zwraca
    */
    public void Add(string name, string description, string priority) =>
        _items.Add(new Ticket(++_nextId, name, description, priority));

    /*
    funkcja: FindById

    opis:
    Wyszukuje zgłoszenie po identyfikatorze. Zgłasza błąd, jeśli zgłoszenie nie istnieje.

    parametry:
    id, int, identyfikator szukanego zgłoszenia

    zwraca:
    Ticket, znalezione zgłoszenie
    */
    public Ticket FindById(int id) => _items.FirstOrDefault(ticket => ticket.Id == id)
        ?? throw new KeyNotFoundException($"Nie znaleziono zgłoszenia nr {id}.");

    /*
    funkcja: Remove

    opis:
    Usuwa zgłoszenie o podanym identyfikatorze.

    parametry:
    id, int, identyfikator usuwanego zgłoszenia

    zwraca:
    void, nic nie zwraca
    */
    public void Remove(int id) => _items.Remove(FindById(id));
}