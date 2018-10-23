using System.ComponentModel;

namespace Service.Contract
{
    public enum ErrorResponseCode
    {
        [Description("Success")]
        Success = 0,

        [Description("Failure")]
        Failure = 1,

        [Description("Deferred")]
        Deferred = 2,

        [Description("Request cannot be null, any value must be filled")]
        RequestCannotBeNull = 3,

        [Description("Setup or configuration error in the data hub")]
        SetupError = 8,

        [Description("InternalCoding error, e.g. if an attribute is missing on a method")]
        InternalCodingError = 9,

        [Description("Insufficient credentials")]
        InsufficientCredentials = 10,

        [Description("Customer is already registered")]
        CustomerAlreadyRegistered = 11,

        [Description("Customer is unknown")]
        CustomerUnknown = 12,

        [Description("Error by registration customer")]
        CustomerRegistrationError = 13,

        [Description("Rooftop not known")]
        RooftopUnknown = 14,

        [Description("Token is unknown")]
        TokenUnknow = 15,

        [Description("Illegal token received")]
        TokenIllegal = 16,

        [Description("Password must be changed")]
        MustChangePassword = 17,

        [Description("Username must be changed")]
        MustChangeUsername = 18,

        [Description("May not change the username")]
        MayNotChangeUsername = 19,

        [Description("Vehicle is unknown")]
        VehicleUnknow = 20,

        [Description("Vehicle is already registered")]
        VehicleAlreadyRegistered = 21,

        [Description("User is same then the old username")]
        NewUsernameSameAsOld = 22,

        [Description("The chosen username is no valid email address.")]
        UsernameNoEmailAddress = 23,

        [Description("Customer not activated!")]
        CustomerNotActivated = 24,

        [Description("Customer account is locked")]
        CustomerAccountLocked = 25,

        [Description("Customer account already unlocked")]
        CustomerAccountAlreadyUnlocked = 26,

        [Description("Customer is not an administrator")]
        CustomerNoAdministrator = 27,

        [Description("The token to (re-) set the password is invalid.")]
        PasswordTokenIllega = 28,

        [Description("Password and password confirmation do not match.")]
        PasswordMismatch = 29,

        [Description("Appointment is unknown")]
        AppointmentUnknown = 30,

        [Description("Schedule of the appointment has failed")]
        ScheduleAppointmentFailed = 31,

        [Description("Unfortunately your vehicle is located outside of our collection and delivery zone.Please select a different appointment option.")]
        PostCodeOptionError = 32,

        [Description("The parent appointment is unknown")]
        ParentAppointmentUnknown = 33,

        [Description("When trying to ammend an appointment, LastStep has to be 4, CustomerLoginId, RooftopId and CommunityId must be the same as in the ParentAppointment")]
        ParentAppointmentMismatch = 34,


        [Description("The parent appointment is not amendable")]
        ParentAppointmentNotAmendable = 35,


        [Description("Feature not active.")]
        FeatureNotActive = 36,


        [Description("Parts order is unknown")]
        PartsOrderUnknown = 37,

        [Description("Mandatory fields are missing")]
        MandatoryFieldsMissing = 38,

        [Description("The customer is registered in SOL but not yet in the DMS")]
        CustomerUnknownInDms = 39,

        [Description("Parts Online not enabled.")]
        POLNotEnabled = 40,

        [Description("Service Online not enabled.")]
        SOLNotEnabled = 41,

        [Description("Rooftop not enabled.")]
        RooftopNotEnabled = 42,

        [Description("Activation key not found")]
        ActivationKeyNotFound = 43,

        [Description("The password does not match the password complexity requirements.")]
        PasswordComplexityMismatch = 44,

        [Description("Wrong answer to the security question")]
        WrongSecurityAnswer = 45,

        [Description("Community and/or rooftop do not match")]
        CommunityRooftopMismatch = 46,

        [Description("The difference between initial date and end date is out of range (&gt; 1 month + 6 days).")]
        DatesOutOfRange = 47,

