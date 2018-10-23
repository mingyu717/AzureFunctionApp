using NUnit.Framework;
using Service.Contract;
using Service.Implementation;
using System;
using System.Text;

namespace Service.Tests
{
    [TestFixture]
    public class UtilityHelperTest
    {
        private string _serializeString =
            "{\"CustomerNo\":10,\"CommunityId\":\"EBBETDEV\",\"RoofTopId\":\"EBBVW11DEV\"}";


        [Test]
        public void ByteToString_Test()
        {
            byte[] byteContent = Encoding.UTF8.GetBytes(TestResources.CdkCustomer.Token.ToString());
            Assert.IsInstanceOf(typeof(string), UtilityHelper.ByteToString(byteContent));
        }

        [Test]
        public void ByteToString_Null_Exception()
        {
            Assert.Throws<NullReferenceException>(() => UtilityHelper.ByteToString(null));
        }

        [Test]
        public void SerializeObject_Test()
        {
            string actualString = UtilityHelper.SerializeObject(TestResources.CustomerVerifyRequest);
            Assert.AreEqual(_serializeString, actualString);
        }

        [Test]
        public void DeserializeObject_Test()
        {
            var passwordModel = UtilityHelper.DeserializeObject(_serializeString);
            Assert.IsNotNull(passwordModel);
        }

        [TestCase("0", ExpectedResult = "Success")]
        [TestCase("1", ExpectedResult = "Failure")]
        public string GetEnumDescription_Test(string enumCode)
        {
            ErrorResponseCode enumValue = (ErrorResponseCode) int.Parse(enumCode);
            return UtilityHelper.GetEnumDescription(enumValue);
        }

        [Test]
        public void GetEnumDescription_NoDescription_Test()
        {
            const string expected = "GET";

            const HttpVerbs enumValue = 0;
            var result = UtilityHelper.GetEnumDescription(enumValue);
            Assert.AreEqual(expected, result);
        }


        [Test]
        public void GetEnumDescription_Invalid_Code_Test()
        {
            const ErrorResponseCode enumValue = (ErrorResponseCode) 4;
            Assert.Throws<NullReferenceException>(() => UtilityHelper.GetEnumDescription(enumValue));
        }

        [TestCase("1", typeof(int), ExpectedResult = 1)]
        [TestCase("1.2", typeof(double), ExpectedResult = 1.2)]
        public dynamic Cast_Test(dynamic obj, Type type)
        {
            return UtilityHelper.Cast(obj, type);
        }
    }
}