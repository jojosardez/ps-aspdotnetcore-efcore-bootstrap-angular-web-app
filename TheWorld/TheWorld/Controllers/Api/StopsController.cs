using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopsController> _logger;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName);

                return Ok(
                    Mapper.Map<IEnumerable<StopViewModel>>
                    (
                        trip.Stops.OrderBy(s => s.Order).ToList()
                    ));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops {0}", ex);
            }

            return BadRequest("Failed to get stops");
        }
    }
}
