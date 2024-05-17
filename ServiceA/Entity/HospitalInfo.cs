using Microsoft.EntityFrameworkCore;
using MSCore.EntityFramework.Entity;
using ServiceA.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSCore.EntityFramework
{
    public partial class DBContext
    {
        public DbSet<HospitalInfo> hospitals { get; set; }
    }
}

namespace ServiceA.Entity
{
    [Table("hospital")]
    public class HospitalInfo : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }

        public string? TelNo { get; set; }

        //测试如果不存在updateTime字段，设置为notmapped
        //[NotMapped]
        //public override DateTime UpdateTime {  get; set; }
    }
}
