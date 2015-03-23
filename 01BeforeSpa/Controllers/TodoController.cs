using System.Linq;
using System.Web.Mvc;
using _00Common;
using _01BeforeSpa.Models;

namespace _01BeforeSpa.Controllers
{
    public class TodoController : Controller
    {
        private static readonly TodoRepository Repo;

        static TodoController()
        {
            Repo = new TodoRepository();
            TodoInitializer.Initialize(Repo);
        }
        
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.TodoItems = Repo.Get().ToList();
            return View();            
        }

        [HttpPost]
        public ActionResult Index(TodoActions actions)
        {
            if (actions.RemoveDone == "on")
            {
                foreach (var item in Repo.Get().Where(t => t.IsDone).ToList())
                {
                    Repo.Delete(item.Id);
                }
            }

            if (actions.MarkComplete > 0)
            {
                var item = Repo.Get(actions.MarkComplete);
                if (item != null)
                {
                    item.IsDone = true;
                    Repo.Save(item);
                }
            }

            var query = Repo.Get();
            if (!string.IsNullOrWhiteSpace(actions.SortBy))
            {
                if (actions.SortDescending == "on")
                {
                    switch (actions.SortBy)
                    {
                        case "Id":
                            query = query.OrderByDescending(t => t.Id);
                            break;
                        case "Completed":
                            query = query.OrderByDescending(t => t.IsDone);
                            break;
                        case "Description":
                            query = query.OrderByDescending(t => t.Description);
                            break;
                        case "DueDate":
                            query = query.OrderByDescending(t => t.DueDate);
                            break;
                    }
                }
                else
                {
                    switch (actions.SortBy)
                    {
                        case "Id":
                            query = query.OrderBy(t => t.Id);
                            break;
                        case "Completed":
                            query = query.OrderBy(t => t.IsDone);
                            break;
                        case "Description":
                            query = query.OrderBy(t => t.Description);
                            break;
                        case "DueDate":
                            query = query.OrderBy(t => t.DueDate);
                            break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(actions.FilterText))
            {
                query = query.Where(t => t.Description.Contains(actions.FilterText));
            }

            ViewBag.TodoItems = query.ToList();
            return View();
        }
    }
}