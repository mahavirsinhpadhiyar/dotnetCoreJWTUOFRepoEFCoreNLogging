using System.ComponentModel;

public class LoginModel
{
    [DefaultValue("johndoe")]
    public string UserName { get; set; }
    [DefaultValue("def@123")]
    public string Password { get; set; }
}