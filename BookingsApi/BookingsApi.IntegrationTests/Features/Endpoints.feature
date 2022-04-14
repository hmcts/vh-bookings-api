Feature: Endpoints
  In order to manage endpoints in a hearing
  As an api service
  I want to get, set, update or delete endpoint data

Scenario: Remove non-existent endpoint from a hearing
    Given I have a hearing without endpoints
    And I have remove non-existent endpoint from a hearing request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
  
Scenario: Remove an endpoint from a non-existent hearing
    Given I have a hearing with endpoints
    And I have remove endpoint from a non-existent hearing request
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
    Then the response should have the status Ok and success status True
    And the endpoint should be added

Scenario: Add an endpoint to a hearing that doesnt have any endpoints
    Given I have a hearing without endpoints
    And I have add endpoint to a hearing request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the endpoint should be added
    
 Scenario: Update an endpoint display name
    Given I have a hearing with endpoints
    And I have update display name of an endpoint request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be updated

  Scenario: Update an endpoint with defence advocate
    Given I have a hearing with endpoints
    And I have update an endpoint request with a defence advocate
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be updated with a defence advocate