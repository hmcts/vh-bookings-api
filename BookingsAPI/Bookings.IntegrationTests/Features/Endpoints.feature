Feature: Endpoints
  In order to manage endpoints in a hearing
  As an api service
  I want to get, set, update or delete endpoint data

Scenario: Remove non-existent endpoint from a hearing
    Given I have a hearing without endpoints
    And I have remove non-existent endpoint from a hearing request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    
  Scenario: Remove an endpoint from a hearing
    Given I have a hearing with endpoints
    And I have remove endpoint from a hearing request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be removed

Scenario: Add an additional endpoint to a hearing 
    Given I have a hearing with endpoints
    And I have add endpoint to a hearing request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be added

Scenario: Add an endpoint to a hearing that doesnt have any endpoints
    Given I have a hearing without endpoints
    And I have add endpoint to a hearing request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be added