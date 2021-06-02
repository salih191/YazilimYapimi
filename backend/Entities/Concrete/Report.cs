using System;
using Core.Entities;

namespace Entities.Concrete
{
    public class Report:IEntity
    {
        public string Tarih { get; set; }
        public string UrunTipi { get; set; }
        public decimal AlimTutari { get; set; }
        public decimal Miktar { get; set; }
    }
}