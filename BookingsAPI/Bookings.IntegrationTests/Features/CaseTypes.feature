@VIH-3622
Feature: CaseTypes
	In order to retrieve a list of case types
	As an api service
	I want to be able to return a list of case types

@VIH-3582	
Scenario: Get available case types
	Given I have a get available case types request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case types should be contain
	| CaseTypeName              | HearingTypeName                                                                                                                           |
	| Civil Money Claims        | Application to Set Judgment Aside,First Application,Directions Hearing,Case Management Hearing,Final Hearing                              |
	| Financial Remedy          | First Directions Appointment,First Application,Directions Hearing,Case Management Hearing,Hearing,Final Hearing                           |
	| Generic                   | Hearing                                                                                                                                   |
	| Children Act              | Hearing                                                                                                                                   |
	| Tax                       | First Hearing,Substantive Hearing,Case Management,Directions Hearing,Hearing,Final Hearing,Basic,Standard,Complex,Schedule 36 Application |
	| Family Law Act            | Hearing                                                                                                                                   |
	| Tribunal                  | Hearing                                                                                                                                   |
	| Civil                     | Fast Track Trial                                                                                                                          |
	| Land Registration         | Case Management,Substantive hearing,Mediation                                                                                             |
	| Housing Act               | Case Management,Substantive hearing,Mediation                                                                                             |
	| Housing & Planning Act    | Case Management,Substantive hearing,Mediation                                                                                             |
	| Leasehold Enfranchisement | Case Management,Substantive hearing,Mediation                                                                                             |
	| Leasehold Management      | Case Management,Substantive hearing,Mediation                                                                                             |
	| Park Homes                | Case Management,Substantive hearing,Mediation                                                                                             |
	| Rents                     | Case Management,Substantive hearing,Mediation                                                                                             |
	| Right to buy              | Case Management,Substantive hearing,Mediation                                                                                             |


Scenario: Get case roles for a case type with nonexistent case type
	Given I have a get case roles for a case type of 'nonexistent' request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Get hearing roles for a case role of a case type with nonexistent case type
	Given I have a get hearing roles for a case type of 'Civil Money Claims' and case role of 'nonexistent' request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario: Get hearing roles for a case role of a case type with nonexistent role name
	Given I have a get hearing roles for a case type of 'nonexistent' and case role of 'Claimant' request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

Scenario Outline: Get case roles for a case type 
	Given I have a get case roles for a case type of <CaseTypes> request 
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of case roles should be retrieved
Examples:
| CaseTypes          |
| Civil Money Claims |
| Financial Remedy   |
| Generic            |
| Children Act       |
| Tax                |
| Family Law Act     |
| Tribunal           |
| Civil              |

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
| Tax                | Appellant  |
| Tax                | State      |
| Tribunal           | Applicant  |
| Tribunal           | Respondent |
| Tribunal           | Judge      |
| Civil              | Claimant   |
| Civil              | Defendant  |
| Civil              | Judge      |
