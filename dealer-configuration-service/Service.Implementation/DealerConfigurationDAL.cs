using Service.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class DealerConfigurationDAL : IDealerConfigurationDAL
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor,it is responsible for resolve dependency using Unity.
        /// </summary>
        /// <param name="connectionString"></param>
        public DealerConfigurationDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Add dealer configuration 
        /// </summary>
        /// <param name="objDealerConfiguration"></param>
        /// <returns></returns>
        public async Task<int> AddDealerConfiguration(DealerConfiguration objDealerConfiguration)
        {
            using (ConfigurationManagerContext context = new ConfigurationManagerContext(_connectionString))
            {
                context.DealConfigurations.Add(objDealerConfiguration);
                await context.SaveChangesAsync();
                return objDealerConfiguration.DealerId;
            }
        }

        /// <summary>
        /// Delete dealer configuration by dealerid.
        /// </summary>
        /// <param name="dealerId"></param>
        public async Task DeleteDealerConfiguration(int dealerId)
        {
            using (ConfigurationManagerContext context = new ConfigurationManagerContext(_connectionString))
            {
                var objDealer = context.DealConfigurations.First(x => x.DealerId == dealerId);
                if (objDealer != null)
                {
                    context.DealConfigurations.Remove(objDealer);
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Get configuration by dealerid.
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public DealerConfiguration GetDealerConfigurationById(int dealerId)
        {
            using (ConfigurationManagerContext context = new ConfigurationManagerContext(_connectionString))
            {
                return context.DealConfigurations.FirstOrDefault(c => c.DealerId == dealerId);
            }
        }

        /// <summary>
        /// Get dealer configuration by rooftopid and community id.
        /// </summary>
        /// <param name="roofTopId"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public DealerConfiguration GetDealerConfigurationByRoofTopIdAndCommunityId(string roofTopId, string communityId)
        {
            using (ConfigurationManagerContext context = new ConfigurationManagerContext(_connectionString))
            {
                return context.DealConfigurations.FirstOrDefault(c => c.RooftopId == roofTopId && c.CommunityId == communityId);
            }
        }

        /// <summary>
        /// Get all dealers configuration details.
        /// </summary>
        /// <returns></returns>
        public List<int> GetDealersCsvSources()
        {
            using (ConfigurationManagerContext context = new ConfigurationManagerContext(_connectionString))
            {
                return context.DealConfigurations.Select(x=> x.CsvSource).Distinct().ToList();
            }
        }

        /// <summary>
        /// Edit dealer configuration by id.
        /// </summary>
        /// <param name="objDealerConfiguration"></param>
        /// <param name="dealerId"></param>
        public async Task EditDealerConfiguration(DealerConfiguration objDealerConfiguration, int dealerId)
        {
            using (ConfigurationManagerContext context = new ConfigurationManagerContext(_connectionString))
            {
                var list = context.Set<DealerConfiguration>().ToList();
                objDealerConfiguration.DealerId = dealerId;
                context.Entry(context.Set<DealerConfiguration>().Find(dealerId)).CurrentValues.SetValues(objDealerConfiguration);
                await context.SaveChangesAsync();
            }
        }
    }
}
