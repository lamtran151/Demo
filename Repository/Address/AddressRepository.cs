using MyrStore.API.Infrastructure.Entities;
using MyrStore.Common.Caching;
using MyrStore.Common.Infrastructure.Repository;

namespace MyrStore.API.Infrastructure.Repositories
{
    public partial class AddressRepository : IAddressRepository
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<Country> _countryRepository;

        #endregion

        #region Ctor

        public AddressRepository(
            IStaticCacheManager staticCacheManager,
            IRepository<Country> countryRepository)
        {
            _staticCacheManager = staticCacheManager;
            _countryRepository = countryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false)
        {
            var countries = await _countryRepository.GetAllAsync(async query =>
            {
                if (!showHidden)
                    query = query.Where(c => c.Published);

                return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
            });
            return countries;
        }

        #endregion
    }
}
