using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary.Interfaces;

namespace DatabaseLibrary.HotelDatabase.Models
{
    public enum UserRoles
    {
        System,
        Admin,
        Guest,
    }
    public class UserRole : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string RoleName { get; set; } = null!;
    }
}
