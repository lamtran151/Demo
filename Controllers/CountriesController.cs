using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyrStore.API.Infrastructure.Services.Address;
using MyrStore.Common.Entity;
using System.Net;

namespace MyrStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IAddressService _addressApiService;

        public CountriesController(
            IAddressService addressApiService)
        {
            _addressApiService = addressApiService;
        }

        /// <summary>
        ///     Receive a list of all Countries
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/countries", Name = "GetCountries")]
        public async Task<Result> GetCountries([FromQuery] bool? mustAllowBilling = null, [FromQuery] bool? mustAllowShipping = null)
        {
            Result result = new Result();
            try
            {
                result.ResultData = await _addressApiService.GetAllCountriesAsync(mustAllowBilling ?? false, mustAllowShipping ?? false);
                return result;
            }
            catch (Exception ex)
            {
                return new Result()
                {
                    Success = false,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