        [Description("The SOAP header is missing!")]
        MissingSoapHeader = 90,

        [Description("DMS parameters are missing!")]
        MissingDmsParameters = 91,

        [Description("System is not available at the moment.")]
        SystemLocked = 92,

        [Description("Error in a task started by the scheduler.")]
        SchedulerError = 95,

        [Description("Error in the reply from the DMS.")]
        DmsReplyError = 96,

        [Description("Database error")]
        DatabaseError = 97,

        [Description("DMS not answering")]
        DmsCommunicationError = 98,

        [Description("Undefined exception")]
        UnknownException = 99,

        [Description("DMS Error")]
        DmsErrorCode = 100,

        [Description("Wrong rooftop id used")]
        WrongRooftopID = 101,

        [Description("Unable to get recommended services")]
        UnableToGetRecommendedServices = 102,

        [Description("No CRM company found")]
        NoCRMCompanyFound = 103,

        [Description("No calendar setup in DMS")]
        NoCalendarSetup = 104,

        [Description("No options available")]
        NoOptionsAvailable = 105,

        [Description("No technicians available for chosen date")]
        NoTechniciansAvailableForChosenDate = 106,

        [Description("No second appointment time required")]
        NoSecondAppointmentTimeRequired = 107,

        [Description("No time available for chosen booking option")]
        NoTimeAvailableForBookingOptionChosen = 108,

        [Description("Incorrect selected option for apointment")]
        IncorrectAppointmentOptionSelected = 109,

        [Description("No skills defined in job request")]
        NoSkillsDefinedInJobsRequested = 110,

        [Description("Advisor time not setup")]
        AdvisorTimesNotSetup = 111,

        [Description("Unable to get service advisors for this rooftop")]
        UnableToGetServiceAdvisorsForThisRooftop = 112,

        [Description("DMS not available at the moment! Please try again later")]
        SystemIsNotAvailableAtTheMoment_PleaseTryLater = 113,

        [Description("Vehicle Registration not found")]
        NoVehicleRegistrationFound = 114,

        [Description("Unable to access workshop parameters")]
        UnableToAccessWorkshopParameters = 115,

        [Description("Error by saving WIP")]
        ErrorSavingWIP = 116,

        [Description("Error by creating customer")]
        ErrorCreatingCustomer = 117,

        [Description("Post code weigthing not available")]
        PostCodeWeigthingNotAvailable = 118,

        [Description("Incorrect job request")]
        IncorrectJobRequested = 119,

        [Description("Wrong appointment option (wrong optioncode)")]
        WrongAppointmentOption_OptionCode = 120,

        [Description("No appointment options defined")]
        NoAppointmentOptionsDefined_NoAppointmentOptionsFile = 121,

        [Description("Advisor booked for the same time slot")]
        AdvisorBookedForSameTimeSlot = 122,

        [Description("Post code option error (32)")]
        PostCodeOptionError_32 = 123,

        [Description("Invalid WIPNo for an appointment")]
        InvalidWIPNo = 124,

        [Description("No services provided")]
        NoServicesProvided = 125,

        [Description("Wrong services provided")]
        WrongServicesProvided = 126,

        [Description("Advisor timeslot no longer available")]
        TimeslotNoLongerAvailable = 127,

        [Description("Unable to create vehicle")]
        UnableToCreateVehicle = 201,

        [Description("Vehicle already exists in the system")]
        VehicleAlreadyCreatedInSystem = 202,

        [Description("Vehicle already allocated to a customer")]
        VehicleAlreadyAllocatedToCustomer = 203,

        [Description("Customer unknown")]
        CustomerUnkown = 204,

        [Description("Error by deleting vehicle")]
        ErrorDeletingVehicle = 205,

        [Description("Unable to read vehicle details")]
        UnableToGetVehicleDetails = 206,

        [Description("Unable to update vehicle details")]
        UnableToUpdateVehicleDetails = 207,

