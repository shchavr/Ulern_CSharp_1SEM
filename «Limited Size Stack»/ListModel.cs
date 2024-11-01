using System;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class ListModel<TItem>
{
    private readonly List<TItem> items;
    private readonly LimitedSizeStack<ICommand<TItem>> undoStack;
    private readonly CommandInvoker<TItem> commandInvoker;
    private readonly ListOperationsExecutor<TItem> operationsHandler;

    public List<TItem> Items { get; }
    public int UndoLimit { get; }

    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
    }

    public ListModel(List<TItem> initialList, int maxUndoActions)
    {
        Items = initialList;
        UndoLimit = maxUndoActions;
        undoStack = new LimitedSizeStack<ICommand<TItem>>(maxUndoActions);
        commandInvoker = new CommandInvoker<TItem>(initialList);
        operationsHandler = new ListOperationsExecutor<TItem>();
    }

    public void AddItem(TItem newItem)
    {
        var addOperation = new AddCommand<TItem>(newItem, Items.Count, operationsHandler);
        commandInvoker.SetCommand(addOperation);
        commandInvoker.Execute();
        undoStack.Push(addOperation);
    }

    public void RemoveItem(int itemPosition)
    {
        var removeOperation = new RemoveCommand<TItem>(Items[itemPosition], itemPosition, operationsHandler);
        commandInvoker.SetCommand(removeOperation);
        commandInvoker.Execute();
        undoStack.Push(removeOperation);
    }

    public void Undo()
    {
        if (CanUndo())
        {
            var redoableCommand = undoStack.Pop();
            commandInvoker.SetCommand(redoableCommand);
            commandInvoker.Undo();
        }
        else throw new InvalidOperationException(); 
    }

    public bool CanUndo()
    {
        return undoStack.Count > 0;
    }
}

public interface ICommand<TItem>
{
    void Execute(List<TItem> items);
    void Undo(List<TItem> items);
}

public class AddCommand<TItem> : ICommand<TItem>
{
    private readonly TItem collectionItem;
    private readonly int itemPosition;
    private readonly ListOperationsExecutor<TItem> listOperationsExecutor;

    public AddCommand(TItem itemAdd, int position, ListOperationsExecutor<TItem> executor)
    {
        collectionItem = itemAdd;
        itemPosition = position;
        listOperationsExecutor = executor;
    }

    public void Execute(List<TItem> items)
    {
        listOperationsExecutor.Add(collectionItem, items, itemPosition);
    }

    public void Undo(List<TItem> items)
    {
        listOperationsExecutor.Remove(itemPosition, items);
    }
}

public class RemoveCommand<TItem> : ICommand<TItem>
{
    private readonly TItem targetItem;
    private readonly int targetIndex;
    private readonly ListOperationsExecutor<TItem> listOperationsExecutor;

    public RemoveCommand(TItem element, int removalIndex, ListOperationsExecutor<TItem> operationsHandler)
    {
        targetItem = element;
        targetIndex = removalIndex;
        listOperationsExecutor = operationsHandler;
    }

    public void Execute(List<TItem> items)
    {
        listOperationsExecutor.Remove(targetIndex, items);
    }

    public void Undo(List<TItem> items)
    {
        listOperationsExecutor.Add(targetItem, items, targetIndex);
    }
}

public class ListOperationsExecutor<TItem>
{
    public void Add(TItem item, List<TItem> itemsCollection, int index)
    {
        itemsCollection.Insert(index, item);
    }

    public void Remove(int index, List<TItem> itemsCollection)
    {
        itemsCollection.RemoveAt(index);
    }
}

public class CommandInvoker<TItem>
{
    private ICommand<TItem> executedCommand;
    private readonly List<TItem> managedList;

    public CommandInvoker(List<TItem> itemsCollection)
    {
        managedList = itemsCollection;
    }

    public void SetCommand(ICommand<TItem> operation)
    {
        executedCommand = operation;
    }

    public void Execute()
    {
        executedCommand.Execute(managedList);
    }

    public void Undo()
    {
        executedCommand.Undo(managedList);
    }
}
