using AutoMapper;
using Service.Contract;
using Service.Contract.Request;
using System;
using System.Threading.Tasks;
using Service.Contract.Exceptions;
using Service.Contract.Response;
using System.Collections.Generic;

namespace Service.Implementation
{
    public class DealerConfigurationService : IDealerConfigurationService
    {
        private readonly IDealerConfigurationDAL _dealerConfigurationDal;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor, it is responsible for resolving the dealconfigurationdal object by unity.
        /// </summary>
        /// <param name="dealerConfigurationDal"></param>
        /// <param name="mapper"></param>
        public DealerConfigurationService(IDealerConfigurationDAL dealerConfigurationDal, IMapper mapper)
        {
            _dealerConfigurationDal =
                dealerConfigurationDal ?? throw new ArgumentNullException(nameof(dealerConfigurationDal));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Call Add dealer configuration  
        /// </summary>
        /// <param name="objAddDealerConfiguration"></param>
        /// <returns></returns>
        public async Task<int> AddDealerConfiguration(DealerConfigurationCreateRequest objAddDealerConfiguration)
        {
            if (objAddDealerConfiguration == null) throw new ArgumentNullException(nameof(objAddDealerConfiguration));
            if (!CheckCommunicationMethod(objAddDealerConfiguration.CommunicationMethodName)) throw new CommunicationMethodException();
            if (!CheckCsvSource(objAddDealerConfiguration.CsvSourceName)) throw new CsvSourceException();
            var objDealerConfiguration = MapAddDealerConfiguration(objAddDealerConfiguration);
            return await _dealerConfigurationDal.AddDealerConfiguration(objDealerConfiguration);
        }

        /// <summary>
        /// Check dealer exists either by dealer id or rooftopidandcommunityid.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="id">DealerId</param>
        /// <param name="rooftopId">Rooftop id</param>
        /// <param name="communityId">Community id</param>
        /// <returns></returns>
        public bool CheckDealerExist(DealerSearchCriteria searchCriteria, int id, string rooftopId, string communityId)
        {
            //Sub-function -> checking dealerid.
            bool CheckByDealerId()
            {
                if (id != default(int))
                {
                    return GetDealerConfigurationById(id) != null;
                }

                return false;
            }

            // Sub-function -> checking rooftopid and community id
            bool CheckByRoofTopIdAndCommunityId()
            {
                if (!string.IsNullOrEmpty(rooftopId) && !string.IsNullOrEmpty(communityId))
                {
                    var dealerResponse = GetDealerConfigurationByRoofTopIdAndCommunityId(rooftopId, communityId);

                    // checking two scenario(s) if dealer response is not null.
                    // 1. if id is not equals to default value of integer and response dealerid is equalto requestedid
                    // 2. if id is equalto default value of integer.

                    return dealerResponse != null &&
                           (id != default(int) && id == dealerResponse.DealerId || id == default(int));
                }

                return false;
            }

            switch (searchCriteria)
            {
                case DealerSearchCriteria.SearchByDealerId: return CheckByDealerId();
                case DealerSearchCriteria.SearchByRooftopAndCommunityId: return CheckByRoofTopIdAndCommunityId();
                case DealerSearchCriteria.SearchByAll: return CheckByDealerId() && CheckByRoofTopIdAndCommunityId();
            }

            return false;
        }

        /// <summary>
        /// Delete dealer configuration by dealer id
        /// </summary>
        /// <param name="dealerId">dealer id</param>
        public async Task DeleteDealerConfiguration(int dealerId)
        {
            await _dealerConfigurationDal.DeleteDealerConfiguration(dealerId);
        }

        /// <summary>
        /// Edit dealer configurations
        /// </summary>
        /// <param name="objEditDealerConfiguration"></param>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public async Task EditDealerConfiguration(DealerConfigurationUpdateRequest objEditDealerConfiguration,
            int dealerId)
        {
            if (objEditDealerConfiguration == null) throw new ArgumentNullException(nameof(objEditDealerConfiguration));
            if (!CheckCommunicationMethod(objEditDealerConfiguration.CommunicationMethodName)) throw new CommunicationMethodException();
            if (!CheckCsvSource(objEditDealerConfiguration.CsvSourceName)) throw new CsvSourceException();

            var existingDealer = GetDealerConfigurationById(dealerId);

            if (existingDealer == null) throw new DealerNotExistException();

            existingDealer = GetDealerConfigurationByRoofTopIdAndCommunityId(objEditDealerConfiguration.RooftopId,
                objEditDealerConfiguration.CommunityId);

            if (existingDealer != null && existingDealer.DealerId != dealerId) throw new DealerAlreadyExistException();

            var objDealerConfiguration = MapUpdateDealerConfiguration(objEditDealerConfiguration);

            await _dealerConfigurationDal.EditDealerConfiguration(objDealerConfiguration, dealerId);
        }

