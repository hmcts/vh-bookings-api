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
| CaseTypes                         | CaseRole     |
| Children Act                      | Applicant    |
| Children Act                      | Respondent   |
| Children Act                      | Judge        |
| Children Act                      | Observer     |
| Children Act                      | Panel Member |
| Civil Money Claims                | Claimant     |
| Civil Money Claims                | Defendant    |
| Civil Money Claims                | Judge        |
| Civil Money Claims                | Observer     |
| Civil Money Claims                | Panel Member |
| Family Law Act                    | Applicant    |
| Family Law Act                    | Respondent   |
| Family Law Act                    | Judge        |
| Family Law Act                    | Observer     |
| Family Law Act                    | Panel Member |
| Financial Remedy                  | Applicant    |
| Financial Remedy                  | Respondent   |
| Financial Remedy                  | Judge        |
| Financial Remedy                  | Observer     |
| Financial Remedy                  | Panel Member |
| Generic                           | Applicant    |
| Generic                           | Respondent   |
| Generic                           | Judge        |
| Generic                           | Observer     |
| Generic                           | Panel Member |
| Tax                               | Applicant    |
| Tax                               | Respondent   |
| Tax                               | Judge        |
| Tax                               | Observer     |
| Tax                               | Panel Member |
| Tax                               | Appellant    |
| Tax                               | State        |
| Tribunal                          | Applicant    |
| Tribunal                          | Respondent   |
| Tribunal                          | Judge        |
| Tribunal                          | Observer     |
| Tribunal                          | Panel Member |
| Civil                             | Claimant     |
| Civil                             | Defendant    |
| Civil                             | Judge        |
| Civil                             | Observer     |
| Civil                             | Panel Member |
| Land Registration                 | Applicant    |
| Land Registration                 | Respondent   |
| Land Registration                 | Judge        |
| Land Registration                 | Observer     |
| Land Registration                 | Panel Member |
| Housing Act                       | Applicant    |
| Housing Act                       | Respondent   |
| Housing Act                       | Judge        |
| Housing Act                       | Observer     |
| Housing Act                       | Panel Member |
| Housing & Planning Act            | Applicant    |
| Housing & Planning Act            | Respondent   |
| Housing & Planning Act            | Judge        |
| Housing & Planning Act            | Observer     |
| Housing & Planning Act            | Panel Member |
| Leasehold Enfranchisement         | Applicant    |
| Leasehold Enfranchisement         | Respondent   |
| Leasehold Enfranchisement         | Judge        |
| Leasehold Enfranchisement         | Observer     |
| Leasehold Enfranchisement         | Panel Member |
| Leasehold Management              | Applicant    |
| Leasehold Management              | Respondent   |
| Leasehold Management              | Judge        |
| Leasehold Management              | Observer     |
| Leasehold Management              | Panel Member |
| Park Homes                        | Applicant    |
| Park Homes                        | Respondent   |
| Park Homes                        | Judge        |
| Park Homes                        | Observer     |
| Park Homes                        | Panel Member |
| Rents                             | Applicant    |
| Rents                             | Respondent   |
| Rents                             | Judge        |
| Rents                             | Observer     |
| Rents                             | Panel Member |
| Right to buy                      | Applicant    |
| Right to buy                      | Respondent   |
| Right to buy                      | Judge        |
| Right to buy                      | Observer     |
| Right to buy                      | Panel Member |
| Employment Tribunal               | Claimant     |
| Employment Tribunal               | Respondent   |
| Employment Tribunal               | Judge        |
| Employment Tribunal               | Observer     |
| Employment Tribunal               | Panel Member |
| Court of Appeal Criminal Division | None         |