using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccessLibrary.Models
{
    public enum Status
    {
        ONGOING,
        UPCOMING,
        CANCELLED,
        COMPLETED
    };

    public enum ProtestType
    {
        STATIONARY,
        WALK
    };

    public class Protest
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public string Description { get; private set; }
        public Status Status { get; private set; }
        public bool IsStationary { get; private set; }
        public ProtestType Type { get; private set; }
        private readonly List<Location> _locations = new List<Location>();
        public IReadOnlyList<Location> Locations => _locations.AsReadOnly();

        public Protest(int id, string name, DateTime startTime, DateTime endTime, string description,
                       Status status, bool isStationary, ProtestType type)
        {
            Id = id;
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
            Description = description;
            Status = status;
            IsStationary = isStationary;
            Type = type;
        }

        public void AddLocation(Location location)
        {
            if (!_locations.Contains(location))
                _locations.Add(location);
        }

        public void RemoveLocation(Location location)
        {
            _locations.Remove(location);
        }
    }
}
