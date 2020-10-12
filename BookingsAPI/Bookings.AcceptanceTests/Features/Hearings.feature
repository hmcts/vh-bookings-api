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

  Scenario: Get a hearing for a given username
    Given I have a hearing
    And I have a valid get hearing by username request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list of hearing details should be retrieved

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
    Given I have a valid book a new hearing for a case type Civil Money Claims
    And I have a get details for a given hearing request for case type Civil Money Claims
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing details should be retrieved for the case type

  Scenario: Cancel a hearing
    Given I have a hearing
    And I have a cancel hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And hearing should be cancelled

  @VIH-4121
  Scenario: Created a hearing
    Given I have a hearing
    And I have a created hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And hearing should be created

  Scenario: Search for hearing with a case number
    Given I have a hearing
    And I have a valid search for recorded hearings by case number request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list of hearing details should be retrieved for the case number

  Scenario: Search for hearing with an invalid case number
    Given I have a hearing
    And I have an invalid search for recorded hearings by case number request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And an empty list of hearing details should be retrieved

  @VIH-6168
  Scenario: Created a hearing and set it to failed
    Given I have a hearing
    And I have a created hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And hearing should be created
    When I have a failed confirmation hearing request with a valid hearing id
    And I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And hearing should be failed
