using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    internal class Equipment : IEntity
    {
        public Guid Id { get; set; }
        public string EquipmentName { get; set; }
        public double PriceEquipment { get; set; }
        public string PriceEquipmentWithPdv { get; set; }
        public string IMEIPT { get; set; }
        public string IMEIDRP { get; set; }
        public string ModelDRP { get; set; }
        public string ModelRT { get; set; }
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

    }
}
