using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clubs.Models;
using Quill.Delta;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Html;
using Ganss.XSS;

namespace Clubs.Models
{
    public class Trip
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid TripId { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Body { get; set; }

        public IList<TripUser> TripUsers { get; set; }

        public ApplicationUser Owner { get; set; }

        public bool Visible { get; set; }

        public decimal Price { get; set; }
        
        public IList<string> RequiredQualifications { get; set; }

        public HtmlString BodyRendered {
            get {
                var opsObj = JObject.Parse(Body ?? "{ops: []}");
                var htmlConverter = new HtmlConverter((JArray)opsObj["ops"]);
                var sanitizer = new HtmlSanitizer();
                return new HtmlString(sanitizer.Sanitize(htmlConverter.Convert()));
            }
        }
    }
    
    public class TripViewModel {
        public Guid TripId { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
        
        public decimal Price { get; set; }

        public HtmlString BodyRendered { get; set; }
        
        public bool IsOrganizer { get; set; }
        
        public bool IsVisible { get; set; }
        
        public IList<string> RequiredQualifications { get; set; }
        
    }

    public class TripUser
    {
        public Guid TripId { get; set; }
        public Trip Trip { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool Paid { get; set; }

        public bool Accepted { get; set; }
        
        public Role Role { get; set; }
    }

    public enum Role
    {
        Organizer = 1,
        Member = 2
    }
}
