using MyrStore.API.Infrastructure.DTOs;
using MyrStore.Common.Infrastructure.Services;

namespace MyrStore.API.Infrastructure.Services.Address
{
    public interface IAddressService : ITransientService
    {
        Task<IList<CountryDto>> GetAllCountriesAsync(bool mustAllowBilling = false, bool mustAllowShipping = false);
    }
}
