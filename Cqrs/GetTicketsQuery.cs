using System.Collections.ObjectModel;
using MiniHelpDesk.Data;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.Cqrs;

/*
klasa: GetTicketsQuery

opis:
Odczytuje obserwowalną listę zgłoszeń z repozytorium.

pola / właściwości:
repository, TicketRepository, źródło zgłoszeń
*/
internal sealed class GetTicketsQuery(TicketRepository repository)
{
    /*
    funkcja: Execute

    opis:
    Pobiera listę do odczytu i powiązania z widokiem.

    parametry:
    brak

    zwraca:
    ReadOnlyObservableCollection<Ticket>, aktualna lista zgłoszeń
    */
    public ReadOnlyObservableCollection<Ticket> Execute() => repository.Tickets;
}