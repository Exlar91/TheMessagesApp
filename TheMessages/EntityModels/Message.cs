using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMessages.EntityModels
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public AppUser Sender { get; set; }
        public AppUser Owner { get; set; }
        public string Text {  get; set; }
        public bool isInput { get; set; }
        public bool isReaded { get; set; }
        public DateTime Created { get; set; }
    }
}
