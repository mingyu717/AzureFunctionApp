using NUnit.Framework;
using Service.Implementation;
using System;
using System.Collections.Generic;

namespace Service.Test
{
    [TestFixture]
    public class UtilityHelperTest
    {
        private List<KeyValuePair<string, string>> _queryStrings;
        [SetUp]
        public void Setup()
        {
            _queryStrings = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("roofTopId", "sampleRoofTopId"),
            };
        }

        [Test]
        public void GetQueryStringValue_Null_Test()
        {
            Assert.Throws<ArgumentNullException>(() => UtilityHelper.GetQueryStringValue(null, "roofTopId"));
        }

        /// <summary>
        /// 1. Test scenario, when passed query name key as parameter which is present in key value pair list
        ///    then expected result key(s) value.
        /// 2. Test scenario, when psseed query name key as paramerter which is not present in key value pair list
        ///    then expected result is null.
        /// </summary>
        /// <param name="queryName"></param>
        /// <returns></returns>
        [TestCase("roofTopId", ExpectedResult = "sampleRoofTopId")]
        [TestCase("communityId", ExpectedResult = null)]
        public string GetQueryStringValue_Test(string queryName)
        {
            return UtilityHelper.GetQueryStringValue(_queryStrings, queryName);
        }

        /// <summary>
        /// Test scenario when passed dynamic object the expected result should be expected retutn type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [TestCase("1", typeof(int), ExpectedResult = 1)]
        [TestCase("mockstring", typeof(string), ExpectedResult = "mockstring")]
        public dynamic Cast_Test(dynamic obj, Type type)
        {
            return UtilityHelper.Cast(obj, type);
        }

        /// <summary>
        /// Test scenario when passed null object the expected result will be invalid cast exception.
        /// </summary>
        [Test]
        public void Cast_InvalidCastException_Test()
        {
            Assert.Throws<InvalidCastException>(() => UtilityHelper.Cast(null, typeof(int)));
        }
    }
}
