@VIH-3622
Feature: Persons
	In order to manage persons
	As an api service
	I want to be able to retrieve persons from the database

Scenario: Get person by username
	Given I have a hearing
	And I have a get a person by username request with a valid username
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a person should be retrieved

	Scenario: Get person by contact email
	Given I have a hearing
	And I have a get a person by contact email request with a valid email
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a person should be retrieved