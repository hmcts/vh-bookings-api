@VIH-3983
Feature: JudiciaryPersons
  In order to add JudiciaryPersons
  As an api service
  I want to be able to bulk add JudiciaryPersons to the system

  Scenario: Get person by a valid username
    Given I have a get person by username request with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And person details should be retrieved