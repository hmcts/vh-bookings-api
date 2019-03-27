@VIH-3622
Feature: CaseTypes
	In order to retrieve a list of case types
	As an api service
	I want to be able to return a list of case types

@VIH-3582	
Scenario: Get available case types
	Given I have a get available case types request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case types should be retrieved

Scenario: Get case roles for a case type with nonexistent case type
	Given I have a get case roles for a case type of 'nonexistent' request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Get hearing roles for a case role of a case type with nonexistent case type
	Given I have a get hearing roles for a case role of 'Civil Money Claims' and case type of 'nonexistent' request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Get hearing roles for a case role of a case type with nonexistent role name
	Given I have a get hearing roles for a case role of 'nonexistent' and case type of 'Claimant' request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario Outline: Get case roles for a case type 
	Given I have a get case roles for a case type of <CaseTypes> request 
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case roles should be retrieved
Examples:
| CaseTypes          |
| Financial Remedy   |
| Civil Money Claims |

Scenario Outline: Get hearing roles for a case role of a case type
	Given I have a get hearing roles for a case role of '<CaseTypes>' and case type of '<CaseRole>' request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of hearing roles should be retrieved

Examples:
| CaseTypes          | CaseRole   |
| Financial Remedy   | Applicant  |
| Financial Remedy   | Respondent |
| Financial Remedy   | Judge      |
| Civil Money Claims | Claimant   |
| Civil Money Claims | Defendant  |
| Civil Money Claims | Judge      |