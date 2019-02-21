@VIH-3622
Feature: Hearing Venues
	In order to manage hearing venues
	As an api service
	I want to be able to retrieve hearing venue data

Scenario: Get all hearing venues available for booking
	Given I have a get all hearing venues available for booking request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And hearing venues should be retrieved