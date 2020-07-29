using Bookings.DAL.Commands.Core;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bookings.DAL.Commands
{
    public class AnonymiseHearingsCommand : ICommand
    {
        public int RecordsUpdated { get; set; }
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
			"AND pr.Username NOT LIKE '%atif.%' " +
			"AND pr.Username NOT LIKE '%y''test.%' " +
			"AND pr.Username NOT LIKE 'ferdinand.porsche%' " +
			"AND pr.Username NOT LIKE 'enzo.ferrari%' " +
			"AND pr.Username NOT LIKE 'mike.tyson%' " +
			"AND pr.Username NOT LIKE 'george.foreman%' " +
			"AND pr.Username NOT LIKE 'rocky.marciano%' " +
			"AND pr.Username NOT LIKE 'cassius.clay%' " +
			"AND pr.Username NOT LIKE 'george.clinton%' " +
			"AND pr.Username NOT LIKE 'metalface.doom%' " +
			"AND pr.Username NOT LIKE 'karl.benz%' " +
			"AND pr.Username NOT LIKE 'henry.ford%' " +
			"AND pr.Username NOT LIKE 'feuer.frei%' " +
			"AND pr.Username NOT LIKE 'wasser.kalt%' " +
			"AND pr.Username NOT LIKE 'dan.brown%' " +
			"AND pr.Username NOT LIKE 'tom.clancy%' " +
			"AND pr.Username NOT LIKE 'stephen.king%' " +
			"AND pr.Username NOT LIKE 'Manual01VideoHearingsOfficer01%' " +
			"AND pr.Username NOT LIKE 'sue.burke%' " +
			"AND pr.Username NOT LIKE 'yeliz.admin%' " +
			"AND pr.Username NOT LIKE 'yeliz.judge%' " +
			"AND pr.Username NOT LIKE 'yeliz.judge2%' " +
			"AND pr.Username NOT LIKE 'one.three%' " +
			"AND pr.Username NOT LIKE 'one.four%' " +
			"AND pr.Username NOT LIKE 'michael.jordan%' " +
			"AND pr.Username NOT LIKE 'scottie.pippen%' " +
			"AND pr.Username NOT LIKE 'steve.kerr%' " +
			"AND pr.Username NOT LIKE 'dennis.rodman%' " +
			"AND pr.Username NOT LIKE 'john.doe%' " +
			"AND pr.Username NOT LIKE 'jane.doe%' " +
			"AND pr.Username NOT LIKE 'chris.green%' " +
			"AND pr.Username NOT LIKE 'james.green%' " +

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
					"[ContactEmail] = @randomString + '@email.com', " +
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
        }
    }
}
