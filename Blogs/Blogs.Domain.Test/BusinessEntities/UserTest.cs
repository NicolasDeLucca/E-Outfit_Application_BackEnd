using Blogs.Domain.BusinessEntities;
using Blogs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blogs.Domain.Test.BusinessEntities
{
    [TestClass]
    public class UserTest
    {
        private User _validUser;
        private const int userNameMaxSize = 12;

        [TestInitialize]
        public void SetUp()
        {
            _validUser = new User
            {
                Id = 1,
                Name = "Pedro",
                LastName = "Gonzalez",
                Email = "pgonzalez@gmail.com",
                Password = "pgon_1234",
                UserName = "PGonza",
                Role = UserRole.Blogger
            };
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithNoNameFailsValidation()
        {
            _validUser.Name = null;
            _validUser.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithNoLastNameFailsValidation()
        {
            _validUser.LastName = null;
            _validUser.ValidOrFail();
        }
            
        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithSpacedEmailFailsValidation()
        {
            _validUser.Email = RandomStringInsertion(_validUser.Email, " ");
            _validUser.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithEmailWithoutDotFailsValidation()
        {
            _validUser.Email = SubStringDeletion(_validUser.Email, ".");
            _validUser.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithEmailWithoutAsperandFailsValidation()
        {
            _validUser.Email = SubStringDeletion(_validUser.Email, "@");
            _validUser.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithNoPasswordFailsValidation()
        {
            _validUser.Password = null;
            _validUser.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithExceededUserNameLengthFailsValidation()
        {
           _validUser.UserName = RandomAlphanumeric(userNameMaxSize + 1);
           _validUser.ValidOrFail();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRequestDataException))]
        public void UserWithSpacedUserNameFailsValidation()
        {
            _validUser.UserName = RandomStringInsertion(_validUser.UserName, " ");
            _validUser.ValidOrFail();
        }

        [TestMethod]
        public void ValidUserPassesValidation()
        {
            _validUser.ValidOrFail();
        }

        #region String Helpers

        private static string RandomStringInsertion(string input, string str)
        {
            int pos = new Random().Next(input.Length);
            return input.Insert(pos, str);
        }

        private static string SubStringDeletion(string input, string str)
        {
            return string.Concat(input.Split(str));
        }

        private static string RandomAlphanumeric(int size) {

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Func<string, char> random = s => s[new Random().Next(s.Length)];
            return new string(Enumerable.Repeat(chars, size).Select(random).ToArray());
        }

        #endregion
    }
}