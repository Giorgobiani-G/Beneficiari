using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Test.Controllers;

namespace Test.Models
{
    public class Visit
    {
        [Key]
        public int Vsid { get; set; }
        public string VistisTipi { get; set; }
        public DateTime TarigiDro { get; set; }
        public string Piradoba { get; set; }
        public string Saxeli { get; set; }
        public string Gvari { get; set; }
        public string Symptomi { get; set; }
        private string? _currentuser;
        public string Currentuser
        {
            get => _currentuser;
            set => _currentuser =value;
        }
        public string Mdgomareoba { get; set; }
    }   
}

