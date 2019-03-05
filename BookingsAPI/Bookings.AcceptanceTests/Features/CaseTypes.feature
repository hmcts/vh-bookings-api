@VIH-3622
Feature: CaseTypes
	In order to retrieve a list of case types
	As an api service
	I want to be able to return a list of case types

Scenario: Get case roles for a case type
	Given I have a get case roles for a case type of 'Civil Money Claims' request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case roles should be retrieved

Scenario: Get hearing roles for a case role of a case type
	Given I have a get hearing roles for a case role of 'Civil Money Claims' and case type of 'Claimant' request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of hearing roles should be retrieved