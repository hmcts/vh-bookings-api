@VIH-3622
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
	Given I have an invalid remove hearing request
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the response message should read 'Please provide a valid hearingId'

Scenario: Hearing not deleted with a nonexistent hearing id
	Given I have a nonexistent remove hearing request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False


	Scenario: Hearing not created with an invalid address
    Given I have a book a new hearing request with an invalid address
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And the error response message should contain 'HouseNumber is required'
	And the error response message should also contain 'Street is required'
	And the error response message should also contain 'City is required'
	And the error response message should also contain 'County is required'
	And the error response message should also contain 'Postcode is required'

Scenario: Get hearing details for a given case type
	Given I have a valid book a new hearing request
	And I have a get details for a given hearing request with a valid case type
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing details should be retrieved for the case type

Scenario: Get a paged list of booked hearings
	Given I have a request to the get booked hearings endpoint
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the response should contain a list of booked hearings

Scenario: Get a paged list of booked hearings continued from previous page
	Given I have a request to the second page of booked hearings
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the response should contain a list of booked hearings

Scenario: Get a paged list of booked hearings limited in size
	Given I have a request to the get booked hearings endpoint with a limit of one
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the response should contain a list of one booked hearing

Scenario: Get a paged list of booked hearings with a given case type
	Given I have a request to the get booked hearings endpoint filtered on a valid case type
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the response should contain a list of booked hearings

Scenario: Cannot get a paged list of booked hearings with invalid case type
	Given I have a request to the get booked hearings endpoint filtered on an invalid case type
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	
Scenario: Hearing status does not change for a nonexistent hearing id
	Given I have a nonexistent hearing cancellation request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False
	And hearing status should be unchanged

Scenario: Hearing status does not change for an invalid hearing id
	Given I have a invalid hearing cancellation request
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And hearing status should be unchanged

Scenario: Hearing status does not change with empty username for given hearing id
	Given I have an empty username in a hearing status request
	When I send the request to the endpoint
	Then the response should have the status BadRequest and success status False
	And hearing status should be unchanged

Scenario: Hearing status changes to cancelled for a given hearing id
	Given I have a valid hearing cancellation request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And hearing status should be cancelled
	
Scenario: Hearing status cannot change for an invalid state transition for given hearing id
	Given I have a valid hearing request
	And set the booking status to Cancelled
	When I send the request to the endpoint
	And set the booking status to Created
	When I send the request to the endpoint
	Then the response should have the status Conflict and success status False
	And hearing status should be Cancelled

