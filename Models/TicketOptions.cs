namespace MiniHelpDesk.Models;

/*
klasa: TicketOptions

opis:
Udostępnia listy wartości używane przez widok i walidację.

pola / właściwości:
New, string, status nowego zgłoszenia
InProgress, string, status zgłoszenia w trakcie obsługi
Closed, string, status zakończonego zgłoszenia
Priorities, IList<string>, dostępne priorytety
Statuses, IList<string>, dostępne statusy
*/
internal static class TicketOptions
{
    public const string New = "Nowe";
    public const string InProgress = "W trakcie";
    public const string Closed = "Zamknięte";

    public static IList<string> Priorities { get; } = Array.AsReadOnly(new[] { "Niski", "Średni", "Wysoki" });
    public static IList<string> Statuses { get; } = Array.AsReadOnly(new[] { New, InProgress, Closed });
}