        [Description("Invalid chassis number")]
        InvalidChassisNumber = 208,

        [Description("Chassis number not found")]
        ChassisNotFound = 209,

        [Description("Registration number not found")]
        RegistrationNumberNotFound = 210,

        [Description("Provide registration number or cahssis number for lookup")]
        ProvideRegistrationNumberOrChassiNumberForLookup = 211,

        [Description("No data found for selected ranges")]
        NoDataFoundForSelectedRanges = 212,

        [Description("Unable to retrieve customer data")]
        UnableToRetrieveCustomerData = 301,

        [Description("Unable to create customer")]
        UnableToCreateCustomer = 302,

        [Description("Customer with that email address already exists")]
        AlreadyExistsCustomerWithThatEmail = 303,

        [Description("Error by saving customer details")]
        ErrorSavingCustomerDetails = 304,

        [Description("Temporary company not found")]
        TemporaryCompanyNotFound = 305,

        [Description("Unable to retrieve company details")]
        UnableToRetrieveCompanyDetails = 306,

        [Description("No company details for given customer found")]
        NoCompanyDetailsForGivenCustomer = 307,

        [Description("No addresses found for the entered post code")]
        NoAddressesFoundForEnteredPostCode = 308,

        [Description("Temporary company couldn't updated")]
        TemporaryCompanyNotUpdated = 309,

        [Description("Company couldn't updated")]
        CompanyNotUpdated = 310,

        [Description("Unable to delete company record")]
        UnableToDeleteCompanyRecord = 311,

        [Description("Company record not found")]
        CompanyRecordNotFound = 312,

        [Description("Temporary customer not found")]
        TemporaryCustomerNotFound = 313,

        [Description("Company details not stored")]
        CouldNotStoreCompanyDetails = 314,

        [Description("Delivery address exist")]
        DeliveryAddressExists = 315,

        [Description("Delivery address NOT exist")]
        DeliveryAddressNotExists = 316,

        [Description("No data returned for the search criteria")]
        NoDataFoundForTheSearchCriteria = 400,

        [Description("Appointment could not be deleted (checks NumberOfDaysAptDeletablein in the Community_Rooftop_Settings table DB)")]
        DeleteAppointmentImpossible = 401,

        [Description("Community does not exist")]
        WrongCommunity = 501,

        [Description("Rooftop does not exist")]
        WrongRoofTop = 502,

        [Description("No communication data available (DMS entry missing)")]
        DMSEntryMissing = 503,

        [Description("Rooftop in Error Status (LastCallFailure &gt; LastCallSuccess)")]
        RooftopInErrorStatus = 504,

        [Description("DMSId unknown in DMS table")]
        DMSIdUnknown = 505,

        [Description("Rooftop is unknown in community")]
        RooftopUnknownInCommunity = 506,

        [Description("Temporary and Permanent Company numbers (both) are filled in")]
        TemporaryANDPermanentCompanyNumbers_Filled = 507,

        [Description("Temporary and Permanent Company numbers (both) are empty")]
        TemporaryANDPermanentCompanyNumbers_Empty = 508,

        [Description("failed to create an order line")]
        FailedToCreateOrderLine = 600,

        [Description("Failed to create WIP in company")]
        FailedToCreateWIP = 601,

        [Description("No lines in WIP")]
        NoLinesInWIP = 602,

        [Description("No data found for the selected ranges")]
        NoDataFoundForPOLSearchCriteria = 604,

        [Description("Dealer code blank for customer")]
        DealerCodeForCustomerBlank = 605,

        [Description("Invalid Customer Id")]
        InvalidCustomerId = 608,

        [Description("Invalid SL account")]
        InvalidSLAccount = 609,

        [Description("Invalid dealer code")]
        InvalidDearlerCode = 610,

        [Description("Failed to read POS operator record for operator number %d userid %s opers handle")]
        FailedToReadPOSOperator = 611,

        [Description("Change of Community is not possible")]
        CommunityChangeNotPossible = 999
    }
}
