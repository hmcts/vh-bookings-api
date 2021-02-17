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
      | CaseTypeName                      | HearingTypeName                                                                                                                                                                                                                                                                                                                                            |
      | Civil Money Claims                | Application to Set Judgment Aside,First Application,Directions Hearing,Case Management Hearing,Final Hearing                                                                                                                                                                                                                                               |
      | Financial Remedy                  | First Directions Appointment,First Application,Directions Hearing,Case Management Hearing,Hearing,Final Hearing,Financial Dispute Resolution,Interim hearing                                                                                                                                                                                                                                            |
      | Generic                           | Hearing,Daily Test,Demo,Familiarisation,One to one,Test,Automated Test                                                                                                                                                                                                                                                                                                                                          |
      | Children Act                      | Hearing                                                                                                                                                                                                                                                                                                                                                    |
      | Tax                               | First Hearing,Substantive Hearing,Case Management,Directions Hearing,Hearing,Final Hearing,Basic,Standard,Complex,Schedule 36 Application                                                                                                                                                                                                                  |
      | Family Law Act                    | Hearing,Application,Case Management Conference,Directions,First hearing,Full                                                                                                                                                                                                                                                                                                                                                    |
      | Tribunal                          | Hearing                                                                                                                                                                                                                                                                                                                                                    |
      | Civil                             | Fast Track Trial,Case Management Hearing,Costs,Enforcement Hearing,General Application,Infant Settlement,Injunction,Insolvency,Multi Track Trial,Part 8 (General),Possession Hearing,Return of Goods,Small Claim Trial,Stage 3 Part 8 Hearing                                                                                                                                                                                                                                                                                                                                           |
      | Land Registration                 | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Housing Act                       | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Housing & Planning Act            | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Leasehold Enfranchisement         | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Leasehold Management              | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Park Homes                        | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Rents                             | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Right to buy                      | Case Management,Substantive hearing,Mediation                                                                                                                                                                                                                                                                                                              |
      | Employment Tribunal               | Public Preliminary Hearing,Private Preliminary Hearing,Mediation Hearing,Interim Relief Hearing,Substantive Hearing (Judge sit alone),Substantive Hearing (Full Panel),Costs Hearing                                                                                                                                                                       |
      | Court of Appeal Criminal Division | FC Appeal,FC Application,Directions,Appn to treat abandonment as a nullity,Section 13,PII on notice,PII not on notice,Costs application,SC pronouncement,Reserved Judgment,Reasons for Judgment,Hand down Judgment,Appn to reopen case,Appn for leave to appeal to SC,Order of SC to be made order of CACD,Ref for summary dismissal under S20,For mention |
      | GRC - Charity                     | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - CRB                         | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - DVSA                        | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Estate Agents               | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Food                        | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Environment                 | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Gambling                    | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Immigration Services        | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Information Rights          | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Pensions Regulation         | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Professional Regulations    | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Query Jurisdiction          | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | GRC - Welfare of Animals          | Case Management Hearing,Final Hearing,Costs Hearing                                                                                                                                                                                                                                                                                                        |
      | Immigration and Asylum            | Appeals - Substantive Statutory,Bail Hearing,Case Management Appointment,Case Management Review,Costs Hearing,Payment Liability Hearing,Preliminary Hearing                                                                                                                                                                                                |
      | Private Law                       | Application,Case Management Conference,Directions,First hearing,Full hearing,Pre hearing review,Review                                                                                                                                                                                                                                                     |
      | Public Law - Care                 | Application,Case Management Conference,Case Management Hearing,Directions,Full,Further CMH,Interim Care Order,Issues Resolution Hearing,Pre Hearing Review                                                                                                                                                                                                |

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
      | CaseTypes                         |
      | Civil Money Claims                |
      | Financial Remedy                  |
      | Generic                           |
      | Children Act                      |
      | Tax                               |
      | Family Law Act                    |
      | Tribunal                          |
      | Civil                             |
      | Land Registration                 |
      | Housing Act                       |
      | Housing & Planning Act            |
      | Leasehold Enfranchisement         |
      | Leasehold Management              |
      | Park Homes                        |
      | Rents                             |
      | Right to buy                      |
      | Employment Tribunal               |
      | Court of Appeal Criminal Division |
      | GRC - Charity                     |
      | GRC - CRB                         |
      | GRC - DVSA                        |
      | GRC - Estate Agents               |
      | GRC - Food                        |
      | GRC - Environment                 |
      | GRC - Gambling                    |
      | GRC - Immigration Services        |
      | GRC - Information Rights          |
      | GRC - Pensions Regulation         |
      | GRC - Professional Regulations    |
      | GRC - Query Jurisdiction          |
      | GRC - Welfare of Animals          |
      | Immigration and Asylum            |

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
      | Civil Money Claims                | Claimant     |
      | Civil Money Claims                | Defendant    |
      | Civil Money Claims                | Judge        |
      | Family Law Act                    | Applicant    |
      | Family Law Act                    | Respondent   |
      | Family Law Act                    | Judge        |
      | Financial Remedy                  | Applicant    |
      | Financial Remedy                  | Respondent   |
      | Financial Remedy                  | Judge        |
      | Generic                           | Applicant    |
      | Generic                           | Respondent   |
      | Generic                           | Judge        |
      | Tax                               | Applicant    |
      | Tax                               | Respondent   |
      | Tax                               | Judge        |
      | Tax                               | Appellant    |
      | Tax                               | State        |
      | Tribunal                          | Applicant    |
      | Tribunal                          | Respondent   |
      | Tribunal                          | Judge        |
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
      | GRC - Charity                     | Applicant    |
      | GRC - Charity                     | Respondent   |
      | GRC - Charity                     | Judge        |
      | GRC - Charity                     | Panel Member |
      | GRC - Charity                     | Observer     |
      | GRC - CRB                         | Applicant    |
      | GRC - CRB                         | Respondent   |
      | GRC - CRB                         | Judge        |
      | GRC - CRB                         | Panel Member |
      | GRC - CRB                         | Observer     |
      | GRC - DVSA                        | Applicant    |
      | GRC - DVSA                        | Respondent   |
      | GRC - DVSA                        | Judge        |
      | GRC - DVSA                        | Panel Member |
      | GRC - DVSA                        | Observer     |
      | GRC - Estate Agents               | Applicant    |
      | GRC - Estate Agents               | Respondent   |
      | GRC - Estate Agents               | Judge        |
      | GRC - Estate Agents               | Panel Member |
      | GRC - Estate Agents               | Observer     |
      | GRC - Food                        | Applicant    |
      | GRC - Food                        | Respondent   |
      | GRC - Food                        | Judge        |
      | GRC - Food                        | Panel Member |
      | GRC - Food                        | Observer     |
      | GRC - Environment                 | Applicant    |
      | GRC - Environment                 | Respondent   |
      | GRC - Environment                 | Judge        |
      | GRC - Environment                 | Panel Member |
      | GRC - Environment                 | Observer     |
      | GRC - Gambling                    | Applicant    |
      | GRC - Gambling                    | Respondent   |
      | GRC - Gambling                    | Judge        |
      | GRC - Gambling                    | Panel Member |
      | GRC - Gambling                    | Observer     |
      | GRC - Immigration Services        | Applicant    |
      | GRC - Immigration Services        | Respondent   |
      | GRC - Immigration Services        | Judge        |
      | GRC - Immigration Services        | Panel Member |
      | GRC - Immigration Services        | Observer     |
      | GRC - Information Rights          | Applicant    |
      | GRC - Information Rights          | Respondent   |
      | GRC - Information Rights          | Judge        |
      | GRC - Information Rights          | Panel Member |
      | GRC - Information Rights          | Observer     |
      | GRC - Pensions Regulation         | Applicant    |
      | GRC - Pensions Regulation         | Respondent   |
      | GRC - Pensions Regulation         | Judge        |
      | GRC - Pensions Regulation         | Panel Member |
      | GRC - Pensions Regulation         | Observer     |
      | GRC - Professional Regulations    | Applicant    |
      | GRC - Professional Regulations    | Respondent   |
      | GRC - Professional Regulations    | Judge        |
      | GRC - Professional Regulations    | Panel Member |
      | GRC - Professional Regulations    | Observer     |
      | GRC - Query Jurisdiction          | Applicant    |
      | GRC - Query Jurisdiction          | Respondent   |
      | GRC - Query Jurisdiction          | Judge        |
      | GRC - Query Jurisdiction          | Panel Member |
      | GRC - Query Jurisdiction          | Observer     |
      | GRC - Welfare of Animals          | Applicant    |
      | GRC - Welfare of Animals          | Respondent   |
      | GRC - Welfare of Animals          | Judge        |
      | GRC - Welfare of Animals          | Panel Member |
      | GRC - Welfare of Animals          | Observer     |
      | Immigration and Asylum            | Appellant    |
      | Immigration and Asylum            | Home Office  |
      | Immigration and Asylum            | Observer     |
      | Immigration and Asylum            | Panel Member |
      | Immigration and Asylum            | Judge        |
