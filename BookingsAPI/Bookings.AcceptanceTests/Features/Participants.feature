@VIH-3622
Feature: Participants
	In order to manage participants in a hearing
	As an api service
	I want to be able to create, update and retrieve hearing participants

Scenario: Get participants in a hearing
	Given I have a hearing
	And I have a get participants in a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of hearing participants should be retrieved

Scenario: Add participant to a hearing
	Given I have a hearing
	And I have an add participant to a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the participant should be added

	Scenario: Get a single participant in a hearing
	Given I have a hearing
	And I have a get a single participant in a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a hearing participant should be retrieved

	Scenario: Remove participant from a hearing
	Given I have a hearing
	And I have a remove participant from a hearing with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the participant should be removed