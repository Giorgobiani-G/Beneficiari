using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Test.Models
{
    public class Beneficiari
    {
        private DateTime _birthDate;

        [Key]
        public int Benid { get; set; }


        public string Piradobisnomeri { get; set; }
        public string Saxeli { get; set; }
        public string Gvari { get; set; }

        public int Asaki { get; private set; }
        public string Misamarti { get; set; }

        public int Telefoni { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime  Tarigi { get; set; }
        public DateTime DabTarigi
        {
            get => _birthDate;
            set
            {
                _birthDate = value;
                Asaki = (new DateTime(1, 1, 1) + (DateTime.Now - DabTarigi)).Year-1;
            }
        }

        public void SetBirthDate(DateTime value)
        {
            _birthDate = value;
        }
    }
}
