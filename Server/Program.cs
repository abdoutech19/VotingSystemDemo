using Akka.Actor;
using Akka.Configuration;
using NSec.Cryptography;
using Server.DataAccess;
using Server.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = remote
                    }
                    remote {
                        dot-netty.tcp {
                            port = 8081
                            hostname = localhost
                        }
                    }
                }");
            using var system = ActorSystem.Create("System", config);
            var sessionActor = system.ActorOf(SessionMasterManager.Props(sessionName: "TestSession", password: "root"), "SessionActor");

            /*using var key = Key.Create(
               SignatureAlgorithm.Ed25519,
               new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });

            string publicKey = Convert.ToBase64String(key.PublicKey.Export(KeyBlobFormat.NSecPublicKey));
            string username = "abdane";
            string password = "root";
            string zipCode = "01000";
            var passwordByteArray = HashAlgorithm.Sha256.Hash(Encoding.UTF8.GetBytes("abdou"));
            string password = Convert.ToBase64String(passwordByteArray);
        
            
            

            sessionActor.Tell(new RegistrationRequest(System.Guid.NewGuid().ToString("n"), publicKey, username, password, zipCode));
            await Task.Delay(20000);
            Console.WriteLine("20 seconds passed started authetication ...");
            //sessionActor.Tell(new AuthenticationRequest(System.Guid.NewGuid().ToString("n"), 18, publicKey, username, password));
            sessionActor.Tell(new AccountVerificationRequest("whatever", 24, publicKey, username, password));*/
            Console.ReadLine();
            
            

        }
    }
}
