using System.ComponentModel.DataAnnotations;

namespace Document_Intelligence_Task.Domain.Models
{
    public class IDDocument_Passport
    {
        [Key]
        public Guid DocumentId { get; set; }
        public string? DocumentNumber { get; set; } // Passport number
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfUpload { get; set; } = DateTime.Now;
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfExpiration { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string? Sex { get; set; }
        public string? CountryRegion { get; set; }
        public string? DocumentType { get; set; }
        public string? Nationality { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? PlaceOfIssue { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? PersonalNumber { get; set; }
        public string? fileUrl { get; set; }
    }
}
