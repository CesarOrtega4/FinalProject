namespace FinalProject;
using System.Data;
class GuiTier{
    User user = new User();
    DataTier database = new DataTier();

    // print login page
    public User Login(){
        Console.WriteLine("------Welcome to Package Management System------");
        Console.WriteLine("Please input username: ");
        user.userID = Console.ReadLine();
        Console.WriteLine("Please input password: ");
        user.userPassword = Console.ReadLine()!;
        return user;
    }
    // print Dashboard after user logs in successfully
    public int Dashboard(User user){
        DateTime localDate = DateTime.Now;
        Console.WriteLine("---------------Dashboard-------------------");
        Console.WriteLine("Please select an option to continue:");
        Console.WriteLine("1. Send email for unknown or unpicked package");
        Console.WriteLine("2. Show package records");
        Console.WriteLine("3. Logout");
        int option = Convert.ToInt16(Console.ReadLine());
        return option;
    }

     public void DisplayRecords(DataTable tableRecords){
        Console.WriteLine("---------------Record List-------------------");
        foreach(DataRow row in tableRecords.Rows){
           Console.WriteLine($"Package ID: {row["id"]}");
        }
    }
    public void DisplaySendEmail(DataTable tableSendEmail){
        Console.WriteLine("---------------Package List-------------------");
        foreach(DataRow row in tableSendEmail.Rows){
           Console.WriteLine($"Package ID: {row["package_id"]}, Agency: {row["agency"]} ");
        }
    }
}