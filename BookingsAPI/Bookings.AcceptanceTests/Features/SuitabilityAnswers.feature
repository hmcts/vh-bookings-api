Feature: SuitabilityAnswer
	In order to view all suitability answers
	As an api service
	I want to be able to retrieve suitability answers

@VIH-4460
Scenario: Get all suitability answers sorted by latest one
	Given I have a hearing
	And I have the suitable answers submitted
	And I have a request to get the suitable answers
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And suitable answers should be retrieved

@VIH-4460-1
Scenario: Get next set of suitable answers for participants continued from the next cursor
	Given I have a request to the second set of suitable answers
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And suitable answers should be retrieved
	
