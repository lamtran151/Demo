using MyrStore.API.Infrastructure.DTOs;
using MyrStore.API.Infrastructure.Entities;
using MyrStore.API.Infrastructure.Repositories;
using MyrStore.Common.Infrastructure.Mapper;

namespace MyrStore.API.Infrastructure.Services.Address
{
    public class AddressService : IAddressService
    {
        #region Fields

        private readonly IAddressRepository _addressRepository;

        #endregion

        #region Ctor

        public AddressService(
            IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        #endregion

        #region Methods

        public async Task<IList<CountryDto>> GetAllCountriesAsync(bool mustAllowBilling = false, bool mustAllowShipping = false)
        {
            IEnumerable<Country> countries = await _addressRepository.GetAllCountriesAsync();
            if (mustAllowBilling)
                countries = countries.Where(c => c.AllowsBilling);
            if (mustAllowShipping)
                countries = countries.Where(c => c.AllowsShipping);
            return countries.Select(c => c.MapTo<Country, CountryDto>()).ToList();
        }

        #endregion
    }
}
