using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Processor.Contract;
using Processor.Contract.Exceptions;
using Processor.Implementation;
using Unity;

namespace CustomerDetailsProcessor
{
    public static class Processor
    {
        /// <summary>
        /// Azure Function to process email data and save in Customer Vehicle Database
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CustomerDetailsProcessor")]
        public static async Task Run([TimerTrigger("%schedule%")] TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"Customer Vehicle Timer trigger function executed at: {DateTime.Now}");
            try
            {
                var container = UnityContainer.Instance;

                var dealerConfigurationClient = container.Resolve<IDealerConfigurationClient>();
                var fileContentService = container.Resolve<IFileContentService>();
                var dealersCsvSource = await dealerConfigurationClient.GetDealerCsvSources();

                var fileContents = await fileContentService.GetFileContent(dealersCsvSource);
                if (fileContents != null && fileContents.Any())
                {
                    var customerVehicleClient = container.Resolve<ICustomerVehicleClient>();
                    var csvProcess = container.Resolve<ICsvProcessor>();
                    foreach (var fileContent in fileContents)
                    {
                        try
                        {
                            var lstCustomerVehicles = await csvProcess.ProcessCSVFile(fileContent.Content);

                            foreach (var customerVehicle in lstCustomerVehicles)
                            {
                                try
                                {
                                    var dealerConfigurationResponse =
                                        await dealerConfigurationClient.GetDealerConfiguration(customerVehicle.RooftopId,
                                            customerVehicle.CommunityId);
                                    if (dealerConfigurationResponse == null) throw new DealerNotFoundException();

                                    if (dealerConfigurationClient.ValidateEmailOrSmsByCommunicationMethod(
                                        dealerConfigurationResponse,
                                        customerVehicle.PhoneNumber,
                                        customerVehicle.CustomerEmail))
                                    {
                                        await customerVehicleClient.SaveCustomerVehicle(customerVehicle);
                                    }
                                }
                                catch (DealerNotFoundException ex)
                                {
                                    log.Error($"Could not find dealer with rooftopId: {customerVehicle.RooftopId} and communityId: {customerVehicle.CommunityId} for this customer: customerNo: {customerVehicle.CustomerNo}", ex);
                                }
                                catch (PhoneOrEmailNullException ex)
                                {
                                    log.Error(
                                        $"An error happen while processing this customer vehicle, customerNo: {customerVehicle.CustomerNo}. vehicleNo: {customerVehicle.VehicleNo}", ex);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(
                                        $"An error happens while processing this customer vehicle, customerNo: {customerVehicle.CustomerNo}. vehicleNo: {customerVehicle.VehicleNo}",
                                        ex);
                                }
                            }
                        }
                        catch (CsvInvalidHeaderException ex)
                        {
                            log.Error($"File, {fileContent.FileName}, has invalid headers", ex);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"An error happens while processing file {fileContent.FileName}", ex);
                        }
                    }
                }
            }
            catch (GetEmailContentException ex)
            {
                log.Error("An error happens while trying to get email content", ex);
            }
            catch (Exception ex)
            {
                log.Error("An error happens while processing customer vehicle details", ex);
            }
        }
    }
}