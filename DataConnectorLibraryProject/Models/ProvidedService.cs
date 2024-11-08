﻿using DataConnectorLibraryProject.Interface;

namespace DataConnectorLibraryProject.Models
{
    internal class ProvidedService : IEntity
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
        public Guid PerformerId { get; set; }
        public Performer Performer { get; set; }
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime ExecutionDate { get; set; }
    }
}
