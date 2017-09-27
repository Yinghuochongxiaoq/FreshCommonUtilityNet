using System;
using System.Collections.Generic;
using FreshCommonUtility.Dapper;

namespace FreshCommonUtilityNetTest.Dapper
{
    /// <summary>
    /// users Tabel
    /// </summary>
    [Table("Users")]
    public class UserEditableSettings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    //For .Net 4.5> [System.ComponentModel.DataAnnotations.Schema.Table("Users")]  or the attribute built into SimpleCRUD
    [Table("Users")]
    public class User : UserEditableSettings
    {
        //we modified so enums were automatically handled, we should also automatically handle nullable enums
        public DayOfWeek? ScheduledDayOff { get; set; }

        [ReadOnly(true)]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public int NotMappedInt { get; set; }
    }

    [Table("Users")]
    public class User1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int? ScheduledDayOff { get; set; }
    }

    public class Car
    {
        #region DatabaseFields
        //System.ComponentModel.DataAnnotations.Key
        [Key]
        public int CarId { get; set; }
        public int? Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        #endregion

        #region RelatedTables
        public List<User> Users { get; set; }
        #endregion

        #region AdditionalFields
        [Editable(false)]
        public string MakeWithModel { get { return Make + " (" + Model + ")"; } }
        #endregion

    }

    public class BigCar
    {
        #region DatabaseFields
        //System.ComponentModel.DataAnnotations.Key
        [Key]
        public long CarId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        #endregion

    }

    [Table("CarLog", Schema = "Log")]
    public class CarLog
    {
        public int Id { get; set; }
        public string LogNotes { get; set; }
    }

    /// <summary>
    /// This class should be used for failing tests, since no schema is specified and 'CarLog' is not on dbo
    /// </summary>
    [Table("CarLog")]
    public class SchemalessCarLog
    {
        public int Id { get; set; }
        public string LogNotes { get; set; }
    }

    public class City
    {
        [Key]
        public string Name { get; set; }
        public int Population { get; set; }
    }

    public class GUIDTest
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class StrangeColumnNames
    {
        [Key]
        [Column("ItemId")]
        public int Id { get; set; }
        public string Word { get; set; }
        [Column("colstringstrangeword")]
        public string StrangeWord { get; set; }
        [Column("KeywordedProperty")]
        public string Select { get; set; }
        [Editable(false)]
        public string ExtraProperty { get; set; }
    }

    public class IgnoreColumns
    {
        [Key]
        public int Id { get; set; }
        [IgnoreInsert]
        public string IgnoreInsert { get; set; }
        [IgnoreUpdate]
        public string IgnoreUpdate { get; set; }
        [IgnoreSelect]
        public string IgnoreSelect { get; set; }
        [IgnoreInsert]
        [IgnoreUpdate]
        [IgnoreSelect]
        public string IgnoreAll { get; set; }
    }

    public class UserWithoutAutoIdentity
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    public class KeyMaster
    {
        [Key, Required]
        public int Key1 { get; set; }
        [Key, Required]
        public int Key2 { get; set; }
    }

    public class Test
    {
        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
