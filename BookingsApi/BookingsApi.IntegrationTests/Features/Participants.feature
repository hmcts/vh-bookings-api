@VIH-3622
Feature: Participants
	In order to manage participants in a hearing
	As an api service
	I want to be able to create, update and retrieve hearing participants

Scenario: Get participants in a hearing
	Given I have a get participants in a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of hearing participants should be retrieved

Scenario: Participants in a hearing not retrieved with nonexistent hearing id
	Given I have a get participants in a hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participants in a hearing not retrieved with invalid hearing id
	Given I have a get participants in a hearing request with an invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Add new participant to a hearing
	Given I have an add participant to a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the participant should be added

Scenario: Existing participant not added to the same hearing
	Given I have an add participant to a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	When I send the same request twice
	Then the response should have the status BadRequest and success status False
	And the error response message should contain 'Participant already exists'

Scenario: Existing participant added to a new hearing
	Given I have an add participant to a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the participant should be added
	When I send the same request but with a new hearing id
	Then the response should have the status NoContent and success status True
	And the participant should be added

Scenario: Participant not added to a hearing with nonexistent hearing id
	Given I have an add participant to a hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participant not added to a hearing with invalid hearing id
	Given I have an add participant to a hearing request with an invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Participants in a hearing not added with invalid participant
	Given I have an add participants in a hearing request with an invalid participant
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Get a single participant in a hearing
	Given I have a get a single participant in a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a hearing participant should be retrieved

Scenario: Single participant not retrieved with nonexistent hearing id
	Given I have a get a single participant in a hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Single participant not retrieved with invalid hearing id
	Given I have a get a single participant in a hearing request with an invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Single participant not retrieved with nonexistent participant id
	Given I have a get a single participant in a hearing request with a nonexistent participant id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Single participant not retrieved with invalid participant id
	Given I have a get a single participant in a hearing request with an invalid participant id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Remove participant from a hearing
	Given I have a remove participant from a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the participant should be removed

Scenario: Participants in a hearing not removed with nonexistent hearing id
	Given I have a remove participant from a hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participants in a hearing not removed with invalid hearing id
	Given I have a remove participant from a hearing request with an invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Participants in a hearing not removed with nonexistent participant id
	Given I have a remove participant from a hearing request with a nonexistent participant id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participants in a hearing not removed with invalid participant id
	Given I have a remove participant from a hearing request with an invalid participant id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Participant not updated with nonexistent hearing id
	Given I have an update participant in a hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participant not updated with invalid hearing id
	Given I have an update participant in a hearing request with a invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Participant not updated with invalid representee
	Given I have an update participant in a hearing request with a invalid representee
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	
Scenario: Participant details updated successfully
	Given I have an update participant in a hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True

Scenario: Participant submits suitability answers with invalid participant id
	Given I have an update suitability answers request with an valid hearing id and invalid participant id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Participant submits suitability answers with invalid hearing id
	Given I have an update suitability answers request with an invalid hearing id and valid participant id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Participant submits suitability answers with nonexisting hearing id
	Given I have an update suitability answers request with an Nonexistent hearing id and valid participant id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participant submits suitability answers with nonexisting participant id
	Given I have an update suitability answers request with an valid hearing id and Nonexistent participant id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Participant submits suitability answers with valid data
	Given I have an update suitability answers request with an valid hearing id and valid participant id
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True