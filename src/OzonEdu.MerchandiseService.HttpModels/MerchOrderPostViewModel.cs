﻿using System.ComponentModel.DataAnnotations;

namespace OzonEdu.MerchandiseService.HttpModels
{
    public class MerchOrderPostViewModel
    {
        [Required]
        public int EmployeeId { get; init; }
        [Required]
        public string EmployeeLastName { get; init; }
        [Required]
        public string EmployeeFirstName { get; init; }
        public string EmployeeMiddleName { get; init; }
        [Required]
        public string EmployeeEmail { get; init; }
        [Required]
        public long Sku { get; init; }
        public string SkuDescription { get; init; }
        [Required]
        public int Quantity { get; init; }
        [Required]
        public int ManagerId { get; init; }
        [Required]
        public string ManagerLastName { get; init; }
        [Required]
        public string ManagerFirstName { get; init; }
        public string ManagerMiddleName { get; init; }
        [Required]
        public string ManagerEmail { get; init; }
    }
}