Feature: Search for Persons
  In order to view persons' information
  As an api service
  I want to be able to search for persons stored in the system
    
  Scenario: Search for person without contact email
    Given I have a search for a individual with an empty query request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    
  Scenario: Search for person who does not exist
    Given I have a search for a individual request for a non existent person
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Search for a judge
    Given I have a hearing
    And I have a search for a individual request for a judge
    When I send the request to the endpoint
    Then the response should have the status Unauthorized and success status False
    
  Scenario: Search for an individual
    Given I have a hearing
    And I have a search for a individual request for an individual
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And person details should be retrieved