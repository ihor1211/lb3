using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Web.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace ShoppingCart.Tests
{

    public class CategoryControllerTests
    {
        [Fact]
        public void GetCategories_All_ReturnsAllCategories()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };
            mockCategoryRepository.Setup(repo => repo.GetAll(It.IsAny<string>())).Returns(categoryList);


            mockUnitOfWork.Setup(uow => uow.Category).Returns(mockCategoryRepository.Object);

            var controller = new CategoryController(mockUnitOfWork.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.Equal(categoryList, result.Categories);
        }



        [Fact]
        public void CreateUpdate_ValidModel_AddsOrUpdateCategory()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var category = new Category { Id = 1, Name = "Category1" };
            var categoryVM = new CategoryVM { Category = category };
            mockCategoryRepository.Setup(repo => repo.Update(It.IsAny<Category>()));
            mockCategoryRepository.Setup(repo => repo.Add(It.IsAny<Category>()));
            mockUnitOfWork.Setup(uow => uow.Category).Returns(mockCategoryRepository.Object);

            var controller = new CategoryController(mockUnitOfWork.Object);

            // Act
            controller.CreateUpdate(categoryVM);

            // Assert
            if (category.Id == 0)
            {
                mockCategoryRepository.Verify(repo => repo.Add(It.IsAny<Category>()), Times.Once);
            }
            else
            {
                mockCategoryRepository.Verify(repo => repo.Update(It.IsAny<Category>()), Times.Once);
            }
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void CreateUpdate_InvalidModel_ThrowsException()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var controller = new CategoryController(mockUnitOfWork.Object);
            var categoryVM = new CategoryVM { Category = new Category() };
            controller.ModelState.AddModelError("Name", "Required");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => controller.CreateUpdate(categoryVM));
            Assert.Equal("Model is invalid", exception.Message);
        }



    }
}