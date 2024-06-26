﻿using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAccteptedYearOfExperince = 15;
        private readonly IIdentityValidator identityValidator;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };

        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            this.identityValidator = identityValidator;
        }
        public ApplicationResult Evaluate(JobApplication form)
        {
            if (form.Applicant.Age < minAge)
            {
                return ApplicationResult.AutoRejected;
            }

            form.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;

            if (identityValidator.Country != "TURKEY")
            {
                return ApplicationResult.TransferredToCTO;
            }

            var validIdentity = identityValidator.IsValid(form.Applicant.IdentityNumber);

            if (!validIdentity)
            {
                return ApplicationResult.TransferredToHr;
            }

            var sr = GetTechStackSimilarity(form.TechStackList);
            if (sr < 25)
            {
                return ApplicationResult.AutoRejected;
            }

            if (sr > 75 && form.YearsOfExperience > autoAccteptedYearOfExperince)
            {
                return ApplicationResult.AutoAccepted;
            }

            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarity(List<string> techStacks)
        {
            var matchedCount =
                techStacks
                .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                .Count();

            return (int)((double)matchedCount / techStackList.Count) * 100;
        }
    }

    public enum ApplicationResult
    {
        AutoRejected,
        TransferredToHr,
        TransferredToLead,
        TransferredToCTO,
        AutoAccepted
    }
}
