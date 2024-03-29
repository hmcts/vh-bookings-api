﻿@VIH-3622
Feature: Hearings
  In order to manage hearings
  As an api service
  I want to be able to create, update and retrieve hearings data

  Scenario: Get hearings by a username
    Given I have a valid get hearings by username request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list of hearing details should be retrieved

  Scenario: Hearing not retrieved with a nonexistent username
    Given I have a nonexistent get hearings by username request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the response should be an empty list

  Scenario: Delete a hearing
    Given I have a valid remove hearing request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the hearing should be removed

  Scenario: Delete a hearing with linked participants
    Given I have a hearing with linked participants
    And I have a remove a hearing request
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

  Scenario: Hearing status does not change for an invalid hearing id
    Given I have a invalid hearing cancellation request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

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
    
  @VIH-6168
  Scenario: Hearing status changes to failed for a given hearing id
    Given I have a valid hearing request
    And set the booking status to Failed
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And hearing status should be Failed

  Scenario: Get booking status for a given hearing
    Given I have a get booking status for a given request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And booking status should be retrieved