using System.Text.Json;
using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Microsoft.Extensions.Options;

namespace BookingsApi.UnitTests.Infrastructure.Services
{
    public class ServiceBusQueueClientTests
    {
        private ServiceBusQueueClient _client;
        
        [SetUp]
        public void Setup()
        {
            var settings = new ServiceBusSettings()
            {
                ConnectionString = "DevelopmentMode=true",
                QueueName = "booking"
            };
            _client = new ServiceBusQueueClient(Options.Create(settings));
        }

        [Test]
        public void Should_initialise_serialiser_settings()
        {
            ServiceBusQueueClient.SerializerSettings.Should().BeOfType<JsonSerializerOptions>();
        }
        
        [Test]
        public void Should_serialize_message()
        {
            // Arrange
            var hearing = new VideoHearingBuilder().WithCase().Build();
            var integrationEvent = new HearingIsReadyForVideoIntegrationEvent(hearing, hearing.Participants);
            var eventMessage = new EventMessage(integrationEvent);
            
            // Act
            var serializedMessage = ServiceBusQueueClient.SerializeMessage(eventMessage);
            
            // Assert
            var id = eventMessage.Id;
            var timestamp = eventMessage.Timestamp.ToString("O");
            var hearingId = hearing.Id;
            var scheduledDateTime = hearing.ScheduledDateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var participant1 = ParticipantDtoMapper.MapToDto(hearing.Participants[0]);
            var participant2 = ParticipantDtoMapper.MapToDto(hearing.Participants[1]);
            var participant3 = ParticipantDtoMapper.MapToDto(hearing.Participants[2]);
            var participant4 = ParticipantDtoMapper.MapToDto(hearing.Participants[3]);
            var participant5 = ParticipantDtoMapper.MapToDto(hearing.Participants[4]);
            var expectedMessage = $@"
            {{
              ""$type"": ""BookingsApi.Infrastructure.Services.IntegrationEvents.EventMessage, BookingsApi.Infrastructure.Services"",
              ""id"": ""{id}"",
              ""timestamp"": ""{timestamp}"",
              ""integration_event"": {{
                ""$type"": ""BookingsApi.Infrastructure.Services.IntegrationEvents.Events.HearingIsReadyForVideoIntegrationEvent, BookingsApi.Infrastructure.Services"",
                ""hearing"": {{
                  ""$type"": ""BookingsApi.Infrastructure.Services.Dtos.HearingDto, BookingsApi.Infrastructure.Services"",
                  ""hearing_id"": ""{hearingId}"",
                  ""group_id"": ""00000000-0000-0000-0000-000000000001"",
                  ""scheduled_date_time"": ""{scheduledDateTime}"",
                  ""scheduled_duration"": 80,
                  ""case_type"": ""Generic"",
                  ""case_number"": ""AutoTest"",
                  ""case_name"": ""Test"",
                  ""hearing_venue_name"": ""Birmingham Civil and Family Justice Centre"",
                  ""record_audio"": true,
                  ""hearing_type"": ""Automated Test"",
                  ""case_type_service_id"": ""ZZY1"",
                  ""video_supplier"": ""kinly""
                }},
                ""participants"": [
                  {{
                    ""$type"": ""BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services"",
                    ""participant_id"": ""{participant1.ParticipantId}"",
                    ""fullname"": ""{participant1.Fullname}"",
                    ""username"": ""{participant1.ContactEmail}"",
                    ""first_name"": ""VH Automation_FirstName"",
                    ""last_name"": ""VH Automation_LastName"",
                    ""contact_email"": ""{participant1.ContactEmail}"",
                    ""contact_telephone"": ""01234567890"",
                    ""display_name"": ""VH Automation_FirstName VH Automation_LastName"",
                    ""hearing_role"": ""Litigant in person"",
                    ""user_role"": ""Individual"",
                    ""case_group_type"": ""applicant"",
                    ""representee"": """",
                    ""linked_participants"": [],
                    ""contact_email_for_non_e_jud_judge_user"": null,
                    ""contact_phone_for_non_e_jud_judge_user"": null,
                    ""send_hearing_notification_if_new"": true
                  }},
                  {{
                    ""$type"": ""BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services"",
                    ""participant_id"": ""{participant2.ParticipantId}"",
                    ""fullname"": ""{participant2.Fullname}"",
                    ""username"": ""{participant2.ContactEmail}"",
                    ""first_name"": ""VH Automation_FirstName"",
                    ""last_name"": ""VH Automation_LastName"",
                    ""contact_email"": ""{participant2.ContactEmail}"",
                    ""contact_telephone"": ""01234567890"",
                    ""display_name"": ""VH Automation_FirstName VH Automation_LastName"",
                    ""hearing_role"": ""Litigant in person"",
                    ""user_role"": ""Individual"",
                    ""case_group_type"": ""respondent"",
                    ""representee"": """",
                    ""linked_participants"": [],
                    ""contact_email_for_non_e_jud_judge_user"": null,
                    ""contact_phone_for_non_e_jud_judge_user"": null,
                    ""send_hearing_notification_if_new"": true
                  }},
                  {{
                    ""$type"": ""BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services"",
                    ""participant_id"": ""{participant3.ParticipantId}"",
                    ""fullname"": ""{participant3.Fullname}"",
                    ""username"": ""{participant3.ContactEmail}"",
                    ""first_name"": ""VH Automation_FirstName"",
                    ""last_name"": ""VH Automation_LastName"",
                    ""contact_email"": ""{participant3.ContactEmail}"",
                    ""contact_telephone"": ""01234567890"",
                    ""display_name"": ""VH Automation_FirstName VH Automation_LastName"",
                    ""hearing_role"": ""Representative"",
                    ""user_role"": ""Representative"",
                    ""case_group_type"": ""respondent"",
                    ""representee"": """",
                    ""linked_participants"": [],
                    ""contact_email_for_non_e_jud_judge_user"": null,
                    ""contact_phone_for_non_e_jud_judge_user"": null,
                    ""send_hearing_notification_if_new"": true
                  }},
                  {{
                    ""$type"": ""BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services"",
                    ""participant_id"": ""{participant4.ParticipantId}"",
                    ""fullname"": ""{participant4.Fullname}"",
                    ""username"": ""{participant4.ContactEmail}"",
                    ""first_name"": ""VH Automation_FirstName"",
                    ""last_name"": ""VH Automation_LastName"",
                    ""contact_email"": ""{participant4.ContactEmail}"",
                    ""contact_telephone"": ""01234567890"",
                    ""display_name"": ""VH Automation_FirstName VH Automation_LastName"",
                    ""hearing_role"": ""Judge"",
                    ""user_role"": ""Judge"",
                    ""case_group_type"": ""judge"",
                    ""representee"": """",
                    ""linked_participants"": [],
                    ""contact_email_for_non_e_jud_judge_user"": """",
                    ""contact_phone_for_non_e_jud_judge_user"": """",
                    ""send_hearing_notification_if_new"": true
                  }},
                  {{
                    ""$type"": ""BookingsApi.Infrastructure.Services.Dtos.ParticipantDto, BookingsApi.Infrastructure.Services"",
                    ""participant_id"": ""{participant5.ParticipantId}"",
                    ""fullname"": ""{participant5.Fullname}"",
                    ""username"": ""{participant5.ContactEmail}"",
                    ""first_name"": ""VH Automation_FirstName"",
                    ""last_name"": ""VH Automation_LastName"",
                    ""contact_email"": ""{participant5.ContactEmail}"",
                    ""contact_telephone"": ""01234567890"",
                    ""display_name"": ""VH Automation_FirstName VH Automation_LastName"",
                    ""hearing_role"": ""Judicial Office Holder"",
                    ""user_role"": ""Judicial Office Holder"",
                    ""case_group_type"": ""winger"",
                    ""representee"": """",
                    ""linked_participants"": [],
                    ""contact_email_for_non_e_jud_judge_user"": null,
                    ""contact_phone_for_non_e_jud_judge_user"": null,
                    ""send_hearing_notification_if_new"": true
                  }}
                ],
                ""endpoints"": []
              }}
            }}";

            var expectedJson = NormalizeJson(expectedMessage);
            var actualJson = NormalizeJson(serializedMessage);
            actualJson.Should().BeEquivalentTo(expectedJson);
        }
        
        private static string NormalizeJson(string json)
        {
          return new string(json.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}