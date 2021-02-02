@VIH-3983
Feature: Persons
  In order to view persons' information
  As an api service
  I want to be able to retrieve persons stored in the system

  Scenario: Get person by a valid username
    Given I have a get person by username request with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And person details should be retrieved

  Scenario: Get person by an invalid username
    Given I have a get person by username request with an invalid username
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Get person by non existent username
    Given I have a get person by username request with a nonexistent username
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Get person by a valid contact email
    Given I have a get person by contact email request with a valid contact email
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And person details should be retrieved

  Scenario: Get person by an invalid contact email
    Given I have a get person by contact email request with an invalid contact email
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Get person by non existent contact email
    Given I have a get person by contact email request with a nonexistent contact email
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Get person by contact email search term
    Given I have a get person by contact email search term request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And persons details should be retrieved

  Scenario: Get person by contact email search term, case insensitive
    Given I have a get person by contact email search term request that case insensitive
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And persons details should be retrieved

  Scenario: Get person suitability answers for invalid username
    Given I have a get person suitability answers by username request with an invalid username
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Get person suitability answers for valid username
    Given I have a get person suitability answers by username request with an valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And suitability answers retrieved should 'not be empty'

  Scenario: Get person without suitability answers for valid username
    Given I have a get person without suitability answers by username request with an valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And suitability answers retrieved should 'be empty'

  Scenario: Get a list of usernames for hearings older than 3 months
    Given I have an hearing older than 3 months
    And I have a request to get the usernames for old hearings
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list of hearing usernames should be retrieved

  Scenario: Get list of hearings for judge username for deletion
    Given I have a hearing
    And I have a search for hearings using a judge username request
    When I send the request to the endpoint
    Then the response should have the status Unauthorized and success status False

  Scenario: Get list of hearings for non-judge username for deletion
    Given I have a hearing
    And I have a search for hearings using a non-judge username request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list of hearings for deletion is 1

  Scenario: Get Not Found for non-existent username for deletion
    Given I have a search for hearings using non-existent username request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    
  Scenario: Successfully anonymise a person
    Given I have a hearing
    And I have a valid anonymise person request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True

  Scenario: Anonymise a non-existent person
    Given I have a hearing
    And I have a non-existent anonymise person request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update details for a non-existent person
    Given I have a hearing
    And I have a non-existent update person details request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update details for a person with missing details
    Given I have a hearing
    And I have a malformed update person details request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Update details for a person with valid details
    Given I have a hearing
    And I have a valid update person details request
    When I send the request to the endpoint
    Then the response should have the status Accepted and success status True