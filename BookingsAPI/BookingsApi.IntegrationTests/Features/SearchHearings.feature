Feature: Search for Hearings
  In order to search get audio recordings
  As an api service
  I want to be able to search for hearings by a given criteria

  Scenario: Search for hearings by case number
    Given I have a confirmed hearing
    And I have a search hearings by case number request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should contain the given hearing

  Scenario: Search for hearings by date
    Given I have a confirmed hearing
    And I have a search hearings by date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should contain the given hearing

  Scenario: Search for hearings by partial case number and date
    Given I have a confirmed hearing
    And I have a search hearings by partial case number and date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should contain the given hearing

  Scenario: Search for hearings by partial case number and wrong date
    Given I have a confirmed hearing
    And I have a search hearings by partial case number and the wrong date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should be empty

  Scenario: Search for hearings by incorrect case number and correct date
    Given I have a confirmed hearing
    And I have a search hearings by incorrect case number but the correct date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should be empty

  Scenario: Search for non-confirmed hearings by case number
    Given I have a hearing
    And I have a search hearings by case number request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should be empty

  Scenario: Search for non-confirmed hearings by date
    Given I have a hearing
    And I have a search hearings by date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should be empty

  Scenario: Search for non-confirmed hearings by partial case number and date
    Given I have a hearing
    And I have a search hearings by partial case number and date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And hearing search response should be empty