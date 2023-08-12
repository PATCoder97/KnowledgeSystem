using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using DevExpress.Utils.Svg;
using System;
using System.Drawing;

namespace KnowledgeSystem
{
    public partial class Function
    {
        public int Id { get; set; }
        public Nullable<int> IdParent { get; set; }
        public string DisplayName { get; set; }
        public string ControlName { get; set; }
        public Nullable<int> Prioritize { get; set; }
        public Nullable<bool> Status { get; set; }
        public string Images { get; set; }
        public SvgImage ImageLive
        {
            get
            {
                return string.IsNullOrEmpty(Images) ?
                    SvgImage.FromFile($@"Images\none.svg") :
                    SvgImage.FromFile($@"Images\{Images}");
            }
            set { }
        }
    }
}