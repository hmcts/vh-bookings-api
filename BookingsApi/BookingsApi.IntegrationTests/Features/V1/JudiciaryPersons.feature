@VIH-3983
Feature: JudiciaryPersons
  In order to add JudiciaryPersons
  As an api service
  I want to be able to bulk add JudiciaryPersons to the system

  Scenario: Bulk add Judiciary Persons
    Given I have a post bulk judiciary persons request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the judiciary persons should be saved