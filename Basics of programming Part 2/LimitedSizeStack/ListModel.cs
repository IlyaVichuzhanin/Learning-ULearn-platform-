﻿using System;
using System.Collections.Generic;
using TodoApplication;

namespace LimitedSizeStack;

public interface ICommand
{
    //void ExecuteAction();
    void UndoAction();
}

public enum ItemAction
{
    AddItem,
    RemoveItem
}

public class ItemCommand<TItem> : ICommand
{
    private readonly ListModel<TItem> listModel;
    private readonly TItem item;
	private readonly ItemAction action;
    private readonly int index;

    public ItemCommand(ListModel<TItem> listModel, TItem item, ItemAction action, int removeIndex)
	{
		this.listModel = listModel;
		this.item = item;
		this.action = action;
        this.index = removeIndex;
    }

    //public ItemCommand(ListModel<TItem> listModel, ItemAction action, int removeIndex)
    //{
    //    this.listModel = listModel;
    //    this.action = action;
    //    this.index = removeIndex;
    //}

	//  public void ExecuteAction()
	//  {
	//if (action == ItemAction.AddItem)
	//	listModel.AddItem(item);
	//else
	//	listModel.RemoveItem(index);
	//  }

	public void UndoAction()
	{
		if (action == ItemAction.AddItem)
			listModel.RemoveItem(index);
		else
			listModel.AddItem(item);
	}
}

public class Modifier<TItem>
{
    public readonly LimitedSizeStack<ICommand> commands;
	private ICommand command;
    public Modifier(int stackLimit)
	{
		this.commands = new LimitedSizeStack<ICommand>(stackLimit);
    }

	public void SetCommand(ICommand command) => this.command = command;
	public void Invoke()
	{
		commands.Push(command);
		//command.ExecuteAction();
	}
}


public class ListModel<TItem>
{
	public List<TItem> Items { get; }
	public int UndoLimit;
	public Modifier<TItem> Modifier;
        
	public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
	{
	}

	public ListModel(List<TItem> items, int undoLimit)
	{
		Items = items;
		UndoLimit = undoLimit;
		Modifier = new Modifier<TItem>(undoLimit);
	}

    public void AddItem(TItem item)
	{
		Items.Add(item);
		Modifier.SetCommand(new ItemCommand<TItem>(this, item, ItemAction.AddItem, Items.Count));
		Modifier.Invoke();
	}

	public void RemoveItem(int index)
	{
		Items.RemoveAt(index);
        //Modifier.SetCommand(new ItemCommand<TItem>(this, Items[index], ItemAction.RemoveItem, index));
		Modifier.Invoke();
    }

	public bool CanUndo()
	{
        return Modifier.commands.Count > 0;
    }

	public void Undo()
	{
		Modifier.commands.Pop().UndoAction();
	}
}