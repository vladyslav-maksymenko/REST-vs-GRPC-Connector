using System.Runtime.Serialization;

namespace WebApiProject.ExtendSwager
{
    public enum DbType
    {
        [EnumMember(Value = "sql")]
        Sql = 0,
        [EnumMember(Value = "mongo")]
        Mongo = 1
    }
}
