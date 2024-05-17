using Microsoft.EntityFrameworkCore;

namespace MSCore.EntityFramework
{
    public partial class DBContext : BaseDBContext
    {
        /// <summary>
        /// Key
        /// </summary>
        public override string ConnectionKey { get; set; } = "App.Db.Project";
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {

        }
    }
}
