using System;
using dtos_service_insights_tests.TestServices;
using TechTalk.SpecFlow;
//using Reqnroll;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using dtos_service_insights_tests.Config;
using dtos_service_insights_tests.Contexts;
using dtos_service_insights_tests.Models;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace dtos_service_insights_tests.StepDefinitions;

[Binding]
public sealed class FileUploadSteps
{
        private readonly EndToEndFileUploadService _fileUploadService;
    private readonly AppSettings _appSettings;
    private SmokeTestsContexts _smokeTestsContext;

    public FileUploadSteps(IServiceProvider services, AppSettings appSettings, SmokeTestsContexts smokeTestsContext)
    {
        _appSettings = appSettings;
        _smokeTestsContext = smokeTestsContext;
        _fileUploadService = services.GetRequiredService<EndToEndFileUploadService>();
    }

    [Given(@"the database is cleaned of all records for NHS Numbers: (.*)")]
    public async Task GivenDatabaseIsCleaned(string nhsNumbersString)
    {
        var nhsNumbers = nhsNumbersString.Split(',', StringSplitOptions.TrimEntries);

        // _fileUploadService.CleanDatabaseAsync accepts a list of NHS numbers
        await _fileUploadService.CleanDatabaseAsync(nhsNumbers);
    }

    [Given(@"the application is properly configured")]
    public void GivenApplicationIsConfigured()
    {
        _fileUploadService.Should().NotBeNull("EndToEndFileUploadService is not initialized.");
    }

    [Given(@"file (.*) exists in the configured location for ""(.*)"" with NHS numbers : (.*)")]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public void GivenFileExistsAtConfiguredPath(string fileName, string? recordType, string nhsNumbersData)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    {
        //var folderPath1=_appSettings.FilePaths.ToString();
        //var folderPath = typeof(FilePaths).GetProperty(recordType!)?.GetValue(_appSettings.FilePaths)?.ToString();
        //dtos-service-insights-tests/TestFiles/add/bss_episodes_add_one_row.csv
        ///Users/nageswarundapalli/Documents/GitHub/dtos-service-insights-tests/dtos-service-insights-tests/TestFiles/add/bss_episodes_add_one_row.csv
        string workingDirectory = Environment.CurrentDirectory;
        string path = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        
        var folderPath=path + "/TestFiles/" + recordType +"/";
        //Console.WriteLine("Path is " + folderPath);
            var filePath = Path.Combine(folderPath!, fileName);
            //Console.WriteLine("File Path is " + filePath);

            _smokeTestsContext.FilePath = filePath;
           _smokeTestsContext.RecordType = (RecordTypesEnum)Enum.Parse(typeof(RecordTypesEnum), recordType, ignoreCase: true);

           _smokeTestsContext.NhsNumbers = nhsNumbersData.Split(',', StringSplitOptions.TrimEntries).ToList();
    }

    [Given(@"the file is uploaded to the Blob Storage container")]
    [When(@"the file is uploaded to the Blob Storage container")]
    public async Task WhenFileIsUploaded()
    {
        var filePath =_smokeTestsContext.FilePath;
        await _fileUploadService.UploadFileAsync(filePath);
    }

    [Given(@"the NHS numbers in the database should match the file data")]
    [Then(@"the NHS numbers in the database should match the file data")]
    public async Task ThenVerifyNhsNumbersInDatabase()
    {
        await _fileUploadService.VerifyNhsNumbersAsync("EPISODE", _smokeTestsContext.NhsNumbers!);
    }

        [Then("there should be (.*) records for the NHS Number in the database")]
    public async Task ThenThereShouldBeRecordsForThe(int count)
    {
        await _fileUploadService.VerifyNhsNumbersCountAsync("EPISODE", _smokeTestsContext.NhsNumbers.FirstOrDefault(),count);
    }

    [Then("the database should match the amended (.*) for the NHS Number")]
    public async Task ThenTheDatabaseShouldMatchTheAmendedForTheNHSNumber(string expectedGivenName)
    {
        await _fileUploadService.VerifyFieldUpdateAsync("EPISODE", _smokeTestsContext.NhsNumbers.FirstOrDefault(), "EPISODE_OPEN_DATE", expectedGivenName);
    }


}
