using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMModel
{
    public class Sale
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long Sheet { get; set; }

        public long Row { get; set; }

        public DateTime? Date_Received { get; set; }

        public string Patient_Type { get; set; }

        public string Patient_First { get; set; }

        public string Patient_Last { get; set; }

        public DateTime? Patient_DOB { get; set; }

        public string Patient_Address { get; set; }

        public string Patient_City { get; set; }

        public string Patient_State { get; set; }

        public string Patient_ZIP { get; set; }

        public string Status { get; set; }

        public DateTime? Date_Shipped { get; set; }

        public string Rx_Type { get; set; }

        public string Provider_Last { get; set; }

        public string Provider_First { get; set; }

        public string Provider_DEA { get; set; }

        public string Provider_LicNum { get; set; }

        public string Provider_Address { get; set; }

        public string Provider_City { get; set; }

        public string Provider_State { get; set; }

        public string Provider_Zip { get; set; }

        public string Patient_ID_Num { get; set; }

        public string RX_Num { get; set; }

        public string Drug_NDC_Num { get; set; }

        public string Drug_Name { get; set; }

        public decimal? Drug_Cost { get; set; }

        public decimal? Drug_Sale { get; set; }

        public decimal? Drug_Profit { get; set; }

        public double? Drug_Margin { get; set; }

        public string Drug_Type { get; set; }

        public decimal? Primary_Pay { get; set; }

        public string Primary_Name { get; set; }

        public string Primary_Type { get; set; }

        public decimal? Secondary_Pay { get; set; }

        public string Secondary_Type { get; set; }

        public string Secondary_Pay_Type { get; set; }

        public decimal? Funding_Paid { get; set; }

        public string Funding_Type { get; set; }

        public string Funding_Name { get; set; }

        public decimal? Total_Pay { get; set; }

        public decimal? Patient_Due { get; set; }

        public decimal? Patient_Pay { get; set; }

        public decimal? Balance { get; set; }

        public string Shipped_via { get; set; }

        public string Diagnosis_ICD_10 { get; set; }

        public string Diagnosis_Name { get; set; }

        public DateTime? Processed_On { get; set; }

        public string Specialty { get; set; }

        public string Rep { get; set; }

        public long? Refill_Num { get; set; }

        public long? Refills_Remain { get; set; }

        public long? Qty { get; set; }

        public DateTime? Need_by_date { get; set; }

        public bool? Urgent { get; set; }

        public DateTime? CreateDT { get; set; }

        public DateTime? UpdateDT { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }

        public bool Deleted { get; set; }

        [ForeignKey("Import")]
        public long ImportId { get; set; }

        public Import Import { get; set; }
    }
}

