using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.DbModels;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Contract.Response;
using Service.Implementation;
using System;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;

namespace Service.Tests
{
    [TestFixture]
    public class CDKAppointmentSlotsServiceTest
    {
        private Mock<IRestApiClient> _restApiClientMock;
        private Mock<ICdkCustomerDAL> _cdkCustomerDALMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IValidateRequest> _validateRequestMock;
        private IMapper _mapper;
        private CDKAppointmentSlotsService _underTest;

        [SetUp]
        public void Setup()
        {
            _restApiClientMock = new Mock<IRestApiClient>();
            _cdkCustomerDALMock = new Mock<ICdkCustomerDAL>();
            _tokenServiceMock = new Mock<ITokenService>();
            _validateRequestMock = new Mock<IValidateRequest>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GetAppointmentSlotsRequest, CDKGetAppointmentSlotsRequest>();
            });
            _mapper = config.CreateMapper();

            _underTest = new CDKAppointmentSlotsService(_restApiClientMock.Object, _cdkCustomerDALMock.Object,
                _tokenServiceMock.Object, null, _validateRequestMock.Object, _mapper, TestResources.CdkAutolineUrl);

            _cdkCustomerDALMock.Setup(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()))
               .Returns(TestResources.CdkCustomer);
            _tokenServiceMock.Setup(mock => mock.GetCustomerToken(It.IsAny<CdkCustomer>(), It.IsAny<string>()))
                .Returns(Task.FromResult(TestResources.TokenResponse.Token));
            _restApiClientMock.Setup(mock => mock.Invoke<GetAppointmentSlotsResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true, Result = TestResources.GetAppointmentSlotsResponse }));
            _validateRequestMock.Setup(mock => mock.ValidateRequestData<GetAppointmentSlotsRequest>(It.IsAny<GetAppointmentSlotsRequest>()))
                .Returns((List<string>)null);
        }

        [Test]
        public void CDKAppointmentSlots_Constructor_Exception_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CDKAppointmentSlotsService(null,
                _cdkCustomerDALMock.Object, _tokenServiceMock.Object, null, _validateRequestMock.Object,
                _mapper, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKAppointmentSlotsService(_restApiClientMock.Object,
                null, _tokenServiceMock.Object, null, _validateRequestMock.Object,
                _mapper, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKAppointmentSlotsService(_restApiClientMock.Object,
                _cdkCustomerDALMock.Object, null, null, _validateRequestMock.Object,
                _mapper, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKAppointmentSlotsService(_restApiClientMock.Object,
                 _cdkCustomerDALMock.Object, _tokenServiceMock.Object, null,
                 null, _mapper, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKAppointmentSlotsService(_restApiClientMock.Object,
                _cdkCustomerDALMock.Object, _tokenServiceMock.Object, null,
                _validateRequestMock.Object, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKAppointmentSlotsService(_restApiClientMock.Object,
                _cdkCustomerDALMock.Object, _tokenServiceMock.Object, null, _validateRequestMock.Object,
                _mapper, null));
        }

        [Test]
        public void GetAppointmentSlots_InvalidCustomer_Exception_Test()
        {
            _cdkCustomerDALMock.Setup(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()))
                .Returns((CdkCustomer)null);
            Assert.ThrowsAsync<InvalidCustomerException>(() => _underTest.GetAppointmentSlots(TestResources.GetAppointmentSlotsRequest));
        }

        [Test]
        public void GetAppointmentSlots_NullRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                _underTest.GetAppointmentSlots(It.IsAny<GetAppointmentSlotsRequest>()));
        }

        [Test]
        public void GetAppointmentSlot_InvalidRequest()
        {
            _validateRequestMock.Setup(mock => mock.ValidateRequestData<GetAppointmentSlotsRequest>(It.IsAny<GetAppointmentSlotsRequest>()))
                .Returns(new List<string> { "CustomerNo need be bigger than 0" });
            Assert.ThrowsAsync<InvalidRequestException>(() => _underTest.GetAppointmentSlots(new GetAppointmentSlotsRequest()
            {
                CommunityId = "testCommunityId"
            }));
        }

        [Test]
        public void GetAppointSlot_NullResponse_Exception()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<GetAppointmentSlotsResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = false, Result = null }));
            Assert.ThrowsAsync<CDKAutolineException>(() =>
                _underTest.GetAppointmentSlots(TestResources.GetAppointmentSlotsRequest));
        }

        [Test]
        public async Task GetAppointmentSlots_Test()
        {
            var getServiceAdvisorsResponse = await _underTest.GetAppointmentSlots(TestResources.GetAppointmentSlotsRequest);

            for (int i = 0; i < getServiceAdvisorsResponse.NonWorkingDates.Count; i++)
            {
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.NonWorkingDates[i], getServiceAdvisorsResponse.NonWorkingDates[i]);
            }

            for (int i = 0; i < getServiceAdvisorsResponse.Option.Count; i++)
            {
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].AdvisorsPhotos,
                    getServiceAdvisorsResponse.Option[i].AdvisorsPhotos);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].FirstAddressLabel,
                    getServiceAdvisorsResponse.Option[i].FirstAddressLabel);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].InitialAppFieldName,
                    getServiceAdvisorsResponse.Option[i].InitialAppFieldName);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].InitialAppTimeRequired,
                    getServiceAdvisorsResponse.Option[i].InitialAppTimeRequired);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].OptionAdvisor,
                    getServiceAdvisorsResponse.Option[i].OptionAdvisor);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].OptionDescription,
                    getServiceAdvisorsResponse.Option[i].OptionDescription);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].OptionDisplayName,
                    getServiceAdvisorsResponse.Option[i].OptionDisplayName);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].OptionID,
                    getServiceAdvisorsResponse.Option[i].OptionID);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].OptionPrice,
                    getServiceAdvisorsResponse.Option[i].OptionPrice);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].RequestAddress,
                    getServiceAdvisorsResponse.Option[i].RequestAddress);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].RequestPostCode,
                    getServiceAdvisorsResponse.Option[i].RequestPostCode);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].RequestSecondAddress,
                    getServiceAdvisorsResponse.Option[i].RequestSecondAddress);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].RequestSecondPostCode,
                    getServiceAdvisorsResponse.Option[i].RequestSecondPostCode);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].Resource.Id,
                    getServiceAdvisorsResponse.Option[i].Resource.Id);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].Resource.Type,
                    getServiceAdvisorsResponse.Option[i].Resource.Type);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].SecondAddressLabel,
                    getServiceAdvisorsResponse.Option[i].SecondAddressLabel);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].SecondAppFieldName,
                    getServiceAdvisorsResponse.Option[i].SecondAppFieldName);
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].SecondAppTimeRequired,
                    getServiceAdvisorsResponse.Option[i].SecondAppTimeRequired);
                for (int j = 0; j < getServiceAdvisorsResponse.Option[i].Slots.Count; j++)
                {
                    Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].Slots[j].Date,
                        getServiceAdvisorsResponse.Option[i].Slots[j].Date);
                    for (int k = 0; k < getServiceAdvisorsResponse.Option[i].Slots[j].Slots.Count; k++)
                    {
                        Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.Option[i].Slots[j].Slots[k].ToString(),
                            getServiceAdvisorsResponse.Option[i].Slots[j].Slots[k].ToString());
                    }
                }
            }

            for (int i = 0; i < getServiceAdvisorsResponse.WorksDiary.Count; i++)
            {
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.WorksDiary[i],
                    getServiceAdvisorsResponse.WorksDiary[i]);
            }
            for (int i = 0; i < getServiceAdvisorsResponse.WorksDiaryDetails.Count; i++)
            {
                Assert.AreEqual(TestResources.GetAppointmentSlotsResponse.WorksDiaryDetails[i].ToString(),
                    getServiceAdvisorsResponse.WorksDiaryDetails[i].ToString());
            }

            _cdkCustomerDALMock.Verify(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()));
            _restApiClientMock.Verify(mock => mock.Invoke<GetAppointmentSlotsResponse>(It.IsAny<ApiRequest>()));
        }
    }
}
