using Stateless;

// --- Демонстрация работы ---
var bug1 = new Bug("Login page crash");
Console.WriteLine($"New bug: {bug1.CurrentState}");
bug1.Assign("Alice");
bug1.StartWork();
bug1.Resolve();
bug1.Verify();
bug1.Close();
Console.WriteLine($"After full cycle: {bug1.CurrentState}");

var bug2 = new Bug("UI glitch");
bug2.Defer();
Console.WriteLine($"Deferred bug: {bug2.CurrentState}");
bug2.Activate();
bug2.Assign("Bob");
bug2.StartWork();
bug2.Resolve();
bug2.Reopen();
Console.WriteLine($"Reopened bug: {bug2.CurrentState}");

var bug3 = new Bug("Invalid request");
bug3.Reject();
Console.WriteLine($"Rejected bug: {bug3.CurrentState}");

// --- Класс Bug ---
public class Bug
{
    public enum State
    {
        Open, Assigned, InProgress, Resolved, Verified, Closed, Deferred, Rejected
    }

    public enum Trigger
    {
        Assign, StartWork, Resolve, Verify, Close, Reopen, Defer, Reject, Activate
    }

    private readonly StateMachine<State, Trigger> _machine;
    public string Title { get; }
    public string? Assignee { get; private set; }

    public Bug(string title, State initialState = State.Open)
    {
        Title = title;
        _machine = new StateMachine<State, Trigger>(initialState);

        _machine.Configure(State.Open)
            .Permit(Trigger.Assign, State.Assigned)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reject, State.Rejected);

        _machine.Configure(State.Assigned)
            .Permit(Trigger.StartWork, State.InProgress)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reject, State.Rejected)
            .Permit(Trigger.Reopen, State.Open);

        _machine.Configure(State.InProgress)
            .Permit(Trigger.Resolve, State.Resolved)
            .Permit(Trigger.Defer, State.Deferred)
            .Permit(Trigger.Reopen, State.Open);

        _machine.Configure(State.Resolved)
            .Permit(Trigger.Verify, State.Verified)
            .Permit(Trigger.Reopen, State.Open);

        _machine.Configure(State.Verified)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Reopen, State.Open);

        _machine.Configure(State.Closed)
            .Permit(Trigger.Reopen, State.Open);

        _machine.Configure(State.Deferred)
            .Permit(Trigger.Activate, State.Open)
            .Permit(Trigger.Reject, State.Rejected);

        _machine.Configure(State.Rejected)
            .Permit(Trigger.Reopen, State.Open);
    }

    public State CurrentState => _machine.State;

    public void Assign(string assignee) { Assignee = assignee; _machine.Fire(Trigger.Assign); }
    public void StartWork() => _machine.Fire(Trigger.StartWork);
    public void Resolve() => _machine.Fire(Trigger.Resolve);
    public void Verify() => _machine.Fire(Trigger.Verify);
    public void Close() => _machine.Fire(Trigger.Close);
    public void Reopen() => _machine.Fire(Trigger.Reopen);
    public void Defer() => _machine.Fire(Trigger.Defer);
    public void Reject() => _machine.Fire(Trigger.Reject);
    public void Activate() => _machine.Fire(Trigger.Activate);

    public bool CanFire(Trigger trigger) => _machine.CanFire(trigger);
    public IEnumerable<Trigger> PermittedTriggers => _machine.GetPermittedTriggers();
}
