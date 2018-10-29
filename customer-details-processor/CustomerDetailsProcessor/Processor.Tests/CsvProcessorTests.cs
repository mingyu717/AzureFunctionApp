using NUnit.Framework;
using Processor.Contract;
using Processor.Implementation;
using System.Threading.Tasks;

namespace Processor.Tests
{
    [TestFixture]
    public class CsvProcessorTests : TestBase
    {
        private ICsvProcessor _csvProcess;

        [SetUp]
        public void Setup()
        {
            var csvColumnNames = new CsvColumnNames
            {
                CustomerNo = "Customer.number",
                CustomerEmail = "E-mail.address",
                FirstName = "First.name",
                Surname = "Surname",
                PhoneNumber = "Telephone.numbers(4)",
                RooftopId = "Branch(buy,stk,sel(1)",
                CommunityId = "Branch(buy,stk,sel(1)",
                VehicleNo = "Vehicle.number",
                RegistrationNo = "Registration.number.",
                VinNumber = "Chassis.number",
                MakeCode = "Franchise",
                ModelCode = "Model.code",
                ModelYear = "Model.year",
                ModelDescription = "Description",
                VariantCode = "Variant.Code",
                NextServiceMileage = "Next.service.mileage"
            };
            _csvProcess = new CsvProcess(csvColumnNames);
        }

        /// <summary>
        /// Test scenario when the csv has no content,the expected result should also be null.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ProcessCSVFile_IsNull()
        {
            var expectedResult = await _csvProcess.ProcessCSVFile(null);
            Assert.IsNull(expectedResult);
        }

        /// <summary>
        /// Test scenario when CSV data is not null and that the expected number of rows are 6 as in our test data. And test the first row data.
        /// </summary>
        [Test]
        public async Task ProcessCSVFile_Test()
        {
            byte[] byteContent = GetEmbeddedFile("Processor.Tests.MockCustomerDetails.CustomerMockFile.csv");
            var lstCustomerVehicles = await _csvProcess.ProcessCSVFile(byteContent);
            Assert.IsNotNull(lstCustomerVehicles);
            Assert.AreEqual(lstCustomerVehicles.Count, 6);
            Assert.AreEqual(lstCustomerVehicles[0].CustomerNo, 132525);
            Assert.AreEqual(lstCustomerVehicles[0].CustomerEmail, "Romualdo.Valer@mail.com");
            Assert.AreEqual(lstCustomerVehicles[0].FirstName, "Romualdo");
            Assert.AreEqual(lstCustomerVehicles[0].Surname, "Valer");
            Assert.AreEqual(lstCustomerVehicles[0].PhoneNumber, "21111111");
            Assert.AreEqual(lstCustomerVehicles[0].RooftopId, "EUR");
            Assert.AreEqual(lstCustomerVehicles[0].CommunityId, "EUR");
            Assert.AreEqual(lstCustomerVehicles[0].VehicleNo, 188562);
            Assert.AreEqual(lstCustomerVehicles[0].RegistrationNo, "JYF113");
            Assert.AreEqual(lstCustomerVehicles[0].VinNumber, "WVGZZZ5NZDW025176");
            Assert.AreEqual(lstCustomerVehicles[0].MakeCode, "W");
            Assert.AreEqual(lstCustomerVehicles[0].ModelCode, "TIGUAN");
            Assert.AreEqual(lstCustomerVehicles[0].ModelYear, "2013");
            Assert.AreEqual(lstCustomerVehicles[0].ModelDescription, "VW Tiguan 103KW TDI 7DSG");
            Assert.AreEqual(lstCustomerVehicles[0].VariantCode, "VariantCode1");
            Assert.AreEqual(lstCustomerVehicles[0].NextServiceMileage, 89700);
            Assert.AreEqual(lstCustomerVehicles[1].PhoneNumber, "");
            Assert.AreEqual(lstCustomerVehicles[1].NextServiceMileage, 0);
            Assert.AreEqual(lstCustomerVehicles[3].CustomerEmail, "");
        }

        /// <summary>
        /// Test scenario when the CSV has missing columns which were expected by us. In that case ProcessCSVFile method
        /// should throw the custom exception of type ExperiecoException and over here we are verifying if that exception type 
        /// is being thrown or not.
        /// </summary>
        /// <returns></returns>
        [Test]
        public void ProcessCSVFile_CustomerNoSurnameColumn()
        {
            byte[] byteContent =
                GetEmbeddedFile("Processor.Tests.MockCustomerDetails.CustomerNoSurnameMockFile.csv");
            Assert.ThrowsAsync<CsvInvalidHeaderException>(() => _csvProcess.ProcessCSVFile(byteContent));
        }

        [Test]
        public void ProcessCSVFile_CustomerInvalidVehicleNoColumn()
        {
            byte[] byteContent =
                GetEmbeddedFile("Processor.Tests.MockCustomerDetails.CustomerInvalidVehicleNoHeaderMockFile.csv");
            Assert.ThrowsAsync<CsvInvalidHeaderException>(() => _csvProcess.ProcessCSVFile(byteContent));
        }

        [TearDown]
        public void TearDown()
        {
            _csvProcess = null;
        }
    }
}