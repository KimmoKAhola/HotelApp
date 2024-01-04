using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary.HotelDatabase.DatabaseConfiguration;
using DatabaseLibrary.Interfaces;

namespace DatabaseLibrary.HotelDatabase.Models
{
    public class UserInbox : IEntity
    {
        public int Id { get; set; }
        public List<SystemMessage> Messages { get; set; } = new List<SystemMessage>();
    }
}
