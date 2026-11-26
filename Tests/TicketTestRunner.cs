namespace MiniHelpDesk.Tests;

/*
klasa: TicketTestRunner

opis:
Uruchamia testy z poziomu aplikacji. Każdy przypadek używa asercji MSTest.

pola / właściwości:
Cases, (string, Action)[], podpisy i metody testowe
*/
internal static class TicketTestRunner
{
    private static readonly (string Name, Action Execute)[] Cases =
    [
        ("Długość nazwy po przycięciu", TicketTests.NameAfterTrim),
        ("Długość opisu po przycięciu", TicketTests.DescriptionAfterTrim),
        ("Obowiązkowy priorytet", TicketTests.RequiredPriority),
        ("Dodawanie i odczyt", TicketTests.AddAndQuery),
        ("Usuwanie i powiadomienie listy", TicketTests.DeleteAndNotify),
        ("Zmiana statusu i powiadomienie", TicketTests.ChangeStatusAndNotify),
        ("Edycja i powiadomienia pól", TicketTests.EditAndNotify),
        ("Odrzucenie błędnej edycji", TicketTests.RejectInvalidEdit)
    ];

    /*
    funkcja: Run

    opis:
    Wykonuje wszystkie przypadki i tworzy tekstowy raport dla widoku.

    parametry:
    brak

    zwraca:
    (string Title, string Report), podsumowanie i wynik każdego testu
    */
    public static (string Title, string Report) Run()
    {
        var passed = 0;
        var results = new List<string>();
        foreach (var (name, execute) in Cases)
        {
            try
            {
                execute();
                passed++;
                results.Add($"OK · {name}");
            }
            catch (Exception error)
            {
                results.Add($"Błąd · {name}\n{error.Message}");
            }
        }

        return ($"Testy MSTest · {passed}/{Cases.Length}", string.Join("\n", results));
    }
}