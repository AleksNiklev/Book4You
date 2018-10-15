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
    public class UpdateUserAddress_Should
    {
        [TestMethod]
        public void Change_User_Address_IfUser_Exists()
        {
            var contextOptions = new DbContextOptionsBuilder<LibrarySystemContext>()
                .UseInMemoryDatabase(databaseName: "Change_User_Address")
                .Options;

            string firstName = "Ivan",
                middleName = "Ivanov",
                lastName = "Ivanov",
                phoneNumber = "1234567899";
            DateTime addOnDate = DateTime.Now;
            bool isDeleted = false;

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
                var validationMock = new Mock<CommonValidations>();

                var townService = new TownService(unit, new CommonValidations());
                var addressService = new AddressService(unit, new CommonValidations());
                var userService = new UsersServices(unit, new CommonValidations());

                var town = townService.AddTown("NewTown");
                var newAddress = addressService.AddAddress("NewAddress", town);

                userService.UpdateUserAddress(firstName, middleName, lastName, newAddress);

                Assert.AreEqual(newAddress, assertContext.Users.First().AddressId);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(UserNullableException))]
        public void Throw_Exeption_IfUser_IsDeleted()
        {
            var contextOptions = new DbContextOptionsBuilder<LibrarySystemContext>()
                .UseInMemoryDatabase(databaseName: "Change_User_Address")
                .Options;
            string firstName = "Ivan1",
                middleName = "Ivanov1",
                lastName = "Ivanov1",
                phoneNumber = "1234567899";
            DateTime addOnDate = DateTime.Now;
            bool isDeleted = false;

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
                userService.RemoveUser(firstName, middleName, lastName);
                userService.UpdateUserAddress(firstName, middleName, lastName, 3);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(UserNullableException))]
        public void Throw_Exeption_IfUser_DoesNot_Exist()
        {
            var contextOptions = new DbContextOptionsBuilder<LibrarySystemContext>()
                .UseInMemoryDatabase(databaseName: "Change_User_Address").Options;

            using (var actContext = new LibrarySystemContext(contextOptions))
            {
                var unit = new UnitOfWork(actContext);
                var validationMock = new Mock<CommonValidations>();

                var userService = new UsersServices(unit, new CommonValidations());
                userService.UpdateUserAddress("hjasmnzx", "iqdkwjsan", "kjasn",5);
            }
        }
    }
}
