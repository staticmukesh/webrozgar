using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebRozgar.Helpers
{
    public class MaxSize : ValidationAttribute
    {
        private readonly int _Size;
        public MaxSize(int size) : base("Maximum allowed file size is 2MB")
        {
            _Size = size;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                HttpPostedFileBase file = (HttpPostedFileBase)value;
                if (file.ContentLength > _Size)
                {
                    return new ValidationResult("Maximum allowed file size is 2MB");
                }
                else
                {
                    string extension = file.FileName.Substring(file.FileName.IndexOf("."), file.FileName.Length - file.FileName.IndexOf(".")).ToLower();
                    if ((extension != ".pdf") && (extension != ".docx") && (extension != ".doc"))
                    {
                        return new ValidationResult("Only pdf, docx and doc are allowed");
                    }
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Resume is required");
        }
    }
}