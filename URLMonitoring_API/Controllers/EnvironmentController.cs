using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using URLMonitoring_API.Data;
using URLMonitoring_API.DTOs;
using URLMonitoring_API.Model;

namespace URLMonitoring_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EnvironmentController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<EnvironmentController>
        //use IActionResult when you return multiple return type like badrequest, ok within the same end point
        //however when returning just one model type regardles of the conditions, use ActionResult<T>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var environments = _mapper.Map<IEnumerable<EnvironmentDTO>>(
                    await _context.Environments
                    .Include(a => a.Collection)
                    .ToListAsync()
                );
            return Ok(environments);
        }


        [HttpGet("~/GetPaginatedEnvironments")]
        public async Task<IActionResult> Get([FromQuery] PaginationParameters filter)
        {
            PaginationDefaultParameter paginationDefaultParameter = new PaginationDefaultParameter();

            var pData = await _context.Environments
                .Include(b => b.Collection)
                .OrderBy(a => a.Name)
                .Select(ev => new EnvironmentReadDTO()
                {
                    Id = ev.Id,
                    Name = ev.Name,
                    Deleteable = (ev.Collection != null) ? false : true
                })
                .ToListAsync();

            //var environmentList = await _context.Environments
            //            //.Include(a => a.EnvironmentVars)
            //            .Include(b => b.Collection)
            //            .OrderBy(a => a.Name)
            //            .ToListAsync();
            //IList<EnvironmentReadDTO> pData = new List<EnvironmentReadDTO>();

            //foreach (var e in environmentList)
            //{
            //    pData.Add(new EnvironmentReadDTO { 
            //        Id = e.Id, 
            //        Name = e.Name, 
            //        Deleteable = (e.Collection != null) ? false : true //e.EnvironmentVars.Count() > 0 || 
            //    });
            //}

            var environments = PagedList<EnvironmentReadDTO>.ToPagedList(
                    pData.AsQueryable(),
                    filter.PageNumber,
                    filter.PageSize
                );

            var metadata = new
            {
                environments.TotalCount,
                environments.PageSize,
                environments.CurrentPage,
                environments.TotalPages,
                environments.HasNext,
                environments.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(environments);
        }

        //GET api/<EnvironmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if(id < 0)
            {
                return BadRequest();
            }

            return Ok(
                    _mapper.Map<EnvironmentDTO>(
                        await _context.Environments.FindAsync(id)
                    )
                );
        }

        // POST api/<EnvironmentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody][Bind("Name")] EnvironmentPostDTO environmentDTO)
        {
            //var environment = _mapper.Map<Data.Environment>(environmentDTO);

            if (ModelState.IsValid)
            {
                Data.Environment environment = new Data.Environment() { Name = environmentDTO.Name };
                _context.Add(environment);
                await _context.SaveChangesAsync();
                return CreatedAtAction("Get", new { id = environment.Id }
                    , _mapper.Map<EnvironmentDTO>(environment));
            }
            return BadRequest();
        }

        // PUT api/<EnvironmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody][Bind("Name")] EnvironmentPostDTO environmentDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var environment = await _context.Environments.FindAsync(id);

            if (environment == null)
            {
                return NotFound();
            }

            environment.Name = environmentDTO.Name;
            try
            {
                _context.Update(environment);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //throw;
                return  Problem("Database Issue: "+ e.Message + ". (Inner Exception - " + e.InnerException + ")");
            }
            
        }

        // DELETE api/<EnvironmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var environment = await _context.Environments
                .Include(a => a.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (environment == null)
            {
                return NotFound();
            }
            else if (environment.Collection != null)
            {
                //return new ResponseMessage { };
                return Conflict(new ResponseMessage { Message = "One or more collection associated to this environment." });
            }

            try
            {
                _context.Environments.Remove(environment);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //throw;
                return Problem("Database Issue: " + e.Message + ". (Inner Exception - " + e.InnerException + ")");
            }
        }

        // DELETE api/<EnvironmentController>/5
        [HttpDelete("~/Deleteenvironments")]
        public async Task<IActionResult> Delete(DeletionIds idList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (idList == null)
            {
                return NotFound();
            }

            List<Environment> environments = new List<Environment>{};
            foreach (var id in idList.Ids)
            {
                var environment = await _context.Environments
                .Include(a => a.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);

                if (environment != null && environment.Collection == null)
                {
                    environments.Add(environment);
                }
            }

            if (!(environments.Count > 0))
            {
                return NotFound();
            }


            try
            {
                _context.Environments.RemoveRange(environments);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //throw;
                return Problem("Database Issue: " + e.Message + ". (Inner Exception - " + e.InnerException + ")");
            }
        }
    }
}
