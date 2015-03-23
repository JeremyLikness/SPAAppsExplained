using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using _00Common;
using _02AfterSPA.Models;

namespace _02AfterSPA.api
{
    [RoutePrefix("api/todo")]
    public class TodoController : ApiController
    {
        private static readonly TodoRepository Repo = new TodoRepository();

        private static readonly IDictionary
                <string,
                    Tuple
                        <Func<IQueryable<TodoItem>, IQueryable<TodoItem>>,
                            Func<IQueryable<TodoItem>, IQueryable<TodoItem>>>>
            Sorts =
                new Dictionary
                    <string,
                        Tuple
                            <Func<IQueryable<TodoItem>, IQueryable<TodoItem>>,
                                Func<IQueryable<TodoItem>, IQueryable<TodoItem>>>>();

        static TodoController()
        {
            TodoInitializer.Initialize(Repo);
            Sorts.Add("Id", Tuple.Create<Func<IQueryable<TodoItem>, IQueryable<TodoItem>>,
                                Func<IQueryable<TodoItem>, IQueryable<TodoItem>>>(
                                q => q.OrderBy(i => i.Id), q => q.OrderByDescending(i => i.Id)));
            Sorts.Add("Completed", Tuple.Create<Func<IQueryable<TodoItem>, IQueryable<TodoItem>>,
                                Func<IQueryable<TodoItem>, IQueryable<TodoItem>>>(
                                q => q.OrderBy(i => i.IsDone), q => q.OrderByDescending(i => i.IsDone)));
            Sorts.Add("Description", Tuple.Create<Func<IQueryable<TodoItem>, IQueryable<TodoItem>>,
                                Func<IQueryable<TodoItem>, IQueryable<TodoItem>>>(
                                q => q.OrderBy(i => i.Description), q => q.OrderByDescending(i => i.Description)));
            Sorts.Add("DueDate", Tuple.Create<Func<IQueryable<TodoItem>, IQueryable<TodoItem>>,
                                Func<IQueryable<TodoItem>, IQueryable<TodoItem>>>(
                                q => q.OrderBy(i => i.DueDate), q => q.OrderByDescending(i => i.DueDate)));
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]TodoFilter filter)
        {
            var query = Repo.Get();
            if (filter == null)
            {
                return Ok(query.ToList());
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterText))
            {
                query = query.Where(t => t.Description.Contains(filter.FilterText));
            }

            if (string.IsNullOrWhiteSpace(filter.ColumnName))
            {
                return Ok(query.ToList());
            }

            if (!Sorts.ContainsKey(filter.ColumnName))
            {
                return Ok(query.ToList());
            }

            var sort = Sorts[filter.ColumnName];
            query = filter.SortAscending ? sort.Item1(query) : sort.Item2(query);
            return Ok(query.ToList());
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                Repo.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPut]
        public IHttpActionResult Add(TodoItem newItem)
        {
            try
            {
                return Ok(Repo.Save(newItem));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPost]
        public IHttpActionResult Update(TodoItem item)
        {
            try
            {
                return Ok(Repo.Save(item));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
