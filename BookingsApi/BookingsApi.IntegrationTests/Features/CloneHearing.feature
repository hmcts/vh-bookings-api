Feature: Clone Hearings
  In order to create multi-day bookings quickly
  As an API service
  I want to be able to clone a hearing

Scenario: Attempt to clone a non-existent hearing
	Given I have a request to clone a non-existent hearing
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Attempt to clone a hearing using invalid dates
	Given I have a hearing with endpoints
	And I have a request to clone a hearing with invalid dates
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Dates cannot be before original hearing'
	And the response message should read 'Dates must be unique'

Scenario: Successfully clone a hearing with valid dates
	Given I have a hearing with endpoints
	And I have a request to clone a hearing with valid dates
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the database should have cloned hearings

Scenario: Successfully clone a hearing with valid dates and with Linked Participants
	Given I have a hearing with linked participants
	And I have a request to clone a hearing with valid dates
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the database should have cloned hearings