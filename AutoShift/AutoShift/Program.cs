using System;

namespace AutoShift
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var shiftClient = new Shift();
            await shiftClient.LoginAsync("jondbrown@gmail.com", "Xenotrain1");
            await shiftClient.GetRedeemForm("12345-12345-12345-12345-12345");
        }
    }
}
