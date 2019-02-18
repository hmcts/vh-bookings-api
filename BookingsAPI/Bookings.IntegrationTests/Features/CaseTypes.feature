Feature: CaseTypes
	In order to retrieve a list of case types
	As an api service
	I want to be able to return a list of case types
	
Scenario: Get case roles for a case type
	Given I have a valid get case roles for a case type request
	When I send the request to the endpoint
	Then the response should have the status OK
	And a list of case roles should be retrieved

Scenario: Get case roles for a case type with nonexistent case type
	Given I have a nonexistent case type in a get case roles for a case type request
	When I send the request to the endpoint
	Then the response should have the status NotFound

Scenario: Get hearing roles for a case role of a case type
	Given I have a valid get hearing roles for a case role of a case type request
	When I send the request to the endpoint
	Then the response should have the status OK
	And a list of hearing roles should be retrieved

Scenario: Get hearing roles for a case role of a case type with nonexistent case type
	Given I have a nonexistentCaseType in a get hearing roles for a case role of a case type request
	When I send the request to the endpoint
	Then the response should have the status NotFound

Scenario: Get hearing roles for a case role of a case type with nonexistent role name
	Given I have a nonexistentRoleName in a get hearing roles for a case role of a case type request
	When I send the request to the endpoint
	Then the response should have the status NotFound