Feature: DTOS Application Insights Smoke tests

Smoke tests to check the framework

Background:
        Given the database is cleaned of all records for NHS Numbers: 9990007068
		And the application is properly configured

@DTOSS-XXXX
Scenario: 01. Verify new episode is created 
	Given file <FileName> exists in the configured location for "Add" with NHS numbers : <NhsNumbers>
	When the file is uploaded to the Blob Storage container
	Then the NHS numbers in the database should match the file data

    Examples:
        | FileName                     | RecordType | NhsNumbers             |
        | bss_episodes_add_one_row.csv | Add        | 9990007068 |


@DTOSS-6257
Scenario: 02. Verify episode is updated in the database
    Given file <AddFileName> exists in the configured location for "Add" with NHS numbers : <NhsNumbers>
    And the file is uploaded to the Blob Storage container
    And the NHS numbers in the database should match the file data
    Given file <AmendedFileName> exists in the configured location for "Amended" with NHS numbers : <NhsNumbers>
    When the file is uploaded to the Blob Storage container
    Then there should be 1 records for the NHS Number in the database
    And the database should match the amended <AmendedEpisodeDateValue> for the NHS Number

    Examples:
        | AddFileName                  |  AmendedFileName                | NhsNumbers | AmendedEpisodeDateValue |
        | bss_episodes_add_one_row.csv | bss_episodes_update_one_row.csv | 9990007068 | 01/03/2021    |
