using System.ComponentModel.DataAnnotations;

namespace DataAccess.Dto.Request
{
    public class UserCredReqDto
    {        
        public int empCode { get; set; }        
        public string password { get; set; }
    }
}
