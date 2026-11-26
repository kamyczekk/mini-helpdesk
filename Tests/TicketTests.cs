using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniHelpDesk.Cqrs;
using MiniHelpDesk.Data;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.Tests;

/*
klasa: TicketTests

opis:
Sprawdza walidację, obsługę komend oraz zdarzenia kolekcji i zgłoszenia. Każdy test zapisu korzysta z nowego repozytorium.

pola / właściwości:
Name, string, nazwa przykładowego zgłoszenia
Description, string, opis przykładowego zgłoszenia
Priority, string, priorytet przykładowego zgłoszenia
*/
internal static class TicketTests
{
    private const string Name = "Awaria Wi-Fi";
    private const string Description = "Komputer nie łączy się z siecią.";
    private const string Priority = "Wysoki";

    public static void NameAfterTrim()
    {
        Assert.IsNotNull(TicketValidator.Validate(" ab ", Description, Priority).Name);
        Assert.IsNull(TicketValidator.Validate(" abc ", Description, Priority).Name);
    }

    public static void DescriptionAfterTrim()
    {
        Assert.IsNotNull(TicketValidator.Validate(Name, " 123456789 ", Priority).Description);
        Assert.IsNull(TicketValidator.Validate(Name, " 1234567890 ", Priority).Description);
    }

    public static void RequiredPriority()
    {
        var (bus, query) = CreateContext();
        Assert.IsNotNull(TicketValidator.Validate(Name, Description, null).Priority);
        Assert.ThrowsExactly<ArgumentException>(() => bus.Send(new AddTicketCommand(Name, Description, "Pilny")));
        Assert.IsEmpty(query.Execute());
    }

    public static void AddAndQuery()
    {
        var (bus, query) = CreateContext();
        bus.Send(new AddTicketCommand($" {Name} ", $" {Description} ", Priority));
        bus.Send(new AddTicketCommand(Name, Description, Priority));
        var tickets = query.Execute();
        Assert.HasCount(2, tickets);
        Assert.AreEqual(Name, tickets[0].Name);
        Assert.AreEqual(Description, tickets[0].Description);
        Assert.AreEqual(Priority, tickets[0].Priority);
        Assert.AreEqual(TicketOptions.New, tickets[0].Status);
        Assert.AreNotEqual(tickets[0].Id, tickets[1].Id);
    }

    public static void DeleteAndNotify()
    {
        var (bus, query) = CreateContext();
        var tickets = query.Execute();
        var actions = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)tickets).CollectionChanged += (_, e) => actions.Add(e.Action);
        bus.Send(new AddTicketCommand(Name, Description, Priority));
        bus.Send(new DeleteTicketCommand(tickets.Single().Id));

        Assert.IsEmpty(tickets);
        CollectionAssert.AreEqual(new[] { NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove }, actions);
    }

    public static void ChangeStatusAndNotify()
    {
        var (bus, query) = CreateContext();
        bus.Send(new AddTicketCommand(Name, Description, Priority));
        var tickets = query.Execute();
        var ticket = tickets.Single();
        var properties = new List<string?>();
        var collectionChanges = 0;
        ticket.PropertyChanged += (_, e) => properties.Add(e.PropertyName);
        ((INotifyCollectionChanged)tickets).CollectionChanged += (_, _) => collectionChanges++;

        bus.Send(new ChangeTicketStatusCommand(ticket.Id, TicketOptions.InProgress));
        Assert.AreEqual(TicketOptions.InProgress, ticket.Status);
        bus.Send(new ChangeTicketStatusCommand(ticket.Id, TicketOptions.Closed));
        Assert.AreEqual(TicketOptions.Closed, ticket.Status);
        bus.Send(new ChangeTicketStatusCommand(ticket.Id, TicketOptions.Closed));
        Assert.ThrowsExactly<ArgumentException>(() => bus.Send(new ChangeTicketStatusCommand(ticket.Id, "Inny")));
        Assert.AreEqual(TicketOptions.Closed, ticket.Status);
        bus.Send(new ChangeTicketStatusCommand(ticket.Id, TicketOptions.New));

        Assert.AreSame(ticket, tickets.Single());
        Assert.AreEqual(TicketOptions.New, ticket.Status);
        Assert.AreEqual(0, collectionChanges);
        CollectionAssert.AreEqual(new[] { "Status", "Status", "Status" }, properties);
    }

    public static void EditAndNotify()
    {
        var (bus, query) = CreateContext();
        bus.Send(new AddTicketCommand(Name, Description, Priority));
        var ticket = query.Execute().Single();
        var properties = new List<string?>();
        ticket.PropertyChanged += (_, e) => properties.Add(e.PropertyName);

        bus.Send(new EditTicketCommand(ticket.Id, " Naprawa drukarki ", " Drukarka nie pobiera papieru. ", "Niski"));

        Assert.AreSame(ticket, query.Execute().Single());
        Assert.AreEqual("Naprawa drukarki", ticket.Name);
        Assert.AreEqual("Drukarka nie pobiera papieru.", ticket.Description);
        Assert.AreEqual("Niski", ticket.Priority);
        Assert.AreEqual(TicketOptions.New, ticket.Status);
        CollectionAssert.AreEqual(new[] { "Name", "Description", "Priority" }, properties);
    }

    public static void RejectInvalidEdit()
    {
        var (bus, query) = CreateContext();
        bus.Send(new AddTicketCommand(Name, Description, Priority));
        var ticket = query.Execute().Single();

        Assert.ThrowsExactly<ArgumentException>(() => bus.Send(new EditTicketCommand(ticket.Id, " a ", "Za krótko", "Niski")));

        Assert.AreEqual(Name, ticket.Name);
        Assert.AreEqual(Description, ticket.Description);
        Assert.AreEqual(Priority, ticket.Priority);
    }

    private static (CommandBus Bus, GetTicketsQuery Query) CreateContext()
    {
        var repository = new TicketRepository();
        return (MauiProgram.CreateBus(repository), new GetTicketsQuery(repository));
    }
}