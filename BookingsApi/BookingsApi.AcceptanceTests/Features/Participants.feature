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

	Scenario: Update participant details with user role Individual
	Given I have a hearing
	And I have an update participant details request with a valid user Individual 
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And 'Individual' details should be updated 

	Scenario: Update participant details with user role Representative
	Given I have a hearing
	And I have an update participant details request with a valid user Representative 
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And 'Representative' details should be updated

Scenario: Update suitability answers with user role Individual
	Given I have a hearing
	And I have an update participant suitability answers with a valid user 'Individual'
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And suitability answers for 'Individual' should be updated

Scenario: Update suitability answers with user role Representative
	Given I have a hearing
	And I have an update participant suitability answers with a valid user 'Representative'
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And suitability answers for 'Representative' should be updated
	
Scenario: Create a single linked participant for a participant
	Given I have a hearing
	And I create a request to link 2 distinct participants in the hearing
	When I send the request to the endpoint
	Then the response should have the status OK and success status True