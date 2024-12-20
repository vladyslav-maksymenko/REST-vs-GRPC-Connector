﻿using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models.ServerSideModels
{
    public class Equipment : IEntity
    {
        public string Id { get; set; } 
        public string EquipmentName { get; set; }
        public double PriceEquipment { get; set; }
        public string PriceEquipmentWithPdv { get; set; }
        public string IMEIPT { get; set; }
        public string IMEIDRP { get; set; }
        public string ModelDRP { get; set; }
        public string ModelRT { get; set; }
        public string VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

    }
}
