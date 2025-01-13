using Bogus;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryPersons
{
    public class BulkJudiciaryPersonsTests : ApiTest
    {
        private static readonly Faker Faker = new();
        
        [Test]
        public async Task Should_insert_judiciary_person()
        {
            // Arrange
            var request = new List<JudiciaryPersonRequest>
            {
                new()
                {
                    Id = "ExternalRefId",
                    PersonalCode = Guid.NewGuid().ToString(),
                    Title = "Title",
                    KnownAs = "KnownAs",
                    Fullname = "Fullname",
                    Surname = "Surname",
                    PostNominals = "PostNominals",
                    Email = $"automation_{Faker.Random.Number(0, 9999999)}@email.com",
                    WorkPhone = "WorkPhone"
                }
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryPersonsEndpoints.BulkJudiciaryPersons(), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<BulkJudiciaryPersonResponse>(result.Content);
            response.ErroredRequests.Should().BeEmpty();
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);

            var newPerson = await db.JudiciaryPersons.SingleOrDefaultAsync(jp => jp.PersonalCode == request[0].PersonalCode);
            newPerson.Should().NotBeNull();
            newPerson.Title.Should().Be(request[0].Title);
            newPerson.KnownAs.Should().Be(request[0].KnownAs);
            newPerson.Fullname.Should().Be(request[0].Fullname);
            newPerson.Surname.Should().Be(request[0].Surname);
            newPerson.PostNominals.Should().Be(request[0].PostNominals);
            newPerson.Email.Should().Be(request[0].Email);
            newPerson.WorkPhone.Should().Be(request[0].WorkPhone);
        }
        
        [Test]
        public async Task Should_update_judiciary_persons()
        {
            // Arrange
            var person1 = await Hooks.AddJudiciaryPerson(Guid.NewGuid().ToString());
            var person2 = await Hooks.AddJudiciaryPerson(Guid.NewGuid().ToString());
            var person3 = await Hooks.AddJudiciaryPerson(Guid.NewGuid().ToString());
            var request = new List<JudiciaryPersonRequest>
            {
                new()
                {
                    PersonalCode = person1.PersonalCode,
                    Id = null,
                    Deleted = true,
                    DeletedOn = "2023-01-01"
                },
                new()
                {
                    PersonalCode = person2.PersonalCode,
                    Id = null,
                    Leaver = true,
                    HasLeft = true,
                    LeftOn = "2023-01-01"
                },
                new()
                {
                    Id = person3.ExternalRefId,
                    PersonalCode = person3.PersonalCode,
                    Title = person3.Title + "_Edited",
                    KnownAs = person3.KnownAs + "_Edited",
                    Fullname = person3.Fullname + "_Edited",
                    Surname = person3.Surname + "_Edited",
                    PostNominals = person3.PostNominals + "_Edited",
                    Email = person3.Email + "_Edited",
                    WorkPhone = person3.WorkPhone + "_Edited"
                }
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryPersonsEndpoints.BulkJudiciaryPersons(), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<BulkJudiciaryPersonResponse>(result.Content);
            response.ErroredRequests.Should().BeEmpty();
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var updatedPersons = await db.JudiciaryPersons
                .Where(jp => jp.PersonalCode == person1.PersonalCode || 
                             jp.PersonalCode == person2.PersonalCode ||
                             jp.PersonalCode == person3.PersonalCode)
                .ToListAsync();

            var deletedPerson = updatedPersons.SingleOrDefault(p => p.PersonalCode == person1.PersonalCode);
            deletedPerson.Should().NotBeNull();
            deletedPerson.Deleted.Should().BeTrue();
            deletedPerson.DeletedOn.Should().Be(request[0].DeletedOn);
            
            var leaverPerson = updatedPersons.SingleOrDefault(p => p.PersonalCode == person2.PersonalCode);
            leaverPerson.Should().NotBeNull();
            leaverPerson.Leaver.Should().BeTrue();
            leaverPerson.HasLeft.Should().BeTrue();
            leaverPerson.LeftOn.Should().Be(request[1].LeftOn);

            var newPerson = updatedPersons.SingleOrDefault(p => p.PersonalCode == person3.PersonalCode);
            newPerson.Should().NotBeNull();
            newPerson.Title.Should().Be(request[2].Title);
            newPerson.KnownAs.Should().Be(request[2].KnownAs);
            newPerson.Fullname.Should().Be(request[2].Fullname);
            newPerson.Surname.Should().Be(request[2].Surname);
            newPerson.PostNominals.Should().Be(request[2].PostNominals);
            newPerson.Email.Should().Be(request[2].Email);
            newPerson.WorkPhone.Should().Be(request[2].WorkPhone);
        }

        [Test]
        public async Task Should_not_insert_inactive_judiciary_persons()
        {
            // Arrange
            var deletedPerson = new
            {
                PersonalCode = Guid.NewGuid().ToString()
            };
            var leaverPerson = new
            {
                PersonalCode = Guid.NewGuid().ToString()
            };
            var request = new List<JudiciaryPersonRequest>
            {
                new()
                {
                    PersonalCode = deletedPerson.PersonalCode,
                    Id = null,
                    Deleted = true,
                    DeletedOn = "2023-01-01"
                },
                new()
                {
                    PersonalCode = leaverPerson.PersonalCode,
                    Id = null,
                    Leaver = true,
                    HasLeft = true,
                    LeftOn = "2023-01-01"
                }
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryPersonsEndpoints.BulkJudiciaryPersons(), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<BulkJudiciaryPersonResponse>(result.Content);
            response.ErroredRequests.Should().BeEmpty();
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var updatedPersons = await db.JudiciaryPersons
                .Where(jp => jp.PersonalCode == deletedPerson.PersonalCode || 
                             jp.PersonalCode == leaverPerson.PersonalCode)
                .ToListAsync();

            updatedPersons.Should().BeEmpty();
        }
    }
}
