using Boticario.Api.Controllers;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Xunit;

namespace Boticario.Api.Tests
{       
    public class UnitTest1
    {   
        [Fact]
        public async void AuthTest()
        {
            var userTest = new UserInfo
            {
                Email = "teste@email.com",
                Password = "VIrLp6nhE4xrYEhFwkBevQ=="
            };

            var mockLoginRepo = new Mock<ILoginRepository>();
            mockLoginRepo.Setup(repo => repo.Auth(userTest))
                .ReturnsAsync(GetTestAuth());
            var mockUserRepo = new Mock<IUserRepository>();

            var controller = new UserController(mockUserRepo.Object, mockLoginRepo.Object);

            var result = await controller.Login(userTest);

            var actionResult = Assert.IsAssignableFrom<ActionResult<UserTokenModel>>(result);
            var model = Assert.IsAssignableFrom<ActionResult<UserTokenModel>>(actionResult);
            
            Assert.True(model.Value.Authenticated);
            Assert.NotNull(model.Value.AccessToken);
        }

        private UserTokenModel GetTestAuth()
        {
            return new UserTokenModel
            {
                Authenticated = true                
            };            
        }       
    }
}