        /// <summary>
        /// Get dealer configuration by id.
        /// </summary>
        /// <param name="dealerId"></param>
        /// <returns></returns>
        public DealerConfigurationResponse GetDealerConfigurationById(int dealerId)
        {
            var dealerResponse = _dealerConfigurationDal.GetDealerConfigurationById(dealerId);
            return MapDealerConfigurationResponse(dealerResponse);
        }

        /// <summary>
        /// Get dealer configuration by rooftopid and community id.
        /// </summary>
        /// <param name="roofTopId"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public DealerConfigurationResponse GetDealerConfigurationByRoofTopIdAndCommunityId(string roofTopId, string communityId)
        {
            var dealerResponse = _dealerConfigurationDal.GetDealerConfigurationByRoofTopIdAndCommunityId(roofTopId, communityId);
            return MapDealerConfigurationResponse(dealerResponse);
        }

        /// <summary>
        /// Get all dealers configuration details
        /// </summary>
        /// <returns></returns>
        public List<string> GetDealersCsvSources()
        {
            var csvSources = _dealerConfigurationDal.GetDealersCsvSources();
            List<string> lstCsvSources = new List<string>();
            if (csvSources == null) return lstCsvSources;
            csvSources.ForEach(x =>
            {
                lstCsvSources.Add(Enum.GetName(typeof(CsvSource), x));
            });
            return lstCsvSources;
        }

        /// <summary>
        /// Get Invitation content either Sms or Email.
        /// </summary>
        /// <param name="roofTopId"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        public DealerInvitationContentResponse GetInvitationContent(string roofTopId, string communityId)
        {
            var dealerResponse = _dealerConfigurationDal.GetDealerConfigurationByRoofTopIdAndCommunityId(roofTopId, communityId);
            return MapDealerInvitationResponse(dealerResponse);
        }

        /// <summary>
        /// Map add dealer configuration with dealer configuraion create request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal DealerConfiguration MapAddDealerConfiguration(DealerConfigurationCreateRequest request)
        {
            var objDealerConfiguration =
               _mapper.Map<DealerConfigurationCreateRequest, DealerConfiguration>(request);
            objDealerConfiguration.CommunicationMethod = GetCommunicationMethod(request.CommunicationMethodName);
            objDealerConfiguration.CsvSource = GetCsvSource(request.CsvSourceName);
            return objDealerConfiguration;
        }

        internal DealerConfiguration MapUpdateDealerConfiguration(DealerConfigurationUpdateRequest request)
        {
            var objDealerConfiguration =
               _mapper.Map<DealerConfigurationUpdateRequest, DealerConfiguration>(request);
            objDealerConfiguration.CommunicationMethod = GetCommunicationMethod(request.CommunicationMethodName);
            objDealerConfiguration.CsvSource = GetCsvSource(request.CsvSourceName);

            return objDealerConfiguration;
        }

        internal DealerConfigurationResponse MapDealerConfigurationResponse(DealerConfiguration dealerConfig)
        {
            return _mapper.Map<DealerConfiguration, DealerConfigurationResponse>(dealerConfig);
        }

        /// <summary>
        /// Map dealer invitation response with dealer configuration.
        /// </summary>
        /// <param name="dealerConfig"></param>
        /// <returns></returns>
        internal DealerInvitationContentResponse MapDealerInvitationResponse(DealerConfiguration dealerConfig)
        {
            return _mapper.Map<DealerConfiguration, DealerInvitationContentResponse>(dealerConfig);
        }

        /// <summary>
        /// Get communication method value.
        /// </summary>
        /// <param name="communicationMethodName"></param>
        /// <returns></returns>
        internal int GetCommunicationMethod(string communicationMethodName)
        {
            return (int)((CommunicationMethod)Enum.Parse(typeof(CommunicationMethod), communicationMethodName, true));
        }

        internal int GetCsvSource(string csvSource)
        {
            return (int)((CsvSource)Enum.Parse(typeof(CsvSource), csvSource, true));
        }

        public bool CheckCsvSource(string csvSource)
        {
            return Enum.TryParse(csvSource, true, out CsvSource method);
        }

        /// <summary>
        /// Check communication Method
        /// </summary>
        /// <param name="communicationMethod"></param>
        /// <returns></returns>
        public bool CheckCommunicationMethod(string communicationMethod)
        {
            return Enum.TryParse(communicationMethod, true, out CommunicationMethod method);
        }
    }
}