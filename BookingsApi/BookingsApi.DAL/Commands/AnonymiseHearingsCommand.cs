using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
	public class AnonymiseHearingsCommand : ICommand
    {
        public int RecordsUpdated { get; set; }
		public DateTime UpdatedDate { get; set; }
		public AnonymiseHearingsCommand()
        {
        }
    }

    public class AnonymiseHearingsCommandHandler : ICommandHandler<AnonymiseHearingsCommand>
    {
        private readonly BookingsDbContext _context;
        public AnonymiseHearingsCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseHearingsCommand command)
        {
            var query = "DECLARE " +
				"@randomString AS VARCHAR(64), " +
				"@months AS INT, " +
				"@anonymiseBeforeDate AS DATETIME, " +
				"@hearingId AS uniqueidentifier, " +
				"@conferenceId AS uniqueidentifier, " +
				"@participantId AS uniqueidentifier, " +
				"@personId AS uniqueidentifier, " +
				"@organisationId as INT, " +
				"@caseId AS INT " +
			"SET @months = -3 " +
			"SET @anonymiseBeforeDate = (SELECT DATEADD(MONTH, @months, GETDATE())) " +

			"DECLARE case_cursor CURSOR FOR " +
			"select c.Id from [dbo].[Hearing] h JOIN [dbo].[HearingCase] hc on h.Id = hc.HearingId JOIN [dbo].[Case] c on hc.CaseId = c.Id " +
			"where h.[ScheduledDateTime] < @anonymiseBeforeDate and c.Name not like '%@email.net%' " +

			"OPEN case_cursor " +
			"FETCH NEXT FROM case_cursor " +
			"INTO @caseId " +

			"WHILE @@FETCH_STATUS = 0 " +
			"BEGIN " +
				"SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9); " +

				"UPDATE [dbo].[Case] SET [Name] = @randomString + '@email.net' WHERE Id = @caseId " +

				"FETCH NEXT FROM case_cursor " +
				"INTO @caseId " +
			"END " +
			"CLOSE case_cursor; " +
			"DEALLOCATE case_cursor; " +

			"DECLARE participant_cursor CURSOR FOR " +
			"select distinct p.Id from [dbo].[Hearing] h JOIN [dbo].[HearingCase] hc on h.Id = hc.HearingId JOIN [dbo].[Participant] p on p.HearingId = h.Id " +
			"where h.[ScheduledDateTime] < @anonymiseBeforeDate and p.DisplayName not like '%@email.net%' " +

			"OPEN participant_cursor " +
			"FETCH NEXT FROM participant_cursor " +
			"INTO @participantId " +

			"WHILE @@FETCH_STATUS = 0 " +
			"BEGIN " +
				"SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9); " +

				"UPDATE [dbo].[Participant] " +
				"SET [DisplayName] = @randomString + '@email.net', " +
					"[Representee] = CASE WHEN Representee = '' THEN '' WHEN Representee IS NULL THEN NULL ELSE @randomString END " +
				"WHERE Id = @participantId " +

				"FETCH NEXT FROM participant_cursor " +
				"INTO @participantId " +
			"END " +
			"CLOSE participant_cursor; " +
			"DEALLOCATE participant_cursor; " +

			"DECLARE participant_cursor CURSOR FOR " +
			"SELECT pr.Id, pr.OrganisationId " +
			"FROM [dbo].[Participant] p " +
			"JOIN [dbo].[Hearing] h ON p.HearingId = h.Id " +
			"JOIN [dbo].[Person] pr on pr.Id = p.PersonId " +
			"LEFT JOIN [dbo].[Organisation] o on o.Id = pr.OrganisationId " +
			"where h.[ScheduledDateTime] < @anonymiseBeforeDate " +
			"AND pr.Username NOT LIKE '%@email.net%' " +
			"AND pr.Username NOT LIKE '%JUDGE%' " +
			"AND pr.Username NOT LIKE '%TaylorHousecourt%' " +
			"AND pr.Username NOT LIKE '%ManchesterCFJCcourt%' " +
			"AND pr.Username NOT LIKE '%BirminghamCFJCcourt%' " +
			"AND pr.Username NOT LIKE '%ManchesterCFJCDDJretiringroom%' " +
			"AND pr.Username NOT LIKE '%ManchesterCFJCcourtGen%' " +
			"AND pr.Username NOT LIKE '%BirminghamCFJCcourtGen%' " +
			"AND pr.Username NOT LIKE '%BirminghamCJC.Judge%' " +
			"AND pr.Username NOT LIKE '%holdingroom%' " +
			"AND pr.Username NOT LIKE '%Property.Judge%' " +
			"AND pr.Username NOT LIKE '%TaylorHousecourt%' " +
			"AND pr.Username NOT LIKE '%TaylorHousecourtGen%' " +

			"AND pr.Username NOT LIKE '%Automation01%' " +
			"AND pr.Username NOT LIKE '%auto.%' " +
			"AND pr.Username NOT LIKE '%UserApiTestUser%' " +
			"AND pr.Username NOT LIKE '%Manual0%' " +
			"AND pr.Username NOT LIKE '%performance%' " +
            "AND pr.Username NOT LIKE 'kinly.clerk%' " +
			"AND pr.Username NOT LIKE 'Test%' " +
            "AND pr.Username NOT LIKE 'TP%' " +
            "AND pr.Username NOT LIKE 'Auto_%' " +
            "AND pr.Username NOT LIKE 'CACD%' " +
            "AND pr.Username NOT LIKE 'Employment%' " +
            "AND pr.Username NOT LIKE 'GRC%' " +
            "AND pr.Username NOT LIKE 'IAC%' " +
            "AND pr.Username NOT LIKE 'Judge%' " +
            "AND pr.Username NOT LIKE 'Property%' " +

			"and pr.Id not IN ( " +
				"SELECT pr.Id " +
				"FROM [dbo].[Participant] p  " +
				"JOIN [dbo].[Hearing] h ON p.HearingId = h.Id " +
				"JOIN [dbo].[Person] pr on pr.Id = p.PersonId " +
				"AND h.[ScheduledDateTime] > @anonymiseBeforeDate " +
			") " +

			"OPEN participant_cursor " +
			"FETCH NEXT FROM participant_cursor " +
			"INTO @personId, @organisationId " +

			"WHILE @@FETCH_STATUS = 0 " +
			"BEGIN " +
				"SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9); " +
				"UPDATE [dbo].[Person] " +
				"SET [FirstName] = @randomString, " +
					"[LastName] = @randomString, " +
					"[MiddleNames] = @randomString, " +
					"[Username] = @randomString + '@email.net', " +
					"[ContactEmail] = @randomString + '@hmcts.net', " +
					"[TelephoneNumber] = '00000000000' " +
				"WHERE Id = @personId " +

				"IF @organisationId IS NOT NULL " +
				"BEGIN " +
					"SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9); " +
					"UPDATE [dbo].[Organisation] " +
					"SET [Name] = CASE WHEN [Name] = '' THEN '' WHEN [Name] IS NULL THEN NULL ELSE @randomString END " +
					"WHERE ID = @organisationId " +
				"END " +

				"FETCH NEXT FROM participant_cursor " +
				"INTO @personId, @organisationId " +
			"END " +
			"CLOSE participant_cursor; " +
			"DEALLOCATE participant_cursor; ";
			command.RecordsUpdated = await _context.Database.ExecuteSqlRawAsync(query);

			var updatedDate = DateTime.UtcNow;

			var updateQuery = @$"IF EXISTS(select * from JobHistory)
								Update JobHistory Set LastRunDate= '{updatedDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
							ELSE
							  Insert into JobHistory values (NEWID(), '{updatedDate.ToString("yyyy-MM-dd HH:mm:ss.fff")}','true', 'AnonymiseHearings')";
			await _context.Database.ExecuteSqlRawAsync(updateQuery);
			command.UpdatedDate = updatedDate;
		}
    }
}
