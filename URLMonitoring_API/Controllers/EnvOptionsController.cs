using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using URLMonitoring_API.Data;
using URLMonitoring_API.DTOs;
using URLMonitoring_API.Model;
using URLMonitoring_API.Repo;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace URLMonitoring_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EnvOptionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EnvOptionsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<EnvOptionsController>
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            if (id < 1)
            {
                return NotFound();
            }

            //var environmentVarDTO = from ev in _context.EnvironmentVars.Where(m => m.EnvironmentId == envId).ToList()
            //                        join e in _context.Environments.ToList() on ev.EnvironmentId equals e.Id //into t1
            //                        join p in _context.pParams.ToList() on ev.TypeId equals p.Id //into t1
            //                                                                                     //from i in t1.ToList()
            //                        select new EnvironmentVarDTO
            //                        {
            //                            Id = ev.Id,
            //                            EnvironmentId = ev.EnvironmentId,
            //                            Variable = ev.Variable,
            //                            TypeId = ev.TypeId,
            //                            Value = ev.Value,
            //                            EnvironmentName = ev.environment.Name,
            //                            TypeName = ev.pParam.Code
            //                        };

            var environmentVarDT = await _context.EnvironmentVars
                .Include(e => e.environment)
                .Include(e => e.pParam)
                .Where(m => m.EnvironmentId == id)
                .Select(ev => new EnvironmentVarDTO()
                {
                    Id = ev.Id,
                    EnvironmentId = ev.EnvironmentId,
                    Variable = ev.Variable,
                    TypeId = ev.TypeId,
                    Value = (ev.pParam.Code == "Secret" ? DataCryptography.DecryptString(ev.Value) :  ev.Value),
                    EnvironmentName = ev.environment.Name,
                    TypeName = ev.pParam.Code
                })
                .ToListAsync();


            //var env = (await _context.Environments.FindAsync(id)).Name;

            return Ok(environmentVarDT);
        }

        [HttpGet("~/GetPaginatedEnvOptions")]
        public async Task<IActionResult> Get(int id,[FromQuery] PaginationParameters filter)
        {
            if (id < 1)
            {
                return NotFound();
            }

            PaginationDefaultParameter paginationDefaultParameter = new PaginationDefaultParameter();

            var pData = await _context.EnvironmentVars
                .Include(e => e.environment)
                .Include(e => e.pParam)
                .Where(m => m.EnvironmentId == id)
                .Select(ev => new EnvironmentVarDTO()
                {
                    Id = ev.Id,
                    EnvironmentId = ev.EnvironmentId,
                    Variable = ev.Variable,
                    TypeId = ev.TypeId,
                    Value = (ev.pParam.Code == "Secret" ? DataCryptography.DecryptString(ev.Value) : ev.Value),
                    EnvironmentName = ev.environment.Name,
                    TypeName = ev.pParam.Code,
                    Deleteable = true //to be determined later
                })
                .ToListAsync();

            var envOptions = PagedList<EnvironmentVarDTO>.ToPagedList(
                    pData.AsQueryable(),
                    filter.PageNumber,
                    filter.PageSize
                );

            var metadata = new
            {
                envOptions.TotalCount,
                envOptions.PageSize,
                envOptions.CurrentPage,
                envOptions.TotalPages,
                envOptions.HasNext,
                envOptions.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(envOptions);
        }

        // GET api/<EnvOptionsController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<EnvOptionsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody][Bind("EnvironmentId,Variable,TypeId,Value")] EnvOptPostVarDTO environmentVarDTO)
        {
            if(environmentVarDTO == null)
            {
                return BadRequest();
            }

            var environmentVar = _mapper.Map<EnvironmentVar>(environmentVarDTO);

            //if(environmentVar.TypeId == (await GetEnvOptiontypeSecretId()))
            //    environmentVar.Value = DataCryptography.EncryptString(environmentVar.Value);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(environmentVar);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("Get", new { id = environmentVar.Id });
                }
                catch (Microsoft.Data.SqlClient.SqlException e)
                {
                    return Problem("Database Issue: " + e.Message + ". (Inner Exception - " + e.InnerException + ")");
                }
            }

            return BadRequest();
        }

        // PUT api/<EnvOptionsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody][Bind("Variable,TypeId,Value")] EnvironmentPutVarDTO environmentVarDTO)
        {
            if (ModelState.IsValid)
            {
                var environmentVar = await _context.EnvironmentVars.Where(a => a.Id == id).FirstOrDefaultAsync();

                if (environmentVar == null || environmentVarDTO == null)
                {
                    return BadRequest();
                }

                var SecretId = await GetEnvOptiontypeSecretId();

                environmentVar.Variable = environmentVarDTO.Variable;
                environmentVar.TypeId = environmentVarDTO.TypeId;
                environmentVar.Value = (
                    environmentVar.TypeId == SecretId ? DataCryptography.EncryptString(environmentVar.Value) :
                    environmentVarDTO.Value
                );

                try
                {
                    _context.Update(environmentVar);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    return Problem("Database Issue: " + e.Message + ". (Inner Exception - " + e.InnerException + ")");
                }
            }
            return BadRequest();
        }

        // DELETE api/<EnvOptionsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var environmentVar = await _context.EnvironmentVars.FindAsync(id);

            if (environmentVar == null)
            {
                return BadRequest();
            }

            try
            {
                _context.EnvironmentVars.Remove(environmentVar);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Problem("Database Issue: " + e.Message + ". (Inner Exception - " + e.InnerException + ")");
            }
        }

        // DELETE api/<EnvironmentController>/5
        [HttpDelete("~/DeleteEnvOptions")]
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

            List<EnvironmentVar> environmentVars = new List<EnvironmentVar> { };

            foreach (var id in idList.Ids)
            {
                var environmentVar = await _context.EnvironmentVars.FindAsync(id);

                if (environmentVar != null)
                {
                    environmentVars.Add(environmentVar);
                }
            }

            if (!(environmentVars.Count > 0))
            {
                return NotFound();
            }

            try
            {
                _context.EnvironmentVars.RemoveRange(environmentVars);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //throw;
                return Problem("Database Issue: " + e.Message + ". (Inner Exception - " + e.InnerException + ")");
            }
        }

        [Route("GetEnvironmentVariableType")]
        [HttpGet]
        public async Task<IActionResult> GetEnvironmentVariableType()
        {
            var envTypes = await _context.pParams.Where(a => a.Type == "ENVOPTTYPE")
                .Select(b => new EnvVarTypeDTO
                {
                    Id = b.Id,
                    Code = b.Code,
                    Description = b.Description,
                })
                .ToListAsync();
            return Ok(envTypes);
        }

        private async Task<int> GetEnvOptiontypeSecretId()
        {
            var row = await _context.pParams.Where(a => a.Type == "ENVOPTTYPE" && a.Code == "Secret").FirstOrDefaultAsync();
            return row.Id;
        }


    }
}
