
Feature: SuitabilityAnswer
	In order to view all suitability answers
	As an api service
	I want to be able to retrieve suitability data
@VIH-4460
Scenario: Get all suitability answers sorted by latest one 
	Given I have a get suitable answers for participants
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And suitable answers should be retrieved
