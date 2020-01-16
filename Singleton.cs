using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MessagingCash
{
    class Singleton
    {
        IAzure azure;
        public Singleton()
        {
            var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal("clientId", "clientsecret","tenantId", AzureEnvironment.AzureGlobalCloud);
            azure = Azure.Configure().Authenticate(credentials).WithDefaultSubscription();
        }
        public async Task ListVms()
        {
            var vms = await azure.VirtualMachines.ListAsync();
            foreach (var vm in vms)
            {
                Console.WriteLine(vm.Name);
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public async Task ListVmsFilter(string name)
        {
            var allvms = await azure.VirtualMachines.ListAsync();
            IVirtualMachine targetvm = allvms
                .Where(vm => vm.Name == name)
                .SingleOrDefault();
            Console.WriteLine(targetvm?.Plan);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public async Task GetVmVn()
        {
            var allvms = await azure.VirtualMachines.ListAsync();
            IVirtualMachine targetvm = allvms
                .Where(vm => vm.Name == "Annvm")
                .SingleOrDefault();
            Console.WriteLine(targetvm?.Id);

            INetworkInterface targetnic = targetvm.GetPrimaryNetworkInterface();

            INicIPConfiguration targetipconfig = targetnic.PrimaryIPConfiguration;

            IPublicIPAddress targetipaddress = targetipconfig.GetPublicIPAddress();

            Console.WriteLine($"IP Address:\t{targetipaddress.IPAddress}");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public void CreateVm()
        {
            var rgName = "az203";
            Console.WriteLine("Creating public IP address...");
            var publicIPAddress = azure.PublicIPAddresses.Define("myPublicIP")
                .WithRegion(Region.USEast)
                .WithExistingResourceGroup(rgName)
                .WithDynamicIP()
                .Create();

            Console.WriteLine("Creating Network...");
            var network = azure.Networks.Define("myVNet")
                .WithRegion(Region.USEast)
                .WithExistingResourceGroup(rgName)
                .WithAddressSpace("10.5.0.0/16")
                .WithSubnet("mySubnet", "10.5.0.0/24")
                .Create();

            Console.WriteLine("Creating NIC...");
            var networkInterface = azure.NetworkInterfaces.Define("myNIC")
                .WithRegion(Region.USEast)
                .WithExistingResourceGroup(rgName)
                .WithExistingPrimaryNetwork(network)
                .WithSubnet("mySubnet")
                .WithPrimaryPrivateIPAddressDynamic()
                .WithExistingPrimaryPublicIPAddress(publicIPAddress)
                .Create();

            Console.WriteLine("Creating VM...");
            azure.VirtualMachines.Define("myTestvm3")
                            .WithRegion(Region.USEast)
                            .WithExistingResourceGroup(rgName)
                            .WithExistingPrimaryNetworkInterface(networkInterface)
                            .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer", "2012-R2-Datacenter").
                            WithAdminUsername("student")
                            .WithAdminPassword("AzurePa55w0rd")
                            .WithComputerName("myTestvm3")
                            .WithSize(VirtualMachineSizeTypes.StandardDS1)
                            .Create();
        }
    }
}