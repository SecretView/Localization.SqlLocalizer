// Copyright (c) SecretCollect B.V. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace SimpleMvc.Models
{
    public enum Months
    {
        [Display(Name = "JANUARY")]
        January = 1,
        [Display(Name = "FEBRUARY")]
        February = 2,
        [Display(Name = "MARCH")]
        March = 3,
        [Display(Name = "APRIL")]
        April = 4,
        [Display(Name = "MAY")]
        May = 5,
        [Display(Name = "JUNE")]
        June = 6,
        [Display(Name = "JULY")]
        July = 7,
        [Display(Name = "AUGUST")]
        August = 8,
        [Display(Name = "SEPTEMBER")]
        September = 9,
        [Display(Name = "OCTOBER")]
        October = 10,
        [Display(Name = "NOVEMBER")]
        November = 11,
        [Display(Name = "DECEMBER")]
        December = 12
    }
}
