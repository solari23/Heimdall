// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Heimdall.Models;

public class CollectionNotEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is null 
            || (value is ICollection valueCollection && valueCollection.Count == 0))
        {
            return new ValidationResult(
                $"{validationContext.DisplayName} cannot be empty.",
                new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}
