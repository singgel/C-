using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CertificateMvcApplication.Models
{
    public class FileModel
    {
        #region 字段属性

        [Required]
        [DisplayName("id")]
        public int id { get; set; }

        [Required]
        [DisplayName("pId")]
        public int pId { get; set; }

        [Required]
        [DisplayName("name")]
        public string name { get; set; }

        #endregion

        public FileModel() { }

        protected FileModel(FileModel model)
        {
            this.id = model.id;
            this.pId = model.pId;
            this.name = model.name;
        }
    }
}