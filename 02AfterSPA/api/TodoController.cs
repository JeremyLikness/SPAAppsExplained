using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using _00Common;
using _02AfterSPA.Models;

namespace _02AfterSPA.api
{
    /// <summary>
    /// This is the REST interface for the to-do list
    /// </summary>
    /// <remarks>
    /// To learn more about the Web API, watch my Web API Jumpstart:
    /// http://csharperimage.jeremylikness.com/2015/01/web-api-design-jumpstart.html 
    /// </remarks>
    [RoutePrefix("api/todo")]
    public class TodoController : ApiController
    {
        /// <summary>
        /// Only for demos
        /// </summary>
        private static readonly TodoRepository Repo = new TodoRepository();

        /// <summary>
        /// This maps a column name to the strategy for sorting it 
        /// </summary>
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

        /// <summary>
        /// This will get the list of items
        /// </summary>
        /// <remarks>
        /// By default, complex types are parsed from the body. This is "get" so we want to be explicit
        /// about the parts of the complex type being passed as query string parameters
        /// </remarks>
        /// <param name="filter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete it 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add a new item 
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update an existing item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
