using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Data.Entities;
using TheCodeCamp.Data.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps")]
    public class CampsController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        [Route()]
        public async Task<IHttpActionResult> Get(bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetAllCampsAsync(includeTalks);

                // Mapping

                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                //TODO Add Logging
                return InternalServerError(ex);
            }

        }

        [HttpGet]
        [Route("{moniker}", Name = "GetCamp")]
        public async Task<IHttpActionResult> GetCamp(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker, includeTalks);

                if (result == null) return NotFound();

                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("searchByDate/{eventDate:datetime}")]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var result = await _repository.GetAllCampsByEventDate(eventDate, includeTalks);

                return Ok(_mapper.Map<CampModel[]>(result));
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [Route()]
        [HttpPost]
        public async Task<IHttpActionResult> CreateCamp(CampModel campModel)
        {
            try
            {
                if (await _repository.GetCampAsync(campModel.Moniker) != null) ModelState.AddModelError("Moniker", "Moniker in use");
                if (ModelState.IsValid)
                {

                    var camp = _mapper.Map<Camp>(campModel);

                    _repository.AddCamp(camp);

                    if (await _repository.SaveChangesAsync())
                    {
                        var newModel = _mapper.Map<CampModel>(camp);

                        return CreatedAtRoute("GetCamp", new { moniker = newModel.Moniker }, newModel);
                    }
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }
        [HttpPut]
        [Route("{moniker}")]
        public async Task<IHttpActionResult> UpdateCamp(string moniker, CampModel campModel)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);

                if (camp == null) return NotFound();

                _mapper.Map(campModel, camp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<CampModel>(camp));
                }

                return InternalServerError();
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{moniker}")]
        public async Task<IHttpActionResult> DeleteCamp(string moniker)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);

                if (camp == null) return NotFound();

                _repository.DeleteCamp(camp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}