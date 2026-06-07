using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BugTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void Bug_InitialState_IsOpen()
    {
        var bug = new Bug("Test");
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Title_IsCorrect()
    {
        var bug = new Bug("My bug");
        Assert.AreEqual("My bug", bug.Title);
    }

    [TestMethod]
    public void Bug_Assign_TransitionsToAssigned()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        Assert.AreEqual(Bug.State.Assigned, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Assign_SetsAssigneeName()
    {
        var bug = new Bug("Test");
        bug.Assign("Bob");
        Assert.AreEqual("Bob", bug.Assignee);
    }

    [TestMethod]
    public void Bug_StartWork_TransitionsToInProgress()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        Assert.AreEqual(Bug.State.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Resolve_TransitionsToResolved()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        Assert.AreEqual(Bug.State.Resolved, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Verify_TransitionsToVerified()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        Assert.AreEqual(Bug.State.Verified, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Close_TransitionsToClosed()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_FullCycle_EndsClosed()
    {
        var bug = new Bug("Test");
        bug.Assign("Dev");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        Assert.AreEqual(Bug.State.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenFromClosed_IsOpen()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenFromResolved_IsOpen()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenFromVerified_IsOpen()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenFromAssigned_IsOpen()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.Reopen();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenFromInProgress_IsOpen()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Defer_TransitionsToDeferred()
    {
        var bug = new Bug("Test");
        bug.Defer();
        Assert.AreEqual(Bug.State.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Activate_TransitionsDeferredToOpen()
    {
        var bug = new Bug("Test");
        bug.Defer();
        bug.Activate();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_Reject_TransitionsToRejected()
    {
        var bug = new Bug("Test");
        bug.Reject();
        Assert.AreEqual(Bug.State.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_ReopenFromRejected_IsOpen()
    {
        var bug = new Bug("Test");
        bug.Reject();
        bug.Reopen();
        Assert.AreEqual(Bug.State.Open, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_DeferFromAssigned_IsDeferred()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.Defer();
        Assert.AreEqual(Bug.State.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_RejectFromDeferred_IsRejected()
    {
        var bug = new Bug("Test");
        bug.Defer();
        bug.Reject();
        Assert.AreEqual(Bug.State.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void Bug_CanFire_AssignFromOpen_IsTrue()
    {
        var bug = new Bug("Test");
        Assert.IsTrue(bug.CanFire(Bug.Trigger.Assign));
    }

    [TestMethod]
    public void Bug_CanFire_StartWorkFromOpen_IsFalse()
    {
        var bug = new Bug("Test");
        Assert.IsFalse(bug.CanFire(Bug.Trigger.StartWork));
    }

    [TestMethod]
    public void Bug_ClosedState_OnlyReopenAllowed()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        var triggers = bug.PermittedTriggers.ToList();
        Assert.AreEqual(1, triggers.Count);
        Assert.AreEqual(Bug.Trigger.Reopen, triggers[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_StartWorkFromOpen_ThrowsException()
    {
        var bug = new Bug("Test");
        bug.StartWork();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_ResolveFromOpen_ThrowsException()
    {
        var bug = new Bug("Test");
        bug.Resolve();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_CloseFromOpen_ThrowsException()
    {
        var bug = new Bug("Test");
        bug.Close();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_ActivateFromOpen_ThrowsException()
    {
        var bug = new Bug("Test");
        bug.Activate();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Bug_AssignFromClosed_ThrowsException()
    {
        var bug = new Bug("Test");
        bug.Assign("Alice");
        bug.StartWork();
        bug.Resolve();
        bug.Verify();
        bug.Close();
        bug.Assign("Bob");
    }
}
