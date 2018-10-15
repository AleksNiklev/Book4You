﻿using LibrarySystem.Data.Context;
using LibrarySystem.Services;
using LibrarySystem.Services.Exceptions.UserServices;
using LibrarySystem.Services.Services;
using LibrarySystem.Services.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace LibrarySystem.Tests.Services.UserServiceTests
{
    [TestClass]
    public class ListUsers_Should
    {
        [TestMethod]
        public void List_Users_IfUsers_Exist()
        {
            var contextOptions = new DbContextOptionsBuilder<LibrarySystemContext>()
                .UseInMemoryDatabase(databaseName: "List_Users")
                .Options;

            string firstName = "Ivan",
                middleName = "Ivanov",
                lastName = "Ivanov",
                phoneNumber = "1234567899";
            DateTime addOnDate = DateTime.Now;
            bool isDeleted = false;

            string fullName = firstName + " " + middleName + " " + lastName;

            using (var actContext = new LibrarySystemContext(contextOptions))
            {
                var unit = new UnitOfWork(actContext);
                var validationMock = new Mock<CommonValidations>();

                var townService = new TownService(unit, new CommonValidations());
                var addressService = new AddressService(unit, new CommonValidations());
                var userService = new UsersServices(unit, new CommonValidations());

                var town = townService.AddTown("test");
                var address = addressService.AddAddress("test address", town);

                userService.AddUser(firstName, middleName, lastName, phoneNumber, addOnDate, isDeleted, address);

            }
            // Assert
            using (var assertContext = new LibrarySystemContext(contextOptions))
            {
                var unit = new UnitOfWork(assertContext);
                var userService = new UsersServices(unit, new CommonValidations());

                var count=userService.ListUsers(false).Count();

                Assert.AreEqual(1, count);
                Assert.AreEqual(fullName, userService.ListUsers(false).First().FullName);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(UserNullableException))]
        public void Throw_If_NoUsers()
        {
            var contextOptions = new DbContextOptionsBuilder<LibrarySystemContext>()
                .UseInMemoryDatabase(databaseName: "Throw_IfNoUsers").Options;
            
            // Act
            using (var actContext = new LibrarySystemContext(contextOptions))
            {
                var unit = new UnitOfWork(actContext);
                var validationMock = new Mock<CommonValidations>();

                var service = new UsersServices(unit, validationMock.Object);

                service.ListUsers(true);
            }
        }
    }
}
