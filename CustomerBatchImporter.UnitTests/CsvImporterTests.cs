using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CustomerBatchImporter.UnitTests
{
    public class CsvImporterTests
    {
        private readonly CsvImporter _csvImporter;
        private readonly ICustomerRepository _customerRepository;

        public CsvImporterTests()
        {
            _customerRepository = A.Fake<ICustomerRepository>();
            _csvImporter = new CsvImporter(_customerRepository);
        }
        [Fact]
        public async Task OneCustomer()
        {
            string email = "some@email.com";
            string name = "A Customer";
            string license = "Basic";
            string csvLine = string.Join(',', email, name, license);

            var stream = GetStreamFromString(csvLine);
            await _csvImporter.ReadAsync(stream);
        }

        [Fact]
        public async Task ValidCustomerOneLine()
        {
            // Arrange
            string email = "some@email.com";
            string name = "A Customer";
            string license = "Basic";
            string csvLine = string.Join(',', email, name, license);

            A.CallTo(() => _customerRepository.GetByEmailAsync(email)).Returns(default(Customer));

            //Act
            var stream = GetStreamFromString(csvLine);
            await _csvImporter.ReadAsync(stream);

            //Assert
            A.CallTo(() => _customerRepository.GetByEmailAsync(email)).MustHaveHappened();
            A.CallTo(() => _customerRepository.CreateAsync(A<NewCustomerDto>.That.Matches(n =>
            n.Email == email &&
            n.Name == name &&
            n.License == license))).MustHaveHappened();
        }

        [Fact]
        public async Task InvalidLine()
        {
            var stream = GetStreamFromString("not a valid line");
            await _csvImporter.ReadAsync(stream);

            var calls = Fake.GetCalls(_customerRepository);
            Assert.Empty(calls);
        }

        [Fact]
        public async Task ThreeLinesOneInvalid()
        {
            string email1 = "some@email.com";
            string email2 = "another@email.com";

            A.CallTo(() => _customerRepository.GetByEmailAsync(A<string>.Ignored)).Returns(default(Customer));
            string data = $"{email1},customer1,None\ninvalidline\n{email2},customer2,None";
            var stream = GetStreamFromString(data);
            await _csvImporter.ReadAsync(stream);
            A.CallTo(() => _customerRepository.CreateAsync(A<NewCustomerDto>.Ignored)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public async Task GetThrows()
        {
            A.CallTo(() => _customerRepository.GetByEmailAsync("")).Throws<ArgumentException>();

            var stream = GetStreamFromString(",name,license");
            await Assert.ThrowsAsync<ArgumentException>(() => _csvImporter.ReadAsync(stream));
        }

        [Fact]
        public async Task UpdateExisting()
        {
            // Arrange
            string email = "some@email.com";
            var existing = new Customer()
            {
                Id = 1,
                Email = email,
                Name = "A Customer",
                License = "Basic"
            };

            A.CallTo(() => _customerRepository.GetByEmailAsync(email)).Returns(existing);

            // Act
            var stream = GetStreamFromString($"{email},customer1,None");
            await _csvImporter.ReadAsync(stream);

            //Assert
            A.CallTo(() => _customerRepository.UpdateAsync(A<UpdateCustomerDto>.That.Matches(u=>
            u.Id == 1 &&
            u.NewName == "customer1" &&
            u.NewLicense == "None")))
            .MustHaveHappened();
        }

        private Stream GetStreamFromString(string csvLine)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(csvLine));
        }
    }
}
