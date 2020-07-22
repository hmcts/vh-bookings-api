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
				"@addressId AS INT, " +
				"@organisationId as INT, " +
				"@caseId AS INT " +
			"SET @months = -3 " +
			"SET @anonymiseBeforeDate = (SELECT DATEADD(MONTH, @months, GETDATE())) " +

			"DECLARE case_cursor CURSOR FOR " +
			"select c.Id, p.Id from [dbo].[Hearing] h JOIN [dbo].[HearingCase] hc on h.Id = hc.HearingId JOIN [dbo].[Case] c on hc.CaseId = c.Id JOIN [dbo].[Participant] p on p.HearingId = h.Id " +
			"where h.[ScheduledDateTime] < @anonymiseBeforeDate " +

			"OPEN case_cursor " +
			"FETCH NEXT FROM case_cursor " +
			"INTO @caseId, @participantId " +

			"WHILE @@FETCH_STATUS = 0 " +
			"BEGIN " +
				"SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9); " +

				"UPDATE [dbo].[Case] SET [Name] = @randomString WHERE Id = @caseId " +

				"UPDATE [dbo].[Participant] " +
				"SET [DisplayName] = @randomString, " +
					"[Representee] = CASE WHEN Representee = '' THEN '' WHEN Representee IS NULL THEN NULL ELSE @randomString END " +
				"WHERE Id = @participantId " +

				"FETCH NEXT FROM case_cursor " +
				"INTO @caseId, @participantId " +
			"END " +
			"CLOSE case_cursor; " +
			"DEALLOCATE case_cursor; " +

			"DECLARE participant_cursor CURSOR FOR " +
			"SELECT pr.Id, pr.AddressId, pr.OrganisationId " +
			"FROM [dbo].[Participant] p " +
			"JOIN [dbo].[Hearing] h ON p.HearingId = h.Id " +
			"JOIN [dbo].[Person] pr on pr.Id = p.PersonId " +
			"LEFT JOIN [dbo].[Address] a on a.Id = pr.AddressId " +
			"LEFT JOIN [dbo].[Organisation] o on o.Id = pr.OrganisationId " +
			"where h.[ScheduledDateTime] < @anonymiseBeforeDate " +
			"and pr.Id not IN ( " +
				"SELECT pr.Id " +
				"FROM [dbo].[Participant] p  " +
				"JOIN [dbo].[Hearing] h ON p.HearingId = h.Id " +
				"JOIN [dbo].[Person] pr on pr.Id = p.PersonId " +
				"AND h.[ScheduledDateTime] > @anonymiseBeforeDate " +
			") " +

			"OPEN participant_cursor " +
			"FETCH NEXT FROM participant_cursor " +
			"INTO @personId, @addressId, @organisationId " +

			"WHILE @@FETCH_STATUS = 0 " +
			"BEGIN " +
				"SELECT @randomString = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9); " +
				"UPDATE [dbo].[Person] " +
				"SET [FirstName] = @randomString, " +
					"[LastName] = @randomString, " +
					"[MiddleNames] = @randomString, " +
					"[Username] = @randomString + '@hearings.reform.hmcts.net', " +
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
				"INTO @personId, @addressId, @organisationId " +
			"END " +
			"CLOSE participant_cursor; " +
			"DEALLOCATE participant_cursor; ";
			command.RecordsUpdated = await _context.Database.ExecuteSqlRawAsync(query);
        }
    }
}
