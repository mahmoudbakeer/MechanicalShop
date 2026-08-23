
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Customers;



public static class CustomerError
{
    public static Error CustomerIdRequired => Error.Validation("Customer_Id_Required.", "Id cannot be null or empty.");

    public static Error NameRequired =>
     Error.Validation("Customer_Name_Required.", "The Name cannot be null or empty.");

    public static Error EmailRequired =>
    Error.Validation("Customer_Email_Required.", "The Email cannot be null or empty.");
    public static Error PhoneNumberRequired =>
    Error.Validation("Customer_PhoneNumber_Required.", "The PhoneNumber cannot be null or empty.");
    public static Error PhoneNumberInvalid =>
    Error.Validation("Phone_Number_InValid..", "The PhoneNumber should start with + and be between 7 and 15 digit.");
    public static Error EmailInValid => Error.Validation("Customer_Email_InValid.", "The Email is InValid.");
    public static Error CustomerExist => Error.Validation("Customer_Exists.", "Customer with Email already exist.");
    public static Error CannotDeleteCustomer => Error.Validation("Customer_Has_WorkOrder.", "Cannot delete customer with active WorkOrder.");
}