using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepairRequests;

namespace UnitTestProject
{
    [TestClass]
    public class RequestValidatorTests
    {
        [TestMethod]
        public void ValidateResponsiblePerson_Empty_ReturnsError()
        {
            // Act
            var result = RequestValidator.ValidateResponsiblePerson("");

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Поле 'Ответственный' обязательно для заполнения!", result.ErrorMessage);
        }      

        [TestMethod]
        public void ValidateResponsiblePerson_WithDigits_ReturnsError()
        {
            // Act
            var result = RequestValidator.ValidateResponsiblePerson("Иван123");

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("В поле 'Ответственный' нельзя вводить цифры!", result.ErrorMessage);
        }        

        [TestMethod]
        public void ValidateResponsiblePerson_Correct_ReturnsSuccess()
        {
            // Act
            var result = RequestValidator.ValidateResponsiblePerson("Иван Иванов");

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateResponsiblePerson_WithSpecialCharacters_ReturnsSuccess()
        {
            // Act
            var result = RequestValidator.ValidateResponsiblePerson("Иван-Иванов");

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateDeadlineForInProgress_PastDate_ReturnsError()
        {
            // Arrange
            var pastDate = DateTime.Now.AddDays(-1);
            var status = "В процессе";

            // Act
            var result = RequestValidator.ValidateDeadlineForInProgress(pastDate, status);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Нельзя установить статус 'В процессе' при просроченной дате!", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateDeadlineForInProgress_FutureDate_ReturnsSuccess()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(1);
            var status = "В процессе";

            // Act
            var result = RequestValidator.ValidateDeadlineForInProgress(futureDate, status);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateDeadlineForInProgress_TodayDate_ReturnsSuccess()
        {
            // Arrange
            var today = DateTime.Now;
            var status = "В процессе";

            // Act
            var result = RequestValidator.ValidateDeadlineForInProgress(today, status);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateStatusChange_FromCompletedToOther_ReturnsError()
        {
            // Arrange
            var currentStatus = "Выполнено";
            var newStatus = "В процессе";

            // Act
            var result = RequestValidator.ValidateStatusChange(currentStatus, newStatus);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Нельзя изменить статус 'Выполнено' на другой!", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateStatusChange_FromNotCompletedToOther_ReturnsError()
        {
            // Arrange
            var currentStatus = "Не выполнено";
            var newStatus = "В процессе";

            // Act
            var result = RequestValidator.ValidateStatusChange(currentStatus, newStatus);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Нельзя изменить статус 'Не выполнено' на другой!", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateStatusChange_FromCompletedToNotCompleted_ReturnsError()
        {
            // Arrange
            var currentStatus = "Выполнено";
            var newStatus = "Не выполнено";

            // Act
            var result = RequestValidator.ValidateStatusChange(currentStatus, newStatus);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Нельзя изменить статус 'Выполнено' на другой!", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateStatusChange_FromCompletedToCompleted_ReturnsSuccess()
        {
            // Arrange
            var currentStatus = "Выполнено";
            var newStatus = "Выполнено";

            // Act
            var result = RequestValidator.ValidateStatusChange(currentStatus, newStatus);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateStatusChange_FromNotCompletedToNotCompleted_ReturnsSuccess()
        {
            // Arrange
            var currentStatus = "Не выполнено";
            var newStatus = "Не выполнено";

            // Act
            var result = RequestValidator.ValidateStatusChange(currentStatus, newStatus);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }


        [TestMethod]
        public void ValidateStatusChange_FromInProgressToCompleted_ReturnsSuccess()
        {
            // Arrange
            var currentStatus = "В процессе";
            var newStatus = "Выполнено";

            // Act
            var result = RequestValidator.ValidateStatusChange(currentStatus, newStatus);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateDeadlineForInProgress_MinDate_ReturnsError()
        {
            // Arrange
            var minDate = DateTime.MinValue;
            var status = "В процессе";

            // Act
            var result = RequestValidator.ValidateDeadlineForInProgress(minDate, status);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Нельзя установить статус 'В процессе' при просроченной дате!", result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateResponsiblePerson_SingleWord_ReturnsSuccess()
        {
            // Act
            var result = RequestValidator.ValidateResponsiblePerson("Иван");

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateResponsiblePerson_MultipleWords_ReturnsSuccess()
        {
            // Act
            var result = RequestValidator.ValidateResponsiblePerson("Иван Иванович Иванов");

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        [TestMethod]
        public void ValidateDeadlineForInProgress_ExactlyMidnightToday_ReturnsSuccess()
        {
            // Arrange
            var midnightToday = DateTime.Today;
            var status = "В процессе";

            // Act
            var result = RequestValidator.ValidateDeadlineForInProgress(midnightToday, status);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }
    }
}
