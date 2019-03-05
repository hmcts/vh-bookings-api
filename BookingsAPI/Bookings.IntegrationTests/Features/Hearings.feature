﻿@VIH-3622
Feature: Hearings
	In order to manage hearings
	As an api service
	I want to be able to create, update and retrieve hearings data

Scenario: Get details for a given hearing
	Given I have a get details for a given hearing request with a valid hearing id
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing details should be retrieved

Scenario: Get details for a given hearing with a nonexistent hearing
	Given I have a get details for a given hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Get details for a given hearing with a bad request
	Given I have a get details for a given hearing request with an invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False

Scenario: Create a new hearing
	Given I have a valid book a new hearing request
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And hearing details should be retrieved

Scenario: Hearing not created with an invalid request
	Given I have an invalid book a new hearing request
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the error response message should contain 'Cases' must not be empty'
	And the error response message should also contain 'Please provide at least one case'
	And the error response message should also contain 'Please provide a case type name'
	And the error response message should also contain 'Participants' must not be empty'
	And the error response message should also contain 'Please provide at least one participant'
	And the error response message should also contain 'Please provide a hearing type name'
	And the error response message should also contain 'Hearing venue cannot not be blank'
	And the error response message should also contain 'Schedule duration must be greater than 0'
	And the error response message should also contain 'ScheduledDateTime cannot be in the past'

Scenario: Hearing not created with an invalid case type
	Given I have a book a new hearing request with an invalid case type
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Case type does not exist'

Scenario: Hearing not created with an invalid hearing type
	Given I have a book a new hearing request with an invalid hearing type
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Hearing type does not exist'

Scenario: Hearing not created with an invalid venue name
	Given I have a book a new hearing request with an invalid venue name
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Hearing venue does not exist'

Scenario: Update a hearing
	Given I have a valid update hearing request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing details should be updated

Scenario: Hearing not updated with an invalid request
	Given I have an invalid update hearing request
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the error response message should contain 'Hearing name cannot not be blank'
	And the error response message should also contain 'Schedule duration must be greater than 0"'
	And the error response message should also contain 'ScheduledDateTime cannot be in the past"'

Scenario: Hearing not updated with a nonexistent hearing id
	Given I have a update hearing request with a nonexistent hearing id
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Hearing not updated with an invalid hearing id
	Given I have a update hearing request with an invalid hearing id
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Please provide a valid hearingId'

Scenario: Hearing not updated with an invalid venue
	Given I have a update hearing request with an invalid venue
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Hearing venue does not exist'

Scenario: Delete a hearing
	Given I have a valid remove hearing request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the hearing should be removed

Scenario: Hearing not deleted with an invalid hearing id
	Given I have remove hearing request with an nonexistent hearing id 
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False