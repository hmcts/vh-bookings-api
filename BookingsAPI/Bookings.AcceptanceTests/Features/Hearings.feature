@VIH-3622
Feature: Hearings
	In order to manage hearings
	As an api service
	I want to be able to create, update and retrieve hearings data

Scenario: Get details for a given hearing
	Given I have a hearing
	And I have a get details for a given hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing details should be retrieved

Scenario: Create a new hearing
	Given I have a valid book a new hearing request
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And hearing details should be retrieved

Scenario: Update a hearing
	Given I have a hearing
	And I have a valid update hearing request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing details should be updated

Scenario: Delete a hearing
	Given I have a hearing
	And I have a remove hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the hearing no longer exists

@VIH-4193
Scenario: Get hearing details for a given case type
	Given I have a valid book a new hearing for a case type 
	And I have a get details for a given hearing request with a valid case type 
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing details should be retrieved for the case type

Scenario: Cancel a hearing
Given I have a hearing
And I have a cancel hearing request with a valid hearing id
When I send the request to the endpoint
Then the response should have the status OK and success status True
And hearing details should be updated