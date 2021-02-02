@VIH-6330
Feature: Endpoints
  In order to manage endpoints in a hearing
  As an api service
  I want to get, set, update or delete endpoint data

Scenario: Add an endpoint to a valid hearing 
    Given I have a hearing
    And I have add endpoint to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be added

Scenario: Add an endpoint with invalid data to a valid hearing 
    Given I have a hearing
    And I have add endpoint with invalid data to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    
Scenario: Add an endpoint to an invalid hearing 
    Given I have a hearing
    And I have add endpoint to a hearing request with a Invalid hearing id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Add an endpoint to an nonexistent hearing 
    Given I have a hearing
    And I have add endpoint to a hearing request with a Nonexistent hearing id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

Scenario: Remove an endpoint from a valid hearing 
    Given I have a hearing
    And I have remove endpoint to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be deleted

Scenario: Remove an endpoint from an invalid hearing 
    Given I have a hearing
    And I have remove endpoint to a hearing request with a Invalid hearing id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Remove an endpoint from an nonexistent hearing 
    Given I have a hearing
    And I have add endpoint to a hearing request with a Nonexistent hearing id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

Scenario: Remove an nonexistent endpoint from a valid hearing 
    Given I have a hearing
    And I have remove nonexistent endpoint to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

Scenario: Update an endpoint to a valid hearing 
    Given I have a hearing
    And I have update endpoint to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be updated

Scenario: Update an endpoint with invalid data to a valid hearing 
    Given I have a hearing
    And I have update endpoint with invalid data to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    
Scenario: Update an endpoint to an invalid hearing 
    Given I have a hearing
    And I have update endpoint to a hearing request with a Invalid hearing id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Update an endpoint to an nonexistent hearing 
    Given I have a hearing
    And I have update endpoint to a hearing request with a Nonexistent hearing id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

Scenario: Update an nonexistent endpoint from a valid hearing 
    Given I have a hearing
    And I have update nonexistent endpoint to a hearing request with a valid hearing id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False