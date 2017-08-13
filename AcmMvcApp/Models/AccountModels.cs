using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;
using System.Web;


namespace AcmMvcApp.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ContentDisp> Contents { get; set; }
        public DbSet<Display> Displays { get; set; }
        public DbSet<Package> Packages { get; set; }

        public DbSet<u_package_u_client> Pkg_client_links { get; set; }
        public DbSet<u_package_u_content> Pkg_cont_links { get; set; }
        public DbSet<u_package_u_display> Pkg_disp_links { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; }
    }

    public class UserProfileViewModel
    {

        public UserProfile userProfile { get; set; }
        public List<UserProfile> userProfiles { get; set; }
        public string toSetUser { get; set; }
        public string toSetRole { get; set; }

        public string getUserProfileRoles(string userName)
        {
            string rolesList = "";

            foreach (string s in Roles.GetRolesForUser(userName)) 
            {
                rolesList += s + ';';
            }

            return rolesList;
        }

    }

    public class UserProfileEditModel
    {

        public UserProfile userProfile { get; set; }

        [Display(Name = "Роль")]
        public string selectedRole { get; set; }

        [StringLength(100, ErrorMessage = "Значение \"{0}\" должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        public string[] getAllRoles ()
        {
            return Roles.GetAllRoles();
        }

        [Display(Name = "Активен")]
        public bool isActive { get; set; }
    }

    [Table("Package")]
    public class Package
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Код пакета")]
        public string urlName { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Активен")]
        public bool isActive { get; set; }

        public virtual ICollection<u_package_u_client> Clients { get; set; }
        public virtual ICollection<u_package_u_content> Contents { get; set; }
        public virtual ICollection<u_package_u_display> Displays { get; set; }

    }

    public class PackageViewModel
    {

        public Package package { get; set; }
        public List<Client> _clients { get; set; }
        public List<ContentDisp> _contents { get; set; }
        public List<Display> _displays { get; set; }

        // Display Attribute will appear in the Html.LabelFor
        [Display(Name = "Клиент")]
        public int SelectedClientId { get; set; }
        public IEnumerable<SelectListItem> ClientItems 
        {
            get { return new SelectList(_clients, "Id", "Name"); }
        }

        // Display Attribute will appear in the Html.LabelFor
        [Display(Name = "Контент")]
        public int SelectedContentId { get; set; }
        public IEnumerable<SelectListItem> ContentItems
        {
            get { return new SelectList(_contents, "Id", "Name"); }
        }

        // Display Attribute will appear in the Html.LabelFor
        [Display(Name = "Дисплей")]
        public int SelectedDisplayId { get; set; }
        public IEnumerable<SelectListItem> DisplayItems
        {
            get { return new SelectList(_displays, "Id", "Name"); }
        }
    }

    public class ContentViewModel
    {

        public ContentDisp content { get; set; }
        public HttpPostedFileBase file { get; set; } 

    }

    [Table("Client")]
    public class Client
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование клиента")]
        public string name { get; set; }

        public virtual ICollection<u_package_u_client> Packages { get; set; }
    }

    [Table("Content")]
    public class ContentDisp
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название файла")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Расширение")]
        public string extension { get; set; }

        [Required]
        [Display(Name = "Продолжительность (сек)")]
        public int duration { get; set; }

        public virtual ICollection<u_package_u_content> Packages { get; set; }
    }

    [Table("Display")]
    public class Display
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название дисплея")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Адрес")]
        public string address { get; set; }

        public virtual ICollection<u_package_u_display> Packages { get; set; }
    }

    [Table("u_package_u_client")]
    public class u_package_u_client
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int PackageId { get; set; }

        public virtual Client Client { get; set; }
        public virtual Package Package { get; set; }
    }

    [Table("u_package_u_content")]
    public class u_package_u_content
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ContentId { get; set; }
        public int PackageId { get; set; }

        public virtual ContentDisp Content { get; set; }
        public virtual Package Package { get; set; }
    }

    [Table("u_package_u_display")]
    public class u_package_u_display
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int DisplayId { get; set; }
        public int PackageId { get; set; }

        public virtual Display Display { get; set; }
        public virtual Package Package { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение \"{0}\" должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение \"{0}\" должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Роль")]
        public string selectedRole { get; set; }
        public string[] getAllRoles()
        {
            return Roles.GetAllRoles();
        }

        [Display(Name = "Активен")]
        public bool isActive { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
