using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Moq;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        // UnitOfWork_Condition_ExpectedResult
        // UnitOfWork_ExpectedResult_Condition

        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);

        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age=19},
                TechStackList = new System.Collections.Generic.List<string>() { "" }
                
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithFullTechStack_TransferredToAutoAccepted()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                { Age=25},
                TechStackList = new System.Collections.Generic.List<string>() { "C#", "RabbitMQ", "Microservice", "Visual Studio" },
                YearsOfExperience = 16

            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoAccepted);
        }

        [Test]
        public void Application_WithInValidIdentity_TransferredToHr()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                { Age = 25 }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.TransferredToHr);
        }


    }
}