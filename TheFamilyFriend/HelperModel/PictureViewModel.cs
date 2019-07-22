using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheFamilyFriend.HelperModel
{
    public class PictureViewModel
    {

        [Display(Name = "图片路径")]
        public string PicturePath { get; set; }

        [Display(Name = "图片分类id")]
        public int PictureTypeId { get; set; }
        [Display(Name = "图片名称")]
        public string PictureName { get; set; }

    }
}