using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace _00Common
{
    /// <summary>
    /// This is for a simple, single user demo so no synchronization or thread-safety has been implemented.
    /// </summary>
    public class TodoRepository
    {
        private readonly List<TodoItem> _todoItems = new List<TodoItem>();
        private static readonly Random Random = new Random();

        private static TodoItem Clone(TodoItem todoItem)
        {
            Thread.Sleep(Random.Next(10, 50));
            var result = new TodoItem();
            Map(todoItem, result);
            return result;
        }

        private static void Map(TodoItem source, TodoItem destination)
        {
            destination.Id = source.Id;
            destination.Description = source.Description;
            destination.DueDate = source.DueDate;
            destination.IsDone = source.IsDone;           
        }

        public TodoItem Get(int id)
        {
            var result = _todoItems.FirstOrDefault(i => i.Id == id);
            return result == null ? null : Clone(result);
        }

        public IQueryable<TodoItem> Get()
        {
            return _todoItems.Select(Clone).AsQueryable();
        }

        public TodoItem Save(TodoItem item)
        {
            if (item.Id == 0)
            {
                item.Id = _todoItems.Any() ? _todoItems.Max(i => i.Id) + 1 : 1;
                _todoItems.Add(Clone(item));
            }
            else
            {
                var repoItem = _todoItems.FirstOrDefault(i => i.Id == item.Id);
                if (repoItem == null)
                {
                    throw new InvalidOperationException();
                }
                Map(item, repoItem);
            }

            return Get(item.Id);
        }

        public void Delete(int id)
        {
            var repoItem = _todoItems.FirstOrDefault(i => i.Id == id);
            if (repoItem == null)
            {
                return;
            }
            _todoItems.Remove(repoItem);
        }
    }
}
