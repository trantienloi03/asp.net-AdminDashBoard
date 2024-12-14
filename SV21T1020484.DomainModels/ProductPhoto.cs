﻿namespace SV21T1020484.DomainModels
{
    public class ProductPhoto
    {
        public long PhotoID { get; set; }
        public int ProductID { get; set; }
        public string Photo { get; set; }
        public string Description { get; set; } = "";
        public int DisplayOrder {  get; set; }
        public bool IsHidden { get; set; }
    }
}
