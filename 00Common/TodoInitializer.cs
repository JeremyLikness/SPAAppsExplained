using System;

namespace _00Common
{
    public static class TodoInitializer
    {
        public static void Initialize(TodoRepository repo)
        {
            repo.Save(new TodoItem
            {
                Description = "Prepare demos for presentation.",
                DueDate = new DateTimeOffset(new DateTime(2015, 03, 04)),
                IsDone = true
            });

            repo.Save(new TodoItem
            {
                Description = "Research demos.",
                DueDate = new DateTimeOffset(new DateTime(2015, 02, 20)),
                IsDone = true
            });

            repo.Save(new TodoItem
            {
                Description = "Prepare slide deck for presentation.",
                DueDate = new DateTimeOffset(new DateTime(2015, 03, 08)),
                IsDone = true
            });

            repo.Save(new TodoItem
            {
                Description = "Comment about past due items.",
                DueDate = new DateTimeOffset(new DateTime(2015, 03, 07)),
                IsDone = false
            });

            repo.Save(new TodoItem
            {
                Description = "Present the topic to a live web audience.",
                DueDate = new DateTimeOffset(new DateTime(2015, 03, 25)),
                IsDone = false
            });

            repo.Save(new TodoItem
            {
                Description = "Respond to feedback received after the event is done.",
                DueDate = new DateTimeOffset(new DateTime(2015, 04, 01)),
                IsDone = false
            });
        }
    }
}
