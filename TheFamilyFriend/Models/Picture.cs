using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.Models
{
    [Table("Picture")]
    public partial class Picture
    { [Key]
        public int PictureID { get; set; }
        public string PictureName { get; set; }
        public string PicturePath { get; set; }
        public int PictureTypeId { get; set; }
        public string UserName { get; set; }
        public Nullable<int> PersonId { get; set; }
        public Nullable<System.DateTime> UploadTime { get; set; }
    }
}