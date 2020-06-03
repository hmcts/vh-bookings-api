﻿@VIH-3622
Feature: CaseTypes
	In order to retrieve a list of case types
	As an api service
	I want to be able to return a list of case types

@VIH-3582
Scenario: Get available case types
	Given I have a get available case types request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case types should be retrieved

Scenario: Get case roles for a case type
	Given I have a get case roles for a case type of 'Civil Money Claims' request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case roles should be retrieved

Scenario Outline: Get hearing roles for a case role of a case type
	Given I have a get hearing roles for a case type of '<CaseTypes>' and case role of '<CaseRole>' request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of hearing roles should be retrieved
Examples:
| CaseTypes          | CaseRole   |
| Children Act       | Applicant  |
| Children Act       | Respondent |
| Children Act       | Judge      |
| Civil Money Claims | Claimant   |
| Civil Money Claims | Defendant  |
| Civil Money Claims | Judge      |
| Family Law Act     | Applicant  |
| Family Law Act     | Respondent |
| Family Law Act     | Judge      |
| Financial Remedy   | Applicant  |
| Financial Remedy   | Respondent |
| Financial Remedy   | Judge      |
| Generic            | Applicant  |
| Generic            | Respondent |
| Generic            | Judge      |
| Tax                | Applicant  |
| Tax                | Respondent |
| Tax                | Judge      |
| Tribunal           | Applicant  |
| Tribunal           | Respondent |
| Tribunal           | Judge      |
| Civil              | Claimant   |
| Civil              | Defendant  |
| Civil              | Judge      |