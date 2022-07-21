using MyrStore.API.Infrastructure.Entities;
using MyrStore.Common.Infrastructure.Repository;

namespace MyrStore.API.Infrastructure.Repositories
{
    public partial interface IAddressRepository : ITransientRepository
    {
        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false);
    }
}
