using System;

namespace _00Common
{
    public class TodoItem : IEquatable<TodoItem>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public DateTimeOffset DueDate { get; set; }

        
        public bool Equals(TodoItem other)
        {
            return other != null && other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
