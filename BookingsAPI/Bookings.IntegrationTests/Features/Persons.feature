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
	
  Scenario: Get person address by a valid contact email
    Given I have a get individual by contact email request with a valid contact email
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And person address should be retrieved

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


