using CsvHelper;
using Processor.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Processor.Implementation
{
    public class CsvProcess : ICsvProcessor
    {
        private readonly CsvColumnNames _csvColumnNames;
        private const string IntTypeName = "Int32";

        public CsvProcess(CsvColumnNames csvColumnNames)
        {
            _csvColumnNames = csvColumnNames ?? throw new ArgumentNullException(nameof(csvColumnNames));
        }

        /// <summary>
        /// Extract customer vehicle details from bytes content
        /// </summary>
        /// <param name="content">csv file in byte format</param>
        /// <returns></returns>
        public async Task<List<CustomerVehicle>> ProcessCSVFile(byte[] content)
        {
            if (content == null)
            {
                return null;
            }

            var customers = new List<CustomerVehicle>();

            using (var mStream = new MemoryStream(content))
            using (var textReader = new StreamReader(mStream))
            {
                var csv = new CsvReader(textReader);
                csv.Configuration.BadDataFound = null;
                while (await csv.ReadAsync())
                {
                    csv.GetRecord<dynamic>();

                    var customer = new CustomerVehicle
                    {
                        CustomerNo = GetValue<int>(csv, _csvColumnNames.CustomerNo),
                        CustomerEmail = GetValue<string>(csv, _csvColumnNames.CustomerEmail),
                        FirstName = GetValue<string>(csv, _csvColumnNames.FirstName),
                        Surname = GetValue<string>(csv, _csvColumnNames.Surname),
                        PhoneNumber = GetValue<string>(csv, _csvColumnNames.PhoneNumber),
                        RooftopId = GetValue<string>(csv, _csvColumnNames.RooftopId),
                        CommunityId = GetValue<string>(csv, _csvColumnNames.CommunityId),
                        VehicleNo = GetValue<int>(csv, _csvColumnNames.VehicleNo),
                        RegistrationNo = GetValue<string>(csv, _csvColumnNames.RegistrationNo),
                        VinNumber = GetValue<string>(csv, _csvColumnNames.VinNumber),
                        MakeCode = GetValue<string>(csv, _csvColumnNames.MakeCode),
                        ModelCode = GetValue<string>(csv, _csvColumnNames.ModelCode),
                        ModelYear = GetValue<string>(csv, _csvColumnNames.ModelYear),
                        ModelDescription = GetValue<string>(csv, _csvColumnNames.ModelDescription),
                        VariantCode = GetValue<string>(csv, _csvColumnNames.VariantCode),
                        NextServiceMileage = GetValue<int>(csv, _csvColumnNames.NextServiceMileage),
                    };

                    if (customer.CustomerNo == -1 || customer.VehicleNo == -1) continue;

                    customers.Add(customer);
                }
            }

            return customers;
        }


        internal T GetValue<T>(CsvReader csvReader, string headerName)
        {
            switch (typeof(T).Name)
            {
                case IntTypeName:
                {
                    if (csvReader.TryGetField(headerName, out string intVal))
                    {
                        if (string.IsNullOrWhiteSpace(intVal)) return UtilityHelper.Cast(-1, typeof(int));
                        return UtilityHelper.Cast(intVal, typeof(int));
                    }

                    throw new CsvInvalidHeaderException($"Could not find header: {headerName} in the file");
                }

                default:
                {
                    if (csvReader.TryGetField(headerName, out string value))
                    {
                        return UtilityHelper.Cast(value, typeof(string));
                    }

                    throw new CsvInvalidHeaderException($"Could not find header: {headerName} in the file");
                }
            }
        }
    }
}