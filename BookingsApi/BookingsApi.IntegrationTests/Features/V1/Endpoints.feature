﻿Feature: Endpoints
  In order to manage endpoints in a hearing
  As an api service
  I want to get, set, update or delete endpoint data

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