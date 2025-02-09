using System.Runtime.Serialization;

namespace DataConnectorLibraryProject.Enums
{
    public enum DbContextType
    {
        [EnumMember(Value = "sql")]
        Sql = 0,
        [EnumMember(Value = "mongo")]
        Mongo = 1
    }
}